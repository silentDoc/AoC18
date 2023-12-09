using AoC18.Common;
using System.Drawing;

namespace AoC18.Day22
{
    class MazeRiskAnalyzer
    {
        Dictionary<Coord2D, int> GeoIndex  = new();
        Dictionary<Coord2D, int> ErosionLevels = new();
        Dictionary<Coord2D, int> Types = new();
        int Depth = 0;
        Coord2D Target = new(0, 0);
        Coord2D up = new Coord2D(0, -1);
        Coord2D left = new Coord2D(-1, 0);

        public void ParseInput(List<string> lines)
        {
            Depth = int.Parse(lines[0].Replace("depth:", ""));
            var target = lines[1].Replace("target:", "").Split(',').Select(x => int.Parse(x)).ToList();
            Target = new Coord2D(target[0], target[1]);
        }

        int CellRisk(Coord2D coord)
        {
            var retVal = coord switch
            {
                (0, 0) => 0,
                (_, 0) => coord.x * 16807,
                (0, _) => coord.y * 48271,
                (_, _) => ErosionLevels[coord + up] * ErosionLevels[coord + left]
            };
            return retVal;
        }

        int Modulo(int a, int b)
            => (a % b + b) % b;

        int SolveMaze(int part =1)
        {
            for (int i = 0; i <= Target.x; i++)
                for (int j = 0; j <= Target.y; j++)
                {
                    GeoIndex[(i, j)] = CellRisk((i, j));
                    ErosionLevels[(i, j)] = Modulo(GeoIndex[(i, j)] + Depth, 20183);
                    Types[(i, j)] = ErosionLevels[(i, j)] % 3;
                }

            GeoIndex[Target] = 0;
            ErosionLevels[Target] = Modulo(GeoIndex[Target] + Depth, 20183);
            Types[Target] = ErosionLevels[Target] % 3;

            return Types.Values.Sum();
        }

        public int Solve(int part = 1)
            => SolveMaze(part);
    }
}
