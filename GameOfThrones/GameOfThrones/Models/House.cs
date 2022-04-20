using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Models
{
    public class House
    {
        public string ID { get; private set;}
        public string Name { get; set; }
        public string Region { get; set; }
        public string CoatOfArms { get; set; }
        public string Words { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Seats { get; set; }
        public Character CurrentLord { get; set; }
        public Character Heir { get; set; }
        public House OverLord { get; set; }
        public string Founded { get; set; }
        public Character Founder { get; set; }
        public string DiedOut { get; set; }
        public List<string> AncestralWeapons { get; set; }
        List<House> CadetBranches { get; set; }
        List<Character> SwornMembers { get; set; }
    }
}
