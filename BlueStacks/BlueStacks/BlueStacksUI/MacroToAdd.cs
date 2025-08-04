using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000C1 RID: 193
	public class MacroToAdd : UserControl, IComponentConnector
	{
		// Token: 0x060007BC RID: 1980 RVA: 0x00006F5F File Offset: 0x0000515F
		public MacroToAdd(MergeMacroWindow window, string macroName)
		{
			this.InitializeComponent();
			this.mMergeMacroWindow = window;
			base.Tag = macroName;
			this.mMacroName.Text = macroName;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0002BB00 File Offset: 0x00029D00
		private void AddMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MergedMacroConfiguration mergedMacroConfiguration = new MergedMacroConfiguration();
			mergedMacroConfiguration.MacrosToRun.Add(this.mMacroName.Text);
			MergedMacroConfiguration mergedMacroConfiguration2 = mergedMacroConfiguration;
			MergeMacroWindow mergeMacroWindow = this.mMergeMacroWindow;
			int mAddedMacroTag = mergeMacroWindow.mAddedMacroTag;
			mergeMacroWindow.mAddedMacroTag = mAddedMacroTag + 1;
			mergedMacroConfiguration2.Tag = mAddedMacroTag;
			if (this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations == null)
			{
				this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>();
			}
			this.mMergeMacroWindow.MergedMacroRecording.MergedMacroConfigurations.Add(mergedMacroConfiguration);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x00006F3F File Offset: 0x0000513F
		private void MacroName_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			(sender as TextBlock).SetTextblockTooltip();
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0002BB84 File Offset: 0x00029D84
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotoadd.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0002BBB4 File Offset: 0x00029DB4
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
				this.mMacroName = (TextBlock)target;
				this.mMacroName.SizeChanged += this.MacroName_SizeChanged;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.mAddMacro = (CustomPictureBox)target;
			this.mAddMacro.MouseLeftButtonUp += this.AddMacro_MouseLeftButtonUp;
		}

		// Token: 0x0400042B RID: 1067
		private MergeMacroWindow mMergeMacroWindow;

		// Token: 0x0400042C RID: 1068
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMacroName;

		// Token: 0x0400042D RID: 1069
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAddMacro;

		// Token: 0x0400042E RID: 1070
		private bool _contentLoaded;
	}
}
