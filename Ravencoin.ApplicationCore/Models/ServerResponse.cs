using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
namespace Ravencoin.ApplicationCore.Models
{
    public class ServerResponse{
        public HttpStatusCode statusCode { get; set; }
        public string errorEx { get; set; }
        public string responseContent { get; set; }
    }
}
