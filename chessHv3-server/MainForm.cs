using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class MainForm : Form
{
    public TrackBar eloSlider;
    public TrackBar depthSlider;
    public TrackBar multipvSlider;
    public Label eloLabel;
    public Label depthLabel;
    public Label multipvLabel;
    public Button applyButton;

    public MainForm()
    {
        // Form settings
        this.Text = "ChessHv3 External Engine";
        this.Width = 420;
        this.Height = 300;
        this.BackColor = Color.FromArgb(35, 36, 40);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        // Labels
        eloLabel = CreateLabel($"Elo: 3500", 20, 20);
        depthLabel = CreateLabel($"Depth: 10", 20, 90);
        multipvLabel = CreateLabel($"MultiPV: 5", 20, 160);

        // Sliders
        eloSlider = CreateSlider(20, 50, 500, 3500, 3500);
        depthSlider = CreateSlider(20, 120, 1, 20, 10);
        multipvSlider = CreateSlider(20, 190, 2, 5, 5);

        eloSlider.Scroll += (s, e) => eloLabel.Text = $"Elo: {eloSlider.Value}";
        depthSlider.Scroll += (s, e) => depthLabel.Text = $"Depth: {depthSlider.Value}";
        multipvSlider.Scroll += (s, e) => multipvLabel.Text = $"MultiPV: {multipvSlider.Value}";

        // Apply button
        applyButton = new Button()
        {
            Text = "Apply",
            Width = 120,
            Height = 40,
            Top = 230,
            Left = 140,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            BackColor = Color.FromArgb(88, 101, 242),
            ForeColor = Color.White
        };
        applyButton.FlatAppearance.BorderSize = 0;
        applyButton.MouseEnter += (s, e) => applyButton.BackColor = Color.FromArgb(114, 137, 218);
        applyButton.MouseLeave += (s, e) => applyButton.BackColor = Color.FromArgb(88, 101, 242);

        // Add controls
        this.Controls.Add(eloLabel);
        this.Controls.Add(depthLabel);
        this.Controls.Add(multipvLabel);
        this.Controls.Add(eloSlider);
        this.Controls.Add(depthSlider);
        this.Controls.Add(multipvSlider);
        this.Controls.Add(applyButton);
    }

    private Label CreateLabel(string text, int left, int top)
    {
        return new Label()
        {
            Text = text,
            ForeColor = Color.FromArgb(114, 137, 218),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Top = top,
            Left = left,
            Width = 200
        };
    }

    private TrackBar CreateSlider(int left, int top, int min, int max, int value)
    {
        var tb = new TrackBar()
        {
            Minimum = min,
            Maximum = max,
            Value = value,
            Width = 350,
            Height = 45,
            Top = top,
            Left = left,
            TickStyle = TickStyle.None,
            BackColor = Color.FromArgb(35, 36, 40)
        };

        tb.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Track gradient
            Rectangle track = new Rectangle(0, tb.Height / 2 - 3, tb.Width, 6);
            using (var gradient = new LinearGradientBrush(track, Color.FromArgb(88, 101, 242), Color.FromArgb(114, 137, 218), LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(gradient, track);

            // Thumb
            int thumbX = (int)((float)(tb.Value - tb.Minimum) / (tb.Maximum - tb.Minimum) * (tb.Width - 15));
            Rectangle thumb = new Rectangle(thumbX, tb.Height / 2 - 9, 18, 18);
            using (Brush b = new SolidBrush(Color.White))
                e.Graphics.FillEllipse(b, thumb);
            using (Pen p = new Pen(Color.FromArgb(88, 101, 242), 2))
                e.Graphics.DrawEllipse(p, thumb);
        };

        return tb;
    }
}
