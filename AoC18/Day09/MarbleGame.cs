namespace AoC18.Day09
{
    class CirclePosition
    {
        public long Value = 0;
        public CirclePosition? Left = null;    
        public CirclePosition? Right = null;
    }

    #pragma warning disable CS8602
    class Game
    {
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
