using System.Text.RegularExpressions;

namespace AoC18.Day24
{
    class ArmyGroup
    {
        public bool IsInfection = true;
        public int Id = 0;

        public int Number;
        public int CurrentNumber;
        public int HitPoints;
        public int Damage;
        public string DamageType = "";
        public int Initiative;
        public List<string> Immunities = new();
        public List<string> Weakneses = new();

        public int EffectivePower
            => CurrentNumber * Damage;
        public bool WeakTo(string type)
            => Weakneses.Contains(type);
        public bool ImmuneTo(string type)
            => Immunities.Contains(type);

        public ArmyGroup Clone()
        {
            ArmyGroup clone = new ArmyGroup();
            clone.IsInfection = IsInfection;
            clone.Id = Id;
            clone.Number = Number;
            clone.CurrentNumber = CurrentNumber;
            clone.HitPoints = HitPoints;
            clone.Damage = Damage;
            clone.DamageType = DamageType;
            clone.Initiative = Initiative;
            clone.Weakneses.AddRange(Weakneses);
            clone.Immunities.AddRange(Immunities);
            return clone;
        }
    }

    internal class BloodBattle
    {
        List<ArmyGroup> Armies = new();

        ArmyGroup ParseLine(string line, bool isInfection, int id)
        {
            string pattern = @"(\d+) units each with (\d+) hit points (\((immune to (,? ?\w+)*)?;?( ?weak to (,? ?\w+)*)?\) )?with an attack that does (\d+) (\w+) damage at initiative (\d+)";
            Regex regex = new Regex(pattern);
            var groups = regex.Match(line).Groups;
            ArmyGroup armyGroup = new();
            armyGroup.Id = id;
            armyGroup.IsInfection = isInfection;
            armyGroup.Number = int.Parse(groups[1].Value);
            armyGroup.CurrentNumber = armyGroup.Number;
            armyGroup.HitPoints = int.Parse(groups[2].Value);
            string immune = groups[4].Value.Trim().Replace("immune to ", "");
            if (!string.IsNullOrEmpty(immune))
                armyGroup.Immunities = immune.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            string weak = groups[6].Value.Trim().Replace("weak to ", "");
            if (!string.IsNullOrEmpty(weak))
                armyGroup.Weakneses = weak.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            armyGroup.Damage = int.Parse(groups[8].Value);
            armyGroup.DamageType = groups[9].Value.Trim();
            armyGroup.Initiative = int.Parse(groups[10].Value);
            return armyGroup;
        }

        public void ParseInput(List<string> input)
        {
            input.Remove("Immune System:");
            input.Remove("Infection:");
            bool inInfection = false;
            int i = 0;

            foreach (var line in input)
                if (string.IsNullOrEmpty(line))
                    inInfection = true;
                else
                    Armies.Add(ParseLine(line, inInfection,i++));
        }

        void CombatTurn()
        {
            var immuneGuys = Armies.Where(x => !x.IsInfection && x.CurrentNumber > 0).ToList();
            var infectGuys = Armies.Where(x =>  x.IsInfection && x.CurrentNumber > 0).ToList();
            List<int> alreadySelectedAsTarget = new();
            Dictionary<int, int> targetSelection = new();

            // Target Selection
            foreach (var army in Armies.OrderByDescending(x => x.EffectivePower)
                                       .ThenByDescending(x => x.Initiative))
            {
                var targets = (army.IsInfection ? immuneGuys : infectGuys).Where(x => !x.ImmuneTo(army.DamageType) 
                                                                                      && !targetSelection.Values.Contains(x.Id));
                if (targets.Count() == 0)   // No target available
                    continue;

                if (targets.Any(x => x.WeakTo(army.DamageType)))
                    targets = targets.Where(x => x.WeakTo(army.DamageType));
                
                var target = targets.OrderByDescending(x => x.EffectivePower).ThenByDescending(x => x.Initiative).First();
                targetSelection[army.Id] = target.Id;
            }

            // Attack phase
            foreach (var army in Armies.OrderByDescending(x => x.Initiative))
            {
                if (army.CurrentNumber <= 0)
                    continue;       // KIA

                if (!targetSelection.ContainsKey(army.Id))
                    continue;

                var target = Armies.First(x => x.Id == targetSelection[army.Id]);
                var Damage = target.WeakTo(army.DamageType) ? 2 * army.EffectivePower : army.EffectivePower;
                var kills = Damage / target.HitPoints;

                target.CurrentNumber -= Math.Min(kills, target.CurrentNumber);
            }
        }

        private bool CheckBoost(int boost)
        {
            List<ArmyGroup> backup = new();
            foreach (var army in Armies)
                backup.Add(army.Clone());

            foreach (var army in Armies.Where(x => !x.IsInfection).ToList())
                army.Damage += boost;

            int battleResult = Fight();

            bool ImmuneWins = (battleResult != -1) &&       // Stalemate
                              Armies.Any(x => !x.IsInfection && x.CurrentNumber > 0);

            var units = Armies.Where(x => !x.IsInfection).Sum(x => x.CurrentNumber);

            if(ImmuneWins)
                Console.WriteLine(boost.ToString() + " , units: " + units.ToString());
            else
                Console.WriteLine("Infection wins");

            Armies.Clear();
            foreach (var army in backup)
                Armies.Add(army.Clone());
            return ImmuneWins;
        }

        private int FindSmallerBoost()  // Binary search
        {
            int lowerBoost = 0;
            int upperBoost = 10000;

            while (lowerBoost < upperBoost - 1)
            {
                Console.WriteLine(lowerBoost.ToString() + " - " + upperBoost.ToString());

                int midBoost = (upperBoost + lowerBoost)/2;
                if (CheckBoost(midBoost))
                    upperBoost = midBoost;
                else
                    lowerBoost = midBoost;
            }
            return upperBoost;
        }

        public int Fight()
        {
            var activeImmuneArmies = Armies.Count(x => !x.IsInfection && x.CurrentNumber > 0);
            var activeInfectionArmies = Armies.Count(x => x.IsInfection && x.CurrentNumber > 0);

            var lastTurnCount = -1;

            while (activeImmuneArmies > 0 && activeInfectionArmies > 0)
            {
                CombatTurn();
                activeImmuneArmies = Armies.Count(x => !x.IsInfection && x.CurrentNumber > 0);
                activeInfectionArmies = Armies.Count(x => x.IsInfection && x.CurrentNumber > 0);
                var currentCount = Armies.Sum(x => x.CurrentNumber);
                if (lastTurnCount == currentCount)
                    return -1;
                lastTurnCount = currentCount;
            }

            return Armies.Sum(x => x.CurrentNumber);
        }

        public int Solve(int part)
            => part == 1 ? Fight() : FindSmallerBoost();
    }
}
