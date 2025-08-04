using System;
using System.Runtime.Serialization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001C8 RID: 456
	[Serializable]
	public class FractionException : Exception
	{
		// Token: 0x06001234 RID: 4660 RVA: 0x0000D06C File Offset: 0x0000B26C
		public FractionException()
		{
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x0000D074 File Offset: 0x0000B274
		public FractionException(string Message)
			: base(Message)
		{
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0000D07D File Offset: 0x0000B27D
		public FractionException(string Message, Exception InnerException)
			: base(Message, InnerException)
		{
		}

		// Token: 0x06001237 RID: 4663 RVA: 0x0000D087 File Offset: 0x0000B287
		protected FractionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
