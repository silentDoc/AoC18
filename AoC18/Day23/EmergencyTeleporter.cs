using AoC18.Common;
using System.Text.RegularExpressions;

namespace AoC18.Day23
{
    public class Nanobot
    {
        public Coord3DL Position;
        public long Range;

        public Nanobot(string inputLine)
        { 
            Regex regex = new Regex(@"pos=<(-?\d+),(-?\d+),(-?\d+)>, r=(\d+)");
            var groups = regex.Match(inputLine).Groups;
            Position = (long.Parse(groups[1].Value), long.Parse(groups[2].Value), long.Parse(groups[3].Value));
            Range = long.Parse(groups[4].Value);
        }

        public int HowManyInRange(List<Nanobot> list)
            => list.Count(n => Position.Manhattan(n.Position) <= Range);
    }

    internal class EmergencyTeleporter
    {
        List<Nanobot> Bots = new();
        public void ParseInput(List<string> lines)
            => lines.ForEach(x => Bots.Add(new Nanobot(x)));

        public int Solve(int part)
            => part == 1 ? Bots.First(x => x.Range == Bots.Max(b => b.Range)).HowManyInRange(Bots) : 0;
    }
}
