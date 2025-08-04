using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200013C RID: 316
	public class AdvancedGameControlWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x06000CAD RID: 3245 RVA: 0x0004622C File Offset: 0x0004442C
		internal AdvancedGameControlWindow(MainWindow window)
		{
			this.ParentWindow = window;
			this.InitializeComponent();
			if (KMManager.sIsDeveloperModeOn)
			{
				this.mStatePrimitive.Visibility = Visibility.Visible;
				this.mMouseZoomPrimitive.Visibility = Visibility.Visible;
				this.mScrollPrimitive.Visibility = Visibility.Visible;
			}
			else
			{
				this.mStatePrimitive.Visibility = Visibility.Collapsed;
				this.mMouseZoomPrimitive.Visibility = Visibility.Collapsed;
				this.mScrollPrimitive.Visibility = Visibility.Collapsed;
			}
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mBrowserHelp.Visibility = Visibility.Collapsed;
			}
			base.Width = 0.0;
			base.Height = 0.0;
			BlueStacksUIBinding.Bind(this.mShowHelpHyperlink, "STRING_SCRIPT_GUIDE", "");
			AdvancedSettingsItemPanel advancedSettingsItemPanel = this.mTapPrimitive;
			advancedSettingsItemPanel.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel2 = this.mTapRepeatPrimitive;
			advancedSettingsItemPanel2.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel2.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel3 = this.mDpadPrimitive;
			advancedSettingsItemPanel3.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel3.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel4 = this.mZoomPrimitive;
			advancedSettingsItemPanel4.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel4.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel5 = this.mFreeLookPrimitive;
			advancedSettingsItemPanel5.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel5.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel6 = this.mPanPrimitive;
			advancedSettingsItemPanel6.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel6.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel7 = this.mMOBASkillPrimitive;
			advancedSettingsItemPanel7.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel7.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel8 = this.mSwipePrimitive;
			advancedSettingsItemPanel8.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel8.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel9 = this.mTiltPrimitive;
			advancedSettingsItemPanel9.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel9.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel10 = this.mStatePrimitive;
			advancedSettingsItemPanel10.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel10.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel11 = this.mScriptPrimitive;
			advancedSettingsItemPanel11.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel11.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel12 = this.mMouseZoomPrimitive;
			advancedSettingsItemPanel12.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel12.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel13 = this.mRotatePrimitive;
			advancedSettingsItemPanel13.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel13.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel14 = this.mScrollPrimitive;
			advancedSettingsItemPanel14.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel14.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel15 = this.mEdgeScrollPrimitive;
			advancedSettingsItemPanel15.MouseDragStart = (EventHandler)Delegate.Combine(advancedSettingsItemPanel15.MouseDragStart, new EventHandler(this.AdvancedSettingsItemPanel_MouseDragStart));
			AdvancedSettingsItemPanel advancedSettingsItemPanel16 = this.mTapPrimitive;
			advancedSettingsItemPanel16.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel16.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel17 = this.mTapRepeatPrimitive;
			advancedSettingsItemPanel17.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel17.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel18 = this.mDpadPrimitive;
			advancedSettingsItemPanel18.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel18.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel19 = this.mZoomPrimitive;
			advancedSettingsItemPanel19.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel19.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel20 = this.mFreeLookPrimitive;
			advancedSettingsItemPanel20.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel20.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel21 = this.mPanPrimitive;
			advancedSettingsItemPanel21.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel21.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel22 = this.mMOBASkillPrimitive;
			advancedSettingsItemPanel22.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel22.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel23 = this.mSwipePrimitive;
			advancedSettingsItemPanel23.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel23.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel24 = this.mTiltPrimitive;
			advancedSettingsItemPanel24.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel24.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel25 = this.mStatePrimitive;
			advancedSettingsItemPanel25.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel25.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel26 = this.mScriptPrimitive;
			advancedSettingsItemPanel26.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel26.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel27 = this.mMouseZoomPrimitive;
			advancedSettingsItemPanel27.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel27.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel28 = this.mRotatePrimitive;
			advancedSettingsItemPanel28.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel28.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel29 = this.mScrollPrimitive;
			advancedSettingsItemPanel29.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel29.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
			AdvancedSettingsItemPanel advancedSettingsItemPanel30 = this.mEdgeScrollPrimitive;
			advancedSettingsItemPanel30.Tap = (EventHandler)Delegate.Combine(advancedSettingsItemPanel30.Tap, new EventHandler(this.AdvancedSettingsItemPanel_Tap));
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x00009F35 File Offset: 0x00008135
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x00009F3D File Offset: 0x0000813D
		private void CloseWindow()
		{
			KMManager.sIsDeveloperModeOn = false;
			base.Close();
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x00046788 File Offset: 0x00044988
		internal void ToggleAGCWindowVisiblity(bool isScriptModeWindow)
		{
			KMManager.sIsInScriptEditingMode = isScriptModeWindow;
			this.mScriptModeDictionary["isInScriptMode"] = isScriptModeWindow.ToString(CultureInfo.InvariantCulture);
			this.BindOrUnbindMouseEvents(isScriptModeWindow);
			if (isScriptModeWindow)
			{
				this.PrimaryGrid.Visibility = Visibility.Collapsed;
				this.KeySequenceScriptGrid.Visibility = Visibility.Visible;
				base.Owner = this.ParentWindow;
				this.PopulateScriptTextBox();
				this.CanvasWindow.Hide();
				this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(false);
			}
			else
			{
				this.PrimaryGrid.Visibility = Visibility.Visible;
				this.KeySequenceScriptGrid.Visibility = Visibility.Collapsed;
				this.CanvasWindow.Show();
				base.Owner = this.CanvasWindow;
				base.Activate();
				this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(true);
			}
			HTTPUtils.SendRequestToEngineAsync("scriptEditingModeEntered", this.mScriptModeDictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0);
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x00046870 File Offset: 0x00044A70
		private void BindOrUnbindMouseEvents(bool bind)
		{
			base.MouseEnter -= this.AdvancedGameControlWindow_MouseEnter;
			base.MouseLeave -= this.AdvancedGameControlWindow_MouseLeave;
			if (bind)
			{
				base.MouseEnter += this.AdvancedGameControlWindow_MouseEnter;
				base.MouseLeave += this.AdvancedGameControlWindow_MouseLeave;
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x00009F4B File Offset: 0x0000814B
		private void AdvancedGameControlWindow_MouseLeave(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (this.ParentWindow.IsActive)
			{
				this.ParentWindow.mFrontendHandler.ShowGLWindow();
			}
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x00004786 File Offset: 0x00002986
		private void AdvancedGameControlWindow_MouseEnter(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000468C8 File Offset: 0x00044AC8
		private void AdvancedGameControlWindow_Closing(object sender, CancelEventArgs evt)
		{
			if (KeymapCanvasWindow.sIsDirty)
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), delegate(object o, EventArgs e)
				{
					KMManager.SaveIMActions(this.ParentWindow, false, false);
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), delegate(object o, EventArgs e)
				{
					KMManager.LoadIMActions(this.ParentWindow, KMManager.sPackageName);
					KeymapCanvasWindow.sIsDirty = false;
				}, null, false, null);
				customMessageWindow.CloseButtonHandle(delegate(object o, EventArgs e)
				{
					this.CanvasWindow.mIsClosing = false;
					evt.Cancel = true;
				}, null);
				customMessageWindow.Owner = this.CanvasWindow;
				customMessageWindow.ShowDialog();
			}
			this.CanvasWindow.SidebarWindowLeft = base.Left;
			this.CanvasWindow.SidebarWindowTop = base.Top;
			this.ParentWindow.Activate();
			this.ParentWindow.Utils.ToggleTopBarSidebarEnabled(true);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000469DC File Offset: 0x00044BDC
		private void AdvancedGameControlWindow_Closed(object sender, EventArgs e)
		{
			this.CanvasWindow.SidebarWindow = null;
			if (KeymapCanvasWindow.sWasMaximized)
			{
				this.ParentWindow.MaximizeWindow();
			}
			else
			{
				this.ParentWindow.ChangeHeightWidthTopLeft(this.CanvasWindow.mParentWindowWidth, this.CanvasWindow.mParentWindowHeight, this.CanvasWindow.mParentWindowTop, this.CanvasWindow.mParentWindowLeft);
			}
			KeymapCanvasWindow.sWasMaximized = false;
			if (this.CanvasWindow.IsLoaded && !this.CanvasWindow.mIsClosing)
			{
				this.CanvasWindow.Close();
			}
			if (RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
			}
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x00046A84 File Offset: 0x00044C84
		internal void Init(KeymapCanvasWindow window)
		{
			this.CanvasWindow = window;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mNCTransSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
				this.mLastSavedSliderValue = this.mNCTransSlider.Value;
				this.mNCTransparencyLevel.Text = ((int)(this.mNCTransSlider.Value * 100.0)).ToString(CultureInfo.InvariantCulture);
				if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
				{
					this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
				}
				this.ParentWindow.mCommonHandler.OverlayStateChangedEvent += this.ParentWindow_OverlayStateChangedEvent;
			}
			else
			{
				this.mNCTransparencySlider.Visibility = Visibility.Collapsed;
			}
			this.FillProfileCombo();
			this.ProfileChanged();
			this.mSaveBtn.IsEnabled = false;
			this.mUndoBtn.IsEnabled = false;
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00046B70 File Offset: 0x00044D70
		internal void InsertXYInScript(double x, double y)
		{
			string text = " " + x.ToString("00.00", CultureInfo.InvariantCulture) + " " + y.ToString("00.00", CultureInfo.InvariantCulture);
			int num = this.mScriptText.SelectionStart + text.Length;
			this.mScriptText.Text = this.mScriptText.Text.Insert(this.mScriptText.SelectionStart, text);
			this.mScriptText.SelectionStart = num;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x00046BF8 File Offset: 0x00044DF8
		internal void OrderingControlSchemes()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.ParentWindow.SelectedConfig.ControlSchemes.Sort(new Comparison<IMControlScheme>(this.CompareSchemesAlphabetically));
			foreach (IMControlScheme imcontrolScheme in new List<IMControlScheme>(this.ParentWindow.SelectedConfig.ControlSchemes))
			{
				if (imcontrolScheme.BuiltIn)
				{
					if (imcontrolScheme.IsBookMarked)
					{
						this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
						this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num3, imcontrolScheme);
						num3++;
						num2++;
						num++;
					}
					else
					{
						this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
						this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num2, imcontrolScheme);
						num2++;
						num++;
					}
				}
				else if (imcontrolScheme.IsBookMarked)
				{
					this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
					this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num, imcontrolScheme);
					num++;
				}
			}
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00046D4C File Offset: 0x00044F4C
		private int CompareSchemesAlphabetically(IMControlScheme x, IMControlScheme y)
		{
			string text = x.Name.ToLower(CultureInfo.InvariantCulture).Trim();
			string text2 = y.Name.ToLower(CultureInfo.InvariantCulture).Trim();
			if (text.Contains(text2))
			{
				return 1;
			}
			if (text2.Contains(text))
			{
				return -1;
			}
			if (string.CompareOrdinal(text, text2) < 0)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x00046DA8 File Offset: 0x00044FA8
		public void FillProfileCombo()
		{
			this.OrderingControlSchemes();
			ComboBoxSchemeControl comboBoxSchemeControl = null;
			this.mSchemeComboBox.Items.Children.Clear();
			if (this.ParentWindow.SelectedConfig.ControlSchemes != null && this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
			{
				this.mProfileHeader.Visibility = Visibility.Visible;
				using (Dictionary<string, IMControlScheme>.ValueCollection.Enumerator enumerator = this.ParentWindow.SelectedConfig.ControlSchemesDict.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IMControlScheme item = enumerator.Current;
						ComboBoxSchemeControl comboBoxSchemeControl2 = new ComboBoxSchemeControl(this.CanvasWindow, this.ParentWindow);
						comboBoxSchemeControl2.mSchemeName.Text = item.Name;
						comboBoxSchemeControl2.IsEnabled = true;
						if (item.Selected)
						{
							comboBoxSchemeControl = comboBoxSchemeControl2;
							BlueStacksUIBinding.BindColor(comboBoxSchemeControl2, global::System.Windows.Controls.Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
						}
						if (item.BuiltIn || this.ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, item.Name, StringComparison.InvariantCulture)) == 2)
						{
							comboBoxSchemeControl2.mEditImg.Visibility = Visibility.Hidden;
							comboBoxSchemeControl2.mDeleteImg.Visibility = Visibility.Hidden;
						}
						if (item.IsBookMarked)
						{
							comboBoxSchemeControl2.mBookmarkImg.ImageName = "bookmarked";
						}
						this.mSchemeComboBox.Items.Children.Add(comboBoxSchemeControl2);
					}
				}
				if (comboBoxSchemeControl == null)
				{
					comboBoxSchemeControl = this.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl;
					this.ParentWindow.SelectedConfig.ControlSchemesDict[comboBoxSchemeControl.mSchemeName.Text].Selected = true;
				}
				else
				{
					this.mSchemeComboBox.SelectedItem = comboBoxSchemeControl.mSchemeName.Text.ToString(CultureInfo.InvariantCulture);
					this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeComboBox.SelectedItem];
					this.mSchemeComboBox.mName.Text = this.mSchemeComboBox.SelectedItem;
				}
			}
			else
			{
				BlueStacksUIBinding.Bind(this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
			}
			if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes != null && this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
			{
				this.mExport.IsEnabled = true;
			}
			else
			{
				this.mExport.IsEnabled = false;
			}
			this.mRevertBtn.IsEnabled = this.ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) == 2;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x00047088 File Offset: 0x00045288
		private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				base.DragMove();
			}
			catch
			{
			}
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x00009F35 File Offset: 0x00008135
		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x00009F6A File Offset: 0x0000816A
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			KMManager.SaveIMActions(this.ParentWindow, false, false);
			this.mLastSavedSliderValue = this.mNCTransSlider.Value;
			this.FillProfileCombo();
			this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x000470B0 File Offset: 0x000452B0
		internal void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this);
				}
				this.mToastPopup.Init(this, message, null, null, global::System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x00009FA5 File Offset: 0x000081A5
		private void AdvancedSettingsItemPanel_Tap(object sender, EventArgs e)
		{
			this.AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel, true);
			KeymapCanvasWindow.sIsDirty = true;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x00009FBA File Offset: 0x000081BA
		private void AdvancedSettingsItemPanel_MouseDragStart(object sender, EventArgs e)
		{
			this.AddAdvancedControlToCanvas(sender as AdvancedSettingsItemPanel, false);
			base.Cursor = global::System.Windows.Input.Cursors.Arrow;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x00047138 File Offset: 0x00045338
		private void AddAdvancedControlToCanvas(AdvancedSettingsItemPanel sender, bool isTap = false)
		{
			if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
			{
				KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, null, false);
			}
			KMManager.CheckAndCreateNewScheme();
			if (!isTap)
			{
				base.Focus();
				base.Cursor = global::System.Windows.Input.Cursors.Hand;
			}
			KeyActionType actionType = sender.ActionType;
			IMAction imaction = Assembly.GetExecutingAssembly().CreateInstance(actionType.ToString()) as IMAction;
			KMManager.GetCanvasElement(this.ParentWindow, imaction, this.mCanvas, false);
			if (isTap)
			{
				List<IMAction> list = KMManager.ClearElement();
				this.CanvasWindow.AddNewCanvasElement(list, true);
				KMManager.ClearElement();
			}
			sender.ReatchedMouseMove();
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x00009FD4 File Offset: 0x000081D4
		private void mCanvas_PreviewMouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			KMManager.RepositionCanvasElement();
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x00009FDB File Offset: 0x000081DB
		private void mCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			base.Cursor = global::System.Windows.Input.Cursors.Arrow;
			KMManager.ClearElement();
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x000471DC File Offset: 0x000453DC
		private void mButtonsGrid_Loaded(object sender, RoutedEventArgs e)
		{
			base.MaxWidth = ((this.mButtonsGrid.ActualWidth < 320.0) ? 320.0 : this.mButtonsGrid.ActualWidth);
			base.MinWidth = base.MaxWidth;
			base.Left = ((this.CanvasWindow.SidebarWindowLeft == -1.0) ? (this.ParentWindow.Left + this.ParentWindow.ActualWidth - (double)(this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? 60 : 0)) : this.CanvasWindow.SidebarWindowLeft);
			base.Top = ((this.CanvasWindow.SidebarWindowTop == -1.0) ? this.ParentWindow.Top : this.CanvasWindow.SidebarWindowTop);
			base.Height = this.ParentWindow.ActualHeight;
			Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
			double sScalingFactor = MainWindow.sScalingFactor;
			Rectangle rectangle = new Rectangle((int)((double)screen.WorkingArea.X / sScalingFactor), (int)((double)screen.WorkingArea.Y / sScalingFactor), (int)((double)screen.WorkingArea.Width / sScalingFactor), (int)((double)screen.WorkingArea.Height / sScalingFactor));
			Rectangle rectangle2 = new Rectangle(new global::System.Drawing.Point((int)base.Left, (int)base.Top), new global::System.Drawing.Size((int)base.ActualWidth, (int)base.ActualHeight));
			if (!rectangle.Contains(rectangle2))
			{
				base.Left = (double)rectangle.Width - base.Width;
			}
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x0004737C File Offset: 0x0004557C
		public void ProfileChanged()
		{
			if (this.mSchemeComboBox.SelectedItem != null)
			{
				string selectedItem = this.mSchemeComboBox.SelectedItem;
				if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(selectedItem))
				{
					if (!this.ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem].Selected)
					{
						this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
						foreach (object obj in this.mSchemeComboBox.Items.Children)
						{
							ComboBoxSchemeControl comboBoxSchemeControl = (ComboBoxSchemeControl)obj;
							if (comboBoxSchemeControl.mSchemeName.Text == this.ParentWindow.SelectedConfig.SelectedControlScheme.Name)
							{
								BlueStacksUIBinding.BindColor(comboBoxSchemeControl, global::System.Windows.Controls.Control.BackgroundProperty, "ComboBoxBackgroundColor");
								break;
							}
						}
						this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[selectedItem];
						this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
						KeymapCanvasWindow.sIsDirty = true;
					}
					this.CanvasWindow.Init();
				}
				this.mRevertBtn.IsEnabled = this.ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) == 2;
			}
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x00009FEE File Offset: 0x000081EE
		private void AdvancedGameControlWindow_Loaded(object sender, RoutedEventArgs e)
		{
			base.Activate();
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x000474FC File Offset: 0x000456FC
		private void AdvancedGameControlWindow_KeyDown(object sender, global::System.Windows.Input.KeyEventArgs e)
		{
			string text = string.Empty;
			if (e.Key != Key.None)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					text = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
				}
				text += IMAPKeys.GetStringForFile(e.Key);
			}
			Logger.Debug("SHORTCUT: KeyPressed.." + text);
			if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
			{
				foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
				{
					if (string.Equals(shortcutKeys.ShortcutKey, text, StringComparison.InvariantCulture) && string.Equals(shortcutKeys.ShortcutName, "STRING_CONTROLS_EDITOR", StringComparison.InvariantCulture))
					{
						KMManager.CloseWindows();
					}
				}
			}
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x00047634 File Offset: 0x00045834
		private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = true;
					if (!Directory.Exists(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles")))
					{
						process.StartInfo.FileName = RegistryStrings.InputMapperFolder;
					}
					else
					{
						process.StartInfo.FileName = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
					}
					process.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error in Open folder err: " + ex.ToString());
			}
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000476E0 File Offset: 0x000458E0
		private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("ExportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail, null, null);
			if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
			{
				this.mOverlayGrid.Visibility = Visibility.Visible;
				if (this.mExportSchemesWindow == null)
				{
					this.mExportSchemesWindow = new ExportSchemesWindow(this.CanvasWindow, this.ParentWindow)
					{
						Owner = this
					};
					this.mExportSchemesWindow.Init();
					this.mExportSchemesWindow.Show();
					return;
				}
			}
			else
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_AVAILABLE", ""), 1.3, false);
			}
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x000477BC File Offset: 0x000459BC
		private void ImportBtn_Click(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("ImportKeymappingClicked", RegistryManager.Instance.UserGuid, KMManager.sPackageName, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, RegistryManager.Instance.RegisteredEmail, null, null);
			this.mOverlayGrid.Visibility = Visibility.Visible;
			using (OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "Cfg files (*.cfg)|*.cfg"
			})
			{
				if (openFileDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
				{
					this.mImportSchemesWindow = new ImportSchemesWindow(this.CanvasWindow, this.ParentWindow)
					{
						Owner = this
					};
					this.mImportSchemesWindow.Init(openFileDialog.FileName);
					this.mImportSchemesWindow.Show();
				}
				else
				{
					this.mOverlayGrid.Visibility = Visibility.Hidden;
					this.mImportSchemesWindow = null;
					base.Focus();
				}
			}
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x00009FF7 File Offset: 0x000081F7
		private void mCloseScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ToggleAGCWindowVisiblity(false);
			this.PopulateScriptTextBox();
			ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_close_click");
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x000478AC File Offset: 0x00045AAC
		private void PopulateScriptTextBox()
		{
			if (this.mLastScriptActionItem != null)
			{
				IMAction imaction = this.mLastScriptActionItem.First<IMAction>();
				if (imaction.Type == KeyActionType.Script)
				{
					string text = string.Join(Environment.NewLine, (imaction as Script).Commands.ToArray());
					this.mScriptText.Text = text;
					KeymapCanvasWindow.sIsDirty = true;
				}
			}
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x0000A01A File Offset: 0x0000821A
		private void ShowHelpHyperlink_Click(object sender, RoutedEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/help_articles") + "&article=keymapping_script_faq");
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x00047904 File Offset: 0x00045B04
		private void mDoneScriptButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mLastScriptActionItem != null)
			{
				IMAction imaction = this.mLastScriptActionItem.First<IMAction>();
				if (imaction.Type == KeyActionType.Script)
				{
					string[] array = this.mScriptText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
					if (!this.CheckIfScriptValid(array))
					{
						this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_INVALID_SCRIPT_COMMANDS", ""));
						return;
					}
					(imaction as Script).Commands.ClearAddRange(array.ToList<string>());
				}
			}
			this.ToggleAGCWindowVisiblity(false);
			ClientStats.SendKeyMappingUIStatsAsync("button_clicked", KMManager.sPackageName, "script_done_click");
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x000479A4 File Offset: 0x00045BA4
		private void NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mNCTransSlider.Value == 0.0)
			{
				this.mNCTransSlider.Value = this.mLastSliderValue;
				this.mNCTransparencyLevel.Text = ((int)(this.mNCTransSlider.Value * 100.0)).ToString(CultureInfo.InvariantCulture);
				if (this.mLastSliderValue > 0.0)
				{
					this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
				}
				RegistryManager.Instance.ShowKeyControlsOverlay = true;
			}
			else
			{
				this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
				double value = this.mNCTransSlider.Value;
				this.mNCTransSlider.Value = 0.0;
				this.mNCTransparencyLevel.Text = ((int)(this.mNCTransSlider.Value * 100.0)).ToString(CultureInfo.InvariantCulture);
				this.mLastSliderValue = value;
				RegistryManager.Instance.ShowKeyControlsOverlay = false;
			}
			KeymapCanvasWindow.sIsDirty = true;
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x00047AAC File Offset: 0x00045CAC
		private void NCTransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			KMManager.ChangeTransparency(this.ParentWindow, this.mNCTransSlider.Value);
			if (this.mNCTransSlider.Value == 0.0)
			{
				this.ParentWindow_OverlayStateChangedEvent(false);
			}
			else
			{
				this.ParentWindow_OverlayStateChangedEvent(true);
			}
			this.mLastSliderValue = this.mNCTransSlider.Value;
			this.mNCTransparencyLevel.Text = ((int)(this.mNCTransSlider.Value * 100.0)).ToString(CultureInfo.InvariantCulture);
			if (this.mNCTransSlider.Value != RegistryManager.Instance.TranslucentControlsTransparency)
			{
				KeymapCanvasWindow.sIsDirty = true;
			}
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x00047B54 File Offset: 0x00045D54
		public void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
		{
			if (isEnabled)
			{
				this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay";
				if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0 && this.mLastSliderValue == 0.0)
				{
					RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
					this.mNCTransSlider.Value = 0.5;
					this.mNCTransparencyLevel.Text = ((int)(this.mNCTransSlider.Value * 100.0)).ToString(CultureInfo.InvariantCulture);
				}
				else if (this.mNCTransSlider.Value == 0.0)
				{
					RegistryManager.Instance.TranslucentControlsTransparency = this.mLastSliderValue;
				}
				else
				{
					RegistryManager.Instance.TranslucentControlsTransparency = this.mNCTransSlider.Value;
				}
				RegistryManager.Instance.ShowKeyControlsOverlay = true;
				return;
			}
			this.mNCTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive";
			RegistryManager.Instance.TranslucentControlsTransparency = 0.0;
			double value = this.mNCTransSlider.Value;
			this.mNCTransSlider.Value = 0.0;
			this.mNCTransparencyLevel.Text = "0";
			this.mLastSliderValue = value;
			RegistryManager.Instance.ShowKeyControlsOverlay = false;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0000A03F File Offset: 0x0000823F
		private void BrowserHelp_MouseEnter(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			base.Cursor = global::System.Windows.Input.Cursors.Hand;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0000A04C File Offset: 0x0000824C
		private void BrowserHelp_MouseLeave(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			base.Cursor = global::System.Windows.Input.Cursors.Arrow;
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0000A059 File Offset: 0x00008259
		private void Export_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (this.mExport.IsEnabled)
			{
				this.mExport.Opacity = 1.0;
				return;
			}
			this.mExport.Opacity = 0.4;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0000A091 File Offset: 0x00008291
		private void KeySequenceScriptGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.KeySequenceScriptGrid.Visibility == Visibility.Visible && !this.mScriptText.IsMouseOver)
			{
				this.mAdvancedGameControlBorder.Focus();
			}
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0000A0B9 File Offset: 0x000082B9
		private void BrowserHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=advanced_game_control");
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x00047CA4 File Offset: 0x00045EA4
		private void RevertBtn_Click(object sender, RoutedEventArgs e)
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
			customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESET", delegate(object o, EventArgs e)
			{
				string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
				bool isBookMarked = this.ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
				this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
				IMControlScheme imcontrolScheme = this.ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture)).FirstOrDefault<IMControlScheme>();
				if (imcontrolScheme != null)
				{
					imcontrolScheme.Selected = true;
					this.ParentWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme;
					this.ParentWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
					imcontrolScheme.IsBookMarked = isBookMarked;
					this.FillProfileCombo();
					this.ProfileChanged();
					this.mSaveBtn.IsEnabled = false;
					this.mUndoBtn.IsEnabled = false;
					KeymapCanvasWindow.sIsDirty = true;
					KMManager.SaveIMActions(this.ParentWindow, false, false);
					ClientStats.SendKeyMappingUIStatsAsync("advancedcontrols_reset", KMManager.sPackageName, "");
				}
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", delegate(object o, EventArgs e)
			{
			}, null, false, null);
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x00047D54 File Offset: 0x00045F54
		private bool CheckIfScriptValid(string[] scriptCmds)
		{
			bool flag = false;
			try
			{
				JObject jobject = new JObject { 
				{
					"Commands",
					JArray.FromObject(scriptCmds.ToList<string>())
				} };
				flag = JObject.Parse((JToken.Parse(HTTPUtils.SendRequestToEngine("validateScriptCommands", new Dictionary<string, string> { 
				{
					"script",
					jobject.ToString(Formatting.None, new JsonConverter[0])
				} }, this.ParentWindow.mVmName, 0, null, false, 1, 0, "")) as JArray)[0].ToString())["success"].ToObject<bool>();
			}
			catch
			{
			}
			return flag;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x00047E00 File Offset: 0x00046000
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/advancedgamecontrolwindow.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x00047E30 File Offset: 0x00046030
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
				((AdvancedGameControlWindow)target).Loaded += this.AdvancedGameControlWindow_Loaded;
				((AdvancedGameControlWindow)target).Closing += this.AdvancedGameControlWindow_Closing;
				((AdvancedGameControlWindow)target).Closed += this.AdvancedGameControlWindow_Closed;
				((AdvancedGameControlWindow)target).KeyDown += this.AdvancedGameControlWindow_KeyDown;
				return;
			case 2:
				this.mAdvancedGameControlBorder = (Border)target;
				this.mAdvancedGameControlBorder.PreviewMouseDown += this.KeySequenceScriptGrid_PreviewMouseDown;
				return;
			case 3:
				this.PrimaryGrid = (Grid)target;
				return;
			case 4:
				((Grid)target).MouseLeftButtonDown += this.TopBar_MouseLeftButtonDown;
				return;
			case 5:
				((TextBlock)target).MouseLeftButtonDown += this.TopBar_MouseLeftButtonDown;
				return;
			case 6:
				this.mCloseSideBarWindow = (CustomPictureBox)target;
				this.mCloseSideBarWindow.MouseDown += this.CustomPictureBox_MouseDown;
				this.mCloseSideBarWindow.MouseLeftButtonUp += this.CloseButton_MouseLeftButtonUp;
				return;
			case 7:
				this.mProfileHeader = (TextBlock)target;
				return;
			case 8:
				this.mImport = (CustomPictureBox)target;
				this.mImport.MouseLeftButtonUp += this.ImportBtn_Click;
				return;
			case 9:
				this.mExport = (CustomPictureBox)target;
				this.mExport.IsEnabledChanged += this.Export_IsEnabledChanged;
				this.mExport.MouseLeftButtonUp += this.ExportBtn_Click;
				return;
			case 10:
				this.mOpenFolder = (CustomPictureBox)target;
				this.mOpenFolder.MouseLeftButtonUp += this.OpenFolder_MouseLeftButtonUp;
				return;
			case 11:
				this.mSchemeComboBox = (SchemeComboBox)target;
				return;
			case 12:
				this.mBrowserHelp = (CustomPictureBox)target;
				this.mBrowserHelp.MouseEnter += this.BrowserHelp_MouseEnter;
				this.mBrowserHelp.MouseLeave += this.BrowserHelp_MouseLeave;
				this.mBrowserHelp.MouseLeftButtonUp += this.BrowserHelp_MouseLeftButtonUp;
				return;
			case 13:
				this.mPrimitivesPanel = (WrapPanel)target;
				return;
			case 14:
				this.mTapPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 15:
				this.mTapRepeatPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 16:
				this.mDpadPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 17:
				this.mPanPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 18:
				this.mZoomPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 19:
				this.mMOBASkillPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 20:
				this.mSwipePrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 21:
				this.mFreeLookPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 22:
				this.mTiltPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 23:
				this.mStatePrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 24:
				this.mScriptPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 25:
				this.mMouseZoomPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 26:
				this.mRotatePrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 27:
				this.mScrollPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 28:
				this.mEdgeScrollPrimitive = (AdvancedSettingsItemPanel)target;
				return;
			case 29:
				this.mNCTransparencySlider = (Grid)target;
				return;
			case 30:
				this.mNCTransparencyLevel = (CustomTextBox)target;
				return;
			case 31:
				this.mNCTranslucentControlsSliderButton = (CustomPictureBox)target;
				this.mNCTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += this.NCTranslucentControlsSliderButton_PreviewMouseLeftButtonUp;
				return;
			case 32:
				this.mNCTransSlider = (Slider)target;
				this.mNCTransSlider.ValueChanged += this.NCTransparencySlider_ValueChanged;
				return;
			case 33:
				this.mButtonsGrid = (StackPanel)target;
				this.mButtonsGrid.Loaded += this.mButtonsGrid_Loaded;
				return;
			case 34:
				this.mRevertBtn = (CustomButton)target;
				this.mRevertBtn.Click += this.RevertBtn_Click;
				return;
			case 35:
				this.mUndoBtn = (CustomButton)target;
				this.mUndoBtn.Click += this.UndoButton_Click;
				return;
			case 36:
				this.mSaveBtn = (CustomButton)target;
				this.mSaveBtn.Click += this.SaveButton_Click;
				return;
			case 37:
				this.mCanvas = (Canvas)target;
				this.mCanvas.PreviewMouseMove += this.mCanvas_PreviewMouseMove;
				this.mCanvas.PreviewMouseUp += this.mCanvas_MouseUp;
				return;
			case 38:
				this.mOverlayGrid = (Grid)target;
				return;
			case 39:
				this.KeySequenceScriptGrid = (Grid)target;
				return;
			case 40:
				this.mScriptHeaderGrid = (Grid)target;
				this.mScriptHeaderGrid.MouseLeftButtonDown += this.TopBar_MouseLeftButtonDown;
				return;
			case 41:
				this.mHeaderText = (TextBlock)target;
				return;
			case 42:
				this.mCloseScriptWindow = (CustomPictureBox)target;
				this.mCloseScriptWindow.MouseDown += this.CustomPictureBox_MouseDown;
				this.mCloseScriptWindow.MouseLeftButtonUp += this.mCloseScriptButton_MouseLeftButtonUp;
				return;
			case 43:
				this.mSubheadingText = (TextBlock)target;
				return;
			case 44:
				this.mScriptText = (CustomTextBox)target;
				return;
			case 45:
				this.mXYCurrentCoordinatesText = (TextBlock)target;
				return;
			case 46:
				((Hyperlink)target).Click += this.ShowHelpHyperlink_Click;
				return;
			case 47:
				this.mShowHelpHyperlink = (TextBlock)target;
				return;
			case 48:
				this.mFooterGrid = (Grid)target;
				return;
			case 49:
				this.mFooterText = (TextBlock)target;
				return;
			case 50:
				this.mKeySeqDoneButton = (CustomButton)target;
				this.mKeySeqDoneButton.PreviewMouseLeftButtonUp += this.mDoneScriptButton_MouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040007B6 RID: 1974
		private MainWindow ParentWindow;

		// Token: 0x040007B7 RID: 1975
		internal KeymapCanvasWindow CanvasWindow;

		// Token: 0x040007B8 RID: 1976
		private CustomToastPopupControl mToastPopup;

		// Token: 0x040007B9 RID: 1977
		internal ExportSchemesWindow mExportSchemesWindow;

		// Token: 0x040007BA RID: 1978
		internal ImportSchemesWindow mImportSchemesWindow;

		// Token: 0x040007BB RID: 1979
		internal double mLastSliderValue;

		// Token: 0x040007BC RID: 1980
		internal double mLastSavedSliderValue;

		// Token: 0x040007BD RID: 1981
		internal Dictionary<string, string> mScriptModeDictionary = new Dictionary<string, string>();

		// Token: 0x040007BE RID: 1982
		internal List<IMAction> mLastScriptActionItem;

		// Token: 0x040007BF RID: 1983
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mAdvancedGameControlBorder;

		// Token: 0x040007C0 RID: 1984
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid PrimaryGrid;

		// Token: 0x040007C1 RID: 1985
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseSideBarWindow;

		// Token: 0x040007C2 RID: 1986
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mProfileHeader;

		// Token: 0x040007C3 RID: 1987
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mImport;

		// Token: 0x040007C4 RID: 1988
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mExport;

		// Token: 0x040007C5 RID: 1989
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mOpenFolder;

		// Token: 0x040007C6 RID: 1990
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SchemeComboBox mSchemeComboBox;

		// Token: 0x040007C7 RID: 1991
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mBrowserHelp;

		// Token: 0x040007C8 RID: 1992
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal WrapPanel mPrimitivesPanel;

		// Token: 0x040007C9 RID: 1993
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mTapPrimitive;

		// Token: 0x040007CA RID: 1994
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mTapRepeatPrimitive;

		// Token: 0x040007CB RID: 1995
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mDpadPrimitive;

		// Token: 0x040007CC RID: 1996
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mPanPrimitive;

		// Token: 0x040007CD RID: 1997
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mZoomPrimitive;

		// Token: 0x040007CE RID: 1998
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mMOBASkillPrimitive;

		// Token: 0x040007CF RID: 1999
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mSwipePrimitive;

		// Token: 0x040007D0 RID: 2000
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mFreeLookPrimitive;

		// Token: 0x040007D1 RID: 2001
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mTiltPrimitive;

		// Token: 0x040007D2 RID: 2002
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mStatePrimitive;

		// Token: 0x040007D3 RID: 2003
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mScriptPrimitive;

		// Token: 0x040007D4 RID: 2004
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mMouseZoomPrimitive;

		// Token: 0x040007D5 RID: 2005
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mRotatePrimitive;

		// Token: 0x040007D6 RID: 2006
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mScrollPrimitive;

		// Token: 0x040007D7 RID: 2007
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal AdvancedSettingsItemPanel mEdgeScrollPrimitive;

		// Token: 0x040007D8 RID: 2008
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNCTransparencySlider;

		// Token: 0x040007D9 RID: 2009
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mNCTransparencyLevel;

		// Token: 0x040007DA RID: 2010
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mNCTranslucentControlsSliderButton;

		// Token: 0x040007DB RID: 2011
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider mNCTransSlider;

		// Token: 0x040007DC RID: 2012
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mButtonsGrid;

		// Token: 0x040007DD RID: 2013
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mRevertBtn;

		// Token: 0x040007DE RID: 2014
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mUndoBtn;

		// Token: 0x040007DF RID: 2015
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSaveBtn;

		// Token: 0x040007E0 RID: 2016
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Canvas mCanvas;

		// Token: 0x040007E1 RID: 2017
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOverlayGrid;

		// Token: 0x040007E2 RID: 2018
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid KeySequenceScriptGrid;

		// Token: 0x040007E3 RID: 2019
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mScriptHeaderGrid;

		// Token: 0x040007E4 RID: 2020
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHeaderText;

		// Token: 0x040007E5 RID: 2021
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseScriptWindow;

		// Token: 0x040007E6 RID: 2022
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSubheadingText;

		// Token: 0x040007E7 RID: 2023
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mScriptText;

		// Token: 0x040007E8 RID: 2024
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mXYCurrentCoordinatesText;

		// Token: 0x040007E9 RID: 2025
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mShowHelpHyperlink;

		// Token: 0x040007EA RID: 2026
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mFooterGrid;

		// Token: 0x040007EB RID: 2027
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mFooterText;

		// Token: 0x040007EC RID: 2028
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mKeySeqDoneButton;

		// Token: 0x040007ED RID: 2029
		private bool _contentLoaded;
	}
}
