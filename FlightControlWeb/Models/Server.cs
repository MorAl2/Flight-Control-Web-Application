using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Server
    {
        public string ServerId { get; set; }
        public string ServerURL { get; set; }

        public Boolean IsValidServer()
        {
            if(this.ServerId != null && this.ServerURL != null)
            {
                return true;
            }
            return false;
        }

    }
}
