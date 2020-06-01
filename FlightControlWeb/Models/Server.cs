using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Server
    {
        public string ServerId { get; set; } = "";
        public string ServerURL { get; set; } = "";

        // checkking if the server is valid.
        public Boolean IsValidServer()
        {
            if(this.ServerId != "" && this.ServerURL != "")
            {
                return true;
            }
            return false;
        }

    }
}
