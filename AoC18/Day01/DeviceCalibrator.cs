namespace AoC18.Day01
{
    internal class DeviceCalibrator
    {
        List<int> freqChanges = new();

        public void ParseInput(List<string> lines)
            => freqChanges = lines.Select(x => int.Parse(x.Trim())).ToList();

        public int Solve(int part = 1)
            => freqChanges.Sum();
    }
}
