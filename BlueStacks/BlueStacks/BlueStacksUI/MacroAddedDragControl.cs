using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000BF RID: 191
	public class MacroAddedDragControl : UserControl, IComponentConnector, IStyleConnector
	{
		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600079A RID: 1946 RVA: 0x00006E73 File Offset: 0x00005073
		public MergeMacroWindow MergeMacroWindow
		{
			get
			{
				if (this.mMergeMacroWindow == null)
				{
					this.mMergeMacroWindow = Window.GetWindow(this) as MergeMacroWindow;
				}
				return this.mMergeMacroWindow;
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00006E94 File Offset: 0x00005094
		public MacroAddedDragControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0002AD24 File Offset: 0x00028F24
		internal void Init()
		{
			this.mListBox.DataContext = this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
			this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= this.Items_CollectionChanged;
			this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += this.Items_CollectionChanged;
			this.Items_CollectionChanged(null, null);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x0002AD98 File Offset: 0x00028F98
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Count > 0)
			{
				this.mNoMergeMacroGrid.Visibility = Visibility.Collapsed;
				this.mListBox.Visibility = Visibility.Visible;
				return;
			}
			this.mNoMergeMacroGrid.Visibility = Visibility.Visible;
			this.mListBox.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0002ADF0 File Offset: 0x00028FF0
		private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);
			Vector vector = this._dragStartPoint - position;
			if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				ListBoxItem listBoxItem = WpfUtils.FindVisualParent<ListBoxItem>((DependencyObject)e.OriginalSource);
				if (listBoxItem != null)
				{
					DragDrop.DoDragDrop(listBoxItem, listBoxItem.DataContext, DragDropEffects.Move);
				}
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00006EA2 File Offset: 0x000050A2
		private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this._dragStartPoint = e.GetPosition(null);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0002AE64 File Offset: 0x00029064
		private void ListBoxItem_Drop(object sender, DragEventArgs e)
		{
			this.UnsetMarginDuringDrag(null);
			if (sender is ListBoxItem)
			{
				MergedMacroConfiguration mergedMacroConfiguration = e.Data.GetData(typeof(MergedMacroConfiguration)) as MergedMacroConfiguration;
				MergedMacroConfiguration mergedMacroConfiguration2 = ((ListBoxItem)sender).DataContext as MergedMacroConfiguration;
				int num = this.mListBox.Items.IndexOf(mergedMacroConfiguration);
				int num2 = this.mListBox.Items.IndexOf(mergedMacroConfiguration2);
				this.Move(mergedMacroConfiguration, num, num2);
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0002AEDC File Offset: 0x000290DC
		private void ListBoxItem_DragOver(object sender, DragEventArgs e)
		{
			if (sender is ListBoxItem)
			{
				MergedMacroConfiguration mergedMacroConfiguration = e.Data.GetData(typeof(MergedMacroConfiguration)) as MergedMacroConfiguration;
				ListBoxItem listBoxItem = (ListBoxItem)sender;
				MergedMacroConfiguration mergedMacroConfiguration2 = ((ListBoxItem)sender).DataContext as MergedMacroConfiguration;
				int num = this.mListBox.Items.IndexOf(mergedMacroConfiguration);
				int num2 = this.mListBox.Items.IndexOf(mergedMacroConfiguration2);
				if (num2 < num)
				{
					(listBoxItem.Template.FindName("mMainGrid", listBoxItem) as Grid).Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
				}
				else if (num2 > num)
				{
					(listBoxItem.Template.FindName("mMainGrid", listBoxItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 10.0);
				}
				else
				{
					(listBoxItem.Template.FindName("mMainGrid", listBoxItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
				}
				this.UnsetMarginDuringDrag(listBoxItem);
			}
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0002B030 File Offset: 0x00029230
		private void UnsetMarginDuringDrag(ListBoxItem neglectItem = null)
		{
			foreach (object obj in ((IEnumerable)this.mListBox.Items))
			{
				ListBoxItem listBoxItem = this.mListBox.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
				if (neglectItem == null || listBoxItem != neglectItem)
				{
					(listBoxItem.Template.FindName("mMainGrid", listBoxItem) as Grid).Margin = new Thickness(0.0, -1.0, 0.0, 0.0);
				}
			}
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0002B0E4 File Offset: 0x000292E4
		private void Move(MergedMacroConfiguration source, int sourceIndex, int targetIndex)
		{
			if (sourceIndex < targetIndex)
			{
				ObservableCollection<MergedMacroConfiguration> observableCollection = this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>;
				if (observableCollection != null)
				{
					observableCollection.Insert(targetIndex + 1, source);
					observableCollection.RemoveAt(sourceIndex);
					return;
				}
			}
			else
			{
				ObservableCollection<MergedMacroConfiguration> observableCollection2 = this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>;
				if (observableCollection2 != null)
				{
					int num = sourceIndex + 1;
					if (observableCollection2.Count + 1 > num)
					{
						observableCollection2.Insert(targetIndex, source);
						observableCollection2.RemoveAt(num);
					}
				}
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0002B150 File Offset: 0x00029350
		private void ListBox_DragOver(object sender, DragEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			ScrollViewer scrollViewer = WpfUtils.FindVisualChild<ScrollViewer>(listBox);
			double num = 15.0;
			double y = e.GetPosition(listBox).Y;
			double num2 = 10.0;
			if (y < num)
			{
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - num2);
				return;
			}
			if (y > listBox.ActualHeight - num)
			{
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + num2);
			}
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0002B1C0 File Offset: 0x000293C0
		private void Group_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox != null)
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_group", null, null, null, null, null);
				MergedMacroConfiguration mergedMacroConfiguration = customPictureBox.DataContext as MergedMacroConfiguration;
				int num = this.mListBox.Items.IndexOf(mergedMacroConfiguration);
				this.Merge(num, num - 1);
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x0002B228 File Offset: 0x00029428
		private void Merge(int sourceIndex, int targetIndex)
		{
			ObservableCollection<MergedMacroConfiguration> observableCollection = this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>;
			if (observableCollection != null)
			{
				foreach (string text in observableCollection[sourceIndex].MacrosToRun)
				{
					observableCollection[targetIndex].MacrosToRun.Add(text);
				}
				observableCollection.RemoveAt(sourceIndex);
				MacroAddedDragControl.SetDefaultPropertiesForMergedMacroConfig(observableCollection[targetIndex]);
			}
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0002B2B0 File Offset: 0x000294B0
		private void UnGroup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox != null)
			{
				MergedMacroConfiguration mergedMacroConfiguration = customPictureBox.DataContext as MergedMacroConfiguration;
				int num = this.mListBox.Items.IndexOf(mergedMacroConfiguration);
				this.UnMerge(mergedMacroConfiguration, num);
			}
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x00006EB1 File Offset: 0x000050B1
		private static void SetDefaultPropertiesForMergedMacroConfig(MergedMacroConfiguration config)
		{
			config.LoopCount = 1;
			config.LoopInterval = 0;
			config.Acceleration = 1.0;
			config.DelayNextScript = 0;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0002B2F0 File Offset: 0x000294F0
		private void UnMerge(MergedMacroConfiguration source, int sourceIndex)
		{
			ObservableCollection<MergedMacroConfiguration> observableCollection = this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>;
			if (observableCollection != null)
			{
				MacroAddedDragControl.SetDefaultPropertiesForMergedMacroConfig(source);
				for (int i = 0; i < source.MacrosToRun.Count; i++)
				{
					string text = source.MacrosToRun[i];
					MergedMacroConfiguration mergedMacroConfiguration = new MergedMacroConfiguration();
					MergeMacroWindow mergeMacroWindow = this.MergeMacroWindow;
					int mAddedMacroTag = mergeMacroWindow.mAddedMacroTag;
					mergeMacroWindow.mAddedMacroTag = mAddedMacroTag + 1;
					mergedMacroConfiguration.Tag = mAddedMacroTag;
					MergedMacroConfiguration mergedMacroConfiguration2 = mergedMacroConfiguration;
					mergedMacroConfiguration2.MacrosToRun.Add(text);
					observableCollection.Insert(sourceIndex + i + 1, mergedMacroConfiguration2);
				}
				observableCollection.RemoveAt(sourceIndex);
			}
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x0002B380 File Offset: 0x00029580
		private void Remove_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox != null)
			{
				MergedMacroConfiguration mergedMacroConfiguration = customPictureBox.DataContext as MergedMacroConfiguration;
				int num = this.mListBox.Items.IndexOf(mergedMacroConfiguration);
				(this.mListBox.DataContext as ObservableCollection<MergedMacroConfiguration>).RemoveAt(num);
				this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Remove(mergedMacroConfiguration);
			}
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x0002B3E4 File Offset: 0x000295E4
		private void Settings_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox != null)
			{
				ListBoxItem listBoxItem = WpfUtils.FindVisualParent<ListBoxItem>(customPictureBox);
				MergedMacroConfiguration mergedMacroConfiguration = listBoxItem.DataContext as MergedMacroConfiguration;
				mergedMacroConfiguration.IsSettingsVisible = !mergedMacroConfiguration.IsSettingsVisible;
				(listBoxItem.Template.FindName("mMacroSettingsImage", listBoxItem) as CustomPictureBox).ImageName = (mergedMacroConfiguration.IsSettingsVisible ? "outline_settings_collapse" : "outline_settings_expand");
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, mergedMacroConfiguration.IsSettingsVisible ? "merge_dropdown_expand" : "merge_dropdown_collapse", null, null, null, null, null);
				foreach (object obj in ((IEnumerable)this.mListBox.Items))
				{
					ListBoxItem listBoxItem2 = this.mListBox.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
					if (listBoxItem2 != listBoxItem)
					{
						(listBoxItem2.DataContext as MergedMacroConfiguration).IsSettingsVisible = false;
						(listBoxItem2.Template.FindName("mMacroSettingsImage", listBoxItem2) as CustomPictureBox).ImageName = "outline_settings_expand";
					}
				}
			}
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x00006ED7 File Offset: 0x000050D7
		private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !this.IsTextAllowed(e.Text);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0002B524 File Offset: 0x00029724
		private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string text = (string)e.DataObject.GetData(typeof(string));
				if (!this.IsTextAllowed(text))
				{
					e.CancelCommand();
					return;
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00006EEE File Offset: 0x000050EE
		private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x00006F01 File Offset: 0x00005101
		private bool IsTextAllowed(string text)
		{
			return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x00006F22 File Offset: 0x00005122
		private void MacroAddDragControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.mListBox.DataContext = this.MergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations;
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0002B57C File Offset: 0x0002977C
		private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CustomTextBox customTextBox = sender as CustomTextBox;
			customTextBox.InputTextValidity = ((string.IsNullOrEmpty(customTextBox.Text) || customTextBox.Text == "0") ? TextValidityOptions.Error : TextValidityOptions.Success);
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x00006F3F File Offset: 0x0000513F
		private void MacroName_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			(sender as TextBlock).SetTextblockTooltip();
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x0002B5BC File Offset: 0x000297BC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macroaddeddragcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0002B5EC File Offset: 0x000297EC
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
				((MacroAddedDragControl)target).Loaded += this.MacroAddDragControl_Loaded;
				return;
			}
			if (connectionId == 12)
			{
				this.mNoMergeMacroGrid = (Border)target;
				return;
			}
			if (connectionId != 13)
			{
				this._contentLoaded = true;
				return;
			}
			this.mListBox = (ListBox)target;
			this.mListBox.DragOver += this.ListBox_DragOver;
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0002B65C File Offset: 0x0002985C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 2:
				((TextBlock)target).SizeChanged += this.MacroName_SizeChanged;
				return;
			case 3:
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DragOverEvent;
				eventSetter.Handler = new DragEventHandler(this.ListBoxItem_DragOver);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.DropEvent;
				eventSetter.Handler = new DragEventHandler(this.ListBoxItem_Drop);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			case 4:
				((CustomPictureBox)target).PreviewMouseLeftButtonDown += this.ListBoxItem_PreviewMouseLeftButtonDown;
				((CustomPictureBox)target).PreviewMouseMove += this.ListBox_PreviewMouseMove;
				return;
			case 5:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.UnGroup_PreviewMouseLeftButtonUp;
				return;
			case 6:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.Settings_PreviewMouseLeftButtonUp;
				return;
			case 7:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.Remove_PreviewMouseLeftButtonUp;
				return;
			case 8:
				((CustomTextBox)target).PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				((CustomTextBox)target).AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				((CustomTextBox)target).PreviewKeyDown += this.NumericTextBox_KeyDown;
				((CustomTextBox)target).TextChanged += this.LoopCountTextBox_TextChanged;
				return;
			case 9:
				((CustomTextBox)target).PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				((CustomTextBox)target).AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				((CustomTextBox)target).PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 10:
				((CustomTextBox)target).PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				((CustomTextBox)target).AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				((CustomTextBox)target).PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 11:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.Group_PreviewMouseLeftButtonUp;
				return;
			default:
				return;
			}
		}

		// Token: 0x04000422 RID: 1058
		private MergeMacroWindow mMergeMacroWindow;

		// Token: 0x04000423 RID: 1059
		private Point _dragStartPoint;

		// Token: 0x04000424 RID: 1060
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mNoMergeMacroGrid;

		// Token: 0x04000425 RID: 1061
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ListBox mListBox;

		// Token: 0x04000426 RID: 1062
		private bool _contentLoaded;
	}
}
