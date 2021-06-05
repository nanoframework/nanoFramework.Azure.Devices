using System;
using System.Collections;
using System.Text;

namespace Microsoft.Azure.Devices.Client.Extensions
{
    internal delegate bool TryParse(string input, bool ignoreCase, out bool output);

    public static class ExtensionsHelpers
    {
        private const char ValuePairDelimiter = ';';
        private const char ValuePairSeparator = '=';

        public static void AppendKeyValuePairIfNotEmpty(this StringBuilder builder, string name, object value)
        {
            if (value != null)
            {
                builder.Append(name);
                builder.Append(ValuePairSeparator);
                builder.Append(value);
                builder.Append(ValuePairDelimiter);
            }
        }

        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) >= 0;
        }

        public static bool TryGetValue(this Hashtable hash, string key, out string value)
        {
            value = null;
            try
            {
                value = (string)hash[key];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Contains(this char[] cars, char c)
        {
            foreach (char car in cars)
            {
                if (car == c)
                {
                    return true;
                }
            }

            return false;
        }


        public static Hashtable ToDictionary(this string valuePairString, char kvpDelimiter, char kvpSeparator)
        {
            if (string.IsNullOrEmpty(valuePairString))
            {
                throw new ArgumentException("Malformed Token");
            }
            
            var collections = valuePairString.Split(kvpDelimiter);
            if(collections.Length == 0)
            {
                throw new FormatException();
            }

            Hashtable collec = new Hashtable();
            for (int i = 0; i < collections.Length; i++)
            {
                var firstSeparator = collections[i].IndexOf(kvpSeparator);
                var key = collections[i].Substring(0, firstSeparator);
                var value = collections[i].Substring(firstSeparator +1);
                collec.Add(key, value);
            }

            return collec;

        }
    }
}
