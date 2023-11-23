using AoC18.Common;
using System.Linq;

namespace AoC18.Day15
{
    class Unit
    {
        public Coord2D Position = new(0, 0);
        public int Health = 200;
        public int Power = 3;
        public char Type;
        public bool Alive
            => Health <= 0;

        public Unit(Coord2D pos, char type)
        {
            Position = pos;
            Type = type;
        }

        public void Move(Coord2D pos)
            => Position = pos;

        public void Attack(Unit target)
            => target.Health -= Power;

        public List<Coord2D> AdjacentOpenPositions(Dictionary<Coord2D,char> map, List<Unit> aliveUnits)
            =>  Position.GetNeighbors().Where(p => map.ContainsKey(p))
                                       .Where(p => map[p] == '.')
                                       .Where(p => !aliveUnits.Select(u => u.Position).Contains(p))    
                                       .OrderBy(p => p.y).ThenBy(p => p.x)
                                       .ToList();

        public List<Unit> AdjacentTargets(List<Unit> units)
            => units.Where(u => Type != u.Type && Position.GetNeighbors().Contains(u.Position))
                    .OrderBy(u => u.Health)
                    .ThenBy(u => u.Position.y).ThenBy(u => u.Position.x)
                    .ToList();

    }

    class ShortPathResults
    { 
        public int Distance = -1;
        public Dictionary<Coord2D, int> CellsWithCost = null;
    }

    class Battlefield
    {
        Dictionary<Coord2D, char> map = new();
        List<Unit> units = new();
        int width = 0;
        int height = 0;

        public void ParseInput(List<string> lines)
        {
            for (int y = 0; y < lines.Count; y++)
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == 'E' || lines[y][x] == 'G')
                    {
                        units.Add(new Unit(new Coord2D(x, y), lines[y][x]));
                        map.Add(new Coord2D(x, y), '.');
                    }
                    else
                        map.Add(new Coord2D(x, y), lines[y][x]);
                }

            width = lines[0].Length;
            height = lines.Count;
        }

        HashSet<Coord2D> OpenMapPositions
            => new HashSet<Coord2D>(map.Keys.Where(pos => !units.Select(u => u.Position).Contains(pos) && map[pos]=='.').ToList());


        Unit? MoveUnit(Unit currentUnit, Dictionary<Coord2D, char> map, List<Unit> aliveEnemyList)
        {
            // Step 1 - Check if there are any adjacent enemies
            var adjacentTargets = currentUnit.AdjacentTargets(aliveEnemyList);

            if (adjacentTargets.Count > 0)
            {
                // Select the enemy with the lowest health and break tie based on position
                var target = adjacentTargets.OrderBy(u => u.Health)
                                            .ThenBy(u => u.Position.y).ThenBy(u => u.Position.x)
                                            .First();
                return target;
            }

            // Step 2 - No adjacent enemies, so find the shortest path to the nearest enemy
            var openMapPositions = OpenMapPositions;
            var destination = new Coord2D(-1, -1);
            // In Range - Reachable - Nearest - Chosen
            var InRangePositions = aliveEnemyList.SelectMany(u => u.Position.GetNeighbors()).Where(p => openMapPositions.Contains(p)).Distinct().ToList();
            var ReachablePositions = InRangePositions.Where(p => ShortestPath(currentUnit.Position, p, openMapPositions).Distance != -1).ToList();
            var NearestPositions = ReachablePositions.Select(p => ShortestPath(currentUnit.Position, p, openMapPositions)).OrderBy(p => p.Distance).ToList();
            var minDistance = NearestPositions.First().Distance;
            

            Dictionary<Unit, ShortPathResults> moveDecisionInfo = new();

            foreach (var enemy in aliveEnemyList)
                moveDecisionInfo[enemy] = ShortestPath(currentUnit.Position, enemy.Position, openMapPositions);

            var local_minDistance = moveDecisionInfo.Values.Min(p => p.Distance);

            if(minDistance == -1)   // No target can be reached, we dont move
                return null;

            var closestTargets = moveDecisionInfo.Keys.Where(u => moveDecisionInfo[u].Distance == minDistance).ToList();
            Dictionary<Coord2D, int> adjacentCells = new();
            
            foreach (var candidate in closestTargets)
            {
                var info = moveDecisionInfo[candidate];
                var surroundings = candidate.Position.GetNeighbors().Where(p => info.CellsWithCost.ContainsKey(p)).ToList();
                var minCost = surroundings.Min(p => info.CellsWithCost[p]);

                foreach (var cell in surroundings.Where(c => info.CellsWithCost[c] == minCost))
                {
                    if(adjacentCells.ContainsKey(cell))
                        adjacentCells[cell] = minCost;
                    else
                        if (minCost < adjacentCells[cell])
                            adjacentCells[cell] = minCost;
                }
            }

            var minCostAdjacent = adjacentCells.Values.Min();






            /*foreach (var t in closestTargets.)
            { 
                
            }


            var selectedTarget = closestTargets.OrderBy(p => p.Key.Position.y).ThenBy(p => p.Key.Position.x).ToList().First();

            var pathInfo = moveDecisionInfo[selectedTarget.Key];

            return selectedTarget.Key;*/
            return null;
        }


        ShortPathResults ShortestPath(Coord2D start, Coord2D end, HashSet<Coord2D> availableMapCells, int initial_cost = 0)
        {
            Queue<(Coord2D pos, int cost)> priorityQueue = new();
            HashSet<(Coord2D pos, int cost)> visited = new();
            priorityQueue.Enqueue((start, initial_cost));

            while (priorityQueue.Count > 0)
            {
                var item = priorityQueue.Dequeue();
                var mapCell = item.pos;
                var currentCost = item.cost;

                if (mapCell == end)
                {
                    ShortPathResults res = new();
                    res.Distance = currentCost;
                    res.CellsWithCost = visited.ToDictionary(p => p.pos, p => p.cost);
                    return res;
                }

                var newCost = currentCost + 1;

                var candidates = mapCell.GetNeighbors().Where(p => availableMapCells.Contains(p)).ToList();
                
                foreach (var candidate in candidates)
                    if (visited.Add((candidate, newCost)))
                        priorityQueue.Enqueue((candidate, newCost));
            }

            return new ShortPathResults();
        }

        void AttackTargets(Unit currentUnit, Dictionary<Coord2D, char> map, List<Unit> aliveEnemyList)
        {

        }   

        int FindOutcome()
        {
            var aliveGoblins = true;
            var aliveElves = true;

            while (aliveElves && aliveGoblins)
            {
                units = units.OrderBy(c => c.Position.y).ThenBy(c => c.Position.x).ToList();

                foreach (var unit in units) 
                {
                    var aliveEnemyList = units.Where(u => u.Type != unit.Type && u.Alive).ToList();

                    var closestTarget = MoveUnit(unit, map, aliveEnemyList);
                    if (closestTarget == null)
                        continue;

                    //AttackTarget(unit, map, aliveEnemyList);
                }
                

                aliveGoblins = units.Count(u => u.Type == 'G' && u.Alive) > 0;
                aliveElves = units.Count(u => u.Type == 'E' && u.Alive) > 0;
            }

            return 0;
        }

        public string Solve(int part)
            => (part == 1) ? FindOutcome().ToString() : "";


    }
}
