using GameOfThrones.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.Services
{
    /// <summary>
    /// Service, that used, for paging and loading data, in a given amount/event type.
    /// We use it to load the Houses/Allegiances of the selected Character, with a more comfortable UI approach
    /// </summary>
    public class CharacterDetailsPager
    {
        private List<string> _houseURIs;
        private int _pageSize;
        private int _pageNum;
        public Visibility HousePageEmpty = Visibility.Visible;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="houseURIs"></param>
        /// <param name="pageSize"></param>
        public CharacterDetailsPager(List<string> houseURIs, int pageSize)
        {
            _houseURIs = houseURIs;
            _pageSize = pageSize;
            _pageNum = 0;
        }

        /// <summary>
        /// Return the next amount/pageSize of data, if it is available.
        /// Using API connection
        /// </summary>
        /// <returns>The next List of Houses to be shown/loaded</returns>
        public async Task<List<House>> GetNext()
        {
            var uris = GetNextPageURIs();

            return await DataService.GetHouseReviews(uris);
        }

        /// <summary>
        /// Returns the next number of URI of the list,
        /// and if it's not optional (we don't have pageSize amount of data)
        /// ,then sets the HousePageEmpty flag 
        /// </summary>
        /// <returns>The next page of uris</returns>
        private List<string> GetNextPageURIs()
        {
            var result = new List<string>();
            try
            {
                result.AddRange(_houseURIs.GetRange(_pageNum, _pageSize));
                _houseURIs.RemoveRange(_pageNum, _pageSize);
                return result;
            }
            catch//out of range so we dont have pageSize number of element only fewer so we return it all -->last page
            {
                result.AddRange(_houseURIs);
                _houseURIs.Clear();
                SetEmpty();
                return result;
            }
        }

        private void SetEmpty()
        {
            HousePageEmpty = Visibility.Collapsed;
        }
    }
}
