using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000E6 RID: 230
	public class MacroBookmarksPopup : UserControl, IComponentConnector
	{
		// Token: 0x17000236 RID: 566
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x00008148 File Offset: 0x00006348
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x00008150 File Offset: 0x00006350
		private MainWindow ParentWindow { get; set; }

		// Token: 0x0600099F RID: 2463 RVA: 0x00008159 File Offset: 0x00006359
		public MacroBookmarksPopup()
		{
			this.InitializeComponent();
			this.InitList();
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00036450 File Offset: 0x00034650
		public void SetParentWindowAndBindEvents(MainWindow window)
		{
			this.ParentWindow = window;
			if (this.ParentWindow != null)
			{
				this.ParentWindow.mCommonHandler.MacroBookmarkChangedEvent += this.ParentWindow_MacroBookmarkChanged;
				this.ParentWindow.mCommonHandler.MacroSettingChangedEvent += this.ParentWindow_MacroSettingChangedEvent;
				this.ParentWindow.mCommonHandler.MacroDeletedEvent += this.ParentWindow_MacroDeletedEvent;
			}
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x000364C0 File Offset: 0x000346C0
		private void ParentWindow_MacroDeletedEvent(string fileName)
		{
			Grid gridByTag = this.GetGridByTag(fileName);
			if (gridByTag != null)
			{
				this.mMainStackPanel.Children.Remove(gridByTag);
			}
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x000364EC File Offset: 0x000346EC
		private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
		{
			try
			{
				this.mMainStackPanel.Children.Clear();
				this.InitList();
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't update name: {0}", new object[] { ex.Message });
			}
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00036540 File Offset: 0x00034740
		private void ParentWindow_MacroBookmarkChanged(string fileName, bool wasBookmarked)
		{
			if (wasBookmarked)
			{
				this.mMainStackPanel.Children.Add(this.CreateGrid(fileName));
				return;
			}
			foreach (object obj in this.mMainStackPanel.Children)
			{
				Grid grid = (Grid)obj;
				if ((string)grid.Tag == fileName)
				{
					this.mMainStackPanel.Children.Remove(grid);
					break;
				}
			}
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x000365DC File Offset: 0x000347DC
		private Grid GetGridByTag(string tag)
		{
			foreach (object obj in this.mMainStackPanel.Children)
			{
				Grid grid = (Grid)obj;
				if ((string)grid.Tag == tag)
				{
					return grid;
				}
			}
			return null;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00036650 File Offset: 0x00034850
		private void InitList()
		{
			foreach (string text in RegistryManager.Instance.BookmarkedScriptList)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Grid grid = this.CreateGrid(text);
					this.mMainStackPanel.Children.Add(grid);
				}
			}
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00004786 File Offset: 0x00002986
		private void MMacroBookmarksPopup_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0003669C File Offset: 0x0003489C
		private Grid CreateGrid(string fileName)
		{
			Grid grid = new Grid();
			grid.MouseEnter += this.GridElement_MouseEnter;
			grid.MouseLeave += this.GridElement_MouseLeave;
			grid.MouseLeftButtonUp += this.GridElement_MouseLeftButtonUp;
			grid.Background = Brushes.Transparent;
			grid.Tag = fileName;
			TextBlock textBlock = new TextBlock
			{
				FontSize = 12.0,
				TextTrimming = TextTrimming.CharacterEllipsis,
				Margin = new Thickness(10.0, 5.0, 10.0, 5.0)
			};
			BlueStacksUIBinding.BindColor(textBlock, TextBlock.ForegroundProperty, "GuidanceTextColorForeground");
			string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, fileName);
			if (File.Exists(text))
			{
				try
				{
					MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text), Utils.GetSerializerSettings());
					textBlock.Text = macroRecording.Name;
					textBlock.ToolTip = macroRecording.Name;
					goto IL_00F9;
				}
				catch
				{
					goto IL_00F9;
				}
			}
			textBlock.Text = fileName;
			textBlock.ToolTip = fileName;
			IL_00F9:
			grid.Children.Add(textBlock);
			return grid;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x000367C0 File Offset: 0x000349C0
		private void GridElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow.mIsMacroRecorderActive)
			{
				return;
			}
			string macroFileName = (string)(sender as Grid).Tag;
			MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where string.Equals(macro.Name, macroFileName, StringComparison.InvariantCultureIgnoreCase)
				select macro).FirstOrDefault<MacroRecording>();
			if (macroRecording == null)
			{
				this.mMainStackPanel.Children.Remove(sender as Grid);
				return;
			}
			try
			{
				if (!this.ParentWindow.mIsMacroPlaying)
				{
					this.ParentWindow.mCommonHandler.FullMacroScriptPlayHandler(macroRecording);
					ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "bookmark", macroRecording.RecordingType.ToString(), string.IsNullOrEmpty(macroRecording.MacroId) ? "local" : "community", null, null);
				}
				else
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
					BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
					customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
					customMessageWindow.Owner = this.ParentWindow;
					customMessageWindow.ShowDialog();
				}
				if (this.ParentWindow.mSidebar != null)
				{
					this.ParentWindow.mSidebar.mMacroButtonPopup.IsOpen = false;
					this.ParentWindow.mSidebar.ToggleSidebarVisibilityInFullscreen(false);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00007C8F File Offset: 0x00005E8F
		private void GridElement_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00006A61 File Offset: 0x00004C61
		private void GridElement_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00036954 File Offset: 0x00034B54
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrobookmarkspopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00036984 File Offset: 0x00034B84
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
				this.mMacroBookmarksPopup = (MacroBookmarksPopup)target;
				this.mMacroBookmarksPopup.Loaded += this.MMacroBookmarksPopup_Loaded;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				this.mMainStackPanel = (StackPanel)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000580 RID: 1408
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MacroBookmarksPopup mMacroBookmarksPopup;

		// Token: 0x04000581 RID: 1409
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x04000582 RID: 1410
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMainStackPanel;

		// Token: 0x04000583 RID: 1411
		private bool _contentLoaded;
	}
}
