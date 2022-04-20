using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Models
{
    public class Character
    {
        public string ID { get; private set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Culture { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Aliases { get; set; }
        public Character Father { get; set; }
        public Character Mother { get; set; }
        public Character Spouse { get; set; }
        public List<string> Allegiances { get; set; }
        public List<Book> Books { get; set; }
        public List<Book> POVBooks { get; set; }
    }
}
