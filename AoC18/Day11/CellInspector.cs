using AoC18.Common;
using MoreLinq;
using System.Drawing;

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

        string Find3x3Cell(Dictionary<(int x, int y), int> cells)
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
            Coord2D posCenter = squares.Keys.First(x => squares[x] == maxPowerLevel);
            posCenter -= new Coord2D(1, 1); // We have the square center, we want the top left

            return posCenter.ToString();
        }

        int CalcSquare(Dictionary<(int x, int y), int> cells, Coord2D pos, int size)
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

        string FindAnySize(Dictionary<(int x, int y), int> cells)
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

        string FindAnySizeOpt(Dictionary<(int x, int y), int> cells)
        {
            var maxVal = 0;
            var maxPos = (0, 0, 1);
            var previous = 0;

            for (var x = 1; x <= 300; x++)
                for (var y = 1; y <= 300; y++)
                {
                    int maxLength = Math.Min(300 - x, 300 - y); 
                    for (var side = 1; side <= maxLength; side++)
                    {
                        var result = 0;
                        if (side == 1)
                            result = cells[(x, y)];
                        else
                        {
                            result += previous;
                            var offset = side - 1;
                            result += Enumerable.Range(x, side).Sum(p => cells[(p, y + offset)]);
                            result += Enumerable.Range(y, side - 1).Sum(p => cells[(x + offset, p)]);
                        }
                        previous = result;

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
            Dictionary<(int x, int y), int> cells = new();
            

            for(var x = 1; x<=300; x++)
                for (var y = 1; y <= 300; y++)
                {
                    var pos = (x, y);
                    cells[pos] = CalculateValue(pos, serial);
                }

            return part == 1 ? Find3x3Cell(cells) : FindAnySizeOpt(cells);
        }

        public string Solve(int part = 1)
            => FindSquare(part);
    }
}
