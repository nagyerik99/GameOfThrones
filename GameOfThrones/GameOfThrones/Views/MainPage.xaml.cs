using GameOfThrones.Services;
using GameOfThrones.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static GameOfThrones.Services.ErrorService;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GameOfThrones
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
            ViewModel.navigationService = this;
            ViewModel.UIService = this;
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

        public void GoBack()
        {
            if (ViewModel.CanGoBack())
            {
                ContentFrame.GoBack();
                ViewModel.SelectionChanged(ContentFrame.CurrentSourcePageType);
            }
        }

        public bool CanGoBack()
        {
            return ContentFrame.CanGoBack;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(ViewModel.CanGoBack())
                ViewModel.GoBack();
        }

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
