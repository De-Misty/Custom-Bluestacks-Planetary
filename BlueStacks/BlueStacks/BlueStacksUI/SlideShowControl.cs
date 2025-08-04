using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000129 RID: 297
	public class SlideShowControl : global::System.Windows.Controls.UserControl, IDisposable, IComponentConnector
	{
		// Token: 0x06000BF0 RID: 3056 RVA: 0x00009832 File Offset: 0x00007A32
		public SlideShowControl()
		{
			this.InitializeComponent();
			this.image1_MouseEnter(null, null);
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x00042C34 File Offset: 0x00040E34
		internal void AddOrUpdateSlide(SlideShowControl.SlideShowContext slideContext)
		{
			if (this.mSlideShowDict.ContainsKey(slideContext.Key))
			{
				this.mSlideShowDict[slideContext.Key] = slideContext;
				return;
			}
			this.mSlideShowDict.Add((slideContext.Key == 0) ? this.mSlideShowDict.Count : slideContext.Key, slideContext);
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00009853 File Offset: 0x00007A53
		// (set) Token: 0x06000BF3 RID: 3059 RVA: 0x00009865 File Offset: 0x00007A65
		public global::System.Windows.HorizontalAlignment TextHorizontalAlignment
		{
			get
			{
				return (global::System.Windows.HorizontalAlignment)base.GetValue(SlideShowControl.TextHorizontalAlignmentProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.TextHorizontalAlignmentProperty, value);
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00009878 File Offset: 0x00007A78
		// (set) Token: 0x06000BF5 RID: 3061 RVA: 0x0000988A File Offset: 0x00007A8A
		public VerticalAlignment TextVerticalAlignment
		{
			get
			{
				return (VerticalAlignment)base.GetValue(SlideShowControl.TextVerticalAlignmentProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.TextVerticalAlignmentProperty, value);
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x0000989D File Offset: 0x00007A9D
		// (set) Token: 0x06000BF7 RID: 3063 RVA: 0x000098AF File Offset: 0x00007AAF
		public string ImagesFolderPath
		{
			get
			{
				return (string)base.GetValue(SlideShowControl.ImagesFolderPathProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.ImagesFolderPathProperty, value);
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x000098BD File Offset: 0x00007ABD
		// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x000098CF File Offset: 0x00007ACF
		public bool IsArrowVisible
		{
			get
			{
				return (bool)base.GetValue(SlideShowControl.IsArrowVisibleProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.IsArrowVisibleProperty, value);
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x000098E2 File Offset: 0x00007AE2
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x000098F4 File Offset: 0x00007AF4
		public bool HideArrowOnLeave
		{
			get
			{
				return (bool)base.GetValue(SlideShowControl.HideArrowOnLeaveProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.HideArrowOnLeaveProperty, value);
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x00009907 File Offset: 0x00007B07
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x00009919 File Offset: 0x00007B19
		public bool IsAutoPlay
		{
			get
			{
				return (bool)base.GetValue(SlideShowControl.IsAutoPlayProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.IsAutoPlayProperty, value);
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0000992C File Offset: 0x00007B2C
		// (set) Token: 0x06000BFF RID: 3071 RVA: 0x0000993E File Offset: 0x00007B3E
		public int SlideDelay
		{
			get
			{
				return (int)base.GetValue(SlideShowControl.SlideDelayProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.SlideDelayProperty, value);
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000C00 RID: 3072 RVA: 0x00009951 File Offset: 0x00007B51
		// (set) Token: 0x06000C01 RID: 3073 RVA: 0x00009963 File Offset: 0x00007B63
		public SlideShowControl.SlideAnimationType TransitionType
		{
			get
			{
				return (SlideShowControl.SlideAnimationType)base.GetValue(SlideShowControl.TransitionTypeProperty);
			}
			set
			{
				base.SetValue(SlideShowControl.TransitionTypeProperty, value);
			}
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x00009976 File Offset: 0x00007B76
		internal void PlaySlideShow()
		{
			this.IsAutoPlay = true;
			this.SlideShowLoop(false);
			this.StartImageTransition(this._slide + 1);
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x00009994 File Offset: 0x00007B94
		internal void StopSlideShow()
		{
			this.IsAutoPlay = false;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00042C90 File Offset: 0x00040E90
		internal void LoadImagesFromFolder(string folderPath)
		{
			if (!Path.IsPathRooted(folderPath))
			{
				folderPath = Path.Combine(CustomPictureBox.AssetsDir, folderPath);
			}
			if (Directory.Exists(folderPath))
			{
				try
				{
					string text = Path.Combine(folderPath, "slides.json");
					if (File.Exists(text))
					{
						IEnumerable<SlideShowControl.SlideShowContext> enumerable = JObject.Parse(File.ReadAllText(text)).ToObject<IEnumerable<SlideShowControl.SlideShowContext>>();
						if (enumerable != null)
						{
							foreach (SlideShowControl.SlideShowContext slideShowContext in enumerable)
							{
								if (!string.IsNullOrEmpty(slideShowContext.Description))
								{
									slideShowContext.Description = LocaleStrings.GetLocalizedString(slideShowContext.Description, "");
								}
								this.AddOrUpdateSlide(slideShowContext);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Error while trying to read slides.json from " + folderPath + "." + ex.ToString());
					this.mSlideShowDict.Clear();
				}
				if (this.mSlideShowDict.Count == 0)
				{
					FileInfo[] files = new DirectoryInfo(folderPath).GetFiles();
					int num = 0;
					for (int i = 0; i < files.Length; i++)
					{
						if (SlideShowControl.ValidImageExtensions.Contains(files[i].Extension, StringComparer.InvariantCultureIgnoreCase))
						{
							this.AddOrUpdateSlide(new SlideShowControl.SlideShowContext
							{
								Key = num,
								ImageName = files[i].FullName
							});
							num++;
						}
					}
				}
				this.StartImageTransition(0);
			}
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x00042DF8 File Offset: 0x00040FF8
		private void SlideShowLoop(bool forceStart = false)
		{
			if (forceStart && this.timer != null)
			{
				this.timer.Enabled = false;
			}
			if (this.timer != null && !this.timer.Enabled)
			{
				this.timer.Dispose();
			}
			if (this.IsAutoPlay && this.mSlideShowDict.Count > 1)
			{
				this.timer = new Timer
				{
					Interval = this.SlideDelay * 1000
				};
				this.timer.Tick += this.Timer_Tick;
				this.timer.Start();
			}
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00042E94 File Offset: 0x00041094
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (this.timer.Enabled && this.IsAutoPlay && this.mSlideShowDict.Count > 1 && sender == this.timer)
			{
				this.StartImageTransition(this._slide + 1);
				return;
			}
			((Timer)sender).Enabled = false;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x00042EE8 File Offset: 0x000410E8
		private void StartImageTransition(int i)
		{
			if (this.mSlideShowDict.Count > 0)
			{
				if (this._slide == i)
				{
					this.image1.ImageName = this.mSlideShowDict[this._slide].ImageName;
					this.SlideshowName.Text = this.mSlideShowDict[this._slide].Description;
					this.SlideShowLoop(false);
					return;
				}
				if (i >= this.mSlideShowDict.Count)
				{
					this.UnloadImage(0);
					return;
				}
				if (i < 0)
				{
					this.UnloadImage(this.mSlideShowDict.Count - 1);
					return;
				}
				this.UnloadImage(i);
			}
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x00042F90 File Offset: 0x00041190
		private void UnloadImage(int imageToShow)
		{
			Storyboard storyboard = (base.Resources[string.Format(CultureInfo.InvariantCulture, "{0}Out", new object[] { this.TransitionType.ToString() })] as Storyboard).Clone();
			storyboard.Completed += delegate(object o, EventArgs e)
			{
				this.image1.ImageName = this.mSlideShowDict[imageToShow].ImageName;
				this.LoadImage(imageToShow);
			};
			Storyboard.SetTarget(storyboard, this.SlideshowGrid);
			storyboard.Begin();
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x00043018 File Offset: 0x00041218
		private void LoadImage(int imageToShow)
		{
			this._slide = imageToShow;
			this.SlideshowName.Text = this.mSlideShowDict[imageToShow].Description;
			Storyboard storyboard = base.Resources[string.Format(CultureInfo.InvariantCulture, "{0}In", new object[] { this.TransitionType.ToString() })] as Storyboard;
			Storyboard.SetTarget(storyboard, this.SlideshowGrid);
			storyboard.Begin();
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0000999D File Offset: 0x00007B9D
		private void mPrevBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.SlideShowLoop(true);
			this.StartImageTransition(this._slide - 1);
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x000099B4 File Offset: 0x00007BB4
		private void mNextBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.SlideShowLoop(true);
			this.StartImageTransition(this._slide + 1);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x000099CB File Offset: 0x00007BCB
		private void SlideShowControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.ImagesFolderPath))
			{
				this.LoadImagesFromFolder(this.ImagesFolderPath);
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00043098 File Offset: 0x00041298
		private void image1_MouseEnter(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (!this.IsArrowVisible || this.mSlideShowDict.Count < 2)
			{
				this.image1_MouseLeave(sender, e);
				return;
			}
			if (!this.HideArrowOnLeave)
			{
				return;
			}
			if (this.image1.IsMouseOver)
			{
				this.mPrevBtn.Visibility = Visibility.Visible;
				this.mNextBtn.Visibility = Visibility.Visible;
				return;
			}
			this.image1_MouseLeave(sender, e);
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x000430FC File Offset: 0x000412FC
		private void image1_MouseLeave(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (!this.IsArrowVisible || this.mSlideShowDict.Count < 2)
			{
				this.mPrevBtn.Visibility = Visibility.Hidden;
				this.mNextBtn.Visibility = Visibility.Hidden;
				return;
			}
			if (!this.HideArrowOnLeave)
			{
				return;
			}
			if (!this.mPrevBtn.IsMouseOver && !this.mNextBtn.IsMouseOver && !this.image1.IsMouseOver)
			{
				this.mPrevBtn.Visibility = Visibility.Hidden;
				this.mNextBtn.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x000099E6 File Offset: 0x00007BE6
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.timer != null)
				{
					this.timer.Tick += this.Timer_Tick;
					this.timer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00043180 File Offset: 0x00041380
		~SlideShowControl()
		{
			this.Dispose(false);
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00009A23 File Offset: 0x00007C23
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x000431B0 File Offset: 0x000413B0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/slideshowcontrol.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x000431E0 File Offset: 0x000413E0
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
				this.slideControl = (SlideShowControl)target;
				this.slideControl.Loaded += this.SlideShowControl_Loaded;
				return;
			case 2:
				this.SlideshowGrid = (Grid)target;
				return;
			case 3:
				this.image1 = (CustomPictureBox)target;
				this.image1.MouseEnter += this.image1_MouseEnter;
				this.image1.MouseLeave += this.image1_MouseLeave;
				return;
			case 4:
				this.mPrevBtn = (CustomPictureBox)target;
				this.mPrevBtn.MouseLeftButtonUp += this.mPrevBtn_MouseLeftButtonUp;
				this.mPrevBtn.MouseLeave += this.image1_MouseLeave;
				return;
			case 5:
				this.mNextBtn = (CustomPictureBox)target;
				this.mNextBtn.MouseLeftButtonUp += this.mNextBtn_MouseLeftButtonUp;
				this.mNextBtn.MouseLeave += this.image1_MouseLeave;
				return;
			case 6:
				this.SlideshowName = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000746 RID: 1862
		private SortedDictionary<int, SlideShowControl.SlideShowContext> mSlideShowDict = new SortedDictionary<int, SlideShowControl.SlideShowContext>();

		// Token: 0x04000747 RID: 1863
		private static string[] ValidImageExtensions = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

		// Token: 0x04000748 RID: 1864
		public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register("TransitionType", typeof(SlideShowControl.SlideAnimationType), typeof(SlideShowControl), new PropertyMetadata(SlideShowControl.SlideAnimationType.Fade));

		// Token: 0x04000749 RID: 1865
		public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register("TextVerticalAlignment", typeof(VerticalAlignment), typeof(SlideShowControl), new PropertyMetadata(VerticalAlignment.Bottom));

		// Token: 0x0400074A RID: 1866
		public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register("TextHorizontalAlignment", typeof(global::System.Windows.HorizontalAlignment), typeof(SlideShowControl), new PropertyMetadata(global::System.Windows.HorizontalAlignment.Center));

		// Token: 0x0400074B RID: 1867
		public static readonly DependencyProperty IsAutoPlayProperty = DependencyProperty.Register("IsAutoPlay", typeof(bool), typeof(SlideShowControl), new PropertyMetadata(false));

		// Token: 0x0400074C RID: 1868
		public static readonly DependencyProperty HideArrowOnLeaveProperty = DependencyProperty.Register("HideArrowOnLeave", typeof(bool), typeof(SlideShowControl), new PropertyMetadata(true));

		// Token: 0x0400074D RID: 1869
		public static readonly DependencyProperty IsArrowVisibleProperty = DependencyProperty.Register("IsArrowVisible", typeof(bool), typeof(SlideShowControl), new PropertyMetadata(true));

		// Token: 0x0400074E RID: 1870
		public static readonly DependencyProperty SlideDelayProperty = DependencyProperty.Register("SlideDelay", typeof(int), typeof(SlideShowControl), new PropertyMetadata(5));

		// Token: 0x0400074F RID: 1871
		public static readonly DependencyProperty ImagesFolderPathProperty = DependencyProperty.Register("ImagesFolderPath", typeof(string), typeof(SlideShowControl), new PropertyMetadata(""));

		// Token: 0x04000750 RID: 1872
		private int _slide;

		// Token: 0x04000751 RID: 1873
		private Timer timer;

		// Token: 0x04000752 RID: 1874
		private bool disposedValue;

		// Token: 0x04000753 RID: 1875
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SlideShowControl slideControl;

		// Token: 0x04000754 RID: 1876
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid SlideshowGrid;

		// Token: 0x04000755 RID: 1877
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox image1;

		// Token: 0x04000756 RID: 1878
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPrevBtn;

		// Token: 0x04000757 RID: 1879
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mNextBtn;

		// Token: 0x04000758 RID: 1880
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock SlideshowName;

		// Token: 0x04000759 RID: 1881
		private bool _contentLoaded;

		// Token: 0x0200012A RID: 298
		[JsonObject(MemberSerialization.OptIn)]
		internal class SlideShowContext
		{
			// Token: 0x0400075A RID: 1882
			[JsonProperty("key")]
			internal int Key;

			// Token: 0x0400075B RID: 1883
			[JsonProperty("imagename")]
			internal string ImageName;

			// Token: 0x0400075C RID: 1884
			[JsonProperty("description")]
			internal string Description;
		}

		// Token: 0x0200012B RID: 299
		public enum SlideAnimationType
		{
			// Token: 0x0400075E RID: 1886
			Fade,
			// Token: 0x0400075F RID: 1887
			Slide
		}
	}
}
