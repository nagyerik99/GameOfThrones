using GameOfThrones.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GameOfThrones
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IPageNavigation
    {
        public Type prevPageType 
        {
            get { return ContentFrame.CurrentSourcePageType; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel.navigationService = this;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
            ViewModel.ViewLoaded(0);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                ViewModel.NavigateView(navItemTag);
            }
        }

        public bool NavigateTo(Type page, object[] param)
        {
            return ContentFrame.Navigate(page, param);
        }

        public void GoBack()
        {
            ContentFrame.GoBack();
        }

        public bool CanGoBack()
        {
            return ContentFrame.CanGoBack;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ViewModel.GoBack();
        }
    }
}
