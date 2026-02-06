using Microsoft.Win32;

namespace IpFlags;

static class Autostart
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "IpFlags";

    public static bool IsEnabled
    {
        get
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
                var path = key?.GetValue(ValueName) as string;
                return string.Equals(path, Application.ExecutablePath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        set
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                if (key == null) return;
                if (value)
                    key.SetValue(ValueName, Application.ExecutablePath);
                else
                    key.DeleteValue(ValueName, throwOnMissingValue: false);
            }
            catch { /* ignore */ }
        }
    }

    public static void Toggle()
    {
        IsEnabled = !IsEnabled;
    }
}
