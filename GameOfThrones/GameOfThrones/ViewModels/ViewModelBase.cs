using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;

namespace GameOfThrones.ViewModels
{

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public IPageNavigation navigationService;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NavigateTo(Type pageType, object[] param)
        {
            bool result = navigationService.NavigateTo(pageType,param);

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

        public virtual async void Navigated(object parameters)
        {
            //first parameter should always be the navigationService
            var Parameters = parameters as object[];
            navigationService = Parameters[0] as IPageNavigation;

            if (navigationService == null)
                throw new Exception("Navigation Handle Error: NavigationService param cannot be null");
        }
    }
}
