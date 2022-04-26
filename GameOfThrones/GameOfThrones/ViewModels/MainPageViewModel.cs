using GameOfThrones.Services;
using GameOfThrones.Services.Interfaces;
using GameOfThrones.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Navigation;

namespace GameOfThrones.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IUIService uiService;
        private ErrorService errorService;

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
            get { return uiService; }
            set
            {
                if (uiService == null)
                {
                    uiService = value;
                    errorService = new ErrorService(uiService);
                }
            }
        }


        public void ViewLoaded(int index)
        {
            NavigateView(_pages[index].Tag);
        }


        public void NavigateView(string navItemTag)
        {
            var (Tag, Page) = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));

            var preNavPageType = navigationService.PrevPageType;

            if (!(Page is null) && !Type.Equals(preNavPageType, Page))
                navigationService.NavigateTo(Page, new object[] { navigationService });
        }

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


        public void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }



    }
}
