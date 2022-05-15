using System;
using System.Collections.Generic;

namespace GameOfThrones.Models.DTO
{
    public class BookDTO
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public List<string> Authors { get; set; }
        public int NumberOfPages { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
        public string MediaType { get; set; }
        public DateTime Released { get; set; }
        public List<string> Characters { get; set; }
        public List<string> POVCharacters { get; set; }

    }
}
