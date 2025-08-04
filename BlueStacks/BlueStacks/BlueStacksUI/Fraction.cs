using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001C7 RID: 455
	public class Fraction
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x0000CBF6 File Offset: 0x0000ADF6
		public Fraction()
		{
			this.Initialize(0L, 1L);
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0000CC08 File Offset: 0x0000AE08
		public Fraction(long iWholeNumber)
		{
			this.Initialize(iWholeNumber, 1L);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x00070270 File Offset: 0x0006E470
		public Fraction(double dDecimalValue)
		{
			Fraction fraction = Fraction.ToFraction(dDecimalValue);
			this.Initialize(fraction.Numerator, fraction.Denominator);
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x0007029C File Offset: 0x0006E49C
		public Fraction(string strValue)
		{
			Fraction fraction = Fraction.ToFraction(strValue);
			this.Initialize(fraction.Numerator, fraction.Denominator);
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x0000CC19 File Offset: 0x0000AE19
		public Fraction(long iNumerator, long iDenominator)
		{
			this.Initialize(iNumerator, iDenominator);
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x0000CC29 File Offset: 0x0000AE29
		private void Initialize(long iNumerator, long iDenominator)
		{
			this.Numerator = iNumerator;
			this.Denominator = iDenominator;
			Fraction.ReduceFraction(this);
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x0000CC3F File Offset: 0x0000AE3F
		// (set) Token: 0x060011FA RID: 4602 RVA: 0x0000CC47 File Offset: 0x0000AE47
		public long Denominator
		{
			get
			{
				return this.m_iDenominator;
			}
			set
			{
				if (value != 0L)
				{
					this.m_iDenominator = value;
					this.CalculateDoubleValue();
					return;
				}
				throw new FractionException("Denominator cannot be assigned a ZERO Value");
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x0000CC64 File Offset: 0x0000AE64
		// (set) Token: 0x060011FC RID: 4604 RVA: 0x0000CC6C File Offset: 0x0000AE6C
		public long Numerator
		{
			get
			{
				return this.m_iNumerator;
			}
			set
			{
				this.m_iNumerator = value;
				this.CalculateDoubleValue();
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060011FD RID: 4605 RVA: 0x0000CC7B File Offset: 0x0000AE7B
		// (set) Token: 0x060011FE RID: 4606 RVA: 0x0000CC83 File Offset: 0x0000AE83
		public double DoubleValue
		{
			get
			{
				return this.m_iDoubleValue;
			}
			set
			{
				this.m_iDoubleValue = value;
			}
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x0000CC8C File Offset: 0x0000AE8C
		private void CalculateDoubleValue()
		{
			this.m_iDoubleValue = (double)this.m_iNumerator / (double)this.m_iDenominator;
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x0000CCA3 File Offset: 0x0000AEA3
		public double ToDouble()
		{
			return (double)this.Numerator / (double)this.Denominator;
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x000702C8 File Offset: 0x0006E4C8
		public override string ToString()
		{
			string text;
			if (this.Denominator == 1L)
			{
				text = this.Numerator.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				text = this.Numerator.ToString() + "/" + this.Denominator.ToString();
			}
			return text;
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00070320 File Offset: 0x0006E520
		public static Fraction ToFraction(string strValue)
		{
			if (string.IsNullOrEmpty(strValue))
			{
				return null;
			}
			int num = 0;
			while (num < strValue.Length && strValue[num] != '/')
			{
				num++;
			}
			if (num == strValue.Length)
			{
				return Convert.ToDouble(strValue, CultureInfo.InvariantCulture);
			}
			long num2 = Convert.ToInt64(strValue.Substring(0, num), CultureInfo.InvariantCulture);
			long num3 = Convert.ToInt64(strValue.Substring(num + 1), CultureInfo.InvariantCulture);
			return new Fraction(num2, num3);
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x0007039C File Offset: 0x0006E59C
		public static Fraction ToFraction(double dValue)
		{
			Fraction fraction2;
			try
			{
				Fraction fraction;
				if (dValue % 1.0 == 0.0)
				{
					fraction = new Fraction(checked((long)dValue));
				}
				else
				{
					double num = dValue;
					long num2 = 1L;
					string text = dValue.ToString(CultureInfo.InvariantCulture);
					while (text.IndexOf("E", StringComparison.InvariantCulture) > 0)
					{
						num *= 10.0;
						checked
						{
							num2 *= 10L;
							text = num.ToString(CultureInfo.InvariantCulture);
						}
					}
					int num3 = 0;
					checked
					{
						while (text[num3] != '.')
						{
							num3++;
						}
						for (int i = text.Length - num3 - 1; i > 0; i--)
						{
							unchecked
							{
								num *= 10.0;
							}
							num2 *= 10L;
						}
					}
					fraction = new Fraction((long)(checked((int)Math.Round(num))), num2);
				}
				fraction2 = fraction;
			}
			catch (OverflowException)
			{
				throw new FractionException("Conversion not possible due to overflow");
			}
			catch (Exception)
			{
				throw new FractionException("Conversion not possible");
			}
			return fraction2;
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x0000CCB4 File Offset: 0x0000AEB4
		public Fraction Duplicate()
		{
			return new Fraction
			{
				Numerator = this.Numerator,
				Denominator = this.Denominator
			};
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x0007049C File Offset: 0x0006E69C
		public static Fraction Inverse(Fraction frac1)
		{
			if (null == frac1 || frac1.Numerator == 0L)
			{
				throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
			}
			long denominator = frac1.Denominator;
			long numerator = frac1.Numerator;
			return new Fraction(denominator, numerator);
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0000CCD3 File Offset: 0x0000AED3
		public static Fraction operator -(Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Negate(frac1);
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0000CCE6 File Offset: 0x0000AEE6
		public static Fraction operator +(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Add(frac1, frac2);
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x0000CD03 File Offset: 0x0000AF03
		public static Fraction operator +(int iNo, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, new Fraction((long)iNo));
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0000CD1D File Offset: 0x0000AF1D
		public static Fraction operator +(Fraction frac1, int iNo)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, new Fraction((long)iNo));
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0000CD37 File Offset: 0x0000AF37
		public static Fraction operator +(double dbl, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, Fraction.ToFraction(dbl));
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x0000CD50 File Offset: 0x0000AF50
		public static Fraction operator +(Fraction frac1, double dbl)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, Fraction.ToFraction(dbl));
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x0000CD69 File Offset: 0x0000AF69
		public static Fraction operator -(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Add(frac1, -frac2);
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0000CD8B File Offset: 0x0000AF8B
		public static Fraction operator -(int iNo, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(-frac1, new Fraction((long)iNo));
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x0000CDAA File Offset: 0x0000AFAA
		public static Fraction operator -(Fraction frac1, int iNo)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, -new Fraction((long)iNo));
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x0000CDC9 File Offset: 0x0000AFC9
		public static Fraction operator -(double dbl, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(-frac1, Fraction.ToFraction(dbl));
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x0000CDE7 File Offset: 0x0000AFE7
		public static Fraction operator -(Fraction frac1, double dbl)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Add(frac1, -Fraction.ToFraction(dbl));
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0000CE05 File Offset: 0x0000B005
		public static Fraction operator *(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Multiply(frac1, frac2);
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0000CE22 File Offset: 0x0000B022
		public static Fraction operator *(int iNo, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, new Fraction((long)iNo));
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x0000CE3C File Offset: 0x0000B03C
		public static Fraction operator *(Fraction frac1, int iNo)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, new Fraction((long)iNo));
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x0000CE56 File Offset: 0x0000B056
		public static Fraction operator *(double dbl, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.ToFraction(dbl));
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0000CE6F File Offset: 0x0000B06F
		public static Fraction operator *(Fraction frac1, double dbl)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.ToFraction(dbl));
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0000CE88 File Offset: 0x0000B088
		public static Fraction operator /(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.Inverse(frac2));
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x0000CEAA File Offset: 0x0000B0AA
		public static Fraction operator /(int iNo, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(Fraction.Inverse(frac1), new Fraction((long)iNo));
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x0000CEC9 File Offset: 0x0000B0C9
		public static Fraction operator /(Fraction frac1, int iNo)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.Inverse(new Fraction((long)iNo)));
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0000CEE8 File Offset: 0x0000B0E8
		public static Fraction operator /(double dbl, Fraction frac1)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(Fraction.Inverse(frac1), Fraction.ToFraction(dbl));
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0000CF06 File Offset: 0x0000B106
		public static Fraction operator /(Fraction frac1, double dbl)
		{
			if (!(null != frac1))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.Inverse(Fraction.ToFraction(dbl)));
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0000CF24 File Offset: 0x0000B124
		public static bool operator ==(Fraction frac1, Fraction frac2)
		{
			if (frac1 == null)
			{
				return frac2 == null;
			}
			return frac1.Equals(frac2);
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0000CF35 File Offset: 0x0000B135
		public static bool operator !=(Fraction frac1, Fraction frac2)
		{
			return !(frac1 == frac2);
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0000CF41 File Offset: 0x0000B141
		public static bool operator ==(Fraction frac1, int iNo)
		{
			return frac1 != null && frac1.Equals(new Fraction((long)iNo));
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0000CF55 File Offset: 0x0000B155
		public static bool operator !=(Fraction frac1, int iNo)
		{
			return !(frac1 == iNo);
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x0000CF61 File Offset: 0x0000B161
		public static bool operator ==(Fraction frac1, double dbl)
		{
			return frac1 != null && frac1.Equals(new Fraction(dbl));
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0000CF74 File Offset: 0x0000B174
		public static bool operator !=(Fraction frac1, double dbl)
		{
			return !(frac1 == dbl);
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0000CF80 File Offset: 0x0000B180
		public static bool operator <(Fraction frac1, Fraction frac2)
		{
			return null != frac1 && null != frac2 && frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0000CFB2 File Offset: 0x0000B1B2
		public static bool operator >(Fraction frac1, Fraction frac2)
		{
			return null != frac1 && null != frac2 && frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x000704D8 File Offset: 0x0006E6D8
		public static bool operator <=(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null == frac1 && null == frac2;
			}
			return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x0007052C File Offset: 0x0006E72C
		public static bool operator >=(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null == frac1 && null == frac2;
			}
			return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0000CFE4 File Offset: 0x0000B1E4
		public static implicit operator Fraction(long lNo)
		{
			return new Fraction(lNo);
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0000CFEC File Offset: 0x0000B1EC
		public static implicit operator Fraction(double dNo)
		{
			return new Fraction(dNo);
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0000CFF4 File Offset: 0x0000B1F4
		public static implicit operator Fraction(string strNo)
		{
			return new Fraction(strNo);
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0000CFFC File Offset: 0x0000B1FC
		public static explicit operator double(Fraction frac)
		{
			if (!(null != frac))
			{
				return 0.0;
			}
			return frac.ToDouble();
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0000D017 File Offset: 0x0000B217
		public static implicit operator string(Fraction frac)
		{
			if (!(null != frac))
			{
				return string.Empty;
			}
			return frac.ToString();
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x00070580 File Offset: 0x0006E780
		public override bool Equals(object obj)
		{
			Fraction fraction = (Fraction)obj;
			long numerator = this.Numerator;
			long? num = ((fraction != null) ? new long?(fraction.Numerator) : null);
			if ((numerator == num.GetValueOrDefault()) & (num != null))
			{
				long denominator = this.Denominator;
				num = ((fraction != null) ? new long?(fraction.Denominator) : null);
				return (denominator == num.GetValueOrDefault()) & (num != null);
			}
			return false;
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0000D02E File Offset: 0x0000B22E
		public override int GetHashCode()
		{
			return Convert.ToInt32((this.Numerator ^ this.Denominator) & (long)((ulong)(-1)));
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x000705FC File Offset: 0x0006E7FC
		public static Fraction Negate(Fraction frac1)
		{
			if (frac1 != null)
			{
				long num = -frac1.Numerator;
				long denominator = frac1.Denominator;
				return new Fraction(num, denominator);
			}
			return null;
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x00070624 File Offset: 0x0006E824
		public static Fraction Add(Fraction frac1, Fraction frac2)
		{
			checked
			{
				Fraction fraction;
				try
				{
					if (frac1 != null && frac2 != null)
					{
						long num = frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator;
						long num2 = frac1.Denominator * frac2.Denominator;
						fraction = new Fraction(num, num2);
					}
					else if (frac1 != null)
					{
						fraction = frac1;
					}
					else
					{
						fraction = frac2;
					}
				}
				catch (OverflowException)
				{
					throw new FractionException("Overflow occurred while performing arithemetic operation");
				}
				catch (Exception)
				{
					throw new FractionException("An error occurred while performing arithemetic operation");
				}
				return fraction;
			}
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x000706AC File Offset: 0x0006E8AC
		public static Fraction Multiply(Fraction frac1, Fraction frac2)
		{
			checked
			{
				Fraction fraction;
				try
				{
					if (frac1 != null && frac2 != null)
					{
						long num = frac1.Numerator * frac2.Numerator;
						long num2 = frac1.Denominator * frac2.Denominator;
						fraction = new Fraction(num, num2);
					}
					else
					{
						fraction = null;
					}
				}
				catch (OverflowException)
				{
					throw new FractionException("Overflow occurred while performing arithemetic operation");
				}
				catch (Exception)
				{
					throw new FractionException("An error occurred while performing arithemetic operation");
				}
				return fraction;
			}
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0000D045 File Offset: 0x0000B245
		private static long GCD(long iNo1, long iNo2)
		{
			if (iNo1 < 0L)
			{
				iNo1 = -iNo1;
			}
			if (iNo2 < 0L)
			{
				iNo2 = -iNo2;
			}
			do
			{
				if (iNo1 < iNo2)
				{
					long num = iNo1;
					iNo1 = iNo2;
					iNo2 = num;
				}
				iNo1 %= iNo2;
			}
			while (iNo1 != 0L);
			return iNo2;
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0007071C File Offset: 0x0006E91C
		public static void ReduceFraction(Fraction frac)
		{
			try
			{
				if (null != frac)
				{
					if (frac.Numerator == 0L)
					{
						frac.Denominator = 1L;
					}
					else
					{
						long num = Fraction.GCD(frac.Numerator, frac.Denominator);
						frac.Numerator /= num;
						frac.Denominator /= num;
						if (frac.Denominator < 0L)
						{
							frac.Numerator *= -1L;
							frac.Denominator *= -1L;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new FractionException("Cannot reduce Fraction: " + ex.Message);
			}
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x0000CD69 File Offset: 0x0000AF69
		public static Fraction Subtract(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Add(frac1, -frac2);
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0000CE88 File Offset: 0x0000B088
		public static Fraction Divide(Fraction frac1, Fraction frac2)
		{
			if (!(null != frac1) || !(null != frac2))
			{
				return null;
			}
			return Fraction.Multiply(frac1, Fraction.Inverse(frac2));
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x000707C4 File Offset: 0x0006E9C4
		public int CompareTo(Fraction frac)
		{
			if (frac == null)
			{
				return 1;
			}
			if (this.Numerator * this.Denominator == frac.Numerator * frac.Denominator)
			{
				return 0;
			}
			if (this.Numerator * this.Denominator < frac.Numerator * frac.Denominator)
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x04000BBD RID: 3005
		private long m_iNumerator;

		// Token: 0x04000BBE RID: 3006
		private long m_iDenominator;

		// Token: 0x04000BBF RID: 3007
		private double m_iDoubleValue;
	}
}
