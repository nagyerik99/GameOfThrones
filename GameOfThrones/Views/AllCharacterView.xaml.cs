using GameOfThrones.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameOfThrones.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllCharacterView : Page
    {
        public AllCharacterView()
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

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadCharacters();
        }

        private void CharactersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(((Character)e.ClickedItem).ID);
        }
    }
}
