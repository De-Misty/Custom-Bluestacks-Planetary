using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000105 RID: 261
	public class OtsFeedbackControl : UserControl, IComponentConnector
	{
		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x00008D93 File Offset: 0x00006F93
		// (set) Token: 0x06000AE1 RID: 2785 RVA: 0x00008D9B File Offset: 0x00006F9B
		public MainWindow ParentWindow { get; set; }

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00008DA4 File Offset: 0x00006FA4
		public OtsFeedbackControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00008DB9 File Offset: 0x00006FB9
		private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0003DA9C File Offset: 0x0003BC9C
		private void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.TestEmail(this.txtEmail.Text) && this.TestPhone(this.txtPhone.Text))
				{
					ClientStats.SendMiscellaneousStatsAsync("OTSFeedback", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.txtDescIssue.Text, this.txtEmail.Text, this.txtPhone.Text, null, null, null);
					new Thread(delegate
					{
						try
						{
							new Process
							{
								StartInfo = 
								{
									Arguments = "-silent",
									FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe")
								}
							}.Start();
						}
						catch (Exception ex2)
						{
							Logger.Error("Exception in starting HD-logCollector.exe: " + ex2.ToString());
						}
					})
					{
						IsBackground = true
					}.Start();
					BlueStacksUIUtils.CloseContainerWindow(this);
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.ImageName = "help";
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_THANK_YOU", "");
					customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_APPRECIATE_FEEDBACK", "");
					customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), null, null, false, null);
					customMessageWindow.Owner = this.ParentWindow;
					this.ParentWindow.ShowDimOverlay(null);
					customMessageWindow.ShowDialog();
					this.ParentWindow.HideDimOverlay();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Submitting ots feedback " + ex.ToString());
			}
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00008DC1 File Offset: 0x00006FC1
		private bool TestEmail(string text)
		{
			BlueStacksUIBinding.BindColor(this.txtEmailBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
			if (!Regex.IsMatch(text, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase))
			{
				this.txtEmailBorder.BorderBrush = Brushes.Red;
				return false;
			}
			return true;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00008DF9 File Offset: 0x00006FF9
		private bool TestPhone(string text)
		{
			BlueStacksUIBinding.BindColor(this.txtPhoneBorder, Border.BorderBrushProperty, "SettingsWindowTabMenuItemForeground");
			if (!Regex.IsMatch(text, OtsFeedbackControl.MakeCombinedPattern(OtsFeedbackControl.m_Phone_Patterns), RegexOptions.IgnoreCase))
			{
				this.txtPhoneBorder.BorderBrush = Brushes.Red;
				return false;
			}
			return true;
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00008E36 File Offset: 0x00007036
		private static string MakeCombinedPattern(IEnumerable<string> patterns)
		{
			return string.Join("|", patterns.Select((string item) => "(" + item + ")").ToArray<string>());
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00008E6C File Offset: 0x0000706C
		private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.TestEmail(this.txtEmail.Text);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x00008E80 File Offset: 0x00007080
		private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.TestPhone(this.txtPhone.Text);
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x0003DC0C File Offset: 0x0003BE0C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/otsfeedbackcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x0003DC3C File Offset: 0x0003BE3C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.MouseLeftButtonUp += this.CloseBtn_MouseLeftButtonUp;
				return;
			case 2:
				this.txtDescIssue = (TextBox)target;
				return;
			case 3:
				this.txtEmailBorder = (Border)target;
				return;
			case 4:
				this.txtEmail = (TextBox)target;
				this.txtEmail.TextChanged += this.txtEmail_TextChanged;
				return;
			case 5:
				this.txtPhoneBorder = (Border)target;
				return;
			case 6:
				this.txtPhone = (TextBox)target;
				this.txtPhone.TextChanged += this.txtPhone_TextChanged;
				return;
			case 7:
				((CustomButton)target).Click += this.SubmitButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000679 RID: 1657
		private static List<string> m_Phone_Patterns = new List<string> { "^[\\d\\s-\\+]{5,15}$" };

		// Token: 0x0400067A RID: 1658
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x0400067B RID: 1659
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox txtDescIssue;

		// Token: 0x0400067C RID: 1660
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border txtEmailBorder;

		// Token: 0x0400067D RID: 1661
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox txtEmail;

		// Token: 0x0400067E RID: 1662
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border txtPhoneBorder;

		// Token: 0x0400067F RID: 1663
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox txtPhone;

		// Token: 0x04000680 RID: 1664
		private bool _contentLoaded;
	}
}
