using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200022C RID: 556
	public partial class ContainerWindow : CustomWindow
	{
		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001534 RID: 5428 RVA: 0x0000E7E8 File Offset: 0x0000C9E8
		// (set) Token: 0x06001535 RID: 5429 RVA: 0x0000E7FA File Offset: 0x0000C9FA
		public CornerRadius CustomCornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(ContainerWindow.CustomCornerRadiusProperty);
			}
			set
			{
				base.SetValue(ContainerWindow.CustomCornerRadiusProperty, value);
				this.IsCustomCornerRadius = true;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x0000E814 File Offset: 0x0000CA14
		// (set) Token: 0x06001537 RID: 5431 RVA: 0x0000E826 File Offset: 0x0000CA26
		public Brush CustomBorderBrush
		{
			get
			{
				return (Brush)base.GetValue(ContainerWindow.CustomBorderBrushProperty);
			}
			set
			{
				base.SetValue(ContainerWindow.CustomBorderBrushProperty, value);
				this.IsCustomBorderBrush = true;
			}
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0007ED4C File Offset: 0x0007CF4C
		public ContainerWindow(MainWindow window, UserControl control, double width, double height, bool autoHeight = false, bool isShow = true, bool isWindowTransparent = false, double radius = -1.0, Brush brush = null)
		{
			ContainerWindow <>4__this = this;
			this.InitializeComponent();
			base.Closing += delegate(object o, CancelEventArgs e)
			{
				<>4__this.ClosingContainerWindow(o, e, control);
			};
			if (radius != -1.0)
			{
				this.CustomCornerRadius = new CornerRadius(radius);
			}
			if (brush != null)
			{
				this.CustomBorderBrush = brush;
			}
			if (!isWindowTransparent)
			{
				this.SetShadowBorder();
				this.SetOuterBorder();
				this.SetMaskGrid();
			}
			if (autoHeight)
			{
				base.Width = width + (double)(isWindowTransparent ? 0 : 64);
				base.SizeToContent = SizeToContent.Height;
			}
			else
			{
				base.Width = width + (double)(isWindowTransparent ? 0 : 64);
				base.Height = height + (double)(isWindowTransparent ? 0 : 64);
			}
			base.Owner = window;
			if (window != null)
			{
				if (window.mDMMRecommendedWindow != null && window.mDMMRecommendedWindow.Visibility == Visibility.Visible && window.IsUIInPortraitMode)
				{
					double num = (window.Width + window.mDMMRecommendedWindow.Width - base.Width) / 2.0 + window.Left;
					double num2 = (window.Height - base.Height) / 2.0 + window.Top;
					double num3 = num + base.Width;
					double num4 = num2 + base.Height;
					if (num > 0.0 && num3 < SystemParameters.PrimaryScreenWidth && num2 > 0.0 && num4 < SystemParameters.PrimaryScreenHeight)
					{
						base.Left = num;
						base.Top = num2;
					}
					else
					{
						base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					}
				}
				else if (window.WindowState == WindowState.Minimized)
				{
					base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				}
				else
				{
					base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				}
				this.ContentGrid.Children.Add(control);
				if (isShow)
				{
					if (window != null)
					{
						window.ShowDimOverlay(null);
						base.Owner = window.mDimOverlay;
					}
					base.ShowDialog();
					if (window != null)
					{
						window.HideDimOverlay();
					}
				}
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0007EF38 File Offset: 0x0007D138
		private void ClosingContainerWindow(object _1, CancelEventArgs _2, UserControl control)
		{
			IDimOverlayControl dimOverlayControl = control as IDimOverlayControl;
			if (dimOverlayControl != null)
			{
				dimOverlayControl.Close();
			}
			if (control != null)
			{
				this.ContentGrid.Children.Remove(control);
			}
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x0007EF6C File Offset: 0x0007D16C
		private void SetShadowBorder()
		{
			this.mShadowBorder.SnapsToDevicePixels = true;
			this.mShadowBorder.BorderThickness = new Thickness(1.0);
			this.mShadowBorder.Margin = new Thickness(30.0);
			this.mShadowBorder.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
			if (this.IsCustomCornerRadius)
			{
				this.mShadowBorder.CornerRadius = this.CustomCornerRadius;
			}
			else
			{
				BlueStacksUIBinding.BindCornerRadius(this.mShadowBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
			}
			DropShadowEffect dropShadowEffect = new DropShadowEffect();
			BlueStacksUIBinding.BindColor(dropShadowEffect, DropShadowEffect.ColorProperty, "PopupShadowColor");
			dropShadowEffect.Direction = 270.0;
			dropShadowEffect.ShadowDepth = 0.0;
			dropShadowEffect.BlurRadius = 15.0;
			dropShadowEffect.Opacity = 0.7;
			this.mShadowBorder.Effect = dropShadowEffect;
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x0007F05C File Offset: 0x0007D25C
		private void SetOuterBorder()
		{
			this.mOuterBorder.BorderThickness = new Thickness(1.0);
			BlueStacksUIBinding.BindColor(this.mOuterBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
			if (this.IsCustomBorderBrush)
			{
				this.mOuterBorder.BorderBrush = this.CustomBorderBrush;
			}
			else
			{
				BlueStacksUIBinding.BindColor(this.mOuterBorder, Control.BorderBrushProperty, "PopupBorderBrush");
			}
			if (this.IsCustomCornerRadius)
			{
				this.mOuterBorder.CornerRadius = this.CustomCornerRadius;
				return;
			}
			BlueStacksUIBinding.BindCornerRadius(this.mOuterBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x0007F0F8 File Offset: 0x0007D2F8
		private void SetMaskGrid()
		{
			this.mMaskBorder.SnapsToDevicePixels = true;
			this.mMaskBorder.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
			BlueStacksUIBinding.BindColor(this.mMaskBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
			if (this.IsCustomCornerRadius)
			{
				this.mMaskBorder.CornerRadius = this.CustomCornerRadius;
			}
			else
			{
				BlueStacksUIBinding.BindCornerRadius(this.mMaskBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
			}
			VisualBrush visualBrush = new VisualBrush(this.mMaskBorder)
			{
				Stretch = Stretch.None
			};
			this.mMainGrid.OpacityMask = visualBrush;
		}

		// Token: 0x04000D05 RID: 3333
		public static readonly DependencyProperty CustomCornerRadiusProperty = DependencyProperty.Register("CustomCornerRadius", typeof(CornerRadius), typeof(CustomWindow), new PropertyMetadata(new CornerRadius(-1.0)));

		// Token: 0x04000D06 RID: 3334
		private bool IsCustomCornerRadius;

		// Token: 0x04000D07 RID: 3335
		public static readonly DependencyProperty CustomBorderBrushProperty = DependencyProperty.Register("CustomBorderBrush", typeof(Brush), typeof(Border), new PropertyMetadata(Brushes.Transparent));

		// Token: 0x04000D08 RID: 3336
		private bool IsCustomBorderBrush;
	}
}
