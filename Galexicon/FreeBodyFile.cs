using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galexicon
{
    public class FreeBodyFile
    {
        public DateTime DefaultTimestamp { get; set; } // DateTime of last known edit of default
        public FreeBody Default { get; set; }
        public FreeBody Body { get; set; }

    }
}
