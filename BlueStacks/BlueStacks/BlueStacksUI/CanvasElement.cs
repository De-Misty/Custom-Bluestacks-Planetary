using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200015A RID: 346
	public class CanvasElement : UserControl, IComponentConnector
	{
		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x0000A9B6 File Offset: 0x00008BB6
		public List<IMAction> ListActionItem { get; } = new List<IMAction>();

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000E19 RID: 3609 RVA: 0x0000A9BE File Offset: 0x00008BBE
		// (set) Token: 0x06000E1A RID: 3610 RVA: 0x0000A9C6 File Offset: 0x00008BC6
		public KeyActionType ActionType
		{
			get
			{
				return this.mType;
			}
			set
			{
				this.mType = value;
				this.SetActiveImage(false);
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000E1B RID: 3611 RVA: 0x0000A9D6 File Offset: 0x00008BD6
		internal MOBASkillSettingsPopup MOBASkillSettingsPopup
		{
			get
			{
				if (this.mMOBASkillSettingsPopup == null)
				{
					this.mMOBASkillSettingsPopup = new MOBASkillSettingsPopup(this);
				}
				return this.mMOBASkillSettingsPopup;
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000E1C RID: 3612 RVA: 0x0000A9F2 File Offset: 0x00008BF2
		internal MOBAOtherSettingsMoreInfoPopup MOBAOtherSettingsMoreInfoPopup
		{
			get
			{
				if (this.mMOBAOtherSettingsMoreInfoPopup == null)
				{
					this.mMOBAOtherSettingsMoreInfoPopup = new MOBAOtherSettingsMoreInfoPopup(this);
				}
				return this.mMOBAOtherSettingsMoreInfoPopup;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000E1D RID: 3613 RVA: 0x0000AA0E File Offset: 0x00008C0E
		private SkillIconToolTipPopup SkillIconToolTipPopup
		{
			get
			{
				if (this.mSkillIconToolTipPopup == null)
				{
					this.mSkillIconToolTipPopup = new SkillIconToolTipPopup(this);
				}
				return this.mSkillIconToolTipPopup;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000E1E RID: 3614 RVA: 0x0000AA2A File Offset: 0x00008C2A
		internal MOBASkillSettingsMoreInfoPopup MOBASkillSettingsMoreInfoPopup
		{
			get
			{
				if (this.mMOBASkillSettingsMoreInfoPopup == null)
				{
					this.mMOBASkillSettingsMoreInfoPopup = new MOBASkillSettingsMoreInfoPopup(this);
				}
				return this.mMOBASkillSettingsMoreInfoPopup;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000E1F RID: 3615 RVA: 0x0000AA46 File Offset: 0x00008C46
		// (set) Token: 0x06000E20 RID: 3616 RVA: 0x0000AA4E File Offset: 0x00008C4E
		public bool IsRemoveIfEmpty { get; internal set; }

		// Token: 0x06000E21 RID: 3617 RVA: 0x0000AA57 File Offset: 0x00008C57
		public CanvasElement(KeymapCanvasWindow window, MainWindow parentWindow)
		{
			this.mParentWindow = window;
			this.ParentWindow = parentWindow;
			this.InitializeComponent();
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x00055150 File Offset: 0x00053350
		internal void AddAction(IMAction action)
		{
			this.ListActionItem.Add(action);
			this.ActionType = (KeyActionType)Enum.Parse(typeof(KeyActionType), action.GetType().ToString());
			this.SetKeysForActions(this.ListActionItem);
			this.SetSize(action);
			this.SetElementLayout(true, 0.0, 0.0);
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x000551BC File Offset: 0x000533BC
		private void SetKeysForActions(List<IMAction> lst)
		{
			foreach (KeyValuePair<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>> keyValuePair in this.dictTextElemets)
			{
				keyValuePair.Value.Item3.Visibility = Visibility.Collapsed;
			}
			foreach (IMAction imaction in lst)
			{
				this.SetKeysForAction(imaction);
			}
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x00055258 File Offset: 0x00053458
		private void SetKeysForAction(IMAction action)
		{
			KeyActionType type = action.Type;
			switch (type)
			{
			case KeyActionType.Tap:
				this.SetKeys(action, "Key", "", "", "", "");
				return;
			case KeyActionType.Swipe:
				this.mColumn0.Width = new GridLength(0.0, GridUnitType.Star);
				this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
				this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn4.Width = new GridLength(0.0, GridUnitType.Star);
				this.mRow0.Height = new GridLength(5.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(50.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
				if (action.Direction == Direction.Left)
				{
					this.SetKeys(action, "", "Key", "", "", "");
					return;
				}
				if (action.Direction == Direction.Right)
				{
					this.SetKeys(action, "", "", "", "Key", "");
					return;
				}
				if (action.Direction == Direction.Up)
				{
					this.SetKeys(action, "", "", "Key", "", "");
					return;
				}
				if (action.Direction == Direction.Down)
				{
					this.SetKeys(action, "", "", "", "", "Key");
					return;
				}
				break;
			case KeyActionType.Dpad:
				this.mColumn0.Width = new GridLength(10.0, GridUnitType.Star);
				this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn2.Width = new GridLength(20.0, GridUnitType.Star);
				this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn4.Width = new GridLength(10.0, GridUnitType.Star);
				this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
				this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
				return;
			case KeyActionType.Zoom:
				this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
				this.mColumn1.Width = new GridLength(72.0);
				this.mColumn2.Width = new GridLength(70.0, GridUnitType.Star);
				this.mColumn3.Width = new GridLength(30.0);
				this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
				this.mRow0.Height = new GridLength(80.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(35.0);
				this.mRow2.Height = new GridLength(35.0);
				this.mRow3.Height = new GridLength(0.0);
				this.mRow4.Height = new GridLength(80.0, GridUnitType.Star);
				this.SetKeys(action, "", "KeyOut", "KeyIn", "", "");
				return;
			case KeyActionType.Tilt:
				this.mColumn0.Width = new GridLength(0.0, GridUnitType.Star);
				this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
				this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn4.Width = new GridLength(0.0, GridUnitType.Star);
				this.mRow0.Height = new GridLength(5.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(50.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
				this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
				return;
			case KeyActionType.Pan:
			{
				Pan pan = action as Pan;
				this.SetKeys(action, "KeyStartStop", "", "", "", "");
				if (this.mParentWindow.dictCanvasElement.ContainsKey(action) && pan.IsLookAroundEnabled && this.mParentWindow.dictCanvasElement.ContainsKey(pan.mLookAround))
				{
					this.mParentWindow.dictCanvasElement[pan.mLookAround].SetKeysForAction(pan.mLookAround);
					return;
				}
				break;
			}
			case KeyActionType.MOBADpad:
				this.SetKeys(action, "", "", "", "", "");
				return;
			case KeyActionType.MOBASkill:
			{
				MOBASkill mobaskill = action as MOBASkill;
				this.SetKeys(action, "KeyActivate", "", "", "", "");
				if (this.mParentWindow.dictCanvasElement.ContainsKey(action) && mobaskill.IsCancelSkillEnabled && this.mParentWindow.dictCanvasElement.ContainsKey(mobaskill.mMOBASkillCancel))
				{
					this.mParentWindow.dictCanvasElement[mobaskill.mMOBASkillCancel].SetKeysForAction(mobaskill.mMOBASkillCancel);
					return;
				}
				break;
			}
			case KeyActionType.Raw:
			case KeyActionType.KeyInput:
			case KeyActionType.SendOriginalKeys:
			case KeyActionType.DontForwardKeys:
			case KeyActionType.Scroll:
				break;
			case KeyActionType.Script:
				this.SetKeys(action, "Key", "", "", "", "");
				return;
			case KeyActionType.TapRepeat:
				this.mRow1.Height = new GridLength(0.0);
				this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(4.0, GridUnitType.Star);
				this.mCountText.Text = ((TapRepeat)action).Count.ToString(CultureInfo.InvariantCulture);
				this.mKeyRepeatGrid.Visibility = Visibility.Visible;
				this.SetToggleModeValues(action);
				this.SetKeys(action, "Key", "", "", "", "");
				return;
			case KeyActionType.Rotate:
				this.mColumn0.Width = new GridLength(0.0);
				this.mColumn1.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
				this.mColumn3.Width = new GridLength(30.0, GridUnitType.Star);
				this.mColumn4.Width = new GridLength(0.0);
				this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
				this.SetKeys(action, "", "KeyAntiClock", "", "KeyClock", "");
				return;
			case KeyActionType.State:
				if (KMManager.sIsDeveloperModeOn)
				{
					this.SetKeys(action, "Key", "", "", "", "");
					return;
				}
				break;
			case KeyActionType.FreeLook:
				this.SetToggleModeValues(action);
				return;
			case KeyActionType.MouseZoom:
				if (KMManager.sIsDeveloperModeOn)
				{
					this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
					this.mColumn1.Width = new GridLength(72.0);
					this.mColumn2.Width = new GridLength(70.0, GridUnitType.Star);
					this.mColumn3.Width = new GridLength(30.0);
					this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
					this.mRow0.Height = new GridLength(80.0, GridUnitType.Star);
					this.mRow1.Height = new GridLength(35.0);
					this.mRow2.Height = new GridLength(35.0);
					this.mRow3.Height = new GridLength(0.0);
					this.mRow4.Height = new GridLength(80.0, GridUnitType.Star);
					this.SetKeys(action, "Key", "", "", "", "");
					return;
				}
				break;
			case KeyActionType.EdgeScroll:
				this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
				this.mActionIcon.MinHeight = 60.0;
				this.mActionIcon.MinWidth = 60.0;
				this.SetKeys(action, "", "", "", "", "");
				return;
			case KeyActionType.Callback:
				this.mRow0.Height = new GridLength(15.0, GridUnitType.Star);
				this.mRow1.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
				this.mRow3.Height = new GridLength(20.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(15.0, GridUnitType.Star);
				this.mActionIcon.MinHeight = 60.0;
				this.mActionIcon.MinWidth = 60.0;
				this.SetKeys(action, "Id", "", "", "", "");
				break;
			default:
				switch (type)
				{
				case KeyActionType.LookAround:
					this.SetKeys(action, "Key", "", "", "", "");
					return;
				case KeyActionType.PanShoot:
					if (this.mParentWindow.IsInOverlayMode)
					{
						this.SetKeys(action, "Key", "", "", "", "");
						return;
					}
					break;
				case KeyActionType.MOBASkillCancel:
					this.SetKeys(action, "Key", "", "", "", "");
					return;
				default:
					return;
				}
				break;
			}
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x00055E90 File Offset: 0x00054090
		internal void SetToggleModeValues(IMAction action)
		{
			this.mToggleModeGrid.Visibility = Visibility.Visible;
			KeyActionType type = action.Type;
			if (type != KeyActionType.TapRepeat)
			{
				if (type != KeyActionType.FreeLook)
				{
					return;
				}
				BlueStacksUIBinding.Bind(this.mToggleMode1, "STRING_KEYBOARD_MODE", "");
				BlueStacksUIBinding.Bind(this.mToggleMode2, "STRING_MOUSE_MODE", "");
				if (((FreeLook)action).DeviceType == 0)
				{
					base.MinHeight = 182.0;
					this.mToggleImage.ImageName = "left_switch";
					this.mColumn0.Width = new GridLength(0.0);
					this.mColumn1.Width = new GridLength(25.0, GridUnitType.Star);
					this.mColumn2.Width = new GridLength(40.0, GridUnitType.Star);
					this.mColumn3.Width = new GridLength(25.0, GridUnitType.Star);
					this.mColumn4.Width = new GridLength(0.0);
					this.mRow0.Height = new GridLength(10.0, GridUnitType.Star);
					this.mRow1.Height = new GridLength(25.0, GridUnitType.Star);
					this.mRow2.Height = new GridLength(30.0, GridUnitType.Star);
					this.mRow3.Height = new GridLength(25.0, GridUnitType.Star);
					this.mRow4.Height = new GridLength(10.0, GridUnitType.Star);
					this.SetKeys(action, "", "KeyLeft", "KeyUp", "KeyRight", "KeyDown");
				}
				else if (((FreeLook)action).DeviceType == 1)
				{
					base.MinHeight = 117.0;
					this.mToggleImage.ImageName = "right_switch";
					this.SetKeys(action, "Key", "", "", "", "");
				}
				this.SetActiveImage(true);
				return;
			}
			else
			{
				base.MinHeight = 92.0;
				BlueStacksUIBinding.Bind(this.mToggleMode1, "STRING_TAP_MODE", "");
				BlueStacksUIBinding.Bind(this.mToggleMode2, "STRING_LONG_PRESS_MODE", "");
				if (((TapRepeat)action).RepeatUntilKeyUp)
				{
					this.mToggleImage.ImageName = "right_switch";
					return;
				}
				this.mToggleImage.ImageName = "left_switch";
				return;
			}
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x000560F4 File Offset: 0x000542F4
		private void InsertScriptSettingsClickGrid()
		{
			Grid grid = new Grid
			{
				Height = 19.0,
				Width = 19.0,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				Margin = new Thickness(30.0, 0.0, 0.0, 3.0),
				Background = Brushes.Black,
				Opacity = 0.0001
			};
			Grid.SetRow(grid, 3);
			Grid.SetRowSpan(grid, 2);
			Grid.SetColumn(grid, 3);
			grid.PreviewMouseLeftButtonUp += this.ScriptSettingsGrid_MouseLeftButtonUp;
			grid.MouseEnter += this.ScriptSettingsGrid_MouseEnter;
			grid.MouseLeave += this.ScriptSettingsGrid_MouseLeave;
			this.mGrid.Children.Add(grid);
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x000561DC File Offset: 0x000543DC
		private void ScriptSettingsGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			string text = this.ActionType.ToString();
			this.mActionIcon.ImageName = text + "_canvas";
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x00056214 File Offset: 0x00054414
		private void ScriptSettingsGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			string text = this.ActionType.ToString();
			this.mActionIcon.ImageName = text + "_canvas_hover";
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x0005624C File Offset: 0x0005444C
		private static bool CheckForOffsetValueInGameControl(IMAction action)
		{
			bool flag = true;
			switch (action.Type)
			{
			case KeyActionType.LookAround:
				if (string.IsNullOrEmpty(((LookAround)action).LookAroundXOverlayOffset) && string.IsNullOrEmpty(((LookAround)action).LookAroundYOverlayOffset))
				{
					flag = false;
				}
				break;
			case KeyActionType.PanShoot:
				if (string.IsNullOrEmpty(((PanShoot)action).LButtonXOverlayOffset) && string.IsNullOrEmpty(((PanShoot)action).LButtonXOverlayOffset))
				{
					flag = false;
				}
				break;
			case KeyActionType.MOBASkillCancel:
				if (string.IsNullOrEmpty(((MOBASkillCancel)action).MOBASkillCancelOffsetX) && string.IsNullOrEmpty(((MOBASkillCancel)action).MOBASkillCancelOffsetY))
				{
					flag = false;
				}
				break;
			default:
				if (string.IsNullOrEmpty(action.XOverlayOffset) && string.IsNullOrEmpty(action.YOverlayOffset))
				{
					return false;
				}
				break;
			}
			return flag;
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00056314 File Offset: 0x00054514
		private void SetKeys(IMAction action, string center, string left, string up, string right, string down)
		{
			if (this.mParentWindow.IsInOverlayMode && action.Type == KeyActionType.FreeLook)
			{
				this.ShowOverlayKeysOnImage(center, action, 0, 0);
			}
			if (!this.mParentWindow.IsInOverlayMode && action.Type == KeyActionType.Script)
			{
				this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
				this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
				this.mColumn0.Width = new GridLength(2.0, GridUnitType.Star);
				this.InsertScriptSettingsClickGrid();
			}
			string text = string.Empty;
			if (!string.IsNullOrEmpty(center))
			{
				if (!string.IsNullOrEmpty(action[center].ToString()))
				{
					text = KMManager.GetKeyUIValue(action[center].ToString());
				}
				if (this.mParentWindow.IsInOverlayMode)
				{
					base.MinHeight = 50.0;
					base.MinWidth = 50.0;
					if (!string.IsNullOrEmpty(action[center].ToString()) && !Constants.ImapGameControlsHiddenInOverlayList.Contains(action.Type.ToString()))
					{
						if (CanvasElement.CheckForOffsetValueInGameControl(action))
						{
							if (action.Type == KeyActionType.Script)
							{
								this.GetLabelsForOverlay(Positions.Center, text, action, 3, 3, center);
							}
							else
							{
								this.GetLabelsForOverlay(Positions.Center, text, action, 2, 2, center);
							}
						}
						else
						{
							if (action.Type == KeyActionType.Tap)
							{
								this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
								this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
								this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.TapRepeat)
							{
								base.MinWidth = 50.0;
								base.MinHeight = 50.0;
								this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
								this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
								this.mRow1.Height = new GridLength(1.0, GridUnitType.Auto);
								this.mRow2.Height = new GridLength(3.0, GridUnitType.Star);
								this.mRow3.Height = new GridLength(1.0, GridUnitType.Auto);
								this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.MOBASkill)
							{
								this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
								this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
								this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.MOBASkillCancel || action.Type == KeyActionType.Script)
							{
								this.mColumn4.Width = new GridLength(6.0, GridUnitType.Star);
								this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
								this.mColumn0.Width = new GridLength(6.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.Pan || action.Type == KeyActionType.PanShoot || action.Type == KeyActionType.LookAround)
							{
								this.mColumn4.Width = new GridLength(5.0, GridUnitType.Star);
								this.mRow4.Height = new GridLength(5.0, GridUnitType.Star);
								this.mColumn0.Width = new GridLength(5.0, GridUnitType.Star);
							}
							if (action.Type != KeyActionType.FreeLook)
							{
								this.GetLabelsForOverlay(Positions.Center, text, action, 4, 4, center);
							}
						}
					}
				}
				else
				{
					TextBlock newTextBlock = this.GetNewTextBlock(Positions.Center, center, action);
					if (action.Type == KeyActionType.Script)
					{
						Grid.SetColumn(newTextBlock, 3);
						Grid.SetRow(newTextBlock, 3);
					}
					else
					{
						Grid.SetColumn(newTextBlock, 2);
						Grid.SetRow(newTextBlock, 2);
					}
					BlueStacksUIBinding.Bind(newTextBlock, KMManager.GetStringsToShowInUI(text.ToString(CultureInfo.InvariantCulture)), "");
					if (action.Type != KeyActionType.MouseZoom && action.Type != KeyActionType.Callback)
					{
						newTextBlock.Visibility = Visibility.Visible;
						newTextBlock.ToolTip = newTextBlock.Text;
					}
					else
					{
						newTextBlock.Visibility = Visibility.Collapsed;
					}
				}
			}
			if (!string.IsNullOrEmpty(left))
			{
				if (!string.IsNullOrEmpty(action[left].ToString()))
				{
					text = KMManager.GetKeyUIValue(action[left].ToString());
				}
				if (this.mParentWindow.IsInOverlayMode)
				{
					if (!string.IsNullOrEmpty(action[left].ToString()) && !Constants.ImapGameControlsHiddenInOverlayList.Contains(action.Type.ToString()))
					{
						if (CanvasElement.CheckForOffsetValueInGameControl(action))
						{
							this.GetLabelsForOverlay(Positions.Center, text, action, 2, 1, left);
						}
						else
						{
							if (action.Type == KeyActionType.Dpad)
							{
								this.mColumn1.Width = new GridLength(50.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.Rotate)
							{
								this.mColumn1.Width = new GridLength(60.0, GridUnitType.Star);
								this.ShowRotateControlOverlay(2, 1, 16.0, "overlay_left_arrow");
							}
							if (action.Type == KeyActionType.FreeLook)
							{
								this.mColumn2.Width = new GridLength(30.0, GridUnitType.Star);
								this.mColumn3.Width = new GridLength(40.0, GridUnitType.Star);
								this.GetLabelsForOverlay(Positions.Left, text, action, 2, 2, left);
							}
							else
							{
								this.GetLabelsForOverlay(Positions.Left, text, action, 2, 1, left);
							}
						}
					}
				}
				else
				{
					TextBlock newTextBlock2 = this.GetNewTextBlock(Positions.Left, left, action);
					if (action.Type == KeyActionType.Zoom)
					{
						Grid.SetColumn(newTextBlock2, 2);
						Grid.SetRow(newTextBlock2, 2);
					}
					else
					{
						Grid.SetColumn(newTextBlock2, 1);
						Grid.SetRow(newTextBlock2, 2);
					}
					BlueStacksUIBinding.Bind(newTextBlock2, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[left].ToString()), "");
					newTextBlock2.Visibility = Visibility.Visible;
					newTextBlock2.ToolTip = newTextBlock2.Text;
					if (action.Type == KeyActionType.Zoom)
					{
						BlueStacksUIBinding.BindColor(newTextBlock2, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
					}
				}
			}
			if (!string.IsNullOrEmpty(up))
			{
				if (!string.IsNullOrEmpty(action[up].ToString()))
				{
					text = KMManager.GetKeyUIValue(action[up].ToString());
				}
				if (this.mParentWindow.IsInOverlayMode)
				{
					if (!string.IsNullOrEmpty(action[up].ToString()) && !Constants.ImapGameControlsHiddenInOverlayList.Contains(action.Type.ToString()))
					{
						if (CanvasElement.CheckForOffsetValueInGameControl(action))
						{
							this.GetLabelsForOverlay(Positions.Center, text, action, 1, 2, up);
						}
						else
						{
							if (action.Type == KeyActionType.Dpad)
							{
								this.mColumn2.Width = new GridLength(50.0, GridUnitType.Star);
							}
							if (action.Type == KeyActionType.FreeLook)
							{
								this.GetLabelsForOverlay(Positions.Up, text, action, 1, 3, up);
							}
							else
							{
								this.GetLabelsForOverlay(Positions.Up, text, action, 1, 2, up);
							}
						}
					}
				}
				else
				{
					TextBlock newTextBlock3 = this.GetNewTextBlock(Positions.Up, up, action);
					Grid.SetColumn(newTextBlock3, 2);
					Grid.SetRow(newTextBlock3, 1);
					BlueStacksUIBinding.Bind(newTextBlock3, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[up].ToString()), "");
					newTextBlock3.Visibility = Visibility.Visible;
					newTextBlock3.ToolTip = newTextBlock3.Text;
					if (action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom)
					{
						BlueStacksUIBinding.BindColor(newTextBlock3, TextBlock.BackgroundProperty, "CanvasElementsBackgroundColor");
					}
				}
			}
			if (!string.IsNullOrEmpty(right))
			{
				if (!string.IsNullOrEmpty(action[right].ToString()))
				{
					text = KMManager.GetKeyUIValue(action[right].ToString());
				}
				if (this.mParentWindow.IsInOverlayMode)
				{
					if (!string.IsNullOrEmpty(action[right].ToString()) && !Constants.ImapGameControlsHiddenInOverlayList.Contains(action.Type.ToString()))
					{
						if (CanvasElement.CheckForOffsetValueInGameControl(action))
						{
							this.GetLabelsForOverlay(Positions.Center, text, action, 2, 3, right);
						}
						else
						{
							if (action.Type == KeyActionType.Dpad)
							{
								this.mColumn3.Width = new GridLength(50.0, GridUnitType.Star);
							}
							else if (action.Type == KeyActionType.Rotate)
							{
								this.mColumn3.Width = new GridLength(60.0, GridUnitType.Star);
								this.ShowRotateControlOverlay(2, 3, -16.0, "overlay_right_arrow");
							}
							if (action.Type == KeyActionType.FreeLook)
							{
								this.mColumn3.Width = new GridLength(70.0);
								this.mColumn4.Width = new GridLength(30.0, GridUnitType.Star);
								this.GetLabelsForOverlay(Positions.Right, text, action, 2, 4, right);
							}
							else
							{
								this.GetLabelsForOverlay(Positions.Right, text, action, 2, 3, right);
							}
						}
					}
				}
				else
				{
					TextBlock newTextBlock4 = this.GetNewTextBlock(Positions.Right, right, action);
					Grid.SetColumn(newTextBlock4, 3);
					Grid.SetRow(newTextBlock4, 2);
					BlueStacksUIBinding.Bind(newTextBlock4, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[right].ToString()), "");
					newTextBlock4.ToolTip = newTextBlock4.Text;
					newTextBlock4.Visibility = Visibility.Visible;
				}
			}
			if (!string.IsNullOrEmpty(down))
			{
				if (!string.IsNullOrEmpty(action[down].ToString()))
				{
					text = KMManager.GetKeyUIValue(action[down].ToString());
				}
				if (this.mParentWindow.IsInOverlayMode)
				{
					if (!string.IsNullOrEmpty(action[down].ToString()) && !Constants.ImapGameControlsHiddenInOverlayList.Contains(action.Type.ToString()))
					{
						if (CanvasElement.CheckForOffsetValueInGameControl(action))
						{
							this.GetLabelsForOverlay(Positions.Center, text, action, 3, 2, down);
						}
						else
						{
							if (action.Type == KeyActionType.Dpad)
							{
								this.mColumn2.Width = new GridLength(50.0, GridUnitType.Star);
							}
							if (action.Type == KeyActionType.FreeLook)
							{
								this.GetLabelsForOverlay(Positions.Down, text, action, 3, 3, down);
							}
							else
							{
								this.GetLabelsForOverlay(Positions.Down, text, action, 3, 2, down);
							}
						}
					}
				}
				else
				{
					TextBlock newTextBlock5 = this.GetNewTextBlock(Positions.Down, down, action);
					Grid.SetColumn(newTextBlock5, 2);
					Grid.SetRow(newTextBlock5, 3);
					BlueStacksUIBinding.Bind(newTextBlock5, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[down].ToString()), "");
					newTextBlock5.ToolTip = newTextBlock5.Text;
					newTextBlock5.Visibility = Visibility.Visible;
				}
			}
			if (action.Type == KeyActionType.Dpad && (action as Dpad).IsMOBADpadEnabled && this.mParentWindow.IsInOverlayMode)
			{
				this.mGrid.Visibility = Visibility.Visible;
				this.mColumn2.Width = new GridLength(70.0, GridUnitType.Star);
				this.mRow2.Height = new GridLength(70.0, GridUnitType.Star);
				this.GetLabelsForOverlay(Positions.Center, string.Empty, action, 2, 2, center);
			}
			if (this.mParentWindow.IsInOverlayMode)
			{
				this.mCanvasGrid.Visibility = Visibility.Collapsed;
				this.mToggleModeGrid.Visibility = Visibility.Collapsed;
				return;
			}
			this.mCanvasGrid.Visibility = Visibility.Visible;
			if (action.Type == KeyActionType.TapRepeat || action.Type == KeyActionType.FreeLook)
			{
				this.mToggleModeGrid.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x00056E3C File Offset: 0x0005503C
		private void ShowRotateControlOverlay(int row, int column, double margin, string imageName)
		{
			Grid grid = new Grid
			{
				MinHeight = 50.0,
				MinWidth = 50.0,
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center
			};
			CustomPictureBox customPictureBox = new CustomPictureBox
			{
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			customPictureBox.ImageName = imageName;
			grid.Children.Add(customPictureBox);
			grid.Margin = new Thickness(margin, 0.0, 0.0, 0.0);
			Grid.SetRow(grid, row);
			Grid.SetColumn(grid, column);
			this.mGrid.Children.Add(grid);
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x00056EFC File Offset: 0x000550FC
		private void ShowOverlayKeysOnImage(string s, IMAction action, int row, int column)
		{
			Grid grid = new Grid
			{
				MinHeight = 50.0,
				MinWidth = 60.0,
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center
			};
			CustomPictureBox customPictureBox = new CustomPictureBox
			{
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			if (((FreeLook)action).DeviceType == 0)
			{
				customPictureBox.Height = 196.0;
				customPictureBox.Width = 98.0;
				customPictureBox.ImageName = "overlay_keyboard";
				customPictureBox.Margin = new Thickness(0.0, 6.0, 0.0, 0.0);
				grid.Children.Add(customPictureBox);
				Grid.SetRow(grid, row);
				Grid.SetColumn(grid, 1);
				Grid.SetRowSpan(grid, 5);
				Grid.SetColumnSpan(grid, 5);
			}
			else
			{
				customPictureBox.ImageName = "overlay_mouse";
				customPictureBox.HorizontalAlignment = HorizontalAlignment.Stretch;
				Label label = new Label();
				BlueStacksUIBinding.Bind(label, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(action[s].ToString()));
				label.Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
				label.Padding = new Thickness(2.0);
				label.FontSize = 11.0;
				label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
				label.FontWeight = FontWeights.DemiBold;
				label.HorizontalAlignment = HorizontalAlignment.Center;
				label.VerticalAlignment = VerticalAlignment.Center;
				Typeface typeface = new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch);
				FormattedText formattedText = new FormattedText(label.Content.ToString(), Thread.CurrentThread.CurrentCulture, label.FlowDirection, typeface, label.FontSize, label.Foreground);
				customPictureBox.Width = ((formattedText.WidthIncludingTrailingWhitespace + 10.0 > 40.0) ? (formattedText.WidthIncludingTrailingWhitespace + 10.0) : 40.0);
				customPictureBox.Height = ((customPictureBox.Width > 50.0) ? 50.0 : customPictureBox.Width);
				grid.Children.Add(customPictureBox);
				grid.Children.Add(label);
				Grid.SetRow(grid, 2);
				Grid.SetRowSpan(grid, 4);
				Grid.SetColumn(grid, column);
			}
			this.mGrid.Children.Add(grid);
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x000571A8 File Offset: 0x000553A8
		private void GetLabelsForOverlay(Positions p, string text, IMAction action, int row, int column, string key)
		{
			if (ShowImagesOnOverlay.ListShowImagesForKeys.Contains(action[key].ToString()))
			{
				Grid grid = new Grid
				{
					MinHeight = 29.0,
					MinWidth = 29.0,
					Visibility = Visibility.Visible,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				CustomPictureBox customPictureBox = new CustomPictureBox
				{
					Height = 27.0,
					Width = 27.0,
					Visibility = Visibility.Visible,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					ImageName = "Imap_" + action[key].ToString()
				};
				grid.Children.Add(customPictureBox);
				this.mGrid.Children.Add(grid);
				Grid.SetRow(grid, row);
				Grid.SetColumn(grid, column);
				return;
			}
			if (action.Type == KeyActionType.Dpad || action.Type == KeyActionType.MOBASkill || action.Type == KeyActionType.FreeLook || action.Type == KeyActionType.Rotate)
			{
				if (action.Type == KeyActionType.Dpad)
				{
					if ((action as Dpad).IsMOBADpadEnabled)
					{
						Grid grid2 = new Grid
						{
							MinHeight = 67.0,
							MinWidth = 67.0,
							Visibility = Visibility.Visible,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center
						};
						CustomPictureBox customPictureBox2 = new CustomPictureBox
						{
							Height = 67.0,
							Width = grid2.Width,
							Visibility = Visibility.Visible,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center,
							ImageName = "Imap_MOBADpad"
						};
						grid2.Children.Add(customPictureBox2);
						this.mGrid.Children.Add(grid2);
						Grid.SetRow(grid2, row);
						Grid.SetColumn(grid2, column);
						return;
					}
					Grid labelGrid = this.GetLabelGrid(text);
					Grid.SetRow(labelGrid, row);
					Grid.SetColumn(labelGrid, column);
					return;
				}
				else
				{
					Grid labelGrid2 = this.GetLabelGrid(text);
					Grid.SetRow(labelGrid2, row);
					Grid.SetColumn(labelGrid2, column);
					if (p.Equals(Positions.Up))
					{
						labelGrid2.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
					}
					if (p.Equals(Positions.Down))
					{
						labelGrid2.Margin = new Thickness(0.0, -20.0, 0.0, 0.0);
						return;
					}
				}
			}
			else
			{
				OverlayLabelControl overlayLabelControl = new OverlayLabelControl();
				BlueStacksUIBinding.Bind(overlayLabelControl.lbl, KMManager.GetStringsToShowInUI(text));
				overlayLabelControl.MinHeight = 27.0;
				overlayLabelControl.MinWidth = 27.0;
				overlayLabelControl.lbl.HorizontalAlignment = HorizontalAlignment.Center;
				overlayLabelControl.lbl.VerticalAlignment = VerticalAlignment.Center;
				overlayLabelControl.lbl.Margin = new Thickness(3.0, 0.0, 3.0, 1.0);
				overlayLabelControl.lbl.Padding = new Thickness(5.0);
				overlayLabelControl.lbl.FontSize = 11.0;
				this.mGrid.Children.Add(overlayLabelControl);
				Grid.SetRow(overlayLabelControl, row);
				Grid.SetColumn(overlayLabelControl, column);
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0005751C File Offset: 0x0005571C
		private Grid GetLabelGrid(string text)
		{
			Grid grid = new Grid
			{
				MinHeight = 28.0,
				MinWidth = 28.0,
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			Border border = new Border();
			Label label = new Label();
			BlueStacksUIBinding.BindColor(border, Border.BorderBrushProperty, "OverlayLabelBorderColor");
			RenderOptions.SetEdgeMode(border, EdgeMode.Unspecified);
			BlueStacksUIBinding.BindColor(border, Border.BackgroundProperty, "OverlayLabelBackgroundColor");
			border.SnapsToDevicePixels = false;
			border.ClipToBounds = false;
			border.CornerRadius = new CornerRadius(14.0);
			border.BorderThickness = new Thickness(1.5);
			BlueStacksUIBinding.Bind(label, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text));
			label.Margin = new Thickness(2.0, 0.0, 2.0, 1.0);
			label.Padding = new Thickness(1.0);
			label.FontSize = 11.0;
			label.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
			label.FontWeight = FontWeights.Bold;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.VerticalAlignment = VerticalAlignment.Center;
			grid.Children.Add(border);
			grid.Children.Add(label);
			this.mGrid.Children.Add(grid);
			return grid;
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00057694 File Offset: 0x00055894
		private TextBlock GetNewTextBlock(Positions p, string s, IMAction action)
		{
			TextBlock textBlock;
			if (this.dictTextElemets.ContainsKey(p))
			{
				this.dictTextElemets[p].Item4.Add(action);
				textBlock = this.dictTextElemets[p].Item3;
			}
			else
			{
				textBlock = new TextBlock
				{
					FontSize = 14.0,
					FontWeight = FontWeights.Bold,
					Background = Brushes.Transparent,
					TextTrimming = TextTrimming.CharacterEllipsis,
					Padding = new Thickness(5.0, 2.0, 5.0, 2.0),
					Foreground = Brushes.Black,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				textBlock.PreviewMouseUp += this.TxtBlock_PreviewMouseUp;
				textBlock.Name = p.ToString();
				TextBox textBox = new TextBox
				{
					FontSize = 14.0,
					FontWeight = FontWeights.Bold
				};
				BlueStacksUIBinding.BindColor(textBox, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				BlueStacksUIBinding.BindColor(textBox, Control.BorderBrushProperty, "ComboBoxBorderColor");
				BlueStacksUIBinding.BindColor(textBox, Control.ForegroundProperty, "ComboBoxForegroundColor");
				textBox.Padding = new Thickness(0.0, 1.0, 0.0, 1.0);
				textBox.HorizontalAlignment = HorizontalAlignment.Center;
				textBox.VerticalAlignment = VerticalAlignment.Center;
				textBox.TextAlignment = TextAlignment.Center;
				textBox.LostFocus += this.TxtBox_LostFocus;
				textBox.GotFocus += this.TxtBox_GotFocus;
				textBox.MinWidth = 24.0;
				textBox.TextWrapping = TextWrapping.Wrap;
				InputMethod.SetIsInputMethodEnabled(textBox, false);
				textBox.Name = p.ToString();
				textBox.Visibility = Visibility.Collapsed;
				textBox.PreviewMouseLeftButtonDown += this.SelectivelyIgnoreMouseButton;
				textBox.PreviewMouseDown += this.TxtBox_PreviewMouseDown;
				textBox.PreviewKeyDown += this.TxtBox_PreviewKeyDown;
				textBox.KeyUp += this.TxtBox_KeyUp;
				textBox.TextChanged += this.TxtBox_TextChanged;
				textBox.PreviewMouseWheel += this.TxtBox_PreviewMouseWheel;
				this.mGrid.Children.Add(textBlock);
				this.mGrid.Children.Add(textBox);
				this.dictTextElemets.Add(p, new BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>(s, textBox, textBlock, new List<IMAction> { action }));
			}
			return textBlock;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x0005791C File Offset: 0x00055B1C
		private void TxtBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (e.Delta > 0)
			{
				e.Handled = true;
				textBox.Tag = "MouseWheelUp";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
				return;
			}
			if (e.Delta < 0)
			{
				e.Handled = true;
				textBox.Tag = "MouseWheelDown";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
			}
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x00057994 File Offset: 0x00055B94
		private void TxtBox_KeyUp(object sender, KeyEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (this.ActionType == KeyActionType.Tap || this.ListActionItem[0].Type == KeyActionType.TapRepeat || this.ListActionItem[0].Type == KeyActionType.Script)
			{
				if (this.mKeyList.Count >= 2)
				{
					string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
					string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
					textBox.Text = text;
					textBox.Tag = text2;
					this.SetValueHandling(sender);
				}
				else if (this.mKeyList.Count == 1)
				{
					string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(0));
					string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(0));
					textBox.Text = text;
					textBox.Tag = text2;
					this.SetValueHandling(sender);
				}
				textBox.CaretIndex = textBox.Text.Length;
				this.mKeyList.Clear();
				return;
			}
			e.Handled = true;
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x0000AA94 File Offset: 0x00008C94
		internal void SetMousePoint(Point mousePoint)
		{
			this.mMousePointForTap = new Point?(mousePoint);
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x0000AAA2 File Offset: 0x00008CA2
		private void TxtBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.SetValueHandling(sender);
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x00057AFC File Offset: 0x00055CFC
		private void SetValueHandling(object sender)
		{
			TextBox textBox = sender as TextBox;
			Positions positions = EnumHelper.Parse<Positions>(textBox.Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
			string item = this.dictTextElemets[positions].Item1;
			string text = this.dictTextElemets[positions].Item4[0][item].ToString();
			if (textBox.Tag != null)
			{
				text = textBox.Tag.ToString();
			}
			this.Setvalue(textBox, text);
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x00057B78 File Offset: 0x00055D78
		private void Setvalue(TextBox mKeyTextBox, string value)
		{
			Positions positions = EnumHelper.Parse<Positions>(mKeyTextBox.Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
			string item = this.dictTextElemets[positions].Item1;
			foreach (IMAction imaction in this.dictTextElemets[positions].Item4)
			{
				if (!string.Equals(imaction[item].ToString(), value, StringComparison.InvariantCulture))
				{
					imaction[item] = value;
					KeymapCanvasWindow.sIsDirty = true;
				}
			}
			if (item.StartsWith("Key", StringComparison.InvariantCulture))
			{
				mKeyTextBox.Text = mKeyTextBox.Text;
			}
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x00057C38 File Offset: 0x00055E38
		private void TxtBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				return;
			}
			KMManager.CheckAndCreateNewScheme();
			if (this.IsRemoveIfEmpty)
			{
				this.IsRemoveIfEmpty = false;
				this.UpdatePosition(Canvas.GetTop(this), Canvas.GetLeft(this));
			}
			TextBox textBox = sender as TextBox;
			if (this.ActionType == KeyActionType.Tap || this.ActionType == KeyActionType.TapRepeat || this.ActionType == KeyActionType.Script)
			{
				if (e.Key == Key.Back || e.SystemKey == Key.Back)
				{
					textBox.Tag = string.Empty;
					TextBox textBox2 = textBox;
					string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
					object tag = textBox.Tag;
					BlueStacksUIBinding.Bind(textBox2, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
				}
				else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
				{
					if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10)
					{
						this.mKeyList.AddIfNotContain(e.SystemKey);
					}
					else if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
					{
						if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
						{
							this.mKeyList.AddIfNotContain(e.SystemKey);
						}
						else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
						{
							this.mKeyList.AddIfNotContain(e.SystemKey);
						}
						else
						{
							this.mKeyList.AddIfNotContain(e.Key);
						}
					}
					else
					{
						this.mKeyList.AddIfNotContain(e.Key);
					}
				}
			}
			else if (e.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
			{
				textBox.Tag = IMAPKeys.GetStringForFile(e.SystemKey);
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
			}
			else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
			{
				textBox.Tag = IMAPKeys.GetStringForFile(e.Key);
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
			}
			else if (e.Key == Key.Back)
			{
				textBox.Tag = string.Empty;
				TextBox textBox3 = textBox;
				string imapLocaleStringsConstant2 = Constants.ImapLocaleStringsConstant;
				object tag2 = textBox.Tag;
				BlueStacksUIBinding.Bind(textBox3, imapLocaleStringsConstant2 + ((tag2 != null) ? tag2.ToString() : null));
			}
			e.Handled = true;
			textBox.Focus();
			textBox.SelectAll();
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00057E94 File Offset: 0x00056094
		private void TxtBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsRemoveIfEmpty)
			{
				this.IsRemoveIfEmpty = false;
				this.UpdatePosition(Canvas.GetTop(this), Canvas.GetLeft(this));
			}
			TextBox textBox = sender as TextBox;
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				e.Handled = true;
				KMManager.CheckAndCreateNewScheme();
				textBox.Tag = "MouseMButton";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseMButton");
			}
			else if (e.RightButton == MouseButtonState.Pressed)
			{
				e.Handled = true;
				KMManager.CheckAndCreateNewScheme();
				textBox.Tag = "MouseRButton";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseRButton");
			}
			else if (e.XButton1 == MouseButtonState.Pressed)
			{
				e.Handled = true;
				KMManager.CheckAndCreateNewScheme();
				textBox.Tag = "MouseXButton1";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseXButton1");
			}
			else if (e.XButton2 == MouseButtonState.Pressed)
			{
				e.Handled = true;
				KMManager.CheckAndCreateNewScheme();
				textBox.Tag = "MouseXButton2";
				BlueStacksUIBinding.Bind(textBox, Constants.ImapLocaleStringsConstant + "MouseXButton2");
			}
			if (e.LeftButton == MouseButtonState.Pressed && textBox.IsKeyboardFocusWithin)
			{
				e.Handled = true;
			}
			textBox.Focus();
			textBox.SelectAll();
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00057FCC File Offset: 0x000561CC
		private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null && !textBox.IsKeyboardFocusWithin)
			{
				e.Handled = true;
				textBox.Focus();
			}
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00057FFC File Offset: 0x000561FC
		private void TxtBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (Math.Abs(this.TopOnClick - Canvas.GetTop(this)) < 2.0 && Math.Abs(this.LeftOnClick - Canvas.GetLeft(this)) < 2.0)
			{
				this.ShowTextBox(sender);
			}
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x0005804C File Offset: 0x0005624C
		internal void ShowTextBox(object sender)
		{
			IMAction imaction = this.ListActionItem.FirstOrDefault<IMAction>();
			Positions positions = EnumHelper.Parse<Positions>((sender as TextBlock).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
			if (imaction != null && imaction.Type != KeyActionType.MouseZoom)
			{
				this.dictTextElemets[positions].Item2.Visibility = Visibility.Visible;
			}
			this.dictTextElemets[positions].Item3.Visibility = Visibility.Collapsed;
			this.dictTextElemets[positions].Item2.Text = this.dictTextElemets[positions].Item3.Text;
			Grid.SetColumn(this.dictTextElemets[positions].Item2, Grid.GetColumn(this.dictTextElemets[positions].Item3));
			Grid.SetRow(this.dictTextElemets[positions].Item2, Grid.GetRow(this.dictTextElemets[positions].Item3));
			MiscUtils.SetFocusAsync(this.dictTextElemets[positions].Item2, 100);
			if (CanvasElement.sFocusedTextBox == null)
			{
				CanvasElement.sFocusedTextBox = this.dictTextElemets[positions].Item2;
			}
			if (imaction != null && imaction.Type == KeyActionType.MOBASkill)
			{
				this.mActionIcon.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x0000AAAB File Offset: 0x00008CAB
		private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
		{
			CanvasElement.sFocusedTextBox = sender;
			this.SetActiveImage(true);
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x00058190 File Offset: 0x00056390
		internal void TxtBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if ((sender as TextBox).Visibility != Visibility.Visible)
			{
				return;
			}
			CanvasElement.sFocusedTextBox = null;
			Positions positions = EnumHelper.Parse<Positions>((sender as TextBox).Name.ToString(CultureInfo.InvariantCulture), Positions.Center);
			this.dictTextElemets[positions].Item3.Visibility = Visibility.Visible;
			this.dictTextElemets[positions].Item2.Visibility = Visibility.Collapsed;
			this.dictTextElemets[positions].Item3.Text = this.dictTextElemets[positions].Item2.Text;
			this.SetActiveImage(false);
			if (this.IsRemoveIfEmpty && !this.mParentWindow.mIsExtraSettingsPopupOpened)
			{
				Logger.Debug("DELETE_TAP: Empty canvas element deleted");
				this.DeleteElement();
			}
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
			{
				this.mActionIcon.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x00058274 File Offset: 0x00056474
		internal void ShowOtherIcons(bool isShow = true)
		{
			IMAction imaction = this.ListActionItem.FirstOrDefault<IMAction>();
			if (isShow)
			{
				if (imaction != null && imaction.RadiusProperty != -1.0)
				{
					if (KMManager.sIsDeveloperModeOn)
					{
						this.mResizeIcon.Visibility = Visibility.Visible;
						return;
					}
					if (imaction.Type != KeyActionType.MouseZoom)
					{
						this.mResizeIcon.Visibility = Visibility.Visible;
						return;
					}
				}
			}
			else
			{
				this.mSkillImage.Visibility = Visibility.Hidden;
				this.mResizeIcon.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x000582E8 File Offset: 0x000564E8
		private void SetMOBASkillSettingsContent()
		{
			if (!((MOBASkill)this.ListActionItem.First<IMAction>()).AdvancedMode && !((MOBASkill)this.ListActionItem.First<IMAction>()).AutocastEnabled)
			{
				this.MOBASkillSettingsPopup.mManualSkill.IsChecked = new bool?(true);
			}
			else if (((MOBASkill)this.ListActionItem.First<IMAction>()).AdvancedMode && !((MOBASkill)this.ListActionItem.First<IMAction>()).AutocastEnabled)
			{
				this.MOBASkillSettingsPopup.mAutoSkill.IsChecked = new bool?(true);
			}
			else if (((MOBASkill)this.ListActionItem.First<IMAction>()).AdvancedMode && ((MOBASkill)this.ListActionItem.First<IMAction>()).AutocastEnabled)
			{
				this.MOBASkillSettingsPopup.mQuickSkill.IsChecked = new bool?(true);
			}
			this.MOBASkillSettingsPopup.mStopMovementCheckbox.IsChecked = new bool?(((MOBASkill)this.ListActionItem.First<IMAction>()).StopMOBADpad);
			this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.Inlines.Clear();
			this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
			string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=moba_stop_movement_help";
			this.MOBAOtherSettingsMoreInfoPopup.mSettingsHyperLink.NavigateUri = new Uri(text);
			this.MOBASkillSettingsMoreInfoPopup.mHyperLink.Inlines.Clear();
			this.MOBASkillSettingsMoreInfoPopup.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_READ_MORE", ""));
			string text2 = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=";
			this.MOBASkillSettingsMoreInfoPopup.mHyperLink.NavigateUri = new Uri(text2 + "moba_skill_settings_help");
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x0000AABA File Offset: 0x00008CBA
		private void SetSize(IMAction action)
		{
			if (string.IsNullOrEmpty(IMAction.sRadiusPropertyName[action.Type]))
			{
				this.mActionIcon.IsAlwaysHalfSize = true;
				base.MaxHeight = this.mActionIcon.MaxHeight;
			}
		}

		// Token: 0x06000E40 RID: 3648 RVA: 0x00058504 File Offset: 0x00056704
		internal void UpdatePosition(double top, double left)
		{
			Canvas mCanvas = this.mParentWindow.mCanvas;
			double num = (left + base.ActualWidth / 2.0) / mCanvas.ActualWidth * 100.0;
			double num2 = (top + base.ActualHeight / 2.0) / mCanvas.ActualHeight * 100.0;
			double num3 = base.ActualWidth / mCanvas.ActualWidth * 50.0;
			foreach (IMAction imaction in this.ListActionItem)
			{
				imaction.PositionX = Math.Round(num, 2);
				imaction.PositionY = Math.Round(num2, 2);
				imaction.RadiusProperty = Math.Round(num3, 2);
			}
		}

		// Token: 0x06000E41 RID: 3649 RVA: 0x000585E4 File Offset: 0x000567E4
		internal static List<CanvasElement> GetCanvasElement(IMAction action, KeymapCanvasWindow window, MainWindow mainWindow)
		{
			List<CanvasElement> list = new List<CanvasElement>();
			object[] customAttributes = action.GetType().GetCustomAttributes(typeof(DescriptionAttribute), true);
			if (customAttributes.Length != 0)
			{
				DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
				if (descriptionAttribute != null)
				{
					if (descriptionAttribute.Description.Contains("Dependent"))
					{
						if (CanvasElement.dictPoints.ContainsKey(action.PositionX.ToString() + "~" + action.PositionY.ToString()))
						{
							CanvasElement canvasElement = CanvasElement.dictPoints[action.PositionX.ToString() + "~" + action.PositionY.ToString()];
							canvasElement.AddAction(action);
							list.Add(canvasElement);
						}
						else
						{
							CanvasElement canvasElement2 = new CanvasElement(window, mainWindow);
							canvasElement2.AddAction(action);
							canvasElement2.ShowOtherIcons(true);
							CanvasElement.dictPoints.Add(action.PositionX.ToString() + "~" + action.PositionY.ToString(), canvasElement2);
							list.Add(canvasElement2);
						}
					}
					else if (descriptionAttribute.Description.Contains("ParentElement"))
					{
						CanvasElement canvasElement3 = new CanvasElement(window, mainWindow);
						canvasElement3.AddAction(action);
						canvasElement3.ShowOtherIcons(true);
						list.Add(canvasElement3);
						if (action is Pan)
						{
							Pan pan = action as Pan;
							if (pan.mLookAround != null)
							{
								list.Add(CanvasElement.GetCanvasElement(pan.mLookAround, window, mainWindow).First<CanvasElement>());
							}
							if (pan.mPanShoot != null)
							{
								list.Add(CanvasElement.GetCanvasElement(pan.mPanShoot, window, mainWindow).First<CanvasElement>());
							}
						}
						else if (action is MOBASkill)
						{
							MOBASkill mobaskill = action as MOBASkill;
							if (mobaskill.mMOBASkillCancel != null)
							{
								list.Add(CanvasElement.GetCanvasElement(mobaskill.mMOBASkillCancel, window, mainWindow).First<CanvasElement>());
							}
						}
					}
					else
					{
						CanvasElement canvasElement4 = new CanvasElement(window, mainWindow);
						canvasElement4.AddAction(action);
						canvasElement4.ShowOtherIcons(true);
						list.Add(canvasElement4);
					}
				}
			}
			return list;
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000587EC File Offset: 0x000569EC
		internal void RemoveAction(string actionItemProperty = "")
		{
			if (this.ListActionItem.Count > 1)
			{
				this.ListActionItem.RemoveAt(0);
				return;
			}
			if (base.Parent != null)
			{
				(base.Parent as Canvas).Children.Remove(this);
				IMAction parentAction = this.ListActionItem.First<IMAction>().ParentAction;
				if (parentAction.Guidance.ContainsKey(actionItemProperty))
				{
					parentAction.Guidance.Remove(actionItemProperty);
				}
			}
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x0000AAF0 File Offset: 0x00008CF0
		private void CanvasElement_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.MOBASkillSettingsPopup.IsOpen || this.MOBAOtherSettingsMoreInfoPopup.IsOpen || this.MOBASkillSettingsMoreInfoPopup.IsOpen)
			{
				return;
			}
			this.OpenPopup();
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x00058860 File Offset: 0x00056A60
		internal void OpenPopup()
		{
			List<IMAction> list = new List<IMAction>();
			if (this.ListActionItem.First<IMAction>().IsChildAction)
			{
				list = new List<IMAction> { this.ListActionItem.First<IMAction>().ParentAction };
				this.SetActiveImage(true);
			}
			else
			{
				list = this.ListActionItem;
			}
			if (list.Count != 0)
			{
				KeymapExtraSettingWindow keymapExtraSettingWindow = new KeymapExtraSettingWindow(this.ParentWindow);
				keymapExtraSettingWindow.ListAction.ClearAddRange(list);
				keymapExtraSettingWindow.mCanvasElement = this;
				keymapExtraSettingWindow.Placement = PlacementMode.Relative;
				keymapExtraSettingWindow.PlacementTarget = this;
				keymapExtraSettingWindow.StaysOpen = false;
				keymapExtraSettingWindow.Init(false);
				this.mParentWindow.mIsExtraSettingsPopupOpened = true;
				this.SetActiveImage(false);
				keymapExtraSettingWindow.IsTopmost = true;
				keymapExtraSettingWindow.IsOpen = true;
				Point position = Mouse.GetPosition(this);
				keymapExtraSettingWindow.HorizontalOffset = position.X;
				keymapExtraSettingWindow.VerticalOffset = position.Y;
				keymapExtraSettingWindow.Closed += this.ExtraSettingPopup_Closed;
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x0005894C File Offset: 0x00056B4C
		private void SetActiveImage(bool isActive = true)
		{
			string text = this.mType.ToString();
			if (this.mType == KeyActionType.MOBASkill && this.mParentWindow.IsInOverlayMode)
			{
				text = KeyActionType.Tap.ToString();
			}
			if (this.mType == KeyActionType.Dpad)
			{
				if (this.ListActionItem.Count != 0 && (this.ListActionItem.First<IMAction>() as Dpad).IsMOBADpadEnabled && !this.mParentWindow.IsInOverlayMode)
				{
					text = "moba_" + text;
					this.mGrid.Visibility = Visibility.Collapsed;
				}
				else
				{
					this.mGrid.Visibility = Visibility.Visible;
				}
			}
			if (this.mType == KeyActionType.FreeLook)
			{
				if ((this.ListActionItem.First<IMAction>() as FreeLook).DeviceType == 0)
				{
					text += "_keyboard";
				}
				else
				{
					text += "_mouse";
				}
			}
			if (isActive)
			{
				this.mActionIcon.ImageName = text + "_canvas_active";
				this.mActionIcon2.ImageName = this.mActionIcon.ImageName + "_2";
			}
			else
			{
				this.mActionIcon.ImageName = text + "_canvas";
				this.mActionIcon2.ImageName = this.mActionIcon.ImageName + "_2";
			}
			this.mActionIcon.Visibility = Visibility.Visible;
			this.mActionIcon2.Visibility = Visibility.Visible;
			if ((this.mType == KeyActionType.State || this.mType == KeyActionType.MouseZoom || this.mType == KeyActionType.Callback) && !KMManager.sIsDeveloperModeOn)
			{
				this.mCloseIcon.Visibility = Visibility.Collapsed;
				this.mResizeIcon.Visibility = Visibility.Collapsed;
				this.mActionIcon.Visibility = Visibility.Collapsed;
				this.mActionIcon2.Visibility = Visibility.Collapsed;
			}
			if (this.mType == KeyActionType.Zoom || this.mType == KeyActionType.MouseZoom)
			{
				this.mCloseIcon.Margin = new Thickness(-20.0, 20.0, 20.0, -20.0);
				this.mActionIcon2.Margin = new Thickness(-55.0, 0.0, 0.0, 0.0);
				this.mResizeIcon.Margin = new Thickness(-20.0, -20.0, 20.0, 20.0);
			}
			if (this.mType == KeyActionType.MOBASkill)
			{
				this.mSkillImage.Visibility = Visibility.Visible;
				if (base.ActualWidth < 70.0)
				{
					this.mSkillImage.Margin = new Thickness(-50.0, 30.0, 10.0, 0.0);
				}
				if (CanvasElement.sFocusedTextBox == null)
				{
					this.mActionIcon.Visibility = Visibility.Collapsed;
				}
			}
			if (this.ListActionItem.Count != 0 && this.ListActionItem.First<IMAction>().IsChildAction && this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>().ParentAction))
			{
				this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>().ParentAction].SetActiveImage(isActive);
			}
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00058C90 File Offset: 0x00056E90
		internal void ExtraSettingPopup_Closed(object sender, EventArgs e)
		{
			if (!this.mParentWindow.mIsClosing)
			{
				KeymapExtraSettingWindow keymapExtraSettingWindow = sender as KeymapExtraSettingWindow;
				string text = keymapExtraSettingWindow.mGuidanceCategoryComboBox.mAutoComboBox.Text;
				foreach (IMAction imaction in keymapExtraSettingWindow.ListAction)
				{
					if (!string.Equals(imaction.GuidanceCategory, text, StringComparison.InvariantCulture))
					{
						imaction.GuidanceCategory = text;
						KeymapCanvasWindow.sIsDirty = true;
						this.ParentWindow.SelectedConfig.AddString(imaction.GuidanceCategory);
					}
				}
				this.SetKeysForActions(this.ListActionItem);
				this.SetActiveImage(false);
				if (this.ListActionItem.First<IMAction>().Type == KeyActionType.Zoom || this.ListActionItem.First<IMAction>().Type == KeyActionType.MouseZoom)
				{
					this.ListActionItem.First<IMAction>().RadiusProperty = this.ListActionItem.First<IMAction>().RadiusProperty;
				}
				if (this.ParentWindow.SelectedConfig.ControlSchemes == null)
				{
					this.ParentWindow.SelectedConfig.ControlSchemes = new List<IMControlScheme>();
				}
				KMManager.sGamepadDualTextbox = null;
				KMManager.CallGamepadHandler(this.ParentWindow, "false");
				this.SetElementLayout(false, 0.0, 0.0);
				this.mParentWindow.mIsExtraSettingsPopupOpened = false;
			}
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00058DF0 File Offset: 0x00056FF0
		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.ListActionItem.Count != 0 && this.mParentWindow != null)
			{
				if (!this.mParentWindow.IsInOverlayMode)
				{
					this.SetElementLayout(true, 0.0, 0.0);
				}
				else
				{
					this.SetElementLayout(true, this.mXPosition, this.mYPosition);
					if (this.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
					{
						KMManager.CanvasWindow.SetOnboardingControlPosition(this.mXPosition, this.mYPosition);
					}
				}
			}
			this.mIsLoadingfromFile = false;
			this.mMousePointForTap = null;
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00058E8C File Offset: 0x0005708C
		internal void SetElementLayout(bool isLoaded = false, double xPos = 0.0, double yPos = 0.0)
		{
			IMAction imaction = this.ListActionItem.First<IMAction>();
			if (xPos <= 0.0)
			{
				xPos = imaction.PositionX;
			}
			if (yPos <= 0.0)
			{
				yPos = imaction.PositionY;
			}
			double num;
			if (imaction.RadiusProperty == -1.0 || (this.mParentWindow.IsInOverlayMode && imaction.Type == KeyActionType.MOBASkill))
			{
				num = base.ActualWidth;
			}
			else
			{
				num = imaction.RadiusProperty * 2.0 / 100.0 * this.mParentWindow.mCanvas.ActualWidth;
				base.Width = num;
			}
			base.Height = num;
			if (imaction.PositionX == -1.0)
			{
				Point point;
				if (this.mMousePointForTap != null)
				{
					point = this.mMousePointForTap.Value;
					if (imaction.Type == KeyActionType.Tap)
					{
						this.UpdatePosition(point.Y, point.X);
					}
				}
				else
				{
					point = Mouse.GetPosition(base.Parent as IInputElement);
				}
				if (!isLoaded && (imaction.Type == KeyActionType.Tilt || imaction.Type == KeyActionType.State || imaction.Type == KeyActionType.MouseZoom))
				{
					return;
				}
				if (!isLoaded || !this.mIsLoadingfromFile)
				{
					Canvas.SetTop(this, point.Y - num / 2.0);
					Canvas.SetLeft(this, point.X - num / 2.0);
					return;
				}
				if (imaction.Type == KeyActionType.Tilt || ((imaction.Type == KeyActionType.State || imaction.Type == KeyActionType.MouseZoom) && KMManager.sIsDeveloperModeOn))
				{
					Canvas.SetTop(this, 0.0);
					Canvas.SetLeft(this, 0.0);
					return;
				}
			}
			else
			{
				double num2 = xPos / 100.0 * this.mParentWindow.mCanvas.ActualWidth;
				double num3 = yPos / 100.0 * this.mParentWindow.mCanvas.ActualHeight;
				num2 = ((num2 < 0.0) ? 0.0 : num2);
				num3 = ((num3 < 0.0) ? 0.0 : num3);
				double num5;
				double num6;
				if (this.mParentWindow.IsInOverlayMode)
				{
					double num4;
					if (imaction.Type == KeyActionType.Dpad)
					{
						num4 = base.ActualWidth - base.ActualWidth * 0.4;
					}
					else if (imaction.Type == KeyActionType.MOBADpad)
					{
						num4 = base.ActualWidth - base.ActualWidth * 0.5;
					}
					else
					{
						num4 = 30.0;
					}
					if (num2 > this.mParentWindow.mCanvas.ActualWidth - num4)
					{
						num2 = this.mParentWindow.mCanvas.ActualWidth - base.ActualWidth;
						num5 = num2;
					}
					else
					{
						num5 = num2 - num / 2.0;
					}
					if (num3 > this.mParentWindow.mCanvas.ActualHeight - num4)
					{
						num3 = this.mParentWindow.mCanvas.ActualHeight - base.ActualHeight;
						num6 = num3;
					}
					else
					{
						num6 = num3 - num / 2.0;
					}
					Canvas.SetLeft(this, num5);
					Canvas.SetTop(this, num6);
					return;
				}
				num2 = ((num2 > this.mParentWindow.mCanvas.ActualWidth) ? this.mParentWindow.mCanvas.ActualWidth : num2);
				num3 = ((num3 > this.mParentWindow.mCanvas.ActualHeight) ? this.mParentWindow.mCanvas.ActualHeight : num3);
				num5 = num2 - num / 2.0;
				num6 = num3 - num / 2.0;
				Canvas.SetLeft(this, num5);
				Canvas.SetTop(this, num6);
			}
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x0000AB20 File Offset: 0x00008D20
		private void MoveIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!this.mResizeIcon.IsMouseOver)
			{
				base.Cursor = Cursors.Hand;
			}
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x0000AB3A File Offset: 0x00008D3A
		private void MoveIcon_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mParentWindow.mCanvasElement == null)
			{
				base.Cursor = Cursors.Arrow;
			}
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
			{
				this.mActionIcon.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x0000AB73 File Offset: 0x00008D73
		private void ResizeIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.SizeNWSE;
			e.Handled = true;
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x0000AB87 File Offset: 0x00008D87
		private void ResizeIcon_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mParentWindow.mCanvasElement == null)
			{
				base.Cursor = Cursors.Arrow;
				e.Handled = true;
				if (base.IsMouseOver)
				{
					base.Cursor = Cursors.Hand;
				}
			}
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x0000ABBB File Offset: 0x00008DBB
		private void DeleteIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			KeymapCanvasWindow.sIsDirty = true;
			KMManager.CheckAndCreateNewScheme();
			this.DeleteElement();
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0005924C File Offset: 0x0005744C
		private void DeleteElement()
		{
			KeyActionType type = this.ListActionItem.First<IMAction>().Type;
			if (type == KeyActionType.MOBADpad)
			{
				if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
				{
					this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("");
					this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
				}
				this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(this.ListActionItem.First<IMAction>());
				Dpad dpad = (this.ListActionItem.First<IMAction>() as MOBADpad).ParentAction as Dpad;
				dpad.mMOBADpad.OriginX = (dpad.mMOBADpad.OriginY = -1.0);
				this.mParentWindow.dictCanvasElement[dpad].SetActiveImage(false);
				return;
			}
			switch (type)
			{
			case KeyActionType.LookAround:
			{
				if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
				{
					this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyLookAround");
					this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
				}
				Pan pan = (this.ListActionItem.First<IMAction>() as LookAround).ParentAction as Pan;
				pan.LookAroundX = (pan.LookAroundY = -1.0);
				return;
			}
			case KeyActionType.PanShoot:
			{
				if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
				{
					this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyAction");
					this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
				}
				Pan pan2 = (this.ListActionItem.First<IMAction>() as PanShoot).ParentAction as Pan;
				pan2.LButtonX = (pan2.LButtonY = -1.0);
				return;
			}
			case KeyActionType.MOBASkillCancel:
			{
				if (this.mParentWindow.dictCanvasElement.ContainsKey(this.ListActionItem.First<IMAction>()))
				{
					this.mParentWindow.dictCanvasElement[this.ListActionItem.First<IMAction>()].RemoveAction("KeyCancel");
					this.mParentWindow.dictCanvasElement.Remove(this.ListActionItem.First<IMAction>());
				}
				MOBASkill mobaskill = (this.ListActionItem.First<IMAction>() as MOBASkillCancel).ParentAction as MOBASkill;
				mobaskill.CancelX = (mobaskill.CancelY = -1.0);
				return;
			}
			default:
				foreach (IMAction imaction in this.ListActionItem)
				{
					this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(imaction);
				}
				if (base.Parent != null)
				{
					(base.Parent as Canvas).Children.Remove(this);
					foreach (KeyValuePair<IMAction, CanvasElement> keyValuePair in this.mParentWindow.dictCanvasElement)
					{
						if (keyValuePair.Key.ParentAction == this.ListActionItem.First<IMAction>())
						{
							keyValuePair.Value.RemoveAction("");
							this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Remove(keyValuePair.Value.ListActionItem.First<IMAction>());
						}
					}
				}
				return;
			}
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x0005962C File Offset: 0x0005782C
		private void UpArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			int num = Convert.ToInt32(this.mCountText.Text, CultureInfo.InvariantCulture);
			num++;
			this.mCountText.Text = num.ToString(CultureInfo.InvariantCulture);
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
			{
				((TapRepeat)this.ListActionItem.First<IMAction>()).Count = num;
				((TapRepeat)this.ListActionItem.First<IMAction>()).Delay = 1000 / (2 * num);
			}
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x000596B4 File Offset: 0x000578B4
		private void DownArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			int num = Convert.ToInt32(this.mCountText.Text, CultureInfo.InvariantCulture);
			num--;
			if (num <= 1)
			{
				this.mCountText.Text = "1";
			}
			else
			{
				this.mCountText.Text = num.ToString(CultureInfo.InvariantCulture);
			}
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
			{
				((TapRepeat)this.ListActionItem.First<IMAction>()).Count = Convert.ToInt32(this.mCountText.Text, CultureInfo.InvariantCulture);
				((TapRepeat)this.ListActionItem.First<IMAction>()).Delay = 1000 / (2 * ((TapRepeat)this.ListActionItem.First<IMAction>()).Count);
			}
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00059778 File Offset: 0x00057978
		private void mToggleImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (CanvasElement.sFocusedTextBox != null)
				{
					WpfUtils.FindVisualParent<CanvasElement>(CanvasElement.sFocusedTextBox as DependencyObject).TxtBox_LostFocus(CanvasElement.sFocusedTextBox, new RoutedEventArgs());
				}
				if (string.Equals(this.mToggleImage.ImageName, "right_switch", StringComparison.InvariantCulture))
				{
					this.mToggleImage.ImageName = "left_switch";
					if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
					{
						((TapRepeat)this.ListActionItem.First<IMAction>()).RepeatUntilKeyUp = false;
					}
					else if (this.ListActionItem.First<IMAction>().Type == KeyActionType.FreeLook)
					{
						((FreeLook)this.ListActionItem.First<IMAction>()).DeviceType = 0;
						this.SetKeysForActions(this.ListActionItem);
					}
				}
				else
				{
					this.mToggleImage.ImageName = "right_switch";
					if (this.ListActionItem.First<IMAction>().Type == KeyActionType.TapRepeat)
					{
						((TapRepeat)this.ListActionItem.First<IMAction>()).RepeatUntilKeyUp = true;
					}
					else if (this.ListActionItem.First<IMAction>().Type == KeyActionType.FreeLook)
					{
						((FreeLook)this.ListActionItem.First<IMAction>()).DeviceType = 1;
						this.SetKeysForActions(this.ListActionItem);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in toggleMode: " + ex.ToString());
			}
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x000598E4 File Offset: 0x00057AE4
		private void MSkillImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.SetMOBASkillSettingsContent();
			this.SkillIconToolTipPopup.IsOpen = false;
			this.MOBASkillSettingsPopup.IsOpen = true;
			this.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
			this.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
			ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, "moba_skill_settings_clicked", null, null, null, null, null);
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0000ABD5 File Offset: 0x00008DD5
		private void MSkillImage_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill && this.mSkillImage.IsEnabled)
			{
				this.SkillIconToolTipPopup.IsOpen = true;
				this.SkillIconToolTipPopup.StaysOpen = true;
			}
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x0000AC0F File Offset: 0x00008E0F
		private void MSkillImage_MouseLeave(object sender, MouseEventArgs e)
		{
			this.SkillIconToolTipPopup.IsOpen = false;
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x0000AC1D File Offset: 0x00008E1D
		private void ScriptSettingsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mParentWindow.SidebarWindow.mLastScriptActionItem = this.ListActionItem;
			this.mParentWindow.SidebarWindow.ToggleAGCWindowVisiblity(true);
			ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_edit");
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x0005994C File Offset: 0x00057B4C
		internal void SendMOBAStats(string action, string skillName = "")
		{
			try
			{
				string item = this.dictTextElemets[Positions.Center].Item1;
				string text = "";
				if (this.ListActionItem.First<IMAction>().Guidance.ContainsKey(item))
				{
					text = this.ListActionItem.First<IMAction>().Guidance[item];
				}
				ClientStats.SendMiscellaneousStatsAsync("MOBA", RegistryManager.Instance.UserGuid, KMManager.sPackageName, action, text, skillName, null, null, null);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending MOBA stats: " + ex.ToString());
			}
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000599EC File Offset: 0x00057BEC
		internal void AssignMobaSkill(bool advancedMode, bool autoCastEnabled)
		{
			KMManager.CheckAndCreateNewScheme();
			if (this.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
			{
				((MOBASkill)this.ListActionItem.First<IMAction>()).AdvancedMode = advancedMode;
				((MOBASkill)this.ListActionItem.First<IMAction>()).AutocastEnabled = autoCastEnabled;
			}
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00059A40 File Offset: 0x00057C40
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/canvaselement.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00059A70 File Offset: 0x00057C70
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
				this.mCanvasElement = (CanvasElement)target;
				this.mCanvasElement.MouseEnter += this.MoveIcon_MouseEnter;
				this.mCanvasElement.MouseLeave += this.MoveIcon_MouseLeave;
				this.mCanvasElement.PreviewMouseRightButtonUp += this.CanvasElement_PreviewMouseRightButtonUp;
				return;
			case 2:
				((Grid)target).Loaded += this.Grid_Loaded;
				return;
			case 3:
				this.mToggleModeGrid = (Grid)target;
				return;
			case 4:
				this.mToggleMode1 = (TextBlock)target;
				return;
			case 5:
				this.mToggleImage = (CustomPictureBox)target;
				this.mToggleImage.PreviewMouseLeftButtonUp += this.mToggleImage_PreviewMouseLeftButtonUp;
				return;
			case 6:
				this.mToggleMode2 = (TextBlock)target;
				return;
			case 7:
				this.mCanvasGrid = (Grid)target;
				return;
			case 8:
				this.mKeyRepeatGrid = (Grid)target;
				return;
			case 9:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.UpArrow_PreviewMouseDown;
				return;
			case 10:
				this.mCountText = (TextBlock)target;
				return;
			case 11:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.DownArrow_PreviewMouseDown;
				return;
			case 12:
				this.mActionIcon = (CustomPictureBox)target;
				return;
			case 13:
				this.mActionIcon2 = (CustomPictureBox)target;
				return;
			case 14:
				this.mCloseIcon = (CustomPictureBox)target;
				this.mCloseIcon.PreviewMouseDown += this.DeleteIcon_PreviewMouseDown;
				return;
			case 15:
				this.mResizeIcon = (CustomPictureBox)target;
				this.mResizeIcon.MouseEnter += this.ResizeIcon_MouseEnter;
				this.mResizeIcon.MouseLeave += this.ResizeIcon_MouseLeave;
				return;
			case 16:
				this.mSkillImage = (CustomPictureBox)target;
				this.mSkillImage.MouseLeftButtonUp += this.MSkillImage_MouseLeftButtonUp;
				this.mSkillImage.MouseEnter += this.MSkillImage_MouseEnter;
				this.mSkillImage.MouseLeave += this.MSkillImage_MouseLeave;
				return;
			case 17:
				this.mGrid = (Grid)target;
				return;
			case 18:
				this.mColumn0 = (ColumnDefinition)target;
				return;
			case 19:
				this.mColumn1 = (ColumnDefinition)target;
				return;
			case 20:
				this.mColumn2 = (ColumnDefinition)target;
				return;
			case 21:
				this.mColumn3 = (ColumnDefinition)target;
				return;
			case 22:
				this.mColumn4 = (ColumnDefinition)target;
				return;
			case 23:
				this.mRow0 = (RowDefinition)target;
				return;
			case 24:
				this.mRow1 = (RowDefinition)target;
				return;
			case 25:
				this.mRow2 = (RowDefinition)target;
				return;
			case 26:
				this.mRow3 = (RowDefinition)target;
				return;
			case 27:
				this.mRow4 = (RowDefinition)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040008F5 RID: 2293
		internal bool mIsLoadingfromFile;

		// Token: 0x040008F6 RID: 2294
		internal Point Center;

		// Token: 0x040008F7 RID: 2295
		private Point? mMousePointForTap;

		// Token: 0x040008F8 RID: 2296
		private KeymapCanvasWindow mParentWindow;

		// Token: 0x040008F9 RID: 2297
		private MainWindow ParentWindow;

		// Token: 0x040008FA RID: 2298
		private List<Key> mKeyList = new List<Key>();

		// Token: 0x040008FB RID: 2299
		internal Dictionary<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>> dictTextElemets = new Dictionary<Positions, BlueStacks.Common.Tuple<string, TextBox, TextBlock, List<IMAction>>>();

		// Token: 0x040008FC RID: 2300
		internal static Dictionary<string, CanvasElement> dictPoints = new Dictionary<string, CanvasElement>();

		// Token: 0x040008FD RID: 2301
		private KeyActionType mType;

		// Token: 0x040008FE RID: 2302
		internal double TopOnClick;

		// Token: 0x040008FF RID: 2303
		internal double LeftOnClick;

		// Token: 0x04000900 RID: 2304
		internal static object sFocusedTextBox;

		// Token: 0x04000901 RID: 2305
		internal double mXPosition;

		// Token: 0x04000902 RID: 2306
		internal double mYPosition;

		// Token: 0x04000904 RID: 2308
		private MOBASkillSettingsPopup mMOBASkillSettingsPopup;

		// Token: 0x04000905 RID: 2309
		private MOBAOtherSettingsMoreInfoPopup mMOBAOtherSettingsMoreInfoPopup;

		// Token: 0x04000906 RID: 2310
		private SkillIconToolTipPopup mSkillIconToolTipPopup;

		// Token: 0x04000907 RID: 2311
		private MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;

		// Token: 0x04000909 RID: 2313
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CanvasElement mCanvasElement;

		// Token: 0x0400090A RID: 2314
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mToggleModeGrid;

		// Token: 0x0400090B RID: 2315
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mToggleMode1;

		// Token: 0x0400090C RID: 2316
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mToggleImage;

		// Token: 0x0400090D RID: 2317
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mToggleMode2;

		// Token: 0x0400090E RID: 2318
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCanvasGrid;

		// Token: 0x0400090F RID: 2319
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mKeyRepeatGrid;

		// Token: 0x04000910 RID: 2320
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mCountText;

		// Token: 0x04000911 RID: 2321
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mActionIcon;

		// Token: 0x04000912 RID: 2322
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mActionIcon2;

		// Token: 0x04000913 RID: 2323
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseIcon;

		// Token: 0x04000914 RID: 2324
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mResizeIcon;

		// Token: 0x04000915 RID: 2325
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSkillImage;

		// Token: 0x04000916 RID: 2326
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x04000917 RID: 2327
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mColumn0;

		// Token: 0x04000918 RID: 2328
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mColumn1;

		// Token: 0x04000919 RID: 2329
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mColumn2;

		// Token: 0x0400091A RID: 2330
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mColumn3;

		// Token: 0x0400091B RID: 2331
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mColumn4;

		// Token: 0x0400091C RID: 2332
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal RowDefinition mRow0;

		// Token: 0x0400091D RID: 2333
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal RowDefinition mRow1;

		// Token: 0x0400091E RID: 2334
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal RowDefinition mRow2;

		// Token: 0x0400091F RID: 2335
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal RowDefinition mRow3;

		// Token: 0x04000920 RID: 2336
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal RowDefinition mRow4;

		// Token: 0x04000921 RID: 2337
		private bool _contentLoaded;
	}
}
