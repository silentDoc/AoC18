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

        int Hamming(string str1, string str2)
            => str1.ToCharArray().Zip(str2.ToCharArray(), (x, y) => x == y).Count(r => r == false);

        string FindCommonLetters(List<string> lines)
        {
            foreach (var line in lines)
            {
                var rest = lines.Skip(lines.IndexOf(line) + 1).ToList();
                var hamms = rest.Select(x => Hamming(line, x)).ToList();
                if (hamms.IndexOf(1) != -1)
                {
                    var comp = rest[hamms.IndexOf(1)];
                    var range = Enumerable.Range(0, comp.Length);
                    return string.Join("", range.Select(x => comp[x] == line[x] ? comp[x].ToString() : ""));
                }
            }
            return "";
        }

        public string Solve(List<string> input, int part)
            => part == 1 ? CalcCheck(input).ToString() : FindCommonLetters(input);
    }
}
