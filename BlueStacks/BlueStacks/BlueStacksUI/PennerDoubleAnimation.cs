using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001CA RID: 458
	public class PennerDoubleAnimation : DoubleAnimationBase
	{
		// Token: 0x0600123B RID: 4667 RVA: 0x0000D0A0 File Offset: 0x0000B2A0
		public PennerDoubleAnimation()
		{
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x0000D0A8 File Offset: 0x0000B2A8
		public PennerDoubleAnimation(PennerDoubleAnimation.Equations type, double from, double to)
		{
			this.Equation = type;
			this.From = from;
			this.To = to;
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x0000D0C5 File Offset: 0x0000B2C5
		public PennerDoubleAnimation(PennerDoubleAnimation.Equations type, double from, double to, Duration duration)
		{
			this.Equation = type;
			this.From = from;
			this.To = to;
			base.Duration = duration;
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x0007085C File Offset: 0x0006EA5C
		protected override double GetCurrentValueCore(double startValue, double targetValue, AnimationClock clock)
		{
			double num;
			try
			{
				object[] array = new object[]
				{
					(clock != null) ? new double?(clock.CurrentTime.Value.TotalSeconds) : null,
					this.From,
					this.To - this.From,
					base.Duration.TimeSpan.TotalSeconds
				};
				num = (double)this._EasingMethod.Invoke(this, array);
			}
			catch
			{
				num = this.From;
			}
			return num;
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x0000D0EA File Offset: 0x0000B2EA
		protected override Freezable CreateInstanceCore()
		{
			return new PennerDoubleAnimation();
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x0000D0F1 File Offset: 0x0000B2F1
		public static double Linear(double t, double b, double c, double d)
		{
			return c * t / d + b;
		}

		// Token: 0x06001241 RID: 4673 RVA: 0x0000D0FA File Offset: 0x0000B2FA
		public static double ExpoEaseOut(double t, double b, double c, double d)
		{
			if (t != d)
			{
				return c * (-Math.Pow(2.0, -10.0 * t / d) + 1.0) + b;
			}
			return b + c;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0000D12E File Offset: 0x0000B32E
		public static double ExpoEaseIn(double t, double b, double c, double d)
		{
			if (t != 0.0)
			{
				return c * Math.Pow(2.0, 10.0 * (t / d - 1.0)) + b;
			}
			return b;
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x00070918 File Offset: 0x0006EB18
		public static double ExpoEaseInOut(double t, double b, double c, double d)
		{
			if (t == 0.0)
			{
				return b;
			}
			if (t == d)
			{
				return b + c;
			}
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * Math.Pow(2.0, 10.0 * (t - 1.0)) + b;
			}
			return c / 2.0 * (-Math.Pow(2.0, -10.0 * (t -= 1.0)) + 2.0) + b;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x000709C8 File Offset: 0x0006EBC8
		public static double ExpoEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.ExpoEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.ExpoEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0000D167 File Offset: 0x0000B367
		public static double CircEaseOut(double t, double b, double c, double d)
		{
			return c * Math.Sqrt(1.0 - (t = t / d - 1.0) * t) + b;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0000D18E File Offset: 0x0000B38E
		public static double CircEaseIn(double t, double b, double c, double d)
		{
			return -c * (Math.Sqrt(1.0 - (t /= d) * t) - 1.0) + b;
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00070A2C File Offset: 0x0006EC2C
		public static double CircEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return -c / 2.0 * (Math.Sqrt(1.0 - t * t) - 1.0) + b;
			}
			return c / 2.0 * (Math.Sqrt(1.0 - (t -= 2.0) * t) + 1.0) + b;
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00070AB8 File Offset: 0x0006ECB8
		public static double CircEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.CircEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.CircEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0000D1B6 File Offset: 0x0000B3B6
		public static double QuadEaseOut(double t, double b, double c, double d)
		{
			return -c * (t /= d) * (t - 2.0) + b;
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0000D1CF File Offset: 0x0000B3CF
		public static double QuadEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t + b;
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x00070B1C File Offset: 0x0006ED1C
		public static double QuadEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t + b;
			}
			return -c / 2.0 * ((t -= 1.0) * (t - 2.0) - 1.0) + b;
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x00070B8C File Offset: 0x0006ED8C
		public static double QuadEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.QuadEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.QuadEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x0000D1DD File Offset: 0x0000B3DD
		public static double SineEaseOut(double t, double b, double c, double d)
		{
			return c * Math.Sin(t / d * 1.5707963267948966) + b;
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x0000D1F5 File Offset: 0x0000B3F5
		public static double SineEaseIn(double t, double b, double c, double d)
		{
			return -c * Math.Cos(t / d * 1.5707963267948966) + c + b;
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00070BF0 File Offset: 0x0006EDF0
		public static double SineEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * Math.Sin(3.141592653589793 * t / 2.0) + b;
			}
			return -c / 2.0 * (Math.Cos(3.141592653589793 * (t -= 1.0) / 2.0) - 2.0) + b;
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00070C84 File Offset: 0x0006EE84
		public static double SineEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.SineEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.SineEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0000D210 File Offset: 0x0000B410
		public static double CubicEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * t + 1.0) + b;
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x0000D234 File Offset: 0x0000B434
		public static double CubicEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t + b;
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00070CE8 File Offset: 0x0006EEE8
		public static double CubicEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * t + 2.0) + b;
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x00070D50 File Offset: 0x0006EF50
		public static double CubicEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.CubicEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.CubicEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0000D244 File Offset: 0x0000B444
		public static double QuartEaseOut(double t, double b, double c, double d)
		{
			return -c * ((t = t / d - 1.0) * t * t * t - 1.0) + b;
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x0000D26B File Offset: 0x0000B46B
		public static double QuartEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t * t + b;
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00070DB4 File Offset: 0x0006EFB4
		public static double QuartEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t * t + b;
			}
			return -c / 2.0 * ((t -= 2.0) * t * t * t - 2.0) + b;
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00070E20 File Offset: 0x0006F020
		public static double QuartEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.QuartEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.QuartEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x0000D27D File Offset: 0x0000B47D
		public static double QuintEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * t * t * t + 1.0) + b;
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x0000D2A5 File Offset: 0x0000B4A5
		public static double QuintEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * t * t * t + b;
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00070E84 File Offset: 0x0006F084
		public static double QuintEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * t * t * t * t * t + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * t * t * t + 2.0) + b;
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00070EF4 File Offset: 0x0006F0F4
		public static double QuintEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.QuintEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.QuintEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00070F58 File Offset: 0x0006F158
		public static double ElasticEaseOut(double t, double b, double c, double d)
		{
			if ((t /= d) == 1.0)
			{
				return b + c;
			}
			double num = d * 0.3;
			double num2 = num / 4.0;
			return c * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num2) * 6.283185307179586 / num) + c + b;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00070FC8 File Offset: 0x0006F1C8
		public static double ElasticEaseIn(double t, double b, double c, double d)
		{
			if ((t /= d) == 1.0)
			{
				return b + c;
			}
			double num = d * 0.3;
			double num2 = num / 4.0;
			return -(c * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.283185307179586 / num)) + b;
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x00071044 File Offset: 0x0006F244
		public static double ElasticEaseInOut(double t, double b, double c, double d)
		{
			if ((t /= d / 2.0) == 2.0)
			{
				return b + c;
			}
			double num = d * 0.44999999999999996;
			double num2 = num / 4.0;
			if (t < 1.0)
			{
				return -0.5 * (c * Math.Pow(2.0, 10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.283185307179586 / num)) + b;
			}
			return c * Math.Pow(2.0, -10.0 * (t -= 1.0)) * Math.Sin((t * d - num2) * 6.283185307179586 / num) * 0.5 + c + b;
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00071130 File Offset: 0x0006F330
		public static double ElasticEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.ElasticEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.ElasticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00071194 File Offset: 0x0006F394
		public static double BounceEaseOut(double t, double b, double c, double d)
		{
			if ((t /= d) < 0.36363636363636365)
			{
				return c * (7.5625 * t * t) + b;
			}
			if (t < 0.7272727272727273)
			{
				return c * (7.5625 * (t -= 0.5454545454545454) * t + 0.75) + b;
			}
			if (t < 0.9090909090909091)
			{
				return c * (7.5625 * (t -= 0.8181818181818182) * t + 0.9375) + b;
			}
			return c * (7.5625 * (t -= 0.9545454545454546) * t + 0.984375) + b;
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x0000D2B9 File Offset: 0x0000B4B9
		public static double BounceEaseIn(double t, double b, double c, double d)
		{
			return c - PennerDoubleAnimation.BounceEaseOut(d - t, 0.0, c, d) + b;
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00071258 File Offset: 0x0006F458
		public static double BounceEaseInOut(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.BounceEaseIn(t * 2.0, 0.0, c, d) * 0.5 + b;
			}
			return PennerDoubleAnimation.BounceEaseOut(t * 2.0 - d, 0.0, c, d) * 0.5 + c * 0.5 + b;
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x000712D0 File Offset: 0x0006F4D0
		public static double BounceEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.BounceEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.BounceEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0000D2D2 File Offset: 0x0000B4D2
		public static double BackEaseOut(double t, double b, double c, double d)
		{
			return c * ((t = t / d - 1.0) * t * (2.70158 * t + 1.70158) + 1.0) + b;
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x0000D30A File Offset: 0x0000B50A
		public static double BackEaseIn(double t, double b, double c, double d)
		{
			return c * (t /= d) * t * (2.70158 * t - 1.70158) + b;
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x00071334 File Offset: 0x0006F534
		public static double BackEaseInOut(double t, double b, double c, double d)
		{
			double num = 1.70158;
			if ((t /= d / 2.0) < 1.0)
			{
				return c / 2.0 * (t * t * (((num *= 1.525) + 1.0) * t - num)) + b;
			}
			return c / 2.0 * ((t -= 2.0) * t * (((num *= 1.525) + 1.0) * t + num) + 2.0) + b;
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x000713D8 File Offset: 0x0006F5D8
		public static double BackEaseOutIn(double t, double b, double c, double d)
		{
			if (t < d / 2.0)
			{
				return PennerDoubleAnimation.BackEaseOut(t * 2.0, b, c / 2.0, d);
			}
			return PennerDoubleAnimation.BackEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x0000D32E File Offset: 0x0000B52E
		private static void HandleEquationChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			(sender as PennerDoubleAnimation)._EasingMethod = typeof(PennerDoubleAnimation).GetMethod(e.NewValue.ToString());
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600126A RID: 4714 RVA: 0x0000D356 File Offset: 0x0000B556
		// (set) Token: 0x0600126B RID: 4715 RVA: 0x0000D368 File Offset: 0x0000B568
		[TypeConverter(typeof(PennerDoubleAnimationTypeConverter))]
		public PennerDoubleAnimation.Equations Equation
		{
			get
			{
				return (PennerDoubleAnimation.Equations)base.GetValue(PennerDoubleAnimation.EquationProperty);
			}
			set
			{
				base.SetValue(PennerDoubleAnimation.EquationProperty, value);
				this._EasingMethod = base.GetType().GetMethod(value.ToString());
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600126C RID: 4716 RVA: 0x0000D399 File Offset: 0x0000B599
		// (set) Token: 0x0600126D RID: 4717 RVA: 0x0000D3AB File Offset: 0x0000B5AB
		public double From
		{
			get
			{
				return (double)base.GetValue(PennerDoubleAnimation.FromProperty);
			}
			set
			{
				base.SetValue(PennerDoubleAnimation.FromProperty, value);
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x0600126E RID: 4718 RVA: 0x0000D3BE File Offset: 0x0000B5BE
		// (set) Token: 0x0600126F RID: 4719 RVA: 0x0000D3D0 File Offset: 0x0000B5D0
		public double To
		{
			get
			{
				return (double)base.GetValue(PennerDoubleAnimation.ToProperty);
			}
			set
			{
				base.SetValue(PennerDoubleAnimation.ToProperty, value);
			}
		}

		// Token: 0x04000BC1 RID: 3009
		private MethodInfo _EasingMethod;

		// Token: 0x04000BC2 RID: 3010
		public static readonly DependencyProperty EquationProperty = DependencyProperty.Register("Equation", typeof(PennerDoubleAnimation.Equations), typeof(PennerDoubleAnimation), new PropertyMetadata(PennerDoubleAnimation.Equations.Linear, new PropertyChangedCallback(PennerDoubleAnimation.HandleEquationChanged)));

		// Token: 0x04000BC3 RID: 3011
		public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(double), typeof(PennerDoubleAnimation), new PropertyMetadata(0.0));

		// Token: 0x04000BC4 RID: 3012
		public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(double), typeof(PennerDoubleAnimation), new PropertyMetadata(0.0));

		// Token: 0x020001CB RID: 459
		public enum Equations
		{
			// Token: 0x04000BC6 RID: 3014
			Linear,
			// Token: 0x04000BC7 RID: 3015
			QuadEaseOut,
			// Token: 0x04000BC8 RID: 3016
			QuadEaseIn,
			// Token: 0x04000BC9 RID: 3017
			QuadEaseInOut,
			// Token: 0x04000BCA RID: 3018
			QuadEaseOutIn,
			// Token: 0x04000BCB RID: 3019
			ExpoEaseOut,
			// Token: 0x04000BCC RID: 3020
			ExpoEaseIn,
			// Token: 0x04000BCD RID: 3021
			ExpoEaseInOut,
			// Token: 0x04000BCE RID: 3022
			ExpoEaseOutIn,
			// Token: 0x04000BCF RID: 3023
			CubicEaseOut,
			// Token: 0x04000BD0 RID: 3024
			CubicEaseIn,
			// Token: 0x04000BD1 RID: 3025
			CubicEaseInOut,
			// Token: 0x04000BD2 RID: 3026
			CubicEaseOutIn,
			// Token: 0x04000BD3 RID: 3027
			QuartEaseOut,
			// Token: 0x04000BD4 RID: 3028
			QuartEaseIn,
			// Token: 0x04000BD5 RID: 3029
			QuartEaseInOut,
			// Token: 0x04000BD6 RID: 3030
			QuartEaseOutIn,
			// Token: 0x04000BD7 RID: 3031
			QuintEaseOut,
			// Token: 0x04000BD8 RID: 3032
			QuintEaseIn,
			// Token: 0x04000BD9 RID: 3033
			QuintEaseInOut,
			// Token: 0x04000BDA RID: 3034
			QuintEaseOutIn,
			// Token: 0x04000BDB RID: 3035
			CircEaseOut,
			// Token: 0x04000BDC RID: 3036
			CircEaseIn,
			// Token: 0x04000BDD RID: 3037
			CircEaseInOut,
			// Token: 0x04000BDE RID: 3038
			CircEaseOutIn,
			// Token: 0x04000BDF RID: 3039
			SineEaseOut,
			// Token: 0x04000BE0 RID: 3040
			SineEaseIn,
			// Token: 0x04000BE1 RID: 3041
			SineEaseInOut,
			// Token: 0x04000BE2 RID: 3042
			SineEaseOutIn,
			// Token: 0x04000BE3 RID: 3043
			ElasticEaseOut,
			// Token: 0x04000BE4 RID: 3044
			ElasticEaseIn,
			// Token: 0x04000BE5 RID: 3045
			ElasticEaseInOut,
			// Token: 0x04000BE6 RID: 3046
			ElasticEaseOutIn,
			// Token: 0x04000BE7 RID: 3047
			BounceEaseOut,
			// Token: 0x04000BE8 RID: 3048
			BounceEaseIn,
			// Token: 0x04000BE9 RID: 3049
			BounceEaseInOut,
			// Token: 0x04000BEA RID: 3050
			BounceEaseOutIn,
			// Token: 0x04000BEB RID: 3051
			BackEaseOut,
			// Token: 0x04000BEC RID: 3052
			BackEaseIn,
			// Token: 0x04000BED RID: 3053
			BackEaseInOut,
			// Token: 0x04000BEE RID: 3054
			BackEaseOutIn
		}
	}
}
