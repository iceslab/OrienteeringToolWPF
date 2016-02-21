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
        public long Order { get; set; }
        public long Code { get; set; }
        public long RouteId { get; set; }

        public RouteStep() : base()
        {
            Id = null;
            Code = 0;
            RouteId = 0;
        }

        public RouteStep(SiPunch sp, long Order, long RouteId = 0) : this()
        {
            Code = sp.Code;
            this.RouteId = RouteId;
        }

        public static List<RouteStep> Parse(SiPunch[] siPunches, long RouteId = 0)
        {
            if (siPunches != null)
            {
                var rs = new List<RouteStep>(siPunches.Length);
                for (int i = 0; i < siPunches.Length; ++i)
                    rs.Add(new RouteStep(siPunches[i], i + 1, RouteId));

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

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other is RouteStep)
            {
                var rs = other as RouteStep;
                if (Id != rs.Id)
                    return false;
                if (Order != rs.Order)
                    return false;
                if (Code != rs.Code)
                    return false;
                if (RouteId != rs.RouteId)
                    return false;
            }
            else
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hash = Order ^ Code ^ RouteId;
            if (Id != null)
                hash ^= (long)Id;
            return (int)hash;
        }

        public static bool operator <(RouteStep lhs, RouteStep rhs)
        {
            return lhs.Order < rhs.Order;
        }

        public static bool operator >(RouteStep lhs, RouteStep rhs)
        {
            return lhs.Order > rhs.Order;
        }

        public static bool operator ==(RouteStep lhs, RouteStep rhs)
        {
            return lhs.Order == rhs.Order;
        }

        public static bool operator !=(RouteStep lhs, RouteStep rhs)
        {
            return lhs.Order != rhs.Order;
        }
    }
}
