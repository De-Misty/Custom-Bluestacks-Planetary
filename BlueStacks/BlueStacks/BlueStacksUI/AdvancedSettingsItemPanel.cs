using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000159 RID: 345
	public class AdvancedSettingsItemPanel : UserControl, IComponentConnector
	{
		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0000A8E6 File Offset: 0x00008AE6
		// (set) Token: 0x06000E08 RID: 3592 RVA: 0x0000A8EE File Offset: 0x00008AEE
		public EventHandler Tap
		{
			get
			{
				return this.mTap;
			}
			set
			{
				this.mTap = value;
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x0000A8F7 File Offset: 0x00008AF7
		// (set) Token: 0x06000E0A RID: 3594 RVA: 0x0000A8FF File Offset: 0x00008AFF
		public EventHandler MouseDragStart
		{
			get
			{
				return this.mMouseDragStart;
			}
			set
			{
				this.mMouseDragStart = value;
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x0000A908 File Offset: 0x00008B08
		// (set) Token: 0x06000E0C RID: 3596 RVA: 0x00054E2C File Offset: 0x0005302C
		public KeyActionType ActionType
		{
			get
			{
				return this.mActionType;
			}
			set
			{
				this.mActionType = value;
				this.mImage.ImageName = this.mActionType.ToString() + "_sidebar";
				BlueStacksUIBinding.Bind(this.mActionHeader, Constants.ImapLocaleStringsConstant + this.mActionType.ToString() + "_Header_Edit_UI", "");
			}
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x0000A910 File Offset: 0x00008B10
		public AdvancedSettingsItemPanel()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000E0E RID: 3598 RVA: 0x00054E98 File Offset: 0x00053098
		private void Image_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Hand;
			this.mDragImage.Visibility = Visibility.Visible;
			BlueStacksUIBinding.BindColor(this.mBorder, Control.BorderBrushProperty, "AdvancedGameControlHeaderBackgroundColor");
			this.mBorder.Effect = new DropShadowEffect
			{
				Direction = 270.0,
				ShadowDepth = 3.0,
				BlurRadius = 12.0,
				Opacity = 0.75,
				Color = ((SolidColorBrush)this.mBorder.Background).Color
			};
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x00054F38 File Offset: 0x00053138
		private void Image_MouseLeave(object sender, MouseEventArgs e)
		{
			if (KMManager.sDragCanvasElement == null)
			{
				base.Cursor = Cursors.Arrow;
			}
			this.mDragImage.Visibility = Visibility.Hidden;
			BlueStacksUIBinding.BindColor(this.mBorder, Control.BorderBrushProperty, "AdvancedSettingsItemPanelBorder");
			this.mBorder.Effect = null;
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x0000A91E File Offset: 0x00008B1E
		private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.mousePressedPosition = new Point?(e.GetPosition(this));
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x00054F84 File Offset: 0x00053184
		private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mouseReleasedPosition = new Point?(e.GetPosition(this));
			if (this.mousePressedPosition.Equals(this.mouseReleasedPosition))
			{
				EventHandler tap = this.Tap;
				if (tap != null)
				{
					tap(this, null);
				}
			}
			else
			{
				KMManager.ClearElement();
			}
			this.ReatchedMouseMove();
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x0000A932 File Offset: 0x00008B32
		private void OnTimedElapsed(object sender, ElapsedEventArgs e)
		{
			if (!this.mousePressedPosition.Equals(this.mouseReleasedPosition))
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					EventHandler mouseDragStart = this.MouseDragStart;
					if (mouseDragStart == null)
					{
						return;
					}
					mouseDragStart(this, null);
				}), new object[0]);
			}
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x00054FE4 File Offset: 0x000531E4
		private void Image_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.mousePressedPosition != null && !this.mousePressedPosition.Equals(this.mouseReleasedPosition))
			{
				base.MouseMove -= this.Image_MouseMove;
				EventHandler mouseDragStart = this.MouseDragStart;
				if (mouseDragStart == null)
				{
					return;
				}
				mouseDragStart(this, null);
			}
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x0000A970 File Offset: 0x00008B70
		public void ReatchedMouseMove()
		{
			this.mousePressedPosition = null;
			base.MouseMove -= this.Image_MouseMove;
			base.MouseMove += this.Image_MouseMove;
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x00055040 File Offset: 0x00053240
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/advancedsettingsitempanel.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x00055070 File Offset: 0x00053270
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
				((AdvancedSettingsItemPanel)target).MouseEnter += this.Image_MouseEnter;
				((AdvancedSettingsItemPanel)target).MouseLeave += this.Image_MouseLeave;
				((AdvancedSettingsItemPanel)target).PreviewMouseDown += this.Image_PreviewMouseDown;
				((AdvancedSettingsItemPanel)target).MouseMove += this.Image_MouseMove;
				((AdvancedSettingsItemPanel)target).PreviewMouseUp += this.Image_PreviewMouseUp;
				return;
			case 2:
				this.mBorder = (Border)target;
				return;
			case 3:
				this.mDragImage = (CustomPictureBox)target;
				return;
			case 4:
				this.mImage = (CustomPictureBox)target;
				return;
			case 5:
				this.mActionHeader = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040008EB RID: 2283
		private EventHandler mTap;

		// Token: 0x040008EC RID: 2284
		private EventHandler mMouseDragStart;

		// Token: 0x040008ED RID: 2285
		private KeyActionType mActionType;

		// Token: 0x040008EE RID: 2286
		private Point? mousePressedPosition;

		// Token: 0x040008EF RID: 2287
		private Point? mouseReleasedPosition;

		// Token: 0x040008F0 RID: 2288
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x040008F1 RID: 2289
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mDragImage;

		// Token: 0x040008F2 RID: 2290
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mImage;

		// Token: 0x040008F3 RID: 2291
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mActionHeader;

		// Token: 0x040008F4 RID: 2292
		private bool _contentLoaded;
	}
}
