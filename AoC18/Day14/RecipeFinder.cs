using System.Text;

namespace AoC18.Day14
{
    class RecipeFinder
    {
        int num = 0;

        public void ParseInput(List<string> lines)
            => num = int.Parse(lines[0]);

        string FindScore(int part = 1)
        {
            StringBuilder sb = new("37");
            int pos1 = 0;
            int pos2 = 1;

            while(sb.Length < num + 10)
            {
                int sum = int.Parse(sb[pos1].ToString()) + int.Parse(sb[pos2].ToString());
                sb.Append(sum.ToString());
                pos1 = (pos1 + int.Parse(sb[pos1].ToString()) + 1) % sb.Length;
                pos2 = (pos2 + int.Parse(sb[pos2].ToString()) + 1) % sb.Length;
            }
            
            StringBuilder result = new();
            for(int i = 0; i < 10; i++)
                result.Append(sb[num + i]);

            return result.ToString();
        }

        public string Solve(int part)
            => FindScore(part);
    }
}
