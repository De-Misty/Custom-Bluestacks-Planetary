using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001DA RID: 474
	internal static class Stats
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x0000D584 File Offset: 0x0000B784
		// (set) Token: 0x060012A8 RID: 4776 RVA: 0x0000D598 File Offset: 0x0000B798
		private static string SessionId
		{
			get
			{
				if (Stats.sSessionId == null)
				{
					Stats.ResetSessionId();
				}
				return Stats.sSessionId;
			}
			set
			{
				Stats.sSessionId = value;
			}
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
		public static string GetSessionId()
		{
			return Stats.SessionId;
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x0000D5A7 File Offset: 0x0000B7A7
		public static string ResetSessionId()
		{
			Stats.SessionId = Stats.Timestamp;
			return Stats.SessionId;
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060012AB RID: 4779 RVA: 0x000728D4 File Offset: 0x00070AD4
		private static string Timestamp
		{
			get
			{
				long num = DateTime.Now.Ticks - DateTime.Parse("01/01/1970 00:00:00", CultureInfo.InvariantCulture).Ticks;
				return (num / 10000000L).ToString(CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x04000C13 RID: 3091
		private static string sSessionId;
	}
}
