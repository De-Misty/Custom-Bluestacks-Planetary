using System;
using System.Collections.Generic;
using System.IO;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000A3 RID: 163
	public static class QuitActionCollection
	{
		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x00026174 File Offset: 0x00024374
		public static Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> Actions
		{
			get
			{
				if (QuitActionCollection.sQuitActionCollection == null)
				{
					object obj = QuitActionCollection.syncRoot;
					lock (obj)
					{
						if (QuitActionCollection.sQuitActionCollection == null)
						{
							QuitActionCollection.InitDictionary();
						}
					}
				}
				return QuitActionCollection.sQuitActionCollection;
			}
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x000261C0 File Offset: 0x000243C0
		private static void InitDictionary()
		{
			Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> dictionary = new Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>>();
			foreach (object obj in Enum.GetValues(typeof(QuitActionItem)))
			{
				QuitActionItem quitActionItem = (QuitActionItem)obj;
				Dictionary<QuitActionItemProperty, string> dictionary2 = new Dictionary<QuitActionItemProperty, string>();
				switch (quitActionItem)
				{
				case QuitActionItem.None:
					continue;
				case QuitActionItem.StuckAtBoot:
					dictionary2[QuitActionItemProperty.ImageName] = "clock_icon";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_STUCK_AT_BOOT_SCREEN", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenLinkInBrowser.ToString();
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
					dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/stuck_at_boot");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_stuck_at_boot";
					break;
				case QuitActionItem.SomethingElseWrong:
					dictionary2[QuitActionItemProperty.ImageName] = "support_icon";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_SOMETHING_ELSE_WENT_WRONG", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenApplication.ToString();
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_REPORT_A_PROBLEM", "");
					dictionary2[QuitActionItemProperty.ActionValue] = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_support";
					break;
				case QuitActionItem.SlowPerformance:
					dictionary2[QuitActionItemProperty.ImageName] = "rocket";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_SLOW_PERFORMANCE", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenLinkInBrowser.ToString();
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
					dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/enhance_performance");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_enhance_performance";
					break;
				case QuitActionItem.WhyGoogleAccount:
					dictionary2[QuitActionItemProperty.ImageName] = "google_white_icon";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_WHY_GOOGLE_ACCOUNT", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenLinkInBrowser.ToString();
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
					dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/why_google");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_why_google";
					break;
				case QuitActionItem.TroubleSigningIn:
					dictionary2[QuitActionItemProperty.ImageName] = "performance_icon";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_TROUBLE_SIGNING_IN", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenLinkInBrowser.ToString();
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_SEE_SOLUTION", "");
					dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetUrlWithParams("https://cloud.bluestacks.com/bs3/page/trouble_signing");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_trouble_signing";
					break;
				case QuitActionItem.UnsureWhereStart:
					dictionary2[QuitActionItemProperty.ImageName] = "exit_star";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_NOT_SURE_WHERE_START", "");
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_VIEW_TOP_GAMES", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenAppCenter.ToString();
					dictionary2[QuitActionItemProperty.ActionValue] = "";
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_unsure_where_start";
					break;
				case QuitActionItem.IssueInstallingGame:
					dictionary2[QuitActionItemProperty.ImageName] = "exit_exclamation";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_ISSUES_INSTALLING_A_GAME", "");
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_READ_SOLUTION", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenLinkInBrowser.ToString();
					dictionary2[QuitActionItemProperty.ActionValue] = WebHelper.GetHelpArticleURL("trouble_installing_running_game");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_issue_installing_game";
					break;
				case QuitActionItem.FacingOtherTroubles:
					dictionary2[QuitActionItemProperty.ImageName] = "exit_question";
					dictionary2[QuitActionItemProperty.BodyText] = LocaleStrings.GetLocalizedString("STRING_FACING_OTHER_TROUBLE", "");
					dictionary2[QuitActionItemProperty.ActionText] = LocaleStrings.GetLocalizedString("STRING_REPORT_A_PROBLEM", "");
					dictionary2[QuitActionItemProperty.CallToAction] = QuitActionItemCTA.OpenApplication.ToString();
					dictionary2[QuitActionItemProperty.ActionValue] = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
					dictionary2[QuitActionItemProperty.StatEventName] = "click_action_facing_other_troubles";
					break;
				}
				dictionary[quitActionItem] = dictionary2;
			}
			QuitActionCollection.sQuitActionCollection = dictionary;
		}

		// Token: 0x0400038A RID: 906
		private static Dictionary<QuitActionItem, Dictionary<QuitActionItemProperty, string>> sQuitActionCollection = null;

		// Token: 0x0400038B RID: 907
		private static readonly object syncRoot = new object();
	}
}
