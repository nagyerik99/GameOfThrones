using GameOfThrones.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfThrones.Services;
using Template10.Mvvm;

namespace GameOfThrones.ViewModels
{
    public class AllBookViewModel : ViewModelBase
    {
        private ObservableCollection<Book> _bookSeries = new ObservableCollection<Book>();

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

        public async Task LoadBookSeries()
        {
            //if (BookSeries.Count == 0)
            // {
            try
            {
                var result = await DataService.GetBookSeries();
                BookSeries = new ObservableCollection<Book>(result);

            }
            catch (Exception e)
            {
                throw new Exception("Problems getting data from API Message: " + e.Message);
            }
            //}
        }

    }
}
