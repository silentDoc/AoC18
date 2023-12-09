using AoC18.Common;

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

        int FindShortestPath()
        {
            // Build a map of valid cells, adding the items we can handle in that cell as a third axis
            // Coord3D.z => Tool ; 0 - Torch, 1 - Climbing Gear, 2 - Neither
            List<Coord3D> CellsWithTool = new();
            var allKeys = Types.Keys.ToList();

            foreach (var key in Types.Keys.ToList())
            {
                if (key == (0, 0) || key == Target)
                    continue;

                if (Types[key] == 0) // rocky
                {
                    CellsWithTool.Add((key.x, key.y, 0));
                    CellsWithTool.Add((key.x, key.y, 1));
                }
                else if(Types[key] == 1) // wet
                {
                    CellsWithTool.Add((key.x, key.y, 1));
                    CellsWithTool.Add((key.x, key.y, 2));
                }
                else // Narrow
                {
                    CellsWithTool.Add((key.x, key.y, 0));
                    CellsWithTool.Add((key.x, key.y, 2));
                }
            }

            CellsWithTool.Add((0, 0, 0));
            CellsWithTool.Add((Target.x, Target.y, 0));

            Console.WriteLine(CellsWithTool.Count.ToString());

            return Dijkstra(CellsWithTool)+2; // ShortestPath(CellsWithTool);
        }


        Coord3D MinDistance2(Dictionary<Coord3D, int> distances, Dictionary<Coord3D, bool> seen)
        {
            var eligible = seen.Keys.Where(x => seen[x] == false);
            var dist = distances.Keys.Where(x => eligible.Contains(x)).Min(x => distances[x]);
            return eligible.First(x => distances[x] == dist);
        }


        Coord3D MinDistance(Dictionary<Coord3D, int> distances, Dictionary<Coord3D, bool> seen)
        { 
            int min = int.MaxValue;
            Coord3D nodeKey = (0,0,0);

            foreach(var k in distances.Keys)
            {
                if (seen[k] == false && distances[k] <= min)
                {
                    min = distances[k];
                    nodeKey = k;
                }
            }

            return nodeKey;
        }


        int Edge(Coord3D node1, Coord3D node2)
        => (node1 - node2) switch
            {
                (1, 0, 0)  => 1,
                (0, 1, 0)  => 1,
                (-1, 0, 0) => 1,
                (0, -1, 0) => 1,
                (1, 0, _)  => 8,
                (0, 1, _)  => 8,
                (-1, 0, _) => 8,
                (0, -1, _) => 8,
                (_, _, _)  => 0,
            };

        int Dijkstra(List<Coord3D> cells)
        {
            var verticesCount = cells.Count;
            Dictionary<Coord3D, int> distances = new();
            Dictionary<Coord3D, bool> seen = new();

            foreach (var cell in cells)
            {
                distances[cell] = int.MaxValue;
                seen[cell] = false;
            }

            distances[(0, 0, 0)] = 0;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                var newNodeInPath = MinDistance(distances, seen);
                seen[newNodeInPath] = true;

                var distNotMax = distances.Values.Count(x => x < int.MaxValue);

                //for (int v = 0; v < verticesCount; ++v)
                foreach(var key in distances.Keys)
                {
                    var edge = Edge(newNodeInPath, key);
                    if (!seen[key] 
                        && edge > 0
                        && distances[newNodeInPath] != int.MaxValue
                        && distances[newNodeInPath] + edge < distances[key])
                    {
                        distances[key] = distances[newNodeInPath] + edge;
                    }
                }

                if (count % 1000 == 0)
                    Console.WriteLine(count.ToString());
            }
            
            return distances[(Target.x, Target.y, 0)];
        }


       
        int SolveMaze(int part =1)
        {
            var additionalCells = (part == 1) ? 0 : 30;

            for (int i = 0; i <= Target.x + additionalCells; i++)
                for (int j = 0; j <= Target.y + additionalCells; j++)
                {
                    GeoIndex[(i, j)] = CellRisk((i, j));
                    ErosionLevels[(i, j)] = Modulo(GeoIndex[(i, j)] + Depth, 20183);
                    Types[(i, j)] = ErosionLevels[(i, j)] % 3;
                }

            GeoIndex[Target] = 0;
            ErosionLevels[Target] = Modulo(GeoIndex[Target] + Depth, 20183);
            Types[Target] = ErosionLevels[Target] % 3;

            return part == 1 ? Types.Values.Sum()
                            : FindShortestPath();
        }

        public int Solve(int part = 1)
            => SolveMaze(part);
    }
}
