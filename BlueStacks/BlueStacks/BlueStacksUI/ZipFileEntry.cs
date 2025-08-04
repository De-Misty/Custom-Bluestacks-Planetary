using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200026B RID: 619
	public struct ZipFileEntry : IEquatable<ZipFileEntry>
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x0000F13F File Offset: 0x0000D33F
		// (set) Token: 0x0600166F RID: 5743 RVA: 0x0000F147 File Offset: 0x0000D347
		public Compression Method { readonly get; set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x0000F150 File Offset: 0x0000D350
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x0000F158 File Offset: 0x0000D358
		public string FilenameInZip { readonly get; set; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x0000F161 File Offset: 0x0000D361
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x0000F169 File Offset: 0x0000D369
		public uint FileSize { readonly get; set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001674 RID: 5748 RVA: 0x0000F172 File Offset: 0x0000D372
		// (set) Token: 0x06001675 RID: 5749 RVA: 0x0000F17A File Offset: 0x0000D37A
		public uint CompressedSize { readonly get; set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001676 RID: 5750 RVA: 0x0000F183 File Offset: 0x0000D383
		// (set) Token: 0x06001677 RID: 5751 RVA: 0x0000F18B File Offset: 0x0000D38B
		public uint HeaderOffset { readonly get; set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001678 RID: 5752 RVA: 0x0000F194 File Offset: 0x0000D394
		// (set) Token: 0x06001679 RID: 5753 RVA: 0x0000F19C File Offset: 0x0000D39C
		public uint FileOffset { readonly get; set; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x0600167A RID: 5754 RVA: 0x0000F1A5 File Offset: 0x0000D3A5
		// (set) Token: 0x0600167B RID: 5755 RVA: 0x0000F1AD File Offset: 0x0000D3AD
		public uint HeaderSize { readonly get; set; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600167C RID: 5756 RVA: 0x0000F1B6 File Offset: 0x0000D3B6
		// (set) Token: 0x0600167D RID: 5757 RVA: 0x0000F1BE File Offset: 0x0000D3BE
		public uint Crc32 { readonly get; set; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600167E RID: 5758 RVA: 0x0000F1C7 File Offset: 0x0000D3C7
		// (set) Token: 0x0600167F RID: 5759 RVA: 0x0000F1CF File Offset: 0x0000D3CF
		public DateTime ModifyTime { readonly get; set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06001680 RID: 5760 RVA: 0x0000F1D8 File Offset: 0x0000D3D8
		// (set) Token: 0x06001681 RID: 5761 RVA: 0x0000F1E0 File Offset: 0x0000D3E0
		public string Comment { readonly get; set; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001682 RID: 5762 RVA: 0x0000F1E9 File Offset: 0x0000D3E9
		// (set) Token: 0x06001683 RID: 5763 RVA: 0x0000F1F1 File Offset: 0x0000D3F1
		public bool EncodeUTF8 { readonly get; set; }

		// Token: 0x06001684 RID: 5764 RVA: 0x00086A6C File Offset: 0x00084C6C
		public override bool Equals(object obj)
		{
			if (obj is ZipFileEntry)
			{
				ZipFileEntry zipFileEntry = (ZipFileEntry)obj;
				return this.Equals(zipFileEntry);
			}
			return false;
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00086A94 File Offset: 0x00084C94
		public bool Equals(ZipFileEntry other)
		{
			return this.Method == other.Method && this.FilenameInZip == other.FilenameInZip && this.FileSize == other.FileSize && this.CompressedSize == other.CompressedSize && this.HeaderOffset == other.HeaderOffset && this.FileOffset == other.FileOffset && this.HeaderSize == other.HeaderSize && this.Crc32 == other.Crc32 && this.ModifyTime == other.ModifyTime && this.Comment == other.Comment && this.EncodeUTF8 == other.EncodeUTF8;
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x0000F1FA File Offset: 0x0000D3FA
		public override string ToString()
		{
			return this.FilenameInZip;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x0000F202 File Offset: 0x0000D402
		public static bool operator ==(ZipFileEntry left, ZipFileEntry right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x0000F20C File Offset: 0x0000D40C
		public static bool operator !=(ZipFileEntry left, ZipFileEntry right)
		{
			return !(left == right);
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00086B60 File Offset: 0x00084D60
		public override int GetHashCode()
		{
			return this.Method.GetHashCode() ^ this.FilenameInZip.GetHashCode() ^ this.FileSize.GetHashCode() ^ this.CompressedSize.GetHashCode() ^ this.HeaderOffset.GetHashCode() ^ this.FileOffset.GetHashCode() ^ this.HeaderSize.GetHashCode() ^ this.Crc32.GetHashCode() ^ this.ModifyTime.GetHashCode() ^ this.Comment.GetHashCode() ^ this.EncodeUTF8.GetHashCode();
		}
	}
}
