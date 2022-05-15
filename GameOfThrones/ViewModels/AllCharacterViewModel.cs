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
    public class AllCharacterViewModel : ViewModelBase
    {
        private ObservableCollection<Character> _characters;
        private ObservableCollection<Character> _allCharacter;
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

        public ObservableCollection<Character> Characters
        {
            get
            {
                return _characters;
            }

            private set
            {
                _characters = value;
                OnPropertyChanged();
            }
        }


        public AllCharacterViewModel()
        {
            PageSize = 50;
            _pageNum = 1;
            Characters = new ObservableCollection<Character>();
            _allCharacter = new ObservableCollection<Character>();
        }


        /// <summary>
        /// Calls the <see cref="ViewModelBase"/> Navigated function and starts the Laoding of the <see cref="Character"/>s
        /// </summary>
        /// <param name="parameters"></param>
        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            await LoadCharacters();
        }

        /// <summary>
        /// Loads the next page of characters
        /// </summary>
        /// <returns></returns>
        public async Task LoadCharacters()
        {
            if (!_loadMoreEnabled)
                return;

            LoadMoreEnabled = false;
            OnPropertyChanged(nameof(LoadMoreText));
            try
            {
                await GetData(); // Loading Characters to List
            }
            catch (HttpRequestException e)//there is no connection, or file not found
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacters);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacters);
            }
        }


        private async Task GetData()
        {
            List<Character> result = await DataService.GetCharacterPage(_pageNum, PageSize);
            _pageNum++;

            //because if only <pageSize was loaded than it was the last of them
            if (result.Count == 0 || result.Count != PageSize)
            {
                LoadMoreVisibility = Visibility.Collapsed;
                LoadMoreEnabled = false;
            }
            else
            {
                AddCharacters(result);
                LoadMoreEnabled = true;
                LoadMoreVisibility = Visibility.Visible;
                OnPropertyChanged(nameof(LoadMoreText));
            }
            Loaded();
        }

        private void AddCharacters(List<Character> newCharacters)
        {
            foreach (var item in newCharacters)
            {
                _characters.Add(item);
                _allCharacter.Add(item);
            }

            OnPropertyChanged(nameof(Characters));
        }
    }
}
