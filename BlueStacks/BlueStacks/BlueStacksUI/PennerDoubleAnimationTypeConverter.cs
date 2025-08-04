using System;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001CC RID: 460
	public class PennerDoubleAnimationTypeConverter : TypeConverter
	{
		// Token: 0x06001271 RID: 4721 RVA: 0x0000D3E3 File Offset: 0x0000B5E3
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0000D3F2 File Offset: 0x0000B5F2
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(Enum);
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x000714F0 File Offset: 0x0006F6F0
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			foreach (object obj in Enum.GetValues(typeof(PennerDoubleAnimation.Equations)))
			{
				int num = (int)obj;
				if (Enum.GetName(typeof(PennerDoubleAnimation.Equations), num) == ((value != null) ? value.ToString() : null))
				{
					return (PennerDoubleAnimation.Equations)num;
				}
			}
			return null;
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00071580 File Offset: 0x0006F780
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value != null)
			{
				return ((PennerDoubleAnimation.Equations)value).ToString();
			}
			return null;
		}
	}
}
