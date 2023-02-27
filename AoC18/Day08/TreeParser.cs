namespace AoC18.Day08
{
    class Node
    {
        public List<Node> children = new();
        public List<int> metadata = new();
        public int MetadataSum
            => metadata.Sum();
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

        int FindSum()
        {
            var index = 0;
            tree.Add(ParseNode(ref index));
            return tree.Sum(x => x.MetadataSum);
        }

        public int Solve(int part = 1)
            => FindSum();
    }
}
