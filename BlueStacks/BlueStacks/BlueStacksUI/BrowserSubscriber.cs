using System;
using System.Collections.Generic;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200003B RID: 59
	public class BrowserSubscriber : ISubscriber
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x00019768 File Offset: 0x00017968
		public BrowserSubscriber(BrowserControl control)
		{
			this.mControl = control;
			foreach (BrowserControlTags browserControlTags in ((control != null) ? control.TagsSubscribedDict.Keys : null))
			{
				this.SubscribeTag(browserControlTags);
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000197E0 File Offset: 0x000179E0
		public void SubscribeTag(BrowserControlTags args)
		{
			switch (args)
			{
			case BrowserControlTags.bootComplete:
				this.mTokens[BrowserControlTags.bootComplete] = EventAggregator.Subscribe<BootCompleteEventArgs>(new Action<BootCompleteEventArgs>(this.Message));
				return;
			case BrowserControlTags.googleSigninComplete:
				this.mTokens[BrowserControlTags.googleSigninComplete] = EventAggregator.Subscribe<GoogleSignInCompleteEventArgs>(new Action<GoogleSignInCompleteEventArgs>(this.Message));
				return;
			case BrowserControlTags.appPlayerClosing:
				this.mTokens[BrowserControlTags.appPlayerClosing] = EventAggregator.Subscribe<AppPlayerClosingEventArgs>(new Action<AppPlayerClosingEventArgs>(this.Message));
				return;
			case BrowserControlTags.tabClosing:
				this.mTokens[BrowserControlTags.tabClosing] = EventAggregator.Subscribe<TabClosingEventArgs>(new Action<TabClosingEventArgs>(this.Message));
				return;
			case BrowserControlTags.tabSwitched:
				this.mTokens[BrowserControlTags.tabSwitched] = EventAggregator.Subscribe<TabSwitchedEventArgs>(new Action<TabSwitchedEventArgs>(this.Message));
				return;
			case BrowserControlTags.appInstalled:
				this.mTokens[BrowserControlTags.appInstalled] = EventAggregator.Subscribe<AppInstalledEventArgs>(new Action<AppInstalledEventArgs>(this.Message));
				return;
			case BrowserControlTags.appUninstalled:
				this.mTokens[BrowserControlTags.appUninstalled] = EventAggregator.Subscribe<AppUninstalledEventArgs>(new Action<AppUninstalledEventArgs>(this.Message));
				return;
			case BrowserControlTags.grmAppListUpdate:
				this.mTokens[BrowserControlTags.grmAppListUpdate] = EventAggregator.Subscribe<GrmAppListUpdateEventArgs>(new Action<GrmAppListUpdateEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkDownloadStarted:
				this.mTokens[BrowserControlTags.apkDownloadStarted] = EventAggregator.Subscribe<ApkDownloadStartedEventArgs>(new Action<ApkDownloadStartedEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkDownloadFailed:
				this.mTokens[BrowserControlTags.apkDownloadFailed] = EventAggregator.Subscribe<ApkDownloadFailedEventArgs>(new Action<ApkDownloadFailedEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkDownloadCurrentProgress:
				this.mTokens[BrowserControlTags.apkDownloadCurrentProgress] = EventAggregator.Subscribe<ApkDownloadCurrentProgressEventArgs>(new Action<ApkDownloadCurrentProgressEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkDownloadCompleted:
				this.mTokens[BrowserControlTags.apkDownloadCompleted] = EventAggregator.Subscribe<ApkDownloadCompletedEventArgs>(new Action<ApkDownloadCompletedEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkInstallStarted:
				this.mTokens[BrowserControlTags.apkInstallStarted] = EventAggregator.Subscribe<ApkInstallStartedEventArgs>(new Action<ApkInstallStartedEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkInstallFailed:
				this.mTokens[BrowserControlTags.apkInstallFailed] = EventAggregator.Subscribe<ApkInstallFailedEventArgs>(new Action<ApkInstallFailedEventArgs>(this.Message));
				return;
			case BrowserControlTags.apkInstallCompleted:
				this.mTokens[BrowserControlTags.apkInstallCompleted] = EventAggregator.Subscribe<ApkInstallCompletedEventArgs>(new Action<ApkInstallCompletedEventArgs>(this.Message));
				return;
			case BrowserControlTags.getVmInfo:
				this.mTokens[BrowserControlTags.getVmInfo] = EventAggregator.Subscribe<GetVmInfoEventArgs>(new Action<GetVmInfoEventArgs>(this.Message));
				return;
			case BrowserControlTags.userInfoUpdated:
				this.mTokens[BrowserControlTags.userInfoUpdated] = EventAggregator.Subscribe<UserInfoUpdatedEventArgs>(new Action<UserInfoUpdatedEventArgs>(this.Message));
				return;
			case BrowserControlTags.themeChange:
				this.mTokens[BrowserControlTags.themeChange] = EventAggregator.Subscribe<ThemeChangeEventArgs>(new Action<ThemeChangeEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemDownloadStarted:
				this.mTokens[BrowserControlTags.oemDownloadStarted] = EventAggregator.Subscribe<OemDownloadStartedEventArgs>(new Action<OemDownloadStartedEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemDownloadFailed:
				this.mTokens[BrowserControlTags.oemDownloadFailed] = EventAggregator.Subscribe<OemDownloadFailedEventArgs>(new Action<OemDownloadFailedEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemDownloadCurrentProgress:
				this.mTokens[BrowserControlTags.oemDownloadCurrentProgress] = EventAggregator.Subscribe<OemDownloadCurrentProgressEventArgs>(new Action<OemDownloadCurrentProgressEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemDownloadCompleted:
				this.mTokens[BrowserControlTags.oemDownloadCompleted] = EventAggregator.Subscribe<OemDownloadCompletedEventArgs>(new Action<OemDownloadCompletedEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemInstallStarted:
				this.mTokens[BrowserControlTags.oemInstallStarted] = EventAggregator.Subscribe<OemInstallStartedEventArgs>(new Action<OemInstallStartedEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemInstallFailed:
				this.mTokens[BrowserControlTags.oemInstallFailed] = EventAggregator.Subscribe<OemInstallFailedEventArgs>(new Action<OemInstallFailedEventArgs>(this.Message));
				return;
			case BrowserControlTags.oemInstallCompleted:
				this.mTokens[BrowserControlTags.oemInstallCompleted] = EventAggregator.Subscribe<OemInstallCompletedEventArgs>(new Action<OemInstallCompletedEventArgs>(this.Message));
				return;
			case BrowserControlTags.showFlePopup:
				this.mTokens[BrowserControlTags.showFlePopup] = EventAggregator.Subscribe<ShowFlePopupEventArgs>(new Action<ShowFlePopupEventArgs>(this.Message));
				return;
			default:
				return;
			}
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00019B94 File Offset: 0x00017D94
		public void UnsubscribeTag(BrowserControlTags args)
		{
			switch (args)
			{
			case BrowserControlTags.bootComplete:
				EventAggregator.Unsubscribe<BootCompleteEventArgs>((Subscription<BootCompleteEventArgs>)this.mTokens[BrowserControlTags.bootComplete]);
				return;
			case BrowserControlTags.googleSigninComplete:
				EventAggregator.Unsubscribe<GoogleSignInCompleteEventArgs>((Subscription<GoogleSignInCompleteEventArgs>)this.mTokens[BrowserControlTags.googleSigninComplete]);
				return;
			case BrowserControlTags.appPlayerClosing:
				EventAggregator.Unsubscribe<AppPlayerClosingEventArgs>((Subscription<AppPlayerClosingEventArgs>)this.mTokens[BrowserControlTags.appPlayerClosing]);
				return;
			case BrowserControlTags.tabClosing:
				EventAggregator.Unsubscribe<TabClosingEventArgs>((Subscription<TabClosingEventArgs>)this.mTokens[BrowserControlTags.tabClosing]);
				return;
			case BrowserControlTags.tabSwitched:
				EventAggregator.Unsubscribe<TabSwitchedEventArgs>((Subscription<TabSwitchedEventArgs>)this.mTokens[BrowserControlTags.tabSwitched]);
				return;
			case BrowserControlTags.appInstalled:
				EventAggregator.Unsubscribe<AppInstalledEventArgs>((Subscription<AppInstalledEventArgs>)this.mTokens[BrowserControlTags.appInstalled]);
				return;
			case BrowserControlTags.appUninstalled:
				EventAggregator.Unsubscribe<AppUninstalledEventArgs>((Subscription<AppUninstalledEventArgs>)this.mTokens[BrowserControlTags.appUninstalled]);
				return;
			case BrowserControlTags.grmAppListUpdate:
				EventAggregator.Unsubscribe<GrmAppListUpdateEventArgs>((Subscription<GrmAppListUpdateEventArgs>)this.mTokens[BrowserControlTags.grmAppListUpdate]);
				return;
			case BrowserControlTags.apkDownloadStarted:
				EventAggregator.Unsubscribe<ApkDownloadStartedEventArgs>((Subscription<ApkDownloadStartedEventArgs>)this.mTokens[BrowserControlTags.apkDownloadStarted]);
				return;
			case BrowserControlTags.apkDownloadFailed:
				EventAggregator.Unsubscribe<ApkDownloadFailedEventArgs>((Subscription<ApkDownloadFailedEventArgs>)this.mTokens[BrowserControlTags.apkDownloadFailed]);
				return;
			case BrowserControlTags.apkDownloadCurrentProgress:
				EventAggregator.Unsubscribe<ApkDownloadCurrentProgressEventArgs>((Subscription<ApkDownloadCurrentProgressEventArgs>)this.mTokens[BrowserControlTags.apkDownloadCurrentProgress]);
				return;
			case BrowserControlTags.apkDownloadCompleted:
				EventAggregator.Unsubscribe<ApkDownloadCompletedEventArgs>((Subscription<ApkDownloadCompletedEventArgs>)this.mTokens[BrowserControlTags.apkDownloadCompleted]);
				return;
			case BrowserControlTags.apkInstallStarted:
				EventAggregator.Unsubscribe<ApkInstallStartedEventArgs>((Subscription<ApkInstallStartedEventArgs>)this.mTokens[BrowserControlTags.apkInstallStarted]);
				return;
			case BrowserControlTags.apkInstallFailed:
				EventAggregator.Unsubscribe<ApkInstallFailedEventArgs>((Subscription<ApkInstallFailedEventArgs>)this.mTokens[BrowserControlTags.apkInstallFailed]);
				return;
			case BrowserControlTags.apkInstallCompleted:
				EventAggregator.Unsubscribe<ApkInstallCompletedEventArgs>((Subscription<ApkInstallCompletedEventArgs>)this.mTokens[BrowserControlTags.apkInstallCompleted]);
				return;
			case BrowserControlTags.getVmInfo:
				EventAggregator.Unsubscribe<GetVmInfoEventArgs>((Subscription<GetVmInfoEventArgs>)this.mTokens[BrowserControlTags.getVmInfo]);
				return;
			case BrowserControlTags.userInfoUpdated:
				EventAggregator.Unsubscribe<UserInfoUpdatedEventArgs>((Subscription<UserInfoUpdatedEventArgs>)this.mTokens[BrowserControlTags.userInfoUpdated]);
				return;
			case BrowserControlTags.themeChange:
				EventAggregator.Unsubscribe<ThemeChangeEventArgs>((Subscription<ThemeChangeEventArgs>)this.mTokens[BrowserControlTags.themeChange]);
				return;
			case BrowserControlTags.oemDownloadStarted:
				EventAggregator.Unsubscribe<OemDownloadStartedEventArgs>((Subscription<OemDownloadStartedEventArgs>)this.mTokens[BrowserControlTags.oemDownloadStarted]);
				return;
			case BrowserControlTags.oemDownloadFailed:
				EventAggregator.Unsubscribe<OemDownloadFailedEventArgs>((Subscription<OemDownloadFailedEventArgs>)this.mTokens[BrowserControlTags.oemDownloadFailed]);
				return;
			case BrowserControlTags.oemDownloadCurrentProgress:
				EventAggregator.Unsubscribe<OemDownloadCurrentProgressEventArgs>((Subscription<OemDownloadCurrentProgressEventArgs>)this.mTokens[BrowserControlTags.oemDownloadCurrentProgress]);
				return;
			case BrowserControlTags.oemDownloadCompleted:
				EventAggregator.Unsubscribe<OemDownloadCompletedEventArgs>((Subscription<OemDownloadCompletedEventArgs>)this.mTokens[BrowserControlTags.oemDownloadCompleted]);
				return;
			case BrowserControlTags.oemInstallStarted:
				EventAggregator.Unsubscribe<OemInstallStartedEventArgs>((Subscription<OemInstallStartedEventArgs>)this.mTokens[BrowserControlTags.oemInstallStarted]);
				return;
			case BrowserControlTags.oemInstallFailed:
				EventAggregator.Unsubscribe<OemInstallFailedEventArgs>((Subscription<OemInstallFailedEventArgs>)this.mTokens[BrowserControlTags.oemInstallFailed]);
				return;
			case BrowserControlTags.oemInstallCompleted:
				EventAggregator.Unsubscribe<OemInstallCompletedEventArgs>((Subscription<OemInstallCompletedEventArgs>)this.mTokens[BrowserControlTags.oemInstallCompleted]);
				return;
			case BrowserControlTags.showFlePopup:
				EventAggregator.Unsubscribe<ShowFlePopupEventArgs>((Subscription<ShowFlePopupEventArgs>)this.mTokens[BrowserControlTags.showFlePopup]);
				return;
			default:
				return;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00019E78 File Offset: 0x00018078
		public void Message(EventArgs eventArgs)
		{
			BrowserEventArgs browserEventArgs = eventArgs as BrowserEventArgs;
			if (browserEventArgs != null && (string.Equals(this.mControl.ParentWindow.mVmName, browserEventArgs.mVmName, StringComparison.InvariantCultureIgnoreCase) || (this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag].ContainsKey("IsReceiveFromAllVm") && this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag]["IsReceiveFromAllVm"].ToObject<bool>())))
			{
				JObject jobject = new JObject();
				jobject["eventRaised"] = browserEventArgs.ClientTag.ToString();
				jobject["vmName"] = browserEventArgs.mVmName;
				if (browserEventArgs.ExtraData != null)
				{
					jobject["extraData"] = browserEventArgs.ExtraData;
				}
				this.mControl.CallBackToHtml(this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag]["CallbackFunction"].ToString(), jobject.ToString(Formatting.None, new JsonConverter[0]));
			}
		}

		// Token: 0x040001E8 RID: 488
		private BrowserControl mControl;

		// Token: 0x040001E9 RID: 489
		private Dictionary<BrowserControlTags, object> mTokens = new Dictionary<BrowserControlTags, object>();
	}
}
