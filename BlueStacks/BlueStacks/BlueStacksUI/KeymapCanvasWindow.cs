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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000148 RID: 328
	public class KeymapCanvasWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x0000A50E File Offset: 0x0000870E
		public MainWindow ParentWindow { get; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x0000A516 File Offset: 0x00008716
		// (set) Token: 0x06000D52 RID: 3410 RVA: 0x0004BAB4 File Offset: 0x00049CB4
		public static bool sIsDirty
		{
			get
			{
				return KeymapCanvasWindow.IsDirty;
			}
			set
			{
				KeymapCanvasWindow.IsDirty = value;
				if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
				{
					KMManager.CanvasWindow.SidebarWindow.mUndoBtn.IsEnabled = KeymapCanvasWindow.IsDirty;
					KMManager.CanvasWindow.SidebarWindow.mSaveBtn.IsEnabled = KeymapCanvasWindow.IsDirty;
				}
			}
		}

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0000A51D File Offset: 0x0000871D
		// (set) Token: 0x06000D54 RID: 3412 RVA: 0x0000A525 File Offset: 0x00008725
		internal bool IsInOverlayMode
		{
			get
			{
				return this.mIsInOverlayMode;
			}
			set
			{
				this.mIsInOverlayMode = value;
				base.IsShowGLWindow = value;
			}
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0004BB0C File Offset: 0x00049D0C
		internal KeymapCanvasWindow(MainWindow window)
		{
			this.ParentWindow = window;
			this.InitializeComponent();
			this.mParentWindowHeight = this.ParentWindow.ActualHeight * MainWindow.sScalingFactor;
			this.mParentWindowWidth = this.ParentWindow.ActualWidth * MainWindow.sScalingFactor;
			this.mParentWindowTop = this.ParentWindow.Top * MainWindow.sScalingFactor;
			this.mParentWindowLeft = this.ParentWindow.Left * MainWindow.sScalingFactor;
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x0000A535 File Offset: 0x00008735
		internal void ClearWindow()
		{
			this.dictCanvasElement.Clear();
			KMManager.listCanvasElement.Clear();
			CanvasElement.dictPoints.Clear();
			this.mCanvas.Children.Clear();
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0004BC14 File Offset: 0x00049E14
		private void Canvas_MouseEnter(object sender, MouseEventArgs e)
		{
			if (KMManager.IsDragging)
			{
				this.isNewElementAdded = true;
				KeymapCanvasWindow.sIsDirty = true;
				List<IMAction> list = KMManager.ClearElement();
				this.AddNewCanvasElement(list, false);
				this.StartMoving(this.mCanvasElement, e.GetPosition(this));
			}
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0004BC58 File Offset: 0x00049E58
		public void AddNewCanvasElement(List<IMAction> lstAction, bool isTap = false)
		{
			if (lstAction != null)
			{
				this.mCanvasElement = new CanvasElement(this, this.ParentWindow);
				double num = (base.ActualWidth - this.mCanvasElement.ActualWidth) * 100.0 / (base.ActualWidth * 3.0) + (double)(this.mTapXMargin * this.mCurrentTapElementDisplayCol);
				double num2 = (base.ActualHeight - this.mCanvasElement.ActualHeight) * 100.0 / (base.ActualHeight * 3.0) + (double)(this.mTapYMargin * this.mCurrentTapElementDisplayRow);
				foreach (IMAction imaction in lstAction)
				{
					if (isTap)
					{
						imaction.PositionX = Math.Round(num, 2);
						imaction.PositionY = Math.Round(num2, 2);
					}
					this.mCanvasElement.AddAction(imaction);
					this.dictCanvasElement.Add(imaction, this.mCanvasElement);
					if (isTap && lstAction.First<IMAction>().Type != KeyActionType.EdgeScroll)
					{
						this.mCanvasElement.ShowTextBox(this.mCanvasElement.dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
					}
					if (!imaction.IsChildAction && imaction.Type != KeyActionType.MOBADpad)
					{
						this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imaction);
					}
				}
				this.mCurrentTapElementDisplayCol++;
				if (this.mCurrentTapElementDisplayCol == this.mMaxElementPerRow)
				{
					this.mCurrentTapElementDisplayCol = 0;
					this.mCurrentTapElementDisplayRow++;
				}
				this.mCanvasElement.MouseLeftButtonDown += this.MoveIcon_PreviewMouseDown;
				this.mCanvasElement.mResizeIcon.PreviewMouseDown += this.ResizeIcon_PreviewMouseDown;
				this.mCanvas.Children.Add(this.mCanvasElement);
			}
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0004BE54 File Offset: 0x0004A054
		private void ResizeIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			this.mCanvasElement = WpfUtils.FindVisualParent<CanvasElement>(sender as DependencyObject);
			this.mCanvasElement.mResizeIcon.Focus();
			this.startPoint = e.GetPosition(this);
			this.mCanvas.PreviewMouseMove += this.CanvasResizeExistingElement_MouseMove;
			KeymapCanvasWindow.sIsDirty = true;
			e.Handled = true;
			Mouse.Capture(this.mCanvas);
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0004BEC8 File Offset: 0x0004A0C8
		private void CanvasResizeExistingElement_MouseMove(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.SizeNWSE;
			Point position = e.GetPosition(this);
			double num = position.X - this.startPoint.X;
			double num2 = position.Y - this.startPoint.Y;
			double num3 = num2;
			if (Math.Abs(num) > Math.Abs(num2))
			{
				num3 = num;
			}
			num3 = Math.Round(num3, 2);
			double num4 = this.mCanvasElement.ActualWidth + num3;
			if (num4 < 40.0)
			{
				num4 = 40.0;
				num3 = num4 - this.mCanvasElement.ActualWidth;
			}
			if (num4 < 70.0)
			{
				double num5 = this.mCanvasElement.ActualHeight - 20.0;
				this.mCanvasElement.mSkillImage.Margin = new Thickness(-50.0, num5, 10.0, 0.0);
			}
			if (this.mCanvasElement.mSkillImage.Visibility == Visibility.Visible)
			{
				this.mCanvasElement.mActionIcon.Visibility = Visibility.Visible;
			}
			double num6 = Canvas.GetTop(this.mCanvasElement);
			double num7 = Canvas.GetLeft(this.mCanvasElement);
			if (double.IsNaN(num6))
			{
				num6 = 0.0;
			}
			if (double.IsNaN(num7))
			{
				num7 = 0.0;
			}
			num6 -= num3 / 2.0;
			num7 -= num3 / 2.0;
			this.mCanvasElement.Width = num4;
			this.mCanvasElement.Height = num4;
			Canvas.SetLeft(this.mCanvasElement, num7);
			Canvas.SetTop(this.mCanvasElement, num6);
			this.mCanvasElement.UpdatePosition(num6, num7);
			this.startPoint = position;
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0004C088 File Offset: 0x0004A288
		internal void ReloadCanvasWindow()
		{
			this.mCurrentTapElementDisplayRow = 0;
			this.mCurrentTapElementDisplayCol = 0;
			if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null)
			{
				KMManager.LoadIMActions(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			}
			this.mCanvas.Children.Clear();
			this.Init();
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0004C0F8 File Offset: 0x0004A2F8
		private void MoveIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			CanvasElement canvasElement = sender as CanvasElement;
			if ((canvasElement.MOBASkillSettingsPopup != null && canvasElement.MOBASkillSettingsPopup.IsOpen) || canvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen || canvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen)
			{
				e.Handled = true;
				return;
			}
			canvasElement.TopOnClick = Canvas.GetTop(canvasElement);
			canvasElement.LeftOnClick = Canvas.GetLeft(canvasElement);
			Point position = e.GetPosition(this);
			if (!canvasElement.mResizeIcon.IsMouseOver && !canvasElement.mCloseIcon.IsMouseOver)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					if (canvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.Swipe)
					{
						bool flag = true;
						using (List<IMAction>.Enumerator enumerator = canvasElement.ListActionItem.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IMAction imaction = enumerator.Current;
								this.CreateGameControlCopy(imaction, e.GetPosition(this), flag);
								flag = false;
							}
							goto IL_011F;
						}
					}
					if (!canvasElement.ListActionItem.First<IMAction>().IsChildAction)
					{
						this.CreateGameControlCopy(canvasElement.ListActionItem.First<IMAction>(), e.GetPosition(this), true);
					}
				}
				else
				{
					this.StartMoving(canvasElement, position);
				}
				IL_011F:
				e.Handled = true;
			}
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0004C23C File Offset: 0x0004A43C
		private void CreateGameControlCopy(IMAction originalAction, Point point, bool isNewScheme = true)
		{
			IMAction imaction = originalAction.DeepCopy<IMAction>();
			imaction.PositionX = originalAction.PositionX + 1.0;
			List<CanvasElement> list = this.AddCanvasElementsForAction(imaction, false);
			if (isNewScheme)
			{
				KMManager.CheckAndCreateNewScheme();
			}
			this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imaction);
			this.StartMoving(list.First<CanvasElement>(), point);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0000A566 File Offset: 0x00008766
		private void CanvasMoveExistingElement_MouseMove(object sender, MouseEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			base.Focus();
			this.MoveElement(e.GetPosition(this));
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0004C2A0 File Offset: 0x0004A4A0
		internal void StartMoving(CanvasElement element, Point p)
		{
			if (this.mCanvasElement == null || this.mCanvasElement == element)
			{
				if (!element.mSkillImage.IsMouseOver)
				{
					KeymapCanvasWindow.sIsDirty = true;
				}
				this.mCanvasElement = element;
				this.startPoint = p;
				this.mCanvas.PreviewMouseMove -= this.CanvasMoveExistingElement_MouseMove;
				this.mCanvas.PreviewMouseMove += this.CanvasMoveExistingElement_MouseMove;
			}
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x0004C310 File Offset: 0x0004A510
		internal void MoveElement(Point p1)
		{
			if (this.mCanvasElement.IsLoaded)
			{
				base.Cursor = Cursors.Hand;
				double num = Canvas.GetTop(this.mCanvasElement);
				double num2 = Canvas.GetLeft(this.mCanvasElement);
				if (double.IsNaN(num))
				{
					num = 0.0;
				}
				if (double.IsNaN(num2))
				{
					num2 = 0.0;
				}
				double num3 = num2 + this.mCanvasElement.ActualWidth / 2.0;
				double num4 = num + this.mCanvasElement.ActualHeight / 2.0;
				num3 += p1.X - this.startPoint.X;
				num4 += p1.Y - this.startPoint.Y;
				num3 = ((num3 < 0.0) ? 0.0 : num3);
				num4 = ((num4 < 0.0) ? 0.0 : num4);
				num3 = ((num3 > this.mCanvas.ActualWidth) ? this.mCanvas.ActualWidth : num3);
				num4 = ((num4 > this.mCanvas.ActualHeight) ? this.mCanvas.ActualHeight : num4);
				num2 = num3 - this.mCanvasElement.ActualWidth / 2.0;
				num = num4 - this.mCanvasElement.ActualHeight / 2.0;
				Canvas.SetLeft(this.mCanvasElement, num2);
				Canvas.SetTop(this.mCanvasElement, num);
				this.mCanvasElement.UpdatePosition(num, num2);
				this.startPoint = p1;
			}
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0004C498 File Offset: 0x0004A698
		internal void Init()
		{
			IMConfig selectedConfig = this.ParentWindow.SelectedConfig;
			if (((selectedConfig != null) ? selectedConfig.SelectedControlScheme : null) == null)
			{
				return;
			}
			IMConfig selectedConfig2 = this.ParentWindow.SelectedConfig;
			int value = ((selectedConfig2 != null) ? new int?(selectedConfig2.SelectedControlScheme.GetHashCode()) : null).Value;
			if (this.mOldControlSchemeHashCode == value)
			{
				return;
			}
			this.mOldControlSchemeHashCode = value;
			this.ClearWindow();
			IMConfig selectedConfig3 = this.ParentWindow.SelectedConfig;
			bool flag;
			if (selectedConfig3 == null)
			{
				flag = false;
			}
			else
			{
				IMControlScheme selectedControlScheme = selectedConfig3.SelectedControlScheme;
				int? num = ((selectedControlScheme != null) ? new int?(selectedControlScheme.GameControls.Count) : null);
				int num2 = 0;
				flag = (num.GetValueOrDefault() > num2) & (num != null);
			}
			if (flag)
			{
				foreach (IMAction imaction in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
				{
					if (!this.IsInOverlayMode || imaction.IsVisibleInOverlay)
					{
						this.AddCanvasElementsForAction(imaction, true);
					}
					else if (!imaction.IsVisibleInOverlay)
					{
						List<CanvasElement> canvasElement = CanvasElement.GetCanvasElement(imaction, this, this.ParentWindow);
						foreach (CanvasElement canvasElement2 in canvasElement)
						{
							canvasElement2.Visibility = Visibility.Hidden;
						}
						KMManager.listCanvasElement.Add(canvasElement);
					}
				}
			}
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0004C62C File Offset: 0x0004A82C
		internal void InitLayout()
		{
			MainWindow parentWindow = this.ParentWindow;
			Grid mFrontendGrid = parentWindow.mFrontendGrid;
			Point point = mFrontendGrid.TranslatePoint(default(Point), parentWindow);
			if (this.IsInOverlayMode)
			{
				base.Background = Brushes.Transparent;
				this.mCanvas.Background = Brushes.Transparent;
				this.mCanvas2.Background = Brushes.Transparent;
				this.mCanvas.Margin = default(Thickness);
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					base.Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
				}
				else
				{
					string text = Path.Combine(RegistryManager.Instance.ClientInstallDir, "ImapImages");
					string text2 = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName + ".png";
					string text3 = Path.Combine(text, text2);
					if (File.Exists(text3))
					{
						this.mCanvasImage.ImageName = text3;
						this.mCanvas.Opacity = 0.0;
					}
					else
					{
						this.mCanvas.Opacity = 1.0;
					}
				}
				this.Handle = new WindowInteropHelper(this).EnsureHandle();
				int num = 1207959552;
				InteropWindow.SetWindowLong(this.Handle, -16, num);
				return;
			}
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.ParentWindow.Handle, true);
			double num2 = (double)fullscreenMonitorSize.Width - (double)this.mSidebarWidth * MainWindow.sScalingFactor;
			if (!this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
			{
				num2 -= this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor;
			}
			double num3 = this.ParentWindow.GetHeightFromWidth(num2, true, false);
			if (num3 > (double)fullscreenMonitorSize.Height)
			{
				num3 = (double)fullscreenMonitorSize.Height;
				num2 = this.ParentWindow.GetWidthFromHeight(num3, true, false);
			}
			double num4;
			if (this.ParentWindow.Top * MainWindow.sScalingFactor + num3 > (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height))
			{
				num4 = (double)fullscreenMonitorSize.Y + ((double)fullscreenMonitorSize.Height - num3) / 2.0;
			}
			else
			{
				num4 = this.ParentWindow.Top * MainWindow.sScalingFactor;
			}
			double num5;
			if (this.ParentWindow.Left * MainWindow.sScalingFactor + num2 + (double)this.mSidebarWidth * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width))
			{
				num5 = (double)fullscreenMonitorSize.X + ((double)fullscreenMonitorSize.Width - num2 - (double)this.mSidebarWidth * MainWindow.sScalingFactor) / 2.0;
			}
			else
			{
				num5 = this.ParentWindow.Left * MainWindow.sScalingFactor;
			}
			this.ParentWindow.ChangeHeightWidthTopLeft(num2, num3, num4, num5);
			base.Width = this.ParentWindow.ActualWidth;
			base.Height = this.ParentWindow.ActualHeight;
			base.Top = this.ParentWindow.Top;
			base.Left = this.ParentWindow.Left;
			Point point2 = new Point(parentWindow.ActualWidth - (mFrontendGrid.ActualWidth + point.X), parentWindow.ActualHeight - (mFrontendGrid.ActualHeight + point.Y));
			this.mCanvas.Margin = new Thickness(point.X, point.Y, point2.X, point2.Y);
			this.mCanvas.Width = mFrontendGrid.ActualWidth;
			this.mCanvas.Height = mFrontendGrid.ActualHeight;
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0004C9A8 File Offset: 0x0004ABA8
		private List<CanvasElement> AddCanvasElementsForAction(IMAction item, bool isLoadingFromFile = false)
		{
			List<CanvasElement> canvasElement = CanvasElement.GetCanvasElement(item, this, this.ParentWindow);
			foreach (CanvasElement canvasElement2 in canvasElement)
			{
				canvasElement2.mIsLoadingfromFile = isLoadingFromFile;
				foreach (IMAction imaction in canvasElement2.ListActionItem)
				{
					this.dictCanvasElement[imaction] = canvasElement2;
				}
				if (canvasElement2.Parent == null)
				{
					this.mCanvas.Children.Add(canvasElement2);
					canvasElement2.MouseLeftButtonDown -= this.MoveIcon_PreviewMouseDown;
					canvasElement2.mResizeIcon.PreviewMouseDown -= this.ResizeIcon_PreviewMouseDown;
					canvasElement2.MouseLeftButtonDown += this.MoveIcon_PreviewMouseDown;
					canvasElement2.mResizeIcon.PreviewMouseDown += this.ResizeIcon_PreviewMouseDown;
				}
				if (this.SidebarWindow == null)
				{
					canvasElement2.Visibility = Visibility.Hidden;
				}
			}
			KMManager.listCanvasElement.Add(canvasElement);
			return canvasElement;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x0004CADC File Offset: 0x0004ACDC
		private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
			if (this.mCanvasElement != null)
			{
				UIElement uielement = this.mCanvasElement;
				int num = this.zIndex;
				this.zIndex = num + 1;
				Panel.SetZIndex(uielement, num);
				this.mCanvasElement.ShowOtherIcons(true);
				if (this.mCanvasElement.IsMouseDirectlyOver)
				{
					e.Handled = true;
				}
				if (this.isNewElementAdded && this.mCanvasElement.dictTextElemets.Count > 0)
				{
					this.isNewElementAdded = false;
					this.mCanvasElement.ShowTextBox(this.mCanvasElement.dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
				}
			}
			this.startPoint = new Point(-1.0, -1.0);
			this.mCanvas.PreviewMouseMove -= this.CanvasMoveExistingElement_MouseMove;
			this.mCanvas.PreviewMouseMove -= this.CanvasResizeExistingElement_MouseMove;
			Mouse.Capture(null);
			this.mCanvasElement = null;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x0000A581 File Offset: 0x00008781
		private void KeymapCanvasWindow_Closing(object sender, CancelEventArgs e)
		{
			this.mIsClosing = true;
			this.ParentWindow.Focus();
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x0000A596 File Offset: 0x00008796
		private void CustomWindow_Closed(object sender, EventArgs e)
		{
			if (KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) && KMManager.dictOverlayWindow[this.ParentWindow] == this)
			{
				KMManager.dictOverlayWindow.Remove(this.ParentWindow);
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0004CBE0 File Offset: 0x0004ADE0
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			InteropWindow.RemoveWindowFromAltTabUI(new WindowInteropHelper(this).Handle);
			if (!this.IsInOverlayMode)
			{
				this.ShowSideBarWindow();
			}
			else
			{
				this.Handle = new WindowInteropHelper(this).Handle;
				int num = 1207959552;
				InteropWindow.SetWindowLong(this.Handle, -16, num);
				this.ParentWindow.mFrontendHandler.UpdateOverlaySizeStatus();
				this.ParentWindow.LocationChanged += this.ParentWindow_LocationChanged;
				this.ParentWindow.Activated += this.ParentWindow_Activated;
				this.ParentWindow.Deactivated += this.ParentWindow_Deactivated;
				this.UpdateSize();
			}
			this.Init();
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x0000A5CE File Offset: 0x000087CE
		private void ParentWindow_Deactivated(object sender, EventArgs e)
		{
			if (!this.mIsClosing && (KMManager.sGuidanceWindow == null || !KMManager.sGuidanceWindow.IsActive || KMManager.sGuidanceWindow.ParentWindow != this.ParentWindow))
			{
				base.Hide();
			}
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x0000A603 File Offset: 0x00008803
		private void ParentWindow_Activated(object sender, EventArgs e)
		{
			if (!this.mIsClosing)
			{
				base.Show();
			}
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0000A613 File Offset: 0x00008813
		private void ParentWindow_LocationChanged(object sender, EventArgs e)
		{
			this.UpdateSize();
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x0004CC98 File Offset: 0x0004AE98
		internal void UpdateSize()
		{
			if (this.ParentWindow.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero && !this.mIsClosing)
			{
				if (this.mIsShowWindow)
				{
					this.mIsShowWindow = false;
					Logger.Debug("KMP KeymapCanvasWindow UpdateSize");
					this.ParentWindow.mFrontendHandler.DeactivateFrontend();
					base.Show();
					return;
				}
				RECT rect = default(RECT);
				InteropWindow.GetWindowRect(this.ParentWindow.StaticComponents.mLastMappableWindowHandle, ref rect);
				int left = rect.Left;
				int top = rect.Top;
				int num = rect.Right - rect.Left;
				int num2 = rect.Bottom - rect.Top;
				InteropWindow.SetWindowPos(this.Handle, (IntPtr)0, left, top, num, num2, 16448U);
				this.ParentWindow.mFrontendHandler.FocusFrontend();
				this.SetOnboardingControlPosition(0.0, 0.0);
			}
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x0004CD94 File Offset: 0x0004AF94
		private Point GetCorrectCoordinateLocationForAndroid(Point p)
		{
			double num = p.X * 100.0 / this.ParentWindow.Width;
			double num2 = p.Y * 100.0 / this.ParentWindow.Height;
			return new Point(num, num2);
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x0004CDE4 File Offset: 0x0004AFE4
		private void ShowSideBarWindow()
		{
			if (this.SidebarWindow == null)
			{
				this.SidebarWindow = new AdvancedGameControlWindow(this.ParentWindow);
				this.SidebarWindow.Init(this);
				this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
				this.SidebarWindow.Owner = this;
				this.SidebarWindow.Show();
				this.SidebarWindow.Activate();
			}
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x0004CE50 File Offset: 0x0004B050
		private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (CanvasElement.sFocusedTextBox != null)
			{
				WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
				return;
			}
			if (double.IsNaN(this.CanvasWindowLeft) && double.IsNaN(this.CanvasWindowTop))
			{
				this.CanvasWindowLeft = base.Left;
				this.CanvasWindowTop = base.Top;
				this.mMousePointForNewTap = Mouse.GetPosition(this.mCanvas);
			}
			KeymapCanvasWindow.sIsDirty = true;
			try
			{
				base.DragMove();
			}
			catch (Exception)
			{
			}
			if (Math.Abs(this.CanvasWindowLeft - base.Left) < 2.0 && Math.Abs(this.CanvasWindowTop - base.Top) < 2.0)
			{
				if (KMManager.sIsInScriptEditingMode && this.mIsExtraSettingsPopupOpened)
				{
					return;
				}
				IMAction imaction = new Tap
				{
					Type = KeyActionType.Tap
				};
				if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0 && CanvasElement.sFocusedTextBox != null)
				{
					WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
				}
				else
				{
					if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
					{
						KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, null, false);
					}
					else if (this.ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
					{
						KMManager.CheckAndCreateNewScheme();
					}
					this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(imaction);
					List<CanvasElement> list = this.AddCanvasElementsForAction(imaction, false);
					list.First<CanvasElement>().SetMousePoint(this.mMousePointForNewTap);
					list.First<CanvasElement>().IsRemoveIfEmpty = true;
					list.First<CanvasElement>().ShowTextBox(list.First<CanvasElement>().dictTextElemets.First<KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>>().Value.Item3);
				}
			}
			this.CanvasWindowLeft = double.NaN;
			this.CanvasWindowTop = double.NaN;
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x0004D050 File Offset: 0x0004B250
		private void CustomWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.mMousePointForNewTap = Mouse.GetPosition(this.mCanvas);
			this.CanvasWindowLeft = base.Left;
			this.CanvasWindowTop = base.Top;
			try
			{
				base.DragMove();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x0000A61B File Offset: 0x0000881B
		private void CustomWindow_LocationChanged(object sender, EventArgs e)
		{
			if (!this.IsInOverlayMode)
			{
				this.ParentWindow.Top = base.Top;
				this.ParentWindow.Left = base.Left;
			}
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0004D0A4 File Offset: 0x0004B2A4
		private void CustomWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			foreach (object obj in this.mCanvas.Children)
			{
				if (this.IsInOverlayMode)
				{
					(obj as CanvasElement).SetElementLayout(true, (obj as CanvasElement).mXPosition, (obj as CanvasElement).mYPosition);
					if ((obj as CanvasElement).ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
					{
						this.SetOnboardingControlPosition((obj as CanvasElement).mXPosition, (obj as CanvasElement).mYPosition);
					}
				}
				else
				{
					(obj as CanvasElement).SetElementLayout(false, 0.0, 0.0);
				}
			}
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x0004D17C File Offset: 0x0004B37C
		private void MCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			ToolTip toolTip = new ToolTip();
			if (KMManager.sIsInScriptEditingMode)
			{
				Point correctCoordinateLocationForAndroid = this.GetCorrectCoordinateLocationForAndroid(Mouse.GetPosition(this.mCanvas));
				if (toolTip.IsOpen)
				{
					toolTip.IsOpen = false;
				}
				toolTip.Content = string.Format(" X: {0} Y: {1}", correctCoordinateLocationForAndroid.X, correctCoordinateLocationForAndroid.Y);
				toolTip.StaysOpen = true;
				toolTip.Placement = PlacementMode.Mouse;
				toolTip.IsOpen = true;
			}
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x0004D1F4 File Offset: 0x0004B3F4
		internal void ShowOnboardingOverlayControl(double left, double top, bool isVisible = true)
		{
			if (isVisible && File.Exists(Path.Combine(CustomPictureBox.AssetsDir, "onboarding_step_" + KMManager.mOnboardingCounter.ToString(CultureInfo.InvariantCulture) + ".png")))
			{
				PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
				bool? flag;
				if (mPostBootCloudInfo == null)
				{
					flag = null;
				}
				else
				{
					AppPackageListObject gameAwareOnBoardingAppPackages = mPostBootCloudInfo.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages;
					flag = ((gameAwareOnBoardingAppPackages != null) ? new bool?(gameAwareOnBoardingAppPackages.IsPackageAvailable(KMManager.sPackageName)) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					this.mCanvas2.Opacity = 1.0;
					this.mOnboardingControl.mOnboardingImg.ImageName = "onboarding_step_" + KMManager.mOnboardingCounter.ToString(CultureInfo.InvariantCulture);
					this.mOnboardingControl.Visibility = Visibility.Visible;
					this.mOnboardingControl.mOnboardingImg.Visibility = Visibility.Visible;
					this.mGrid.Visibility = Visibility.Visible;
					this.SetOnboardingControlPosition(left, top);
					return;
				}
			}
			this.mOnboardingControl.Visibility = Visibility.Collapsed;
			this.mGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0004D308 File Offset: 0x0004B508
		internal void SetOnboardingControlPosition(double left, double top)
		{
			if (left == 0.0 || top == 0.0)
			{
				left = 62.8;
				top = 15.6;
			}
			double num = left / 100.0 * this.mCanvas.ActualWidth;
			double num2 = top / 100.0 * this.mCanvas.ActualHeight;
			num = ((num < 0.0) ? 0.0 : num);
			num2 = ((num2 < 0.0) ? 0.0 : num2);
			num = ((num > this.ParentWindow.ActualWidth) ? this.ParentWindow.ActualWidth : num);
			num2 = ((num2 > this.ParentWindow.ActualHeight) ? this.ParentWindow.ActualHeight : num2);
			double num3 = 310.0;
			double num4 = 85.0;
			this.mOnboardingControl.mOnboardingImg.Height = num4 / 100.0 * this.mCanvas.ActualHeight * 0.2;
			this.mOnboardingControl.mOnboardingImg.Width = num3 / 100.0 * this.mCanvas.ActualWidth * 0.1;
			left = num - this.mOnboardingControl.mOnboardingImg.Width / 2.0;
			top = num2 - this.mOnboardingControl.mOnboardingImg.Height / 2.0;
			Canvas.SetLeft(this.mOnboardingControl, left);
			Canvas.SetTop(this.mOnboardingControl, top);
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0004D4AC File Offset: 0x0004B6AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/keymapcanvaswindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x0004D4DC File Offset: 0x0004B6DC
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
				((KeymapCanvasWindow)target).Closing += this.KeymapCanvasWindow_Closing;
				((KeymapCanvasWindow)target).Closed += this.CustomWindow_Closed;
				((KeymapCanvasWindow)target).Loaded += this.Window_Loaded;
				((KeymapCanvasWindow)target).LocationChanged += this.CustomWindow_LocationChanged;
				((KeymapCanvasWindow)target).MouseLeftButtonDown += this.CustomWindow_MouseDown;
				((KeymapCanvasWindow)target).SizeChanged += this.CustomWindow_SizeChanged;
				return;
			case 2:
				((Grid)target).MouseLeftButtonDown += this.Canvas_MouseDown;
				return;
			case 3:
				this.mCanvas = (Canvas)target;
				this.mCanvas.MouseEnter += this.Canvas_MouseEnter;
				this.mCanvas.PreviewMouseUp += this.Canvas_MouseUp;
				this.mCanvas.MouseDown += this.CustomWindow_MouseDown;
				return;
			case 4:
				this.mCanvasImage = (CustomPictureBox)target;
				return;
			case 5:
				this.mGrid = (Grid)target;
				return;
			case 6:
				this.mCanvas2 = (Canvas)target;
				return;
			case 7:
				this.mOnboardingControl = (OnboardingOverlayControl)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400083E RID: 2110
		private int mCurrentTapElementDisplayRow;

		// Token: 0x0400083F RID: 2111
		private int mCurrentTapElementDisplayCol;

		// Token: 0x04000840 RID: 2112
		private int mTapYMargin = 5;

		// Token: 0x04000841 RID: 2113
		private int mTapXMargin = 5;

		// Token: 0x04000842 RID: 2114
		private int mMaxElementPerRow = 10;

		// Token: 0x04000843 RID: 2115
		internal bool mIsShowWindow = true;

		// Token: 0x04000844 RID: 2116
		private bool isNewElementAdded;

		// Token: 0x04000845 RID: 2117
		private int mSidebarWidth = 260;

		// Token: 0x04000847 RID: 2119
		internal AdvancedGameControlWindow SidebarWindow;

		// Token: 0x04000848 RID: 2120
		private Point startPoint = new Point(-1.0, -1.0);

		// Token: 0x04000849 RID: 2121
		private int zIndex;

		// Token: 0x0400084A RID: 2122
		internal CanvasElement mCanvasElement;

		// Token: 0x0400084B RID: 2123
		internal double mParentWindowHeight;

		// Token: 0x0400084C RID: 2124
		internal double mParentWindowWidth;

		// Token: 0x0400084D RID: 2125
		internal double mParentWindowTop;

		// Token: 0x0400084E RID: 2126
		internal double mParentWindowLeft;

		// Token: 0x0400084F RID: 2127
		internal Dictionary<IMAction, CanvasElement> dictCanvasElement = new Dictionary<IMAction, CanvasElement>();

		// Token: 0x04000850 RID: 2128
		internal static bool IsDirty;

		// Token: 0x04000851 RID: 2129
		internal static bool sWasMaximized;

		// Token: 0x04000852 RID: 2130
		internal bool mIsExtraSettingsPopupOpened;

		// Token: 0x04000853 RID: 2131
		private int mOldControlSchemeHashCode;

		// Token: 0x04000854 RID: 2132
		private bool mIsInOverlayMode;

		// Token: 0x04000855 RID: 2133
		internal bool mIsClosing;

		// Token: 0x04000856 RID: 2134
		internal double SidebarWindowLeft = -1.0;

		// Token: 0x04000857 RID: 2135
		internal double SidebarWindowTop = -1.0;

		// Token: 0x04000858 RID: 2136
		internal double CanvasWindowLeft = -1.0;

		// Token: 0x04000859 RID: 2137
		internal double CanvasWindowTop = -1.0;

		// Token: 0x0400085A RID: 2138
		private Point mMousePointForNewTap;

		// Token: 0x0400085B RID: 2139
		private IntPtr Handle;

		// Token: 0x0400085C RID: 2140
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Canvas mCanvas;

		// Token: 0x0400085D RID: 2141
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCanvasImage;

		// Token: 0x0400085E RID: 2142
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x0400085F RID: 2143
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Canvas mCanvas2;

		// Token: 0x04000860 RID: 2144
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal OnboardingOverlayControl mOnboardingControl;

		// Token: 0x04000861 RID: 2145
		private bool _contentLoaded;
	}
}
