using Microsoft.Data.Sqlite;

namespace Tyt.Infrastructure.Persistence;

public sealed class TytDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public TytDatabase(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        };
        _connection = new SqliteConnection(builder.ToString());
        _connection.Open();
        using (var pragma = _connection.CreateCommand())
        {
            pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA foreign_keys=ON;";
            pragma.ExecuteNonQuery();
        }

        EnsureSchema();
        CloseOrphanedSegments();
    }

    public SqliteConnection Connection => _connection;

    public void Dispose() => _connection.Dispose();

    private void EnsureSchema()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText =
            """
            CREATE TABLE IF NOT EXISTS apps (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                key TEXT NOT NULL UNIQUE,
                name TEXT NOT NULL,
                exe_path TEXT
            );

            CREATE TABLE IF NOT EXISTS segments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                app_id INTEGER NULL REFERENCES apps(id),
                kind TEXT NOT NULL,
                started_at TEXT NOT NULL,
                ended_at TEXT NULL,
                is_open INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS settings (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS ix_segments_started ON segments(started_at);
            """;
        cmd.ExecuteNonQuery();
    }

    private void CloseOrphanedSegments()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText =
            """
            UPDATE segments
            SET is_open = 0,
                ended_at = COALESCE(ended_at, started_at)
            WHERE is_open = 1;
            """;
        cmd.ExecuteNonQuery();
    }
}
