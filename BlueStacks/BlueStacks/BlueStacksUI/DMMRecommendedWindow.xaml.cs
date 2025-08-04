using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000173 RID: 371
	public partial class DMMRecommendedWindow : CustomWindow
	{
		// Token: 0x06000EDA RID: 3802 RVA: 0x0005E17C File Offset: 0x0005C37C
		public DMMRecommendedWindow(MainWindow window)
		{
			this.InitializeComponent();
			base.IsShowGLWindow = true;
			this.ParentWindow = window;
			base.Owner = this.ParentWindow;
			base.Topmost = false;
			base.Left = ((this.ParentWindow != null) ? this.ParentWindow.Left : 0.0) + ((this.ParentWindow != null) ? this.ParentWindow.ActualWidth : 0.0);
			base.Top = ((this.ParentWindow != null) ? this.ParentWindow.Top : 0.0);
			base.Height = ((this.ParentWindow != null) ? this.ParentWindow.Height : 0.0);
			base.Width = (base.Height - (double)(((this.ParentWindow != null) ? this.ParentWindow.ParentWindowHeightDiff : 0) * 9)) / 16.0 + (double)((this.ParentWindow != null) ? this.ParentWindow.ParentWindowWidthDiff : 0);
			base.Closing += this.RecommendedWindow_Closing;
			base.IsVisibleChanged += this.RecommendedWindow_IsVisibleChanged;
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0005E2B0 File Offset: 0x0005C4B0
		private void RecommendedWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.Visibility == Visibility.Visible)
			{
				this.ParentWindow.mDmmBottomBar.mRecommendedWindowBtn.ImageName = "recommend_click";
			}
			else
			{
				this.ParentWindow.mDmmBottomBar.mRecommendedWindowBtn.ImageName = "recommend";
			}
			this.UpdateSize();
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0000B075 File Offset: 0x00009275
		private void RecommendedWindow_Closing(object sender, CancelEventArgs e)
		{
			this.ParentWindow.mDMMRecommendedWindow.mRecommendedBrowserControl.DisposeBrowser();
			this.ParentWindow.mDMMRecommendedWindow = null;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0005E304 File Offset: 0x0005C504
		public void Init(string url)
		{
			this.mRecommendedBrowserControl.mUrl = url;
			this.mRecommendedBrowserControl.mGrid = new Grid();
			this.mRecommendedBrowserControl.Content = this.mRecommendedBrowserControl.mGrid;
			this.mRecommendedBrowserControl.CreateNewBrowser();
			this.mRecommendedBrowserControl.ProcessMessageRecieved += this.MRecommendedBrowserControl_ProcessMessageRecieved;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0005E368 File Offset: 0x0005C568
		public void UpdateSize()
		{
			base.Top = this.ParentWindow.Top;
			base.Left = this.ParentWindow.Left + this.ParentWindow.Width;
			base.Height = this.ParentWindow.Height;
			base.Width = (this.ParentWindow.Height - (double)this.ParentWindow.ParentWindowHeightDiff) * 9.0 / 16.0 + (double)this.ParentWindow.ParentWindowWidthDiff;
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0000B098 File Offset: 0x00009298
		public void UpdateLocation()
		{
			base.Top = this.ParentWindow.Top;
			base.Left = this.ParentWindow.Left + this.ParentWindow.Width;
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x00004786 File Offset: 0x00002986
		private void MRecommendedBrowserControl_ProcessMessageRecieved(object sender, ProcessMessageEventArgs e)
		{
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0005E3F4 File Offset: 0x0005C5F4
		private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mIsDMMRecommendedWindowOpen = false;
			base.Hide();
			InteropWindow.ShowWindow(this.ParentWindow.Handle, 9);
			if (!this.ParentWindow.Topmost)
			{
				this.ParentWindow.Topmost = true;
				this.ParentWindow.Topmost = false;
			}
		}

		// Token: 0x040009B6 RID: 2486
		private MainWindow ParentWindow;
	}
}
