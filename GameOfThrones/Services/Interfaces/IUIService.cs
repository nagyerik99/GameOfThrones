using System;
using System.Threading.Tasks;
using static GameOfThrones.Services.ErrorService;

namespace GameOfThrones.Services.Interfaces
{
    public interface IUIService
    {
        void ShowErrorDialog(string title, string message, EventOnClose onClose, Func<Task> handler);
    }
}
