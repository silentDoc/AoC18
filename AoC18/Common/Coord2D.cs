namespace AoC18.Common
{
    public class Coord2D : IEquatable<Coord2D>
    {
        public enum Arrangement
        { 
            UpDownLeftRight = 0,
            UpRightDownLeft = 1
        }

        public int x = 0; 
        public int y = 0;

        public Coord2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Coord2D operator +(Coord2D coord_a, Coord2D coord_b)
            => new Coord2D(coord_a.x + coord_b.x, coord_a.y + coord_b.y);

        public static Coord2D operator -(Coord2D coord_a, Coord2D coord_b)
            => new Coord2D(coord_a.x - coord_b.x, coord_a.y - coord_b.y);

        public static Coord2D operator *(Coord2D coord, int scalar)
            => new Coord2D(coord.x * scalar, coord.y * scalar);

        public static Coord2D operator *(int scalar, Coord2D coord)
            => new Coord2D(coord.x * scalar, coord.y * scalar);
        public static Coord2D operator /(Coord2D coord, int scalar)
            => new Coord2D(coord.x / scalar, coord.y / scalar);

        public static bool operator ==(Coord2D coord_a, Coord2D coord_b)
           => coord_a.Equals(coord_b);

        public static bool operator !=(Coord2D coord_a, Coord2D coord_b)
           => !coord_a.Equals(coord_b);

        public void Deconstruct(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }

        public bool Equals(Coord2D? other)
            => other is null ? false : other.x == x && other.y == y;

        public override bool Equals(object? other) 
            => other is Coord2D c && c.x.Equals(x)  && c.y.Equals(y);

        public static implicit operator (int, int)(Coord2D c)       // Cast bw Coord and tuple
            => (c.x, c.y);

        public static implicit operator Coord2D((int X, int Y) c) 
            => new Coord2D(c.X, c.Y);

        public int Manhattan(Coord2D other)
            => Math.Abs(x - other.x) + Math.Abs(y - other.y);

        public double VectorModule
            => Math.Sqrt( x*x + y*y );

        public IEnumerable<Coord2D> GetNeighbors(Arrangement arrange = Arrangement.UpRightDownLeft)
        {
            if (arrange == Arrangement.UpRightDownLeft)
            {
                // Up - Right - Down - Left
                yield return new Coord2D(x, y - 1);
                yield return new Coord2D(x + 1, y);
                yield return new Coord2D(x, y + 1);
                yield return new Coord2D(x - 1, y);
            }
            else
            {
                yield return new Coord2D(x, y - 1);
                yield return new Coord2D(x, y + 1);
                yield return new Coord2D(x - 1, y);
                yield return new Coord2D(x + 1, y);
            }
        }

        public IEnumerable<Coord2D> GetNeighbors8()
        {
            yield return new Coord2D(x + 1, y);
            yield return new Coord2D(x + 1, y - 1);
            yield return new Coord2D(x, y - 1);
            yield return new Coord2D(x - 1, y - 1);
            yield return new Coord2D(x - 1, y);
            yield return new Coord2D(x - 1, y + 1);
            yield return new Coord2D(x, y + 1);
            yield return new Coord2D(x + 1, y + 1);
        }

        public override int GetHashCode()
        {
            unchecked // Wraps around max value
            {
                int hash = 17;
                hash = hash * 23 + x;
                hash = hash * 23 + y;
                return hash;
            }
        }

        public override string ToString()
        {
            return x.ToString() + "," + y.ToString();
        }


    }
}
