﻿using System;
using Alex.Blocks.State;
using Microsoft.Xna.Framework;

namespace Alex.Utils
{
	public static class MathUtils
	{
		public static float ConstrainAngle(float targetAngle, float centreAngle, float maximumDifference)
		{
			return centreAngle + Clamp(NormDeg(targetAngle - centreAngle), -maximumDifference, maximumDifference);
		}

		// normalizes a double degrees angle to between +180 and -180
		public static float NormDeg(float a)
		{
			a %= 360f;
			if (a >= 180f)
			{
				a -= 360f;
			}
			if (a < -180)
			{
				a += 360f;
			}
			return a;
		}

		// numeric double clamp
		public static float Clamp(float value, float min, float max)
		{
			return (value < min ? min : (value > max ? max : value));
		}

		public static float ToRadians(float deg)
		{
			return (float)(Math.PI* deg) / 180F;
		}

		public static float RadianToDegree(float angle)
		{
			return (float)(angle * (180.0f / Math.PI));
		}

		public static float AngleToDegree(sbyte angle)
		{
			return (angle / 256f) * 360f;
		}

		public static float AngleToNotchianDegree(sbyte angle)
		{
			return AngleToDegree(angle);
		}

		public static sbyte DegreeToAngle(float angle)
		{
			return (sbyte)(((angle % 360) / 360) * 256);
		}

		public static float FromFixedPoint(int f)
		{
			return ((float) f) / (32.0f * 128.0f);
		}

		public static uint Hash(uint value)
		{
			unchecked
			{
				value ^= value >> 16;
				value *= 0x85ebca6b;
				value ^= value >> 13;
				value *= 0xc2b2ae35;
				value ^= value >> 16;
			}

			return value;
		}

		public static int HsvToRGB(float hue, float saturation, float value)
		{
			int i = (int)(hue * 6.0F) % 6;
			float f = hue * 6.0F - (float)i;
			float f1 = value * (1.0F - saturation);
			float f2 = value * (1.0F - f * saturation);
			float f3 = value * (1.0F - (1.0F - f) * saturation);
			float f4;
			float f5;
			float f6;

			switch (i)
			{
				case 0:
					f4 = value;
					f5 = f3;
					f6 = f1;
					break;
				case 1:
					f4 = f2;
					f5 = value;
					f6 = f1;
					break;
				case 2:
					f4 = f1;
					f5 = value;
					f6 = f3;
					break;
				case 3:
					f4 = f1;
					f5 = f2;
					f6 = value;
					break;
				case 4:
					f4 = f3;
					f5 = f1;
					f6 = value;
					break;
				case 5:
					f4 = value;
					f5 = f1;
					f6 = f2;
					break;
				default:
					throw new Exception("Something went wrong when converting from HSV to RGB. Input was " + hue + ", " + saturation + ", " + value);
			}

			int j = MathHelper.Clamp((int)(f4 * 255.0F), 0, 255);
			int k = MathHelper.Clamp((int)(f5 * 255.0F), 0, 255);
			int l = MathHelper.Clamp((int)(f6 * 255.0F), 0, 255);
			return j << 16 | k << 8 | l;
		}
	}
}
