using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameOfThrones.Services
{
    public static class DataMappingService
    {

        private static string ImagePathBase = "ms-appx:///Assets/Images/";
        private static string UnknownCoverImage = ImagePathBase+"unknown_cover.jpg";

        public static async Task<List<Book>> MappBookSeriesReview(List<BookDTO> bookDTOs)
        {
            List<Book> result = new List<Book>();

            foreach (var bookDTO in bookDTOs)
            {
                result.Add(await MappReview(bookDTO));
            }

            return result;
        }

        public static async Task<Book> MappReview(BookDTO bookDTO)
        {
            return new Book
            {
                ID = bookDTO.Url,
                Name = bookDTO.Name,
                Authors = bookDTO.Authors,
                Publisher = bookDTO.Publisher,
                Released = bookDTO.Released,
                Path = await GetImagePath(bookDTO.Name)
            };
        }

        public static List<Character> MappCharactersReview(List<CharacterDTO> characterDTOs)
        {
            List<Character> characters = new List<Character>();

            foreach (var characterDTO in characterDTOs)
            {
                characters.Add(MappCharacterReview(characterDTO));
            }

            return characters.OrderBy(x=>x.Name).ToList();
        }

        public static Character MappCharacterReview(CharacterDTO characterDTO)
        {
            return new Character
            {
                ID = characterDTO.Url,
                Name = characterDTO.Name,
                Aliases = characterDTO.Aliases,
            };
        }

        private static async Task<string> GetImagePath(string name)
        {
            string LoweredName = name.ToLower();
            string pathName = LoweredName.Replace(" ", "_")+".jpg";
            string fullPath = ImagePathBase + pathName;
            try
            {
                await StorageFile.GetFileFromApplicationUriAsync(new Uri(fullPath));
                return fullPath;
            }catch(Exception)
            {
                return UnknownCoverImage;
            }
            
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
