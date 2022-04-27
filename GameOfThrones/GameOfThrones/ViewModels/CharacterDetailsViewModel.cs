using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static GameOfThrones.Services.ErrorService;

namespace GameOfThrones.ViewModels
{

    /// <summary>
    /// Viewmodel for the characterDetailsView, containing the business logic
    /// </summary>
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private Character _selectedCharacter;
        private ObservableCollection<House> _allegiances;
        private ObservableCollection<Book> _books;
        private Visibility _allegiancesVisibility = Visibility.Collapsed;
        private Visibility _booksVisibility = Visibility.Collapsed;
        private bool _allegiancesDataLoaded = false;
        private string _allegiancesLoadMoreText = s_LoadMoreText;
        private CharacterDetailsPager _pager;

        public bool ADataLoaded
        {
            get { return _allegiancesDataLoaded; }
            private set
            {
                _allegiancesDataLoaded = value;
                OnPropertyChanged();
            }
        }

        public string ALoadMoreText
        {
            get { return _allegiancesLoadMoreText; }
            private set
            {
                _allegiancesLoadMoreText = value;
                OnPropertyChanged();
            }
        }

        public Visibility AllegiancesVisibility
        {
            get { return _allegiancesVisibility; }
            private set
            {
                if (_allegiancesVisibility != value)
                {
                    _allegiancesVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility BooksVisibility
        {
            get { return _booksVisibility; }

            private set
            {
                if (_booksVisibility != value)
                {
                    _booksVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility LoadMoreAllegiancesVisibility
        {
            get
            {
                if (_pager == null || _allegiancesVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return _pager.HousePageEmpty;
                }

            }
        }

        public Character SelectedCharacter
        {
            get { return _selectedCharacter; }
            private set
            {
                _selectedCharacter = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<House> Allegiances
        {
            get
            {
                return _allegiances;
            }

            private set
            {
                _allegiances = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Book> Books
        {
            get
            {
                return _books;
            }

            private set
            {
                _books = value;
                OnPropertyChanged();
            }
        }

        public CharacterDetailsViewModel()
        {
            PageSize = 10;
            Allegiances = new ObservableCollection<House>();
            Books = new ObservableCollection<Book>();
        }

        /// <summary>
        /// Called when the page is navigatedTo, calls the base function and 
        /// Start to load the given CharacterDetails.
        /// Throws error if can't load 
        /// </summary>
        /// <param name="parameters"></param>
        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            var Parameters = parameters as object[];

            if (Parameters[1] == null)
                ErrorService.Instance.ShowErrorMessage(typeof(NavigationException));
            else
            {
                URI = Parameters[1].ToString();
                await this.LoadCharacter(URI);
            }
        }


        /// <summary>
        /// 
        /// Sets the visibility and the view availeable.
        /// Should be Called when the requested functionality6event loaded/finished
        /// </summary>
        protected override void Loaded()
        {
            base.Loaded();//Isbusy=false;
            ADataLoaded = true;
            ViewLoadingVisibility = Visibility.Visible; //viewLoaded
            AllegiancesVisibility = Visibility.Visible;
            BooksVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));//set visible if have to 
        }


        /// <summary>
        /// Loads the next page of <see cref="House"/> instances to the Allegiances
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllegiances()
        {
            ADataLoaded = false;
            ALoadMoreText = s_LoadingText;
            OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));

            try
            {
                await LoadNextAllegiances();
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadAllegiances);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadAllegiances);
            }
            finally
            {
                ADataLoaded = true;
                ALoadMoreText = s_LoadMoreText;
                OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));
            }
        }

        /// <summary>
        /// Sets the BookList visible or collapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModifyBooksListView(object sender, RoutedEventArgs e)
        {
            BooksVisibility = BooksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Sets the Allegiances list visible, or collapsed
        /// </summary>
        /// <param name="sender"> the sender</param>
        /// <param name="e">event arguments</param>
        public void ModifyAllegiancesVisibility(object sender, RoutedEventArgs e)
        {
            AllegiancesVisibility = AllegiancesVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));
        }

        /// <summary>
        /// Loads the next page of <see cref="Book"/> instances into <see cref="Books"/>
        /// </summary>
        /// <param name="bookURIs">the books that shoudl be loaded</param>
        /// <returns></returns>
        private async Task LoadBooks(List<string> bookURIs)
        {
            var result = await DataService.GetBookReviews(bookURIs);

            foreach (Book book in result)
            {
                Books.Add(book);
            }
        }

        private async Task LoadNextAllegiances()
        {
            var result = await _pager.GetNext();
            foreach (var item in result)
            {
                Allegiances.Add(item);
            }
        }
        private async Task LoadCharacter(string id)
        {
            try
            {
                await GetData(id);
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacter);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadCharacter);
            }
        }

        private async Task GetData(string id)
        {
            var result = await DataService.GetCharacterDetails(id);
            SelectedCharacter = result.Item1;
            await LoadBooks(result.Item3);
            _pager = new CharacterDetailsPager(result.Item2, PageSize);
            await LoadAllegiances();
            Loaded();
            //Load character and books and Data

        }

        private async Task LoadCharacter()
        {
            if (URI != null)
                await LoadCharacter(URI);
        }
    }
}
