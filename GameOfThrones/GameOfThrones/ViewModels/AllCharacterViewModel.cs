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
            pageSize = 50;
            pageNum = 1;
            _searchEnabled = true;
            Characters = new ObservableCollection<Character>();
            _allCharacter = new ObservableCollection<Character>();
        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            await LoadCharacters();
        }

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
            catch (HttpRequestException e)
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
            List<Character> result = await DataService.GetCharacterPage(pageNum, pageSize);
            pageNum++;

            //because if only <pageSize was loaded than it was the last of them
            if (result.Count == 0 || result.Count != pageSize)
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
