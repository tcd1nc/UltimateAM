using System;

namespace AssetManager.Models
{
    public class AuditDateModel : BaseModel
    {
       
        int _assetid;
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }

        DateTime? _dt;
        public DateTime? DateAudit
        {
            get { return _dt; }
            set { SetField(ref _dt, value); }
        }       
    }
}
