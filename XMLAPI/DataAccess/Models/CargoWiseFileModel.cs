using System;

namespace XMLAPI.DataAccess
{
    public class CargoWiseFileModel:ModelBase
    {
        public int? ID { get; set; }

        public string XMLType { get; set; }
        public string Key { get; set; }
        public string FileContext { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool SAPProcessed { get; set; } = false;
        public DateTime? SAPProcessedDate { get; set; }
        public bool LandSeaProcessed { get; set; } = false;
        public DateTime? LandSeaProcessedDate { get; set; }
        public string ETNNumber { get; set; }

    }
}