using AoC18.Common;

namespace AoC18.Day25
{
    class ConstellationMaker
    {
        List<Coord4D> Stars = new();

        public void ParseLine(string line)
        {
            var c = line.Split(',').ToList().Select(int.Parse).ToList();
            Stars.Add( (c[0], c[1], c[2], c[3]) );
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        bool BelongsToConstellation(List<Coord4D> constellation, Coord4D star)
            => constellation.Select(x => x.Manhattan(star)).Any(x => x <= 3);

        void AddStarsToConstellation(List<Coord4D> cons)
        {
            var added = true;
            while (added)
            {
                added = false;
                foreach(var st in Stars)
                { 
                    var minDist = cons.Min(x => st.Manhattan(x));
                    if (minDist <= 3)
                    {
                        added = true;
                        cons.Add(st);
                        Stars.Remove(st);
                        break;
                    }
                }
            }
        }

        int BuildConstellations()
        {
            List<List<Coord4D>> Constellations = new();
            while(Stars.Count>0) 
            {
                var star = Stars[0];
                Stars.RemoveAt(0);

                List<Coord4D> newConstellation = new() { star };
                AddStarsToConstellation(newConstellation);
                Constellations.Add(newConstellation);
            }
            return Constellations.Count;
        }

        public int Solve(int part = 1)
            => BuildConstellations();

    }
}


