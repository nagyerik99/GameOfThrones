using System;
using System.Collections.Generic;

namespace GameOfThrones.Models
{
    public class House
    {
        public string ID { get; set;}
        public string Name { get; set; }
        public string Region { get; set; }
        public string CoatOfArms { get; set; }
        public string Words { get; set; }
        public string Titles { get; set; }
        public string Seats { get; set; }
        public Character CurrentLord { get; set; }
        public Character Heir { get; set; }
        public House OverLord { get; set; }
        public string Founded { get; set; }
        public Character Founder { get; set; }
        public string DiedOut { get; set; }
        public string  AncestralWeapons { get; set; }
    }
}
