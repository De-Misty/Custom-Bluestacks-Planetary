using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000149 RID: 329
	public class KeymapExtraSettingWindow : CustomPopUp, IComponentConnector
	{
		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000D79 RID: 3449 RVA: 0x0000A647 File Offset: 0x00008847
		public List<IMAction> ListAction { get; } = new List<IMAction>();

		// Token: 0x06000D7A RID: 3450 RVA: 0x0004D644 File Offset: 0x0004B844
		public KeymapExtraSettingWindow(MainWindow window)
		{
			this.InitializeComponent();
			base.IsFocusOnMouseClick = true;
			this.ParentWindow = window;
			this.mStackPanel = this.mScrollBar.Content as StackPanel;
			this.AddGuidanceCategories();
			this.AddDualTextBlockControl();
			this.SetPopupDraggableProperty();
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0000A64F File Offset: 0x0000884F
		private void AddDualTextBlockControl()
		{
			this.AddDualTextBlockControlToMOBAPanel();
			this.AddDualTextBlockControlToLookAroundPanel();
			this.AddDualTextBlockControlToShootGBPanel();
			this.AddDualTextBlockControlToMOBASkillCancelGBPanel();
			this.AddDualTextBlockControlToGroupBox();
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0004D6C0 File Offset: 0x0004B8C0
		private void AddDualTextBlockControlToGroupBox()
		{
			this.mEnableConditionTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Height = 32.0
			};
			this.mEnableConditionGB.Content = this.mEnableConditionTB;
			this.mNoteTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Height = 32.0
			};
			this.mNoteGB.Content = this.mNoteTB;
			this.mStartConditionTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Height = 32.0
			};
			this.mStartConditionGB.Content = this.mStartConditionTB;
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x0004D818 File Offset: 0x0004BA18
		internal void AddDualTextBlockControlToMOBASkillCancelGBPanel()
		{
			this.mMOBASkillCancelDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelDTB);
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mMOBASkillCancelXExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBASkillCancelYExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBASkillCancelXOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBASkillCancelYOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBASkillCancelShowOnOverlayDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelXExprDTB);
				this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelYExprDTB);
				this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelXOffsetDTB);
				this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelYOffsetDTB);
				this.mMOBASkillCancelGBPanel.Children.Add(this.mMOBASkillCancelShowOnOverlayDTB);
			}
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0004DA3C File Offset: 0x0004BC3C
		private void AddDualTextBlockControlToShootGBPanel()
		{
			this.mShootXDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mShootYDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mShootDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mShootGBPanel.Children.Add(this.mShootXDTB);
			this.mShootGBPanel.Children.Add(this.mShootYDTB);
			this.mShootGBPanel.Children.Add(this.mShootDTB);
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mShootXExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mShootYExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mShootXOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mShootYOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mShootShowOnOverlayDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mShootGBPanel.Children.Add(this.mShootXExprDTB);
				this.mShootGBPanel.Children.Add(this.mShootYExprDTB);
				this.mShootGBPanel.Children.Add(this.mShootXOffsetDTB);
				this.mShootGBPanel.Children.Add(this.mShootYOffsetDTB);
				this.mShootGBPanel.Children.Add(this.mShootShowOnOverlayDTB);
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0004DD0C File Offset: 0x0004BF0C
		private void AddDualTextBlockControlToLookAroundPanel()
		{
			this.mLookAroundXDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mLookAroundYDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mLookAroundDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mLookAroundPanel.Children.Add(this.mLookAroundXDTB);
			this.mLookAroundPanel.Children.Add(this.mLookAroundYDTB);
			this.mLookAroundPanel.Children.Add(this.mLookAroundDTB);
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mLookAroundXExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mLookAroundYExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mLookAroundXOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mLookAroundYOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mLookAroundShowOnOverlayDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mLookAroundPanel.Children.Add(this.mLookAroundXExprDTB);
				this.mLookAroundPanel.Children.Add(this.mLookAroundYExprDTB);
				this.mLookAroundPanel.Children.Add(this.mLookAroundXOffsetDTB);
				this.mLookAroundPanel.Children.Add(this.mLookAroundYOffsetDTB);
				this.mLookAroundPanel.Children.Add(this.mLookAroundShowOnOverlayDTB);
			}
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0004DFDC File Offset: 0x0004C1DC
		private void AddDualTextBlockControlToMOBAPanel()
		{
			this.mMOBADpadOriginXDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mMOBADpadOriginYDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mMOBADpadCharSpeedDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mMOBADpadKeyDTB = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			this.mMOBAPanel.Children.Add(this.mMOBADpadOriginXDTB);
			this.mMOBAPanel.Children.Add(this.mMOBADpadOriginYDTB);
			this.mMOBAPanel.Children.Add(this.mMOBADpadCharSpeedDTB);
			this.mMOBAPanel.Children.Add(this.mMOBADpadKeyDTB);
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mMOBADpadXExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBADpadYExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBADpadXOverlayOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBADpadYOverlayOffsetDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBADpadHeroOriginXExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBADpadHeroOriginYExprDTB = new DualTextBlockControl(this.ParentWindow)
				{
					Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
				};
				this.mMOBAPanel.Children.Add(this.mMOBADpadXExprDTB);
				this.mMOBAPanel.Children.Add(this.mMOBADpadYExprDTB);
				this.mMOBAPanel.Children.Add(this.mMOBADpadXOverlayOffsetDTB);
				this.mMOBAPanel.Children.Add(this.mMOBADpadYOverlayOffsetDTB);
				this.mMOBAPanel.Children.Add(this.mMOBADpadHeroOriginXExprDTB);
				this.mMOBAPanel.Children.Add(this.mMOBADpadHeroOriginYExprDTB);
			}
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x0004E35C File Offset: 0x0004C55C
		private void AddControls(DualTextBlockControl control, StackPanel panel)
		{
			control = new DualTextBlockControl(this.ParentWindow)
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0)
			};
			panel.Children.Add(control);
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x0004E3B4 File Offset: 0x0004C5B4
		private void AddGuidanceCategories()
		{
			this.mListSuggestions.Clear();
			foreach (IMAction imaction in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
			{
				string text;
				if (string.Equals(imaction.GuidanceCategory, "MISC", StringComparison.InvariantCulture))
				{
					text = LocaleStrings.GetLocalizedString("STRING_" + imaction.GuidanceCategory, "");
				}
				else
				{
					text = this.ParentWindow.SelectedConfig.GetUIString(imaction.GuidanceCategory);
				}
				if (!this.mListSuggestions.Contains(text))
				{
					this.mListSuggestions.Add(text);
				}
			}
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x0000A66F File Offset: 0x0000886F
		private void AddListOfSuggestions()
		{
			this.mGuidanceCategoryComboBox.AddSuggestions(this.mListSuggestions);
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x0000A682 File Offset: 0x00008882
		private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.IsOpen = false;
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x0004E47C File Offset: 0x0004C67C
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			KeymapCanvasWindow.sIsDirty = true;
			foreach (IMAction imaction in this.ListAction)
			{
				this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(imaction);
			}
			(this.mCanvasElement.Parent as Canvas).Children.Remove(this.mCanvasElement);
			base.IsOpen = false;
			foreach (KeyValuePair<IMAction, CanvasElement> keyValuePair in KMManager.CanvasWindow.dictCanvasElement)
			{
				if (keyValuePair.Key.ParentAction == this.ListAction.First<IMAction>())
				{
					keyValuePair.Value.RemoveAction("");
					this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(keyValuePair.Value.ListActionItem.First<IMAction>());
				}
			}
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0004E5AC File Offset: 0x0004C7AC
		private void SetPopupDraggableProperty()
		{
			try
			{
				KeymapExtraSettingWindow.<>c__DisplayClass56_0 CS$<>8__locals1 = new KeymapExtraSettingWindow.<>c__DisplayClass56_0();
				CS$<>8__locals1.Thumb = new Thumb
				{
					Width = 0.0,
					Height = 0.0
				};
				this.mHeaderGrid.Children.Add(CS$<>8__locals1.Thumb);
				this.mHeaderGrid.MouseLeftButtonDown -= CS$<>8__locals1.<SetPopupDraggableProperty>g__mouseDownHandler|0;
				this.mHeaderGrid.MouseLeftButtonDown += CS$<>8__locals1.<SetPopupDraggableProperty>g__mouseDownHandler|0;
				CS$<>8__locals1.Thumb.DragDelta -= this.<SetPopupDraggableProperty>g__deltaEventHandler|56_1;
				CS$<>8__locals1.Thumb.DragDelta += this.<SetPopupDraggableProperty>g__deltaEventHandler|56_1;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in draggable popup: " + ex.ToString());
			}
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x0004E688 File Offset: 0x0004C888
		internal void Init(bool isGamepadTabSelected = false)
		{
			bool sIsDirty = KeymapCanvasWindow.sIsDirty;
			this.mDictGroupBox.Clear();
			this.mDummyGrid.Children.Clear();
			this.mStackPanel.Children.Clear();
			this.mDictDualTextBox.Clear();
			BlueStacksUIBinding.Bind(this.mHeader, Constants.ImapLocaleStringsConstant + this.ListAction.First<IMAction>().Type.ToString() + "_Settings", "");
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mEnableConditionTB.Visibility = Visibility.Visible;
				this.mEnableConditionGB.Visibility = Visibility.Visible;
				this.mEnableConditionTB.ActionItemProperty = "EnableCondition";
				this.mStartConditionTB.Visibility = Visibility.Visible;
				this.mStartConditionGB.Visibility = Visibility.Visible;
				this.mStartConditionTB.ActionItemProperty = "StartCondition";
				this.mNoteTB.Visibility = Visibility.Visible;
				this.mNoteGB.Visibility = Visibility.Visible;
				this.mNoteTB.ActionItemProperty = "Note";
			}
			this.AddListOfSuggestions();
			this.mStackPanel.Children.Add(this.mGuidanceCategory);
			this.mStackPanel.Children.Add(this.mTabsGrid);
			if (isGamepadTabSelected)
			{
				this.mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
				this.mKeyboardTabBorder.Background = Brushes.Transparent;
				BlueStacksUIBinding.BindColor(this.mKeyboardTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
				BlueStacksUIBinding.BindColor(this.mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
				this.mGamepadTabBorder.BorderThickness = new Thickness(0.0);
			}
			else
			{
				this.mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
				this.mGamepadTabBorder.Background = Brushes.Transparent;
				BlueStacksUIBinding.BindColor(this.mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
				this.mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
			}
			foreach (IMAction imaction in this.ListAction)
			{
				if (string.Equals(imaction.GuidanceCategory, "MISC", StringComparison.InvariantCulture))
				{
					this.mGuidanceCategoryComboBox.mAutoComboBox.Text = LocaleStrings.GetLocalizedString("STRING_" + imaction.GuidanceCategory, "");
				}
				else
				{
					this.mGuidanceCategoryComboBox.mAutoComboBox.Text = this.ParentWindow.SelectedConfig.GetUIString(imaction.GuidanceCategory);
				}
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mEnableConditionGB.Visibility = Visibility.Visible;
					this.mEnableConditionTB.Visibility = Visibility.Visible;
					this.mEnableConditionTB.AddActionItem(imaction);
					this.mNoteGB.Visibility = Visibility.Visible;
					this.mNoteTB.Visibility = Visibility.Visible;
					this.mNoteTB.AddActionItem(imaction);
					this.mStartConditionGB.Visibility = Visibility.Visible;
					this.mStartConditionTB.Visibility = Visibility.Visible;
					this.mStartConditionTB.AddActionItem(imaction);
				}
				else
				{
					this.mEnableConditionGB.Visibility = Visibility.Collapsed;
					this.mStartConditionGB.Visibility = Visibility.Collapsed;
					this.mNoteGB.Visibility = Visibility.Collapsed;
				}
				foreach (KeyValuePair<string, PropertyInfo> keyValuePair in IMAction.DictPopUpUIElements[imaction.Type])
				{
					if (string.Equals(keyValuePair.Key, "IsMOBADpadEnabled", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mStackPanel.Children.Add(this.mMOBAGB);
						this.mMOBACB.IsChecked = new bool?(Convert.ToBoolean(imaction[keyValuePair.Key], CultureInfo.InvariantCulture));
						bool value = this.mMOBACB.IsChecked.Value;
						this.mMOBADpadCharSpeedDTB.IsEnabled = value;
						this.mMOBADpadCharSpeedDTB.mKeyPropertyNameTextBox.IsEnabled = false;
						this.mMOBADpadCharSpeedDTB.ActionItemProperty = "CharSpeed";
						this.mMOBADpadCharSpeedDTB.AddActionItem((imaction as Dpad).mMOBADpad);
						this.mMOBADpadOriginXDTB.IsEnabled = value;
						this.mMOBADpadOriginXDTB.mKeyPropertyNameTextBox.IsEnabled = false;
						this.mMOBADpadOriginXDTB.ActionItemProperty = "OriginX";
						this.mMOBADpadOriginXDTB.AddActionItem((imaction as Dpad).mMOBADpad);
						this.mMOBADpadOriginYDTB.IsEnabled = value;
						this.mMOBADpadOriginYDTB.mKeyPropertyNameTextBox.IsEnabled = false;
						this.mMOBADpadOriginYDTB.ActionItemProperty = "OriginY";
						this.mMOBADpadOriginYDTB.AddActionItem((imaction as Dpad).mMOBADpad);
						this.mMOBADpadKeyDTB.IsEnabled = value;
						this.mMOBADpadKeyDTB.mKeyPropertyNameTextBox.IsEnabled = false;
						this.mMOBADpadKeyDTB.mKeyTextBox.IsEnabled = false;
						this.mMOBADpadKeyDTB.ActionItemProperty = "KeyMove";
						this.mMOBADpadKeyDTB.AddActionItem((imaction as Dpad).mMOBADpad);
						if (KMManager.sIsDeveloperModeOn)
						{
							this.mMOBADpadXExprDTB.IsEnabled = value;
							this.mMOBADpadXExprDTB.ActionItemProperty = "XExpr";
							this.mMOBADpadXExprDTB.AddActionItem((imaction as Dpad).mMOBADpad);
							this.mMOBADpadYExprDTB.IsEnabled = value;
							this.mMOBADpadYExprDTB.ActionItemProperty = "YExpr";
							this.mMOBADpadYExprDTB.AddActionItem((imaction as Dpad).mMOBADpad);
							this.mMOBADpadXOverlayOffsetDTB.IsEnabled = value;
							this.mMOBADpadXOverlayOffsetDTB.ActionItemProperty = "XOverlayOffset";
							this.mMOBADpadXOverlayOffsetDTB.AddActionItem((imaction as Dpad).mMOBADpad);
							this.mMOBADpadYOverlayOffsetDTB.IsEnabled = value;
							this.mMOBADpadYOverlayOffsetDTB.ActionItemProperty = "YOverlayOffset";
							this.mMOBADpadYOverlayOffsetDTB.AddActionItem((imaction as Dpad).mMOBADpad);
							this.mMOBADpadHeroOriginXExprDTB.IsEnabled = value;
							this.mMOBADpadHeroOriginXExprDTB.ActionItemProperty = "OriginXExpr";
							this.mMOBADpadHeroOriginXExprDTB.AddActionItem((imaction as Dpad).mMOBADpad);
							this.mMOBADpadHeroOriginYExprDTB.IsEnabled = value;
							this.mMOBADpadHeroOriginYExprDTB.ActionItemProperty = "OriginYExpr";
							this.mMOBADpadHeroOriginYExprDTB.AddActionItem((imaction as Dpad).mMOBADpad);
						}
					}
					else if (string.Equals(keyValuePair.Key, "IsCancelSkillEnabled", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mStackPanel.Children.Add(this.mMOBASkillCancelGB);
						this.mMOBASkillCancelCB.IsChecked = new bool?(Convert.ToBoolean(imaction[keyValuePair.Key], CultureInfo.InvariantCulture));
						bool value2 = this.mMOBASkillCancelCB.IsChecked.Value;
						this.mMOBASkillCancelDTB.IsEnabled = value2;
						if (isGamepadTabSelected)
						{
							this.mMOBASkillCancelDTB.ActionItemProperty = "KeyCancel_alt1";
						}
						else
						{
							this.mMOBASkillCancelDTB.ActionItemProperty = "KeyCancel";
						}
						this.mMOBASkillCancelDTB.AddActionItem(imaction);
						if (KMManager.sIsDeveloperModeOn)
						{
							this.mMOBASkillCancelXExprDTB.IsEnabled = value2;
							this.mMOBASkillCancelXExprDTB.ActionItemProperty = "CancelXExpr";
							this.mMOBASkillCancelXExprDTB.AddActionItem(imaction);
							this.mMOBASkillCancelYExprDTB.IsEnabled = value2;
							this.mMOBASkillCancelYExprDTB.ActionItemProperty = "CancelYExpr";
							this.mMOBASkillCancelYExprDTB.AddActionItem(imaction);
							this.mMOBASkillCancelXOffsetDTB.IsEnabled = value2;
							this.mMOBASkillCancelXOffsetDTB.ActionItemProperty = "CancelXOverlayOffset";
							this.mMOBASkillCancelXOffsetDTB.AddActionItem(imaction);
							this.mMOBASkillCancelYOffsetDTB.IsEnabled = value2;
							this.mMOBASkillCancelYOffsetDTB.ActionItemProperty = "CancelYOverlayOffset";
							this.mMOBASkillCancelYOffsetDTB.AddActionItem(imaction);
							this.mMOBASkillCancelShowOnOverlayDTB.IsEnabled = value2;
							this.mMOBASkillCancelShowOnOverlayDTB.ActionItemProperty = "CancelShowOnOverlayExpr";
							this.mMOBASkillCancelShowOnOverlayDTB.AddActionItem(imaction);
						}
					}
					else if (string.Equals(keyValuePair.Key, "IsLookAroundEnabled", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mStackPanel.Children.Add(this.mLookAroundGB);
						this.mLookAroundCB.IsChecked = new bool?(Convert.ToBoolean(imaction[keyValuePair.Key], CultureInfo.InvariantCulture));
						bool value3 = this.mLookAroundCB.IsChecked.Value;
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundDTB, value3, "KeyLookAround");
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundXDTB, value3, "LookAroundX");
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundYDTB, value3, "LookAroundY");
						if (KMManager.sIsDeveloperModeOn)
						{
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundXExprDTB, value3, "LookAroundXExpr");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundYExprDTB, value3, "LookAroundYExpr");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundXOffsetDTB, value3, "LookAroundXOverlayOffset");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundYOffsetDTB, value3, "LookAroundYOverlayOffset");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mLookAroundShowOnOverlayDTB, value3, "LookAroundShowOnOverlayExpr");
						}
					}
					else if (string.Equals(keyValuePair.Key, "IsShootOnClickEnabled", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mStackPanel.Children.Add(this.mShootGB);
						this.mShootCB.IsChecked = new bool?(Convert.ToBoolean(imaction[keyValuePair.Key], CultureInfo.InvariantCulture));
						bool value4 = this.mShootCB.IsChecked.Value;
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootDTB, value4, "KeyAction");
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootXDTB, value4, "LButtonX");
						KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootYDTB, value4, "LButtonY");
						if (KMManager.sIsDeveloperModeOn)
						{
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootXExprDTB, value4, "LButtonXExpr");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootYExprDTB, value4, "LButtonYExpr");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootXOffsetDTB, value4, "LButtonXOverlayOffset");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootYOffsetDTB, value4, "LButtonYOverlayOffset");
							KeymapExtraSettingWindow.SetChildControlsValues(imaction, this.mShootShowOnOverlayDTB, value4, "LButtonShowOnOverlayExpr");
						}
					}
					else if (string.Equals(keyValuePair.Key, "ShowOnOverlay", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mOverlayCB.IsChecked = new bool?(Convert.ToBoolean(imaction[keyValuePair.Key], CultureInfo.InvariantCulture));
						this.mOverlayCB.Tag = keyValuePair.Key;
						if (!this.mStackPanel.Children.Contains(this.mOverlayGB))
						{
							this.mStackPanel.Children.Add(this.mOverlayGB);
						}
					}
					else if (imaction.Type == KeyActionType.FreeLook && (string.Equals(keyValuePair.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase)))
					{
						if (((FreeLook)imaction).DeviceType == 0)
						{
							if (string.Equals(keyValuePair.Key, "Speed", StringComparison.InvariantCultureIgnoreCase))
							{
								this.AddFields(keyValuePair, imaction);
							}
						}
						else if (string.Equals(keyValuePair.Key, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) || string.Equals(keyValuePair.Key, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase))
						{
							this.AddFields(keyValuePair, imaction);
						}
					}
					else if (isGamepadTabSelected)
					{
						if (keyValuePair.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !keyValuePair.Key.ToString(CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture))
						{
							this.AddFields(keyValuePair, imaction);
						}
					}
					else if (!keyValuePair.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
					{
						this.AddFields(keyValuePair, imaction);
					}
				}
				if (KMManager.sIsDeveloperModeOn)
				{
					foreach (KeyValuePair<string, PropertyInfo> keyValuePair2 in IMAction.sDictDevModeUIElements[imaction.Type])
					{
						if (isGamepadTabSelected)
						{
							if (keyValuePair2.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture) || !keyValuePair2.Key.ToString(CultureInfo.InvariantCulture).StartsWith("Key", StringComparison.InvariantCulture))
							{
								this.AddFields(keyValuePair2, imaction);
							}
						}
						else if (!keyValuePair2.Key.ToString(CultureInfo.InvariantCulture).EndsWith("_alt1", StringComparison.InvariantCulture))
						{
							this.AddFields(keyValuePair2, imaction);
						}
					}
				}
			}
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mStackPanel.Children.Add(this.mEnableConditionGB);
				this.mStackPanel.Children.Add(this.mNoteGB);
				this.mStackPanel.Children.Add(this.mStartConditionGB);
			}
			this.UpdateFieldsForMOBADpad();
			KeymapCanvasWindow.sIsDirty = sIsDirty;
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x0000A68B File Offset: 0x0000888B
		private static void SetChildControlsValues(IMAction action, DualTextBlockControl control, bool isEnabled, string actionItemProperty)
		{
			control.mKeyPropertyNameTextBox.IsEnabled = isEnabled;
			control.ActionItemProperty = actionItemProperty;
			control.AddActionItem(action);
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0004F3C0 File Offset: 0x0004D5C0
		private void AddFields(KeyValuePair<string, PropertyInfo> item, IMAction action)
		{
			bool flag = false;
			object[] array = item.Value.GetCustomAttributes(typeof(CategoryAttribute), true);
			CategoryAttribute categoryAttribute = array[0] as CategoryAttribute;
			string category = categoryAttribute.Category;
			string text = categoryAttribute.Category + "~" + item.Key;
			array = item.Value.GetCustomAttributes(typeof(DescriptionAttribute), true);
			if (array.Length != 0 && (array[0] as DescriptionAttribute).Description.Contains("NotCommon"))
			{
				text = text + "~" + action.Direction.ToString();
				flag = true;
			}
			GroupBox groupBox = this.GetGroupBox(category);
			if (this.mDictDualTextBox.ContainsKey(text))
			{
				this.mDictDualTextBox[text].AddActionItem(action);
				return;
			}
			DualTextBlockControl dualTextBlockControl = new DualTextBlockControl(this.ParentWindow)
			{
				IsAddDirectionAttribute = flag,
				ActionItemProperty = item.Key
			};
			dualTextBlockControl.AddActionItem(action);
			if (string.Equals(item.Key, "GuidanceCategory", StringComparison.InvariantCultureIgnoreCase))
			{
				dualTextBlockControl.mKeyPropertyNameTextBox.IsEnabled = false;
				this.mStackPanel.Children.Remove(groupBox);
				this.mStackPanel.Children.Insert(0, groupBox);
			}
			(groupBox.Content as StackPanel).Children.Add(dualTextBlockControl);
			this.mDictDualTextBox[text] = dualTextBlockControl;
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x0004F524 File Offset: 0x0004D724
		private GroupBox GetGroupBox(string category)
		{
			GroupBox groupBox;
			if (this.mDictGroupBox.ContainsKey(category))
			{
				groupBox = this.mDictGroupBox[category];
			}
			else
			{
				groupBox = new GroupBox();
				this.mDictGroupBox.Add(category, groupBox);
				groupBox.Header = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + category, "");
				this.mStackPanel.Children.Add(groupBox);
				groupBox.Content = new StackPanel();
			}
			return groupBox;
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0000A03F File Offset: 0x0000823F
		private void CustomPictureBox_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Hand;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x0000A6A8 File Offset: 0x000088A8
		private void CustomPictureBox_MouseLeave(object sender, MouseEventArgs e)
		{
			if (KMManager.CanvasWindow.mCanvasElement == null)
			{
				base.Cursor = Cursors.Arrow;
			}
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x0004F59C File Offset: 0x0004D79C
		private void MOBAHeroCB_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this.ListAction.Count > 0)
			{
				KMManager.CheckAndCreateNewScheme();
				this.mMOBADpadCharSpeedDTB.IsEnabled = (this.mMOBADpadOriginXDTB.IsEnabled = (this.mMOBADpadOriginYDTB.IsEnabled = (this.mMOBADpadKeyDTB.IsEnabled = this.mMOBACB.IsChecked.Value)));
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mMOBADpadXExprDTB.IsEnabled = (this.mMOBADpadYExprDTB.IsEnabled = (this.mMOBADpadXOverlayOffsetDTB.IsEnabled = (this.mMOBADpadYOverlayOffsetDTB.IsEnabled = (this.mMOBADpadHeroOriginXExprDTB.IsEnabled = (this.mMOBADpadHeroOriginYExprDTB.IsEnabled = this.mMOBACB.IsChecked.Value)))));
				}
				this.mMOBADpadKeyDTB.mKeyTextBox.IsEnabled = false;
				Dpad dpad = this.ListAction.First<IMAction>() as Dpad;
				if (this.mMOBACB.IsChecked.Value)
				{
					dpad.mMOBADpad.mDpad = dpad;
					dpad.mMOBADpad.ParentAction = dpad;
				}
				else if (dpad.IsMOBADpadEnabled)
				{
					MOBADpad.sListMOBADpad.Remove(dpad.mMOBADpad);
					if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(dpad.mMOBADpad))
					{
						KMManager.CanvasWindow.dictCanvasElement[dpad.mMOBADpad].RemoveAction("");
						KMManager.CanvasWindow.dictCanvasElement.Remove(dpad.mMOBADpad);
					}
					this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(dpad.mMOBADpad);
					dpad.mMOBADpad.OriginX = (dpad.mMOBADpad.OriginY = -1.0);
				}
				this.UpdateFieldsForMOBADpad();
				KeymapCanvasWindow.sIsDirty = true;
			}
			if (this.mMOBAPB != null)
			{
				this.mMOBAPB.IsEnabled = this.mMOBACB.IsChecked.Value;
			}
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x0004F7B0 File Offset: 0x0004D9B0
		private void MOBAHeroPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			base.IsOpen = false;
			Dpad dpad = this.ListAction.First<IMAction>() as Dpad;
			if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(dpad.mMOBADpad))
			{
				CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[dpad.mMOBADpad];
				KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft(canvasElement), Canvas.GetTop(canvasElement)));
				return;
			}
			dpad.mMOBADpad.X = dpad.X;
			dpad.mMOBADpad.Y = dpad.Y;
			KMManager.CheckAndCreateNewScheme();
			this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Add(dpad.mMOBADpad);
			this.AddUIInCanvas(dpad.mMOBADpad);
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x0004F878 File Offset: 0x0004DA78
		private void UpdateFieldsForMOBADpad()
		{
			if (this.ListAction.First<IMAction>().Type == KeyActionType.Dpad && this.mMOBACB.IsChecked != null)
			{
				foreach (KeyValuePair<string, DualTextBlockControl> keyValuePair in this.mDictDualTextBox)
				{
					if (keyValuePair.Key.Contains("~Key"))
					{
						keyValuePair.Value.mKeyTextBox.IsEnabled = !this.mMOBACB.IsChecked.Value;
						keyValuePair.Value.mKeyPropertyNameTextBox.IsEnabled = !this.mMOBACB.IsChecked.Value;
						if (!keyValuePair.Value.mKeyTextBox.IsEnabled)
						{
							keyValuePair.Value.mKeyPropertyNameTextBox.Text = string.Empty;
							keyValuePair.Value.mKeyTextBox.Tag = string.Empty;
							keyValuePair.Value.mKeyTextBox.Text = string.Empty;
						}
					}
				}
			}
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x0004F9B4 File Offset: 0x0004DBB4
		private void MOBASkillCancelCB_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this.ListAction.Count > 0)
			{
				KMManager.CheckAndCreateNewScheme();
				this.mMOBASkillCancelDTB.IsEnabled = this.mMOBASkillCancelCB.IsChecked.Value;
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mMOBASkillCancelXExprDTB.IsEnabled = (this.mMOBASkillCancelYExprDTB.IsEnabled = (this.mMOBASkillCancelYExprDTB.IsEnabled = (this.mMOBASkillCancelYOffsetDTB.IsEnabled = (this.mMOBASkillCancelShowOnOverlayDTB.IsEnabled = this.mMOBASkillCancelCB.IsChecked.Value))));
				}
				MOBASkill mobaskill = this.ListAction.First<IMAction>() as MOBASkill;
				if (this.mMOBASkillCancelCB.IsChecked.Value)
				{
					mobaskill.mMOBASkillCancel = new MOBASkillCancel(mobaskill);
				}
				else if (mobaskill.IsCancelSkillEnabled)
				{
					if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(mobaskill.mMOBASkillCancel))
					{
						KMManager.CanvasWindow.dictCanvasElement[mobaskill.mMOBASkillCancel].RemoveAction("KeyCancel");
						KMManager.CanvasWindow.dictCanvasElement.Remove(mobaskill.mMOBASkillCancel);
						this.mMOBASkillCancelDTB.mKeyPropertyNameTextBox.Text = string.Empty;
					}
					mobaskill.CancelX = (mobaskill.CancelY = -1.0);
				}
				KeymapCanvasWindow.sIsDirty = true;
			}
			if (this.mMOBASkillCancelPB != null)
			{
				this.mMOBASkillCancelPB.IsEnabled = this.mMOBASkillCancelCB.IsChecked.Value;
			}
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x0004FB3C File Offset: 0x0004DD3C
		private void MOBASkillCancelPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			base.IsOpen = false;
			MOBASkill mobaskill = this.ListAction.First<IMAction>() as MOBASkill;
			if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(mobaskill.mMOBASkillCancel))
			{
				CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[mobaskill.mMOBASkillCancel];
				KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft(canvasElement), Canvas.GetTop(canvasElement)));
				return;
			}
			this.AddUIInCanvas(mobaskill.mMOBASkillCancel);
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x0004FBBC File Offset: 0x0004DDBC
		private void LookAroundCB_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this.ListAction.Count > 0)
			{
				KMManager.CheckAndCreateNewScheme();
				this.mLookAroundXDTB.IsEnabled = (this.mLookAroundYDTB.IsEnabled = (this.mLookAroundDTB.IsEnabled = this.mLookAroundCB.IsChecked.Value));
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mLookAroundXExprDTB.IsEnabled = (this.mLookAroundYExprDTB.IsEnabled = (this.mLookAroundXOffsetDTB.IsEnabled = (this.mLookAroundYOffsetDTB.IsEnabled = (this.mLookAroundShowOnOverlayDTB.IsEnabled = this.mLookAroundCB.IsChecked.Value))));
				}
				Pan pan = this.ListAction.First<IMAction>() as Pan;
				if (this.mLookAroundCB.IsChecked.Value)
				{
					pan.mLookAround = new LookAround(pan);
				}
				else if (pan.IsLookAroundEnabled)
				{
					if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
					{
						KMManager.CanvasWindow.dictCanvasElement[pan.mLookAround].RemoveAction("KeyLookAround");
						KMManager.CanvasWindow.dictCanvasElement.Remove(pan.mLookAround);
						this.mLookAroundDTB.mKeyPropertyNameTextBox.Text = string.Empty;
					}
					pan.LookAroundX = (pan.LookAroundY = -1.0);
				}
				KeymapCanvasWindow.sIsDirty = true;
			}
			if (this.mLookAroundPB != null)
			{
				this.mLookAroundPB.IsEnabled = this.mLookAroundCB.IsChecked.Value;
			}
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x0004FD60 File Offset: 0x0004DF60
		private void LookAroundPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			base.IsOpen = false;
			Pan pan = this.ListAction.First<IMAction>() as Pan;
			if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
			{
				CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[pan.mLookAround];
				KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft(canvasElement), Canvas.GetTop(canvasElement)));
				return;
			}
			this.AddUIInCanvas(pan.mLookAround);
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x0004FDE0 File Offset: 0x0004DFE0
		private void ShootCB_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this.ListAction.Count > 0)
			{
				KMManager.CheckAndCreateNewScheme();
				this.mShootXDTB.IsEnabled = (this.mShootYDTB.IsEnabled = (this.mShootDTB.IsEnabled = this.mLookAroundCB.IsChecked.Value));
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mShootXExprDTB.IsEnabled = (this.mShootYExprDTB.IsEnabled = (this.mShootXOffsetDTB.IsEnabled = (this.mShootYOffsetDTB.IsEnabled = (this.mShootShowOnOverlayDTB.IsEnabled = this.mShootCB.IsChecked.Value))));
				}
				Pan pan = this.ListAction.First<IMAction>() as Pan;
				if (this.mShootCB.IsChecked.Value)
				{
					pan.mPanShoot = new PanShoot(pan);
				}
				else if (pan.IsShootOnClickEnabled)
				{
					if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mPanShoot))
					{
						KMManager.CanvasWindow.dictCanvasElement[pan.mPanShoot].RemoveAction("KeyAction");
						KMManager.CanvasWindow.dictCanvasElement.Remove(pan.mPanShoot);
						this.mShootDTB.mKeyPropertyNameTextBox.Text = string.Empty;
					}
					pan.LButtonX = (pan.LButtonY = -1.0);
				}
				KeymapCanvasWindow.sIsDirty = true;
			}
			if (this.mShootPB != null)
			{
				this.mShootPB.IsEnabled = this.mShootCB.IsChecked.Value;
			}
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x0004FF84 File Offset: 0x0004E184
		private void ShootPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			base.IsOpen = false;
			Pan pan = this.ListAction.First<IMAction>() as Pan;
			if (KMManager.CanvasWindow.dictCanvasElement.ContainsKey(pan.mPanShoot))
			{
				CanvasElement canvasElement = KMManager.CanvasWindow.dictCanvasElement[pan.mPanShoot];
				KMManager.CanvasWindow.StartMoving(canvasElement, new Point(Canvas.GetLeft(canvasElement), Canvas.GetTop(canvasElement)));
				return;
			}
			this.AddUIInCanvas(pan.mPanShoot);
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x00050004 File Offset: 0x0004E204
		private void mOverlayCB_Checked(object sender, RoutedEventArgs e)
		{
			if (this.ListAction.Count > 0)
			{
				KMManager.CheckAndCreateNewScheme();
				foreach (IMAction imaction in this.ListAction)
				{
					imaction.IsVisibleInOverlay = true;
				}
				KeymapCanvasWindow.sIsDirty = true;
			}
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x00050070 File Offset: 0x0004E270
		private void mOverlayCB_Unchecked(object sender, RoutedEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			if (this.ListAction.Count > 0)
			{
				foreach (IMAction imaction in this.ListAction)
				{
					imaction.IsVisibleInOverlay = false;
				}
			}
			KeymapCanvasWindow.sIsDirty = true;
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x0000A6C1 File Offset: 0x000088C1
		private void AddUIInCanvas(IMAction hero)
		{
			base.Cursor = Cursors.Hand;
			KMManager.GetCanvasElement(this.ParentWindow, hero, this.mCanvas, true);
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x00009FD4 File Offset: 0x000081D4
		private void mCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			KMManager.RepositionCanvasElement();
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x00009FDB File Offset: 0x000081DB
		private void mCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
			KMManager.ClearElement();
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x000500DC File Offset: 0x0004E2DC
		private void mGamepadTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				KMManager.CheckAndCreateNewScheme();
				foreach (IMAction imaction in this.ListAction)
				{
					if (!string.Equals(imaction.GuidanceCategory, this.mGuidanceCategoryComboBox.mAutoComboBox.Text, StringComparison.InvariantCulture))
					{
						imaction.GuidanceCategory = this.mGuidanceCategoryComboBox.mAutoComboBox.Text;
						KeymapCanvasWindow.sIsDirty = true;
						this.ParentWindow.SelectedConfig.AddString(imaction.GuidanceCategory);
					}
				}
				this.mGamepadTabBorder.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
				this.mGamepadTabBorder.Background = Brushes.Transparent;
				BlueStacksUIBinding.BindColor(this.mGamepadTabBorder, Border.BorderBrushProperty, "GuidanceKeyBorderBackgroundColor");
				BlueStacksUIBinding.BindColor(this.mKeyboardTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
				this.mKeyboardTabBorder.BorderThickness = new Thickness(0.0);
				this.AddGuidanceCategories();
				this.Init(true);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in switching to Gamepad tab: " + ex.ToString());
			}
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x00050250 File Offset: 0x0004E450
		private void mKeyboardTabBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				KMManager.CheckAndCreateNewScheme();
				foreach (IMAction imaction in this.ListAction)
				{
					if (!string.Equals(imaction.GuidanceCategory, this.mGuidanceCategoryComboBox.mAutoComboBox.Text, StringComparison.InvariantCulture))
					{
						imaction.GuidanceCategory = this.mGuidanceCategoryComboBox.mAutoComboBox.Text;
						KeymapCanvasWindow.sIsDirty = true;
						this.ParentWindow.SelectedConfig.AddString(imaction.GuidanceCategory);
					}
				}
				this.AddGuidanceCategories();
				this.mKeyboardTabBorder.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
				this.mKeyboardTabBorder.Background = Brushes.Transparent;
				BlueStacksUIBinding.BindColor(this.mGamepadTabBorder, Border.BackgroundProperty, "GuidanceKeyBorderBackgroundColor");
				this.mGamepadTabBorder.BorderThickness = new Thickness(0.0);
				this.Init(false);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in switching to Keyboard tab: " + ex.ToString());
			}
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x000503B0 File Offset: 0x0004E5B0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/keymapextrasettingwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000D9E RID: 3486 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x000503E0 File Offset: 0x0004E5E0
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
				this.mHeaderGrid = (Grid)target;
				return;
			case 2:
				this.mHeader = (TextBlock)target;
				return;
			case 3:
				((CustomPictureBox)target).PreviewMouseLeftButtonDown += this.CloseButton_MouseLeftButtonDown;
				return;
			case 4:
				this.mScrollBar = (CustomScrollViewer)target;
				return;
			case 5:
				this.mDeleteButton = (CustomButton)target;
				this.mDeleteButton.Click += this.DeleteButton_Click;
				return;
			case 6:
				this.mDummyGrid = (Grid)target;
				return;
			case 7:
				this.mMOBAGB = (GroupBox)target;
				return;
			case 8:
				this.mMOBAPanel = (StackPanel)target;
				return;
			case 9:
				this.mMOBACB = (CustomCheckbox)target;
				this.mMOBACB.Checked += this.MOBAHeroCB_CheckedChanged;
				this.mMOBACB.Unchecked += this.MOBAHeroCB_CheckedChanged;
				return;
			case 10:
				this.mMOBAPB = (CustomPictureBox)target;
				this.mMOBAPB.MouseEnter += this.CustomPictureBox_MouseEnter;
				this.mMOBAPB.MouseLeave += this.CustomPictureBox_MouseLeave;
				this.mMOBAPB.MouseDown += this.MOBAHeroPictureBox_MouseDown;
				return;
			case 11:
				this.mGuidanceCategory = (GroupBox)target;
				return;
			case 12:
				this.mGuidanceCategoryComboBox = (AutoCompleteComboBox)target;
				return;
			case 13:
				this.mTabsGrid = (GroupBox)target;
				return;
			case 14:
				this.mKeyboardTabBorder = (Border)target;
				this.mKeyboardTabBorder.MouseLeftButtonUp += this.mKeyboardTabBorder_MouseLeftButtonUp;
				return;
			case 15:
				this.keyboardBtn = (TextBlock)target;
				return;
			case 16:
				this.mGamepadTabBorder = (Border)target;
				this.mGamepadTabBorder.MouseLeftButtonUp += this.mGamepadTabBorder_MouseLeftButtonUp;
				return;
			case 17:
				this.gamepadBtn = (TextBlock)target;
				return;
			case 18:
				this.mMOBASkillCancelGB = (GroupBox)target;
				return;
			case 19:
				this.mMOBASkillCancelGBPanel = (StackPanel)target;
				return;
			case 20:
				this.mMOBASkillCancelCB = (CustomCheckbox)target;
				this.mMOBASkillCancelCB.Checked += this.MOBASkillCancelCB_CheckedChanged;
				this.mMOBASkillCancelCB.Unchecked += this.MOBASkillCancelCB_CheckedChanged;
				return;
			case 21:
				this.mMOBASkillCancelPB = (CustomPictureBox)target;
				this.mMOBASkillCancelPB.MouseEnter += this.CustomPictureBox_MouseEnter;
				this.mMOBASkillCancelPB.MouseLeave += this.CustomPictureBox_MouseLeave;
				this.mMOBASkillCancelPB.MouseDown += this.MOBASkillCancelPictureBox_MouseDown;
				return;
			case 22:
				this.mLookAroundGB = (GroupBox)target;
				return;
			case 23:
				this.mLookAroundPanel = (StackPanel)target;
				return;
			case 24:
				this.mLookAroundCB = (CustomCheckbox)target;
				this.mLookAroundCB.Checked += this.LookAroundCB_CheckedChanged;
				this.mLookAroundCB.Unchecked += this.LookAroundCB_CheckedChanged;
				return;
			case 25:
				this.mLookAroundPB = (CustomPictureBox)target;
				this.mLookAroundPB.MouseEnter += this.CustomPictureBox_MouseEnter;
				this.mLookAroundPB.MouseLeave += this.CustomPictureBox_MouseLeave;
				this.mLookAroundPB.MouseDown += this.LookAroundPictureBox_MouseDown;
				return;
			case 26:
				this.mShootGB = (GroupBox)target;
				return;
			case 27:
				this.mShootGBPanel = (StackPanel)target;
				return;
			case 28:
				this.mShootCB = (CustomCheckbox)target;
				this.mShootCB.Checked += this.ShootCB_CheckedChanged;
				this.mShootCB.Unchecked += this.ShootCB_CheckedChanged;
				return;
			case 29:
				this.mShootPB = (CustomPictureBox)target;
				this.mShootPB.MouseEnter += this.CustomPictureBox_MouseEnter;
				this.mShootPB.MouseLeave += this.CustomPictureBox_MouseLeave;
				this.mShootPB.MouseDown += this.ShootPictureBox_MouseDown;
				return;
			case 30:
				this.mSchemesGB = (GroupBox)target;
				return;
			case 31:
				this.mEnableConditionGB = (GroupBox)target;
				return;
			case 32:
				this.mNoteGB = (GroupBox)target;
				return;
			case 33:
				this.mStartConditionGB = (GroupBox)target;
				return;
			case 34:
				this.mOverlayGB = (GroupBox)target;
				return;
			case 35:
				this.mOverlayCB = (CustomCheckbox)target;
				this.mOverlayCB.Checked += this.mOverlayCB_Checked;
				this.mOverlayCB.Unchecked += this.mOverlayCB_Unchecked;
				return;
			case 36:
				this.mCanvas = (Canvas)target;
				this.mCanvas.PreviewMouseMove += this.mCanvas_PreviewMouseMove;
				this.mCanvas.MouseUp += this.mCanvas_MouseUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x0000A6E1 File Offset: 0x000088E1
		[CompilerGenerated]
		private void <SetPopupDraggableProperty>g__deltaEventHandler|56_1(object o, DragDeltaEventArgs e)
		{
			base.HorizontalOffset += e.HorizontalChange;
			base.VerticalOffset += e.VerticalChange;
		}

		// Token: 0x04000862 RID: 2146
		private Dictionary<string, GroupBox> mDictGroupBox = new Dictionary<string, GroupBox>();

		// Token: 0x04000863 RID: 2147
		private Dictionary<string, DualTextBlockControl> mDictDualTextBox = new Dictionary<string, DualTextBlockControl>();

		// Token: 0x04000865 RID: 2149
		private List<string> mListSuggestions = new List<string>();

		// Token: 0x04000866 RID: 2150
		internal CanvasElement mCanvasElement;

		// Token: 0x04000867 RID: 2151
		private StackPanel mStackPanel;

		// Token: 0x04000868 RID: 2152
		private MainWindow ParentWindow;

		// Token: 0x04000869 RID: 2153
		private DualTextBlockControl mMOBADpadOriginXDTB;

		// Token: 0x0400086A RID: 2154
		private DualTextBlockControl mMOBADpadOriginYDTB;

		// Token: 0x0400086B RID: 2155
		private DualTextBlockControl mMOBADpadCharSpeedDTB;

		// Token: 0x0400086C RID: 2156
		private DualTextBlockControl mMOBADpadKeyDTB;

		// Token: 0x0400086D RID: 2157
		private DualTextBlockControl mMOBADpadXExprDTB;

		// Token: 0x0400086E RID: 2158
		private DualTextBlockControl mMOBADpadYExprDTB;

		// Token: 0x0400086F RID: 2159
		private DualTextBlockControl mMOBADpadXOverlayOffsetDTB;

		// Token: 0x04000870 RID: 2160
		private DualTextBlockControl mMOBADpadYOverlayOffsetDTB;

		// Token: 0x04000871 RID: 2161
		private DualTextBlockControl mMOBADpadHeroOriginXExprDTB;

		// Token: 0x04000872 RID: 2162
		private DualTextBlockControl mMOBADpadHeroOriginYExprDTB;

		// Token: 0x04000873 RID: 2163
		private DualTextBlockControl mLookAroundXDTB;

		// Token: 0x04000874 RID: 2164
		private DualTextBlockControl mLookAroundYDTB;

		// Token: 0x04000875 RID: 2165
		private DualTextBlockControl mLookAroundDTB;

		// Token: 0x04000876 RID: 2166
		private DualTextBlockControl mLookAroundXExprDTB;

		// Token: 0x04000877 RID: 2167
		private DualTextBlockControl mLookAroundYExprDTB;

		// Token: 0x04000878 RID: 2168
		private DualTextBlockControl mLookAroundXOffsetDTB;

		// Token: 0x04000879 RID: 2169
		private DualTextBlockControl mLookAroundYOffsetDTB;

		// Token: 0x0400087A RID: 2170
		private DualTextBlockControl mLookAroundShowOnOverlayDTB;

		// Token: 0x0400087B RID: 2171
		private DualTextBlockControl mShootXDTB;

		// Token: 0x0400087C RID: 2172
		private DualTextBlockControl mShootYDTB;

		// Token: 0x0400087D RID: 2173
		private DualTextBlockControl mShootDTB;

		// Token: 0x0400087E RID: 2174
		private DualTextBlockControl mShootXExprDTB;

		// Token: 0x0400087F RID: 2175
		private DualTextBlockControl mShootYExprDTB;

		// Token: 0x04000880 RID: 2176
		private DualTextBlockControl mShootXOffsetDTB;

		// Token: 0x04000881 RID: 2177
		private DualTextBlockControl mShootYOffsetDTB;

		// Token: 0x04000882 RID: 2178
		private DualTextBlockControl mShootShowOnOverlayDTB;

		// Token: 0x04000883 RID: 2179
		private DualTextBlockControl mMOBASkillCancelDTB;

		// Token: 0x04000884 RID: 2180
		private DualTextBlockControl mMOBASkillCancelXExprDTB;

		// Token: 0x04000885 RID: 2181
		private DualTextBlockControl mMOBASkillCancelYExprDTB;

		// Token: 0x04000886 RID: 2182
		private DualTextBlockControl mMOBASkillCancelXOffsetDTB;

		// Token: 0x04000887 RID: 2183
		private DualTextBlockControl mMOBASkillCancelYOffsetDTB;

		// Token: 0x04000888 RID: 2184
		private DualTextBlockControl mMOBASkillCancelShowOnOverlayDTB;

		// Token: 0x04000889 RID: 2185
		private DualTextBlockControl mEnableConditionTB;

		// Token: 0x0400088A RID: 2186
		private DualTextBlockControl mStartConditionTB;

		// Token: 0x0400088B RID: 2187
		private DualTextBlockControl mNoteTB;

		// Token: 0x0400088C RID: 2188
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHeaderGrid;

		// Token: 0x0400088D RID: 2189
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHeader;

		// Token: 0x0400088E RID: 2190
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomScrollViewer mScrollBar;

		// Token: 0x0400088F RID: 2191
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mDeleteButton;

		// Token: 0x04000890 RID: 2192
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mDummyGrid;

		// Token: 0x04000891 RID: 2193
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mMOBAGB;

		// Token: 0x04000892 RID: 2194
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMOBAPanel;

		// Token: 0x04000893 RID: 2195
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mMOBACB;

		// Token: 0x04000894 RID: 2196
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMOBAPB;

		// Token: 0x04000895 RID: 2197
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mGuidanceCategory;

		// Token: 0x04000896 RID: 2198
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AutoCompleteComboBox mGuidanceCategoryComboBox;

		// Token: 0x04000897 RID: 2199
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mTabsGrid;

		// Token: 0x04000898 RID: 2200
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mKeyboardTabBorder;

		// Token: 0x04000899 RID: 2201
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock keyboardBtn;

		// Token: 0x0400089A RID: 2202
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mGamepadTabBorder;

		// Token: 0x0400089B RID: 2203
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock gamepadBtn;

		// Token: 0x0400089C RID: 2204
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mMOBASkillCancelGB;

		// Token: 0x0400089D RID: 2205
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMOBASkillCancelGBPanel;

		// Token: 0x0400089E RID: 2206
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mMOBASkillCancelCB;

		// Token: 0x0400089F RID: 2207
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMOBASkillCancelPB;

		// Token: 0x040008A0 RID: 2208
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mLookAroundGB;

		// Token: 0x040008A1 RID: 2209
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mLookAroundPanel;

		// Token: 0x040008A2 RID: 2210
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mLookAroundCB;

		// Token: 0x040008A3 RID: 2211
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mLookAroundPB;

		// Token: 0x040008A4 RID: 2212
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mShootGB;

		// Token: 0x040008A5 RID: 2213
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mShootGBPanel;

		// Token: 0x040008A6 RID: 2214
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mShootCB;

		// Token: 0x040008A7 RID: 2215
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mShootPB;

		// Token: 0x040008A8 RID: 2216
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mSchemesGB;

		// Token: 0x040008A9 RID: 2217
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mEnableConditionGB;

		// Token: 0x040008AA RID: 2218
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mNoteGB;

		// Token: 0x040008AB RID: 2219
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mStartConditionGB;

		// Token: 0x040008AC RID: 2220
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GroupBox mOverlayGB;

		// Token: 0x040008AD RID: 2221
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mOverlayCB;

		// Token: 0x040008AE RID: 2222
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Canvas mCanvas;

		// Token: 0x040008AF RID: 2223
		private bool _contentLoaded;
	}
}
