using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using BlueStacks.Common;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001C5 RID: 453
	public static class FileImporter
	{
		// Token: 0x060011EB RID: 4587 RVA: 0x0000CB6E File Offset: 0x0000AD6E
		internal static void Init(MainWindow window)
		{
			window.AllowDrop = true;
			window.DragEnter += FileImporter.HandleDragEnter;
			window.Drop += FileImporter.HandleDragDrop;
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0000CB9B File Offset: 0x0000AD9B
		private static void HandleDragDrop(object sender, DragEventArgs e)
		{
			new Thread(delegate
			{
				FileImporter.HandleDragDropAsync(e, sender as MainWindow);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0000CBCC File Offset: 0x0000ADCC
		private static bool IsSharedFolderEnabled(int fileSystem)
		{
			if (fileSystem == 0)
			{
				Logger.Info("Shared folders disabled");
				return false;
			}
			return true;
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0006FDF4 File Offset: 0x0006DFF4
		private static void HandleDragDropAsync(DragEventArgs evt, MainWindow window)
		{
			string mVmName = window.mVmName;
			if (FileImporter.IsSharedFolderEnabled(window.EngineInstanceRegistry.FileSystem))
			{
				try
				{
					Array array = (Array)evt.Data.GetData(DataFormats.FileDrop);
					List<string> list = new List<string>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					for (int i = 0; i < array.Length; i++)
					{
						string text = array.GetValue(i).ToString();
						string fileName = Path.GetFileName(text);
						if (string.Equals(Path.GetExtension(text), ".apk", StringComparison.InvariantCultureIgnoreCase) || string.Equals(Path.GetExtension(text), ".xapk", StringComparison.InvariantCultureIgnoreCase))
						{
							list.Add(text);
						}
						else
						{
							dictionary.Add(fileName, text);
						}
					}
					string text2 = RegistryStrings.SharedFolderDir;
					if (dictionary.Count > 0)
					{
						string text3 = Utils.CreateRandomBstSharedFolder(text2);
						text2 = Path.Combine(RegistryStrings.SharedFolderDir, text3);
						Logger.Info("Shared Folder path : " + text2);
						foreach (KeyValuePair<string, string> keyValuePair in dictionary)
						{
							Logger.Info("DragDrop File: {0}", new object[] { keyValuePair.Key });
							string text4 = Path.Combine(text2, keyValuePair.Key);
							try
							{
								FileSystem.CopyFile(keyValuePair.Value, text4, UIOption.AllDialogs);
								File.SetAttributes(text4, FileAttributes.Normal);
							}
							catch (Exception ex)
							{
								Logger.Error("Failed to copy file : " + keyValuePair.Value + "...Err : " + ex.ToString());
							}
						}
						JArray jarray = new JArray
						{
							new JObject
							{
								new JProperty("foldername", text3)
							}
						};
						Dictionary<string, string> dictionary2 = new Dictionary<string, string> { 
						{
							"data",
							jarray.ToString(Formatting.None, new JsonConverter[0])
						} };
						Logger.Info("Sending drag drop request: " + jarray.ToString());
						try
						{
							HTTPUtils.SendRequestToGuest("fileDrop", dictionary2, mVmName, 0, null, false, 1, 0, "bgp64");
						}
						catch (Exception ex2)
						{
							Logger.Error("Failed to send FileDrop request. err: " + ex2.ToString());
						}
					}
					if (list.Count > 0)
					{
						foreach (string text5 in list)
						{
							try
							{
								Dictionary<string, string> dictionary3 = new Dictionary<string, string> { { "filePath", text5 } };
								HTTPUtils.SendRequestToClient("dragDropInstall", dictionary3, mVmName, 0, null, false, 1, 0, "bgp64");
							}
							catch (Exception ex3)
							{
								Logger.Warning("Failed to send drag drop install. Err: " + ex3.Message);
							}
						}
					}
				}
				catch (Exception ex4)
				{
					Logger.Error("Error in DragDrop function: " + ex4.Message);
				}
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0007013C File Offset: 0x0006E33C
		public static void HandleDragEnter(object obj, DragEventArgs evt)
		{
			if (evt != null)
			{
				if (evt.Data.GetDataPresent(DataFormats.FileDrop))
				{
					evt.Effects = DragDropEffects.Copy;
					return;
				}
				Logger.Debug("FileDrop DataFormat not supported");
				string[] formats = evt.Data.GetFormats();
				Logger.Debug("Supported formats:");
				string[] array = formats;
				for (int i = 0; i < array.Length; i++)
				{
					Logger.Debug(array[i]);
				}
				evt.Effects = DragDropEffects.None;
			}
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x000701A4 File Offset: 0x0006E3A4
		public static string GetMimeFromFile(string filename)
		{
			string text = "";
			if (!File.Exists(filename))
			{
				return text;
			}
			byte[] array = new byte[256];
			using (FileStream fileStream = new FileStream(filename, FileMode.Open))
			{
				if (fileStream.Length >= 256L)
				{
					fileStream.Read(array, 0, 256);
				}
				else
				{
					fileStream.Read(array, 0, (int)fileStream.Length);
				}
			}
			try
			{
				uint num;
				NativeMethods.FindMimeFromData(0U, null, array, 256U, null, 0U, out num, 0U);
				IntPtr intPtr = new IntPtr((long)((ulong)num));
				text = Marshal.PtrToStringUni(intPtr);
				Marshal.FreeCoTaskMem(intPtr);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get mime type. err: " + ex.Message);
			}
			return text;
		}
	}
}
