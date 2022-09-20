using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class CardModel
    {
        public string CardData { get; set; }

        public string Password { get; set; }

        public DateTime? Expiry { get; set; }

        public int[] TimeGroup { get; set; }

        public Models.DoorsModel Doors { get; set; }

        public int? OpenTimes { get; set; }
    }
}
