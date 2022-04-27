using GameOfThrones.Services;
using GameOfThrones.Services.Interfaces;
using GameOfThrones.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;

namespace GameOfThrones.ViewModels
{
    /// <summary>
    /// Viewmodel for the MainPageView
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        private IUIService _uiService;
        private ErrorService _errorService;

        private readonly List<(string Tag, Type Page)> _pages = new List<(string, Type Page)>
            {
                ("AllBookView", typeof(AllBookView)),
                ("AllCharacterView", typeof(AllCharacterView)),
                ("AllHouseView", typeof(AllHousesView)),
            };

        private readonly List<(Type subPage, Type mainPage)> _subPages = new List<(Type subPage, Type mainPage)>
            {
                (typeof(BookDetailsView),typeof(AllBookView)),
                (typeof(CharacterDetailsView),typeof(AllCharacterView)),
                (typeof(HouseDetailsView),typeof(AllHousesView)),
                (typeof(CharacterDetailsView),typeof(AllCharacterView))
            };

        public IUIService UIService
        {
            get { return _uiService; }
            set
            {
                if (_uiService == null)
                {
                    _uiService = value;
                    _errorService = new ErrorService(_uiService);
                }
            }
        }

        /// <summary>
        /// Called when the actual first view is loaded
        /// </summary>
        /// <param name="index"></param>
        public void ViewLoaded(int index)
        {
            NavigateView(_pages[index].Tag);
        }

        /// <summary>
        /// Navigates to the pages that has the given tag, if it is not null
        /// </summary>
        /// <param name="navItemTag"></param>
        public void NavigateView(string navItemTag)
        {
            var (Tag, Page) = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));

            var preNavPageType = NavigationService.PrevPageType;

            if (!(Page is null) && !Type.Equals(preNavPageType, Page))
                NavigationService.NavigateTo(Page, new object[] { NavigationService });
        }

        /// <summary>
        /// Called, when the selection of the navigationItems changed due to the navigation
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public int SelectionChanged(Type page)
        {
            var mainPage = _pages.FirstOrDefault(x => x.Page == page);
            if (mainPage != (null, null))
            {
                return _pages.IndexOf(mainPage);
            }
            else
            {
                var subPage = _subPages.FirstOrDefault(y => y.subPage == page);
               
                if (subPage == (null, null)) return -1;//rossz elem lett atadva ne selectaljunk semmit

                return _pages.FindIndex(x => x.Page == subPage.mainPage);
            }
        }


        /// <summary>
        /// Called when tha navigation is failed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            ErrorService.Instance.ShowErrorMessage(typeof(Exception));
        }

    }
}
