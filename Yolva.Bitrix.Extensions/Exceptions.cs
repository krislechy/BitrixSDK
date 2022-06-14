using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.Extensions
{
    public class UnexpectedStatusCodeException : Exception
    {
        public UnexpectedStatusCodeException(HttpStatusCode expectedStatusCode, HttpStatusCode currentStatusCode, string responseContent) :
            base(String.Format("Unexpected status code from request.\nExpected: {0} ({1})\nCurrent: {2} ({3})\nResponse content: {4}",
                expectedStatusCode.ToString(),
                expectedStatusCode,
                currentStatusCode.ToString(),
                currentStatusCode,
                responseContent
                ))
        {
        }
    }
}
