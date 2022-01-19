using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// 通用資料處理介面
    /// </summary>
    interface IModel
    {
        public JObject Run(dynamic parm);
    }
}
