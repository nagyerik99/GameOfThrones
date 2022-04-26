using GameOfThrones.Models;
using GameOfThrones.Models.Resources;
using GameOfThrones.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{
    public class BookDetailsViewModel : ViewModelBase
    {
        private Book _selectedBook;
        private ObservableCollection<Character> _characters;
        private ObservableCollection<Character> _povCharacters;
        private Visibility _characterViewVisibility = Visibility.Collapsed;
        private Visibility _povCharacterViewVisibility = Visibility.Collapsed;
        private bool _cdataLoaded = false;
        private bool _povDataLoaded = false;
        private string _cLoadMoreText = loadMoreText;
        private string _povLoadMoreText = loadMoreText;
        private BookDetailsPager characterPager;

        public bool CDataLoaded
        {
            get { return _cdataLoaded; }
            private set
            {
                _cdataLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool POVDataLoaded
        {
            get { return _povDataLoaded; }
            private set
            {
                _povDataLoaded = value;
                OnPropertyChanged();
            }
        }

        public string CLoadMoreText
        {
            get { return _cLoadMoreText; }
            private set
            {
                _cLoadMoreText = value;
                OnPropertyChanged();
            }
        }

        public string POVLoadMoreText
        {
            get { return _povLoadMoreText; }
            private set
            {
                _povLoadMoreText = value;
                OnPropertyChanged();
            }
        }

        public Visibility CharactersVisibility
        {
            get { return _characterViewVisibility; }
            private set
            {
                if (_characterViewVisibility != value)
                {
                    _characterViewVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility POVCharactersVisibility
        {
            get { return _povCharacterViewVisibility; }

            private set
            {
                if (_povCharacterViewVisibility != value)
                {
                    _povCharacterViewVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility LoadMoreCharactersVisibility
        {
            get
            {
                if (characterPager == null || _characterViewVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return characterPager.IsCharacterListEmpty(Models.Resources.DataType.Character);
                }

            }
        }

        public Visibility LoadMorePOVCharactersVisibility
        {
            get
            {
                if (characterPager == null || _povCharacterViewVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return characterPager.IsCharacterListEmpty(Models.Resources.DataType.POVCharacter);
                }

            }
        }

        public Book SelectedBook
        {
            get { return _selectedBook; }
            private set
            {
                _selectedBook = value;
                OnPropertyChanged();
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

        public ObservableCollection<Character> PovCharacters
        {
            get
            {
                return _povCharacters;
            }

            private set
            {
                _povCharacters = value;
                OnPropertyChanged();
            }
        }

        public BookDetailsViewModel()
        {
            pageSize = 10;
            Characters = new ObservableCollection<Character>();
            PovCharacters = new ObservableCollection<Character>();
        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            var Parameters = parameters as object[];

            if (Parameters[1] == null)
                ErrorService.Instance.ShowErrorMessage(typeof(ErrorService.NavigationException));
            else
            {
                URI = Parameters[1].ToString();
                await this.LoadBook(URI);
            }
        }

        protected override void Loaded()
        {
            base.Loaded();//Isbusy=false;
            CDataLoaded = true;
            POVDataLoaded = true;
            ViewLoadingVisibility = Visibility.Visible; //viewLoaded
            CharactersVisibility = Visibility.Visible;
            POVCharactersVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreCharactersVisibility));//set visible if have to 
            OnPropertyChanged(nameof(LoadMorePOVCharactersVisibility));//set visible if have to

        }

        public async Task LoadCharacters()
        {
            CDataLoaded = false;
            CLoadMoreText = loadingText;
            OnPropertyChanged(nameof(LoadMoreCharactersVisibility));
            try
            {
                await LoadDataToList(Characters, DataType.Character);
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacters);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacters);
            }
            finally
            {
                CDataLoaded = true;
                CLoadMoreText = loadMoreText;
                OnPropertyChanged(nameof(LoadMoreCharactersVisibility));

            }
        }

        public async Task LoadPOV()
        {
            POVDataLoaded = false;
            POVLoadMoreText = loadingText;
            try
            {
                await LoadDataToList(PovCharacters, DataType.POVCharacter);
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadPOV);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadPOV);
            }
            finally
            {
                POVDataLoaded = true;
                POVLoadMoreText = loadMoreText;
                OnPropertyChanged(nameof(LoadMorePOVCharactersVisibility));
            }
        }

        public async Task LoadDataToList(ObservableCollection<Character> characters, DataType type)
        {
            var result = await characterPager.GetNext(type);
            foreach (var item in result)
            {
                characters.Add(item);
            }
        }

        public void ModifyCharacterListView(object sender, RoutedEventArgs e)
        {
            CharactersVisibility = CharactersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreCharactersVisibility));
        }

        public void ModifyPOVCharacterListView(object sender, RoutedEventArgs e)
        {
            POVCharactersVisibility = POVCharactersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMorePOVCharactersVisibility));
        }

        private async Task LoadBook(string id)
        {
            try
            {
                await GetData(id);
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadBook);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadBook);
            }
        }

        private async Task GetData(string id)
        {
            var result = await DataService.GetBookDetails(id);
            SelectedBook = result.Item1;
            characterPager = new BookDetailsPager(result.Item2, result.Item3, pageSize);
            await LoadDataToList(Characters, DataType.Character);
            await LoadDataToList(PovCharacters, DataType.POVCharacter);
            Loaded();
        }

        private async Task LoadBook()
        {
            if (URI != null)
                await LoadBook(URI);
        }
    }
}
