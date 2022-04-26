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
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private Character _selectedCharacter;
        private ObservableCollection<House> _allegiances;
        private ObservableCollection<Book> _books;
        private Visibility _allegiancesVisibility = Visibility.Collapsed;
        private Visibility _booksVisibility = Visibility.Collapsed;
        private bool _allegiancesDataLoaded = false;
        private string _allegiancesLoadMoreText = loadMoreText;
        private CharacterDetailsPager Pager;

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
                if (Pager == null || _allegiancesVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Pager.HousePageEmpty;
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
            pageSize = 10;
            Allegiances = new ObservableCollection<House>();
            Books = new ObservableCollection<Book>();
        }


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


        protected override void Loaded()
        {
            base.Loaded();//Isbusy=false;
            ADataLoaded = true;
            ViewLoadingVisibility = Visibility.Visible; //viewLoaded
            AllegiancesVisibility = Visibility.Visible;
            BooksVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));//set visible if have to 
        }

        public async Task LoadAllegiances()
        {
            ADataLoaded = false;
            ALoadMoreText = loadingText;
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
                ALoadMoreText = loadMoreText;
                OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));
            }
        }

        public void ModifyBooksListView(object sender, RoutedEventArgs e)
        {
            BooksVisibility = BooksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ModifyAllegiancesVisibility(object sender, RoutedEventArgs e)
        {
            AllegiancesVisibility = AllegiancesVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreAllegiancesVisibility));
        }

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
            var result = await Pager.GetNext();
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
            Pager = new CharacterDetailsPager(result.Item2, pageSize);
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
