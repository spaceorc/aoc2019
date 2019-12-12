using System;

namespace aoc
{
    public struct V : IEquatable<V>
    {
        public readonly int X;
        public readonly int Y;

        public V(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(V other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is V other && Equals(other);
        public override int GetHashCode() => unchecked((X * 397) ^ Y);
        public static bool operator ==(V a, V b) => a.Equals(b);
        public static bool operator !=(V a, V b) => !(a == b);
        public static V operator +(V a, V b) => new V(a.X + b.X, a.Y + b.Y);
        public static V operator -(V a, V b) => new V(a.X - b.X, a.Y - b.Y);

        public int MLen() => Math.Abs(X) + Math.Abs(Y);
        public static int XProd(V a, V b) => a.X * b.Y - a.Y * b.X;
        public static int DProd(V a, V b) => a.X * b.X + a.Y * b.Y;

        public override string ToString() => $"{X} {Y}";
    }
}