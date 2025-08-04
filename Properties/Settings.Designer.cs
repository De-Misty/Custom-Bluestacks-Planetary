using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace BlueStacks.BlueStacksUI.Properties
{
	// Token: 0x020002BA RID: 698
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x1700038D RID: 909
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x000114ED File Offset: 0x0000F6ED
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400103A RID: 4154
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
