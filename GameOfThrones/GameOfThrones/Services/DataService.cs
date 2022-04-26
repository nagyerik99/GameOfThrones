using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
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

            var result = await DataMappingService.MappReviews(bookDTOs);
            return result;
        }


        public async static Task<(Book, List<string>, List<string>)> GetBookDetails(string ID)
        {
            var bookDTO = await GetAsync<BookDTO>(new Uri(ID));
            var bookBase = await DataMappingService.MappReview(bookDTO);
            return (bookBase, bookDTO.Characters, bookDTO.POVCharacters);
        }

        public async static Task<(Character, List<string>, List<string>)> GetCharacterDetails(string ID)
        {
            CharacterDTO characterDTO = await GetAsync<CharacterDTO>(new Uri(ID));
            Character character = DataMappingService.MappReview(characterDTO);
            List<string> familyUris = new List<string> { characterDTO.Father, characterDTO.Mother, characterDTO.Spouse };

            List<Character> family = await GetCharacterReviews(familyUris);

            character.Father = family[0]; //father
            character.Mother = family[1]; //mother
            character.Spouse = family[2]; //spouse

            return (character, characterDTO.Allegiances, characterDTO.Books);
        }

        public async static Task<List<Character>> GetCharacterReviews(List<string> characterURIs)
        {
            List<Character> characters = new List<Character>();

            foreach (var item in characterURIs)
            {
                Character character;
                if (item != "")
                {
                    character = DataMappingService.MappReview(await GetAsync<CharacterDTO>(new Uri(item)));
                }
                else
                {
                    character = new Character { Name = "Unknown" };
                }
                characters.Add(character);
            }

            return characters;
        }

        public async static Task<List<House>> GetHouseReviews(List<string> houseURIs)
        {
            List<House> houses = new List<House>();

            foreach (var uri in houseURIs)
            {
                House house;
                if (uri != "")
                {
                    house = DataMappingService.MappReview(await GetAsync<HouseDTO>(new Uri(uri)));
                }
                else
                {
                    house = new House { Name = "Unknown" };
                }

                houses.Add(house);
            }

            return houses;
        }

        public async static Task<(House, List<string>, List<string>)> GetHouseDetails(string uri)
        {
            HouseDTO houseDTO = await GetAsync<HouseDTO>(new Uri(uri));
            House houseBase = DataMappingService.MappReview(houseDTO);
            List<string> characterURIs = new List<string>()
            { houseDTO.Founder, houseDTO.CurrentLord, houseDTO.Heir};

            List<Character> characters = await GetCharacterReviews(characterURIs);
            houseBase.Founder = characters[0];//founder
            houseBase.CurrentLord = characters[1];//currentLord
            houseBase.Heir = characters[2];//heir
            houseBase.OverLord = (await GetHouseReviews(new List<string>() { houseDTO.OverLord }))[0];

            return (houseBase, houseDTO.SwornMembers, houseDTO.CadetBranches);
        }

        public async static Task<List<Book>> GetBookReviews(List<string> bookURIs)
        {
            List<BookDTO> bookDTOs = new List<BookDTO>();
            List<Book> books = new List<Book>();

            foreach (var uri in bookURIs)
            {
                bookDTOs.Add(await GetAsync<BookDTO>(new Uri(uri)));
            }

            foreach (var bookDTO in bookDTOs)
            {
                books.Add(await DataMappingService.MappReview(bookDTO));
            }

            return books;
        }

        public async static Task<List<Character>> GetCharacterPage(int pageNum, int PageSize)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(serverUrl, "characters"));
            uriBuilder.Query += $"page={pageNum}";
            uriBuilder.Query += $"&pageSize={PageSize}";

            var characterDTOs = await GetAsync<List<CharacterDTO>>(uriBuilder.Uri);

            return DataMappingService.MappReviews(characterDTOs);
        }

        public async static Task<List<House>> GetHousePage(int pageNum, int PageSize)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(serverUrl, "houses"));
            uriBuilder.Query += $"page={pageNum}";
            uriBuilder.Query += $"&pageSize={PageSize}";

            var houseDTOs = await GetAsync<List<HouseDTO>>(uriBuilder.Uri);

            return DataMappingService.MappReviews(houseDTOs);
        }

    }
}
