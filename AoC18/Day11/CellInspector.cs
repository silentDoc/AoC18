using AoC18.Common;
using MoreLinq;

namespace AoC18.Day11
{
    internal class CellInspector
    {
        int serial;

        Dictionary<(int x, int y, int size), int> allSizeSquares = new();

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

        string Find3x3Cell(Dictionary<Coord2D, int> cells)
        {
            Dictionary<Coord2D, int> squares = new();
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

        int CalcSquare(Dictionary<Coord2D, int> cells, Coord2D pos, int size)
        {
            var key = (pos.x, pos.y, size);

            if (size ==1)
            {
                allSizeSquares[key] = cells[pos];
                return cells[pos];
            }

            // Calc a new rect
            var previousKey = (pos.x, pos.y, size-1);
            var previousVal = allSizeSquares[previousKey];
            var offset = size - 1;
            var sum1 = Enumerable.Range(pos.x, size).Sum(x => cells[(x, pos.y + offset)]);
            var sum2 = Enumerable.Range(pos.y, size-1).Sum(x => cells[(pos.x+offset, x)]);
            allSizeSquares[key] = previousVal + sum1 + sum2;
            return allSizeSquares[key];
        }

        string FindAnySize(Dictionary<Coord2D, int> cells)
        {
            var maxVal = 0;
            var maxPos = (0, 0, 1);

            for (var x = 1; x <= 300; x++)
                for (var y = 1; y <= 300; y++)
                {
                    int maxLength = Math.Min(300 - x, 300 - y); // Optimize, every square contains the previous !!!
                    for (var side = 1; side <= maxLength; side++)
                    {
                        var result = CalcSquare(cells, (x, y), side);
                        if (result > maxVal)
                        {
                            maxVal = result;
                            maxPos = (x, y, side);
                        }
                    }
                }
            return maxPos.Item1.ToString() + "," + maxPos.Item2.ToString() + "," + maxPos.Item3.ToString();
        }

        string FindSquare(int part = 1)
        {
            Dictionary<Coord2D, int> cells = new();
            

            for(var x = 1; x<=300; x++)
                for (var y = 1; y <= 300; y++)
                {
                    var pos = new Coord2D(x, y);
                    cells[pos] = CalculateValue(pos, serial);
                }

            return part == 1 ? Find3x3Cell(cells) : FindAnySize(cells);
        }

        public string Solve(int part = 1)
            => FindSquare(part);
    }
}
