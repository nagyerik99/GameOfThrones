using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace GameOfThrones.Services
{
    public static class DataService
    {
        private static readonly Uri serverUrl = new Uri("https://anapioficeandfire.com/api/");

        private static async Task<T> GetAsync<T>(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }



        //public async static List<Character> GetCharacters()
        //{

        //}

        //public async static Character GetCharacterByID(string ID)
        //{

        //}

        public async static Task<List<Book>> GetBookSeries()
        {
            var bookDTOs = await GetAsync<List<BookDTO>>(new Uri(serverUrl, "books"));

            var result = await DataMappingService.MappBookReview(bookDTOs);
            return result;
        }

        //public async static Character GetHouses()
        //{

        //}


        //private async Character MapCharacter(CharacterDTO characterDTO)
        //{

        //}

    }
}
