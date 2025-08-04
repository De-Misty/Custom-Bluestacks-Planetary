using System;
using System.Collections.Generic;
using System.Linq;
using BlueStacks.Common;
using Newtonsoft.Json;

// Token: 0x0200001F RID: 31
[Serializable]
internal class IMConfig
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000264 RID: 612 RVA: 0x00003877 File Offset: 0x00001A77
	// (set) Token: 0x06000265 RID: 613 RVA: 0x0000387F File Offset: 0x00001A7F
	public MetaData MetaData { get; set; } = new MetaData();

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000266 RID: 614 RVA: 0x00003888 File Offset: 0x00001A88
	// (set) Token: 0x06000267 RID: 615 RVA: 0x00003890 File Offset: 0x00001A90
	public List<IMControlScheme> ControlSchemes { get; set; } = new List<IMControlScheme>();

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000268 RID: 616 RVA: 0x00003899 File Offset: 0x00001A99
	// (set) Token: 0x06000269 RID: 617 RVA: 0x000038A1 File Offset: 0x00001AA1
	[JsonIgnore]
	public Dictionary<string, IMControlScheme> ControlSchemesDict { get; private set; } = new Dictionary<string, IMControlScheme>();

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x0600026A RID: 618 RVA: 0x000038AA File Offset: 0x00001AAA
	// (set) Token: 0x0600026B RID: 619 RVA: 0x000038B2 File Offset: 0x00001AB2
	public Dictionary<string, Dictionary<string, string>> Strings { get; set; } = new Dictionary<string, Dictionary<string, string>>();

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x0600026C RID: 620 RVA: 0x000038BB File Offset: 0x00001ABB
	// (set) Token: 0x0600026D RID: 621 RVA: 0x000038C3 File Offset: 0x00001AC3
	[JsonIgnore]
	public IMControlScheme SelectedControlScheme { get; set; } = new IMControlScheme();

	// Token: 0x0600026E RID: 622 RVA: 0x00013808 File Offset: 0x00011A08
	internal string GetUIString(string key)
	{
		string text = key;
		if (this.Strings.ContainsKey(LocaleStrings.Locale) && this.Strings[LocaleStrings.Locale].ContainsKey(key))
		{
			text = this.Strings[LocaleStrings.Locale][key];
		}
		else if (this.Strings.ContainsKey("en-US") && this.Strings["en-US"].ContainsKey(key))
		{
			text = this.Strings["en-US"][key];
		}
		else if (this.Strings.ContainsKey("User-Defined") && this.Strings["User-Defined"].ContainsKey(key))
		{
			text = this.Strings["User-Defined"][key];
		}
		return text;
	}

	// Token: 0x0600026F RID: 623 RVA: 0x000038CC File Offset: 0x00001ACC
	internal void AddString(string key)
	{
		if (!this.Strings.ContainsKey("User-Defined"))
		{
			this.Strings.Add("User-Defined", new Dictionary<string, string>());
		}
		this.Strings["User-Defined"][key] = key;
	}

	// Token: 0x06000270 RID: 624 RVA: 0x000138E4 File Offset: 0x00011AE4
	public IMConfig DeepCopy()
	{
		IMConfig imconfig = (IMConfig)base.MemberwiseClone();
		MetaData metaData = this.MetaData;
		imconfig.MetaData = ((metaData != null) ? metaData.DeepCopy<MetaData>() : null);
		List<IMControlScheme> controlSchemes = this.ControlSchemes;
		List<IMControlScheme> list;
		if (controlSchemes == null)
		{
			list = null;
		}
		else
		{
			list = controlSchemes.ConvertAll<IMControlScheme>(delegate(IMControlScheme cs)
			{
				if (cs == null)
				{
					return null;
				}
				return cs.DeepCopy();
			});
		}
		imconfig.ControlSchemes = list;
		Dictionary<string, IMControlScheme> controlSchemesDict = this.ControlSchemesDict;
		Dictionary<string, IMControlScheme> dictionary;
		if (controlSchemesDict == null)
		{
			dictionary = null;
		}
		else
		{
			dictionary = controlSchemesDict.ToDictionary((KeyValuePair<string, IMControlScheme> kvp) => kvp.Key, delegate(KeyValuePair<string, IMControlScheme> kvp)
			{
				IMControlScheme value = kvp.Value;
				if (value == null)
				{
					return null;
				}
				return value.DeepCopy();
			});
		}
		imconfig.ControlSchemesDict = dictionary;
		Dictionary<string, Dictionary<string, string>> strings = this.Strings;
		Dictionary<string, Dictionary<string, string>> dictionary2;
		if (strings == null)
		{
			dictionary2 = null;
		}
		else
		{
			dictionary2 = strings.ToDictionary((KeyValuePair<string, Dictionary<string, string>> kvp) => kvp.Key, (KeyValuePair<string, Dictionary<string, string>> kvp) => kvp.Value);
		}
		imconfig.Strings = dictionary2;
		IMControlScheme selectedControlScheme = this.SelectedControlScheme;
		imconfig.SelectedControlScheme = ((selectedControlScheme != null) ? selectedControlScheme.DeepCopy() : null);
		return imconfig;
	}
}
