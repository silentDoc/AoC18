using System.Text;

namespace AoC18.Day14
{
    class RecipeMaker
    { 
        List<int> recipes = new() { 3, 7 };

        int pos1 = 0;
        int pos2 = 1;

        public int RecipesToLeft = -1;

        public int Step()
        {
            int n1 = recipes[pos1];
            int n2 = recipes[pos2];

            int sum = n1 + n2;
            if (sum >= 10)
            {
                recipes.Add(1);
                recipes.Add(sum - 10);
            }
            else
                recipes.Add(sum);

            pos1 = (pos1 + n1 + 1) % recipes.Count;
            pos2 = (pos2 + n2 + 1) % recipes.Count;

            return recipes.Count;
        }

        public string GetScore(int num)
        {
            StringBuilder sb = new();
            for (int i = num; i < num + 10; i++)
                sb.Append(recipes[i]);

            return sb.ToString();
        }

        public bool CheckScore(string score)
        {
            int len = score.Length+1;   // Double digits
            if (recipes.Count < len)
                return false;

            StringBuilder sb = new();
            for (int i = recipes.Count - len; i < recipes.Count; i++)
                sb.Append(recipes[i]);

            var result =  sb.ToString().Contains(score);

            if(result)
                RecipesToLeft = recipes.Count - len;
            return result;
        }
    }

    class RecipeFinder
    {
        int num = 0;
        string strNum = "";
        public void ParseInput(List<string> lines)
            => num = int.Parse(strNum = lines[0]);

        string FindScore(int part = 1)
        {
            RecipeMaker maker = new();

            if (part == 1)
            {
                while (maker.Step() < num + 10);
                return maker.GetScore(num).ToString();
            }

            while (!maker.CheckScore(strNum))
                maker.Step();

            return maker.RecipesToLeft.ToString();
        }

        public string Solve(int part)
            => FindScore(part);
    }
}
