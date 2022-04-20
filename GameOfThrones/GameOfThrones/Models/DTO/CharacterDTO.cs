using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Models.DTO
{
    public class CharacterDTO
    {
        public string ID { get; private set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Culture { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public List<string> Titles { get; set; }
        public List<string> Aliases { get; set; }
        public string Father { get; set; }
        public string Mother { get; set; }
        public string Spouse { get; set; }
        public List<string> Allegiances { get; set; }
        public List<Book> Books { get; set; }
        public List<Book> POVBooks { get; set; }
    }
}
