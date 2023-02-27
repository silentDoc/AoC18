using System.Diagnostics;
using System.Text;

namespace AoC18.Day07
{
    class Node
    {
        public char Id;
        public bool Solved = false;
        public bool CanBeSolved
            => Dependencies.Count == 0 || !Dependencies.Any(x => x.Solved == false);
        public List<Node> Dependencies = new();
    }

    internal class PrecedenceSolver
    {
        List<Node> nodes = new();

        void ParseLine(string line)
        {
            //Step O must be finished before step W can begin
            var nodesStr = line.Replace("Step ", "").Replace("must be finished before step ", "").Replace(" can begin.","");
            char[] nodeArray = nodesStr.Split(" ").Select(x => char.Parse(x.Trim())).ToArray();
            
            var dependencyNode = nodes.FirstOrDefault(x => x.Id == nodeArray[0]);
            if (dependencyNode == null)
            {
                dependencyNode = new Node() { Id = nodeArray[0] };
                nodes.Add(dependencyNode);
            }

            var node = nodes.FirstOrDefault(x => x.Id == nodeArray[1]);
            if (node == null)
            {
                node = new Node() { Id = nodeArray[1] };
                nodes.Add(node);
            }
            node.Dependencies.Add(dependencyNode);
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        string FindOrder()
        {
            // I will implement this one iteratively instead of using recursion, for a change
            StringBuilder nodeOrder = new();
            while (nodes.Any(x => x.Solved == false))
            {
                var iterationSet = nodes.Where(x => x.CanBeSolved && !x.Solved).OrderBy(x => x.Id).ToList();
                var node = iterationSet.FirstOrDefault();
                if (node != null) 
                {
                    node.Solved = true;
                    nodeOrder.Append(node.Id);
                }
            }
            return nodeOrder.ToString();
        }

        string TeamWork()
        {
            List<int> releaseSecond = new();
            List<char> nodeBeingProcessed = new();
            int tick = 0;
            const int Num_Workers = 5;  
            const int Cycle_Add = 60;

            StringBuilder nodeOrder = new();
            while (nodes.Any(x => x.Solved == false))
            {
                // Check if the work has finished
                if (releaseSecond.Any(x => x == tick))
                {
                    var releases = releaseSecond.Where(x => x == tick).ToList();
                    foreach(var release in releases) 
                    {
                        var ind = releaseSecond.IndexOf(tick);
                        var nodeId = nodeBeingProcessed[ind];
                        nodes.First(x => x.Id == nodeId).Solved = true;
                        releaseSecond.RemoveAt(ind);
                        nodeBeingProcessed.RemoveAt(ind);
                        nodeOrder.Append(nodeId);
                    }
                }
                
                // Check if we can start solving something
                var iterationSet = nodes.Where(x => x.CanBeSolved && !x.Solved && !nodeBeingProcessed.Contains(x.Id)).OrderBy(x => x.Id).ToList();
                var avaialableElves = Math.Max(0, Num_Workers - nodeBeingProcessed.Count);
                var numToSolve = Math.Min(iterationSet.Count, avaialableElves);

                for (int i = 0; i < numToSolve; i++)
                {
                    char id = iterationSet[i].Id;
                    nodeBeingProcessed.Add(id);
                    int relSecond = Cycle_Add + (id - 'A' + 1);
                    releaseSecond.Add(tick+relSecond);
                }
                if (releaseSecond.Count() == 0)
                    break;
                else
                    tick = releaseSecond.Min();
            }
            return tick.ToString();
        }

        public string Solve(int part = 1)
            => part ==1 ? FindOrder() : TeamWork();
    }
}
