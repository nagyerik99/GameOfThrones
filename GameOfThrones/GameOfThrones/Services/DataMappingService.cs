using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameOfThrones.Services
{
    /// <summary>
    /// <para>Responsible for mapping the DTO obejct(s) (served by the API), to the required modelType, used by the application</para>
    /// </summary>
    public static class DataMappingService
    {
        private static string s_imagePathBase = "ms-appx:///Assets/Images/";
        private static string s_unknownCoverImage = s_imagePathBase + "unknown_cover.jpg";
        private static string _unknownName = "Unknown";

        /// <summary>
        /// Mapps a list of Data Transfer Object (DTO) to a List of book
        /// </summary>
        /// <param name="bookDTOs"></param>
        /// <returns>List of Books that corresponds with the type <see cref="Book"/> </returns>
        public static async Task<List<Book>> MappReviews(List<BookDTO> bookDTOs)
        {
            List<Book> result = new List<Book>();

            foreach (var bookDTO in bookDTOs)
            {
                result.Add(await MappReview(bookDTO));
            }

            return result;
        }

        /// <summary>
        /// Mapps a List of <see cref="CharacterDTO"/> to a List of <see cref="Character"/>
        /// </summary>
        /// <param name="characterDTOs">the data served by the API</param>
        /// <returns>the list of <see cref="Character"/> instances</returns>
        public static List<Character> MappReviews(List<CharacterDTO> characterDTOs)
        {
            List<Character> characters = new List<Character>();

            foreach (var characterDTO in characterDTOs)
            {
                characters.Add(MappReview(characterDTO));
            }

            return characters.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Mapps a List of <see cref="HouseDTO"/> to a List of <see cref="House"/>
        /// </summary>
        /// <param name="houseDTOs">the data served by the API</param>
        /// <returns>the List of <see cref="Book"/> instances</returns>
        public static List<House> MappReviews(List<HouseDTO> houseDTOs)
        {
            List<House> houses = new List<House>();

            foreach (var houseDTO in houseDTOs)
            {
                houses.Add(MappReview(houseDTO));
            }

            return houses.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Maps one instance of <see cref="BookDTO"/> to <see cref="Book"/> type instance
        /// </summary>
        /// <param name="bookDTO">the <see cref="BookDTO" instance/></param>
        /// <returns>the <see cref="Book"/> instance</returns>
        public static async Task<Book> MappReview(BookDTO bookDTO)
        {
            return new Book
            {
                ID = bookDTO.Url,
                Name = bookDTO.Name,
                Authors = string.Join(",", bookDTO.Authors),
                Publisher = bookDTO.Publisher,
                Released = bookDTO.Released.ToString("yyyy.MM.dd"),
                Path = await GetImagePath(bookDTO.Name),
                Country = bookDTO.Country,
                ISBN = bookDTO.ISBN,
                MediaType = bookDTO.MediaType,
                NumOfPages = bookDTO.NumberOfPages,
            };
        }

        /// <summary>
        /// Maps one instance of <see cref="CharacterDTO"/> to <see cref="Character"/>.
        /// <paramref name="forDetails"/> is used to Determine if the instance is used for Review, or more
        /// Detailed purposes, like Detailed view
        /// </summary>
        /// <param name="characterDTO">the <see cref="CharacterDTO"/>instance</param>
        /// <param name="forDetails">the flag</param>
        /// <returns>the <see cref="House"/> instance</returns>
        public static Character MappReview(CharacterDTO characterDTO, bool forDetails=false)
        {
            return new Character
            {
                ID = characterDTO.Url,
                Name = characterDTO.Name == "" ? _unknownName : characterDTO.Name,
                Aliases = forDetails?"\"" + string.Join(", ", characterDTO.Aliases) + "\"":
                                    "\"" + string.Join(", ", characterDTO.Aliases.Take(3)) + "\"",
                Born = characterDTO.Born == "" ? "-" : characterDTO.Born,
                Culture = characterDTO.Culture == "" ? "-" : characterDTO.Culture,
                Died = characterDTO.Died == "" ? "-" : characterDTO.Died,
                Titles = "\"" + string.Join(", ", characterDTO.Titles) + "\"",
                Gender = characterDTO.Gender == "" ? "-" : characterDTO.Gender,
            };
        }


        /// <summary>
        /// Maps an instance of <see cref="HouseDTO"/> to and instance of <see cref="House"/>
        /// </summary>
        /// <param name="houseDTO">the <see cref="HouseDTO"/></param>
        /// <returns>the isntance of <see cref="House"/></returns>
        public static House MappReview(HouseDTO houseDTO)
        {
            return new House
            {
                ID = houseDTO.Url,
                Name = houseDTO.Name,
                Region = houseDTO.Region == "" ? "-" : houseDTO.Region,
                AncestralWeapons = "\"" + string.Join(",", houseDTO.AncestralWeapons) + "\"",
                CoatOfArms = houseDTO.CoatOfArms == "" ? "-" : houseDTO.CoatOfArms,
                DiedOut = houseDTO.DiedOut == "" ? "-" : houseDTO.DiedOut,
                Founded = houseDTO.Founded == "" ? "-" : houseDTO.Founded,
                Seats = "\"" + string.Join(", ", houseDTO.Seats) + "\"",
                Titles = "\"" + string.Join(", ", houseDTO.Titles) + "\"",
                Words = "\"" + houseDTO.Words + "\""
            };
        }



        private static async Task<string> GetImagePath(string name)
        {
            string LoweredName = name.ToLower();
            string pathName = LoweredName.Replace(" ", "_") + ".jpg";
            string fullPath = s_imagePathBase + pathName;
            try
            {
                await StorageFile.GetFileFromApplicationUriAsync(new Uri(fullPath));
                return fullPath;
            }
            catch
            {
                return s_unknownCoverImage;
            }

        }
    }
}
