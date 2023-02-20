namespace AoC18.Day01
{
    internal class DeviceCalibrator
    {
        List<int> freqChanges = new();

        public void ParseInput(List<string> lines)
            => freqChanges = lines.Select(x => int.Parse(x.Trim())).ToList();

        int FirstDuplicateFreq()
        { 
            HashSet<int> freqs = new HashSet<int>();
            var current = 0;
            var pos = 0;

            while (freqs.Add(current))
            {
                current += freqChanges[pos % freqChanges.Count];
                pos++;
            }
            return current;
        }

        public int Solve(int part = 1)
            => part == 1 ? freqChanges.Sum() : FirstDuplicateFreq();
    }
}
