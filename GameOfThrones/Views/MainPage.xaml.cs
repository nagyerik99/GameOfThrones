using GameOfThrones.Services;
using GameOfThrones.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static GameOfThrones.Services.ErrorService;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GameOfThrones.Views
{
    /// <summary>
    /// MainPage that contains the main navigation functionality, and appearance services.
    /// <para>Implements the <see cref="IPageNavigation"/> interface, and <see cref="IUIService"/> interface,
    /// for navigation, and to show the sent error/dialog messages</para>
    /// </summary>
    public sealed partial class MainPage : Page, IPageNavigation, IUIService
    {
        public Type PrevPageType 
        {
            get { return ContentFrame.CurrentSourcePageType; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel.NavigationService = this;
            ViewModel.UIService = this;
        }

        /// <summary>
        /// Called when the navigationview is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
            ViewModel.ViewLoaded(0);
        }


        /// <summary>
        /// Responsible, for handling the changed navigation item selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                ViewModel.NavigateView(navItemTag);
            }
        }

        /// <summary>
        /// Called, through to the navigationService, when an other page is requested
        /// </summary>
        /// <param name="page"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool NavigateTo(Type page, object[] param)
        {
            var result = ContentFrame.Navigate(page, param);

            if(result)
                SelectionChanged(page);

            return result;
        }

        private void SelectionChanged(Type page)
        {
            int index = ViewModel.SelectionChanged(page);

            if (index > -1)
                NavigationView.SelectedItem = NavigationView.MenuItems[index];
        }


        /// <summary>
        /// called, when through navigationservice back is requested
        /// </summary>
        public void GoBack()
        {
            if (ViewModel.CanGoBack())
            {
                ContentFrame.GoBack();
                SelectionChanged(ContentFrame.CurrentSourcePageType);
            }
        }

        /// <summary>
        /// Determines, wheter the navigation/page history has any other element, for loading
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack()
        {
            return ContentFrame.CanGoBack;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(ViewModel.CanGoBack())
                ViewModel.GoBack();
        }


        /// <summary>
        /// Implemnets the <see cref="IUIService"/> interface <see cref="ShowErrorDialog(string, string, EventOnClose, Func{Task})"/> function
        /// to show the error/dialog messages , sent by the <see cref="ErrorService"/>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="onClose"></param>
        /// <param name="handler"></param>
        public async void ShowErrorDialog(string title, string message, EventOnClose onClose, Func<Task> handler)
        {
            MessageDialog messageDialog = new MessageDialog(message)
            {
                Title = title
            };

            if (handler!=null)
                messageDialog.Commands.Add(new UICommand("Try Again", new UICommandInvokedHandler(TryAgainHandler),handler));
            
            messageDialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(x=> {
                switch (onClose)
                {
                    case EventOnClose.GoBack:
                        GoBack();
                        break;
                    case EventOnClose.CloseApp:
                        Application.Current.Exit();
                        break;
                }
            })));

            await messageDialog.ShowAsync();
        }

        private async void TryAgainHandler(IUICommand command)
        {
            if (command.Id is Func<Task> task)
                await task();
        }
    }
}
