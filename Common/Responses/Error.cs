using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Responses
{
    public class Error
    {
        public List<string> ErrorMessages { get; set; }
        public string FriendlyErrorMessage { get; set; }
        //public Error(List<string> errorMessages, string friendlyMessage)
        //{
        //    ErrorMessages = errorMessages;
        //    FriendlyErrorMessage = friendlyMessage;
        //}
    }
}
