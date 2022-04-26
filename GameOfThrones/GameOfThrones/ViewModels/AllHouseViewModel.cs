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
        private int pageNum;
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
                    return loadMoreText;
                }
                else
                {
                    return loadingText;
                }
            }
        }

        public AllHouseViewModel()
        {
            pageSize = 50;
            pageNum = 1;
            _searchEnabled = true;
            Houses = new ObservableCollection<House>();
            _allHouses = new ObservableCollection<House>();
        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);

            await LoadHouses();
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

        private async Task GetData()
        {
            List<House> result = await DataService.GetHousePage(pageNum, pageSize);
            pageNum++;

            //because if only <pageSize was loaded than it was the last of them
            if (result.Count == 0 || result.Count != pageSize)
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
