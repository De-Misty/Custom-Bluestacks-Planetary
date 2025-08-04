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
	// Token: 0x02000174 RID: 372
	public class SidebarPopup : UserControl, IComponentConnector
	{
		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x0000B0C8 File Offset: 0x000092C8
		private int NumColumns
		{
			get
			{
				return this.mMainStackPanel.Children.Count;
			}
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0000B0DA File Offset: 0x000092DA
		public SidebarPopup()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0005E4CC File Offset: 0x0005C6CC
		public void AddElement(SidebarElement element)
		{
			if (element != null)
			{
				SidebarPopup.RemoveParentIfExists(element);
				if (this.NumColumns == 0)
				{
					this.AddToNewPanel(element);
					return;
				}
				StackPanel stackPanel = this.mMainStackPanel.Children[this.NumColumns - 1] as StackPanel;
				if (stackPanel.Children.Count == 3)
				{
					this.AddToNewPanel(element);
					return;
				}
				stackPanel.Children.Add(element);
			}
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0005E534 File Offset: 0x0005C734
		private static void RemoveParentIfExists(SidebarElement element)
		{
			StackPanel stackPanel = element.Parent as StackPanel;
			if (stackPanel != null)
			{
				stackPanel.Children.Remove(element);
			}
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0000B0E8 File Offset: 0x000092E8
		private void AddToNewPanel(SidebarElement element)
		{
			this.CreateStackPanel().Children.Add(element);
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0005E55C File Offset: 0x0005C75C
		public SidebarElement PopElement()
		{
			StackPanel stackPanel = this.mMainStackPanel.Children[this.NumColumns - 1] as StackPanel;
			SidebarElement sidebarElement = stackPanel.Children[stackPanel.Children.Count - 1] as SidebarElement;
			stackPanel.Children.Remove(sidebarElement);
			if (stackPanel.Children.Count == 0)
			{
				this.mMainStackPanel.Children.Remove(stackPanel);
			}
			return sidebarElement;
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0005E5D0 File Offset: 0x0005C7D0
		private StackPanel CreateStackPanel()
		{
			StackPanel stackPanel = new StackPanel
			{
				Margin = new Thickness(2.0, 0.0, 2.0, 0.0),
				Orientation = Orientation.Vertical
			};
			this.mMainStackPanel.Children.Add(stackPanel);
			return stackPanel;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x00004786 File Offset: 0x00002986
		private void SidebarPopup_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x0005E62C File Offset: 0x0005C82C
		internal void InitAllElements(IEnumerable<SidebarElement> listOfHiddenElements)
		{
			foreach (SidebarElement sidebarElement in listOfHiddenElements)
			{
				if (sidebarElement.Visibility == Visibility.Visible)
				{
					sidebarElement.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
					this.AddElement(sidebarElement);
				}
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x0005E6AC File Offset: 0x0005C8AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebarpopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x0005E6DC File Offset: 0x0005C8DC
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
				((SidebarPopup)target).Loaded += this.SidebarPopup_Loaded;
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

		// Token: 0x040009BA RID: 2490
		private const int NumElementsPerRow = 3;

		// Token: 0x040009BB RID: 2491
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x040009BC RID: 2492
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMainStackPanel;

		// Token: 0x040009BD RID: 2493
		private bool _contentLoaded;
	}
}
