using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    static class ExtensionMethods
    {
        public static List<int> With(this List<int> list, int i)
        {
            List<int> output = new List<int>(list);
            output.Add(i);
            return output;
        }

        public static List<int> Without(this List<int> list, int i)
        {
            List<int> output = new List<int>(list);
            output.Remove(i);
            return output;
        }
    }
}
