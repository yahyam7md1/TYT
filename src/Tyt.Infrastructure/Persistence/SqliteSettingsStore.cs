using Tyt.Core.Abstractions;
using Tyt.Core.Models;
using Tyt.Infrastructure.Tracking;

namespace Tyt.Infrastructure.Persistence;

public sealed class SqliteSettingsStore(TytDatabase db) : ISettingsStore
{
    public event EventHandler? Changed;

    public TrackingSettings Get()
    {
        var settings = new TrackingSettings();
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText = "SELECT key, value FROM settings;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var key = (string)reader["key"];
            var value = (string)reader["value"];
            switch (key)
            {
                case "idle_minutes":
                    if (int.TryParse(value, out var minutes))
                    {
                        settings.IdleThreshold = TimeSpan.FromMinutes(Math.Clamp(minutes, 1, 60));
                    }

                    break;
                case "launch_at_startup":
                    settings.LaunchAtStartup = value == "1";
                    break;
            }
        }

        return settings;
    }

    public bool IsPaused()
    {
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText = "SELECT value FROM settings WHERE key = 'is_paused';";
        var value = cmd.ExecuteScalar() as string;
        return value == "1";
    }

    public void SetPaused(bool paused)
    {
        Upsert("is_paused", paused ? "1" : "0");
    }

    public void Save(TrackingSettings settings)
    {
        var minutes = (int)Math.Round(settings.IdleThreshold.TotalMinutes);
        Upsert("idle_minutes", Math.Clamp(minutes, 1, 60).ToString());
        Upsert("launch_at_startup", settings.LaunchAtStartup ? "1" : "0");
        Autostart.Apply(settings.LaunchAtStartup);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void Upsert(string key, string value)
    {
        using var cmd = db.Connection.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO settings (key, value) VALUES ($k, $v)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value;
            """;
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$v", value);
        cmd.ExecuteNonQuery();
    }
}
