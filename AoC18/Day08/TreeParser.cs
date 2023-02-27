namespace AoC18.Day08
{
    class Node
    {
        public List<Node> children = new();
        public List<int> metadata = new();
        public int Value()
        {
            if (children.Count() == 0)
                return metadata.Sum();
            else
            {
                var interesting = metadata.Select(x => x - 1).Where(y => y >= 0 && y < children.Count()).ToList();
                return interesting.Sum(i => children[i].Value());
            }
        }
    }

    internal class TreeParser
    {
        List<int> inputData = new();
        List<Node> tree = new();

        public void ParseInput(List<string> lines)
            => inputData = lines[0].Split(" ").Select(x => int.Parse(x)).ToList();

        Node ParseNode(ref int index)
        {
            Node currentNode = new();
            var numChildren = inputData[index++];
            var numMetadata = inputData[index++];

            for(int i =0; i<numChildren; i++) 
            {
                var child = ParseNode(ref index);
                currentNode.children.Add(child);
                tree.Add(child);
            }

            for (int j = 0; j < numMetadata; j++)
                currentNode.metadata.Add(inputData[index++]);

            return currentNode;
        }

        int FindSum(int part = 1)
        {
            var index = 0;
            var root = ParseNode(ref index);
            tree.Add(root);
            return part == 1 ? tree.Sum(x => x.metadata.Sum()) : root.Value();
        }

        public int Solve(int part = 1)
            => FindSum(part);
    }
}
