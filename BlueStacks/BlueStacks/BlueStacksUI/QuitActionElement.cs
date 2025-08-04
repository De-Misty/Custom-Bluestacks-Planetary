using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200010E RID: 270
	public class QuitActionElement : UserControl, IComponentConnector
	{
		// Token: 0x06000B2E RID: 2862 RVA: 0x000090BC File Offset: 0x000072BC
		public QuitActionElement(MainWindow window, QuitPopupControl qpc)
		{
			this.ParentWindow = window;
			this.ParentQuitPopup = qpc;
			this.InitializeComponent();
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x000090F9 File Offset: 0x000072F9
		// (set) Token: 0x06000B30 RID: 2864 RVA: 0x00009101 File Offset: 0x00007301
		public string ParentPopupTag { get; set; } = string.Empty;

		// Token: 0x06000B31 RID: 2865 RVA: 0x0003F5DC File Offset: 0x0003D7DC
		private void SetProperties(QuitActionItem item)
		{
			string text = QuitActionCollection.Actions[item][QuitActionItemProperty.CallToAction];
			this.mBodyTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.BodyText];
			this.mMainImage.ImageName = QuitActionCollection.Actions[item][QuitActionItemProperty.ImageName];
			this.mHyperlinkTextBlock.Text = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionText];
			this.mQuitActionValue = QuitActionCollection.Actions[item][QuitActionItemProperty.ActionValue];
			this.mCTAEventName = QuitActionCollection.Actions[item][QuitActionItemProperty.StatEventName];
			this.mCallToAction = (QuitActionItemCTA)Enum.Parse(typeof(QuitActionItemCTA), text, true);
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000B32 RID: 2866 RVA: 0x0000910A File Offset: 0x0000730A
		// (set) Token: 0x06000B33 RID: 2867 RVA: 0x0000911C File Offset: 0x0000731C
		public QuitActionItem ActionElement
		{
			get
			{
				return (QuitActionItem)base.GetValue(QuitActionElement.ActionElementProperty);
			}
			set
			{
				base.SetValue(QuitActionElement.ActionElementProperty, value);
			}
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0003F69C File Offset: 0x0003D89C
		private static void ActionElementPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			QuitActionElement quitActionElement = d as QuitActionElement;
			if (!DesignerProperties.GetIsInDesignMode(quitActionElement))
			{
				quitActionElement.SetProperties((QuitActionItem)e.NewValue);
			}
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0003F6CC File Offset: 0x0003D8CC
		private void QAE_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				switch (this.mCallToAction)
				{
				case QuitActionItemCTA.OpenLinkInBrowser:
					if (!string.IsNullOrEmpty(this.mQuitActionValue))
					{
						BlueStacksUIUtils.OpenUrl(this.mQuitActionValue);
					}
					this.SendCTAStat();
					break;
				case QuitActionItemCTA.OpenAppCenter:
					this.OpenAppCenter();
					this.ParentQuitPopup.Close();
					this.SendCTAStat();
					break;
				case QuitActionItemCTA.OpenApplication:
					if (!string.IsNullOrEmpty(this.mQuitActionValue))
					{
						Process.Start(this.mQuitActionValue);
					}
					this.SendCTAStat();
					break;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Some error while CallToAction of QuitPopup. Ex: {0}", new object[] { ex });
			}
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0003F778 File Offset: 0x0003D978
		private void OpenAppCenter()
		{
			try
			{
				MainWindow parentWindow = this.ParentWindow;
				if (parentWindow != null)
				{
					parentWindow.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl(null), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter", false, "");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't open app center. Ex: {0}", new object[] { ex });
			}
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0000912F File Offset: 0x0000732F
		private void SendCTAStat()
		{
			ClientStats.SendLocalQuitPopupStatsAsync(this.ParentPopupTag, this.mCTAEventName);
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x00009142 File Offset: 0x00007342
		private void QAE_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mExternalLinkImage.Visibility = Visibility.Hidden;
			BlueStacksUIBinding.BindColor(this.maskBorder, Border.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
			base.Cursor = Cursors.Hand;
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00009170 File Offset: 0x00007370
		private void QAE_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mExternalLinkImage.Visibility = Visibility.Hidden;
			BlueStacksUIBinding.BindColor(this.maskBorder, Border.BackgroundProperty, "LightBandingColor");
			base.Cursor = Cursors.Arrow;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0003F7E4 File Offset: 0x0003D9E4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitactionelement.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0003F814 File Offset: 0x0003DA14
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
				((QuitActionElement)target).MouseEnter += this.QAE_MouseEnter;
				((QuitActionElement)target).MouseLeave += this.QAE_MouseLeave;
				((QuitActionElement)target).PreviewMouseUp += this.QAE_PreviewMouseUp;
				return;
			case 2:
				this.maskBorder = (Border)target;
				return;
			case 3:
				this.mParentGrid = (Grid)target;
				return;
			case 4:
				this.mExternalLinkImage = (CustomPictureBox)target;
				return;
			case 5:
				this.mMainImage = (CustomPictureBox)target;
				return;
			case 6:
				this.mBodyTextBlock = (TextBlock)target;
				return;
			case 7:
				this.mHyperlinkTextBlock = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006BD RID: 1725
		private MainWindow ParentWindow;

		// Token: 0x040006BE RID: 1726
		private QuitPopupControl ParentQuitPopup;

		// Token: 0x040006BF RID: 1727
		private string mQuitActionValue = string.Empty;

		// Token: 0x040006C0 RID: 1728
		private string mCTAEventName = string.Empty;

		// Token: 0x040006C1 RID: 1729
		private QuitActionItemCTA mCallToAction;

		// Token: 0x040006C3 RID: 1731
		public static readonly DependencyProperty ActionElementProperty = DependencyProperty.Register("ActionElement", typeof(QuitActionItem), typeof(QuitActionElement), new PropertyMetadata(QuitActionItem.None, new PropertyChangedCallback(QuitActionElement.ActionElementPropertyChangedCallback)));

		// Token: 0x040006C4 RID: 1732
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border maskBorder;

		// Token: 0x040006C5 RID: 1733
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mParentGrid;

		// Token: 0x040006C6 RID: 1734
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mExternalLinkImage;

		// Token: 0x040006C7 RID: 1735
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMainImage;

		// Token: 0x040006C8 RID: 1736
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mBodyTextBlock;

		// Token: 0x040006C9 RID: 1737
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHyperlinkTextBlock;

		// Token: 0x040006CA RID: 1738
		private bool _contentLoaded;
	}
}
