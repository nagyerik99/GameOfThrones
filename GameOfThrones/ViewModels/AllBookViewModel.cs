using GameOfThrones.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GameOfThrones.Services;
using System.Net.Http;

namespace GameOfThrones.ViewModels
{
    public class AllBookViewModel : ViewModelBase
    {
        private ObservableCollection<Book> _bookSeries = new ObservableCollection<Book>();
        private ObservableCollection<Book> _allbookSeries = new ObservableCollection<Book>();

        /// <summary>
        /// The BookSeries provided by the API
        /// </summary>
        public ObservableCollection<Book> BookSeries
        {
            get
            {
                return _bookSeries;
            }
            set
            {
                _bookSeries = value;
                OnPropertyChanged();
            }
        }

        public AllBookViewModel()
        {
            BookSeries = new ObservableCollection<Book>();
        }


        /// <summary>
        /// Calls the <see cref="ViewModelBase"/> functionality and starts the loading of the Bookseries
        /// </summary>
        /// <param name="parameters"></param>
        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);

            await LoadBookSeries();
        }


        /// <summary>
        /// Loads the Bookseries provided by the API
        /// async method because of API access
        /// </summary>
        /// <returns></returns>
        public async Task LoadBookSeries()
        {
            try
            {
                var result = await DataService.GetBookSeries();
                _allbookSeries = new ObservableCollection<Book>(result);
                BookSeries = _allbookSeries;
                Loaded();
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadBookSeries);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadBookSeries);
            }
        }
    }
}
