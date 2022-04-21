using GameOfThrones.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GameOfThrones.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
            private readonly List<(string Tag, Type Page)> _pages = new List<(string, Type Page)>
            {
                ("AllBookView", typeof(AllBookView)),
                ("AllCharacterView", typeof(AllCharacterView)),
                ("AllHouseView", typeof(AllHousesView)),
            };

            public MainPageViewModel() { }

        public void ViewLoaded(int index)
            {
                NavigateView(_pages[index].Tag);
            }


            public void NavigateView(string navItemTag)
            {
            Type _page = null;
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            _page = item.Page;

            var preNavPageType = navigationService.prevPageType;

            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                navigationService.NavigateTo(_page,new object[] { navigationService});
               //contentFrame.Navigate(_page, null, new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }
        }

        public void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
