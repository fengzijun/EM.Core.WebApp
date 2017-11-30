using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace EM
{
    public static class Extensions
    {

        public static string GenerateKey(this object sourceObject)
        {
            String hashString;

            //Catch unuseful parameter values
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Null as parameter is not allowed");
            }
            else
            {
                //We determine if the passed object is really serializable.
                try
                {
                    //Now we begin to do the real work.
                    hashString = ComputeHash(ObjectToByteArray(sourceObject));
                    return hashString;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                //If something occurred during serialization, 
                //this method is called with a null argument. 
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
                formatter.Serialize(fs, objectToSerialize);
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occurred during serialization. Message: " +
                se.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        /// <summary>
        /// 将BASE64字符串转字节数组
        /// </summary>
        /// <param name="str">BASE64字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }

        public static T Clone<T>(this T value) where T : class
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);
            stream.Position = 0;
            return formatter.Deserialize(stream) as T;
        }

        public static List<T> CloneList<T>(this List<T> list) where T : class
        {
            List<T> result = new List<T>();
            list.ForEach((item) => result.Add(item));
            return result;
        }

        public static List<SqlParameter> CloneSqlParameterList(this List<SqlParameter> list)
        {
            List<SqlParameter> result = new List<SqlParameter>();
            list.ForEach((item) => result.Add(new SqlParameter(item.ParameterName, item.Value)));
            return result;
        }

        internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object valueObj;
            if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
            {
                value = (T)valueObj;
                return true;
            }

            value = default(T);
            return false;
        }

      
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            return dictionary.TryGetValue(key, out obj) ? obj : default(TValue);
        }

    
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            TValue obj;
            if (dictionary.TryGetValue(key, out obj))
            {
                return obj;
            }

            return dictionary[key] = factory(key);
        }

     
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, k => factory());
        }
    }
}