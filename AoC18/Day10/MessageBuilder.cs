using AoC18.Common;
using System.Text;

namespace AoC18.Day10
{
    class Pixel
    {
        public Coord2D Position;
        public Coord2D Velocity;

        public Pixel(int px, int py, int vx, int vy)
        {
            Position = new Coord2D(px, py);
            Velocity = new Coord2D(vx, vy);
        }

        public void Update(int numSeconds = 1)
            => Position += (Velocity * numSeconds);
    }

    internal class MessageBuilder
    {
        List<Pixel> pixels = new();

        void ParseLine(string line)
        {
            //position=< 9,  1> velocity=< 0,  2>
            var data = line.Replace("position=<", "").Replace("> velocity=<",";").Replace(">", "").Split(";");
            
            var pos_xy = data[0].Split(",")
                                .Select(x => int.Parse(x.Trim()))
                                .ToList();

            var vel_xy = data[1].Split(",")
                                .Select(x => int.Parse(x.Trim()))
                                .ToList();

            pixels.Add(new Pixel(pos_xy[0], pos_xy[1], vel_xy[0], vel_xy[1]));
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        (Coord2D topLeft, Coord2D bottomRight) FindWindow()
        {
            int min_x = pixels.Select(p => p.Position.x).Min();
            int max_x = pixels.Select(p => p.Position.x).Max();
            int min_y = pixels.Select(p => p.Position.y).Min();
            int max_y = pixels.Select(p => p.Position.y).Max();

            return (new Coord2D(min_x, min_y), new Coord2D(max_x, max_y) );
        }

        void RenderSky()
        {
            var (tl, br) = FindWindow();

            int width = br.x - tl.x+1;
            int height = br.y - tl.y+1;

            Console.Clear();
            int nRows = Math.Min(height, 40);
            int nCols = Math.Min(width, 250);

            List<string> lines = new List<string>();
            StringBuilder sb = new();
            for (int row = 0; row < nRows; row++)
            {
                sb.Clear();
                sb.Append(Enumerable.Repeat('.', nCols).ToArray());
                var pixelsInRow = pixels.Where(p => p.Position.y == tl.y + row).ToList();
                foreach (var p in pixelsInRow)
                {
                    var pixelX = p.Position.x - tl.x;
                    if (pixelX < nCols && pixelX>=0)
                        sb[pixelX] = '#';
                }
                lines.Add(sb.ToString());
            }
            lines.ForEach(Console.WriteLine);
        }

        void FastForward(int numSeconds)
            => pixels.ForEach(x => x.Update(numSeconds));

        string MoveMessage(int part = 1)
        {
            FastForward(9980);
            long area = long.MaxValue;
            bool keepLooking = true;
            while (keepLooking)
            {
                pixels.ForEach(x => x.Update());
                var (tl, br) = FindWindow();

                long tmp = Math.Abs( ((long)(br.x - tl.x)) * ((long)(br.y - tl.y)));
                if (tmp < area)
                    area = tmp;
                else
                    keepLooking = false;
            }
            pixels.ForEach(x => x.Update(-1));
            RenderSky();

            return "";
        }

        public string Solve(int part = 1)
            => MoveMessage(part);
    }
}
