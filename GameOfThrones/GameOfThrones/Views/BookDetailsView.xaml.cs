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
    public sealed partial class BookDetailsView : Page
    {
        public BookDetailsView()
        {
            this.InitializeComponent();
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

        private async void LodeMorePOVCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadPOV();
        }

        private async void LodeMoreCharacterButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadCharacters();
        }

        private void CharactersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(((Character)(e.ClickedItem)).ID);
        }

        private void POVCharactersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(((Character)(e.ClickedItem)).ID);
        }
    }
}
