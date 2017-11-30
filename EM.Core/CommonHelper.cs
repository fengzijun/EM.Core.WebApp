using CryptoModule;
using EM.ComponentModel;
using EM.Configuration;
using EM.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace EM
{
    /// <summary>
    /// 公共类
    /// </summary>
    public partial class CommonHelper
    {
        protected static string solidIV = "RlF7sNgSUBgehF0NWviBPA==";
        protected static string solidKey = "RLu2rWULa0uC78VfIZgNN2yMI8xXB7TMipEcxnQuX8U=";

        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Default.GetBytes(strText);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        /// <summary>
        /// 获取内网IP
        /// </summary>
        /// <returns></returns>
        public static string GetInternalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// 解密通行证的手机号
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string DecryptDES(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
                return string.Empty;
        
            try
            {
                return AES.Decrypt(encryptString.GetBase64(), solidIV.GetBase64(), solidKey.GetBase64()).GetString();
            }
            catch (Exception e)
            {
                return string.Empty;

            }

        }

     
        /// <summary>
        /// Ensures the subscriber email or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new ArticleException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }

            return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            if (str == null)
                return string.Empty;

            return str;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            bool result = false;
            Array.ForEach(stringsToValidate, str =>
            {
                if (string.IsNullOrEmpty(str)) result = true;
            });
            return result;
        }

        /// <summary>
        /// Compare two arrasy
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        private static AspNetHostingPermissionLevel? _trustLevel;

        /// <summary>
        /// Finds the trust level of the running application (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            if (!_trustLevel.HasValue)
            {
                //set minimum
                _trustLevel = AspNetHostingPermissionLevel.None;

                //determine maximum
                foreach (AspNetHostingPermissionLevel trustLevel in new[] {
                                AspNetHostingPermissionLevel.Unrestricted,
                                AspNetHostingPermissionLevel.High,
                                AspNetHostingPermissionLevel.Medium,
                                AspNetHostingPermissionLevel.Low,
                                AspNetHostingPermissionLevel.Minimal
                            })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        _trustLevel = trustLevel;
                        break; //we've set the highest permission we can
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }
                }
            }
            return _trustLevel.Value;
        }

        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new ArticleException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new ArticleException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();

            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                TypeConverter destinationConverter = GetCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get difference in years
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            int age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        public static string GenerateOrderNumber()
        {
            string strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmssms");
            strDateTimeNumber = strDateTimeNumber.PadRight(18, '0');
            string strRandomResult = GetRandomSeed();
            var config = EngineContext.Current.Resolve<SystemConfig>();
            if (config != null && config.IsDev)
                return strDateTimeNumber + strRandomResult + "0";
            return strDateTimeNumber + strRandomResult;
        }

        public static string GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            var tempresult = BitConverter.ToInt16(bytes, 0);
            var result = tempresult.ToString().Replace("-", "");
            if (result.Length < 4)
                result = result.PadLeft(4, '0');
            else
                result = result.Substring(0, 4);
            return result;
        }

        /// <summary>
        /// 参考：msdn上的RNGCryptoServiceProvider例子
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int NextRandom(int numSeeds, int length)
        {
            // Create a byte array to hold the random value.
            byte[] randomNumber = new byte[length];
            // Create a new instance of the RNGCryptoServiceProvider.
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.
            rng.GetBytes(randomNumber);
            // Convert the byte to an uint value to make the modulus operation easier.
            uint randomResult = 0x0;
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)randomNumber[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds) + 1;
        }

        /// <summary>
        /// post data
        /// </summary>
        /// <param name="url"></param>
        /// <param name="byteArray"></param>
        public static string PostRequest(string url, byte[] byteArray)
        {
            ILogger logger = EngineContext.Current.Resolve<ILogger>();
            try
            {
                HttpWebRequest requestPage = (HttpWebRequest)WebRequest.Create(url);
                requestPage.Method = "Post";
                requestPage.ContentLength = byteArray.Length;
                requestPage.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = requestPage.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                var response = requestPage.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch(Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(url, ex);

                return null;
            }
        }

        public static string GetRequest(string url)
        {
            ILogger logger = EngineContext.Current.Resolve<ILogger>();
            try
            {

                WebRequest hr = HttpWebRequest.Create(url);
                hr.Method = "GET";
                System.Net.WebResponse response = hr.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string ReturnVal = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return ReturnVal;
            }
            catch(Exception ex)
            {
                if (logger != null)
                    logger.WriteErrorLog(url, ex);
                return null;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var len = text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x;
            for (x = 0; x < len; x++)
            {
                var i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            var bKey = Encoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string text, string sKey)
        {
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(text);
            var bKey = Encoding.ASCII.GetBytes(Md5Hash(sKey).Substring(0, 8));
            des.Key = bKey;
            des.IV = bKey;
            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        //// <summary>
        /// 取得MD5加密串
        /// </summary>
        /// <param name="input">源明文字符串</param>
        /// <returns>密文字符串</returns>
        public static string Md5Hash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (var b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }

        public static string HmacSha256(string plain)
        {
            string secretKey = "eastmoney88";
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var plainBytes = Encoding.UTF8.GetBytes(plain);

            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                var sb = new StringBuilder();
                var hashValue = hmacsha256.ComputeHash(plainBytes);
                foreach (byte x in hashValue)
                {
                    sb.Append($"{x:x2}");
                }
                return sb.ToString();
            }
        }

        public static string MakeSignPlain(Dictionary<string, string> queryString)
        {
            var sb = new StringBuilder();
            foreach (var keyValue in queryString)
            {
                sb.AppendFormat("{0}={1}&", keyValue.Key, keyValue.Value);
            }
            if (sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public static bool Valid(string requestSign, string signPlain)
        {
            if (string.IsNullOrEmpty(requestSign) || string.IsNullOrEmpty(signPlain))
            {
                return false;
            }

            //hashmac
            var sign = HmacSha256(signPlain);

            return requestSign.Equals(sign, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public static string Decrypt(string cipher)
        {
            if (string.IsNullOrWhiteSpace(cipher)) return "";
            string solidKey = "RLu2rWULa0uC78VfIZgNN2yMI8xXB7TMipEcxnQuX8U=";
            string solidIV = "RlF7sNgSUBgehF0NWviBPA==";
            return AES.Decrypt(cipher.GetBase64(), solidIV.GetBase64(), solidKey.GetBase64()).GetString();
        }
    }
}