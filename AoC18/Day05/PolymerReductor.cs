using System.Text;

namespace AoC18.Day05
{
    internal class PolymerReductor
    {
        string polymer = "";

        public void ParseInput(List<string> lines)
            => polymer = lines[0];

        int Reduce(string input)
        {
            var sb = new StringBuilder(input);
            int dif = Math.Abs('A' - 'a');
            var currentPos = 0;

            while(currentPos< sb.Length-1) 
            {
                if (Math.Abs(sb[currentPos] - sb[currentPos + 1]) == dif)
                {
                    sb.Remove(currentPos, 2);
                    currentPos--;
                    if (currentPos < 0)
                        currentPos = 0;
                }
                else
                    currentPos++;
            }
            return sb.Length;
        }

        int Optimize()
        {
            string strChars = "abcdefghijklmnopqrstuvwxyz";
            string polymer_copy = polymer;
            var min_len = int.MaxValue;

            foreach (char c in strChars)
            {
                StringBuilder sb = new(polymer);
                sb = sb.Replace(c.ToString(), "").Replace(char.ToUpper(c).ToString(), "");

                var len = Reduce(sb.ToString());
                min_len = min_len > len ? len : min_len;
            }
            return min_len;
        }

        public int Solve(int part = 1)
            => part == 1 ? Reduce(polymer): Optimize();
    }
}
