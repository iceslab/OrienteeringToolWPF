﻿using GecoSI.Net.Dataframe;
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

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Route)
            {
                Route r = (Route)obj;
                if (Id == r.Id)
                {
                    if (Name.Equals(r.Name))
                        return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Id.GetHashCode();
        }
    }
}
