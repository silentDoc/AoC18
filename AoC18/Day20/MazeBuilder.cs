using AoC18.Common;

namespace AoC18.Day20
{
    class MazeRoom
    {
        public Coord2D position = new(0, 0);
        public List<Coord2D> openAdjacentRooms = new List<Coord2D>();

        public MazeRoom(Coord2D pos)
            => position = pos;

        public int Steps = 9999999;

        public void AddAdjacent(Coord2D adjacentRoom)
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

        void UpdateRoom(Coord2D newRoom, Coord2D adjacent)
        {
            var nRoom = Maze.FirstOrDefault(x => x.position == newRoom);
            if (nRoom == null)
            { 
                nRoom = new MazeRoom(newRoom);
                Maze.Add(nRoom);
            }
            nRoom.AddAdjacent(adjacent);
            var aRoom = Maze.First(x => x.position == adjacent);
            aRoom.AddAdjacent(newRoom);
        }

        MazeRoom GetRoom(Coord2D pos)
            => Maze.First(x => x.position == pos);

        int ShortestPath(Coord2D start, Coord2D end, int initial_cost = 0)
        {
            // I leave it here in case I need it later

            Queue<(Coord2D pos, int cost)> priorityQueue = new();
            HashSet<(Coord2D pos, int cost)> visited = new();
            priorityQueue.Enqueue((start, initial_cost));

            while (priorityQueue.Count > 0)
            {
                var item = priorityQueue.Dequeue();
                var room = item.pos;
                var currentCost = item.cost;

                if (room == end)
                    return currentCost;

                var newCost = currentCost + 1;
                var candidates = GetRoom(room).openAdjacentRooms;

                foreach (var candidate in candidates)
                    if (visited.Add((candidate, newCost)))
                        priorityQueue.Enqueue((candidate, newCost));
            }

            return -1;  // Not found
        }

        void TraverseMaze(List<MazeRoom> rooms, int newCost)
        {
            if(rooms.Count == 0) 
                return;

            var adjacentSet = rooms.SelectMany(x => x.openAdjacentRooms);
            var adjacentRoomSet = Maze.Where(r => r.Steps == 9999999 && adjacentSet.Contains(r.position)).ToList();
            foreach (var room in adjacentRoomSet)
                room.Steps = newCost;

            TraverseMaze(adjacentRoomSet, newCost+1);
        }


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

                    UpdateRoom(nextPos, currentPos);
                    currentPos = nextPos;
                }
                else
                    throw new Exception("Unexpected symol" + MazeRegex[charPtr].ToString());

                charPtr++;
            }
        }

        int SolveMaze()
        {
            // Part 1 - Build the maze
            var startPos = new Coord2D(0, 0);
            MazeRoom start = new MazeRoom(startPos);
            start.Steps = 0;
            Maze.Add(start);
            int currentChar = 1;
            int endChar = MazeRegex.Length - 2;
            ParseSubstring(currentChar, endChar, start);
           
            // First attempt - go room by room and get shortest path - very inefficient, takes a lot (10000 rooms)
            // Better to Traverse all the Maze
            // Find the shortest path for all rooms 
            /*var allRooms = Maze.Select(x => x.position).ToList();
            var allCosts = allRooms.Select(x => ShortestPath(startPos, x)).ToList();
            return allCosts.Max();*/

            TraverseMaze(new List<MazeRoom> { start }, 1);
            
            return Maze.Max(x => x.Steps);
        }

        public int Solve(int part = 1)
            => part == 1 ? SolveMaze() : 0;

    }
}
