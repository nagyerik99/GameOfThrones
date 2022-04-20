using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Services
{
    public static class DataMappingService
    {

        public static async Task<List<Book>> MappBookReview(List<BookDTO> bookDTOs)
        {
            List<Book> result = new List<Book>();

            foreach (var bookDTO in bookDTOs)
            {
                result.Add(MappReview(bookDTO));
            }

            return result;
        }

        private static Book MappReview(BookDTO bookDTO)
        {
            return new Book
            {
                ID = bookDTO.ID,
                Name = bookDTO.Name,
                Authors = bookDTO.Authors,
                Publisher = bookDTO.Publisher,
                Released = bookDTO.Released
            };
        }

        //public static List<Book> MappBookSeries(List<BookDTO> bookDTOs)
        //{
        //    List<Book> result;
        //    foreach(var bookDTO in bookDTOs)
        //    {

        //    }
        //}


        //public static async Task<Book> MappBook(BookDTO bookDTO)
        //{
        //    var authors = await MappAuthors(bookDTO.Authors);
        //    var characters = await MappCharacters(bookDTO.Authors);

        //}

        //private static async Task<List<Character>> MappAuthors(List<string> authors)
        //{

        //}

        //private static async Task<List<Character>> MappCharacters(List<string> characters)
        //{

        //}
    }
}
