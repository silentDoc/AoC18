using System.Text.RegularExpressions;

namespace AoC18.Day16
{
    class OpcodeSample
    {
        public int[] before = new int[4];
        public int[] instruction = new int[4];
        public int[] after = new int[4];

        public OpcodeSample(List<string> lines)
        {
            ParseLine(lines[0], before);
            ParseLine(lines[1], instruction);
            ParseLine(lines[2], after);
        }

        private void ParseLine(string line, int[] element)
        {
            Regex regex = new Regex(@"(\d+) (\d+) (\d+) (\d+)");
            var groups = regex.Match(line.Replace(",","")).Groups;
            element[0] = int.Parse(groups[1].Value);
            element[1] = int.Parse(groups[2].Value);
            element[2] = int.Parse(groups[3].Value);
            element[3] = int.Parse(groups[4].Value);
        }
    }

    class OpcodeCracker
    {
        List<string> operations = new List<string> { "addr", "addi", "mulr", "muli", "banr", "bani", "borr", "bori",
                                                     "setr", "seti", "gtir", "gtri", "gtrr", "eqir", "eqri", "eqrr"};

        List<OpcodeSample> samples = new List<OpcodeSample>();

        bool testOpcode(int[] before, int[] instruction, int[] after, string op)
        {
            int[] result = (int[])before.Clone();
            try
            {
                result[instruction[3]] = op switch
                {
                    "addr" => before[instruction[1]] + before[instruction[2]],
                    "addi" => before[instruction[1]] + instruction[2],
                    "mulr" => before[instruction[1]] * before[instruction[2]],
                    "muli" => before[instruction[1]] * instruction[2],
                    "banr" => before[instruction[1]] & before[instruction[2]],
                    "bani" => before[instruction[1]] & instruction[2],
                    "borr" => before[instruction[1]] | before[instruction[2]],
                    "bori" => before[instruction[1]] | instruction[2],
                    "setr" => before[instruction[1]],
                    "seti" => instruction[1],
                    "gtir" => instruction[1] > before[instruction[2]] ? 1 : 0,
                    "gtri" => before[instruction[1]] >instruction[2] ? 1 : 0,
                    "gtrr" => before[instruction[1]] > before[instruction[2]] ? 1 : 0,
                    "eqir" => instruction[1] == before[instruction[2]] ? 1 : 0,
                    "eqri" => before[instruction[1]] == instruction[2] ? 1 : 0,
                    "eqrr" => before[instruction[1]] == before[instruction[2]] ? 1 : 0,
                    _ => throw new System.Exception("Invalid operation")
                };
            }
            catch(System.IndexOutOfRangeException)  // Control invalid register
            {
                return false;
            }

            return result.SequenceEqual(after);
        }

        public void ParseInput(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("Before"))
                {
                    samples.Add(new OpcodeSample(lines.GetRange(i, 3)));
                    i += 3;
                }
            }
        }

        int SolvePart1()
        {
            int count = 0;
            foreach (var sample in samples)
            {
                var numCompatible = operations.Select(operation => testOpcode(sample.before, sample.instruction, sample.after, operation)).Count(valid => valid);
                if (numCompatible >= 3)
                    count++;
            }
            return count;
        }

        public int Solve(int part)
            => part ==1 ? SolvePart1() : 0;
    }
}
