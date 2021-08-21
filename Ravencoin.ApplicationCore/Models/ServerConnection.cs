using System;
using System.Collections.Generic;
using System.Text;

namespace Ravencoin.ApplicationCore.Models
{ 
    public class ServerConnection{
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
