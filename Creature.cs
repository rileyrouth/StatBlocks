using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace StatBlocks
{
    internal class Creature
    {
        public string Name;
        private string Level;
        private string Alignment;
        private string Size;
        private List<string> Traits = new();
        private string Source;
        private string Perception;
        private string Languages;
        private string Skills;
        private string Items;
        private List<string> Abilities = new();

        private string AC;
        private string Fort;
        private string Ref;
        private string Will;

        private string HP;
        private string Immunities;
        private string Resistances;
        private string Weaknesses;

        private string Speed;

        private List<string> PreSpeed = new();
        private List<string> Actions = new();

        public Creature(string[] input)
        {
                int i = 0;
            Name = input[i];
                i++;
            Level = RemoveFirstWord(input[i]);
                i++;
            Traits = new(SeparateTraits(input[i]));
                i++;
            Source = RemoveFirstWord(input[i]);
                i++;
            Perception = RemoveFirstWord(input[i]);
                i++;
            if (input[i].StartsWith("Languages"))
            {
                Languages = RemoveFirstWord(input[i]);
                i++;
            }
            if (input[i].StartsWith("Skills"))
            {
                Skills = RemoveFirstWord(input[i]);
                i++;
            }
            if (SeparateAbilities(input[i])) // If the block has Items
            {
                i++;
                int ACIndex = input[i].IndexOf("AC");
                Items = RemoveFirstWord(input[i].Remove(ACIndex));
            }

            SeparateDCs(input[i][input[i].IndexOf("AC")..]);
            i++;

            SeparateHP(input[i].Split(";"));
            i++;

            for(bool speedCheck = false; i < input.Count(); i++)
            {
                if (input[i].StartsWith("Speed"))
                {
                    Speed = RemoveFirstWord(input[i]);
                    speedCheck = true;
                }
                else if (speedCheck == false)
                {
                    string res = "**";
                    res += input[i]
                        .Replace(":", "**")
                        .Replace(", Damage", ", **Damage**")
                        .Replace("Frequency", "**Frequency**")
                        .Replace("Requirements", "**Requirements**")
                        .Replace("Effect", "**Effect**")
                        .Replace("Trigger", "**Trigger**");
                    PreSpeed.Add(res);
                }
                else
                {
                    string res = "**";
                    res += input[i]
                        .Replace(":", "**")
                        .Replace(", Damage", ", **Damage**")
                        .Replace("Frequency", "**Frequency**")
                        .Replace("Requirements", "**Requirements**")
                        .Replace("Effect", "**Effect**")
                        .Replace("Trigger", "**Trigger**")
                        .Replace("Single Action", "<span class=\"sym\">A</span>")
                        .Replace("Two Actions", "<span class=\"sym\">D</span>")
                        .Replace("Three Actions", "<span class=\"sym\">T</span>")
                        .Replace("Reaction", "<span class=\"sym\">R</span>")
                        .Replace("Free Action", "<span class=\"sym\">F</span>");
                    Actions.Add(res);
                }
            }
        }
        public string RemoveFirstWord(string input)
        {
            return input[(input.Split()[0].Length + 1)..];
        }
        public string[] SeparateTraits(string traits)
        {
            int sizeIndex = -1;
            var sizes = new List<string> { "Tiny", "Small", "Medium", "Large", "Huge", "Gargantuan" };
            
            foreach (string size in sizes)
            {
                if (sizeIndex != -1)
                    break;
                else
                {
                    sizeIndex = traits.IndexOf(size);
                    this.Size = size;
                }
            }

            this.Alignment = traits.Substring(0, sizeIndex);

            traits = traits.Remove(0, Size.Length + Alignment.Length);

            var r = new Regex(@"
                 (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Split(traits);
        }
        public bool SeparateAbilities(string abilityScores)
        {
            bool result = true;
            if (abilityScores.Contains("AC"))
            {
                abilityScores.Remove(46);
                result = false;
            }
            
            abilityScores += ",";

            for (int i = 0; i < 6; i++)
            {
                abilityScores = abilityScores.Remove(0, 4);
                Abilities.Add(abilityScores.Remove(2));
                if (abilityScores.Length > 4)
                {
                    abilityScores = abilityScores.Remove(0, 4);
                }
            }
            return result;
        }
        public void SeparateDCs(string DCs)
        {
            AC = DCs.Substring(0, DCs.IndexOf("Fort") - 2);
            Fort = DCs.Substring(DCs.IndexOf("Fort"), DCs.IndexOf("Ref") - AC.Length - 4);
            Ref = DCs.Substring(DCs.IndexOf("Ref"), DCs.IndexOf("Will") - AC.Length - Fort.Length - 6);
            Will = DCs.Substring(DCs.IndexOf("Will"));

            AC = RemoveFirstWord(AC);
            Fort = RemoveFirstWord(Fort);
            Ref = RemoveFirstWord(Ref);
            Will = RemoveFirstWord(Will);
        }
        public void SeparateHP(string[] HPLine)
        {
            foreach (string item in HPLine)
            {
                string trimmed = item.Trim();
                string category = trimmed.Substring(0, trimmed.IndexOf(" "));
                switch (category)
                {
                    case "HP":
                        HP = RemoveFirstWord(trimmed);
                        break;
                    case "Weaknesses":
                        Weaknesses = RemoveFirstWord(trimmed);
                        break;
                    case "Immunities":
                        Immunities = RemoveFirstWord(trimmed);
                        break;
                    case "Resistances":
                        Resistances = RemoveFirstWord(trimmed);
                        break;
                    case "Hardness":
                        HP += "; " + trimmed;
                        break;
                    default:
                        throw new InvalidDataException("Unrecognised category in HP Line");
                }
            }
        }
        public async Task WriteToFile(string destination)
        {
            var file = new List<string>();

            string[] lines1 =
            {
                $"---",
                $"name: { Name }",
                $"level: { Level }",
                $"alignment: { Alignment }",
                $"size: { Size }",
                $"traits:"
            };

            file.AddRange(lines1);

            foreach (string t in Traits)
            {
                file.Add($"- {t}");
            }

            string[] lines2 =
            {
                $"perception: {Perception}",
                $"languages: {Languages}",
                $"skills: {Skills}",
                $"str: \"{Abilities[0]}\"",
                $"dex: \"{Abilities[1]}\"",
                $"con: \"{Abilities[2]}\"",
                $"int: \"{Abilities[3]}\"",
                $"wis: \"{Abilities[4]}\"",
                $"cha: \"{Abilities[5]}\"",
                $"items: \"{Items}\"",
                $"ac: \"{AC}\"",
                $"fort: \"{Fort}\"",
                $"ref: \"{Ref}\"",
                $"will: \"{Will}\"",
                $"hp: \"{HP}\"",
                $"immunities: \"{Immunities}\"",
                $"weaknesses: \"{Weaknesses}\"",
                $"resistances: \"{Resistances}\"",
                $"---"
            };

            file.AddRange(lines2);

            foreach (string a in PreSpeed)
            {
                file.Add(a);
                file.Add("");
            }

            file.Add($"<hr>");
            file.Add($"**Speed** {Speed}");
            file.Add("");

            foreach (string a in Actions)
            {
                file.Add(a);
                file.Add("");
            }

            await File.WriteAllLinesAsync(destination, file);
        }
    }
}
