using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{
    public class BookDetailsViewModel : ViewModelBase
    {
        private Book _selectedBook;
        public string ID;
        private Visibility _characterViewVisibility = Visibility.Visible;
        private Visibility _povCharacterViewVisibility = Visibility.Visible;
        
        public Visibility CharactersVisibility
        {
            get { return _characterViewVisibility; }
            set
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

            set
            {
                if (_povCharacterViewVisibility != value)
                {
                    _povCharacterViewVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        public BookDetailsViewModel()
        {
        }

        private async Task LoadBook(string id)
        {
            try
            {
                var result = await DataService.GetBookDetails(id);
                SelectedBook = result;
            }
            catch (Exception e)
            {
                throw new Exception("Problems getting data from API Message: " + e.Message);
            }
        }

        public void ModifyCharacterListView(object sender, RoutedEventArgs e)
        {
            CharactersVisibility = CharactersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ModifyPOVCharacterListView(object sender, RoutedEventArgs e)
        {
            POVCharactersVisibility = POVCharactersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            var Parameters = parameters as object[];

            await this.LoadBook(Parameters[1].ToString());
        }
    }
}
