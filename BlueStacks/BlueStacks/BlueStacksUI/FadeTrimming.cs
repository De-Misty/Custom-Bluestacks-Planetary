using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000137 RID: 311
	public static class FadeTrimming
	{
		// Token: 0x06000C84 RID: 3204 RVA: 0x00009EA8 File Offset: 0x000080A8
		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)((obj != null) ? obj.GetValue(FadeTrimming.IsEnabledProperty) : null);
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x00009EC0 File Offset: 0x000080C0
		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			if (obj != null)
			{
				obj.SetValue(FadeTrimming.IsEnabledProperty, value);
			}
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x00009ED6 File Offset: 0x000080D6
		public static void SetIsVerticalFadingEnabled(DependencyObject obj, bool value)
		{
			if (obj != null)
			{
				obj.SetValue(FadeTrimming.IsVerticalFadingEnabledProperty, value);
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x00009EEC File Offset: 0x000080EC
		private static FadeTrimming.Fader GetFader(DependencyObject obj)
		{
			return (FadeTrimming.Fader)obj.GetValue(FadeTrimming.FaderProperty);
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x00009EFE File Offset: 0x000080FE
		private static void SetFader(DependencyObject obj, FadeTrimming.Fader value)
		{
			obj.SetValue(FadeTrimming.FaderProperty, value);
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x00045B24 File Offset: 0x00043D24
		private static void HandleVerticalFadingEnabled(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			TextBlock textBlock = source as TextBlock;
			if (textBlock != null)
			{
				FadeTrimming.Fader fader = FadeTrimming.GetFader(textBlock);
				if (fader != null)
				{
					fader.ToggleVerticalFading((bool)e.NewValue);
				}
			}
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x00045B58 File Offset: 0x00043D58
		private static void HandleIsEnabledChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			TextBlock textBlock = source as TextBlock;
			if (textBlock != null)
			{
				if ((bool)e.OldValue)
				{
					FadeTrimming.Fader fader = FadeTrimming.GetFader(textBlock);
					if (fader != null)
					{
						fader.Detach();
						FadeTrimming.SetFader(textBlock, null);
					}
					textBlock.Loaded -= FadeTrimming.HandleTextBlockLoaded;
					textBlock.Unloaded -= FadeTrimming.HandleTextBlockUnloaded;
				}
				if ((bool)e.NewValue)
				{
					textBlock.Loaded += FadeTrimming.HandleTextBlockLoaded;
					textBlock.Unloaded += FadeTrimming.HandleTextBlockUnloaded;
					FadeTrimming.Fader fader2 = new FadeTrimming.Fader(textBlock);
					FadeTrimming.SetFader(textBlock, fader2);
					fader2.Attach();
				}
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x00045C04 File Offset: 0x00043E04
		private static void HandleTextBlockUnloaded(object sender, RoutedEventArgs e)
		{
			DependencyObject dependencyObject = sender as DependencyObject;
			if (dependencyObject != null)
			{
				FadeTrimming.Fader fader = FadeTrimming.GetFader(dependencyObject);
				if (fader != null)
				{
					fader.Detach();
				}
			}
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x00045C2C File Offset: 0x00043E2C
		private static void HandleTextBlockLoaded(object sender, RoutedEventArgs e)
		{
			DependencyObject dependencyObject = sender as DependencyObject;
			if (dependencyObject != null)
			{
				FadeTrimming.Fader fader = FadeTrimming.GetFader(dependencyObject);
				if (fader != null)
				{
					fader.Attach();
				}
			}
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x00045C54 File Offset: 0x00043E54
		private static bool HorizontalBrushNeedsUpdating(LinearGradientBrush brush, double visibleWidth)
		{
			return brush.EndPoint.X < visibleWidth - 1E-05 || brush.EndPoint.X > visibleWidth + 1E-05;
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x00045C9C File Offset: 0x00043E9C
		private static bool VerticalBrushNeedsUpdating(LinearGradientBrush brush, double visibleHeight)
		{
			return brush.EndPoint.Y < visibleHeight - 1E-05 || brush.EndPoint.Y > visibleHeight + 1E-05;
		}

		// Token: 0x040007AA RID: 1962
		private const double Epsilon = 1E-05;

		// Token: 0x040007AB RID: 1963
		private const double FadeWidth = 10.0;

		// Token: 0x040007AC RID: 1964
		private const double FadeHeight = 20.0;

		// Token: 0x040007AD RID: 1965
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FadeTrimming), new PropertyMetadata(false, new PropertyChangedCallback(FadeTrimming.HandleIsEnabledChanged)));

		// Token: 0x040007AE RID: 1966
		private static readonly DependencyProperty FaderProperty = DependencyProperty.RegisterAttached("Fader", typeof(FadeTrimming.Fader), typeof(FadeTrimming), new PropertyMetadata(null));

		// Token: 0x040007AF RID: 1967
		public static readonly DependencyProperty IsVerticalFadingEnabledProperty = DependencyProperty.RegisterAttached("IsVerticalFadingEnabledProperty", typeof(bool), typeof(FadeTrimming), new PropertyMetadata(false, new PropertyChangedCallback(FadeTrimming.HandleVerticalFadingEnabled)));

		// Token: 0x02000138 RID: 312
		private class Fader
		{
			// Token: 0x06000C90 RID: 3216 RVA: 0x00009F0C File Offset: 0x0000810C
			public Fader(TextBlock textBlock)
			{
				this._textBlock = textBlock;
			}

			// Token: 0x06000C91 RID: 3217 RVA: 0x00045D90 File Offset: 0x00043F90
			public void Attach()
			{
				FrameworkElement frameworkElement = VisualTreeHelper.GetParent(this._textBlock) as FrameworkElement;
				if (frameworkElement != null && !this._isAttached)
				{
					frameworkElement.SizeChanged += new SizeChangedEventHandler(this.UpdateForegroundBrush);
					this._textBlock.SizeChanged += new SizeChangedEventHandler(this.UpdateForegroundBrush);
					this._opacityMask = this._textBlock.OpacityMask;
					if (this._verticalFadingEnabled || this._textBlock.TextWrapping == TextWrapping.NoWrap)
					{
						this._textBlock.TextTrimming = TextTrimming.None;
					}
					this.UpdateForegroundBrush(this._textBlock, EventArgs.Empty);
					this._isAttached = true;
				}
			}

			// Token: 0x06000C92 RID: 3218 RVA: 0x00045E30 File Offset: 0x00044030
			public void Detach()
			{
				this._textBlock.SizeChanged -= new SizeChangedEventHandler(this.UpdateForegroundBrush);
				FrameworkElement frameworkElement = VisualTreeHelper.GetParent(this._textBlock) as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.SizeChanged -= new SizeChangedEventHandler(this.UpdateForegroundBrush);
				}
				this._textBlock.OpacityMask = this._opacityMask;
				this._isAttached = false;
			}

			// Token: 0x06000C93 RID: 3219 RVA: 0x00009F1B File Offset: 0x0000811B
			public void ToggleVerticalFading(bool newValue)
			{
				this._verticalFadingEnabled = newValue;
				this.UpdateForegroundBrush(this._textBlock, EventArgs.Empty);
			}

			// Token: 0x06000C94 RID: 3220 RVA: 0x00045E94 File Offset: 0x00044094
			private void UpdateForegroundBrush(object sender, EventArgs e)
			{
				Geometry layoutClip = LayoutInformation.GetLayoutClip(this._textBlock);
				bool flag = layoutClip != null && ((this._textBlock.TextWrapping == TextWrapping.NoWrap && layoutClip.Bounds.Width > 0.0 && layoutClip.Bounds.Width < this._textBlock.ActualWidth) || (this._verticalFadingEnabled && this._textBlock.TextWrapping == TextWrapping.Wrap && layoutClip.Bounds.Height > 0.0 && layoutClip.Bounds.Height < this._textBlock.ActualHeight));
				if (this._isClipped && !flag)
				{
					this._textBlock.OpacityMask = this._opacityMask;
					this._brush = null;
					this._isClipped = false;
				}
				if (flag)
				{
					double width = layoutClip.Bounds.Width;
					double height = layoutClip.Bounds.Height;
					bool flag2 = this._textBlock.TextWrapping == TextWrapping.Wrap;
					if (this._brush == null)
					{
						this._brush = (flag2 ? this.GetVerticalClipBrush(height) : this.GetHorizontalClipBrush(width));
						this._textBlock.OpacityMask = this._brush;
					}
					else if (flag2 && FadeTrimming.VerticalBrushNeedsUpdating(this._brush, height))
					{
						this._brush.EndPoint = new Point(0.0, height);
						this._brush.GradientStops[1].Offset = (height - 20.0) / height;
					}
					else if (!flag2 && FadeTrimming.HorizontalBrushNeedsUpdating(this._brush, width))
					{
						this._brush.EndPoint = new Point(width, 0.0);
						this._brush.GradientStops[1].Offset = (width - 10.0) / width;
					}
					this._isClipped = true;
				}
			}

			// Token: 0x06000C95 RID: 3221 RVA: 0x0004608C File Offset: 0x0004428C
			private LinearGradientBrush GetHorizontalClipBrush(double visibleWidth)
			{
				return new LinearGradientBrush
				{
					MappingMode = BrushMappingMode.Absolute,
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(visibleWidth, 0.0),
					GradientStops = 
					{
						new GradientStop
						{
							Color = Colors.Black,
							Offset = 0.0
						},
						new GradientStop
						{
							Color = Colors.Black,
							Offset = (visibleWidth - 10.0) / visibleWidth
						},
						new GradientStop
						{
							Color = Colors.Transparent,
							Offset = 1.0
						}
					}
				};
			}

			// Token: 0x06000C96 RID: 3222 RVA: 0x0004615C File Offset: 0x0004435C
			private LinearGradientBrush GetVerticalClipBrush(double visibleHeight)
			{
				return new LinearGradientBrush
				{
					MappingMode = BrushMappingMode.Absolute,
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, visibleHeight),
					GradientStops = 
					{
						new GradientStop
						{
							Color = Colors.Black,
							Offset = 0.0
						},
						new GradientStop
						{
							Color = Colors.Black,
							Offset = (visibleHeight - 20.0) / visibleHeight
						},
						new GradientStop
						{
							Color = Colors.Transparent,
							Offset = 1.0
						}
					}
				};
			}

			// Token: 0x040007B0 RID: 1968
			private readonly TextBlock _textBlock;

			// Token: 0x040007B1 RID: 1969
			private bool _isAttached;

			// Token: 0x040007B2 RID: 1970
			private LinearGradientBrush _brush;

			// Token: 0x040007B3 RID: 1971
			private Brush _opacityMask;

			// Token: 0x040007B4 RID: 1972
			private bool _isClipped;

			// Token: 0x040007B5 RID: 1973
			private bool _verticalFadingEnabled;
		}
	}
}
