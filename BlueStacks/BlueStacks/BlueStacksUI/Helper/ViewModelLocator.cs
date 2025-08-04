using System;
using GalaSoft.MvvmLight.Ioc;

namespace BlueStacks.BlueStacksUI.Helper
{
	// Token: 0x020002CE RID: 718
	public class ViewModelLocator
	{
		// Token: 0x06001AB8 RID: 6840 RVA: 0x00011D57 File Offset: 0x0000FF57
		static ViewModelLocator()
		{
			SimpleIoc.Default.Register<MinimizeBlueStacksOnCloseView>();
		}
	}
}
