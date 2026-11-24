using Tyt.Core.Models;

namespace Tyt.Infrastructure.Persistence;

public sealed class SegmentStore(TytDatabase db)
{
    private long? _openId;

    public long InsertOpen(Segment segment)
    {
        var appId = segment.App is null ? (long?)null : UpsertApp(segment.App);
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO segments (app_id, kind, started_at, ended_at, is_open)
            VALUES ($app, $kind, $start, $end, 1);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$app", (object?)appId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$kind", segment.Kind.ToString());
        cmd.Parameters.AddWithValue("$start", segment.StartedAt.ToString("O"));
        cmd.Parameters.AddWithValue("$end", DBNull.Value);
        _openId = (long)(cmd.ExecuteScalar() ?? 0L);
        return _openId.Value;
    }

    public void CheckpointOpen(DateTime now)
    {
        if (_openId is null)
        {
            return;
        }

        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText = "UPDATE segments SET ended_at = $end WHERE id = $id AND is_open = 1;";
        cmd.Parameters.AddWithValue("$end", now.ToString("O"));
        cmd.Parameters.AddWithValue("$id", _openId.Value);
        cmd.ExecuteNonQuery();
    }

    public void CloseOpen(Segment closed)
    {
        if (_openId is null)
        {
            return;
        }

        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText =
            """
            UPDATE segments
            SET ended_at = $end, is_open = 0
            WHERE id = $id;
            """;
        cmd.Parameters.AddWithValue("$end", (closed.EndedAt ?? DateTime.Now).ToString("O"));
        cmd.Parameters.AddWithValue("$id", _openId.Value);
        cmd.ExecuteNonQuery();
        _openId = null;
    }

    public void ForgetOpen() => _openId = null;

    public IReadOnlyList<Segment> GetOverlapping(DateTime rangeStart, DateTime rangeEnd, DateTime now)
    {
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText =
            """
            SELECT s.kind, s.started_at, s.ended_at, s.is_open,
                   a.key, a.name, a.exe_path
            FROM segments s
            LEFT JOIN apps a ON a.id = s.app_id
            WHERE s.started_at < $end
              AND (s.is_open = 1 OR IFNULL(s.ended_at, s.started_at) > $start)
            ORDER BY s.started_at;
            """;
        cmd.Parameters.AddWithValue("$start", rangeStart.ToString("O"));
        cmd.Parameters.AddWithValue("$end", rangeEnd.ToString("O"));

        var list = new List<Segment>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var started = DateTime.Parse((string)reader["started_at"]);
            var isOpen = Convert.ToInt32(reader["is_open"]) == 1;
            DateTime? ended = reader["ended_at"] is DBNull ? null : DateTime.Parse((string)reader["ended_at"]);
            AppInfo? app = null;
            if (reader["key"] is string key)
            {
                app = new AppInfo
                {
                    Id = key,
                    Name = reader["name"] as string ?? key,
                    ExePath = reader["exe_path"] as string
                };
            }

            list.Add(new Segment
            {
                App = app,
                Kind = Enum.Parse<SegmentKind>((string)reader["kind"]),
                StartedAt = started,
                EndedAt = isOpen ? null : ended
            });
        }

        return list.Where(segment => Overlaps(segment, rangeStart, rangeEnd, now)).ToArray();
    }

    public void DeleteAll()
    {
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText = "DELETE FROM segments; DELETE FROM apps;";
        cmd.ExecuteNonQuery();
        _openId = null;
    }

    private long UpsertApp(AppInfo app)
    {
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO apps (key, name, exe_path)
            VALUES ($key, $name, $exe)
            ON CONFLICT(key) DO UPDATE SET name = excluded.name, exe_path = excluded.exe_path;
            SELECT id FROM apps WHERE key = $key;
            """;
        cmd.Parameters.AddWithValue("$key", app.Id);
        cmd.Parameters.AddWithValue("$name", app.Name);
        cmd.Parameters.AddWithValue("$exe", (object?)app.ExePath ?? DBNull.Value);
        return (long)(cmd.ExecuteScalar() ?? 0L);
    }

    private static bool Overlaps(Segment segment, DateTime rangeStart, DateTime rangeEnd, DateTime now)
    {
        var from = segment.StartedAt;
        var to = segment.EndedAt ?? now;
        return from < rangeEnd && to > rangeStart;
    }
}
