using AoC18.Common;

namespace AoC18.Day11
{
    internal class CellInspector
    {
        int serial;

        public void ParseInput(List<string> lines)
            => serial = int.Parse(lines[0].Trim());

        int CalculateValue(Coord2D pos, int serial)
        {
            var rackId = pos.x + 10;
            int power = rackId * pos.y;
            power += serial;
            power *= rackId;
            power = (power / 100) % 10;
            return power - 5;
        }

        string FindSquare(int part = 1)
        {
            Dictionary<Coord2D, int> cells = new();
            Dictionary<Coord2D, int> squares = new();

            for(var x = 1; x<=300; x++)
                for (var y = 1; y <= 300; y++)
                {
                    var pos = new Coord2D(x, y);
                    cells[pos] = CalculateValue(pos, serial);
                }

            for (var x = 2; x <= 299; x++)
                for (var y = 2; y <= 299; y++)
                {
                    var center = new Coord2D(x, y);
                    var neighbors = center.GetNeighbors8();
                    var powerLevel = neighbors.Sum(x => cells[x]) + cells[center];
                    squares[center] = powerLevel;
                }

            var maxPowerLevel = squares.Values.Max();
            var posCenter = squares.Keys.First(x => squares[x] == maxPowerLevel);
            posCenter -= new Coord2D(1, 1); // We have the square center, we want the top left

            return posCenter.ToString();
        }

        public string Solve(int part = 1)
            => FindSquare(part);
    }
}
