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
    public sealed partial class HouseDetailsView : Page
    {
        public HouseDetailsView()
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

        private void MembersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(((Character)e.ClickedItem).ID);
        }

        private void OverLordName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<HouseDetailsView>(ViewModel.SelectedHouse.OverLord.ID);
        }

        private void HeirName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedHouse.Heir.ID);
        }

        private void CurrentLordName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedHouse.CurrentLord.ID);
        }

        private void FounderName_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateToDetails<CharacterDetailsView>(ViewModel.SelectedHouse.Founder.ID);
        }

        private void CadetBrancesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.NavigateToDetails<HouseDetailsView>(((House)e.ClickedItem).ID);
        }

        private async void LoadMoreCadetBranchesButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMoreCadets();
        }

        private async void LoadMoreMembersButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMoreMembers();
        }
    }
}
