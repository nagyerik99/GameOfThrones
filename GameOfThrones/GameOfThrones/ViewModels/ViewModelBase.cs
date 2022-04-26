using GameOfThrones.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public IPageNavigation navigationService;
        private bool busy = true;
        protected int pageSize;
        protected string URI;
        protected bool _searchEnabled = false;
        protected Visibility _viewLoadingVisibility = Visibility.Collapsed;
        protected static string loadMoreText = "Load More";
        protected static string loadingText = "Loading...";

        public Visibility ViewLoadingVisibility
        {
            get
            {
                return _viewLoadingVisibility;
            }
            protected set
            {
                if (_viewLoadingVisibility != value)
                {
                    _viewLoadingVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return busy;
            }

            protected set
            {
                if (busy != value)
                {
                    busy = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NavigateTo(Type pageType, object[] param)
        {
            bool result = navigationService.NavigateTo(pageType, param);

            if (result == false)
                throw new Exception($"Cant navigate to {pageType}");
        }

        public void GoBack()
        {
            navigationService.GoBack();
        }

        public bool CanGoBack()
        {
            return navigationService.CanGoBack();
        }

        public virtual void Navigated(object parameters)
        {
            Loading();
            //first parameter should always be the navigationService
            var Parameters = parameters as object[];
            navigationService = Parameters[0] as IPageNavigation;
            if (navigationService == null)
                throw new Exception("Navigation Handle Error: NavigationService param cannot be null");
        }

        public virtual void NavigatedFrom()
        {
        }

        protected virtual void Loaded()
        {
            IsBusy = false;
            ViewLoadingVisibility = Visibility.Visible;
            //TODO any other functionality
        }

        protected virtual void Loading()
        {
            ViewLoadingVisibility = Visibility.Collapsed;
            IsBusy = true;
        }

        public virtual void NavigateToDetails<PageType>(string ID)
        {
            object[] parameters = new object[] { navigationService, ID };
            navigationService.NavigateTo(typeof(PageType), parameters);
        }
    }
}
