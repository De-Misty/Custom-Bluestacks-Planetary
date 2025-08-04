using System;
using System.Drawing;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200027C RID: 636
	public struct IntereopRect : IEquatable<IntereopRect>
	{
		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06001723 RID: 5923 RVA: 0x0000F8CA File Offset: 0x0000DACA
		// (set) Token: 0x06001724 RID: 5924 RVA: 0x0000F8D2 File Offset: 0x0000DAD2
		public int Left { readonly get; set; }

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06001725 RID: 5925 RVA: 0x0000F8DB File Offset: 0x0000DADB
		// (set) Token: 0x06001726 RID: 5926 RVA: 0x0000F8E3 File Offset: 0x0000DAE3
		public int Top { readonly get; set; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06001727 RID: 5927 RVA: 0x0000F8EC File Offset: 0x0000DAEC
		// (set) Token: 0x06001728 RID: 5928 RVA: 0x0000F8F4 File Offset: 0x0000DAF4
		public int Right { readonly get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x0000F8FD File Offset: 0x0000DAFD
		// (set) Token: 0x0600172A RID: 5930 RVA: 0x0000F905 File Offset: 0x0000DB05
		public int Bottom { readonly get; set; }

		// Token: 0x0600172B RID: 5931 RVA: 0x0000F90E File Offset: 0x0000DB0E
		public IntereopRect(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x0000F92D File Offset: 0x0000DB2D
		public IntereopRect(Rectangle r)
		{
			this = new IntereopRect(r.Left, r.Top, r.Right, r.Bottom);
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x0000F951 File Offset: 0x0000DB51
		// (set) Token: 0x0600172E RID: 5934 RVA: 0x0000F959 File Offset: 0x0000DB59
		public int X
		{
			get
			{
				return this.Left;
			}
			set
			{
				this.Right -= this.Left - value;
				this.Left = value;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x0000F977 File Offset: 0x0000DB77
		// (set) Token: 0x06001730 RID: 5936 RVA: 0x0000F97F File Offset: 0x0000DB7F
		public int Y
		{
			get
			{
				return this.Top;
			}
			set
			{
				this.Bottom -= this.Top - value;
				this.Top = value;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x0000F99D File Offset: 0x0000DB9D
		// (set) Token: 0x06001732 RID: 5938 RVA: 0x0000F9AC File Offset: 0x0000DBAC
		public int Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
			set
			{
				this.Bottom = value + this.Top;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x0000F9BC File Offset: 0x0000DBBC
		// (set) Token: 0x06001734 RID: 5940 RVA: 0x0000F9CB File Offset: 0x0000DBCB
		public int Width
		{
			get
			{
				return this.Right - this.Left;
			}
			set
			{
				this.Right = value + this.Left;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001735 RID: 5941 RVA: 0x0000F9DB File Offset: 0x0000DBDB
		// (set) Token: 0x06001736 RID: 5942 RVA: 0x0000F9EE File Offset: 0x0000DBEE
		public Point Location
		{
			get
			{
				return new Point(this.Left, this.Top);
			}
			set
			{
				this.X = value.X;
				this.Y = value.Y;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06001737 RID: 5943 RVA: 0x0000FA0A File Offset: 0x0000DC0A
		// (set) Token: 0x06001738 RID: 5944 RVA: 0x0000FA1D File Offset: 0x0000DC1D
		public Size Size
		{
			get
			{
				return new Size(this.Width, this.Height);
			}
			set
			{
				this.Width = value.Width;
				this.Height = value.Height;
			}
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0000FA39 File Offset: 0x0000DC39
		public static implicit operator Rectangle(IntereopRect r)
		{
			return new Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x0000FA5C File Offset: 0x0000DC5C
		public static implicit operator IntereopRect(Rectangle r)
		{
			return new IntereopRect(r);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0008A07C File Offset: 0x0008827C
		public static bool operator ==(IntereopRect r1, IntereopRect r2)
		{
			if (r1.Equals(default(IntereopRect)))
			{
				return r2.Equals(default(IntereopRect));
			}
			return r1.Equals(r2);
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0000FA64 File Offset: 0x0000DC64
		public static bool operator !=(IntereopRect r1, IntereopRect r2)
		{
			return !(r1 == r2);
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0000FA70 File Offset: 0x0000DC70
		public bool Equals(IntereopRect r)
		{
			return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0000FAB0 File Offset: 0x0000DCB0
		public override bool Equals(object obj)
		{
			if (obj is IntereopRect)
			{
				return this.Equals((IntereopRect)obj);
			}
			return obj is Rectangle && this.Equals(new IntereopRect((Rectangle)obj));
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0008A0B4 File Offset: 0x000882B4
		public override int GetHashCode()
		{
			return this.GetHashCode();
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0008A0DC File Offset: 0x000882DC
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", new object[] { this.Left, this.Top, this.Right, this.Bottom });
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0000FAE2 File Offset: 0x0000DCE2
		public Rectangle ToRectangle()
		{
			return new Rectangle(this.Left, this.Top, this.Width, this.Height);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0000FB01 File Offset: 0x0000DD01
		public IntereopRect ToIntereopRect()
		{
			return new IntereopRect(this);
		}
	}
}
