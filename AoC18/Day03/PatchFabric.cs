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
        HashSet<int> overlaps = new();

        void ParseLine(string line)
        {
            Regex regex = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");
            var groups = regex.Match(line).Groups;
            int id = int.Parse(groups[1].Value);
            Coord2D pos = new Coord2D(int.Parse(groups[2].Value), int.Parse(groups[3].Value));
            Coord2D size = new Coord2D(int.Parse(groups[4].Value), int.Parse(groups[5].Value));

            Patch patch = new Patch() { Id = id, Pos = pos, Size = size };
            patches.Add(patch);
        }
        public void ParseInput(List<string> lines)
            => lines.ForEach(x => ParseLine(x));

        void FillPositions(Patch patch, Dictionary<Coord2D, int> positions, int part =1)
        {
            for (int i = patch.Pos.x; i < patch.Pos.x + patch.Size.x; i++)
                for (int j = patch.Pos.y; j < patch.Pos.y + patch.Size.y; j++)
                {
                    var current = new Coord2D(i, j);

                    if (!positions.ContainsKey(current))
                        positions[current] = part == 1 ? 1 : patch.Id;
                    else
                    {
                        if (part == 2)
                        {
                            overlaps.Add(positions[current]);
                            overlaps.Add(patch.Id);
                        }
                        positions[current] = part == 1 ? positions[current] + 1 : patch.Id;
                    }
                }
        }

        int FindOverlap(int part =1)
        {
            Dictionary<Coord2D, int> positions = new();
            patches.ForEach(x => FillPositions(x, positions, part));
            return part == 1 ? positions.Values.Count(x => x > 1) 
                             : patches.Select(x => x.Id).Where(i => !overlaps.Contains(i)).First();
        }

        public int Solve(int part = 1)
            => FindOverlap(part);
    }
}
