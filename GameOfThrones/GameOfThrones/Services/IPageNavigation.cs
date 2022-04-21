using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfThrones.Services
{
    public interface IPageNavigation
    {
        Type prevPageType { get; }
        bool NavigateTo(Type page, object[] param=null);
        void GoBack();
        bool CanGoBack();
    }
}
