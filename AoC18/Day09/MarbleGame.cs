namespace AoC18.Day09
{
    class Game
    {
        public int NumPlayers;
        public int LastMarble;
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

    internal class MarbleGame
    {
        List<Game> games = new();

        void ParseLine(string line)
        {
            var nums = line.Replace("players; last marble is worth ", "").Replace(" points", "")
                           .Split(" ")
                           .Select(x => int.Parse(x.Trim()))
                           .ToList();

            games.Add(new Game() { NumPlayers = nums[0], LastMarble = nums[1] });
        }

        public void ParseInput(List<string> lines)
            => lines.ForEach(ParseLine);

        int FindWinner(int part = 1)
        { 
            var winners = games.Select(x => x.SiumlateGame()).ToList();
            return winners[0];
        }

        public int Solve(int part = 1)
            => FindWinner(part);
    }
}
