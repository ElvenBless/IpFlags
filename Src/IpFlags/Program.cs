using System.Net.Http.Json;

namespace IpFlags;

static class Program
{
    private const string IpApiUrl = "http://ip-api.com/json/?fields=countryCode";
    private const int PollSeconds = 30;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        var flagService = new FlagImageService();
        using var ni = new NotifyIcon { Visible = true, Text = "IP → Flag" };
        void OnIconClick(object? _, EventArgs __) => UpdateIcon();
        void BindIconClick()
        {
            ni.Click -= OnIconClick;
            ni.DoubleClick -= OnIconClick;
            if (AppConfig.UpdateOnDoubleClick)
                ni.DoubleClick += OnIconClick;
            else
                ni.Click += OnIconClick;
        }
        BindIconClick();

        using var timer = new System.Windows.Forms.Timer { Interval = PollSeconds * 1000 };
        timer.Tick += (_, _) => UpdateIcon();
        timer.Enabled = AppConfig.PollingEnabled;

        using var ctx = new ContextMenuStrip();
        var pollingItem = new ToolStripMenuItem(GetPollingMenuText());
        pollingItem.Click += (_, _) =>
        {
            AppConfig.PollingEnabled = !AppConfig.PollingEnabled;
            timer.Enabled = AppConfig.PollingEnabled;
            pollingItem.Text = GetPollingMenuText();
        };
        ctx.Items.Add(pollingItem);
        var clickModeItem = new ToolStripMenuItem(GetClickModeMenuText());
        clickModeItem.Click += (_, _) =>
        {
            AppConfig.UpdateOnDoubleClick = !AppConfig.UpdateOnDoubleClick;
            BindIconClick();
            clickModeItem.Text = GetClickModeMenuText();
        };
        ctx.Items.Add(clickModeItem);
        var autostartItem = new ToolStripMenuItem(GetAutostartMenuText());
        autostartItem.Click += (_, _) =>
        {
            Autostart.Toggle();
            autostartItem.Text = GetAutostartMenuText();
        };
        ctx.Items.Add(autostartItem);
        ctx.Items.Add("Update", null, (_, _) => UpdateIcon());
        ctx.Items.Add("Exit", null, (_, _) => Application.Exit());
        ctx.Opening += (_, _) => autostartItem.Text = GetAutostartMenuText();
        ni.ContextMenuStrip = ctx;

        static string GetPollingMenuText() => AppConfig.PollingEnabled ? "Polling: On" : "Polling: Off";
        static string GetClickModeMenuText() => AppConfig.UpdateOnDoubleClick ? "Update on: Double-click" : "Update on: Click";
        static string GetAutostartMenuText() => Autostart.IsEnabled ? "Autostart: On" : "Autostart: Off";

        async void UpdateIcon()
        {
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var json = await http.GetFromJsonAsync<IpResponse>(IpApiUrl);
                var code = json?.countryCode;
                if (string.IsNullOrEmpty(code)) return;
                var path = await flagService.GetFlagImagePathAsync(code);
                if (path == null) return;
                using var bmp = new Bitmap(path);
                using var iconHandle = Icon.FromHandle(bmp.GetHicon());
                var icon = new Icon(iconHandle, iconHandle.Size);
                ni.Icon?.Dispose();
                ni.Icon = icon;
                ni.Text = code.ToUpperInvariant();
            }
            catch { /* ignore */ }
        }

        UpdateIcon();
        Application.Run();
    }

    private class IpResponse
    {
        public string? countryCode { get; set; }
    }
}