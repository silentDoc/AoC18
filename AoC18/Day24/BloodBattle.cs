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

        public int Fight()
        {
            var immuneGuys = Armies.Count(x => !x.IsInfection && x.CurrentNumber > 0);
            var infectGuys = Armies.Count(x => x.IsInfection && x.CurrentNumber > 0);

            while (immuneGuys > 0 && infectGuys > 0)
            {
                CombatTurn();
                immuneGuys = Armies.Count(x => !x.IsInfection && x.CurrentNumber > 0);
                infectGuys = Armies.Count(x => x.IsInfection && x.CurrentNumber > 0);
            }

            return Armies.Sum(x => x.CurrentNumber);
        }

        public int Solve(int part)
            => Fight();
    }
}
