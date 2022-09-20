using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class TransactionAndImage
    {
        public int Quantity { get; set; }
        public int Readable { get; set; }
        public List<FaceTransaction> RecordList { get; set; }
    }
}
