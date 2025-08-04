using System;
using System.Collections.Generic;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200017B RID: 379
	internal class NCSoftUtils
	{
		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000F4A RID: 3914 RVA: 0x00061178 File Offset: 0x0005F378
		internal static NCSoftUtils Instance
		{
			get
			{
				if (NCSoftUtils.mInstance == null)
				{
					object obj = NCSoftUtils.sync;
					lock (obj)
					{
						if (NCSoftUtils.mInstance == null)
						{
							NCSoftUtils.mInstance = new NCSoftUtils();
						}
					}
				}
				return NCSoftUtils.mInstance;
			}
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x000611C8 File Offset: 0x0005F3C8
		private int GetNCSoftAgentPort()
		{
			if (this.mNCSoftAgentPort != -1)
			{
				return this.mNCSoftAgentPort;
			}
			string text = "ngpmmf";
			uint num = 2U;
			try
			{
				this.mNCSoftAgentPort = MemoryMappedFile.GetNCSoftAgentPort(text, num);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get ncsoft agent port");
				Logger.Error(ex.ToString());
			}
			return this.mNCSoftAgentPort;
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x00061228 File Offset: 0x0005F428
		internal void SendAppCrashEvent(string crashReason, string vmName)
		{
			try
			{
				int ncsoftAgentPort = this.GetNCSoftAgentPort();
				if (ncsoftAgentPort != -1)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "vm_name", vmName },
						{ "err_message", crashReason }
					};
					Logger.Info("Sending app crash event to NCSoft Agent for vm: " + vmName);
					Logger.Info("Reason: " + crashReason);
					string text = HTTPUtils.SendRequestToNCSoftAgent(ncsoftAgentPort, "error/crash", dictionary, vmName, 0, null, false, 1, 0);
					Logger.Info("app crash event resp:");
					Logger.Info(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to report app crash. Ex : " + ex.ToString());
			}
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0000B2CA File Offset: 0x000094CA
		internal void SendGoogleLoginEventAsync(string vmName)
		{
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				try
				{
					int ncsoftAgentPort = this.GetNCSoftAgentPort();
					if (ncsoftAgentPort != -1)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>
						{
							{ "vm_name", vmName },
							{ "first", "true" }
						};
						Logger.Info("Sending google login event to NCSoft Agent for vm: " + vmName);
						string text = HTTPUtils.SendRequestToNCSoftAgent(ncsoftAgentPort, "account/google/login", dictionary, vmName, 0, null, false, 1, 0);
						Logger.Info("account google login event resp:");
						Logger.Info(text);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to report google login. Ex : " + ex.ToString());
				}
			});
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0000B2F0 File Offset: 0x000094F0
		internal void SendStreamingEvent(string vmName, string streamingStatus)
		{
			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				try
				{
					int ncsoftAgentPort = this.GetNCSoftAgentPort();
					if (ncsoftAgentPort != -1)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>
						{
							{ "button", "streaming" },
							{ "state", streamingStatus },
							{ "vm_name", vmName }
						};
						Logger.Info("Sending streaming event to NCSoft Agent for vm: " + vmName);
						Logger.Info("Status : " + streamingStatus);
						string text = HTTPUtils.SendRequestToNCSoftAgent(ncsoftAgentPort, "action/button/streaming", dictionary, vmName, 0, null, false, 1, 0);
						Logger.Info("action button streaming event resp:");
						Logger.Info(text);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to report action button streaming. Ex : " + ex.ToString());
				}
			});
		}

		// Token: 0x04000A09 RID: 2569
		private static object sync = new object();

		// Token: 0x04000A0A RID: 2570
		internal List<string> BlackListedApps = new List<string> { "com.bluestacks", "com.google", "com.android", "com.uncube" };

		// Token: 0x04000A0B RID: 2571
		private int mNCSoftAgentPort = -1;

		// Token: 0x04000A0C RID: 2572
		private static NCSoftUtils mInstance = null;
	}
}
