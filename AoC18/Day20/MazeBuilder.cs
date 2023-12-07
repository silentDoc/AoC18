using AoC18.Common;

namespace AoC18.Day20
{
    class MazeRoom
    {
        public Coord2D position = new(0, 0);
        public List<MazeRoom> openAdjacentRooms = new List<MazeRoom>();

        public MazeRoom(Coord2D pos)
            => position = pos;

        public int Steps = 9999999;

        public void AddAdjacent(MazeRoom adjacentRoom)
        {
            if (!openAdjacentRooms.Contains(adjacentRoom))
                openAdjacentRooms.Add(adjacentRoom);
        }
    }

    class MazeBuilder
    {
        string MazeRegex = "";
        List<MazeRoom> Maze = new();

        Coord2D dirN = new Coord2D(0, -1);
        Coord2D dirS = new Coord2D(0, 1);
        Coord2D dirW = new Coord2D(-1, 0);
        Coord2D dirE = new Coord2D(1, 0);

        public void ParseInput(List<string> lines)
            => MazeRegex = lines[0];

        void UpdateRoom(Coord2D newRoom, MazeRoom adjacent)
        {
            var nRoom = Maze.FirstOrDefault(x => x.position == newRoom);
            if (nRoom == null)
            { 
                nRoom = new MazeRoom(newRoom);
                Maze.Add(nRoom);
            }
            nRoom.AddAdjacent(adjacent);
            adjacent.AddAdjacent(nRoom);
        }

        MazeRoom GetRoom(Coord2D pos)
            => Maze.First(x => x.position == pos);

        void ParseSubstring(int currentChar, int endChar, MazeRoom currentRoom)
        {
            var directions = "NSEW";
            int charPtr = currentChar;
            var currentPos = currentRoom.position;
            var originalPos = currentRoom.position;

            while (charPtr <= endChar) 
            {
                if (MazeRegex[charPtr] == '(')
                {
                    var level = 0;
                    int i = 0;
                    for (i = charPtr + 1; i < MazeRegex.Length; i++)
                        if (MazeRegex[i] == '(')
                            level++;
                        else if (MazeRegex[i] == ')' && level == 0)
                            break;
                        else if (MazeRegex[i] == ')')
                            level--;

                    ParseSubstring(charPtr + 1, i - 1, GetRoom(currentPos));
                    charPtr = i;
                }
                else if (MazeRegex[charPtr] == '|' && MazeRegex[charPtr + 1] == ')')
                    return;
                else if (MazeRegex[charPtr] == '|')
                    currentPos = originalPos;
                else if (directions.Contains(MazeRegex[charPtr]))
                {
                    var nextPos = MazeRegex[charPtr] switch
                    {
                        'N' => currentPos + dirN,
                        'S' => currentPos + dirS,
                        'E' => currentPos + dirE,
                        'W' => currentPos + dirW,
                        _ => throw new Exception("Unexpected symol" + MazeRegex[charPtr].ToString())
                    };

                    UpdateRoom(nextPos, GetRoom(currentPos));
                    currentPos = nextPos;
                }
                else
                    throw new Exception("Unexpected symol" + MazeRegex[charPtr].ToString());

                charPtr++;
            }
        }

        void TraverseMaze(List<MazeRoom> rooms, int newCost)
        {
            if (rooms.Count == 0)
                return;

            var adjacentSet = rooms.SelectMany(x => x.openAdjacentRooms).Where(r => r.Steps == 9999999).ToList() ;

            foreach (var room in adjacentSet)
                room.Steps = newCost;

            TraverseMaze(adjacentSet, newCost + 1);
        }

        int SolveMaze(int part)
        {
            // Part 1 - Build the maze
            var startPos = new Coord2D(0, 0);
            MazeRoom start = new MazeRoom(startPos);
            start.Steps = 0;
            Maze.Add(start);
            int currentChar = 1;
            int endChar = MazeRegex.Length - 2;
            ParseSubstring(currentChar, endChar, start);
           
            // Part 2  - Traverse and yield results
            TraverseMaze(new List<MazeRoom> { start }, 1);
            
            return part == 1 ? Maze.Max(x => x.Steps)
                             : Maze.Count(x => x.Steps>=1000);
        }

        public int Solve(int part = 1)
            => SolveMaze(part);
    }
}
