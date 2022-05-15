using System;
using System.Collections.Generic;

namespace GameOfThrones.Models
{
    public class Character
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Culture { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public string Titles { get; set; }
        public string Aliases { get; set; }
        public Character Father { get; set; }
        public Character Mother { get; set; }
        public Character Spouse { get; set; }
        public List<House> Allegiances { get; set; }
        public List<Book> Books { get; set; }
        public List<Book> POVBooks { get; set; }
    }
}
