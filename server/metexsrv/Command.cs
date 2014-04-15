using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metexsrv
{
    class Command
    {
        public String command { set; get; }
        public Dictionary<String, String> parameter { set; get; }
    }
}
