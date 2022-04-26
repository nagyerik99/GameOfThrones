using GameOfThrones.Models;
using GameOfThrones.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GameOfThrones.Services
{
    public static class DataMappingService
    {
        private static string ImagePathBase = "ms-appx:///Assets/Images/";
        private static string UnknownCoverImage = ImagePathBase + "unknown_cover.jpg";
        private static string UnknownName = "Unknown";


        public static async Task<List<Book>> MappReviews(List<BookDTO> bookDTOs)
        {
            List<Book> result = new List<Book>();

            foreach (var bookDTO in bookDTOs)
            {
                result.Add(await MappReview(bookDTO));
            }

            return result;
        }

        public static List<Character> MappReviews(List<CharacterDTO> characterDTOs)
        {
            List<Character> characters = new List<Character>();

            foreach (var characterDTO in characterDTOs)
            {
                characters.Add(MappReview(characterDTO));
            }

            return characters.OrderBy(x => x.Name).ToList();
        }

        public static List<House> MappReviews(List<HouseDTO> houseDTOs)
        {
            List<House> houses = new List<House>();

            foreach (var houseDTO in houseDTOs)
            {
                houses.Add(MappReview(houseDTO));
            }

            return houses.OrderBy(x => x.Name).ToList();
        }

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

        public static Character MappReview(CharacterDTO characterDTO)
        {
            return new Character
            {
                ID = characterDTO.Url,
                Name = characterDTO.Name == "" ? UnknownName : characterDTO.Name,
                Aliases = "\"" + string.Join(", ", characterDTO.Aliases) + "\"",
                Born = characterDTO.Born == "" ? "-" : characterDTO.Born,
                Culture = characterDTO.Culture == "" ? "-" : characterDTO.Culture,
                Died = characterDTO.Died == "" ? "-" : characterDTO.Died,
                Titles = "\"" + string.Join(", ", characterDTO.Titles) + "\"",
                Gender = characterDTO.Gender == "" ? "-" : characterDTO.Gender,
            };
        }

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
                Seats = "\"" + string.Join(",", houseDTO.Seats) + "\"",
                Titles = "\"" + string.Join(",", houseDTO.Titles) + "\"",
                Words = houseDTO.Words
            };
        }



        private static async Task<string> GetImagePath(string name)
        {
            string LoweredName = name.ToLower();
            string pathName = LoweredName.Replace(" ", "_") + ".jpg";
            string fullPath = ImagePathBase + pathName;
            try
            {
                await StorageFile.GetFileFromApplicationUriAsync(new Uri(fullPath));
                return fullPath;
            }
            catch
            {
                return UnknownCoverImage;
            }

        }
    }
}
