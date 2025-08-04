using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200017A RID: 378
	internal class DateTimeHelper
	{
		// Token: 0x06000F48 RID: 3912 RVA: 0x000610C0 File Offset: 0x0005F2C0
		internal static string GetReadableDateTimeString(DateTime yourDate)
		{
			if (yourDate.ToLocalTime().Date == DateTime.Now.Date)
			{
				return "Today at " + yourDate.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture);
			}
			if (yourDate.ToLocalTime().Date.Year == DateTime.Now.Date.Year)
			{
				return yourDate.ToLocalTime().ToString("%d MMM',' HH:mm", CultureInfo.InvariantCulture);
			}
			return yourDate.ToLocalTime().ToString("%d MMM yyyy',' HH:mm", CultureInfo.InvariantCulture);
		}
	}
}
