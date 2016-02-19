using GecoSI.Net.Dataframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Model
{
    public class RouteStep : BaseModel
    {
        public long? Id { get; set; }
        public long Code { get; set; }
        public long RouteId { get; set; }

        public RouteStep() : base()
        {
            Id = null;
            Code = 0;
            RouteId = 0;
        }

        public RouteStep(SiPunch sp, long RouteId = 0) : this()
        {
            Code = sp.Code;
            this.RouteId = RouteId;
        }

        public static List<RouteStep> Parse(SiPunch[] siPunches, long RouteId = 0)
        {
            if (siPunches != null)
            {
                var rs = new List<RouteStep>(siPunches.Length);
                foreach (var sp in siPunches)
                    rs.Add(new RouteStep(sp, RouteId));

                return rs;
            }
            else
                return new List<RouteStep>();
        }

        public static void SetRouteIdNumber(ref List<RouteStep> rs, long RouteId)
        {
            foreach (var r in rs)
                r.RouteId = RouteId;
        }
    }
}
