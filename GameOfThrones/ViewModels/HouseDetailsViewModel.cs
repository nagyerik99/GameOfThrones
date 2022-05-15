using GameOfThrones.Models;
using GameOfThrones.Services;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{

    /// <summary>
    /// Viewmodel for the HouseDetailsView, containg the business logic
    /// </summary>
    public class HouseDetailsViewModel : ViewModelBase
    {
        private House _selectedHouse;
        private ObservableCollection<House> _cadetBranches;
        private ObservableCollection<Character> _swornMembers;
        private Visibility _cadetBranchesVisibility = Visibility.Visible;
        private Visibility _swornMembersVisibility = Visibility.Visible;

        private bool _cadetDataLoaded = false;
        private bool _swornMembersLoaded = false;

        private string _cadetLoadMoreText = s_LoadMoreText;
        private string _swornMembersLoadMoreText = s_LoadMoreText;

        private HouseDetailsPager _pager;

        public bool CadetDataLoaded
        {
            get { return _cadetDataLoaded; }
            private set
            {
                _cadetDataLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool MembersDataLoaded
        {
            get { return _swornMembersLoaded; }
            private set
            {
                _swornMembersLoaded = value;
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
                if (_pager == null || _cadetBranchesVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return _pager.CadetBranchesEmpty;
                }

            }
        }

        public Visibility LoadMoreSwornMembersVisibility
        {
            get
            {
                if (_pager == null || _swornMembersVisibility == Visibility.Collapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return _pager.MembersEmpty;
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
                return _cadetLoadMoreText;
            }

            private set
            {
                if (_cadetLoadMoreText != value)
                {
                    _cadetLoadMoreText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MembersLoadMoreText
        {
            get
            {
                return _swornMembersLoadMoreText;
            }

            private set
            {
                if (_swornMembersLoadMoreText != value)
                {
                    _swornMembersLoadMoreText = value;
                    OnPropertyChanged();
                }
            }
        }

        public HouseDetailsViewModel()
        {
            PageSize = 10;
            CadetBranches = new ObservableCollection<House>();
            SwornMembers = new ObservableCollection<Character>();
        }


        /// <summary>
        /// Called on navigation. Calls the base function, and starts to load the selectedHouse if capable.
        /// <para>Throws an error if cant load the data </para>
        /// </summary>
        /// <param name="parameters"></param>
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

        /// <summary>
        /// <para>Sets the view visible and the paging functionality enabled</para>
        /// <para>Should be called, when the event/long procedure finished</para>
        /// </summary>
        protected override void Loaded()
        {
            base.Loaded();//Isbusy=false;
            CadetDataLoaded = true;
            MembersDataLoaded = true;
            ViewLoadingVisibility = Visibility.Visible; //viewLoaded
            OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));//set visible if have to 
            OnPropertyChanged(nameof(LoadMoreSwornMembersVisibility));//set visible if have to 
        }

        /// <summary>
        /// Loads the next page of members associated with the <see cref="SelectedHouse"/>
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task LoadMoreMembers()
        {
            try
            {
                MembersDataLoaded = false;
                MembersLoadMoreText = s_LoadingText;
                var result = await _pager.GetNextMembers();

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
                MembersLoadMoreText = s_LoadMoreText;
                OnPropertyChanged(nameof(LoadMoreSwornMembersVisibility));
            }
        }


        /// <summary>
        /// Loads the next page of characters, if able to, and loads it to <see cref="CadetBranches"/>
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task LoadMoreCadets()
        {
            CadetDataLoaded = false;
            CadetBranchesLoadMoreText = s_LoadingText;
            try
            {
                var result = await _pager.GetNextCadets();

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
                CadetBranchesLoadMoreText = s_LoadMoreText;
                OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));

            }
        }

        /// <summary>
        /// Sets the CadetList visible, or collapsed, depends on the pervious state of the View 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModifyCadetListView(object sender, RoutedEventArgs e)
        {
            CadetBranchesVisibility = CadetBranchesVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(LoadMoreCadetBranchesVisibility));
        }

        /// <summary>
        /// Sets the MembersList visible, org collapsed, depending on the preveious state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            _pager = new HouseDetailsPager(result.Item2, result.Item3, PageSize);
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
