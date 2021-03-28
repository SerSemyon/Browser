using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser
{
    class history
    {
        public DateTime time;
        public string name;
        public string url;
        public history(DateTime newTime, string newName, string newUrl)
        {
            time = newTime;
            name = newName;
            url = newUrl;
        }
    }
}
