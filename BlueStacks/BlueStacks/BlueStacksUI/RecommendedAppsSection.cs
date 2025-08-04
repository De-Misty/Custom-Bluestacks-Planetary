using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000113 RID: 275
	public class RecommendedAppsSection : UserControl, IComponentConnector
	{
		// Token: 0x06000B8D RID: 2957 RVA: 0x000094D3 File Offset: 0x000076D3
		public RecommendedAppsSection(string header)
		{
			this.InitializeComponent();
			this.mSectionHeader.Text = header;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x00040410 File Offset: 0x0003E610
		internal void AddSuggestedApps(MainWindow ParentWindow, List<AppRecommendation> suggestedApps, int clientShowCount)
		{
			int num = 1;
			int num2 = 1;
			ParentWindow.mWelcomeTab.mHomeAppManager.ClearAppRecommendationPool();
			foreach (AppRecommendation appRecommendation in suggestedApps)
			{
				if (!ParentWindow.mAppHandler.IsAppInstalled(appRecommendation.ExtraPayload["click_action_packagename"]))
				{
					RecommendedApps recommendedApps = new RecommendedApps();
					recommendedApps.Populate(appRecommendation, num, num2);
					if (num <= clientShowCount)
					{
						this.mAppRecommendationsPanel.Children.Add(recommendedApps);
						num++;
					}
					else
					{
						ParentWindow.mWelcomeTab.mHomeAppManager.AddToAppRecommendationPool(recommendedApps);
					}
				}
				num2++;
			}
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x000404CC File Offset: 0x0003E6CC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/recommendedappssection.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x000094ED File Offset: 0x000076ED
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
				this.mSectionHeader = (TextBlock)target;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.mAppRecommendationsPanel = (StackPanel)target;
		}

		// Token: 0x040006F6 RID: 1782
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSectionHeader;

		// Token: 0x040006F7 RID: 1783
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mAppRecommendationsPanel;

		// Token: 0x040006F8 RID: 1784
		private bool _contentLoaded;
	}
}
