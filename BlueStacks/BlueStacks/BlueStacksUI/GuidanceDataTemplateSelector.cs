using System;
using System.Windows;
using System.Windows.Controls;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000063 RID: 99
	public class GuidanceDataTemplateSelector : DataTemplateSelector
	{
		// Token: 0x060004F7 RID: 1271 RVA: 0x0001F018 File Offset: 0x0001D218
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			FrameworkElement frameworkElement = container as FrameworkElement;
			if (frameworkElement != null && item != null)
			{
				if (item is GuidanceViewModel)
				{
					return frameworkElement.FindResource("GuidanceViewModelTemplate") as DataTemplate;
				}
				if (item is GuidanceEditTextModel)
				{
					return frameworkElement.FindResource("GuidanceEditTextModelTemplate") as DataTemplate;
				}
				if (item is GuidanceEditDecimalModel)
				{
					return frameworkElement.FindResource("GuidanceEditDecimalModelTemplate") as DataTemplate;
				}
				if (item is GuidanceCategoryViewModel)
				{
					return frameworkElement.FindResource("GuidanceCategoryViewModelTemplate") as DataTemplate;
				}
				if (item is GuidanceCategoryEditModel)
				{
					return frameworkElement.FindResource("GuidanceCategoryEditModelTemplate") as DataTemplate;
				}
			}
			return null;
		}
	}
}
