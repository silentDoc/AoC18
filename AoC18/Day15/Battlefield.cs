using AoC18.Common;

namespace AoC18.Day15
{
    class Unit
    {
        public int Id = 0;
        public Coord2D Position = new(0, 0);
        public int Health = 200;
        public int Power = 3;
        public char Type;
        public bool Alive
            => Health > 0;

        public Unit(Coord2D pos, char type, int unitId = 0)
        {
            Id = unitId;
            Position = pos;
            Type = type;
        }

        public void Move(Coord2D pos)
            => Position = pos;

        public void Attack(Unit target)
            => 
            target.Health -= Math.Min(Power, target.Health);     // We leave health to 0, not negatives

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

    class Battlefield
    {
        Dictionary<Coord2D, char> map = new();
        List<Unit> Units = new();
        int width = 0;
        int height = 0;

        public void ParseInput(List<string> lines)
        {
            int UnitCount = 0;

            for (int y = 0; y < lines.Count; y++)
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == 'E' || lines[y][x] == 'G')
                    {
                        Units.Add(new Unit(new Coord2D(x, y), lines[y][x], ++UnitCount));
                        map.Add(new Coord2D(x, y), '.');
                    }
                    else
                        map.Add(new Coord2D(x, y), lines[y][x]);
                }

            width = lines[0].Length;
            height = lines.Count;
        }

        HashSet<Coord2D> OpenMapPositions
            => new HashSet<Coord2D>(map.Keys.Where(pos => !Units.Where(x=>x.Alive).Select(u => u.Position).Contains(pos) && map[pos]=='.').ToList());

        Dictionary<Coord2D, int> TraverseMap(Coord2D start, List<Coord2D> interestPositions)
        {
            Dictionary<Coord2D, int> retVal = new();
            HashSet<Coord2D> visited = new();

            var openMapPositions = OpenMapPositions;
            int currentCost = 0;
            
            List<Coord2D> evalPositions = new() { start };

            var evalAdjacents = evalPositions.SelectMany(x => x.GetNeighbors()).Distinct()
                                              .Where(x => openMapPositions.Contains(x)).ToList();

            while (evalAdjacents.Count > 0 && interestPositions.Any(x => !retVal.Keys.Contains(x)))
            {
                currentCost++;
                var found = evalAdjacents.Intersect(interestPositions).ToList();

                if (found.Any())
                { 
                    found.ForEach(found => retVal[found] = currentCost);    // Only the closer ones are of interest
                    break;
                }

                evalAdjacents.ForEach(a => visited.Add(a));

                evalAdjacents = evalAdjacents.SelectMany(x => x.GetNeighbors()).Distinct()
                                              .Where(x => openMapPositions.Contains(x) && !visited.Contains(x)).ToList();
            }
            return retVal;
        }

        Unit? SelectEnemyInRange(Unit currentUnit)
        {
            var aliveEnemyList = Units.Where(u => u.Type != currentUnit.Type && u.Alive).ToList();
            var adjacentTargets = currentUnit.AdjacentTargets(aliveEnemyList);

            if (adjacentTargets.Count > 0)
            {
                // Select the enemy with the lowest health and break tie based on position
                var target = adjacentTargets.OrderBy(u => u.Health)
                                            .ThenBy(u => u.Position.y).ThenBy(u => u.Position.x)
                                            .First();
                return target;
            }
            return null;
        }

        Coord2D SelectMoveDest(Unit currentUnit)
        {
            var aliveEnemyList = Units.Where(u => u.Type != currentUnit.Type && u.Alive).ToList();

            // Step 2 - No adjacent enemies, so find the shortest path to the nearest enemy
            var openMapPositions = OpenMapPositions;
            var reachableTargetPositions = aliveEnemyList.SelectMany(x => x.Position.GetNeighbors())
                                                         .Distinct().Where(x => OpenMapPositions.Contains(x)).ToList();
            if (!reachableTargetPositions.Any())
                return new Coord2D(-1, -1);

            var Nearest = TraverseMap(currentUnit.Position, reachableTargetPositions);
            var Chosen = Nearest.Keys.Any() ? Nearest.Keys.OrderBy(p => p.y).ThenBy(p => p.x).First() : new Coord2D(-1, -1);

            return Chosen;
        }

