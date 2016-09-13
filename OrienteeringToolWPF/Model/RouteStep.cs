using GecoSI.Net.Dataframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Models one step in route - one point to acqiure by competitor
/// </summary>
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
            Order = 0;
            Code = 0;
            RouteId = 0;
        }

        public RouteStep(SiPunch sp, long Order, long RouteId = 0) : this()
        {
            this.Order = Order;
            Code = sp.Code;
            this.RouteId = RouteId;
        }

        // Parses array of SiPunch to list of RouteSteps
        public static IList<RouteStep> Parse(SiPunch[] siPunches, long RouteId = 0)
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

        // Sets the same provided RouteId for all RouteStep objects in list
        public static void SetRouteIdNumber(ref List<RouteStep> rs, long RouteId)
        {
            foreach (var r in rs)
                r.RouteId = RouteId;
        }

        public static Dictionary<long, long> GetCodeOccurenceCount(IList<RouteStep> rs)
        {
            var dict = new Dictionary<long, long>();

            foreach(var e in rs)
            {
                if (dict.ContainsKey(e.Code))
                    dict[e.Code]++;
                else
                    dict.Add(e.Code, 1);
            }

            return dict;
        }

        #region Object overrides
        // Equality method (returns true when all fields matches)
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

        // Generates hashcode from Order, Code, RouteId and Id if present
        public override int GetHashCode()
        {
            var hash = Order ^ Code ^ RouteId;
            if (Id != null)
                hash ^= (long)Id;
            return (int)hash;
        }
        #endregion
        #region Operators
        // Lesser than operator (compares only Order field)
        public static bool operator <(RouteStep lhs, RouteStep rhs)
        {
            return lhs?.Order < rhs?.Order;
        }

        // Greater than operator (compares only Order field)
        public static bool operator >(RouteStep lhs, RouteStep rhs)
        {
            return lhs?.Order > rhs?.Order;
        }

        // Equal operator (compares only Order field)
        public static bool operator ==(RouteStep lhs, RouteStep rhs)
        {
            return lhs?.Order == rhs?.Order;
        }

        // Not equal operator (compares only Order field)
        public static bool operator !=(RouteStep lhs, RouteStep rhs)
        {
            return lhs?.Order != rhs?.Order;
        }
        #endregion
    }
}
