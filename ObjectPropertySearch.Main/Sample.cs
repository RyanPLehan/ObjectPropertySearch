using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPropertySearch.Main
{
    internal class Sample
    {
        public int Root_Int => 123;
        public DateTime Root_Date => DateTime.Now;
        public string Root_String => "Object Sample";

        public IDictionary<string, object> Level2 => CreateDictionary("Level2");
        public IDictionary<string, object> Level3
        {
            get
            {
                var level3 = CreateDictionary("Level3");
                level3.Add("Subset", CreateDictionary("Subset"));
                return level3;
            }
        }


        private IDictionary<string, object> CreateDictionary(string keyPrefix)
        {
            IDictionary<string, object> ret = new Dictionary<string, object>();

            ret.Add(keyPrefix + "_" + "Int", 123);
            ret.Add(keyPrefix + "_" + "Date", DateTime.Now);
            ret.Add(keyPrefix + "_" + "String", "Json Sample");

            return ret;
        }
    }
}
