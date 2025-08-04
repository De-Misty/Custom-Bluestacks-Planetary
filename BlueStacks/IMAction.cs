using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using BlueStacks.Common;

// Token: 0x02000008 RID: 8
[Serializable]
public abstract class IMAction
{
	// Token: 0x06000050 RID: 80 RVA: 0x00012334 File Offset: 0x00010534
	public IMAction()
	{
		this.GetPropertyInfo("Type");
		if (this.ParentAction == null)
		{
			this.ParentAction = this;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000051 RID: 81 RVA: 0x0000235C File Offset: 0x0000055C
	// (set) Token: 0x06000052 RID: 82 RVA: 0x00002364 File Offset: 0x00000564
	public KeyActionType Type { get; set; }

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000053 RID: 83 RVA: 0x0000236D File Offset: 0x0000056D
	public Dictionary<string, string> Guidance { get; } = new Dictionary<string, string>();

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000054 RID: 84 RVA: 0x00002375 File Offset: 0x00000575
	// (set) Token: 0x06000055 RID: 85 RVA: 0x000023A1 File Offset: 0x000005A1
	public string GuidanceCategory
	{
		get
		{
			if (!this.IsChildAction)
			{
				return this.GetCurrentGuidanceCategory();
			}
			if (this.ParentAction != this)
			{
				return this.ParentAction.GuidanceCategory;
			}
			return this.GetCurrentGuidanceCategory();
		}
		set
		{
			this.mGuidanceCategory = ((value == null || string.IsNullOrEmpty(value.Trim())) ? "MISC" : value.Trim());
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x000023C6 File Offset: 0x000005C6
	private string GetCurrentGuidanceCategory()
	{
		if (string.IsNullOrEmpty(this.mGuidanceCategory))
		{
			this.mGuidanceCategory = "MISC";
		}
		return this.mGuidanceCategory;
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000057 RID: 87 RVA: 0x000023E6 File Offset: 0x000005E6
	// (set) Token: 0x06000058 RID: 88 RVA: 0x000023EE File Offset: 0x000005EE
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool Exclusive { get; set; }

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000059 RID: 89 RVA: 0x000023F7 File Offset: 0x000005F7
	// (set) Token: 0x0600005A RID: 90 RVA: 0x000023FF File Offset: 0x000005FF
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int ExclusiveDelay { get; set; } = 200;

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600005B RID: 91 RVA: 0x00002408 File Offset: 0x00000608
	// (set) Token: 0x0600005C RID: 92 RVA: 0x00002410 File Offset: 0x00000610
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string XExpr { get; set; } = string.Empty;

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600005D RID: 93 RVA: 0x00002419 File Offset: 0x00000619
	// (set) Token: 0x0600005E RID: 94 RVA: 0x00002421 File Offset: 0x00000621
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string YExpr { get; set; } = string.Empty;

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x0600005F RID: 95 RVA: 0x0000242A File Offset: 0x0000062A
	// (set) Token: 0x06000060 RID: 96 RVA: 0x00002432 File Offset: 0x00000632
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string XOverlayOffset { get; set; } = string.Empty;

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000061 RID: 97 RVA: 0x0000243B File Offset: 0x0000063B
	// (set) Token: 0x06000062 RID: 98 RVA: 0x00002443 File Offset: 0x00000643
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string YOverlayOffset { get; set; } = string.Empty;

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000063 RID: 99 RVA: 0x0000244C File Offset: 0x0000064C
	// (set) Token: 0x06000064 RID: 100 RVA: 0x00002454 File Offset: 0x00000654
	public string EnableCondition { get; set; } = string.Empty;

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000065 RID: 101 RVA: 0x0000245D File Offset: 0x0000065D
	// (set) Token: 0x06000066 RID: 102 RVA: 0x00002465 File Offset: 0x00000665
	public string StartCondition { get; set; }

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000067 RID: 103 RVA: 0x0000246E File Offset: 0x0000066E
	// (set) Token: 0x06000068 RID: 104 RVA: 0x00002476 File Offset: 0x00000676
	public string Note { get; set; }

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000069 RID: 105 RVA: 0x0000247F File Offset: 0x0000067F
	// (set) Token: 0x0600006A RID: 106 RVA: 0x00002487 File Offset: 0x00000687
	public string Comment { get; set; }

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600006B RID: 107 RVA: 0x000123BC File Offset: 0x000105BC
	// (set) Token: 0x0600006C RID: 108 RVA: 0x00002490 File Offset: 0x00000690
	internal double PositionX
	{
		get
		{
			double num;
			if (!double.TryParse(this[IMAction.sPositionXPropertyName[this.Type]].ToString(), out num))
			{
				num = -1.0;
			}
			return num;
		}
		set
		{
			this[IMAction.sPositionXPropertyName[this.Type]] = value;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600006D RID: 109 RVA: 0x000123F8 File Offset: 0x000105F8
	// (set) Token: 0x0600006E RID: 110 RVA: 0x000024AE File Offset: 0x000006AE
	internal double PositionY
	{
		get
		{
			double num;
			if (!double.TryParse(this[IMAction.sPositionYPropertyName[this.Type]].ToString(), out num))
			{
				num = -1.0;
			}
			return num;
		}
		set
		{
			this[IMAction.sPositionYPropertyName[this.Type]] = value;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600006F RID: 111 RVA: 0x00012434 File Offset: 0x00010634
	// (set) Token: 0x06000070 RID: 112 RVA: 0x000024CC File Offset: 0x000006CC
	internal double RadiusProperty
	{
		get
		{
			double num;
			if (!double.TryParse(this[IMAction.sRadiusPropertyName[this.Type]].ToString(), out num))
			{
				num = -1.0;
			}
			return num;
		}
		set
		{
			this[IMAction.sRadiusPropertyName[this.Type]] = value;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000071 RID: 113 RVA: 0x00012470 File Offset: 0x00010670
	// (set) Token: 0x06000072 RID: 114 RVA: 0x000024EA File Offset: 0x000006EA
	public bool IsVisibleInOverlay
	{
		get
		{
			bool flag;
			if (!bool.TryParse(this["ShowOnOverlay"].ToString(), out flag))
			{
				flag = false;
			}
			return flag;
		}
		set
		{
			this["ShowOnOverlay"] = value;
		}
	}

	// Token: 0x17000036 RID: 54
	public object this[string propertyName]
	{
		get
		{
			object obj = null;
			if (this.GetPropertyInfo(propertyName) != null)
			{
				obj = this.GetPropertyInfo(propertyName).GetValue(this, null);
			}
			return obj ?? string.Empty;
		}
		set
		{
			try
			{
				PropertyInfo propertyInfo = this.GetPropertyInfo(propertyName);
				if (propertyInfo != null)
				{
					propertyInfo.SetValue(this, Convert.ChangeType(value, this.GetPropertyInfo(propertyName).PropertyType, CultureInfo.InvariantCulture), null);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(Constants.ImapLocaleStringsConstant + " error parsing variable set " + ex.ToString());
			}
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00012538 File Offset: 0x00010738
	private PropertyInfo GetPropertyInfo(string propertyName)
	{
		PropertyInfo propertyInfo = null;
		KeyActionType keyActionType = EnumHelper.Parse<KeyActionType>(base.GetType().Name, KeyActionType.Alias);
		this.Type = keyActionType;
		if (!IMAction.DictPropertyInfo.ContainsKey(keyActionType))
		{
			IMAction.DictPropertyInfo[keyActionType] = new Dictionary<string, PropertyInfo>();
			IMAction.DictPopUpUIElements[keyActionType] = new Dictionary<string, PropertyInfo>();
			IMAction.sDictDevModeUIElements[keyActionType] = new Dictionary<string, PropertyInfo>();
			IMAction.sPositionXPropertyName[keyActionType] = string.Empty;
			IMAction.sPositionYPropertyName[keyActionType] = string.Empty;
			IMAction.sRadiusPropertyName[keyActionType] = string.Empty;
			foreach (PropertyInfo propertyInfo2 in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				IMAction.DictPropertyInfo[keyActionType].Add(propertyInfo2.Name, propertyInfo2);
				object[] customAttributes = propertyInfo2.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (customAttributes.Length != 0)
				{
					DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
					if (descriptionAttribute.Description.Contains("IMAP_CanvasElementY"))
					{
						IMAction.sPositionXPropertyName[keyActionType] = propertyInfo2.Name;
					}
					if (descriptionAttribute.Description.Contains("IMAP_CanvasElementX"))
					{
						IMAction.sPositionYPropertyName[keyActionType] = propertyInfo2.Name;
					}
					if (descriptionAttribute.Description.Contains("IMAP_CanvasElementRadius"))
					{
						IMAction.sRadiusPropertyName[keyActionType] = propertyInfo2.Name;
					}
					if (descriptionAttribute.Description.Contains("IMAP_PopupUIElement"))
					{
						IMAction.DictPopUpUIElements[keyActionType].Add(propertyInfo2.Name, propertyInfo2);
					}
					if (descriptionAttribute.Description.Contains("IMAP_DeveloperModeUIElemnt"))
					{
						IMAction.sDictDevModeUIElements[keyActionType].Add(propertyInfo2.Name, propertyInfo2);
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(propertyName) && IMAction.DictPropertyInfo[keyActionType].ContainsKey(propertyName))
		{
			propertyInfo = IMAction.DictPropertyInfo[keyActionType][propertyName];
		}
		return propertyInfo;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00012724 File Offset: 0x00010924
	internal List<BlueStacks.Common.Tuple<string, IMAction>> GetListGuidanceElements()
	{
		List<string> list = new List<string>();
		List<BlueStacks.Common.Tuple<string, IMAction>> list2 = new List<BlueStacks.Common.Tuple<string, IMAction>>();
		foreach (KeyValuePair<string, string> keyValuePair in this.Guidance)
		{
			if (keyValuePair.Key.StartsWith("Key", StringComparison.InvariantCulture))
			{
				list2.Add(new BlueStacks.Common.Tuple<string, IMAction>(keyValuePair.Key, this));
				list.Add(keyValuePair.Key);
			}
		}
		foreach (KeyValuePair<string, PropertyInfo> keyValuePair2 in IMAction.DictPropertyInfo[this.Type])
		{
			if (!list.Contains(keyValuePair2.Key) && (keyValuePair2.Key.StartsWith("Key", StringComparison.InvariantCulture) || keyValuePair2.Key.StartsWith("Sensitivity", StringComparison.InvariantCulture) || keyValuePair2.Key.StartsWith("MouseAcceleration", StringComparison.InvariantCulture) || keyValuePair2.Key.StartsWith("EdgeScrollEnabled", StringComparison.InvariantCulture)))
			{
				list2.Add(new BlueStacks.Common.Tuple<string, IMAction>(keyValuePair2.Key, this));
			}
		}
		return list2;
	}

	// Token: 0x04000024 RID: 36
	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> sDictDevModeUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();

	// Token: 0x04000025 RID: 37
	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPropertyInfo = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();

	// Token: 0x04000026 RID: 38
	internal static Dictionary<KeyActionType, Dictionary<string, PropertyInfo>> DictPopUpUIElements = new Dictionary<KeyActionType, Dictionary<string, PropertyInfo>>();

	// Token: 0x04000029 RID: 41
	private string mGuidanceCategory = "MISC";

	// Token: 0x04000034 RID: 52
	internal Direction Direction;

	// Token: 0x04000035 RID: 53
	internal IMAction ParentAction;

	// Token: 0x04000036 RID: 54
	private static Dictionary<KeyActionType, string> sPositionXPropertyName = new Dictionary<KeyActionType, string>();

	// Token: 0x04000037 RID: 55
	private static Dictionary<KeyActionType, string> sPositionYPropertyName = new Dictionary<KeyActionType, string>();

	// Token: 0x04000038 RID: 56
	internal static Dictionary<KeyActionType, string> sRadiusPropertyName = new Dictionary<KeyActionType, string>();

	// Token: 0x04000039 RID: 57
	internal bool IsChildAction;
}
