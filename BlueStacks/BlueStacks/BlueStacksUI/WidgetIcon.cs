using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000263 RID: 611
	public class WidgetIcon : Button, IComponentConnector, IStyleConnector
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06001625 RID: 5669 RVA: 0x0000EDB8 File Offset: 0x0000CFB8
		// (set) Token: 0x06001626 RID: 5670 RVA: 0x0008521C File Offset: 0x0008341C
		public string ImageName
		{
			get
			{
				return this.mImageName;
			}
			set
			{
				this.mImageName = value;
				if (this.mImage != null)
				{
					this.mImage.ImageName = this.mImageName;
				}
				if (this.mBusyImage != null)
				{
					this.mBusyImage.ImageName = this.mImageName + this.mBusyImageNamePostFix;
				}
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06001627 RID: 5671 RVA: 0x0000EDC0 File Offset: 0x0000CFC0
		// (set) Token: 0x06001628 RID: 5672 RVA: 0x0000EDD2 File Offset: 0x0000CFD2
		public string FooterText
		{
			get
			{
				return base.GetValue(WidgetIcon.MyFooterTextProperty) as string;
			}
			set
			{
				base.SetValue(WidgetIcon.MyFooterTextProperty, value);
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0000EDE0 File Offset: 0x0000CFE0
		public WidgetIcon()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0000EDF9 File Offset: 0x0000CFF9
		internal void ShowBusyIcon(Visibility visibility)
		{
			this.mBusyImage.Visibility = visibility;
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x0000EE07 File Offset: 0x0000D007
		private void Image_Initialized(object sender, EventArgs e)
		{
			if (this.mImage == null)
			{
				this.mImage = sender as CustomPictureBox;
			}
			if (!string.IsNullOrEmpty(this.mImageName))
			{
				this.mImage.ImageName = this.mImageName;
			}
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x0000EE3B File Offset: 0x0000D03B
		private void BusyImage_Initialized(object sender, EventArgs e)
		{
			if (this.mBusyImage == null)
			{
				this.mBusyImage = sender as CustomPictureBox;
			}
			if (!string.IsNullOrEmpty(this.mImageName))
			{
				this.mBusyImage.ImageName = this.mImageName + this.mBusyImageNamePostFix;
			}
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x00085270 File Offset: 0x00083470
		private void CustomPictureBox_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (this.mBusyImage.IsVisible)
			{
				if (this.mBusyIconStoryBoard == null)
				{
					this.mBusyIconStoryBoard = new Storyboard();
					DoubleAnimation doubleAnimation = new DoubleAnimation
					{
						From = new double?(0.0),
						To = new double?((double)360),
						RepeatBehavior = RepeatBehavior.Forever,
						Duration = new Duration(new TimeSpan(0, 0, 1))
					};
					Storyboard.SetTarget(doubleAnimation, this.mBusyImage);
					Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)", new object[0]));
					this.mBusyIconStoryBoard.Children.Add(doubleAnimation);
				}
				this.mBusyIconStoryBoard.Begin();
				return;
			}
			this.mBusyIconStoryBoard.Pause();
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x00085338 File Offset: 0x00083538
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/widgeticon.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0000EE7A File Offset: 0x0000D07A
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
				this.mWidgetIcon = (WidgetIcon)target;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x00085368 File Offset: 0x00083568
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((CustomPictureBox)target).Initialized += this.Image_Initialized;
				return;
			}
			if (connectionId != 3)
			{
				return;
			}
			((CustomPictureBox)target).Initialized += this.BusyImage_Initialized;
			((CustomPictureBox)target).IsVisibleChanged += this.CustomPictureBox_IsVisibleChanged;
		}

		// Token: 0x04000D8D RID: 3469
		private CustomPictureBox mImage;

		// Token: 0x04000D8E RID: 3470
		private CustomPictureBox mBusyImage;

		// Token: 0x04000D8F RID: 3471
		private string mBusyImageNamePostFix = "_busy";

		// Token: 0x04000D90 RID: 3472
		private Storyboard mBusyIconStoryBoard;

		// Token: 0x04000D91 RID: 3473
		private string mImageName;

		// Token: 0x04000D92 RID: 3474
		public static readonly DependencyProperty MyFooterTextProperty = DependencyProperty.Register("FooterText", typeof(string), typeof(WidgetIcon));

		// Token: 0x04000D93 RID: 3475
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal WidgetIcon mWidgetIcon;

		// Token: 0x04000D94 RID: 3476
		private bool _contentLoaded;
	}
}
