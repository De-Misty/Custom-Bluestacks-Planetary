using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000060 RID: 96
	[Serializable]
	public class GuidanceData
	{
		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x0000533E File Offset: 0x0000353E
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x00005346 File Offset: 0x00003546
		public ObservableCollection<GuidanceCategoryViewModel> KeymapViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x0000534F File Offset: 0x0000354F
		// (set) Token: 0x060004E6 RID: 1254 RVA: 0x00005357 File Offset: 0x00003557
		public ObservableCollection<GuidanceCategoryViewModel> GamepadViewGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryViewModel>();

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060004E7 RID: 1255 RVA: 0x00005360 File Offset: 0x00003560
		// (set) Token: 0x060004E8 RID: 1256 RVA: 0x00005368 File Offset: 0x00003568
		public ObservableCollection<GuidanceCategoryEditModel> KeymapEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00005371 File Offset: 0x00003571
		// (set) Token: 0x060004EA RID: 1258 RVA: 0x00005379 File Offset: 0x00003579
		public ObservableCollection<GuidanceCategoryEditModel> GamepadEditGuidance { get; private set; } = new ObservableCollection<GuidanceCategoryEditModel>();

		// Token: 0x060004EB RID: 1259 RVA: 0x00005382 File Offset: 0x00003582
		public void Clear()
		{
			this.KeymapViewGuidance.Clear();
			this.GamepadViewGuidance.Clear();
			this.KeymapEditGuidance.Clear();
			this.GamepadEditGuidance.Clear();
			this.mKeymapEditGuidanceCloned = null;
			this.mGamepadEditGuidanceCloned = null;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x000053BE File Offset: 0x000035BE
		public void SaveOriginalData()
		{
			this.mKeymapEditGuidanceCloned = this.KeymapEditGuidance.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
			this.mGamepadEditGuidanceCloned = this.GamepadEditGuidance.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x000053E2 File Offset: 0x000035E2
		public void Reset()
		{
			this.KeymapEditGuidance = this.mKeymapEditGuidanceCloned.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
			this.GamepadEditGuidance = this.mGamepadEditGuidanceCloned.DeepCopy<ObservableCollection<GuidanceCategoryEditModel>>();
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001E988 File Offset: 0x0001CB88
		public void AddGuidance(bool isGamePad, string category, string guidanceText, string guidanceKey, string actionItem, IMAction imAction)
		{
			if (imAction != null && !string.IsNullOrEmpty(guidanceKey) && !string.IsNullOrEmpty(actionItem))
			{
				Type propertyType = IMAction.DictPropertyInfo[imAction.Type][actionItem].PropertyType;
				double num;
				if (propertyType == typeof(double) && double.TryParse(guidanceKey, out num))
				{
					guidanceKey = num.ToString(CultureInfo.InvariantCulture);
				}
				if (imAction is EdgeScroll && actionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					guidanceKey = (Convert.ToBoolean(guidanceKey, CultureInfo.InvariantCulture) ? "ON" : "OFF");
				}
				string text = (isGamePad ? "GamePad" : "KeyMap");
				if (!this.mViewIgnoreList[text].ContainsKey(imAction.GetType()) || !this.mViewIgnoreList[text][imAction.GetType()].ContainsKey(actionItem) || !imAction.Guidance.ContainsKey(this.mViewIgnoreList[text][imAction.GetType()][actionItem]))
				{
					ObservableCollection<GuidanceCategoryViewModel> observableCollection = (isGamePad ? this.GamepadViewGuidance : this.KeymapViewGuidance);
					GuidanceCategoryViewModel guidanceCategoryViewModel = observableCollection.Where((GuidanceCategoryViewModel guide) => string.Equals(guide.Category, category, StringComparison.InvariantCulture)).FirstOrDefault<GuidanceCategoryViewModel>();
					if (guidanceCategoryViewModel == null)
					{
						GuidanceViewModel guidanceViewModel = new GuidanceViewModel
						{
							PropertyType = propertyType
						};
						guidanceViewModel.GuidanceTexts.Add(guidanceText);
						guidanceViewModel.GuidanceKeys.Add(guidanceKey);
						guidanceCategoryViewModel = new GuidanceCategoryViewModel
						{
							Category = category
						};
						guidanceCategoryViewModel.GuidanceViewModels.Add(guidanceViewModel);
						observableCollection.Add(guidanceCategoryViewModel);
					}
					else
					{
						if (propertyType != typeof(double))
						{
							GuidanceViewModel guidanceViewModel2 = guidanceCategoryViewModel.GuidanceViewModels.Where((GuidanceViewModel guide) => guide.PropertyType != typeof(double) && guide.GuidanceTexts.Count == 1 && guide.GuidanceTexts.Contains(guidanceText)).FirstOrDefault<GuidanceViewModel>();
							if (guidanceViewModel2 != null)
							{
								guidanceViewModel2.GuidanceKeys.AddIfNotContain(guidanceKey);
								goto IL_0352;
							}
						}
						if (propertyType != typeof(double))
						{
							GuidanceViewModel guidanceViewModel3 = guidanceCategoryViewModel.GuidanceViewModels.Where((GuidanceViewModel guide) => guide.PropertyType != typeof(double) && guide.GuidanceKeys.Count == 1 && guide.GuidanceKeys.Contains(guidanceKey)).FirstOrDefault<GuidanceViewModel>();
							if (guidanceViewModel3 != null)
							{
								guidanceViewModel3.GuidanceTexts.AddIfNotContain(guidanceText);
								goto IL_0352;
							}
						}
						GuidanceViewModel guidanceViewModel4 = new GuidanceViewModel
						{
							PropertyType = propertyType
						};
						guidanceViewModel4.GuidanceTexts.Add(guidanceText);
						guidanceViewModel4.GuidanceKeys.Add(guidanceKey);
						guidanceCategoryViewModel.GuidanceViewModels.Add(guidanceViewModel4);
					}
				}
				IL_0352:
				if (!this.mEditIgnoreList[text].ContainsKey(imAction.GetType()) || !this.mEditIgnoreList[text][imAction.GetType()].Contains(actionItem))
				{
					ObservableCollection<GuidanceCategoryEditModel> observableCollection2 = (isGamePad ? this.GamepadEditGuidance : this.KeymapEditGuidance);
					GuidanceCategoryEditModel guidanceCategoryEditModel = observableCollection2.Where((GuidanceCategoryEditModel guide) => string.Equals(guide.Category, category, StringComparison.InvariantCulture)).FirstOrDefault<GuidanceCategoryEditModel>();
					if (guidanceCategoryEditModel == null)
					{
						guidanceCategoryEditModel = new GuidanceCategoryEditModel
						{
							Category = category
						};
						observableCollection2.Add(guidanceCategoryEditModel);
					}
					GuidanceEditModel guidanceEditModel = null;
					if (propertyType == typeof(string))
					{
						guidanceEditModel = guidanceCategoryEditModel.GuidanceEditModels.Where((GuidanceEditModel gem) => gem.ActionType == imAction.Type && gem.PropertyType == propertyType && string.Equals(gem.GuidanceText, guidanceText, StringComparison.InvariantCultureIgnoreCase) && string.Equals(gem.GuidanceKey, guidanceKey, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault<GuidanceEditModel>();
					}
					if (guidanceEditModel == null)
					{
						guidanceEditModel = ((propertyType == typeof(string) || propertyType == typeof(bool)) ? new GuidanceEditTextModel() : new GuidanceEditDecimalModel());
						guidanceEditModel.GuidanceText = guidanceText;
						guidanceEditModel.OriginalGuidanceKey = guidanceKey;
						guidanceEditModel.ActionType = imAction.Type;
						guidanceEditModel.PropertyType = propertyType;
						guidanceEditModel.IsEnabled = !string.Equals(actionItem, "KeyAction", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(actionItem, "KeyMove", StringComparison.InvariantCultureIgnoreCase);
						guidanceCategoryEditModel.GuidanceEditModels.Add(guidanceEditModel);
					}
					guidanceEditModel.IMActionItems.Add(new IMActionItem
					{
						ActionItem = actionItem,
						IMAction = imAction
					});
				}
			}
		}

		// Token: 0x04000296 RID: 662
		private ObservableCollection<GuidanceCategoryEditModel> mKeymapEditGuidanceCloned;

		// Token: 0x04000297 RID: 663
		private ObservableCollection<GuidanceCategoryEditModel> mGamepadEditGuidanceCloned;

		// Token: 0x04000298 RID: 664
		private Dictionary<string, Dictionary<Type, Dictionary<string, string>>> mViewIgnoreList = new Dictionary<string, Dictionary<Type, Dictionary<string, string>>>
		{
			{
				"KeyMap",
				new Dictionary<Type, Dictionary<string, string>> { 
				{
					typeof(Dpad),
					new Dictionary<string, string>
					{
						{ "KeyUp", "DpadTitle" },
						{ "KeyLeft", "DpadTitle" },
						{ "KeyDown", "DpadTitle" },
						{ "KeyRight", "DpadTitle" }
					}
				} }
			},
			{
				"GamePad",
				new Dictionary<Type, Dictionary<string, string>>()
			}
		};

		// Token: 0x04000299 RID: 665
		private Dictionary<string, Dictionary<Type, List<string>>> mEditIgnoreList = new Dictionary<string, Dictionary<Type, List<string>>>
		{
			{
				"KeyMap",
				new Dictionary<Type, List<string>> { 
				{
					typeof(Dpad),
					new List<string> { "DpadTitle" }
				} }
			},
			{
				"GamePad",
				new Dictionary<Type, List<string>>()
			}
		};
	}
}
