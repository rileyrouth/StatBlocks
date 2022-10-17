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
        public Trait Rarity;
        private Trait Alignment;
        private Trait Size;
        private List<Trait> Traits = new();
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

        public Creature(List<string> input)
        {
            Neaten(input);
            foreach (var item in input)
            {
                Console.WriteLine(item);
            }
        }
        public void Neaten(List<string> input)
        {
            string ACLine = "";
            int ACIndex = 0;
            string ACPreLine = "";
            int ACPreIndex = 0;
            foreach (string s in input)
            {
                if (s.Contains("; Fort"))
                {
                    ACIndex = s.IndexOf("AC");
                    ACLine = s[ACIndex..];
                    ACPreLine = s.Remove(ACIndex);
                    ACPreIndex = input.IndexOf(s);
                    break;
                }
            }
            input.Add(ACLine);
            input[ACPreIndex] = ACPreLine;
            foreach (string s in input)
            {
                Console.WriteLine(s);
            }
        }
    }
}
