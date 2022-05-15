using GameOfThrones.Models;
using GameOfThrones.Models.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.Services
{
    /// <summary>
    /// Service, that used, for paging and loading data, in a given amount/event type.
    /// We use it to load in the Characters of the selected Book, with a more comfortable UI approach
    /// </summary>
    public class BookDetailsPager
    {
        private readonly int _pageNum;
        private readonly int _pageSize;
        private Visibility CharacterPageEmpty = Visibility.Visible;
        private Visibility POVCharacterPageEmpty = Visibility.Visible;
        private List<string> _characterURIs;
        private List<string> _povCharacterURIs;


        /// <summary>
        /// Constructor, creating the pager, with the given lists, and pageSize
        /// </summary>
        /// <param name="characterURIs">The selected book's character URI informations </param>
        /// <param name="povCharacterURIs">The selected book's povCharacter URI information</param>
        /// <param name="pageSize">the amount of data, wanted to be load per request</param>
        public BookDetailsPager(List<string> characterURIs, List<string> povCharacterURIs, int pageSize)
        {
            _characterURIs = characterURIs;
            _povCharacterURIs = povCharacterURIs;
            _pageNum = 0;
            _pageSize = pageSize;
        }


        /// <summary>
        /// <list type="bullet">
        /// <item>Returns the next page of data, with API requests</item>
        /// <item>Using API connection</item>
        /// </list>
        /// </summary>
        /// <param name="type">The Page/Data type we want to load</param>
        /// <returns>Next page of data</returns>
        public async Task<List<Character>> GetNext(DataType type)
        {
            var  characterList = type == DataType.Character ? _characterURIs : _povCharacterURIs;
            var uris = GetNextPageURIs(characterList, type);

            return await DataService.GetCharacterReviews(uris);
        }

        /// <summary>
        /// Returns the next number of URI of the list,
        /// and if it's not optional (we don't have pageSize amount of data)
        /// ,then sets the page empty flag 
        /// </summary>
        /// <returns>The next page of uris</returns>
        private List<string> GetNextPageURIs(List<string> uris, DataType type)
        {
           var result = new List<string>();
            try
            {
                result.AddRange(uris.GetRange(_pageNum, _pageSize));
                uris.RemoveRange(_pageNum, _pageSize);
                return result;
            }
            catch//out of range so we dont have pageSize number of element only fewer so we return it all -->last page
            {
                result.AddRange(uris);
                uris.Clear();
                SetEmpty(type);
                return result;
            }
        }

        private void SetEmpty(DataType type)
        {
            switch (type)
            {
                case DataType.Character:
                    CharacterPageEmpty = Visibility.Collapsed;
                    break;
                case DataType.POVCharacter:
                    POVCharacterPageEmpty = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// Returns if the loadable page is empty, alias we cant load more data 
        /// </summary>
        /// <param name="type">The Type of dataPage</param>
        /// <returns></returns>
        public Visibility IsCharacterListEmpty(DataType type)
        {
            return type == DataType.Character ? CharacterPageEmpty : POVCharacterPageEmpty;
        }
    }
}
