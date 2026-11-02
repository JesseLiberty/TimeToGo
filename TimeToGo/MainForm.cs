using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TimeToGo;

/// <summary>
/// Borderless main window with a custom title bar so the title bar can host a live hyperlink.
/// </summary>
public partial class MainForm : Form
{
    private const string Title = "TimeToGo \u00A9 Copyright Jesse Liberty 2026";
    private const string LicenseText = "MIT OpenSource License (https://github.com/jesseliberty/TimeToGo)";
    private const string LicenseUrl = "https://github.com/jesseliberty/TimeToGo";

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    [LibraryImport("user32.dll")]
    private static partial int SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ReleaseCapture();

    private readonly TextBox arrivedBox;
    private readonly TextBox soFarBox;
    private readonly Label departureLabel;

    public MainForm()
    {
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        Text = Title;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(440, 420);
        BackColor = Color.White;
        ForeColor = Color.Black;
        Font = new Font("Segoe UI", 10F);
        KeyPreview = true;
        Padding = new Padding(1);

        // ----- Custom title bar -----
        var titleBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 54,
            BackColor = Color.White,
        };

        var titleLabel = new Label
        {
            Text = Title,
            AutoSize = true,
            Location = new Point(8, 6),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };

        var licenseLink = new LinkLabel
        {
            Text = LicenseText,
            AutoSize = true,
            Location = new Point(8, 29),
            Font = new Font("Segoe UI", 8.5F),
            LinkColor = Color.Black,
            ActiveLinkColor = Color.Black,
            VisitedLinkColor = Color.Black,
            LinkBehavior = LinkBehavior.AlwaysUnderline,
            BackColor = Color.White,
            TabStop = false,
        };
        licenseLink.LinkClicked += (_, _) =>
            Process.Start(new ProcessStartInfo(LicenseUrl) { UseShellExecute = true });

        var closeButton = CreateTitleButton("\u2715", "Close");
        closeButton.Click += (_, _) => Close();
        var minimizeButton = CreateTitleButton("\u2014", "Minimize");
        minimizeButton.Click += (_, _) => WindowState = FormWindowState.Minimized;

        var titleButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            BackColor = Color.White,
            Margin = Padding.Empty,
        };
        titleButtons.Controls.Add(closeButton);
        titleButtons.Controls.Add(minimizeButton);

        var separator = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.Black };

        titleBar.Controls.Add(titleLabel);
        titleBar.Controls.Add(licenseLink);
        titleBar.Controls.Add(titleButtons);
        titleBar.Controls.Add(separator);

        titleBar.MouseDown += DragWindow;
        titleLabel.MouseDown += DragWindow;

        // ----- Body (matches the TimeToGo sketch) -----
        var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

        var heading = new Label
        {
            Text = "Time To Go",
            AutoSize = true,
            Location = new Point(24, 20),
            Font = new Font("Segoe UI", 20F),
        };

        var arrivedLabel = new Label
        {
            Text = "Arrived",
            AutoSize = true,
            Location = new Point(24, 92),
            Font = new Font("Segoe UI", 13F),
        };
        arrivedBox = new TextBox
        {
            Location = new Point(130, 88),
            Width = 160,
            Font = new Font("Segoe UI", 13F),
            PlaceholderText = "8:30 am",
            BackColor = Color.White,
            ForeColor = Color.Black,
            TabIndex = 0,
        };

        var soFarLabel = new Label
        {
            Text = "So Far",
            AutoSize = true,
            Location = new Point(24, 146),
            Font = new Font("Segoe UI", 13F),
        };
        soFarBox = new TextBox
        {
            Location = new Point(130, 142),
            Width = 160,
            Font = new Font("Segoe UI", 13F),
            PlaceholderText = "12.5",
            BackColor = Color.White,
            ForeColor = Color.Black,
            TabIndex = 1,
        };

        var timeToGoLabel = new Label
        {
            Text = "Time to Go",
            AutoSize = true,
            Location = new Point(24, 212),
            Font = new Font("Segoe UI", 13F),
        };
        departureLabel = new Label
        {
            Text = string.Empty,
            AutoSize = true,
            Location = new Point(70, 250),
            Font = new Font("Segoe UI", 26F, FontStyle.Bold),
            ForeColor = Color.Red,
            BackColor = Color.White,
        };

        body.Controls.AddRange([heading, arrivedLabel, arrivedBox, soFarLabel, soFarBox, timeToGoLabel, departureLabel]);

        Controls.Add(body);
        Controls.Add(titleBar);

        KeyDown += OnFormKeyDown;
        Shown += (_, _) => arrivedBox.Focus();
    }

    private static Button CreateTitleButton(string glyph, string accessibleName)
    {
        var button = new Button
        {
            Text = glyph,
            AccessibleName = accessibleName,
            Size = new Size(40, 30),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.Black,
            Font = new Font("Segoe UI", 10F),
            Margin = Padding.Empty,
            TabStop = false,
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.Gainsboro;
        return button;
    }

    private void DragWindow(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;
        ReleaseCapture();
        SendMessageW(Handle, WM_NCLBUTTONDOWN, HTCAPTION, IntPtr.Zero);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Thin black frame around the borderless window.
        e.Graphics.DrawRectangle(Pens.Black, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            const int WS_MINIMIZEBOX = 0x20000;
            const int CS_DROPSHADOW = 0x20000;
            var cp = base.CreateParams;
            cp.Style |= WS_MINIMIZEBOX; // allows minimize/restore from the taskbar
            cp.ClassStyle |= CS_DROPSHADOW;
            return cp;
        }
    }

    private void OnFormKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
            return;

        e.Handled = true;
        e.SuppressKeyPress = true; // prevents the error "ding"
        CalculateDeparture();
    }

    private void CalculateDeparture()
    {
        if (!TimeCalculator.TryParseArrival(arrivedBox.Text, out var arrival))
        {
            departureLabel.Text = string.Empty;
            ErrorDialog.Show(this,
                $"\"{arrivedBox.Text}\" is not a valid arrival time.\n\nPlease enter a time such as 8:30 am.");
            arrivedBox.Focus();
            arrivedBox.SelectAll();
            return;
        }

        if (!TimeCalculator.TryParseHoursWorked(soFarBox.Text, out var hoursWorked))
        {
            departureLabel.Text = string.Empty;
            ErrorDialog.Show(this,
                $"\"{soFarBox.Text}\" is not a valid number of hours worked.\n\nPlease enter a decimal number between 0 and 40, such as 12.5.");
            soFarBox.Focus();
            soFarBox.SelectAll();
            return;
        }

        var (timeOfDay, daysLater) = TimeCalculator.ComputeDeparture(arrival, hoursWorked);
        string text = TimeCalculator.FormatTwelveHour(timeOfDay);
        if (daysLater > 0)
            text += daysLater == 1 ? " (+1 day)" : $" (+{daysLater} days)";
        departureLabel.Text = text;
    }
}
