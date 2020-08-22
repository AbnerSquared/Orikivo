﻿using System;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a mathematical degree.
    /// </summary>
    public struct AngleF
    {
        public const float MinValue = 0f;

        public const float MaxValue = 360f - float.Epsilon;

        public const float Up = 90f;

        public const float Down = 270f;

        public const float Left = 180f;

        public const float Right = 0f;

        public AngleF(float f)
        {
            _degrees = Wrap(f);
        }

        private float _degrees;

        public float Normal
        {
            get => RangeF.Convert(RangeF.Degree, RangeF.Percent, Degrees);
            set => Degrees = RangeF.Convert(RangeF.Percent, RangeF.Degree, value);
        }

        public float Degrees
        {
            get => _degrees;
            set => _degrees = Wrap(value);
        }

        public float Radians
        {
            get => CalcF.Radians(Degrees);
            set => Degrees = CalcF.Degrees(value);
        }

        private static float Wrap(float f)
            => f % 360.0f;

        public override bool Equals(object obj)
        {
            return obj switch
            {
                AngleF a => a == this,
                float f => f == this,
                _ => false
            };
        }

        public override int GetHashCode()
            => (int) System.MathF.Floor(Degrees * 1000.0f);

        public static bool operator ==(AngleF a, AngleF b)
            => Math.Abs(a.Degrees - b.Degrees) < 0.001f;

        public static bool operator !=(AngleF a, AngleF b)
            => Math.Abs(a.Degrees - b.Degrees) > 0.001f;

        public static AngleF operator +(AngleF a, AngleF b)
            => a.Degrees + b.Degrees;

        public static AngleF operator -(AngleF a, AngleF b)
            => a.Degrees - b.Degrees;

        public static bool operator ==(AngleF a, float b)
            => Math.Abs(a.Degrees - Wrap(b)) < 0.001f;

        public static bool operator ==(float a, AngleF b)
            => Math.Abs(b.Degrees - Wrap(a)) < 0.001f;

        public static bool operator !=(AngleF a, float b)
            => Math.Abs(a.Degrees - Wrap(b)) > 0.001f;

        public static bool operator !=(float a, AngleF b)
            => Math.Abs(b.Degrees - Wrap(a)) > 0.001f;

        public static AngleF operator +(AngleF a, float b)
            => a.Degrees + Wrap(b);

        public static AngleF operator +(float a, AngleF b)
            => b.Degrees + Wrap(a);

        public static AngleF operator -(AngleF a, float b)
            => a.Degrees - Wrap(b);

        public static AngleF operator -(float a, AngleF b)
            => Wrap(a) - b.Degrees;

        public static AngleF operator -(AngleF a)
            => -a.Degrees;

        public static AngleF operator *(AngleF a, float b)
        {
            float t = b % 1.0f;
            b = Math.Abs(t) < 0.001f ? 1.0f : t;

            return a.Degrees * b;
        }

        public static AngleF operator *(float a, AngleF b)
        {
            float t = a % 1.0f;
            a = Math.Abs(t) < 0.001f ? 1.0f : t;

            return b.Degrees * a;
        }

        public static AngleF operator /(AngleF a, float b)
        {
            float t = b % 1.0f;
            b = Math.Abs(t) < 0.001f ? 1.0f : t;

            return a.Degrees / b;
        }

        public static implicit operator float(AngleF a)
            => a.Degrees;

        public static implicit operator int(AngleF a)
            => (int) MathF.Floor(a.Degrees);

        public static implicit operator AngleF(float f)
            => new AngleF(f);
    }
}
