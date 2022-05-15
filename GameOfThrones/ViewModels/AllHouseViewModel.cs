using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{
    public class AllHouseViewModel : ViewModelBase
    {
        private ObservableCollection<House> _houses;
        private ObservableCollection<House> _allHouses;
        private Visibility _loadMoreVisibility = Visibility.Collapsed;
        private int _pageNum;
        private bool _loadMoreEnabled = true;

        public bool LoadMoreEnabled
        {
            get
            {
                return _loadMoreEnabled;
            }

            private set
            {
                if (_loadMoreEnabled != value)
                {
                    _loadMoreEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility LoadMoreVisibility
        {
            get
            {
                return _loadMoreVisibility;
            }
            set
            {
                if (_loadMoreVisibility != value)
                {
                    _loadMoreVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<House> Houses
        {
            get
            {
                return _houses;
            }

            private set
            {
                _houses = value;
                OnPropertyChanged();
            }
        }

        public string LoadMoreText
        {
            get
            {
                if (_loadMoreEnabled)
                {
                    return s_LoadMoreText;
                }
                else
                {
                    return s_LoadingText;
                }
            }
        }


        /// <summary>
        /// <see cref="AllHouseViewModel"/> contructor, determines the pageSize for the pager and does the init.
        /// </summary>
        public AllHouseViewModel()
        {
            PageSize = 50;
            _pageNum = 1;
            Houses = new ObservableCollection<House>();
            _allHouses = new ObservableCollection<House>();
        }


        /// <summary>
        /// Calls the parent class Navigated method and starts loading the first page of <see cref="House"/> instance
        /// </summary>
        /// <param name="parameters"></param>
        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);

            await LoadHouses();
        }


        /// <summary>
        /// Loads the next page of <see cref="House"/> instances
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task LoadHouses()
        {
            if (!_loadMoreEnabled)
                return;

            LoadMoreEnabled = false;
            OnPropertyChanged(nameof(LoadMoreText));
            try
            {
                await GetData();
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadHouses);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadHouses);
            }
        }

        private void AddHouses(List<House> newHouses)
        {
            foreach (var house in newHouses)
            {
                _houses.Add(house);
                _allHouses.Add(house);
            }

            OnPropertyChanged(nameof(Houses));
        }

        private async Task GetData()
        {
            List<House> result = await DataService.GetHousePage(_pageNum, PageSize);
            _pageNum++;

            //because if only <pageSize was loaded than it was the last of them
            if (result.Count == 0 || result.Count != PageSize)
            {
                LoadMoreEnabled = false;
                LoadMoreVisibility = Visibility.Collapsed;
            }
            else
            {
                AddHouses(result);
                LoadMoreEnabled = true;
                LoadMoreVisibility = Visibility.Visible;
                OnPropertyChanged(nameof(LoadMoreText));
            }
            Loaded();
        }
    }
}
