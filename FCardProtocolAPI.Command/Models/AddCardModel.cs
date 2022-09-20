using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Command.Models
{
    public class AddCardModel
    {
        /// <summary>
        /// 0、非排序区 1、排序区
        /// </summary>
        public int? AreaType { get; set; } = 0;
        /// <summary>
        /// 卡号列表
        /// </summary>
        public List<CardModel> Cards { get; set; }

    }
}
