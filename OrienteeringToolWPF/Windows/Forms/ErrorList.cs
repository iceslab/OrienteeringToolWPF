using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrienteeringToolWPF.Windows.Forms
{
    public class ErrorList : Dictionary<string, string>
    {
        public bool HasErrors()
        {
            return Count > 0;
        }

        public string[] AsStringArray()
        {
            string[] names = new string[Count];
            for (int i = 0; i < Count; i++)
            {
                var e = this.ElementAt(i);
                names[i] = e.Key + ": " + e.Value;
            }

            return names;
        }

        public override string ToString()
        {
            string name = "";
            var names = AsStringArray();
            foreach (var s in names)
                name += s + "\n";
            return name;
        }
    }
}
