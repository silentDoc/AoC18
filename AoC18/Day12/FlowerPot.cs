using System.Text;

namespace AoC18.Day12
{
    class FlowerState
    {
        public string pots = "";
        public long left = 0;
    }


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

        FlowerState Grow(FlowerState state)
        {
            var pots = "....." + state.pots + ".....";
            var newPots = "";
            for (var i = 2; i < pots.Length - 2; i++)
            {
                var x = pots.Substring(i - 2, 5);
                newPots += rules.TryGetValue(x, out var ch) ? ch : ".";
            }

            var firstFlower = newPots.IndexOf("#");
            var newLeft = firstFlower + state.left - 3;

            newPots = newPots.Substring(firstFlower);
            newPots = newPots.Substring(0, newPots.LastIndexOf("#") + 1);
            return  new FlowerState { left = newLeft, pots = newPots };
        }

        long GrowGenerations(long numGenerations)
        {
            long dif_leftPos = 0;
            //long currentGen = 0;
            FlowerState currentState = new FlowerState { left = 0, pots = initialState };

            while (numGenerations > 0)
            {
                var prevState = currentState;
                currentState = Grow(currentState);

                numGenerations--;
                dif_leftPos = currentState.left - prevState.left;

                if (currentState.pots == prevState.pots)
                {
                    currentState = new FlowerState { left = currentState.left + numGenerations * dif_leftPos, pots = currentState.pots };
                    break;
                }
            }

            return Enumerable.Range(0, currentState.pots.Length).Select(i => currentState.pots[i] == '#' ? i + currentState.left : 0).Sum();
        }


        public long Solve(int part = 1)
            => GrowGenerations(part == 1 ? 20L : 50000000000L);
    }
}
