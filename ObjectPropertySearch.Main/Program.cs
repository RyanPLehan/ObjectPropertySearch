using System;
using ObjectPropertySearch.Domain;
using ObjectPropertySearch.Domain.Formatters;


namespace ObjectPropertySearch.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Manually created dictionarys
            SearchJson(Json.Serialize(CreateDictionary()));

            Console.WriteLine();

            // Object from 
            SearchJson(Json.Serialize(new Sample()));
        }

        private static void SearchJson(string json)
        {
            JsonSearch searcher = new JsonSearch(json);
            DisplayValue("root_string", searcher);
            DisplayValue("ROOT_STRING", searcher);
            DisplayValue("Level2.level2_int", searcher);
            DisplayValue("Level3.subset.subset_date", searcher);
        }


        private static void DisplayValue(string key, JsonSearch searcher)
        {
            Console.WriteLine(String.Format("Key : {0} - Value: {1}", key, searcher.GetValue(key)));
        }

        private static IDictionary<string, object> CreateDictionary()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("root" + "_" + "Int", 123);
            dict.Add("root" + "_" + "Date", DateTime.Now);
            dict.Add("root" + "_" + "String", "Dictionary Sample");
            dict.Add("Level2", CreateDictionary("Level2"));

            var level3 = CreateDictionary("Level3");
            level3.Add("Subset", CreateDictionary("Subset"));

            dict.Add("Level3", level3);

            return dict;
        }

        private static IDictionary<string, object> CreateDictionary(string keyPrefix)
        {
            IDictionary<string, object> ret = new Dictionary<string, object>();

            ret.Add(keyPrefix + "_" + "Int", 123);
            ret.Add(keyPrefix + "_" + "Date", DateTime.Now);
            ret.Add(keyPrefix + "_" + "String", "Json Sample");
            return ret;
        }

    }
}
