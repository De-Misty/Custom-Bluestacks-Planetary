using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200017E RID: 382
	public static class Animator
	{
		// Token: 0x06000F55 RID: 3925 RVA: 0x000614A0 File Offset: 0x0005F6A0
		public static AnimationClock AnimatePenner(DependencyObject element, DependencyProperty prop, PennerDoubleAnimation.Equations type, double to, int durationMS, EventHandler callbackFunc)
		{
			return Animator.AnimatePenner(element, prop, type, null, to, durationMS, callbackFunc);
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x000614C4 File Offset: 0x0005F6C4
		public static AnimationClock AnimatePenner(DependencyObject element, DependencyProperty prop, PennerDoubleAnimation.Equations type, double? from, double to, int durationMS, EventHandler callbackFunc)
		{
			double num = (double.IsNaN((double)((element != null) ? element.GetValue(prop) : null)) ? 0.0 : ((double)element.GetValue(prop)));
			PennerDoubleAnimation pennerDoubleAnimation = new PennerDoubleAnimation(type, from.GetValueOrDefault(num), to);
			return Animator.Animate(element, prop, pennerDoubleAnimation, durationMS, null, null, callbackFunc);
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x00061534 File Offset: 0x0005F734
		public static AnimationClock AnimateDouble(DependencyObject element, DependencyProperty prop, double? from, double to, int durationMS, double? accel, double? decel, EventHandler callbackFunc)
		{
			double num = (double.IsNaN((double)((element != null) ? element.GetValue(prop) : null)) ? 0.0 : ((double)element.GetValue(prop)));
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(from.GetValueOrDefault(num)),
				To = new double?(to)
			};
			return Animator.Animate(element, prop, doubleAnimation, durationMS, accel, decel, callbackFunc);
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x0000B32F File Offset: 0x0000952F
		public static void ClearAnimation(DependencyObject animatable, DependencyProperty property)
		{
			if (animatable != null)
			{
				animatable.SetValue(property, animatable.GetValue(property));
				((IAnimatable)animatable).ApplyAnimationClock(property, null);
			}
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x000615A8 File Offset: 0x0005F7A8
		private static AnimationClock Animate(DependencyObject animatable, DependencyProperty prop, AnimationTimeline anim, int duration, double? accel, double? decel, EventHandler func)
		{
			Animator.<>c__DisplayClass4_0 CS$<>8__locals1 = new Animator.<>c__DisplayClass4_0();
			CS$<>8__locals1.animatable = animatable;
			CS$<>8__locals1.prop = prop;
			anim.AccelerationRatio = accel.GetValueOrDefault(0.0);
			anim.DecelerationRatio = decel.GetValueOrDefault(0.0);
			anim.Duration = TimeSpan.FromMilliseconds((double)duration);
			anim.Freeze();
			CS$<>8__locals1.animClock = anim.CreateClock();
			CS$<>8__locals1.animClock.Completed += CS$<>8__locals1.<Animate>g__animClock_Completed|0;
			if (func != null)
			{
				CS$<>8__locals1.animClock.Completed += func;
			}
			CS$<>8__locals1.animClock.Controller.Begin();
			Animator.ClearAnimation(CS$<>8__locals1.animatable, CS$<>8__locals1.prop);
			((IAnimatable)CS$<>8__locals1.animatable).ApplyAnimationClock(CS$<>8__locals1.prop, CS$<>8__locals1.animClock);
			return CS$<>8__locals1.animClock;
		}
	}
}
