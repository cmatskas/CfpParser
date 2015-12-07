using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CfpParser
{
    public class Cfp
    {
        public string name { get; set; }
        public string conferenceStart { get; set; }
        public string conferenceEnd { get; set; }
        public string callForPapersEnd { get; set; }
        public string url { get; set; }
        public string lang { get; set; }
        public string location { get; set; }
        public string tags { get; set; }
    }
}
