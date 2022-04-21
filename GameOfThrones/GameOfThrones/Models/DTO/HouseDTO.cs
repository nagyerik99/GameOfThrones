using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Models.DTO
{
    public class HouseDTO
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string CoatOfArms { get; set; }
        public string Words { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Seats { get; set; }
        public string CurrentLord { get; set; }
        public string Heir { get; set; }
        public string OverLord { get; set; }
        public string Founded { get; set; }
        public string Founder { get; set; }
        public string DiedOut { get; set; }
        public List<string> AncestralWeapons { get; set; }
        List<string> CadetBranches { get; set; }
        List<string> SwornMembers { get; set; }

    }
}
