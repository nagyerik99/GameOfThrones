using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameOfThrones.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CharacterDetailsView : Page
    {
        public CharacterDetailsView()
        {
            this.InitializeComponent();
        }

        private async void LoadMoreAllegiancesButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAllegiances();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Navigated(e.Parameter);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.NavigatedFrom();
            base.OnNavigatedFrom(e);
        }

        private void AllegiancesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<HouseDetailsView>(((House)e.ClickedItem).ID);
        }

        private void BooksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<BookDetailsView>(((Book)e.ClickedItem).ID);
        }

        private void SpouseName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedCharacter.Spouse.ID);
        }

        private void MotherName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedCharacter.Mother.ID);
        }

        private void FatherName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedCharacter.Father.ID);
        }
    }
}
