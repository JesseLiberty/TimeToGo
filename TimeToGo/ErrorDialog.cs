namespace TimeToGo;

/// <summary>Simple black-on-white message dialog with a Close button.</summary>
internal sealed class ErrorDialog : Form
{
    private ErrorDialog(string message)
    {
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        Text = "TimeToGo - Invalid Entry";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        ForeColor = Color.Black;
        Font = new Font("Segoe UI", 10F);
        ClientSize = new Size(360, 150);

        var messageLabel = new Label
        {
            Text = message,
            Location = new Point(16, 16),
            Size = new Size(328, 80),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };

        var closeButton = new Button
        {
            Text = "Close",
            DialogResult = DialogResult.OK,
            Size = new Size(90, 30),
            Location = new Point(254, 106),
            BackColor = Color.White,
            ForeColor = Color.Black,
        };

        Controls.Add(messageLabel);
        Controls.Add(closeButton);
        AcceptButton = closeButton;
        CancelButton = closeButton;
    }

    public static void Show(IWin32Window owner, string message)
    {
        using var dialog = new ErrorDialog(message);
        dialog.ShowDialog(owner);
    }
}
