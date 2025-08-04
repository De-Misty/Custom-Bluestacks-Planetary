using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000EB RID: 235
	public class ImageTranslateControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00008324 File Offset: 0x00006524
		// (set) Token: 0x060009C9 RID: 2505 RVA: 0x0000832B File Offset: 0x0000652B
		public static ImageTranslateControl Instance { get; private set; }

		// Token: 0x060009CA RID: 2506 RVA: 0x00036F3C File Offset: 0x0003513C
		public ImageTranslateControl(MainWindow parentWindow)
		{
			this.InitializeComponent();
			ImageTranslateControl.Instance = this;
			this.ParentWindow = parentWindow;
			if (this.ParentWindow != null)
			{
				base.Width = parentWindow.FrontendParentGrid.ActualWidth;
				base.Height = parentWindow.FrontendParentGrid.ActualHeight;
			}
			this.mLoadingImage.Visibility = Visibility.Visible;
			this.mFrontEndImage.Visibility = Visibility.Collapsed;
			this.mTopBar.Visibility = Visibility.Collapsed;
			this.mBootText.Visibility = Visibility.Visible;
			this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_LOADING_MESSAGE", "");
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00008333 File Offset: 0x00006533
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Window.GetWindow(this).KeyDown += this.UserControl_KeyDown;
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00036FE4 File Offset: 0x000351E4
		public void GetTranslateImage(Bitmap bitmap)
		{
			if (bitmap != null)
			{
				this.httpBackGroundThread = new Thread(delegate
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						try
						{
							bitmap.Save(memoryStream, ImageFormat.Jpeg);
							memoryStream.Position = 0L;
							Dictionary<string, object> dictionary = new Dictionary<string, object>();
							string text = RegistryManager.Instance.UserSelectedLocale;
							text = text.Substring(0, 2);
							if (string.Equals(text, "zh-CN", StringComparison.InvariantCulture) || string.Equals(text, "zh-TW", StringComparison.InvariantCulture))
							{
								text = RegistryManager.Instance.UserSelectedLocale;
							}
							if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocale))
							{
								text = RegistryManager.Instance.TargetLocale;
							}
							dictionary.Add("locale", text);
							dictionary.Add("inputImage", new FormFile
							{
								Name = "image.jpg",
								ContentType = "image/jpeg",
								Stream = memoryStream
							});
							dictionary.Add("oem", RegistryManager.Instance.Oem);
							dictionary.Add("guid", RegistryManager.Instance.UserGuid);
							dictionary.Add("prod_ver", RegistryManager.Instance.ClientVersion);
							string text2 = Convert.ToBase64String(memoryStream.ToArray());
							text2 = text2 + RegistryManager.Instance.UserGuid + "BstTranslate";
							_MD5 md = new _MD5
							{
								Value = text2
							};
							dictionary.Add("token", md.FingerPrint);
							string text3 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
							{
								RegistryManager.Instance.Host,
								"/translate/postimage"
							});
							if (!string.IsNullOrEmpty(RegistryManager.Instance.TargetLocaleUrl))
							{
								text3 = RegistryManager.Instance.TargetLocaleUrl;
							}
							string text4 = string.Empty;
							byte[] dataArray = null;
							try
							{
								text4 = BstHttpClient.PostMultipart(text3, dictionary, out dataArray);
							}
							catch (Exception ex)
							{
								Logger.Error("error while downloading translated image.." + ex.ToString());
								text4 = "error";
							}
							if (text4.Contains("error"))
							{
								this.Dispatcher.Invoke(new Action(delegate
								{
									this.mLoadingImage.Visibility = Visibility.Collapsed;
									this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
								}), new object[0]);
							}
							else
							{
								this.Dispatcher.Invoke(new Action(delegate
								{
									BitmapImage bitmapImage = ImageUtils.ByteArrayToImage(dataArray);
									this.mFrontEndImage.Source = bitmapImage;
									this.mFrontEndImage.ReloadImages();
									this.mFrontEndImage.Visibility = Visibility.Visible;
									this.mTopBar.Visibility = Visibility.Visible;
									this.mLoadingImage.Visibility = Visibility.Collapsed;
									this.mBootText.Visibility = Visibility.Collapsed;
								}), new object[0]);
							}
						}
						catch (Exception ex2)
						{
							string text5 = "Error in GetTranslateImage ";
							Exception ex3 = ex2;
							Logger.Error(text5 + ((ex3 != null) ? ex3.ToString() : null));
							this.Dispatcher.Invoke(new Action(delegate
							{
								this.mLoadingImage.Visibility = Visibility.Collapsed;
								this.mBootText.Text = LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", "");
							}), new object[0]);
						}
					}
				})
				{
					IsBackground = true
				};
				this.httpBackGroundThread.Start();
			}
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00037038 File Offset: 0x00035238
		public bool Close()
		{
			try
			{
				ImageTranslateControl.Instance = null;
				Thread thread = this.httpBackGroundThread;
				if (thread != null)
				{
					thread.Abort();
				}
				MainWindow parentWindow = this.ParentWindow;
				if (parentWindow != null)
				{
					parentWindow.HideDimOverlay();
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close imagetranslateontrol from dimoverlay " + ex.ToString());
			}
			return false;
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0000834C File Offset: 0x0000654C
		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
			}
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0000835F File Offset: 0x0000655F
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x00005AAF File Offset: 0x00003CAF
		// (set) Token: 0x060009D1 RID: 2513 RVA: 0x00004786 File Offset: 0x00002986
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060009D2 RID: 2514 RVA: 0x00008368 File Offset: 0x00006568
		// (set) Token: 0x060009D3 RID: 2515 RVA: 0x00008370 File Offset: 0x00006570
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060009D4 RID: 2516 RVA: 0x00008379 File Offset: 0x00006579
		// (set) Token: 0x060009D5 RID: 2517 RVA: 0x00008381 File Offset: 0x00006581
		public bool ShowTransparentWindow { get; set; } = true;

		// Token: 0x060009D6 RID: 2518 RVA: 0x0000838A File Offset: 0x0000658A
		bool IDimOverlayControl.Close()
		{
			this.Close();
			return true;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0003709C File Offset: 0x0003529C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/imagetranslatecontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x000370CC File Offset: 0x000352CC
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
				((ImageTranslateControl)target).Loaded += this.UserControl_Loaded;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				this.mTopBar = (Grid)target;
				return;
			case 4:
				this.mTitleLabel = (Label)target;
				return;
			case 5:
				this.mCloseButton = (CustomPictureBox)target;
				this.mCloseButton.MouseLeftButtonUp += this.CloseButton_MouseLeftButtonUp;
				return;
			case 6:
				this.mFrontEndImage = (CustomPictureBox)target;
				return;
			case 7:
				this.mLoadingImage = (CustomPictureBox)target;
				return;
			case 8:
				this.mBootText = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040005A5 RID: 1445
		private MainWindow ParentWindow;

		// Token: 0x040005A6 RID: 1446
		private Thread httpBackGroundThread;

		// Token: 0x040005AA RID: 1450
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x040005AB RID: 1451
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTopBar;

		// Token: 0x040005AC RID: 1452
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mTitleLabel;

		// Token: 0x040005AD RID: 1453
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseButton;

		// Token: 0x040005AE RID: 1454
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mFrontEndImage;

		// Token: 0x040005AF RID: 1455
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mLoadingImage;

		// Token: 0x040005B0 RID: 1456
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mBootText;

		// Token: 0x040005B1 RID: 1457
		private bool _contentLoaded;
	}
}
