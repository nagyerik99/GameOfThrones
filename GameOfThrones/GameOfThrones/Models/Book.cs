using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace GameOfThrones.Models
{
    public class Book
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public List<string> Authors { get; set; }
        public int NumOfPages { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
        public string MediaType { get; set; }
        public DateTime Released { get; set; }
        public List<Character> Characters { get; set; }
        public List<Character> POVCharacters { get; set; }
    }
}
