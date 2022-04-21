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

        public async static Task<List<Book>> GetBookSeries()
        {
            var bookDTOs = await GetAsync<List<BookDTO>>(new Uri(serverUrl, "books"));

            var result = await DataMappingService.MappBookSeriesReview(bookDTOs);
            return result;
        }



        public async static Task<Book> GetBookDetails(string ID)
        {
            var bookDTO = await GetAsync<BookDTO>(new Uri(ID));
            var bookBase = await DataMappingService.MappReview(bookDTO);
            List<CharacterDTO> characterDTOs = new List<CharacterDTO>(bookDTO.Characters.Count);


            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 3;
            Parallel.ForEach(bookDTO.Characters, parallelOptions, async (character) => {
                characterDTOs.Add(await GetCharacterDTOByID(character));
            });
            var characters = DataMappingService.MappCharactersReview(characterDTOs);
            bookBase.Characters = characters;

            characterDTOs.Clear();
            Parallel.ForEach(bookDTO.POVCharacters, parallelOptions, async (character) => {
                characterDTOs.Add(await GetCharacterDTOByID(character));
            });

            var povCharacters = DataMappingService.MappCharactersReview(characterDTOs);
            bookBase.POVCharacters = povCharacters;


            return bookBase;
        }

        private async static Task<CharacterDTO> GetCharacterDTOByID(string ID)
        {
            var characterDTO = await GetAsync<CharacterDTO>(new Uri(ID));
            return characterDTO;
        }

        //public async static Character GetHouses()
        //{

        //}


        //private async Character MapCharacter(CharacterDTO characterDTO)
        //{

        //}

    }
}
