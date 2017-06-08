using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    static class ExtensionMethods
    {

        //Adds an extension method to lists of integers, where it returns a copy of the list
        //when supplied a integer
        public static List<int> With(this List<int> list, int i)
        {
            List<int> output = new List<int>(list);
            output.Add(i);
            return output;
        }


        //Returns a copy of a list of integers, where the integer 'i' is removed
        //from the copy, if such an integer exists in the copy
        public static List<int> Without(this List<int> list, int i)
        {
            List<int> output = new List<int>(list);
            output.Remove(i);
            return output;
        }

        //Given a list of type Change, and a new Change, returns a copy of the original
        //list with the new change added
        public static List<Change> With(this List<Change> list, Change move)
        {
            List<Change> output = new List<Change>(list);
            output.Add(move);
            return output;
        }


        //Given a collection that implements IEnumerable, returns true if it contains no objects
        public static bool None<T>(this IEnumerable<T> enumerator)
        {
            return !enumerator.Any();
        }
    }
}