        Coord2D SelectNextStep(Coord2D currentPos, Coord2D dest)
        {
            var openMapPositions = OpenMapPositions;
            var firstStepPositions = currentPos.GetNeighbors().Where(x => openMapPositions.Contains(x)).ToList();
            if (firstStepPositions.Contains(dest))
                return dest;

            var firstStepCandidates = TraverseMap(dest, firstStepPositions);
            return firstStepCandidates.Keys.OrderBy(u => u.y).ThenBy(u => u.x).First();
        }

        bool CombatRound()
        {
            var sortedAliveUnits = Units.Where(x => x.Alive)
                                        .OrderBy(u => u.Position.y).ThenBy(u => u.Position.x)
                                        .ToList();
            
            // Unit by unit - all the turns
            foreach (var current in  sortedAliveUnits) 
            {
                var enemyType = current.Type == 'E' ? 'G' : 'E';
                
                if (!Units.Any(x => x.Alive && x.Type == enemyType))    // Incomplete round
                    return false;

                if (!current.Alive) // May have been killed by a previous unit in this very same round
                    continue;

                var enemyInRange = SelectEnemyInRange(current);
                if (enemyInRange != null)
                {
                    current.Attack(enemyInRange);
                    continue;
                }

                var destination = SelectMoveDest(current);
                if (destination.x == -1)
                    continue;

                var nextPos = SelectNextStep(current.Position, destination);
                current.Position = nextPos;
                
                enemyInRange = SelectEnemyInRange(current);

                if (enemyInRange != null)
                    current.Attack(enemyInRange);
            }
            return true;
        }

        int FindOutcome()
        {
            var aliveGoblins = true;
            var aliveElves = true;
            int rounds = 0;

            while (aliveElves && aliveGoblins)
            {
                if (!CombatRound()) // Incomplete round
                    rounds--;
                aliveGoblins = Units.Count(u => u.Type == 'G' && u.Alive) > 0;
                aliveElves = Units.Count(u => u.Type == 'E' && u.Alive) > 0;
                rounds++;
            }
            PrintMap(rounds);
            return Units.Where(x => x.Alive).Sum(x => x.Health) * (rounds);
        }

        // Helpers
        void PaintActive(Unit currentUnit)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(currentUnit.Position.x, currentUnit.Position.y+1);
            Console.Write(currentUnit.Type);
            Console.ResetColor();
        }
        void PaintEnemy(Unit enemyUnit)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(enemyUnit.Position.x, enemyUnit.Position.y + 1);
            Console.Write(enemyUnit.Type);
            Console.ResetColor();
        }

        void PaintDest(Coord2D dest)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(dest.x, dest.y + 1);
            Console.Write(map[dest]);
            Console.ResetColor();
        }
        

        void PrintMap(int Round = 0)
        {
            Console.WriteLine("Round = " + Round.ToString());
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    var unit = Units.Where(x => x.Alive).FirstOrDefault(x => x.Position == (column, row));
                    if (unit != null)
                    {
                        Console.Write(unit.Type.ToString());
                        continue;
                    }
                    Console.Write(map[(column, row)]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            foreach (var unit in Units.Where(x => x.Alive && x.Type == 'G'))
                Console.Write(unit.Type.ToString() + " : " + unit.Health.ToString() + " ; ");
            Console.WriteLine();
            foreach (var unit in Units.Where(x => x.Alive && x.Type == 'E'))
                Console.Write(unit.Type.ToString() + " : " + unit.Health.ToString() + " ; ");
            Console.WriteLine();
        }

        public string Solve(int part)
            => (part == 1) ? FindOutcome().ToString() : "";
    }
}
