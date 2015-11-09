using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.Core
{
    public class HomeControllerApiException : Exception
    {
        public HomeControllerApiException(string errorMessage)
            : base(errorMessage)
        { }
    }
}
