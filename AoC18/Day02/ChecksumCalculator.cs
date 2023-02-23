namespace AoC18.Day02
{
    internal class ChecksumCalculator
    {
        int FindOccurrences(string str, int targetNum)
        {
            var counts = str.ToCharArray().Select(x => str.Count(i => i == x)).ToList();
            return counts.Any(x => x == targetNum) ? 1 : 0;
        }

        int CalcCheck(List<string> lines)
            =>  lines.Sum(x => FindOccurrences(x, 2)) * lines.Sum(x => FindOccurrences(x, 3));

        public string Solve(List<string> input, int part)
            => CalcCheck(input).ToString();
    }
}
