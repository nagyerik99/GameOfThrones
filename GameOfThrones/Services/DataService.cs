using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace GameOfThrones.Services
{
    /// <summary>
    /// <list type="bullet">
    /// <item> Responsible for requesting, the neccessary datas from the API server.</item>
    /// <item>Static class</item>
    /// </list>
    /// </summary>
    public static class DataService
    {
        private static readonly Uri _serverUrl = new Uri("https://anapioficeandfire.com/api/");

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

        /// <summary>
        /// Loading the Bookseries determined, by the API
        /// </summary>
        /// <returns>The List of <see cref="Book"/> instances</returns>
        public async static Task<List<Book>> GetBookSeries()
        {
            var bookDTOs = await GetAsync<List<BookDTO>>(new Uri(_serverUrl, "books?page=1&pageSize=20"));

            var result = await DataMappingService.MappReviews(bookDTOs);
            return result;
        }


        /// <summary>
        /// <para>
        /// <list type="bullet">
        /// <item>Returns the <see cref="Book"/> instance of the given <paramref name="ID"/> if has one.</item>
        /// <item>Also returns the ID-s/URI-s of the book's characters and povCharacters, for future paging purposes</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async static Task<(Book, List<string>, List<string>)> GetBookDetails(string ID)
        {
            var bookDTO = await GetAsync<BookDTO>(new Uri(ID));
            var bookBase = await DataMappingService.MappReview(bookDTO);
            return (bookBase, bookDTO.Characters, bookDTO.POVCharacters);
        }

        /// <summary>
        /// <para>
        /// <list type="bullet">
        /// <item>Returns the <see cref="Character"/> instance of the given <paramref name="ID"/> if has one.</item>
        /// <item>Also returns the ID-s/URI-s of the characters's allegiances (<see cref="HouseDTO"/> URI-s) 
        /// and books which mentiones the character, for future paging purposes</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>The character and the data associated with it</returns>
        public async static Task<(Character, List<string>, List<string>)> GetCharacterDetails(string ID)
        {
            CharacterDTO characterDTO = await GetAsync<CharacterDTO>(new Uri(ID));
            Character character = DataMappingService.MappReview(characterDTO,true);
            List<string> familyUris = new List<string> { characterDTO.Father, characterDTO.Mother, characterDTO.Spouse };

            List<Character> family = await GetCharacterReviews(familyUris);

            character.Father = family[0]; //father
            character.Mother = family[1]; //mother
            character.Spouse = family[2]; //spouse

            return (character, characterDTO.Allegiances, characterDTO.Books);
        }


        /// <summary>
        /// Returns the <see cref="Character"/>'s associated with the requested URIs
        /// </summary>
        /// <param name="characterURIs">the requested <see cref="Character"/>'s URIs</param>
        /// <returns>the list of <see cref="Character"/></returns>
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

        /// <summary>
        /// Returns the <see cref="House"/>s associated with the requested URIs
        /// </summary>
        /// <param name="houseURIs">the requested URIs</param>
        /// <returns>The Requested List of <see cref="House"/>s</returns>
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


        /// <summary>
        /// <para>
        /// <list type="bullet">
        /// <item>Returns the <see cref="House"/> instance of the given <paramref name="uri"/> if has one.</item>
        /// <item>Also returns the ID-s/URI-s of the house members (<see cref="Character"/> URI-s) 
        /// and <see cref="House"/> URIs as cadetBranches, for future paging purposes</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return a List of <see cref="Book"/> isntances, associated with <paramref name="bookURIs"/>
        /// </summary>
        /// <param name="bookURIs">the requested Book ID's</param>
        /// <returns>The list of <see cref="Book"/> result</returns>
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


        /// <summary>
        /// Returns the next page of Characters.
        /// </summary>
        /// <param name="pageNum">the requested page</param>
        /// <param name="PageSize">the requested amount of Characters on the page</param>
        /// <returns>the next page in the form of collection of <see cref="Character"/></returns>
        public async static Task<List<Character>> GetCharacterPage(int pageNum, int PageSize)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(_serverUrl, "characters"));
            uriBuilder.Query += $"page={pageNum}";
            uriBuilder.Query += $"&pageSize={PageSize}";

            var characterDTOs = await GetAsync<List<CharacterDTO>>(uriBuilder.Uri);

            return DataMappingService.MappReviews(characterDTOs);
        }

        /// <summary>
        /// Returns the next page of Houses
        /// </summary>
        /// <param name="pageNum">the next requested page</param>
        /// <param name="PageSize">the amount of data of the requested page</param>
        /// <returns>The requested Page, in the form of <see cref="House"/> collection</returns>
        public async static Task<List<House>> GetHousePage(int pageNum, int PageSize)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(_serverUrl, "houses"));
            uriBuilder.Query += $"page={pageNum}";
            uriBuilder.Query += $"&pageSize={PageSize}";

            var houseDTOs = await GetAsync<List<HouseDTO>>(uriBuilder.Uri);

            return DataMappingService.MappReviews(houseDTOs);
        }

    }
}
