using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200010D RID: 269
	public class PromptGoogleSigninControl : UserControl, IComponentConnector
	{
		// Token: 0x06000B27 RID: 2855 RVA: 0x000090A7 File Offset: 0x000072A7
		public PromptGoogleSigninControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void CloseBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0003F354 File Offset: 0x0003D554
		private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClose", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID, null, null, null);
				BlueStacksUIUtils.CloseContainerWindow(this);
			}
			catch (Exception ex)
			{
				string text = "Exception in CloseBtn_MouseLeftButtonUp. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0003F3C8 File Offset: 0x0003D5C8
		private void SigninBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
				if (appIcon != null)
				{
					this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
				}
				ClientStats.SendMiscellaneousStatsAsync("GoogleSigninClick", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID, null, null, null);
				BlueStacksUIUtils.CloseContainerWindow(this);
			}
			catch (Exception ex)
			{
				string text = "Exception in SigninBtn_Click. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0003F488 File Offset: 0x0003D688
		private void SigninLaterBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ClientStats.SendMiscellaneousStatsAsync("GoogleSigninLater", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, RegistryManager.Instance.InstallID, null, null, null);
				BlueStacksUIUtils.CloseContainerWindow(this);
			}
			catch (Exception ex)
			{
				string text = "Exception in SigninLaterBtn_Click. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0003F4FC File Offset: 0x0003D6FC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/promptgooglesignincontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x0003F52C File Offset: 0x0003D72C
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
				this.CloseBtn = (CustomPictureBox)target;
				this.CloseBtn.PreviewMouseDown += this.CloseBtn_PreviewMouseDown;
				this.CloseBtn.MouseLeftButtonUp += this.CloseBtn_MouseLeftButtonUp;
				return;
			case 2:
				this.SigninLaterBtn = (CustomButton)target;
				this.SigninLaterBtn.Click += this.SigninLaterBtn_Click;
				return;
			case 3:
				this.SigninBtn = (CustomButton)target;
				this.SigninBtn.Click += this.SigninBtn_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006B8 RID: 1720
		private MainWindow ParentWindow;

		// Token: 0x040006B9 RID: 1721
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox CloseBtn;

		// Token: 0x040006BA RID: 1722
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton SigninLaterBtn;

		// Token: 0x040006BB RID: 1723
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton SigninBtn;

		// Token: 0x040006BC RID: 1724
		private bool _contentLoaded;
	}
}
