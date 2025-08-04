using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000266 RID: 614
	public class FrontendPopupControl : UserControl, IComponentConnector
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001640 RID: 5696 RVA: 0x0000EFA0 File Offset: 0x0000D1A0
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					this.mMainWindow = Window.GetWindow(this) as MainWindow;
				}
				return this.mMainWindow;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06001641 RID: 5697 RVA: 0x0000EFC1 File Offset: 0x0000D1C1
		// (set) Token: 0x06001642 RID: 5698 RVA: 0x0000EFC9 File Offset: 0x0000D1C9
		public PlayStoreAction mAction { get; set; }

		// Token: 0x06001643 RID: 5699 RVA: 0x0000EFD2 File Offset: 0x0000D1D2
		public FrontendPopupControl()
		{
			this.InitializeComponent();
			this.RequestedAppDisplayed = (EventHandler<EventArgs>)Delegate.Combine(this.RequestedAppDisplayed, new EventHandler<EventArgs>(this.RequestedApp_Displayed));
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x000856CC File Offset: 0x000838CC
		private void ProcessArgs(string googlePlayStoreArg, bool isWindowForcedTillLoaded)
		{
			this.mGooglePlayStoreArg = googlePlayStoreArg;
			this.mIsWindowForcedTillLoaded = isWindowForcedTillLoaded;
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mBaseControl.Init(this, this.ParentWindow.mFrontendGrid, false, isWindowForcedTillLoaded);
				this.mBaseControl.DimBackground();
				this.ParentWindow.mCommonHandler.SetCustomCursorForApp("com.android.vending");
				if (this.mAction == PlayStoreAction.OpenApp)
				{
					if (this.ParentWindow.mAppHandler.IsAppInstalled(googlePlayStoreArg))
					{
						this.ParentWindow.mAppHandler.SendRunAppRequestAsync(googlePlayStoreArg, "", false);
					}
					else
					{
						AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
						this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(googlePlayStoreArg);
					}
					this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
					return;
				}
				if (this.mAction == PlayStoreAction.SearchApp)
				{
					AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
					this.ParentWindow.mAppHandler.SendSearchPlayRequestAsync(googlePlayStoreArg);
					this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
					return;
				}
				if (this.mAction == PlayStoreAction.CustomActivity)
				{
					AppHandler.EventOnAppDisplayed = this.RequestedAppDisplayed;
					this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
					Dictionary<string, string> dictionary = new Dictionary<string, string> { { "action", googlePlayStoreArg } };
					this.ParentWindow.mAppHandler.StartCustomActivity(dictionary);
				}
			}), new object[0]);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0000F002 File Offset: 0x0000D202
		internal void Reload()
		{
			if (base.Visibility == Visibility.Visible && !string.IsNullOrEmpty(this.mGooglePlayStoreArg))
			{
				this.ProcessArgs(this.mGooglePlayStoreArg, this.mIsWindowForcedTillLoaded);
			}
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0000F02B File Offset: 0x0000D22B
		internal void RequestedApp_Displayed(object sender, EventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mBaseControl.ShowContent();
			}), new object[0]);
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0008572C File Offset: 0x0008392C
		internal void Init(string args, string appName, PlayStoreAction action, bool isWindowForcedTillLoaded = false)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (!this.ParentWindow.mGuestBootCompleted)
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE", "");
					BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_GUEST_NOT_BOOTED", "");
					customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
					customMessageWindow.Owner = this.ParentWindow;
					customMessageWindow.ShowDialog();
					return;
				}
				if (action == PlayStoreAction.OpenApp && this.ParentWindow.mAppHandler.IsAppInstalled(args) && !"com.android.vending".Equals(args, StringComparison.InvariantCultureIgnoreCase))
				{
					AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(args);
					if (appIcon != null)
					{
						if (appIcon.AppIncompatType != AppIncompatType.None)
						{
							GrmHandler.HandleCompatibility(appIcon.PackageName, this.ParentWindow.mVmName);
							return;
						}
						this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
						return;
					}
				}
				else if (!string.IsNullOrEmpty(args))
				{
					if (!this.ParentWindow.WelcomeTabParentGrid.IsVisible)
					{
						this.ParentWindow.mCommonHandler.HomeButtonHandler(false, false);
					}
					this.mBaseControl.mTitleLabel.Content = appName;
					this.mAction = action;
					this.Visibility = Visibility.Visible;
					this.ParentWindow.ChangeOrientationFromClient(false, false);
					this.ProcessArgs(args, isWindowForcedTillLoaded);
				}
			}), new object[0]);
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0000F04B File Offset: 0x0000D24B
		internal void HideWindow()
		{
			this.mBaseControl.HideWindow();
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x00085784 File Offset: 0x00083984
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/frontendpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0000F058 File Offset: 0x0000D258
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				this.mBaseControl = (DimControlWithProgresBar)target;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x04000D9B RID: 3483
		private MainWindow mMainWindow;

		// Token: 0x04000D9D RID: 3485
		private EventHandler<EventArgs> RequestedAppDisplayed;

		// Token: 0x04000D9E RID: 3486
		private string mGooglePlayStoreArg;

		// Token: 0x04000D9F RID: 3487
		private bool mIsWindowForcedTillLoaded;

		// Token: 0x04000DA0 RID: 3488
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DimControlWithProgresBar mBaseControl;

		// Token: 0x04000DA1 RID: 3489
		private bool _contentLoaded;
	}
}
