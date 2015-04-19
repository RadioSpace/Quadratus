using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using System.Runtime.InteropServices;

namespace Quadrum
{


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct SVector2 : IEquatable<SVector2>, IEquatable<Vector2>
    {
        float x;
        public float X { get { return x; } }

        float y;
        public float Y { get { return y; } }

        public SVector2(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public static implicit operator Vector2(SVector2 sv2)
        {
            return new Vector2(sv2.X, sv2.Y);
        }

        public static explicit operator SVector2(Vector2 v2)
        {
            return new SVector2(v2.X, v2.Y);
        }


        public static SVector2 operator +(SVector2 sv2A, SVector2 sv2B)
        {
            return new SVector2(sv2A.X + sv2B.X, sv2A.Y + sv2B.Y);
        }

        public static SVector2 operator -(SVector2 sv2A, SVector2 sv2B)
        {
            return new SVector2(sv2A.X - sv2B.X, sv2A.Y - sv2B.Y);
        }

        public static float operator *(SVector2 sv2A, SVector2 sv2B)
        {
            return (sv2A.X * sv2B.X) + (sv2A.Y * sv2B.Y);
        }

        public static SVector2 operator +(SVector2 sv2, Vector2 v2)
        {
            return new SVector2(sv2.X + v2.X, sv2.Y + v2.Y);
        }

        public static SVector2 operator -(SVector2 sv2, Vector2 v2)
        {
            return new SVector2(sv2.X - v2.X, sv2.Y - v2.Y);
        }

        public static float operator *(SVector2 sv2, Vector2 v2)
        {
            return (sv2.X * v2.X) + (sv2.Y * v2.Y);
        }

        public static SVector2 operator *(SVector2 sv2A, float sc)
        {
            return new SVector2(sv2A.X * sc, sv2A.Y * sc);
        }

        public static SVector2 operator *(SVector2 sv2A, SharpDX.Matrix m)
        {
            return new SVector2(
                (sv2A.X * m.Column1.X) + (sv2A.X * m.Column2.X) + (sv2A.X * m.Column3.X) + (sv2A.X * m.Column4.X),
                (sv2A.Y * m.Column1.Y) + (sv2A.Y * m.Column2.Y) + (sv2A.Y * m.Column3.Y) + (sv2A.Y * m.Column4.Y)
                );
        }

        public static bool operator ==(SVector2 sv2A, SVector2 sv2B)
        {
            return sv2A.Equals(sv2B);
        }

        public static bool operator !=(SVector2 sv2A, SVector2 sv2B)
        {
            return !sv2A.Equals(sv2B);
        }

        public static bool operator ==(SVector2 sv2, Vector2 v2)
        {
            return sv2.Equals(v2);
        }

        public static bool operator !=(SVector2 sv2, Vector2 v2)
        {
            return !sv2.Equals(v2);
        }



        public bool Equals(SVector2 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public bool Equals(Vector2 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            int hash = 149;
            hash = hash * 43 + x.GetHashCode();
            hash = hash * 43 + y.GetHashCode();

            return hash;

        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (obj is SVector2) return Equals((SVector2)obj);
            else if (obj is Vector2) return Equals((Vector2)obj);
            else return false;
        }

        public override string ToString()
        {
            return "x:" + x + "|y:" + y;
        }


    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    public struct SVector3 : IEquatable<SVector3>, IEquatable<Vector3>
    {
        float x;
        public float X { get { return x; } }

        float y;
        public float Y { get { return y; } }

        float z;
        public float Z { get { return z; } }

        public SVector3(float _x, float _y, float _z)
        {

            x = _x;
            y = _y;
            z = _z;

        }

        public static implicit operator Vector3(SVector3 sv3)
        {
            return new Vector3(sv3.X, sv3.Y, sv3.Z);
        }

        public static explicit operator SVector3(Vector3 v3)
        {
            return new SVector3(v3.X, v3.Y, v3.Z);
        }


        public static SVector3 operator +(SVector3 sv3A, SVector3 sv3B)
        {
            return new SVector3(sv3A.X + sv3B.X, sv3A.Y + sv3B.Y, sv3A.Z + sv3B.Z);
        }

        public static SVector3 operator -(SVector3 sv3A, SVector3 sv3B)
        {
            return new SVector3(sv3A.X - sv3B.X, sv3A.Y - sv3B.Y, sv3A.Z + sv3B.Z);
        }

        public static float operator *(SVector3 sv3A, SVector3 sv3B)
        {
            return (sv3A.X * sv3B.X) + (sv3A.Y * sv3B.Y) + (sv3A.Z + sv3B.Z);
        }

        public static SVector3 operator +(SVector3 sv3, Vector3 v3)
        {
            return new SVector3(sv3.X + v3.X, sv3.Y + v3.Y, sv3.Z + v3.Z);
        }

        public static SVector3 operator -(SVector3 sv3, Vector3 v3)
        {
            return new SVector3(sv3.X - v3.X, sv3.Y - v3.Y, sv3.Z - v3.Z);
        }

        public static float operator *(SVector3 sv3, Vector3 v3)
        {
            return (sv3.X * v3.X) + (sv3.Y * v3.Y) + (sv3.Z * v3.Z);
        }

        public static SVector3 operator *(SVector3 sv3A, float sc)
        {
            return new SVector3(sv3A.X * sc, sv3A.Y * sc, sv3A.Z * sc);
        }

        public static SVector3 operator *(SVector3 sv3A, SharpDX.Matrix m)
        {
            return new SVector3(
                (sv3A.X * m.Column1.X) + (sv3A.X * m.Column2.X) + (sv3A.X * m.Column3.X) + (sv3A.X * m.Column4.X),
                (sv3A.Y * m.Column1.Y) + (sv3A.Y * m.Column2.Y) + (sv3A.Y * m.Column3.Y) + (sv3A.Y * m.Column4.Y),
                (sv3A.Z * m.Column1.Z) + (sv3A.Z * m.Column2.Z) + (sv3A.Z * m.Column3.Z) + (sv3A.Z * m.Column4.Z)
                );
        }

        public static bool operator ==(SVector3 sv3A, SVector3 sv3B)
        {
            return sv3A.Equals(sv3B);
        }

        public static bool operator !=(SVector3 sv3A, SVector3 sv3B)
        {
            return !sv3A.Equals(sv3B);
        }

        public static bool operator ==(SVector3 sv3, Vector3 v3)
        {
            return sv3.Equals(v3);
        }

        public static bool operator !=(SVector3 sv3, Vector3 v3)
        {
            return !sv3.Equals(v3);
        }



        public bool Equals(SVector3 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public bool Equals(Vector3 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            int hash = 149;
            hash = hash * 43 + x.GetHashCode();
            hash = hash * 43 + y.GetHashCode();
            hash = hash * 43 + z.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (obj is SVector3) return Equals((SVector3)obj);
            else if (obj is Vector3) return Equals((Vector3)obj);
            else return false;
        }

        public override string ToString()
        {
            return "x:" + x + "|y:" + y + "|z:" + z;
        }

    }
}
