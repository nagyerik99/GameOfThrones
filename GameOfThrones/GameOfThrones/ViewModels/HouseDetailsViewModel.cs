using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{
    public class HouseDetailsViewModel : ViewModelBase
    {
        private House _selectedHouse;
        private ObservableCollection<House> _cadetBranches;
        private ObservableCollection<Character> _swornMembers;
        private Visibility _cadetBranchesVisibility = Visibility.Visible;
        private Visibility _swornMembersVisibility = Visibility.Visible;

        private bool cadetDataLoaded = false;
        private bool swornMembersLoaded = false;

        private string cadetLoadMoreText = loadMoreText;
        private string swornMembersLoadMoreText = loadMoreText;

        private HouseDetailsPager Pager;

        public bool CadetDataLoaded
        {
            get { return cadetDataLoaded; }
            private set
            {
                cadetDataLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool MembersDataLoaded
        {
            get { return swornMembersLoaded; }
            private set
            {
                swornMembersLoaded = value;
                OnPropertyChanged();
            }
        }

        public Visibility CadetBranchesVisibility
        {
            get { return _cadetBranchesVisibility; }
            private set
            {
                if (_cadetBranchesVisibility != value)
                {
                    _cadetBranchesVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility SwornMembersVisibility
        {
            get { return _swornMembersVisibility; }

            private set
            {
                if (_swornMembersVisibility != value)
                {
                    _swornMembersVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility LoadMoreCadetBranchesVisibility
        {
            get
            {
                if (Pager == null || _cadetBranchesVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Pager.CadetBranchesEmpty;
                }

            }
        }

        public Visibility LoadMoreSwornMembersVisibility
        {
            get
            {
                if (Pager == null || _swornMembersVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Pager.MembersEmpty;
                }

            }
        }

        public House SelectedHouse
        {
            get { return _selectedHouse; }
            private set
            {
                _selectedHouse = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<House> CadetBranches
        {
            get
            {
                return _cadetBranches;
            }

            private set
            {
                _cadetBranches = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Character> SwornMembers
        {
            get
            {
                return _swornMembers;
            }

            private set
            {
                _swornMembers = value;
                OnPropertyChanged();
            }
        }

        public string CadetBranchesLoadMoreText
        {
            get
            {
                return cadetLoadMoreText;
            }

            private set
            {
                if (cadetLoadMoreText != value)
                {
                    cadetLoadMoreText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MembersLoadMoreText
        {
            get
            {
                return swornMembersLoadMoreText;
            }

            private set
            {
                if (swornMembersLoadMoreText != value)
                {
                    swornMembersLoadMoreText = value;
                    OnPropertyChanged();
                }
            }
        }

        public HouseDetailsViewModel()
        {
            pageSize = 10;
            CadetBranches = new ObservableCollection<House>();
            SwornMembers = new ObservableCollection<Character>();
        }

        public override async void Navigated(object parameters)
        {
            base.Navigated(parameters);
            var Parameters = parameters as object[];

            if (Parameters[1] == null)
                ErrorService.Instance.ShowErrorMessage(typeof(ErrorService.NavigationException));
            else
            {
                URI = Parameters[1].ToString();
                await this.LoadHouse(URI);
            }
        }

        protected override void Loaded()
        {
            base.Loaded();//Isbusy=false;
            CadetDataLoaded = true;
            MembersDataLoaded = true;
            ViewLoadingVisibility = Visibility.Visible; //viewLoaded
            OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));//set visible if have to 
            OnPropertyChanged(nameof(LoadMoreSwornMembersVisibility));//set visible if have to 
        }

        public async Task LoadMoreMembers()
        {
            try
            {
                MembersDataLoaded = false;
                MembersLoadMoreText = loadingText;
                var result = await Pager.GetNextMembers();

                foreach (var res in result)
                {
                    SwornMembers.Add(res);
                }
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadMoreMembers);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadMoreMembers);
            }
            finally
            {
                MembersDataLoaded = true;
                MembersLoadMoreText = loadMoreText;
                OnPropertyChanged(nameof(LoadMoreSwornMembersVisibility));
            }
        }

        public async Task LoadMoreCadets()
        {
            CadetDataLoaded = false;
            CadetBranchesLoadMoreText = loadingText;
            try
            {
                var result = await Pager.GetNextCadets();

                foreach (var res in result)
                {
                    CadetBranches.Add(res);
                }
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadMoreCadets);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadMoreCadets);
            }
            finally
            {
                CadetDataLoaded = true;
                CadetBranchesLoadMoreText = loadMoreText;
                OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));

            }
        }


        public void ModifyCadetListView(object sender, RoutedEventArgs e)
        {
            CadetBranchesVisibility = CadetBranchesVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));
        }

        public void ModifyMembersListView(object sender, RoutedEventArgs e)
        {
            SwornMembersVisibility = SwornMembersVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreSwornMembersVisibility));
        }

        private async Task LoadHouse(string id)
        {
            try
            {
                await GetData(id);
            }
            catch (HttpRequestException e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadHouse);
            }
            catch (Exception e)
            {
                ErrorService.Instance.ShowErrorMessage(e.GetType(), LoadHouse);
            }
        }

        private async Task GetData(string id)
        {
            var result = await DataService.GetHouseDetails(id);
            SelectedHouse = result.Item1;
            Pager = new HouseDetailsPager(result.Item2, result.Item3, pageSize);
            await LoadMoreMembers();
            await LoadMoreCadets();
            Loaded();
        }

        private async Task LoadHouse()
        {
            if (URI != null)
            {
                await LoadHouse(URI);
            }
        }
    }
}
