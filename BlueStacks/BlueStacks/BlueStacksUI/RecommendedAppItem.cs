using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000111 RID: 273
	public class RecommendedAppItem : UserControl, IComponentConnector
	{
		// Token: 0x06000B75 RID: 2933 RVA: 0x000093F6 File Offset: 0x000075F6
		public RecommendedAppItem()
		{
			this.InitializeComponent();
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00009404 File Offset: 0x00007604
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0000940C File Offset: 0x0000760C
		internal SearchRecommendation SearchRecomendation { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00009415 File Offset: 0x00007615
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

		// Token: 0x06000B79 RID: 2937 RVA: 0x0003FEA0 File Offset: 0x0003E0A0
		internal void Populate(MainWindow parentWindow, SearchRecommendation recom)
		{
			this.mMainWindow = parentWindow;
			this.recomIcon.IsFullImagePath = true;
			this.recomIcon.ImageName = recom.ImagePath;
			this.installButton.ButtonColor = ButtonColors.Green;
			this.installButton.Content = (this.ParentWindow.mAppHandler.IsAppInstalled(recom.ExtraPayload["click_action_packagename"]) ? LocaleStrings.GetLocalizedString("STRING_PLAY", "") : LocaleStrings.GetLocalizedString("STRING_INSTALL", ""));
			this.appNameTextBlock.Text = recom.ExtraPayload["click_action_title"];
			this.SearchRecomendation = recom;
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0003FF50 File Offset: 0x0003E150
		private void Recommendation_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ClientStats.SendFrontendClickStats("search_suggestion_click", "", (this.SearchRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay", this.SearchRecomendation.ExtraPayload["click_action_packagename"], this.ParentWindow.mAppHandler.IsAppInstalled(this.SearchRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false", null, null, null);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while sending stats to cloud for search_suggestion_click " + ex.ToString());
			}
			this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.SearchRecomendation.ExtraPayload, "search_suggestion", "");
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x00009436 File Offset: 0x00007636
		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x000085F0 File Offset: 0x000067F0
		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			base.Background = Brushes.Transparent;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00040034 File Offset: 0x0003E234
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedappitem.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x00040064 File Offset: 0x0003E264
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
				((RecommendedAppItem)target).MouseUp += new MouseButtonEventHandler(this.Recommendation_Click);
				((RecommendedAppItem)target).MouseEnter += this.UserControl_MouseEnter;
				((RecommendedAppItem)target).MouseLeave += this.UserControl_MouseLeave;
				return;
			case 2:
				this.recomIcon = (CustomPictureBox)target;
				return;
			case 3:
				this.appNameTextBlock = (TextBlock)target;
				return;
			case 4:
				this.installButton = (CustomButton)target;
				this.installButton.Click += this.Recommendation_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006E7 RID: 1767
		private MainWindow mMainWindow;

		// Token: 0x040006E8 RID: 1768
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox recomIcon;

		// Token: 0x040006E9 RID: 1769
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock appNameTextBlock;

		// Token: 0x040006EA RID: 1770
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton installButton;

		// Token: 0x040006EB RID: 1771
		private bool _contentLoaded;
	}
}
