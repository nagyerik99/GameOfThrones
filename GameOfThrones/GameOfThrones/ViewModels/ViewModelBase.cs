using GameOfThrones.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace GameOfThrones.ViewModels
{
    /// <summary>
    /// BaseClass of the ViewModels, which implements the INPC interface
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool busy = true;
        protected int PageSize;
        protected string URI;
        protected Visibility ViewloadingVisibility = Visibility.Collapsed;
        protected static string s_LoadMoreText = "Load More";
        protected static string s_LoadingText = "Loading...";

        public event PropertyChangedEventHandler PropertyChanged;

        public IPageNavigation NavigationService;

        public Visibility ViewLoadingVisibility
        {
            get
            {
                return ViewloadingVisibility;
            }
            protected set
            {
                if (ViewloadingVisibility != value)
                {
                    ViewloadingVisibility = value;
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

        /// <summary>
        /// Signal for the UI, that the associated property is updated
        /// </summary>
        /// <param name="propertyName">the name of the updated property </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Virtual Function which can be called, to navigate to another page, with the given params
        /// </summary>
        /// <param name="pageType">the requested pagetype</param>
        /// <param name="param">the given params</param>
        public void NavigateTo(Type pageType, object[] param)
        {
            bool result = NavigationService.NavigateTo(pageType, param);

            if (result == false)
                throw new Exception($"Cant navigate to {pageType}");
        }

        /// <summary>
        /// Function to request goBack event from the navigation service
        /// </summary>
        public void GoBack()
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// returns wheter we can go back from the actual page or not
        /// </summary>
        /// <returns> <c>true</c> if can go back, else <c>false</c></returns>
        public bool CanGoBack()
        {
            return NavigationService.CanGoBack();
        }

        /// <summary>
        /// Virtual function, that should be called int the view, if the page is navigatedTo, by the Navigation service
        /// <paramref name="parameters"/> <b>First parameter must be the navigationService always</b>
        /// </summary>
        /// <param name="parameters">the parameters sent by the NavigationService</param>
        public virtual void Navigated(object parameters)
        {
            Loading();
            //first parameter should always be the navigationService
            var Parameters = parameters as object[];
            NavigationService = Parameters[0] as IPageNavigation;
            if (NavigationService == null)
                throw new Exception("Navigation Handle Error: NavigationService param cannot be null");
        }

        /// <summary>
        /// Virtual function that is implemented by the derived clasess
        /// And called upon, when navigating from the page is requested
        /// </summary>
        public virtual void NavigatedFrom()
        {
        }


        /// <summary>
        /// Function that called, when the requested data and functionality loaded.
        /// Set everything visible
        /// </summary>
        protected virtual void Loaded()
        {
            IsBusy = false;
            ViewLoadingVisibility = Visibility.Visible;
            //TODO any other functionality
        }

        /// <summary>
        /// Called, when the requested data is currently loading in progress.
        /// Progresswindow is shown
        /// </summary>
        protected virtual void Loading()
        {
            ViewLoadingVisibility = Visibility.Collapsed;
            IsBusy = true;
        }

        /// <summary>
        /// Called when navigatio from review Page type to a Detail page type, is requested
        /// </summary>
        /// <typeparam name="PageType">the requested pageType</typeparam>
        /// <param name="ID">the requested instances ID/URI </param>
        public virtual void NavigateToDetails<PageType>(string ID)
        {
            object[] parameters = new object[] { NavigationService, ID };
            NavigationService.NavigateTo(typeof(PageType), parameters);
        }

    }
}
