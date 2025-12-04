using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class DeletedCampaignTempModel : BindableBase
    {
        [ProtoMember(1)]
        public CampaignDetails CampignDeletedTemps { get; set; }
    }
}
