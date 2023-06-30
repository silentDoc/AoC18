using System.Text;

namespace AoC18.Day12
{
    internal class FlowerPot
    {
        Dictionary<string, string> rules = new();
        string initialState = "";
        int windowWidth = 5;

        void ParseLine(string line)
        {
            var elements = line.Split("=>", StringSplitOptions.TrimEntries).ToList();
            rules[elements[0]] =elements[1];
        }

        public void ParseInput(List<string> lines)
        {
            initialState = lines[0].Replace("initial state: ", "").Trim();
            lines.Skip(2).ToList().ForEach(ParseLine);
        }

        int AdvanceGeneration(string currentGen, out string nextGen)
        {
            StringBuilder sb = new();
            var current = "...." + currentGen + "....";
            int newStartPosition = -1;

            for (int i = 0; i < current.Length - windowWidth; i++)
            {
                var group = current.Substring(i, windowWidth);
                var newChar = rules.ContainsKey(group) ? rules[group] : ".";

                if (sb.Length != 0)
                    sb.Append(newChar);
                else if (newChar == "#")
                {
                    newStartPosition = i - 2;
                    sb.Append(newChar);
                }
            }

            nextGen = sb.ToString();
            return newStartPosition;
        }

        int GrowGenerations(int numGenerations, int part = 1)
        {
            var zeroOffset = 0;
            string next = "";
            string current = initialState;

            for (int gen = 0; gen < numGenerations; gen++)
            {
                zeroOffset += AdvanceGeneration(current, out next);
                current = next;
            }

            var pots = current.ToArray().Select((pot, index) => new { pot, index}).Where(x => x.pot == '#').ToList();
            var realIndices = pots.Select(x => x.index + zeroOffset).ToList();
            var retVal = realIndices.Sum();

            return retVal;
        }

        public int Solve(int part = 1)
            => GrowGenerations(20, part);
    }
}
