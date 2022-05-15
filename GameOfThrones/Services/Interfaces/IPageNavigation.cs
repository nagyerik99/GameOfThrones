using System;

namespace GameOfThrones.Services.Interfaces
{
    public interface IPageNavigation
    {
        Type PrevPageType { get; }
        bool NavigateTo(Type page, object[] param=null);
        void GoBack();
        bool CanGoBack();
    }
}
