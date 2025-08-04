using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using BlueStacks.Common;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000A9 RID: 169
	public class Browser : WpfCefBrowser
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x00006832 File Offset: 0x00004A32
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

		// Token: 0x060006E1 RID: 1761 RVA: 0x00026C7C File Offset: 0x00024E7C
		public Browser(float zoomLevel = 0f)
		{
			if (!CefHelper.CefInited)
			{
				string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
				string[] array = new string[0];
				Logger.Info("Init cef");
				CefHelper.InitCef(array, text);
			}
			base.Loaded += this.Browser_Loaded;
			base.LoadingStateChange += this.Browser_LoadingStateChange;
			this.mCustomZoomLevel = zoomLevel;
			if (RegistryManager.Instance.CefDevEnv == 1)
			{
				base.mAllowDevTool = true;
				base.mDevToolHeader = base.StartUrl;
			}
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x00006853 File Offset: 0x00004A53
		public Browser(string url)
		{
			this.url = url;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00026D10 File Offset: 0x00024F10
		private void Browser_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				this.isLoaded = true;
				Matrix transformToDevice = PresentationSource.FromVisual((Visual)sender).CompositionTarget.TransformToDevice;
				ScaleTransform scaleTransform = new ScaleTransform(1.0 / transformToDevice.M11, 1.0 / transformToDevice.M22);
				if (scaleTransform.CanFreeze)
				{
					scaleTransform.Freeze();
				}
				base.LayoutTransform = scaleTransform;
				this.zoomLevel = Math.Log(transformToDevice.M11) / Math.Log(1.2);
				if (this.mCustomZoomLevel != 0f)
				{
					double num = Math.Log10((double)this.mCustomZoomLevel) / Math.Log10(1.2);
					this.zoomLevel += num;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to get zoom factor of browser with url {0} and error :{1}", new object[]
				{
					this.url,
					ex.ToString()
				});
			}
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00026E08 File Offset: 0x00025008
		private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					string text = base.getURL();
					if (!string.IsNullOrEmpty(text))
					{
						string text2 = text.Substring(text.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
						if (text.Contains("play.google.com"))
						{
							this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(text2);
						}
					}
				}
				catch (Exception)
				{
				}
			}), new object[0]);
			if (!e.IsLoading)
			{
				try
				{
					base.SetZoomLevel(this.zoomLevel);
				}
				catch (Exception ex)
				{
					Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", new object[]
					{
						this.url,
						ex.ToString()
					});
				}
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00026E80 File Offset: 0x00025080
		public void CallJs(string methodName, object[] args)
		{
			if (this.isLoaded)
			{
				new Thread(delegate
				{
					try
					{
						if (args.Length == 1)
						{
							string text = args[0].ToString();
							text = text.Replace("%27", "&#39;").Replace("'", "&#39;");
							string text2 = string.Format(CultureInfo.InvariantCulture, "javascript:{0}('{1}')", new object[] { methodName, text });
							Logger.Info("calling " + methodName);
							this.ExecuteJavaScript(text2, this.getURL(), 0);
						}
						else if (args.Length == 0)
						{
							string text3 = string.Format(CultureInfo.InvariantCulture, "javascript:{0}()", new object[] { methodName });
							Logger.Info("calling " + methodName);
							this.ExecuteJavaScript(text3, this.getURL(), 0);
						}
						else
						{
							Logger.Error("Error: function supported for one length array object to be changed later");
						}
					}
					catch (Exception ex)
					{
						Logger.Error(ex.ToString());
					}
				})
				{
					IsBackground = true
				}.Start();
			}
		}

		// Token: 0x040003A0 RID: 928
		private bool isLoaded;

		// Token: 0x040003A1 RID: 929
		private string url;

		// Token: 0x040003A2 RID: 930
		private double zoomLevel;

		// Token: 0x040003A3 RID: 931
		private float mCustomZoomLevel;

		// Token: 0x040003A4 RID: 932
		private MainWindow mMainWindow;
	}
}
