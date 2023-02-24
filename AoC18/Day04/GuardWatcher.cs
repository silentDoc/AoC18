namespace AoC18.Day04
{
    class Guard
    {
        public int Id;
        public DateTime BeginShift;
        public List<DateTime> FallAsleep;
        public List<DateTime> WakeUp;

        public Guard(int id, DateTime beginShift)
        {
            Id = id;
            BeginShift = beginShift;
            FallAsleep = new();
            WakeUp = new();
        }

        public int SleepTime
            => WakeUp.Zip(FallAsleep, (w, f) => w.Subtract(f).Minutes).Sum();

        public (int which, int howmuch) MostLikelyMinute()
        {
            if (FallAsleep.Count == 0)
                return (0, 0);

            Dictionary<int, int> sleepMap = new();
            for (int i = 0; i < FallAsleep.Count; i++)
                for (int m = FallAsleep[i].Minute; m < WakeUp[i].Minute; m++)
                {
                    if(!sleepMap.ContainsKey(m))
                        sleepMap[m] = 0;
                    sleepMap[m]++;
                }
            return (sleepMap.Keys.Where(x => sleepMap[x] == sleepMap.Values.Max()).First(), sleepMap.Values.Max());
        }

    }

    internal class GuardWatcher
    {
        List<(DateTime when, string what)> sequence = new();
        List<Guard> guards = new();
        void ParseLine(string line)
        { 
            var parts = line.Split(']');
            var dt = DateTime.Parse(parts[0].Substring(1));
            sequence.Add( (dt, parts[1].Trim()));
        }

        public void ParseInput(List<string> lines)
        {
            lines.ForEach(ParseLine);
            var sortedSeq = sequence.OrderBy(x => x.when).ToList();
            var currentGuardId = -1;
            foreach ( var entry in sortedSeq) 
            {
                var info = entry.what;
                if (info.StartsWith("Guard #"))
                {
                    var strNum = info.Replace("Guard #", "").Replace(" begins shift", "");
                    currentGuardId = int.Parse(strNum);
                    guards.Add(new Guard(currentGuardId, entry.when));
                }
                else
                {
                    var guard = guards.First(x => x.Id == currentGuardId);
                    if (info.StartsWith("falls"))
                        guard.FallAsleep.Add(entry.when);
                    else
                        guard.WakeUp.Add(entry.when);
                }
            }
        }

        int Calculate(int part = 1)
        {
            if (part == 1)
            {
                var maxSleep = guards.Max(x => x.SleepTime);
                var guard = guards.First(x => x.SleepTime == maxSleep);
                return guard.Id * guard.MostLikelyMinute().which;
            }

            var mostSlept = guards.Select(x => x.MostLikelyMinute()).ToList();
            var amount = mostSlept.Max(x => x.howmuch);
            var guard_p2 = guards.First(x => x.MostLikelyMinute().howmuch == amount);

            return guard_p2.Id * guard_p2.MostLikelyMinute().which;
        }

        public int Solve(int part = 1)
            => Calculate(part);
    }
}
