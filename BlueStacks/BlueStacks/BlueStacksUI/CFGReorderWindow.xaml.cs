using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000178 RID: 376
	public partial class CFGReorderWindow : CustomWindow
	{
		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x0000B180 File Offset: 0x00009380
		public static CFGReorderWindow Instance
		{
			get
			{
				if (CFGReorderWindow.mInstance == null)
				{
					CFGReorderWindow.mInstance = new CFGReorderWindow();
				}
				return CFGReorderWindow.mInstance;
			}
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0005FE54 File Offset: 0x0005E054
		public CFGReorderWindow()
		{
			this.InitializeComponent();
			base.Closing += this.CFGReorderWindow_Closing;
			base.Owner = BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName];
			base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0005FEB4 File Offset: 0x0005E0B4
		private void ClearState()
		{
			this.mCurrentlySelectedCFG = null;
			this.mLoadedCFGDict.Clear();
			this.mLoadedCFGsListView.Items.Clear();
			this.mSchemeTreeMapping.Clear();
			this.ClearIMLists();
			this.mLoadedCFGsListView.Visibility = Visibility.Collapsed;
			this.mSchemesListView.Visibility = Visibility.Collapsed;
			this.mIMActionsTreeView.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0000B198 File Offset: 0x00009398
		private void ClearIMLists()
		{
			this.mSchemesList = new ObservableCollection<IMControlScheme>();
			this.ClearIMActionsTree();
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0000B1AB File Offset: 0x000093AB
		private void ClearIMActionsTree()
		{
			this.mIMActionsTreeView.Items.Clear();
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0000B1BD File Offset: 0x000093BD
		private void CFGReorderWindow_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			base.Hide();
			this.ClearState();
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0005FF18 File Offset: 0x0005E118
		private void LoadCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ClearState();
			List<string> list = new List<string>();
			string text = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
			if (!Directory.Exists(text) || Directory.GetFileSystemEntries(text).Length == 0)
			{
				text = RegistryStrings.InputMapperFolder;
			}
			using (OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "BlueStacks keyboard controls (*.cfg)|*.cfg",
				InitialDirectory = text,
				Multiselect = true,
				RestoreDirectory = true
			})
			{
				if (openFileDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
				{
					foreach (string text2 in openFileDialog.FileNames)
					{
						if (!this.CheckValidCFGAndLoad(text2))
						{
							list.Add(Path.GetFileNameWithoutExtension(text2));
						}
					}
					if (list.Count > 0)
					{
						global::System.Windows.MessageBox.Show("The following CFG files could not be loaded.\n" + string.Join("\n", list.ToArray()));
					}
					if (this.mLoadedCFGDict.Count > 0)
					{
						this.InitCFGList();
						this.mLoadedCFGsListView.Visibility = Visibility.Visible;
						this.mLoadedCFGsListView.SelectedIndex = 0;
					}
				}
			}
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x00060034 File Offset: 0x0005E234
		private bool CheckValidCFGAndLoad(string filePath)
		{
			bool flag = false;
			try
			{
				IMConfig imconfig = JsonConvert.DeserializeObject<IMConfig>(File.ReadAllText(filePath), Utils.GetSerializerSettings());
				flag = true;
				this.mLoadedCFGDict.Add(filePath, imconfig);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to read cfg file... filepath: " + filePath + " Err : " + ex.Message);
			}
			return flag;
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x00060094 File Offset: 0x0005E294
		private List<IMAction> GetFinalListOfActions(Dictionary<string, List<IMAction>> dict)
		{
			List<IMAction> list = new List<IMAction>();
			foreach (string text in dict.Keys)
			{
				foreach (IMAction imaction in dict[text])
				{
					imaction.GuidanceCategory = text;
					list.Add(imaction);
				}
			}
			return list;
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x00060134 File Offset: 0x0005E334
		private void SaveCFGButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			foreach (object obj in ((IEnumerable)this.mLoadedCFGsListView.Items))
			{
				global::System.Windows.Controls.ListViewItem listViewItem = (global::System.Windows.Controls.ListViewItem)obj;
				if (listViewItem.Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
				{
					try
					{
						string text = listViewItem.Tag.ToString();
						IMConfig imconfig = this.mLoadedCFGDict[text];
						List<IMControlScheme> list = new List<IMControlScheme>();
						foreach (IMControlScheme imcontrolScheme in imconfig.ControlSchemes)
						{
							IMControlScheme imcontrolScheme2 = imcontrolScheme.DeepCopy();
							imcontrolScheme2.SetGameControls(this.GetFinalListOfActions(this.mSchemeTreeMapping[imconfig][imcontrolScheme]));
							list.Add(imcontrolScheme2);
						}
						imconfig.ControlSchemes = list;
						JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
						serializerSettings.Formatting = Formatting.Indented;
						this.WriteFile(text, JsonConvert.SerializeObject(imconfig, serializerSettings));
						listViewItem.Content = listViewItem.Content.ToString().TrimEnd("* (modified)".ToCharArray());
					}
					catch (Exception ex)
					{
						string text2 = string.Format("Couldn't write to file: {0}, Ex: {1}", listViewItem.Tag.ToString(), ex);
						Logger.Error(text2);
						global::System.Windows.MessageBox.Show(text2);
					}
				}
			}
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x000602E0 File Offset: 0x0005E4E0
		private void WriteFile(string fullFilePath, string output)
		{
			string text = fullFilePath + ".tmp";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.WriteAllText(text, output);
			if (File.Exists(fullFilePath))
			{
				File.Delete(fullFilePath);
			}
			File.Move(text, fullFilePath);
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x00060324 File Offset: 0x0005E524
		private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
		{
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			if (parent == null)
			{
				return default(T);
			}
			T t = parent as T;
			if (t != null)
			{
				return t;
			}
			return this.FindVisualParent<T>(parent);
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x00060364 File Offset: 0x0005E564
		private void MapTreeViewFromDict(Dictionary<string, List<IMAction>> dict)
		{
			foreach (string text in dict.Keys)
			{
				TreeViewItem treeViewItem = new TreeViewItem();
				treeViewItem.Header = text;
				foreach (IMAction imaction in dict[text])
				{
					TreeViewItem treeViewItem2 = new TreeViewItem();
					treeViewItem2.Header = CFGReorderWindow.GetGuidanceFromIMAction(imaction.Guidance.Values);
					treeViewItem2.Tag = imaction;
					treeViewItem.Items.Add(treeViewItem2);
				}
				this.mIMActionsTreeView.Items.Add(treeViewItem);
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00060448 File Offset: 0x0005E648
		private void BuildIMActionsTree()
		{
			if (this.mSchemeTreeMapping.ContainsKey(this.mCurrentlySelectedCFG) && this.mSchemeTreeMapping[this.mCurrentlySelectedCFG].ContainsKey(this.mCurrentlySelectedScheme))
			{
				this.MapTreeViewFromDict(this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][this.mCurrentlySelectedScheme]);
				return;
			}
			foreach (IMControlScheme imcontrolScheme in this.mCurrentlySelectedCFG.ControlSchemes)
			{
				Dictionary<string, List<IMAction>> dictionary = new Dictionary<string, List<IMAction>>();
				foreach (IMAction imaction in imcontrolScheme.GameControls)
				{
					if (!dictionary.ContainsKey(imaction.GuidanceCategory))
					{
						dictionary[imaction.GuidanceCategory] = new List<IMAction>();
					}
					dictionary[imaction.GuidanceCategory].Add(imaction);
				}
				if (!this.mSchemeTreeMapping.ContainsKey(this.mCurrentlySelectedCFG))
				{
					this.mSchemeTreeMapping[this.mCurrentlySelectedCFG] = new Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>();
				}
				this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][imcontrolScheme] = dictionary;
				if (imcontrolScheme == this.mCurrentlySelectedScheme)
				{
					this.MapTreeViewFromDict(dictionary);
				}
			}
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x000605BC File Offset: 0x0005E7BC
		private static string GetGuidanceFromIMAction(Dictionary<string, string>.ValueCollection valuePairs)
		{
			if (valuePairs.Count == 0)
			{
				return "NO_GUIDANCE";
			}
			string text = "";
			foreach (string text2 in valuePairs)
			{
				text = text + text2 + " / ";
			}
			if (text.Length > 5)
			{
				text = text.Substring(0, text.Length - 3);
			}
			return text;
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x00060640 File Offset: 0x0005E840
		private void InitCFGList()
		{
			foreach (string text in this.mLoadedCFGDict.Keys)
			{
				global::System.Windows.Controls.ListViewItem listViewItem = new global::System.Windows.Controls.ListViewItem();
				listViewItem.Content = Path.GetFileNameWithoutExtension(text);
				listViewItem.Tag = text;
				this.mLoadedCFGsListView.Items.Add(listViewItem);
			}
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0000B1D2 File Offset: 0x000093D2
		private void GenerateTreeView()
		{
			this.mIMActionsTreeView.PreviewMouseMove += this.TreeView_PreviewMouseMove;
			this.mIMActionsTreeView.ItemContainerStyle = this.GetIMActionsListStyle();
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x000606BC File Offset: 0x0005E8BC
		private void GenerateSchemesListView()
		{
			this.mSchemesListView.DisplayMemberPath = "Name";
			this.mSchemesListView.ItemsSource = this.mSchemesList;
			this.mSchemesListView.PreviewMouseMove += this.ListView_PreviewMouseMove;
			this.mSchemesListView.ItemContainerStyle = this.GetSchemesListStyle();
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00060714 File Offset: 0x0005E914
		private Style GetSchemesListStyle()
		{
			return new Style(typeof(global::System.Windows.Controls.ListViewItem))
			{
				Setters = 
				{
					new Setter(UIElement.AllowDropProperty, true),
					new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(this.AnyItem_PreviewMouseLeftButtonDown)),
					new EventSetter(UIElement.DropEvent, new global::System.Windows.DragEventHandler(this.SchemeItem_Drop))
				}
			};
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x00060790 File Offset: 0x0005E990
		private Style GetIMActionsListStyle()
		{
			return new Style(typeof(TreeViewItem))
			{
				Setters = 
				{
					new Setter(UIElement.AllowDropProperty, true),
					new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(this.AnyItem_PreviewMouseLeftButtonDown)),
					new EventSetter(UIElement.DropEvent, new global::System.Windows.DragEventHandler(this.IMActionItem_Drop))
				}
			};
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0006080C File Offset: 0x0005EA0C
		private void ListView_PreviewMouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			Point position = e.GetPosition(null);
			Vector vector = this._dragStartPoint - position;
			if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				global::System.Windows.Controls.ListViewItem listViewItem = this.FindVisualParent<global::System.Windows.Controls.ListViewItem>((DependencyObject)e.OriginalSource);
				if (listViewItem != null)
				{
					DragDrop.DoDragDrop(listViewItem, listViewItem.DataContext, global::System.Windows.DragDropEffects.Move);
				}
			}
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x00060880 File Offset: 0x0005EA80
		private void TreeView_PreviewMouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			Point position = e.GetPosition(null);
			Vector vector = this._dragStartPoint - position;
			if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				TreeViewItem treeViewItem = this.FindVisualParent<TreeViewItem>((DependencyObject)e.OriginalSource);
				if (treeViewItem != null)
				{
					DragDrop.DoDragDrop(treeViewItem, treeViewItem, global::System.Windows.DragDropEffects.Move);
				}
			}
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0000B1FC File Offset: 0x000093FC
		private void AnyItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._dragStartPoint = e.GetPosition(null);
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x000608F0 File Offset: 0x0005EAF0
		private void SchemeItem_Drop(object sender, global::System.Windows.DragEventArgs e)
		{
			IMControlScheme imcontrolScheme = e.Data.GetData(typeof(IMControlScheme)) as IMControlScheme;
			IMControlScheme imcontrolScheme2 = ((global::System.Windows.Controls.ListViewItem)sender).DataContext as IMControlScheme;
			int num = this.mSchemesListView.Items.IndexOf(imcontrolScheme);
			int num2 = this.mSchemesListView.Items.IndexOf(imcontrolScheme2);
			if (num == -1 || num2 == -1)
			{
				return;
			}
			this.MoveItem(imcontrolScheme, num, num2);
			this.mSchemesListView.Items.Refresh();
			this.MarkCurrentCFGModified();
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x00060978 File Offset: 0x0005EB78
		private void MarkCurrentCFGModified()
		{
			ListBoxItem listBoxItem = this.mLoadedCFGsListView.SelectedItem as ListBoxItem;
			if (!listBoxItem.Content.ToString().EndsWith("* (modified)", StringComparison.InvariantCulture))
			{
				ListBoxItem listBoxItem2 = listBoxItem;
				object content = listBoxItem2.Content;
				listBoxItem2.Content = ((content != null) ? content.ToString() : null) + "* (modified)";
			}
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x000609D0 File Offset: 0x0005EBD0
		public static ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
		{
			DependencyObject dependencyObject = VisualTreeHelper.GetParent(item);
			while (!(dependencyObject is TreeViewItem) && !(dependencyObject is global::System.Windows.Controls.TreeView))
			{
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			return dependencyObject as ItemsControl;
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x00060A04 File Offset: 0x0005EC04
		private void IMActionItem_Drop(object sender, global::System.Windows.DragEventArgs e)
		{
			TreeViewItem treeViewItem = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
			TreeViewItem treeViewItem2 = e.Source as TreeViewItem;
			CFGReorderWindow.TreeItemDrop treeItemDrop = new CFGReorderWindow.TreeItemDrop(treeViewItem, treeViewItem2, this.mIMActionsTreeView);
			this.MoveItem2(treeItemDrop);
			this.mIMActionsTreeView.Items.Refresh();
			this.UpdateTreeDictionary();
			this.MarkCurrentCFGModified();
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x00060A68 File Offset: 0x0005EC68
		private void UpdateTreeDictionary()
		{
			Dictionary<string, List<IMAction>> dictionary = new Dictionary<string, List<IMAction>>();
			foreach (object obj in ((IEnumerable)this.mIMActionsTreeView.Items))
			{
				TreeViewItem treeViewItem = (TreeViewItem)obj;
				List<IMAction> list = new List<IMAction>();
				foreach (object obj2 in ((IEnumerable)treeViewItem.Items))
				{
					TreeViewItem treeViewItem2 = (TreeViewItem)obj2;
					list.Add((IMAction)treeViewItem2.Tag);
				}
				dictionary[(string)treeViewItem.Header] = list;
			}
			this.mSchemeTreeMapping[this.mCurrentlySelectedCFG][this.mCurrentlySelectedScheme] = dictionary;
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x00060B58 File Offset: 0x0005ED58
		private void MoveItem2(CFGReorderWindow.TreeItemDrop item)
		{
			if (item.IsTargetCategory != item.IsSourceCategory)
			{
				return;
			}
			if (item.AreSourceAndTargetCategories)
			{
				if (this.mIMActionsTreeView.Items.Count > item.SourceIndex)
				{
					this.mIMActionsTreeView.Items.RemoveAt(item.SourceIndex);
					this.mIMActionsTreeView.Items.Insert(item.TargetIndex, item.Source);
					return;
				}
			}
			else if (item.SourceParent.Items.Count > item.SourceIndex && item.TargetParent.Items.Count > item.TargetIndex)
			{
				item.SourceParent.Items.RemoveAt(item.SourceIndex);
				item.TargetParent.Items.Insert(item.TargetIndex, item.Source);
			}
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x00060C2C File Offset: 0x0005EE2C
		private void MoveItem(IMControlScheme source, int sourceIndex, int targetIndex)
		{
			if (sourceIndex < targetIndex)
			{
				this.mSchemesList.Insert(targetIndex + 1, source);
				this.mSchemesList.RemoveAt(sourceIndex);
				return;
			}
			int num = sourceIndex + 1;
			if (this.mSchemesList.Count + 1 > num)
			{
				this.mSchemesList.Insert(targetIndex, source);
				this.mSchemesList.RemoveAt(num);
			}
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x00060C88 File Offset: 0x0005EE88
		private void LoadedCFGsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.mLoadedCFGsListView.SelectedItem == null)
			{
				return;
			}
			this.ClearIMLists();
			this.mCurrentlySelectedCFG = this.mLoadedCFGDict[(string)(this.mLoadedCFGsListView.SelectedItem as global::System.Windows.Controls.ListViewItem).Tag];
			this.mSchemesList = this.mCurrentlySelectedCFG.ControlSchemes;
			this.GenerateSchemesListView();
			this.mIMActionsTreeView.Items.Clear();
			if (this.mSchemesListView.Items.Count > 0)
			{
				this.mSchemesListView.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x00060D1C File Offset: 0x0005EF1C
		private void mSchemesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int selectedIndex = this.mSchemesListView.SelectedIndex;
			if (selectedIndex == -1)
			{
				return;
			}
			this.mCurrentlySelectedScheme = this.mSchemesList[selectedIndex];
			this.ClearIMActionsTree();
			this.BuildIMActionsTree();
			this.GenerateTreeView();
			if (this.mIMActionsTreeView.Items.Count > 0)
			{
				this.mIMActionsTreeView.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x00060D80 File Offset: 0x0005EF80
		private void mIMActionsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			this.mActionTextBox.IsEnabled = false;
			this.mActionTextBox.ScrollToLine(0);
			try
			{
				IMAction imaction = (IMAction)(this.mIMActionsTreeView.SelectedItem as TreeViewItem).Tag;
				if (imaction != null)
				{
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Formatting.Indented;
					this.mActionTextBox.Text = JsonConvert.SerializeObject(imaction, serializerSettings);
					this.mActionJsonGrid.Visibility = Visibility.Visible;
				}
				else
				{
					this.mActionTextBox.Text = "";
					this.mActionJsonGrid.Visibility = Visibility.Collapsed;
				}
			}
			catch
			{
				this.mActionTextBox.Text = "";
				this.mActionJsonGrid.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0000B20B File Offset: 0x0000940B
		private void EditButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			global::System.Windows.MessageBox.Show("Not implemented");
			this.mActionTextBox.IsEnabled = true;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0000B224 File Offset: 0x00009424
		private void SaveButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			global::System.Windows.MessageBox.Show("Not implemented");
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x00004786 File Offset: 0x00002986
		private void mLoadedCFGsListView_Scroll(object sender, global::System.Windows.Controls.Primitives.ScrollEventArgs e)
		{
		}

		// Token: 0x040009ED RID: 2541
		private IMConfig mCurrentlySelectedCFG;

		// Token: 0x040009EE RID: 2542
		private IMControlScheme mCurrentlySelectedScheme;

		// Token: 0x040009EF RID: 2543
		private Dictionary<string, IMConfig> mLoadedCFGDict = new Dictionary<string, IMConfig>();

		// Token: 0x040009F0 RID: 2544
		private IList<IMControlScheme> mSchemesList;

		// Token: 0x040009F1 RID: 2545
		private const string NO_GUIDANCE = "NO_GUIDANCE";

		// Token: 0x040009F2 RID: 2546
		private const string CFG_MODIFIED_SUFFIX = "* (modified)";

		// Token: 0x040009F3 RID: 2547
		private Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>> mSchemeTreeMapping = new Dictionary<IMConfig, Dictionary<IMControlScheme, Dictionary<string, List<IMAction>>>>();

		// Token: 0x040009F4 RID: 2548
		private static CFGReorderWindow mInstance;

		// Token: 0x040009F5 RID: 2549
		private Point _dragStartPoint;

		// Token: 0x02000179 RID: 377
		private class TreeItemDrop
		{
			// Token: 0x17000294 RID: 660
			// (get) Token: 0x06000F35 RID: 3893 RVA: 0x0000B231 File Offset: 0x00009431
			// (set) Token: 0x06000F36 RID: 3894 RVA: 0x0000B239 File Offset: 0x00009439
			public TreeViewItem Source { get; set; }

			// Token: 0x17000295 RID: 661
			// (get) Token: 0x06000F37 RID: 3895 RVA: 0x0000B242 File Offset: 0x00009442
			// (set) Token: 0x06000F38 RID: 3896 RVA: 0x0000B24A File Offset: 0x0000944A
			public TreeViewItem Target { get; set; }

			// Token: 0x17000296 RID: 662
			// (get) Token: 0x06000F39 RID: 3897 RVA: 0x0000B253 File Offset: 0x00009453
			// (set) Token: 0x06000F3A RID: 3898 RVA: 0x0000B25B File Offset: 0x0000945B
			public bool IsSourceCategory { get; set; }

			// Token: 0x17000297 RID: 663
			// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0000B264 File Offset: 0x00009464
			// (set) Token: 0x06000F3C RID: 3900 RVA: 0x0000B26C File Offset: 0x0000946C
			public bool IsTargetCategory { get; set; }

			// Token: 0x17000298 RID: 664
			// (get) Token: 0x06000F3D RID: 3901 RVA: 0x0000B275 File Offset: 0x00009475
			// (set) Token: 0x06000F3E RID: 3902 RVA: 0x0000B27D File Offset: 0x0000947D
			public TreeViewItem SourceParent { get; set; }

			// Token: 0x17000299 RID: 665
			// (get) Token: 0x06000F3F RID: 3903 RVA: 0x0000B286 File Offset: 0x00009486
			// (set) Token: 0x06000F40 RID: 3904 RVA: 0x0000B28E File Offset: 0x0000948E
			public TreeViewItem TargetParent { get; set; }

			// Token: 0x1700029A RID: 666
			// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0000B297 File Offset: 0x00009497
			// (set) Token: 0x06000F42 RID: 3906 RVA: 0x0000B29F File Offset: 0x0000949F
			public int SourceIndex { get; set; } = -1;

			// Token: 0x1700029B RID: 667
			// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0000B2A8 File Offset: 0x000094A8
			// (set) Token: 0x06000F44 RID: 3908 RVA: 0x0000B2B0 File Offset: 0x000094B0
			public int TargetIndex { get; set; } = -1;

			// Token: 0x1700029C RID: 668
			// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0000B2B9 File Offset: 0x000094B9
			// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0000B2C1 File Offset: 0x000094C1
			public bool AreSourceAndTargetCategories { get; set; }

			// Token: 0x06000F47 RID: 3911 RVA: 0x00060FD8 File Offset: 0x0005F1D8
			public TreeItemDrop(TreeViewItem sourceItem, TreeViewItem targetItem, global::System.Windows.Controls.TreeView currentTree)
			{
				this.Source = sourceItem;
				this.Target = targetItem;
				this.SourceParent = CFGReorderWindow.GetSelectedTreeViewItemParent(sourceItem) as TreeViewItem;
				if (this.SourceParent != null)
				{
					this.SourceIndex = this.SourceParent.Items.IndexOf(this.Source);
				}
				else
				{
					this.IsSourceCategory = true;
					this.SourceIndex = currentTree.Items.IndexOf(this.Source);
				}
				this.TargetParent = CFGReorderWindow.GetSelectedTreeViewItemParent(targetItem) as TreeViewItem;
				if (this.TargetParent != null)
				{
					this.TargetIndex = this.TargetParent.Items.IndexOf(this.Target);
				}
				else
				{
					this.IsTargetCategory = true;
					this.TargetIndex = currentTree.Items.IndexOf(this.Target);
				}
				this.AreSourceAndTargetCategories = this.SourceParent == null;
			}
		}
	}
}
