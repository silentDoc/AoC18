using AoC18.Common;
using System.Text.RegularExpressions;

namespace AoC18.Day03
{
    record struct Patch
    {
        public int Id;
        public Coord2D Pos;
        public Coord2D Size;
    }

    internal class PatchFabric
    {
        List<Patch> patches = new();

        void ParseLine(string line)
        {
            Regex regex = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");
            Match match = regex.Match(line);
            int id = int.Parse(match.Groups[1].Value);
            Coord2D pos = new Coord2D(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Coord2D size = new Coord2D(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));

            Patch patch = new Patch() { Id = id, Pos = pos, Size = size };
            patches.Add(patch);
        }
        public void ParseInput(List<string> lines)
            => lines.ForEach(x => ParseLine(x));

        void FillPositions(Patch patch, Dictionary<Coord2D, int> positions)
        {
            for (int i = patch.Pos.x; i < patch.Pos.x + patch.Size.x; i++)
                for (int j = patch.Pos.y; j < patch.Pos.y + patch.Size.y; j++)
                {
                    var current = new Coord2D(i, j);
                    if (!positions.ContainsKey(current))
                        positions[current] = 0;
                    positions[current]++;
                }
        }

        int FindOverlap()
        {
            Dictionary<Coord2D, int> positions = new();
            patches.ForEach(x => FillPositions(x, positions));
            return positions.Values.Count(x => x > 1);
        }

        public int Solve(int part = 1)
            => FindOverlap();
    }
}
