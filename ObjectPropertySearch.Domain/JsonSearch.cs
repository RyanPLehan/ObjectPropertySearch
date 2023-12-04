using System;
using System.Collections;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;
using ObjectPropertySearch.Domain.Formatters;

namespace ObjectPropertySearch.Domain
{
    public class JsonSearch
    {
        public const char KEY_DELIMITER = '.';
        private readonly IDictionary<string, object> _searchRepo;

        public JsonSearch(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
                throw new ArgumentException(nameof(json));

            this._searchRepo = CreateSearchRepo(json);
        }

        /// <summary>
        /// Get a specific value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            string[] keys = ParseKey(key);
            return GetValue(keys);
        }


        /// <summary>
        /// Get first Item in an array defined by the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetFirst(string key)
        {
            object ret = null;
            object value = GetValue(key);

            // verify that object is an IEnumerable
            if (!(value is IEnumerable))
                throw new Exception(String.Format("Expected value at key '{0}' to be of type 'IEnumerable'", key));

            foreach(object listItem in (IEnumerable)value)
            {
                ret = listItem;
                break;
            }

            return ret;
        }

        /// <summary>
        /// Get first Item in an array defined by the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetLast(string key)
        {
            object ret = null;
            object value = GetValue(key);

            // verify that object is an IEnumerable
            if (!(value is IEnumerable))
                throw new Exception(String.Format("Expected value at key '{0}' to be of type 'IEnumerable'", key));

            foreach (object listItem in (IEnumerable)value)
            {
                ret = listItem;
            }

            return ret;
        }


        /// <summary>
        /// Given a list of keys, traverse through the search repo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private object GetValue(string[] keys)
        {
            object ret;
            string key = null;
            IDictionary<string, object> node = this._searchRepo;

            try
            {
                // Traverse through the tree of dictionary items
                int length = keys.Length;
                for (int i = 0; i < length - 1; i++)
                {
                    key = keys[i];
                    object value = Search(key, node);

                    // Make sure value is of specific type
                    if (value is IDictionary<string, object>)
                        node = (IDictionary<string, object>)Search(key, node);
                    else
                        throw new Exception(String.Format("Expected value at key '{0}' to be of type 'IDictionary<string, object>'", key));
                }

                key = keys[length - 1];
                ret = Search(key, node);
            }

            catch (ArgumentNullException ex)        // will be raised by the search method
            {
                throw new KeyNotFoundException(String.Format("Key '{0}' not found", key));
            }

            return ret;
        }

        private object Search(string key, IDictionary<string, object> node)
        {
            KeyValuePair<string, object> kvp = node.First(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            return kvp.Value;
        }

        private IDictionary<string, object> CreateSearchRepo(string json)
        {
            var options = Json.CreateOptionsWithObjectConverter();
            return Json.Deserialize<IDictionary<string, object>>(json, options);
        }

        /// <summary>
        /// This will parse the given key into a list
        /// </summary>
        /// <remarks>
        /// Key can have a single value (ie Name)
        /// Key can have multiple values separated by a delimiter (Person.Address.State)
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        private string[] ParseKey(string key)
        {
            return key.Split(KEY_DELIMITER, StringSplitOptions.TrimEntries);
        }

    }
}