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

        string FindOrder(int part = 1)
        {
            // I will implement this one iteratively instead of using recursion, for a change
            StringBuilder nodeOder = new();
            while (nodes.Any(x => x.Solved == false))
            {
                var iterationSet = nodes.Where(x => x.CanBeSolved && !x.Solved).OrderBy(x => x.Id).ToList();
                var node = iterationSet.FirstOrDefault();
                if (node != null) 
                {
                    node.Solved = true;
                    nodeOder.Append(node.Id);
                }
            }
            return nodeOder.ToString();
        }

        public string Solve(int part = 1)
            => FindOrder(part);
    }
}
