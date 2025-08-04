using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000269 RID: 617
	public class ZipStorer : IDisposable
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001651 RID: 5713 RVA: 0x0000F07F File Offset: 0x0000D27F
		// (set) Token: 0x06001652 RID: 5714 RVA: 0x0000F087 File Offset: 0x0000D287
		public bool EncodeUTF8 { get; set; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06001653 RID: 5715 RVA: 0x0000F090 File Offset: 0x0000D290
		// (set) Token: 0x06001654 RID: 5716 RVA: 0x0000F098 File Offset: 0x0000D298
		public bool ForceDeflating { get; set; }

		// Token: 0x06001655 RID: 5717 RVA: 0x00085B40 File Offset: 0x00083D40
		static ZipStorer()
		{
			for (int i = 0; i < ZipStorer.CrcTable.Length; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					if ((num & 1U) != 0U)
					{
						num = 3988292384U ^ (num >> 1);
					}
					else
					{
						num >>= 1;
					}
				}
				ZipStorer.CrcTable[i] = num;
			}
		}

		// Token: 0x06001656 RID: 5718 RVA: 0x0000F0A1 File Offset: 0x0000D2A1
		public static ZipStorer Create(string _filename, string _comment)
		{
			ZipStorer zipStorer = ZipStorer.Create(new FileStream(_filename, FileMode.Create, FileAccess.ReadWrite), _comment);
			zipStorer.Comment = _comment;
			zipStorer.FileName = _filename;
			return zipStorer;
		}

		// Token: 0x06001657 RID: 5719 RVA: 0x0000F0BF File Offset: 0x0000D2BF
		public static ZipStorer Create(Stream _stream, string _comment)
		{
			return new ZipStorer
			{
				Comment = _comment,
				ZipFileStream = _stream,
				Access = FileAccess.Write
			};
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x0000F0DB File Offset: 0x0000D2DB
		public static ZipStorer Open(string _filename, FileAccess _access)
		{
			ZipStorer zipStorer = ZipStorer.Open(new FileStream(_filename, FileMode.Open, (_access == FileAccess.Read) ? FileAccess.Read : FileAccess.ReadWrite), _access);
			zipStorer.FileName = _filename;
			return zipStorer;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x00085BA8 File Offset: 0x00083DA8
		public static ZipStorer Open(Stream _stream, FileAccess _access)
		{
			if (_stream != null)
			{
				if (!_stream.CanSeek && _access != FileAccess.Read)
				{
					throw new InvalidOperationException("Stream cannot seek");
				}
				ZipStorer zipStorer = new ZipStorer
				{
					ZipFileStream = _stream,
					Access = _access
				};
				if (zipStorer.ReadFileInfo())
				{
					return zipStorer;
				}
			}
			throw new InvalidDataException();
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x00085BF4 File Offset: 0x00083DF4
		public void AddFile(Compression _method, string _pathname, string _filenameInZip, string _comment)
		{
			if (this.Access == FileAccess.Read)
			{
				throw new InvalidOperationException("Writing is not alowed");
			}
			FileStream fileStream = new FileStream(_pathname, FileMode.Open, FileAccess.Read);
			this.AddStream(_method, _filenameInZip, fileStream, File.GetLastWriteTime(_pathname), _comment);
			fileStream.Close();
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x00085C38 File Offset: 0x00083E38
		public void AddStream(Compression _method, string _filenameInZip, Stream _source, DateTime _modTime, string _comment)
		{
			if (this.Access == FileAccess.Read || _source == null || string.IsNullOrEmpty(_filenameInZip))
			{
				throw new InvalidOperationException("Writing is not alowed");
			}
			ZipFileEntry zipFileEntry = new ZipFileEntry
			{
				Method = _method,
				EncodeUTF8 = this.EncodeUTF8,
				FilenameInZip = ZipStorer.NormalizedFilename(_filenameInZip),
				Comment = (_comment ?? ""),
				Crc32 = 0U,
				HeaderOffset = (uint)this.ZipFileStream.Position,
				ModifyTime = _modTime
			};
			this.WriteLocalHeader(ref zipFileEntry);
			zipFileEntry.FileOffset = (uint)this.ZipFileStream.Position;
			this.Store(ref zipFileEntry, _source);
			_source.Close();
			this.UpdateCrcAndSizes(ref zipFileEntry);
			this.Files.Add(zipFileEntry);
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x00085D04 File Offset: 0x00083F04
		public void Close()
		{
			if (this.Access != FileAccess.Read)
			{
				uint num = (uint)this.ZipFileStream.Position;
				uint num2 = 0U;
				if (this.CentralDirImage != null)
				{
					this.ZipFileStream.Write(this.CentralDirImage, 0, this.CentralDirImage.Length);
				}
				for (int i = 0; i < this.Files.Count; i++)
				{
					long position = this.ZipFileStream.Position;
					this.WriteCentralDirRecord(this.Files[i]);
					num2 += (uint)(this.ZipFileStream.Position - position);
				}
				if (this.CentralDirImage != null)
				{
					this.WriteEndRecord(num2 + (uint)this.CentralDirImage.Length, num);
				}
				else
				{
					this.WriteEndRecord(num2, num);
				}
			}
			if (this.ZipFileStream != null)
			{
				this.ZipFileStream.Flush();
				this.ZipFileStream.Dispose();
				this.ZipFileStream = null;
			}
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x00085DDC File Offset: 0x00083FDC
		public List<ZipFileEntry> ReadCentralDir()
		{
			if (this.CentralDirImage == null)
			{
				throw new InvalidOperationException("Central directory currently does not exist");
			}
			List<ZipFileEntry> list = new List<ZipFileEntry>();
			int num = 0;
			while (num < this.CentralDirImage.Length && BitConverter.ToUInt32(this.CentralDirImage, num) == 33639248U)
			{
				bool flag = (BitConverter.ToUInt16(this.CentralDirImage, num + 8) & 2048) > 0;
				ushort num2 = BitConverter.ToUInt16(this.CentralDirImage, num + 10);
				uint num3 = BitConverter.ToUInt32(this.CentralDirImage, num + 16);
				uint num4 = BitConverter.ToUInt32(this.CentralDirImage, num + 20);
				uint num5 = BitConverter.ToUInt32(this.CentralDirImage, num + 24);
				ushort num6 = BitConverter.ToUInt16(this.CentralDirImage, num + 28);
				ushort num7 = BitConverter.ToUInt16(this.CentralDirImage, num + 30);
				ushort num8 = BitConverter.ToUInt16(this.CentralDirImage, num + 32);
				uint num9 = BitConverter.ToUInt32(this.CentralDirImage, num + 42);
				uint num10 = (uint)(46 + num6 + num7 + num8);
				Encoding encoding = (flag ? Encoding.UTF8 : ZipStorer.DefaultEncoding);
				ZipFileEntry zipFileEntry = new ZipFileEntry
				{
					Method = (Compression)num2,
					FilenameInZip = encoding.GetString(this.CentralDirImage, num + 46, (int)num6),
					FileOffset = this.GetFileOffset(num9),
					FileSize = num5,
					CompressedSize = num4,
					HeaderOffset = num9,
					HeaderSize = num10,
					Crc32 = num3,
					ModifyTime = DateTime.Now
				};
				if (num8 > 0)
				{
					zipFileEntry.Comment = encoding.GetString(this.CentralDirImage, num + 46 + (int)num6 + (int)num7, (int)num8);
				}
				list.Add(zipFileEntry);
				num += (int)(46 + num6 + num7 + num8);
			}
			return list;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x00085F98 File Offset: 0x00084198
		public bool ExtractFile(ZipFileEntry _zfe, string _filename)
		{
			string directoryName = Path.GetDirectoryName(_filename);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (Directory.Exists(_filename))
			{
				return true;
			}
			bool flag;
			using (Stream stream = new FileStream(_filename, FileMode.Create, FileAccess.Write))
			{
				flag = this.ExtractFile(_zfe, stream);
			}
			File.SetCreationTime(_filename, _zfe.ModifyTime);
			File.SetLastWriteTime(_filename, _zfe.ModifyTime);
			return flag;
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x00086010 File Offset: 0x00084210
		public bool ExtractFile(ZipFileEntry _zfe, Stream _stream)
		{
			if (_stream == null || !_stream.CanWrite)
			{
				throw new InvalidOperationException("Stream cannot be written");
			}
			byte[] array = new byte[4];
			this.ZipFileStream.Seek((long)((ulong)_zfe.HeaderOffset), SeekOrigin.Begin);
			this.ZipFileStream.Read(array, 0, 4);
			if (BitConverter.ToUInt32(array, 0) != 67324752U)
			{
				return false;
			}
			Stream stream;
			if (_zfe.Method == Compression.Store)
			{
				stream = this.ZipFileStream;
			}
			else
			{
				if (_zfe.Method != Compression.Deflate)
				{
					return false;
				}
				stream = new DeflateStream(this.ZipFileStream, CompressionMode.Decompress, true);
			}
			byte[] array2 = new byte[16384];
			this.ZipFileStream.Seek((long)((ulong)_zfe.FileOffset), SeekOrigin.Begin);
			int num2;
			for (uint num = _zfe.FileSize; num > 0U; num -= (uint)num2)
			{
				num2 = stream.Read(array2, 0, (int)Math.Min((long)((ulong)num), (long)array2.Length));
				_stream.Write(array2, 0, num2);
			}
			_stream.Flush();
			if (_zfe.Method == Compression.Deflate)
			{
				stream.Dispose();
			}
			return true;
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x00086108 File Offset: 0x00084308
		public static bool RemoveEntries(ref ZipStorer _zip, List<ZipFileEntry> _zfes)
		{
			if (_zip == null || !(_zip.ZipFileStream is FileStream))
			{
				throw new InvalidOperationException("RemoveEntries is allowed just over streams of type FileStream");
			}
			List<ZipFileEntry> list = _zip.ReadCentralDir();
			string tempFileName = Path.GetTempFileName();
			string tempFileName2 = Path.GetTempFileName();
			try
			{
				ZipStorer zipStorer = ZipStorer.Create(tempFileName, string.Empty);
				foreach (ZipFileEntry zipFileEntry in list)
				{
					if (_zfes != null && !_zfes.Contains(zipFileEntry) && _zip.ExtractFile(zipFileEntry, tempFileName2))
					{
						zipStorer.AddFile(zipFileEntry.Method, tempFileName2, zipFileEntry.FilenameInZip, zipFileEntry.Comment);
					}
				}
				_zip.Close();
				zipStorer.Close();
				File.Delete(_zip.FileName);
				File.Move(tempFileName, _zip.FileName);
				_zip = ZipStorer.Open(_zip.FileName, _zip.Access);
			}
			catch
			{
				return false;
			}
			finally
			{
				if (File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}
				if (File.Exists(tempFileName2))
				{
					File.Delete(tempFileName2);
				}
			}
			return true;
		}

		// Token: 0x06001661 RID: 5729 RVA: 0x00086240 File Offset: 0x00084440
		private uint GetFileOffset(uint _headerOffset)
		{
			byte[] array = new byte[2];
			this.ZipFileStream.Seek((long)((ulong)(_headerOffset + 26U)), SeekOrigin.Begin);
			this.ZipFileStream.Read(array, 0, 2);
			ushort num = BitConverter.ToUInt16(array, 0);
			this.ZipFileStream.Read(array, 0, 2);
			ushort num2 = BitConverter.ToUInt16(array, 0);
			return (uint)((long)(30 + num + num2) + (long)((ulong)_headerOffset));
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x000862A0 File Offset: 0x000844A0
		private void WriteLocalHeader(ref ZipFileEntry _zfe)
		{
			long position = this.ZipFileStream.Position;
			byte[] bytes = (_zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding).GetBytes(_zfe.FilenameInZip);
			this.ZipFileStream.Write(new byte[] { 80, 75, 3, 4, 20, 0 }, 0, 6);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			this.ZipFileStream.Write(new byte[12], 0, 12);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
			_zfe.HeaderSize = (uint)(this.ZipFileStream.Position - position);
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x000863B4 File Offset: 0x000845B4
		private void WriteCentralDirRecord(ZipFileEntry _zfe)
		{
			Encoding encoding = (_zfe.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding);
			byte[] bytes = encoding.GetBytes(_zfe.FilenameInZip);
			byte[] bytes2 = encoding.GetBytes(_zfe.Comment);
			this.ZipFileStream.Write(new byte[] { 80, 75, 1, 2, 23, 11, 20, 0 }, 0, 8);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.EncodeUTF8 ? 2048 : 0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(ZipStorer.DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(0), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(33024), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
			this.ZipFileStream.Write(bytes2, 0, bytes2.Length);
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x00086580 File Offset: 0x00084780
		private void WriteEndRecord(uint _size, uint _offset)
		{
			byte[] bytes = (this.EncodeUTF8 ? Encoding.UTF8 : ZipStorer.DefaultEncoding).GetBytes(this.Comment);
			this.ZipFileStream.Write(new byte[] { 80, 75, 5, 6, 0, 0, 0, 0 }, 0, 8);
			this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes((int)((ushort)this.Files.Count + this.ExistingFiles)), 0, 2);
			this.ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			this.ZipFileStream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x00086664 File Offset: 0x00084864
		private void Store(ref ZipFileEntry _zfe, Stream _source)
		{
			byte[] array = new byte[16384];
			uint num = 0U;
			Stream stream = null;
			long position = this.ZipFileStream.Position;
			long position2 = _source.Position;
			if (_zfe.Method == Compression.Store)
			{
				stream = this.ZipFileStream;
			}
			else if (_zfe.Method == Compression.Deflate)
			{
				stream = new DeflateStream(this.ZipFileStream, CompressionMode.Compress, true);
			}
			_zfe.Crc32 = uint.MaxValue;
			int num2;
			do
			{
				num2 = _source.Read(array, 0, array.Length);
				num += (uint)num2;
				if (num2 > 0)
				{
					if (stream != null)
					{
						stream.Write(array, 0, num2);
					}
					uint num3 = 0U;
					while ((ulong)num3 < (ulong)((long)num2))
					{
						_zfe.Crc32 = ZipStorer.CrcTable[(int)((_zfe.Crc32 ^ (uint)array[(int)num3]) & 255U)] ^ (_zfe.Crc32 >> 8);
						num3 += 1U;
					}
				}
			}
			while (num2 == array.Length);
			stream.Flush();
			if (_zfe.Method == Compression.Deflate)
			{
				stream.Dispose();
			}
			_zfe.Crc32 ^= uint.MaxValue;
			_zfe.FileSize = num;
			_zfe.CompressedSize = (uint)(this.ZipFileStream.Position - position);
			if (_zfe.Method == Compression.Deflate && !this.ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
			{
				_zfe.Method = Compression.Store;
				this.ZipFileStream.Position = position;
				this.ZipFileStream.SetLength(position);
				_source.Position = position2;
				this.Store(ref _zfe, _source);
			}
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x000867BC File Offset: 0x000849BC
		private static uint DateTimeToDosTime(DateTime _dt)
		{
			return (uint)((_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) | (_dt.Day << 16) | (_dt.Month << 21) | (_dt.Year - 1980 << 25));
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x00086810 File Offset: 0x00084A10
		private void UpdateCrcAndSizes(ref ZipFileEntry _zfe)
		{
			long position = this.ZipFileStream.Position;
			this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 8U));
			this.ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			this.ZipFileStream.Position = (long)((ulong)(_zfe.HeaderOffset + 14U));
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			this.ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			this.ZipFileStream.Position = position;
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x000868C0 File Offset: 0x00084AC0
		private static string NormalizedFilename(string _filename)
		{
			string text = _filename.Replace('\\', '/');
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Remove(0, num + 1);
			}
			return text.Trim(new char[] { '/' });
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00086904 File Offset: 0x00084B04
		private bool ReadFileInfo()
		{
			if (this.ZipFileStream.Length < 22L)
			{
				return false;
			}
			try
			{
				this.ZipFileStream.Seek(-17L, SeekOrigin.End);
				using (BinaryReader binaryReader = new BinaryReader(this.ZipFileStream))
				{
					for (;;)
					{
						this.ZipFileStream.Seek(-5L, SeekOrigin.Current);
						if (binaryReader.ReadUInt32() == 101010256U)
						{
							break;
						}
						if (this.ZipFileStream.Position <= 0L)
						{
							goto Block_7;
						}
					}
					this.ZipFileStream.Seek(6L, SeekOrigin.Current);
					ushort num = binaryReader.ReadUInt16();
					int num2 = binaryReader.ReadInt32();
					uint num3 = binaryReader.ReadUInt32();
					ushort num4 = binaryReader.ReadUInt16();
					if (this.ZipFileStream.Position + (long)((ulong)num4) != this.ZipFileStream.Length)
					{
						return false;
					}
					this.ExistingFiles = num;
					this.CentralDirImage = new byte[num2];
					this.ZipFileStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
					this.ZipFileStream.Read(this.CentralDirImage, 0, num2);
					this.ZipFileStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
					return true;
					Block_7:;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0000F0F9 File Offset: 0x0000D2F9
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				this.Close();
				this.disposedValue = true;
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x00086A3C File Offset: 0x00084C3C
		~ZipStorer()
		{
			this.Dispose(false);
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x0000F112 File Offset: 0x0000D312
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000DAC RID: 3500
		private List<ZipFileEntry> Files = new List<ZipFileEntry>();

		// Token: 0x04000DAD RID: 3501
		private string FileName;

		// Token: 0x04000DAE RID: 3502
		private Stream ZipFileStream;

		// Token: 0x04000DAF RID: 3503
		private string Comment = "";

		// Token: 0x04000DB0 RID: 3504
		private byte[] CentralDirImage;

		// Token: 0x04000DB1 RID: 3505
		private ushort ExistingFiles;

		// Token: 0x04000DB2 RID: 3506
		private FileAccess Access;

		// Token: 0x04000DB3 RID: 3507
		private static uint[] CrcTable = new uint[256];

		// Token: 0x04000DB4 RID: 3508
		private static Encoding DefaultEncoding = Encoding.GetEncoding(437);

		// Token: 0x04000DB5 RID: 3509
		private bool disposedValue;
	}
}
