using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using BlueStacks.Common;
using DiscordRPC;
using DiscordRPC.Message;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000134 RID: 308
	public class Discord : IDisposable
	{
		// Token: 0x06000C66 RID: 3174 RVA: 0x00009CC6 File Offset: 0x00007EC6
		public Discord(MainWindow window)
		{
			this.ParentWindow = window;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00009CF6 File Offset: 0x00007EF6
		internal void Init()
		{
			this.SetSystemAppsAndClientID();
			this.InitDiscordClient();
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x00009D04 File Offset: 0x00007F04
		private void AssignTabChangeEventOnOpenedTabs()
		{
			if (this.ParentWindow != null)
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					foreach (AppTabButton appTabButton in this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList<AppTabButton>())
					{
						if (appTabButton.EventOnTabChanged == null)
						{
							Logger.Info("discord attaching tab change event on tab.." + appTabButton.PackageName);
							this.AssignTabChangeEvent(appTabButton);
							if (appTabButton.IsSelected)
							{
								this.Tab_ChangeOrCreateEvent(null, new TabChangeEventArgs(appTabButton.AppName, appTabButton.PackageName, appTabButton.mTabType));
							}
						}
						else if (appTabButton.IsSelected)
						{
							this.Tab_ChangeOrCreateEvent(null, new TabChangeEventArgs(appTabButton.AppName, appTabButton.PackageName, appTabButton.mTabType));
						}
					}
				}), new object[0]);
			}
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00009D31 File Offset: 0x00007F31
		private void RemoveTabChangeEventFromOpenedTabs()
		{
			if (this.ParentWindow != null)
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					foreach (AppTabButton appTabButton in this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList<AppTabButton>())
					{
						if (appTabButton.EventOnTabChanged != null)
						{
							AppTabButton appTabButton2 = appTabButton;
							appTabButton2.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Remove(appTabButton2.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(this.Tab_ChangeOrCreateEvent));
						}
					}
				}), new object[0]);
			}
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x00045238 File Offset: 0x00043438
		private void SetSystemAppsAndClientID()
		{
			if (PromotionObject.Instance != null)
			{
				this.mDiscordClientID = PromotionObject.Instance.DiscordClientID;
				this.mSystemApps = PromotionObject.Instance.AppSuggestionList.Where((AppSuggestionPromotion x) => string.Equals(x.AppLocation, "more_apps", StringComparison.InvariantCulture)).ToList<AppSuggestionPromotion>();
			}
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00009D5E File Offset: 0x00007F5E
		internal void RemoveAppFromTimestampList(string package)
		{
			if (this.mAppStartTimestamp.ContainsKey(package))
			{
				this.mAppStartTimestamp.Remove(package);
			}
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x00009D7B File Offset: 0x00007F7B
		internal bool IsDiscordClientReady()
		{
			return this.mDiscordClient != null && this.mDiscordClient.IsInitialized && this.mIsDiscordConnected;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00045298 File Offset: 0x00043498
		private void Tab_ChangeOrCreateEvent(object sender, TabChangeEventArgs e)
		{
			try
			{
				if (!string.Equals(this.mPreviousAppPackage, e.PackageName, StringComparison.InvariantCulture) && this.IsDiscordClientReady())
				{
					Logger.Info("Discord tab changed event. PkgName: {0}, AppName: {1}", new object[] { e.PackageName, e.AppName });
					RichPresence richPresence = new RichPresence();
					TabType tabType = e.TabType;
					if (tabType != TabType.AppTab)
					{
						if (tabType - TabType.WebTab <= 1)
						{
							if (e.PackageName.Contains("bluestacks") && e.PackageName.Contains("appcenter"))
							{
								richPresence.State = "Searching";
								richPresence.Details = "Google Play Store";
								richPresence.Assets = new global::DiscordRPC.Assets
								{
									LargeImageKey = "bstk-logo",
									LargeImageText = "BlueStacks",
									SmallImageKey = "com_android_vending",
									SmallImageText = "Google Play"
								};
							}
							else
							{
								richPresence.State = "In Lobby";
								richPresence.Details = "About to start a game";
								richPresence.Assets = new global::DiscordRPC.Assets
								{
									LargeImageKey = "bstk-logo",
									LargeImageText = "BlueStacks",
									SmallImageKey = "",
									SmallImageText = ""
								};
							}
						}
					}
					else if (this.mSystemApps.Any((AppSuggestionPromotion _) => object.Equals(_.AppPackage == e.PackageName, StringComparison.InvariantCulture)))
					{
						richPresence.State = "In Lobby";
						richPresence.Details = "About to start a game";
						richPresence.Assets = new global::DiscordRPC.Assets
						{
							LargeImageKey = "bstk-logo",
							LargeImageText = "BlueStacks",
							SmallImageKey = "",
							SmallImageText = ""
						};
					}
					else if (e.PackageName.Contains("android.vending"))
					{
						richPresence.State = "Searching";
						richPresence.Details = "Google Play Store";
						richPresence.Assets = new global::DiscordRPC.Assets
						{
							LargeImageKey = "bstk-logo",
							LargeImageText = "BlueStacks",
							SmallImageKey = "com_android_vending",
							SmallImageText = "Google Play"
						};
					}
					else
					{
						if (this.mAppStartTimestamp.ContainsKey(e.PackageName))
						{
							richPresence.Timestamps = this.mAppStartTimestamp[e.PackageName];
						}
						else
						{
							richPresence.Timestamps = Timestamps.Now;
							this.mAppStartTimestamp.Add(e.PackageName, Timestamps.Now);
						}
						richPresence.State = "Playing";
						richPresence.Details = e.AppName;
						richPresence.Assets = new global::DiscordRPC.Assets
						{
							LargeImageKey = this.GetMD5HashFromPackageName(e.PackageName),
							LargeImageText = e.AppName,
							SmallImageKey = "bstk-logo",
							SmallImageText = "BlueStacks"
						};
					}
					this.SetPresence(richPresence);
					this.mPreviousAppPackage = e.PackageName;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while setting presence in discord with exception : {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x000455E0 File Offset: 0x000437E0
		private string GetMD5HashFromPackageName(string package)
		{
			string text = new _MD5
			{
				Value = package
			}.FingerPrint.ToLower(CultureInfo.InvariantCulture);
			Logger.Info("Md5 hash for package name: {0}..is {1}", new object[] { package, text });
			return text;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x00009D9A File Offset: 0x00007F9A
		internal void AssignTabChangeEvent(AppTabButton button)
		{
			if (button.EventOnTabChanged == null)
			{
				button.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Combine(button.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(this.Tab_ChangeOrCreateEvent));
			}
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00045624 File Offset: 0x00043824
		private void InitDiscordClient()
		{
			try
			{
				if (this.mDiscordClient == null)
				{
					Logger.Info("Initing discord");
					this.mDiscordClient = new DiscordRpcClient(this.mDiscordClientID);
					this.mDiscordClient.OnReady += delegate(object sender, ReadyMessage msg)
					{
						Logger.Info("Connected to discord with user {0}", new object[] { msg.User.Username });
					};
					this.mDiscordClient.OnPresenceUpdate += this.Client_OnPresenceUpdate;
					this.mDiscordClient.OnError += this.Client_OnError;
					this.mDiscordClient.OnConnectionFailed += this.Client_OnConnectionFailed;
					this.mDiscordClient.OnConnectionEstablished += this.Client_OnConnectionEstablished;
					this.mDiscordClientInvokeTimer = new Timer(150.0);
					this.mDiscordClientInvokeTimer.Elapsed += delegate(object sender, ElapsedEventArgs evt)
					{
						this.mDiscordClient.Invoke();
					};
					this.mDiscordClientInvokeTimer.Start();
					bool flag = this.mDiscordClient.Initialize();
					Logger.Info("Discord client init: {0}", new object[] { flag });
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Exception in Discord init. ex:  " + ex.ToString());
			}
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00045770 File Offset: 0x00043970
		private void Client_OnPresenceUpdate(object sender, PresenceMessage args)
		{
			string text = "Discord presence has been updated with details.";
			string text2;
			if (args == null)
			{
				text2 = null;
			}
			else
			{
				RichPresence presence = args.Presence;
				text2 = ((presence != null) ? presence.Details : null);
			}
			Logger.Info(text + text2);
			if (args.Presence.Assets.LargeImageKey == null)
			{
				RichPresence richPresence = args.Presence.Clone();
				richPresence.Assets.LargeImageKey = "bstk-logo";
				richPresence.Assets.SmallImageKey = "";
				richPresence.Assets.SmallImageText = "";
				this.SetPresence(richPresence);
			}
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x00009DC6 File Offset: 0x00007FC6
		private void SetPresence(RichPresence presence)
		{
			if (this.mDiscordClient != null && this.mDiscordClient.IsInitialized)
			{
				this.mDiscordClient.SetPresence(presence);
				return;
			}
			Logger.Warning("SetPresence called without a client being inited");
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000457FC File Offset: 0x000439FC
		private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
		{
			Logger.Info("Discord connection Established");
			this.mIsDiscordConnected = true;
			this.AssignTabChangeEventOnOpenedTabs();
			ClientStats.SendMiscellaneousStatsAsync("DiscordConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, null, null, null, null);
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00045858 File Offset: 0x00043A58
		private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
		{
			Logger.Info("Discord connection failed. ErrorCode: {0}", new object[] { args.Type });
			this.mIsDiscordConnected = false;
			this.Dispose();
			ClientStats.SendMiscellaneousStatsAsync("DiscordNotConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, null, null, null, null);
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x00009DF4 File Offset: 0x00007FF4
		private void Client_OnError(object sender, ErrorMessage args)
		{
			Logger.Info("Discord client error. ErrorCode: {0}, Message: {1}", new object[] { args.Code, args.Message });
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00009E1D File Offset: 0x0000801D
		internal void ToggleDiscordState(bool state)
		{
			if (state)
			{
				if (this.mDiscordClient == null)
				{
					this.Init();
					return;
				}
			}
			else
			{
				this.Dispose();
			}
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x000458C8 File Offset: 0x00043AC8
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.mDiscordClient != null)
				{
					this.mDiscordClient.OnPresenceUpdate -= this.Client_OnPresenceUpdate;
					this.mDiscordClient.OnError -= this.Client_OnError;
					this.mDiscordClient.OnConnectionFailed -= this.Client_OnConnectionFailed;
					this.mDiscordClient.OnConnectionEstablished -= this.Client_OnConnectionEstablished;
					this.mDiscordClient.Dispose();
					this.RemoveTabChangeEventFromOpenedTabs();
				}
				if (this.mDiscordClientInvokeTimer != null)
				{
					this.mDiscordClientInvokeTimer.Elapsed -= delegate(object sender, ElapsedEventArgs evt)
					{
						this.mDiscordClient.Invoke();
					};
					this.mDiscordClientInvokeTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x00045988 File Offset: 0x00043B88
		~Discord()
		{
			this.Dispose(false);
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00009E37 File Offset: 0x00008037
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0400079D RID: 1949
		private DiscordRpcClient mDiscordClient;

		// Token: 0x0400079E RID: 1950
		private Timer mDiscordClientInvokeTimer;

		// Token: 0x0400079F RID: 1951
		private string mPreviousAppPackage;

		// Token: 0x040007A0 RID: 1952
		private List<AppSuggestionPromotion> mSystemApps = new List<AppSuggestionPromotion>();

		// Token: 0x040007A1 RID: 1953
		private string mDiscordClientID = string.Empty;

		// Token: 0x040007A2 RID: 1954
		private Dictionary<string, Timestamps> mAppStartTimestamp = new Dictionary<string, Timestamps>();

		// Token: 0x040007A3 RID: 1955
		private MainWindow ParentWindow;

		// Token: 0x040007A4 RID: 1956
		private bool mIsDiscordConnected;

		// Token: 0x040007A5 RID: 1957
		private bool disposedValue;
	}
}
