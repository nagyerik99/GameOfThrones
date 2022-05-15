using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GameOfThrones.Services.Interfaces;

namespace GameOfThrones.Services
{
    /// <summary>
    /// ErrorService class which can be used, to alert the user, about any not planned event during run.
    /// </summary>
    public class ErrorService
    {
        public enum EventOnClose
        {
            CloseApp,
            GoBack
        }

        private static IUIService _uiService;
        public class NavigationException : Exception { }
        /// <summary>
        /// The only instance of the class, we can use it to access the functions/variables
        /// </summary>
        public static ErrorService Instance = null;

        private List<(Type errorType, string title, string message, EventOnClose onclose)> _errorMessages = new List<(Type errorType, string title, string message, EventOnClose onclose)>
        {
            (typeof(HttpRequestException),"No Internet","Please check you Internet connection, and try again!",EventOnClose.CloseApp),
            (typeof(NavigationException),"Not Found","The Page you've been looking for could not be found!",EventOnClose.GoBack),
            (typeof(Exception),"Error","Error during trying to access data.",EventOnClose.GoBack),
        };

        /// <summary>
        /// Constructor which creates the only Instance of the Class.
        /// </summary>
        /// <param name="service">the UIService which responsible for showing the errorMessage to the user</param>
        public ErrorService(IUIService service)
        {
            if (Instance == null)
            {
                _uiService = service;
                Instance = this;
            }
        }


        /// <summary>
        /// Determines, the showable message regarding to the Exception type.
        /// And calls upon the UIservice for visual representation
        /// The handler can be used, to let the user try again, the procedure gone wrong
        /// </summary>
        /// <param name="errorMessageType"> The Exception type</param>
        /// <param name="handler"> The TryAgain handler</param>
        public void ShowErrorMessage(Type errorMessageType, Func<Task> handler = null)
        {
            var result = _errorMessages.FirstOrDefault(x => x.errorType == errorMessageType);

            if (result.errorType == null)
                result = _errorMessages[2];

            _uiService.ShowErrorDialog(result.title, result.message, result.onclose, handler);
        }
    }
}
