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
            _searchEnabled = true;
        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);

            await LoadBookSeries();
        }

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
