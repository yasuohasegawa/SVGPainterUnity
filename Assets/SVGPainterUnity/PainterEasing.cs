using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVGPainterUnity{
	public static class PainterEasing {
		public static float Linear(float t, float b, float c, float d){
			return c * t / d + b;
		}
			
		public static float EaseOutExpo(float t, float b, float c, float d){
			return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
		}
			
		public static float EaseInExpo(float t, float b, float c, float d){
			return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
		}
			
		public static float EaseInOutExpo(float t, float b, float c, float d){
			if (t == 0)
				return b;

			if (t == d)
				return b + c;

			if ((t /= d / 2) < 1)
				return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;

			return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
		}
			
		public static float EaseOutCirc(float t, float b, float c, float d){
			return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
		}

		public static float EaseInCirc(float t, float b, float c, float d){
			return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
		}

		public static float EaseInOutCirc(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;

			return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
		}

		public static float EaseOutQuad(float t, float b, float c, float d){
			return -c * (t /= d) * (t - 2) + b;
		}

		public static float EaseInQuad(float t, float b, float c, float d){
			return c * (t /= d) * t + b;
		}

		public static float EaseInOutQuad(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return c / 2 * t * t + b;

			return -c / 2 * ((--t) * (t - 2) - 1) + b;
		}

		public static float EaseOutSine(float t, float b, float c, float d){
			return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
		}

		public static float EaseInSine(float t, float b, float c, float d){
			return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
		}

		public static float EaseInOutSine(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return c / 2 * (Mathf.Sin(Mathf.PI * t / 2)) + b;

			return -c / 2 * (Mathf.Cos(Mathf.PI * --t / 2) - 2) + b;
		}

		public static float EaseOutCubic(float t, float b, float c, float d){
			return c * ((t = t / d - 1) * t * t + 1) + b;
		}

		public static float EaseInCubic(float t, float b, float c, float d){
			return c * (t /= d) * t * t + b;
		}

		public static float EaseInOutCubic(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return c / 2 * t * t * t + b;

			return c / 2 * ((t -= 2) * t * t + 2) + b;
		}

		public static float EaseOutQuart(float t, float b, float c, float d){
			return -c * ((t = t / d - 1) * t * t * t - 1) + b;
		}

		public static float EaseInQuart(float t, float b, float c, float d){
			return c * (t /= d) * t * t * t + b;
		}

		public static float EaseInOutQuart(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return c / 2 * t * t * t * t + b;

			return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
		}

		public static float EaseOutQuint(float t, float b, float c, float d){
			return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
		}

		public static float EaseInQuint(float t, float b, float c, float d){
			return c * (t /= d) * t * t * t * t + b;
		}

		public static float EaseInOutQuint(float t, float b, float c, float d){
			if ((t /= d / 2) < 1)
				return c / 2 * t * t * t * t * t + b;
			return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
		}

		public static float EaseOutElastic(float t, float b, float c, float d){
			if ((t /= d) == 1)
				return b + c;

			float p = d * .3f;
			float s = p / 4;

			return (c * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b);
		}

		public static float EaseInElastic(float t, float b, float c, float d){
			if ((t /= d) == 1)
				return b + c;

			float p = d * .3f;
			float s = p / 4;

			return -(c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
		}

		public static float EaseInOutElastic(float t, float b, float c, float d){
			if ((t /= d / 2) == 2)
				return b + c;

			float p = d * (.3f * 1.5f);
			float s = p / 4;

			if (t < 1)
				return -.5f * (c * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
			return c * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + c + b;
		}

		public static float EaseOutBounce(float t, float b, float c, float d){
			if ((t /= d) < (1f / 2.75f))
				return c * (7.5625f * t * t) + b;
			else if (t < (2f / 2.75f))
				return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
			else if (t < (2.5f / 2.75f))
				return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
			else
				return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
		}

		public static float EaseInBounce(float t, float b, float c, float d){
			return c - EaseOutBounce(d - t, 0, c, d) + b;
		}

		public static float EaseInOutBounce(float t, float b, float c, float d){
			if (t < d / 2)
				return EaseInBounce(t * 2, 0, c, d) * .5f + b;
			else
				return EaseOutBounce(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
		}

		public static float EaseOutBack(float t, float b, float c, float d){
			return c * ((t = t / d - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1) + b;
		}

		public static float EaseInBack(float t, float b, float c, float d){
			return c * (t /= d) * t * ((1.70158f + 1) * t - 1.70158f) + b;
		}

		public static float EaseInOutBack(float t, float b, float c, float d){
			float s = 1.70158f;
			if ((t /= d / 2) < 1)
				return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
			return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
		}
	}
}