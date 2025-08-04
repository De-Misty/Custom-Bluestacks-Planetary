using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A9 RID: 425
	public class PromotionControl : global::System.Windows.Controls.UserControl, IDisposable, IComponentConnector
	{
		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x060010F6 RID: 4342 RVA: 0x0000C261 File Offset: 0x0000A461
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

		// Token: 0x060010F7 RID: 4343 RVA: 0x0006A5F4 File Offset: 0x000687F4
		public PromotionControl()
		{
			this.InitializeComponent();
			this.PromoControl = this;
			if (!DesignerProperties.GetIsInDesignMode(this.PromoControl))
			{
				if (!string.IsNullOrEmpty(RegistryManager.Instance.PromotionId) || FeatureManager.Instance.IsPromotionFixed)
				{
					this.mPromotionImage.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promotions/promotion.jpg");
					this.mPromotionImageGrid.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 0, 0, 0));
				}
				BlueStacksUIBinding.Bind(this.BootText, "STRING_BOOT_TIME", "");
				int num = RegistryManager.Instance.LastBootTime / 400;
				if (num <= 0)
				{
					RegistryManager.Instance.LastBootTime = 120000;
					RegistryManager.Instance.NoOfBootCompleted = 0;
					num = 1000;
				}
				this.progressTimer.Tick += this.ProgressTimer_Tick;
				this.progressTimer.Interval = num;
				this.progressTimer.Start();
				if (PromotionObject.Instance == null)
				{
					PromotionObject.LoadDataFromFile();
				}
				PromotionObject.BootPromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.BootPromotionHandler, new EventHandler(this.PromotionControl_BootPromotionHandler));
			}
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0000C282 File Offset: 0x0000A482
		private void PromotionControl_BootPromotionHandler(object sender, EventArgs e)
		{
			this.PromoControl.Dispatcher.Invoke(new Action(delegate
			{
				Fraction fraction = new Fraction((long)RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth, (long)RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight);
				if (App.defaultResolution == fraction && (!this.dictRunningPromotions.Keys.All(new Func<string, bool>(PromotionObject.Instance.DictBootPromotions.Keys.Contains<string>)) || PromotionObject.Instance.DictBootPromotions.Count != this.dictRunningPromotions.Count))
				{
					this.StopSlider();
					this.StartAnimation(PromotionObject.Instance.DictBootPromotions);
				}
			}), new object[0]);
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0006A764 File Offset: 0x00068964
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Fraction fraction = new Fraction((long)RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth, (long)RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight);
			if (App.defaultResolution != fraction)
			{
				this.StartAnimation(new SerializableDictionary<string, BootPromotion>());
				return;
			}
			this.StartAnimation(PromotionObject.Instance.DictBootPromotions);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0006A7E0 File Offset: 0x000689E0
		private void StartAnimation(SerializableDictionary<string, BootPromotion> dict)
		{
			this.PromoControl.Dispatcher.Invoke(new Action(delegate
			{
				this.dictRunningPromotions = dict.DeepCopy<SerializableDictionary<string, BootPromotion>>();
				if (dict.Count > 0)
				{
					List<KeyValuePair<string, BootPromotion>> myList = dict.ToList<KeyValuePair<string, BootPromotion>>();
					myList.Sort((KeyValuePair<string, BootPromotion> pair1, KeyValuePair<string, BootPromotion> pair2) => pair1.Value.Order.CompareTo(pair2.Value.Order));
					this.mPromotionImageGrid.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 0, 0, 0));
					this.mRunPromotion = true;
					this.mSliderAnimationThread = new Thread(delegate
					{
						this.mThreadId = this.mSliderAnimationThread.ManagedThreadId;
						Dictionary<BootPromotion, int> bootPromos = new Dictionary<BootPromotion, int>();
						while (this.mRunPromotion && this.mThreadId == Thread.CurrentThread.ManagedThreadId)
						{
							try
							{
								using (List<KeyValuePair<string, BootPromotion>>.Enumerator enumerator = myList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KeyValuePair<string, BootPromotion> item = enumerator.Current;
										if (!this.mRunPromotion || this.mThreadId != Thread.CurrentThread.ManagedThreadId)
										{
											break;
										}
										if (this.currentBootPromotion == null)
										{
											this.currentBootPromotion = item.Value;
										}
										this.PromoControl.Dispatcher.Invoke(new Action(delegate
										{
											this.HandleAnimation(item.Value);
											this.currentBootPromotion = item.Value;
											this.SetLoadingText(item.Value.ButtonText);
											if (bootPromos.ContainsKey(item.Value))
											{
												bootPromos[item.Value] = bootPromos[item.Value] + 1;
											}
											else
											{
												bootPromos.Add(item.Value, 1);
											}
											PromotionControl.sBootPromotionDisplayed = bootPromos;
										}), new object[0]);
										this.mBootPromotionImageTimeout = PromotionObject.Instance.BootPromoDisplaytime;
										Thread.Sleep(this.mBootPromotionImageTimeout);
									}
								}
							}
							catch (Exception ex)
							{
								Logger.Error(ex.ToString());
							}
						}
					})
					{
						IsBackground = true
					};
					this.mSliderAnimationThread.Start();
				}
			}), new object[0]);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0006A824 File Offset: 0x00068A24
		private void ProgressTimer_Tick(object sender, EventArgs e)
		{
			try
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.InitiateHtmlSidePanel();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while creating HTML sidepanel .Exception: " + ex.ToString());
			}
			if (this.mProgress >= 99.0 && !this.mForceComplete)
			{
				this.mProgressBar.Value = this.mProgress;
				this.mProgress += 0.0;
				return;
			}
			if (this.mProgress >= 95.0 && !this.mForceComplete)
			{
				this.progressTimer.Interval = this.progressTimer.Interval;
				this.mProgressBar.Value = this.mProgress;
				this.mProgress += 0.025;
				return;
			}
			this.mProgressBar.Value = this.mProgress;
			this.mProgress += 0.25;
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x0006A930 File Offset: 0x00068B30
		private void mPromoButton_Click(object sender, RoutedEventArgs e)
		{
			this.StopSlider();
			ClientStats.SendMiscellaneousStatsAsync("BootPromotion", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, JsonConvert.SerializeObject(this.currentBootPromotion.ExtraPayload), null, null, null, null, null);
			GenericAction genericAction = GenericAction.None;
			if (this.currentBootPromotion.ExtraPayload.ContainsKey("click_generic_action"))
			{
				genericAction = EnumHelper.Parse<GenericAction>(this.currentBootPromotion.ExtraPayload["click_generic_action"], GenericAction.None);
			}
			if (genericAction <= GenericAction.HomeAppTab)
			{
				if (genericAction <= GenericAction.ApplicationBrowser)
				{
					if (genericAction - GenericAction.InstallPlay <= 1)
					{
						goto IL_00C8;
					}
					if (genericAction != GenericAction.ApplicationBrowser)
					{
						goto IL_00C8;
					}
				}
				else if (genericAction != GenericAction.UserBrowser && genericAction != GenericAction.HomeAppTab)
				{
					goto IL_00C8;
				}
			}
			else if (genericAction <= GenericAction.KeyBasedPopup)
			{
				if (genericAction != GenericAction.SettingsMenu)
				{
					if (genericAction != GenericAction.KeyBasedPopup)
					{
						goto IL_00C8;
					}
					goto IL_00C8;
				}
			}
			else
			{
				if (genericAction != GenericAction.OpenSystemApp && genericAction != GenericAction.InstallPlayPopup)
				{
					goto IL_00C8;
				}
				goto IL_00C8;
			}
			this.isPerformActionOnClose = false;
			goto IL_00CF;
			IL_00C8:
			this.isPerformActionOnClose = true;
			IL_00CF:
			if (this.isPerformActionOnClose)
			{
				this.mPromotionInfoBorder.Visibility = Visibility.Visible;
				this.mPromoButton.Visibility = Visibility.Hidden;
				this.mPromoInfoText.Text = this.currentBootPromotion.PromoBtnClickStatusText.ToString(CultureInfo.InvariantCulture);
				if (string.Equals(this.currentBootPromotion.ThemeEnabled, "true", StringComparison.InvariantCultureIgnoreCase))
				{
					this.ParentWindow.Utils.ApplyTheme(this.currentBootPromotion.ThemeName);
				}
				return;
			}
			this.mExtraPayloadClicked = this.currentBootPromotion.ExtraPayload;
			this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.currentBootPromotion.ExtraPayload, "boot_promo", "");
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x0006AAB8 File Offset: 0x00068CB8
		private void StopSlider()
		{
			try
			{
				if (this.mRunPromotion)
				{
					this.mRunPromotion = false;
					if (PromotionControl.sBootPromotionDisplayed != null && PromotionControl.sBootPromotionDisplayed.Any<KeyValuePair<BootPromotion, int>>())
					{
						Dictionary<BootPromotion, int> bootPromos = PromotionControl.sBootPromotionDisplayed;
						PromotionControl.sBootPromotionDisplayed = null;
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							PromotionControl.SendPromotionStats(bootPromos);
						});
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception aborting thread" + ex.ToString());
			}
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0006AB38 File Offset: 0x00068D38
		private static void SendPromotionStats(Dictionary<BootPromotion, int> bootPromos)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"prod_ver",
						RegistryManager.Instance.ClientVersion
					},
					{
						"eng_ver",
						RegistryManager.Instance.Version
					},
					{
						"guid",
						RegistryManager.Instance.UserGuid
					},
					{
						"locale",
						RegistryManager.Instance.UserSelectedLocale
					},
					{
						"oem",
						RegistryManager.Instance.Oem
					},
					{
						"partner",
						RegistryManager.Instance.Partner
					},
					{
						"campaign_json",
						RegistryManager.Instance.CampaignJson
					}
				};
				List<BootBanner> list = new List<BootBanner>();
				foreach (KeyValuePair<BootPromotion, int> keyValuePair in bootPromos)
				{
					list.Add(new BootBanner
					{
						Frequency = keyValuePair.Value.ToString(CultureInfo.InvariantCulture),
						ClickActionPackagename = keyValuePair.Key.ExtraPayload["click_action_packagename"],
						ClickGenericAction = keyValuePair.Key.ExtraPayload["click_generic_action"],
						ClickActionValue = keyValuePair.Key.ExtraPayload["click_action_value"],
						Id = keyValuePair.Key.Id,
						ButtonText = keyValuePair.Key.ButtonText,
						Order = keyValuePair.Key.Order.ToString(CultureInfo.InvariantCulture),
						ImageUrl = keyValuePair.Key.ImageUrl,
						HashTags = keyValuePair.Key.ExtraPayload["hash_tags"]
					});
				}
				dictionary.Add("boot_banners", JsonConvert.SerializeObject(list));
				BstHttpClient.Post(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
				{
					RegistryManager.Instance.Host,
					"bs4/stats/client_boot_promotion_stats"
				})), dictionary, null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("SendPromotionStats", new object[] { ex });
			}
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0006ADA8 File Offset: 0x00068FA8
		private void SetLoadingText(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				this.mPromoButton.Content = text;
				this.mPromoButton.Visibility = Visibility.Visible;
			}
			else
			{
				this.mPromoButton.Visibility = Visibility.Hidden;
			}
			this.isPerformActionOnClose = false;
			this.mPromotionInfoBorder.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0000C2A7 File Offset: 0x0000A4A7
		internal void Stop()
		{
			this.progressTimer.Stop();
			this.progressTimer.Dispose();
			this.StopSlider();
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0000C2C5 File Offset: 0x0000A4C5
		internal void HandlePromotionEventAfterBoot()
		{
			if (this.isPerformActionOnClose)
			{
				this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.currentBootPromotion.ExtraPayload, "boot_promo", "");
				this.isPerformActionOnClose = false;
			}
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0000C2FB File Offset: 0x0000A4FB
		private void HandleAnimation(BootPromotion promo)
		{
			PromotionControl.AnimateImage(this.mPromotionImage, promo.ImagePath);
			if (!string.IsNullOrEmpty(promo.ButtonText))
			{
				this.mPromoButton.Visibility = Visibility.Visible;
				this.mPromoButton.Content = promo.ButtonText;
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0006ADF8 File Offset: 0x00068FF8
		private static void AnimateImage(CustomPictureBox image, string imagePath)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(0.6);
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(0.6);
			DoubleAnimation fadeInAnimation = new DoubleAnimation(1.0, timeSpan);
			if (image.Source == null)
			{
				image.Opacity = 1.0;
				image.IsFullImagePath = true;
				image.ImageName = imagePath;
				return;
			}
			if (image.ImageName != imagePath)
			{
				DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, timeSpan2);
				doubleAnimation.Completed += delegate(object o, EventArgs e)
				{
					image.IsFullImagePath = true;
					image.ImageName = imagePath;
					image.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
				};
				image.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0000C338 File Offset: 0x0000A538
		private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mPromotionInfoBorder.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0000C346 File Offset: 0x0000A546
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.progressTimer != null)
				{
					this.progressTimer.Tick -= this.ProgressTimer_Tick;
					this.progressTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0006AEE0 File Offset: 0x000690E0
		~PromotionControl()
		{
			this.Dispose(false);
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0000C383 File Offset: 0x0000A583
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0006AF10 File Offset: 0x00069110
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/promotioncontrol.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0006AF40 File Offset: 0x00069140
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
				((PromotionControl)target).Loaded += this.UserControl_Loaded;
				return;
			case 2:
				this.mPromotionImageGrid = (Grid)target;
				return;
			case 3:
				this.mPromotionImage = (CustomPictureBox)target;
				return;
			case 4:
				this.mPromoButton = (CustomButton)target;
				this.mPromoButton.Click += this.mPromoButton_Click;
				return;
			case 5:
				this.mPromotionInfoBorder = (Border)target;
				return;
			case 6:
				this.mPromoInfoText = (TextBlock)target;
				return;
			case 7:
				this.mCloseButton = (CustomPictureBox)target;
				this.mCloseButton.PreviewMouseLeftButtonUp += this.CloseButton_PreviewMouseLeftButtonUp;
				return;
			case 8:
				this.mProgressBar = (BlueProgressBar)target;
				return;
			case 9:
				this.BootText = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000ADA RID: 2778
		private bool isPerformActionOnClose;

		// Token: 0x04000ADB RID: 2779
		private bool mRunPromotion = true;

		// Token: 0x04000ADC RID: 2780
		private double mProgress = 0.1;

		// Token: 0x04000ADD RID: 2781
		private global::System.Windows.Forms.Timer progressTimer = new global::System.Windows.Forms.Timer();

		// Token: 0x04000ADE RID: 2782
		private bool mForceComplete;

		// Token: 0x04000ADF RID: 2783
		internal string mActionValue;

		// Token: 0x04000AE0 RID: 2784
		internal string mTextOnActionBtn;

		// Token: 0x04000AE1 RID: 2785
		internal bool mIsActionButtonToShow;

		// Token: 0x04000AE2 RID: 2786
		internal PromotionControl PromoControl;

		// Token: 0x04000AE3 RID: 2787
		private int mBootPromotionImageTimeout = 4000;

		// Token: 0x04000AE4 RID: 2788
		internal BootPromotion currentBootPromotion;

		// Token: 0x04000AE5 RID: 2789
		private Thread mSliderAnimationThread;

		// Token: 0x04000AE6 RID: 2790
		private int mThreadId;

		// Token: 0x04000AE7 RID: 2791
		private SerializableDictionary<string, BootPromotion> dictRunningPromotions = new SerializableDictionary<string, BootPromotion>();

		// Token: 0x04000AE8 RID: 2792
		internal static Dictionary<BootPromotion, int> sBootPromotionDisplayed;

		// Token: 0x04000AE9 RID: 2793
		private MainWindow mMainWindow;

		// Token: 0x04000AEA RID: 2794
		internal SerializableDictionary<string, string> mExtraPayloadClicked = new SerializableDictionary<string, string>();

		// Token: 0x04000AEB RID: 2795
		private bool disposedValue;

		// Token: 0x04000AEC RID: 2796
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPromotionImageGrid;

		// Token: 0x04000AED RID: 2797
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPromotionImage;

		// Token: 0x04000AEE RID: 2798
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mPromoButton;

		// Token: 0x04000AEF RID: 2799
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mPromotionInfoBorder;

		// Token: 0x04000AF0 RID: 2800
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mPromoInfoText;

		// Token: 0x04000AF1 RID: 2801
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseButton;

		// Token: 0x04000AF2 RID: 2802
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BlueProgressBar mProgressBar;

		// Token: 0x04000AF3 RID: 2803
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock BootText;

		// Token: 0x04000AF4 RID: 2804
		private bool _contentLoaded;
	}
}
