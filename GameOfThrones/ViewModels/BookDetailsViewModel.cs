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
        private string _cLoadMoreText = s_LoadMoreText;
        private string _povLoadMoreText = s_LoadMoreText;
        private BookDetailsPager _characterPager;

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
                if (_characterPager == null || _characterViewVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return _characterPager.IsCharacterListEmpty(Models.Resources.DataType.Character);
                }

            }
        }

        public Visibility LoadMorePOVCharactersVisibility
        {
            get
            {
                if (_characterPager == null || _povCharacterViewVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return _characterPager.IsCharacterListEmpty(Models.Resources.DataType.POVCharacter);
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
            PageSize = 10;
            Characters = new ObservableCollection<Character>();
            PovCharacters = new ObservableCollection<Character>();
        }


        /// <summary>
        /// Calls the base function and try to load the requested book on the details page.
        /// Throws error if the requested page could not be loaded
        /// </summary>
        /// <param name="parameters">parameters sent by the other page/navigationService</param>
        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            var Parameters = parameters as object[];

            if (Parameters[1] == null)
                ErrorService.Instance.ShowErrorMessage(typeof(ErrorService.NavigationException));
            else
            {
                URI = Parameters[1].ToString();
                await LoadBook(URI);
            }
        }


        /// <summary>
        /// Sets the view visible in case the requested data /event is laoded/finished
        /// </summary>
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

        /// <summary>
        /// Loads the next page of characters
        /// Shows error message if the API is not responding, or there are other issues accessing data
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task LoadCharacters()
        {
            CDataLoaded = false;
            CLoadMoreText = s_LoadingText;
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
                CLoadMoreText = s_LoadMoreText;
                OnPropertyChanged(nameof(LoadMoreCharactersVisibility));

            }
        }


        /// <summary>
        /// Loads the Next page of pov Characters
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task LoadPOV()
        {
            POVDataLoaded = false;
            POVLoadMoreText = s_LoadingText;
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
                POVLoadMoreText = s_LoadMoreText;
                OnPropertyChanged(nameof(LoadMorePOVCharactersVisibility));
            }
        }


        /// <summary>
        /// Loads the requested type of character page to the view, using pager
        /// </summary>
        /// <param name="characters">the view element to be loaded into </param>
        /// <param name="type">the type of character <see cref="DataType"/></param>
        /// <returns></returns>
        private async Task LoadDataToList(ObservableCollection<Character> characters, DataType type)
        {
            var result = await _characterPager.GetNext(type);
            foreach (var item in result)
            {
                characters.Add(item);
            }
        }


        /// <summary>
        /// called upon when the ListView Title button is clicked, sets the view visible, or collapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModifyCharacterListView(object sender, RoutedEventArgs e)
        {
            CharactersVisibility = CharactersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreCharactersVisibility));
        }

        /// <summary>
        /// Called upon when the ListView Title button is clicked sets the view visible or collapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            _characterPager = new BookDetailsPager(result.Item2, result.Item3, PageSize);
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
