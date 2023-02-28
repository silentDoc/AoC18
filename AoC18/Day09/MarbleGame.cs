namespace AoC18.Day09
{
    class Game_Part1
    {
        // Just left here to see differences between part 1 and part 2
        public int NumPlayers = 0;
        public int LastMarble = 0;
        int currentMarble = 0;
        List<int> MarbleCircle = new();
        Dictionary<int, List<int>> PlayerScores = new();

        void InsertMarble(int value)
        {
            if(MarbleCircle.Count ==0)
                MarbleCircle.Add(0);

            if (value % 23 ==0)
            {
                int player = (value % NumPlayers);
                if (!PlayerScores.ContainsKey(player))
                    PlayerScores[player] = new();
                PlayerScores[player].Add(value);

                currentMarble = ((currentMarble - 7) + MarbleCircle.Count) % MarbleCircle.Count;    // Avoids negative mod
                PlayerScores[player].Add(MarbleCircle[currentMarble]);
                MarbleCircle.RemoveAt(currentMarble);

                if (currentMarble == MarbleCircle.Count)
                    currentMarble = 0;
                return;
            }
            
            var two_away = (currentMarble + 2) % MarbleCircle.Count;
            MarbleCircle.Insert(two_away, value);
            currentMarble = two_away;
            return;
        }

        public int SiumlateGame()
        {
            for (int i = 1; i <= LastMarble; i++)
                InsertMarble(i);

            var scores = PlayerScores.Values.Select(x => x.Sum());
            return scores.Max();
        }
    }

    class CirclePosition
    {
        public long Value = 0;
        public CirclePosition? Left = null;     // using new() causes a stackoverflow :)
        public CirclePosition? Right = null;
    }

    #pragma warning disable CS8602
    class Game
    {
        // Part 2 makes the use of Lists unfeasable, we have to use a faster approach - we will make use of linked lists
        public long NumPlayers;
        public long LastMarble;
        Dictionary<long, long> PlayerScores = new();

        void InsertMarble(long value, ref CirclePosition currentMarble)
        {
            if (value % 23 == 0)
            {
                long player = (value % NumPlayers);
                if (!PlayerScores.ContainsKey(player))
                    PlayerScores[player] = 0;
                PlayerScores[player]+= value;

                var pos = currentMarble;
                for (int i = 0; i < 7; i++)
                    pos = pos.Left;
                
                PlayerScores[player] += pos.Value;
                var left = pos.Left;
                var right = pos.Right;

                right.Left = left;
                left.Right = right;
                currentMarble = right;
                return;
            }

            var one_away = currentMarble.Right;
            var two_away = one_away.Right;
            CirclePosition newMarble = new();
            newMarble.Value = value;
            newMarble.Left = one_away;
            newMarble.Right = two_away;
            one_away.Right = newMarble;
            two_away.Left = newMarble;

            currentMarble = newMarble;
            return;
        }

        public long SiumlateGame()
        {
            CirclePosition currentMarble = new CirclePosition();
            currentMarble.Value = 0;
            currentMarble.Left = currentMarble;
            currentMarble.Right = currentMarble;

            for (long i = 1; i <= LastMarble; i++)
                InsertMarble(i, ref currentMarble);

            var scores = PlayerScores.Values;
            return scores.Max();
        }
    }
    #pragma warning restore CS8602

    internal class MarbleGame
    {
        List<Game> games = new();

        void ParseLine(string line)
        {
            var nums = line.Replace("players; last marble is worth ", "").Replace(" points", "")
                           .Split(" ")
                           .Select(x => long.Parse(x.Trim()))
                           .ToList();

            games.Add(new Game() { NumPlayers = nums[0], LastMarble = nums[1] });
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        long FindWinner(int part = 1)
        {
            if (part == 2)
                games.ForEach(x => x.LastMarble *= 100);

            var winners = games.Select(x => x.SiumlateGame()).ToList();
            return winners[0];
        }

        public long Solve(int part = 1)
            => FindWinner(part);
    }
}
