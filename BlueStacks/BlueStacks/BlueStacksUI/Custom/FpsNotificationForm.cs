using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Custom
{
	// Token: 0x020002D5 RID: 725
	public partial class FpsNotificationForm : Form
	{
		// Token: 0x06001ACC RID: 6860
		[DllImport("Gdi32.dll")]
		private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06001ACD RID: 6861 RVA: 0x00011DAE File Offset: 0x0000FFAE
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 134217728;
				createParams.ExStyle |= 8;
				return createParams;
			}
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x0009E200 File Offset: 0x0009C400
		public FpsNotificationForm(string message)
		{
			Logger.Info("Creating FpsNotificationForm with message: {0}", new object[] { message });
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.message = message;
			base.FormBorderStyle = FormBorderStyle.None;
			base.StartPosition = FormStartPosition.Manual;
			base.Size = new Size(200, 80);
			base.Opacity = 0.0;
			base.ShowInTaskbar = false;
			base.TopMost = true;
			this.DoubleBuffered = true;
			base.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - base.Width - 20, Screen.PrimaryScreen.WorkingArea.Bottom - base.Height - 20);
			this.cachedPath = this.GetRoundedRectangle(base.ClientRectangle, 10);
			base.Region = Region.FromHrgn(FpsNotificationForm.CreateRoundRectRgn(0, 0, base.Width, base.Height, 10, 10));
			Panel panel = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(10),
				BackColor = Color.Transparent
			};
			Label label = new Label
			{
				Text = "FPS: " + message,
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 10f, FontStyle.Regular),
				AutoSize = true,
				Location = new Point(10, 10),
				BackColor = Color.Transparent
			};
			Label label2 = new Label
			{
				Text = "@de_mistiyt",
				ForeColor = Color.White,
				Font = new Font("Segoe UI", 8f, FontStyle.Regular),
				AutoSize = true,
				Location = new Point(10, 30),
				BackColor = Color.Transparent
			};
			panel.Controls.Add(label);
			panel.Controls.Add(label2);
			base.Controls.Add(panel);
			base.MouseClick += this.OpenTelegram;
			panel.MouseClick += this.OpenTelegram;
			label.MouseClick += this.OpenTelegram;
			label2.MouseClick += this.OpenTelegram;
			this.fadeTimer = new Timer
			{
				Interval = 16
			};
			this.fadeTimer.Tick += this.FadeTimer_Tick;
			this.fadeTimer.Start();
			this.gradientTimer = new Timer
			{
				Interval = 50
			};
			this.gradientTimer.Tick += this.GradientTimer_Tick;
			this.gradientTimer.Start();
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06001ACF RID: 6863 RVA: 0x00005AAF File Offset: 0x00003CAF
		protected override bool ShowWithoutActivation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x0009E4A0 File Offset: 0x0009C6A0
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Logger.Info("FpsNotificationForm OnPaint called");
			using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(base.Width, base.Height), Color.FromArgb(80, 80, 80), Color.FromArgb(160, 160, 160)))
			{
				linearGradientBrush.TranslateTransform(this.gradientOffset * (float)base.Width, this.gradientOffset * (float)base.Height);
				e.Graphics.FillPath(linearGradientBrush, this.cachedPath);
			}
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x0009E54C File Offset: 0x0009C74C
		private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect.X, rect.Y, radius, radius, 180f, 90f);
			graphicsPath.AddArc(rect.Right - radius, rect.Y, radius, radius, 270f, 90f);
			graphicsPath.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0f, 90f);
			graphicsPath.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90f, 90f);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x00011DD6 File Offset: 0x0000FFD6
		private void GradientTimer_Tick(object sender, EventArgs e)
		{
			this.gradientOffset += 0.02f;
			if (this.gradientOffset > 1f)
			{
				this.gradientOffset -= 1f;
			}
			base.Invalidate();
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x0009E5EC File Offset: 0x0009C7EC
		private void FadeTimer_Tick(object sender, EventArgs e)
		{
			if (!this.isClosing)
			{
				if (this.currentOpacity < 0.8f)
				{
					this.currentOpacity += 0.05f;
					if (this.currentOpacity > 0.8f)
					{
						this.currentOpacity = 0.8f;
					}
					base.Opacity = (double)this.currentOpacity;
					return;
				}
				this.fadeTimer.Stop();
				Timer closeTimer = new Timer
				{
					Interval = 2000
				};
				closeTimer.Tick += delegate(object s, EventArgs ev)
				{
					this.isClosing = true;
					this.fadeTimer.Start();
					closeTimer.Stop();
					closeTimer.Dispose();
				};
				closeTimer.Start();
				return;
			}
			else
			{
				if (this.currentOpacity > 0f)
				{
					this.currentOpacity -= 0.05f;
					if (this.currentOpacity < 0f)
					{
						this.currentOpacity = 0f;
					}
					base.Opacity = (double)this.currentOpacity;
					return;
				}
				this.fadeTimer.Stop();
				this.gradientTimer.Stop();
				Logger.Info("FpsNotificationForm closing");
				base.Close();
				return;
			}
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x0009E708 File Offset: 0x0009C908
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			Logger.Info("FpsNotificationForm OnFormClosing called");
			if (this.fadeTimer != null)
			{
				this.fadeTimer.Stop();
				this.fadeTimer.Dispose();
			}
			if (this.gradientTimer != null)
			{
				this.gradientTimer.Stop();
				this.gradientTimer.Dispose();
			}
			GraphicsPath graphicsPath = this.cachedPath;
			if (graphicsPath == null)
			{
				return;
			}
			graphicsPath.Dispose();
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x00011E0F File Offset: 0x0001000F
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Logger.Info("FpsNotificationForm OnLoad called");
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x0009E774 File Offset: 0x0009C974
		private void OpenTelegram(object sender, MouseEventArgs e)
		{
			Logger.Info("FpsNotificationForm clicked, opening Telegram");
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = "https://t.me/de_mistiyt",
					UseShellExecute = true
				});
				Logger.Info("Telegram link opened successfully");
			}
			catch (Exception ex)
			{
				Logger.Error("Error opening Telegram: " + ex.Message);
				File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR opening Telegram: {1}\n{2}\n\n", DateTime.Now, ex.Message, ex.StackTrace));
			}
		}

		// Token: 0x040010C8 RID: 4296
		private Timer fadeTimer;

		// Token: 0x040010C9 RID: 4297
		private Timer gradientTimer;

		// Token: 0x040010CA RID: 4298
		private const float FADE_SPEED = 0.05f;

		// Token: 0x040010CB RID: 4299
		private string message;

		// Token: 0x040010CC RID: 4300
		private float currentOpacity;

		// Token: 0x040010CD RID: 4301
		private const int BORDER_RADIUS = 10;

		// Token: 0x040010CE RID: 4302
		private const int SCREEN_OFFSET = 20;

		// Token: 0x040010CF RID: 4303
		private bool isClosing;

		// Token: 0x040010D0 RID: 4304
		private float gradientOffset;

		// Token: 0x040010D1 RID: 4305
		private readonly GraphicsPath cachedPath;
	}
}
