using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000099 RID: 153
	internal sealed class GenericNotificationManager
	{
		// Token: 0x06000693 RID: 1683 RVA: 0x00003957 File Offset: 0x00001B57
		private GenericNotificationManager()
		{
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x00025C8C File Offset: 0x00023E8C
		public static GenericNotificationManager Instance
		{
			get
			{
				if (GenericNotificationManager.sInstance == null)
				{
					object obj = GenericNotificationManager.syncRoot;
					lock (obj)
					{
						if (GenericNotificationManager.sInstance == null)
						{
							GenericNotificationManager.sInstance = new GenericNotificationManager();
						}
					}
				}
				return GenericNotificationManager.sInstance;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x00006546 File Offset: 0x00004746
		internal static string GenericNotificationFilePath
		{
			get
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "bst_genericNotification");
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00025CE4 File Offset: 0x00023EE4
		public static void AddNewNotification(GenericNotificationItem notificationItem, bool dontOverwrite = false)
		{
			object obj = GenericNotificationManager.syncNotificationsReadWrite;
			lock (obj)
			{
				try
				{
					SerializableDictionary<string, GenericNotificationItem> savedNotifications = GenericNotificationManager.GetSavedNotifications();
					if (!dontOverwrite)
					{
						savedNotifications[notificationItem.Id] = notificationItem;
						GenericNotificationManager.SaveNotifications(savedNotifications);
					}
					else if (!savedNotifications.ContainsKey(notificationItem.Id))
					{
						savedNotifications[notificationItem.Id] = notificationItem;
						GenericNotificationManager.SaveNotifications(savedNotifications);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to add notification id : {0} titled : {1} and msg : {2}... Err : {3}", new object[]
					{
						notificationItem.Id,
						notificationItem.Title,
						notificationItem.Message,
						ex.ToString()
					});
				}
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00025D98 File Offset: 0x00023F98
		private static void SaveNotifications(SerializableDictionary<string, GenericNotificationItem> lstItem)
		{
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(GenericNotificationManager.GenericNotificationFilePath, Encoding.UTF8)
			{
				Formatting = Formatting.Indented
			})
			{
				SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
				foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in lstItem)
				{
					if (!keyValuePair.Value.IsDeleted)
					{
						serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				new XmlSerializer(typeof(SerializableDictionary<string, GenericNotificationItem>)).Serialize(xmlTextWriter, serializableDictionary);
				xmlTextWriter.Flush();
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00025E54 File Offset: 0x00024054
		private static SerializableDictionary<string, GenericNotificationItem> GetSavedNotifications()
		{
			SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
			if (File.Exists(GenericNotificationManager.GenericNotificationFilePath))
			{
				int i = 3;
				while (i > 0)
				{
					i--;
					try
					{
						using (XmlReader xmlReader = XmlReader.Create(GenericNotificationManager.GenericNotificationFilePath, new XmlReaderSettings
						{
							ProhibitDtd = true
						}))
						{
							serializableDictionary = (SerializableDictionary<string, GenericNotificationItem>)new XmlSerializer(typeof(SerializableDictionary<string, GenericNotificationItem>)).Deserialize(xmlReader);
							break;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Exception when reading saved notifications." + ex.ToString());
					}
				}
			}
			return serializableDictionary;
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00025EF4 File Offset: 0x000240F4
		internal GenericNotificationItem GetNotificationItem(string id)
		{
			return GenericNotificationManager.GetNotificationItems((GenericNotificationItem _) => _.Id == id).FirstOrDefault<KeyValuePair<string, GenericNotificationItem>>().Value;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x00025F2C File Offset: 0x0002412C
		public static SerializableDictionary<string, GenericNotificationItem> MarkNotification(IEnumerable<string> ids, Action<GenericNotificationItem> setter)
		{
			object obj = GenericNotificationManager.syncNotificationsReadWrite;
			SerializableDictionary<string, GenericNotificationItem> lstItem2;
			lock (obj)
			{
				SerializableDictionary<string, GenericNotificationItem> lstItem = new SerializableDictionary<string, GenericNotificationItem>();
				try
				{
					lstItem = GenericNotificationManager.GetSavedNotifications();
					Func<string, bool> func;
					Func<string, bool> <>9__0;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (string id) => id != null && lstItem.ContainsKey(id));
					}
					foreach (string text in ids.Where(func))
					{
						setter(lstItem[text]);
					}
					GenericNotificationManager.SaveNotifications(lstItem);
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to mark notification... Err : " + ex.ToString());
				}
				lstItem2 = lstItem;
			}
			return lstItem2;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x00026020 File Offset: 0x00024220
		public static SerializableDictionary<string, GenericNotificationItem> GetNotificationItems(Predicate<GenericNotificationItem> getter)
		{
			object obj = GenericNotificationManager.syncNotificationsReadWrite;
			SerializableDictionary<string, GenericNotificationItem> serializableDictionary2;
			lock (obj)
			{
				IEnumerable<KeyValuePair<string, GenericNotificationItem>> savedNotifications = GenericNotificationManager.GetSavedNotifications();
				SerializableDictionary<string, GenericNotificationItem> serializableDictionary = new SerializableDictionary<string, GenericNotificationItem>();
				Func<KeyValuePair<string, GenericNotificationItem>, bool> <>9__0;
				Func<KeyValuePair<string, GenericNotificationItem>, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (KeyValuePair<string, GenericNotificationItem> item) => getter(item.Value));
				}
				foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in savedNotifications.Where(func))
				{
					serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
				serializableDictionary2 = serializableDictionary;
			}
			return serializableDictionary2;
		}

		// Token: 0x04000361 RID: 865
		private static volatile GenericNotificationManager sInstance;

		// Token: 0x04000362 RID: 866
		private static object syncRoot = new object();

		// Token: 0x04000363 RID: 867
		private static object syncNotificationsReadWrite = new object();
	}
}
