using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200007A RID: 122
	public class GuidanceListToStringConverter : IValueConverter
	{
		// Token: 0x060005D9 RID: 1497 RVA: 0x000221CC File Offset: 0x000203CC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				List<string> list = new List<string>();
				foreach (string text in enumerable)
				{
					list.Add(KMManager.GetKeyUIValue(text));
				}
				return string.Join(" / ", list.ToArray());
			}
			return string.Empty;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00005EB9 File Offset: 0x000040B9
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
