using AoC18.Common;

namespace AoC18.Day06
{
    internal class LocationSpace
    {
        List<Coord2D> locations = new();

        void ParseLine(string line)
        {
            var coord = line.Split(",").Select(x => int.Parse(x)).ToArray();
            locations.Add(new Coord2D(coord[0], coord[1]));
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        List<(Coord2D position, int distance)> GetDistances(Coord2D position)
            => locations.Select((x, a) => (x, x.Manhattan(position))).ToList();

        int LargestArea(int part=1)
        {
            var minX = locations.Min(c => c.x);
            var maxX = locations.Max(c => c.x);
            var minY = locations.Min(c => c.y);
            var maxY = locations.Max(c => c.y);

            HashSet<Coord2D> infinityPoints = new();
            Dictionary<Coord2D, int> amountOfClosests = new();

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY;y++)
                {
                    var currentPoint = new Coord2D(x, y);
                    var listDistances = GetDistances(currentPoint).OrderBy(x => x.distance).ToList(); 
                    var minDistance = listDistances.Min(x => x.distance);

                    var closest = listDistances.Where(x => x.distance == minDistance).ToList();
                    if ( (x == minX || y == minY || x == maxX || y == maxY) && closest.Count() ==1)
                        infinityPoints.Add(closest.First().position);

                    if (closest.Count == 1)
                    {
                        var closestLocation = closest.Select(x => x.position).First();
                        if (!amountOfClosests.ContainsKey(closestLocation))
                            amountOfClosests[closestLocation] = 0;
                        amountOfClosests[closestLocation]++;
                    }
                }
            if(part==1)
                return amountOfClosests.Where(x => !infinityPoints.Contains(x.Key)).Max(x => x.Value);

            // Part 2
            var safePositionCounter = 0;

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    safePositionCounter += locations.Sum(p => p.Manhattan(new Coord2D(x, y))) < 10000 ? 1 : 0;

            return safePositionCounter;
        }

        public int Solve(int part = 1)
            => LargestArea(part);
    }
}
