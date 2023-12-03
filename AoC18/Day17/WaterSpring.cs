using AoC18.Common;

namespace AoC18.Day17
{
    static class Symbols
    {
        public static char Clay = '#';
        public static char Air = '.';
        public static char MovingWater = '|';
        public static char StillWater = '~';
        public static char Origin = '+';
    }

    internal class WaterSpring
    {
        Dictionary<Coord2D, char> map = new();
        
        Coord2D direction_left = new Coord2D(-1, 0);
        Coord2D direction_right = new Coord2D(1, 0);
        Coord2D direction_down = new Coord2D(0, 1);
        Coord2D direction_up = new Coord2D(0, -1);
        List<Coord2D> clayPositions = new();

        int displayMinX = 0;
        int displayMaxX = 0;
        int minYValue = 0;

        char[,] mapFast;

        private void AddTrail(string line)
        { 
            var parts = line.Split(',');
            var fixedValue = int.Parse(parts[0].Substring(2));
            var rangeValues = parts[1].Trim().Substring(2).Split("..").Select(int.Parse).ToList();

            for(int v = rangeValues[0]; v <= rangeValues[1]; v++) 
                clayPositions.Add(parts[0].StartsWith("x") ? new Coord2D(fixedValue, v) 
                                                           : new Coord2D(v, fixedValue));
        }

        public void ParseInput(List<string> input)
            => input.ForEach(AddTrail);

        private void SetupMap()
        {
            mapFast = new char[clayPositions.Max(p => p.x) + 5, clayPositions.Max(p => p.y) + 1];

            for (int i = 0; i < mapFast.GetLength(0); i++)
                for (int j = 0; j < mapFast.GetLength(1); j++)
                    mapFast[i, j] = Symbols.Air;

            displayMinX = clayPositions.Min(p => p.x) - 1;
            displayMaxX = clayPositions.Max(p => p.x) + 2;
            minYValue = clayPositions.Min(p => p.y);

            clayPositions.ForEach(p => mapFast[p.x, p.y] = Symbols.Clay);
        }

        char Material(Coord2D position)
            => mapFast[position.x, position.y];

        void Set(Coord2D position, char material)
            => mapFast[position.x, position.y] = material;

        Coord2D FindLeftBound(Coord2D position)
        {
            Coord2D current = position;
            while ((Material(current + direction_left) == Symbols.Air || Material(current + direction_left) == Symbols.MovingWater ) &&
                   (Material(current + direction_down) == Symbols.Clay || Material(current + direction_down) == Symbols.StillWater))
                current = current + direction_left;

            return current;
        }

        Coord2D FindRightBound(Coord2D position)
        {
            Coord2D current = position;
            while ((Material(current + direction_right) == Symbols.Air || Material(current + direction_right) == Symbols.MovingWater) &&
                   (Material(current + direction_down) == Symbols.Clay || Material(current + direction_down) == Symbols.StillWater))
                current = current + direction_right;

            return current;
        }

        bool HasFloor(Coord2D position)
            => !(Material(position + direction_down) == Symbols.Air);

        void Fill(Coord2D left, Coord2D right, char symbol)
        {
            for (int i = left.x; i <= right.x; i++)
                mapFast[i, left.y] = symbol;
        }

        void Flood(Coord2D leak, Stack<Coord2D> leaks)
        {
            if (Material(leak + direction_down) == Symbols.MovingWater)
                return;
            var left = FindLeftBound(leak);
            var right = FindRightBound(leak);

            var leftOpen = !HasFloor(left);
            var rightOpen = !HasFloor(right);

            if (!leftOpen && !rightOpen)
            {
                Fill(left, right, Symbols.StillWater);
                leaks.Push(leak + direction_up);
            }
            else
                Fill(left, right, Symbols.MovingWater);

            if (leftOpen)
                if(!leaks.Contains(left))
                    leaks.Push(left);
            if (rightOpen)
                if (!leaks.Contains(right))
                    leaks.Push(right);
        }

        private int FillCavern(int part = 1)
        {
            SetupMap();
            Stack<Coord2D> leaks = new();
            var maxHeight = mapFast.GetLength(1);
            leaks.Push(new Coord2D(500, 0));

            while(leaks.Count > 0) 
            { 
                var currentLeak = leaks.Pop();

                // We make the leak fall while we are falling on air
                while (currentLeak.y < maxHeight && Material(currentLeak  + direction_down) == Symbols.Air)
                {
                    Set(currentLeak, Symbols.MovingWater);
                    currentLeak = currentLeak + direction_down;

                    if (currentLeak.y == maxHeight-1)
                    {
                        Set(currentLeak, Symbols.MovingWater);
                        break;
                    }
                }
                if (currentLeak.y == maxHeight-1)   // End of the map
                    continue;

                if ( Material(currentLeak) == Symbols.Air && Material(currentLeak+direction_down) == Symbols.MovingWater)
                {
                    // We land on an existing overflowing surface which should be resolved already
                    Set(currentLeak, Symbols.MovingWater);
                    continue;
                }
                else
                    Flood(currentLeak, leaks);

            }
            mapFast[500, 0] = Symbols.Origin;
            var totalWater = (part == 1) ? mapFast.Cast<char>().ToArray().Count(x => x == Symbols.StillWater || x == Symbols.MovingWater)
                                         : mapFast.Cast<char>().ToArray().Count(x => x == Symbols.StillWater);

            // Count invalid water to remove it
            int invalidWater = 0;
            for (int j = 0; j < minYValue; j++)
                for (int i = displayMinX; i < displayMaxX; i++)
                    if (mapFast[i, j] == Symbols.StillWater || mapFast[i, j] == Symbols.MovingWater)
                        invalidWater++;

            return (part ==1) ? totalWater - invalidWater : totalWater;     // We do not substract invalid water in part 2 - all invalid water is moving water
        }

        public int Solve(int part)
            => FillCavern(part);   
    }
}
