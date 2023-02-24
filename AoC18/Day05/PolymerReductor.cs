using MoreLinq;
using System.Text;

namespace AoC18.Day05
{
    internal class PolymerReductor
    {
        string polymer = "";

        public void ParseInput(List<string> lines)
            => polymer = lines[0];

        IList<char>? FindMolecule(string polymer)
            => polymer.Window(2).FirstOrDefault(x => ((char.IsUpper(x[0]) && char.IsLower(x[1])) || (char.IsUpper(x[1]) && char.IsLower(x[0])))
                                                     && char.ToUpper(x[0]) == char.ToUpper(x[1]));

        int Reduce(int part = 1)
        {
            var sb = new StringBuilder(polymer);
            // Perfect time to start playing with MoreLinQ library - windows !!!:)
            var occurrence = FindMolecule(sb.ToString());

            while (occurrence != null)
            {
                string molecule = new string(occurrence.ToArray());
                int index = sb.ToString().IndexOf(molecule);
                sb.Remove(index, 2);
                occurrence = FindMolecule(sb.ToString());
            }
            return sb.Length;
        }

        public int Solve(int part = 1)
            => Reduce(part);
    }
}
