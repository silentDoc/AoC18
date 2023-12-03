using AoC18.Common;

namespace AoC18.Day18
{
    public static class CellType
    {
        public static char Open = '.';
        public static char Tree = '|';
        public static char Lumberyard = '#';
    }

    public class LumberjackElves
    {
        Dictionary<Coord2D, char> Map = new();
        int MaxX = 0;
        int MaxY = 0;

        public void ParseLine(string line, int row)
        { 
            for(int i=0; i<line.Length; i++)
                Map[new Coord2D(i,row)] = line[i];
        }

        public void ParseInput(List<string> lines)
        {
            for(int row = 0; row < lines.Count;row++)
                ParseLine(lines[row], row);

            MaxX = Map.Keys.Max(p => p.x);
            MaxY = Map.Keys.Max(p => p.y);
        }

        List<Coord2D> GetValidNeighbors(Coord2D pos)
            => pos.GetNeighbors8().Where(p => p.x>=0 && p.y>=0 && p.x<=MaxX && p.y <= MaxY).ToList();

        char Transform(char currentCell, List<char> adjacent)
        { 
            char result = currentCell;

            if(currentCell == CellType.Open)
                result = adjacent.Count(x => x == CellType.Tree) >= 3 ? CellType.Tree : currentCell;

            if (currentCell == CellType.Tree)
                result = adjacent.Count(x => x == CellType.Lumberyard) >= 3 ? CellType.Lumberyard : currentCell;

            if (currentCell == CellType.Lumberyard)
                result = adjacent.Any(x => x == CellType.Lumberyard) && adjacent.Any(x => x == CellType.Tree) ? CellType.Lumberyard : CellType.Open;

            return result;
        }

        private Dictionary<Coord2D, char> Evolve()
        {
            Dictionary<Coord2D, char> newState = Map.ToDictionary( x => x.Key, x => x.Value);
            foreach(var pos in newState.Keys) 
            {
                var neighs = GetValidNeighbors(pos);
                var materials = neighs.Select(x => Map[x]).ToList();
                newState[pos] = Transform(Map[pos], materials);
            }
            return newState;
        }

        private int SolvePart1()
        {
            for (int i = 0; i < 10; i++)
                Map = Evolve();

            return Map.Values.Count(x => x == CellType.Lumberyard) * Map.Values.Count(x => x == CellType.Tree);
        }

        public int Solve(int part)
            => part == 1 ? SolvePart1() : 0;
    }
}
