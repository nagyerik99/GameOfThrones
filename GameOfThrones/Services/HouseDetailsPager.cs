using GameOfThrones.Models;
using GameOfThrones.Models.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.Services
{
    /// <summary>
    /// Service, that used, for paging and loading data, in a given amount/event type.
    /// We use it to load in the Cadets and Members of the selected <see cref="House"/> instance.
    /// </summary>
    public class HouseDetailsPager
    {
        private List<string> _cadetURIs;
        private List<string> _memberURIs;
        public Visibility CadetBranchesEmpty = Visibility.Visible;
        public Visibility MembersEmpty = Visibility.Visible;
        private int _pageSize;
        private int _pageNum = 0;


        /// <summary>
        /// Constructor of the pager
        /// </summary>
        /// <param name="memberURIs">list of Member ids/uris</param>
        /// <param name="cadetURIs">list of Cadet ids/uris</param>
        /// <param name="pageSize">the amount of data to be loaded on event per request</param>
        public HouseDetailsPager(List<string> memberURIs, List<string> cadetURIs, int pageSize)
        {
            _cadetURIs = cadetURIs;
            _memberURIs = memberURIs;
            _pageSize = pageSize;
            _pageNum = 0;
        }


        /// <summary>
        /// Gets the next page of members
        /// </summary>
        /// <returns>Returns the next page of members</returns>
        public async Task<List<Character>> GetNextMembers()
        {
            var uris = GetNextPageURIs(DataType.Character);
            List<Character> result = await DataService.GetCharacterReviews(uris);
            return result;
        }

        /// <summary>
        /// Gets the next page of Cadets of the SelectedHosue
        /// </summary>
        /// <returns>returns the cadets on the next page Cadets</returns>
        public async Task<List<House>> GetNextCadets()
        {
            var uris = GetNextPageURIs(DataType.House);
            List<House> result = await DataService.GetHouseReviews(uris);
            return result;
        }

        private List<string> GetNextPageURIs(DataType type)
        {
            var result = new List<string>();
            var uris = type == DataType.Character ? _memberURIs : _cadetURIs;

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
                    MembersEmpty = Visibility.Collapsed;
                    break;
                case DataType.House:
                    CadetBranchesEmpty = Visibility.Collapsed;
                    break;
            }
        }

    }
}
