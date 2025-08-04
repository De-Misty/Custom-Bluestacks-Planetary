using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000112 RID: 274
	public class RecommendedApps : UserControl, IComponentConnector
	{
		// Token: 0x06000B7F RID: 2943 RVA: 0x00009448 File Offset: 0x00007648
		public RecommendedApps()
		{
			this.InitializeComponent();
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x00009456 File Offset: 0x00007656
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x0000945E File Offset: 0x0000765E
		internal AppRecommendation AppRecomendation { get; set; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000B82 RID: 2946 RVA: 0x00009467 File Offset: 0x00007667
		// (set) Token: 0x06000B83 RID: 2947 RVA: 0x0000946F File Offset: 0x0000766F
		internal int RecommendedAppPosition { get; set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00009478 File Offset: 0x00007678
		// (set) Token: 0x06000B85 RID: 2949 RVA: 0x00009480 File Offset: 0x00007680
		internal int RecommendedAppRank { get; set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000B86 RID: 2950 RVA: 0x00009489 File Offset: 0x00007689
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

		// Token: 0x06000B87 RID: 2951 RVA: 0x0004011C File Offset: 0x0003E31C
		internal void Populate(AppRecommendation recom, int appPosition, int appRank)
		{
			this.AppRecomendation = recom;
			this.recomIcon.IsFullImagePath = true;
			this.recomIcon.ImageName = recom.ImagePath;
			this.appNameTextBlock.Text = recom.ExtraPayload["click_action_title"];
			this.appGenreTextBlock.Text = recom.GameGenre;
			this.RecommendedAppPosition = appPosition;
			this.RecommendedAppRank = appRank;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x00040188 File Offset: 0x0003E388
		private void Recommendation_Click(object sender, MouseButtonEventArgs e)
		{
			try
			{
				JArray jarray = new JArray();
				JObject jobject = new JObject
				{
					{
						"app_loc",
						(this.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"
					},
					{
						"app_pkg",
						this.AppRecomendation.ExtraPayload["click_action_packagename"]
					},
					{
						"is_installed",
						this.ParentWindow.mAppHandler.IsAppInstalled(this.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false"
					},
					{
						"app_position",
						this.RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)
					},
					{
						"app_rank",
						this.RecommendedAppRank.ToString(CultureInfo.InvariantCulture)
					}
				};
				jarray.Add(jobject);
				ClientStats.SendFrontendClickStats("apps_recommendation", "click", null, this.AppRecomendation.ExtraPayload["click_action_packagename"], null, null, null, jarray.ToString(Formatting.None, new JsonConverter[0]));
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while sending stats to cloud for apps_recommendation_click " + ex.ToString());
			}
			this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.AppRecomendation.ExtraPayload, "search_suggestion", "");
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x000094AA File Offset: 0x000076AA
		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.mMainGrid, Control.BackgroundProperty, "SearchGridBackgroundHoverColor");
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x000094C1 File Offset: 0x000076C1
		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mMainGrid.Background = Brushes.Transparent;
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x00040324 File Offset: 0x0003E524
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedapps.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x00040354 File Offset: 0x0003E554
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
				this.mMainGrid = (Grid)target;
				this.mMainGrid.MouseEnter += this.UserControl_MouseEnter;
				this.mMainGrid.MouseLeave += this.UserControl_MouseLeave;
				this.mMainGrid.PreviewMouseLeftButtonUp += this.Recommendation_Click;
				return;
			case 2:
				this.recomIcon = (CustomPictureBox)target;
				return;
			case 3:
				this.appNameTextBlock = (TextBlock)target;
				return;
			case 4:
				this.appGenreTextBlock = (TextBlock)target;
				return;
			case 5:
				this.installButton = (CustomButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006EF RID: 1775
		private MainWindow mMainWindow;

		// Token: 0x040006F0 RID: 1776
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainGrid;

		// Token: 0x040006F1 RID: 1777
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox recomIcon;

		// Token: 0x040006F2 RID: 1778
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock appNameTextBlock;

		// Token: 0x040006F3 RID: 1779
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock appGenreTextBlock;

		// Token: 0x040006F4 RID: 1780
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton installButton;

		// Token: 0x040006F5 RID: 1781
		private bool _contentLoaded;
	}
}
