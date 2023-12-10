namespace AoC18.Common
{
    public class Coord3DL : IEquatable<Coord3DL>
    {
        public enum Arrangement
        { 
            UpDownLeftRight = 0,
            UpRightDownLeft = 1
        }

        public long x = 0; 
        public long y = 0;
        public long z = 0;


        public Coord3DL(long x, long y, long z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Coord3DL operator +(Coord3DL coord_a, Coord3DL coord_b)
            => new Coord3DL(coord_a.x + coord_b.x, coord_a.y + coord_b.y, coord_a.z + coord_b.z);

        public static Coord3DL operator -(Coord3DL coord_a, Coord3DL coord_b)
            => new Coord3DL(coord_a.x - coord_b.x, coord_a.y - coord_b.y, coord_a.z - coord_b.z);

        public static Coord3DL operator *(Coord3DL coord, long scalar)
            => new Coord3DL(coord.x * scalar, coord.y * scalar, coord.z * scalar);

        public static Coord3DL operator *(long scalar, Coord3DL coord)
            => new Coord3DL(coord.x * scalar, coord.y * scalar, coord.z * scalar);
        public static Coord3DL operator /(Coord3DL coord, long scalar)
            => new Coord3DL(coord.x / scalar, coord.y / scalar, coord.z/scalar);

        public static bool operator ==(Coord3DL coord_a, Coord3DL coord_b)
           => coord_a.Equals(coord_b);

        public static bool operator !=(Coord3DL coord_a, Coord3DL coord_b)
           => !coord_a.Equals(coord_b);

        public void Deconstruct(out long x, out long y, out long z)
        {
            x = this.x;
            y = this.y;
            z = this.z;
        }

        public bool Equals(Coord3DL? other)
            => other is null ? false : other.x == x && other.y == y && other.z == z;

        public override bool Equals(object? other) 
            => other is Coord3DL c && c.x.Equals(x)  && c.y.Equals(y) && c.z.Equals(z);

        public static implicit operator (long, long, long)(Coord3DL c)       // Cast bw Coord and tuple
            => (c.x, c.y, c.z);

        public static implicit operator Coord3DL((long X, long Y, long Z) c) 
            => new Coord3DL(c.X, c.Y, c.Z);

        public long Manhattan(Coord3DL other)
            => Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z);

        public double VectorModule
            => Math.Sqrt(x * x + y * y + z * z);

        public override int GetHashCode()
        {
            unchecked // Wraps around max value
            {
                int hash = 17;
                hash = hash * 23 + (int)x;
                hash = hash * 23 + (int)y;
                hash = hash * 23 + (int)z;
                return hash;
            }
        }
    }
}
