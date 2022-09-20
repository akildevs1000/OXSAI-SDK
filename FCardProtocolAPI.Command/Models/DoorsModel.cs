using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class DoorsModel
    {
        public bool Door1 { get; set; }
        public bool Door2 { get; set; }
        public bool Door3 { get; set; }
        public bool Door4 { get; set; }

        public static DoorsModel GetInstance()
        {
            return new Models.DoorsModel
            {
                Door1 = true,
                Door2 = true,
                Door3 = true,
                Door4 = true
            };
        }
    }
}
