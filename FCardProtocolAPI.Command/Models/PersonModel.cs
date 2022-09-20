using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class PersonModel
    {
        public string name { get; set; }

        public int? userCode { get; set; }

        public string code { get; set; }

        public string cardData { get; set; }

        public string password { get; set; }

        public string job { get; set; }

        public string dept { get; set; }

        public int? identity { get; set; }

        public int? cardStatus { get; set; }

        public int? cardType { get; set; }

        public int? enterStatus { get; set; }

        public string expiry { get; set; }
        public int? openTimes { get; set; }
        public int? timeGroup { get; set; }

        public string faceImage { get; set; }

        public string[] fp { get; set; }
    }
}
