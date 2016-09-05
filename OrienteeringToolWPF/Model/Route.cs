using GecoSI.Net.Dataframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Models route description - eg. every category has its own route (a set of route steps)
/// here you only have Id and Name of it
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Route : BaseModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long Category { get; set; }

        #region Object overrides
        public override string ToString()
        {
            return Name;
        }

        // Equality method (returns true when all fields matches)
        public override bool Equals(object obj)
        {
            if (obj is Route)
            {
                Route r = (Route)obj;
                if (Id != r.Id)
                    return false;
                if (!Name.Equals(r.Name))
                    return false;
                if (Category != r.Category)
                    return false;
                return true;
            }

            return false;
        }

        // Generates hashcode from Name and Id if present
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Id.GetHashCode() + Category.GetHashCode();
        }
        #endregion
    }
}
