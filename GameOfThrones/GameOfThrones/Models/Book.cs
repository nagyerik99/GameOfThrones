using System;
using System.Collections.Generic;

namespace GameOfThrones.Models
{
    public class Book
    {
        public string ID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public string Authors { get; set; }
        public int NumOfPages { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
        public string MediaType { get; set; }
        public string Released { get; set; }
        public List<Character> Characters { get; set; }
        public List<Character> POVCharacters { get; set; }
    }
}
