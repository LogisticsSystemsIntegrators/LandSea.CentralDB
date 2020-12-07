using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XMLAPI.DataAccess
{
    public class ETNNumberModel:ModelBase
    {
        public string CaroWiseKey { get; set; }
        public string ETNNumber { get; set; }
    }
}