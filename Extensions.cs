using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Security;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System
{
    public static class Extensions
    {
        /// <summary>
        /// Calcola il prossimo DateTime in cui si verifica l'ora specificata. Se ad esempio la data
        /// corrente è "25/10/2010 15:10" e absoluteTime è "12:45" il risultato sarà "26/10/2010 12:45"
        /// </summary>
        public static DateTime RoundDateTime(this DateTime date, TimeSpan absoluteTime)
        {
            DateTime result = new DateTime(date.Year, date.Month, date.Day, absoluteTime.Hours, absoluteTime.Minutes, absoluteTime.Seconds, absoluteTime.Milliseconds);
            TimeSpan timeSpanFromDate = new TimeSpan(0, date.Hour, date.Minute, date.Second, date.Millisecond);
            if (absoluteTime < timeSpanFromDate)
            {
                result = result.AddDays(1);
            }
            return result;
        }

        /// <summary>
        /// Salva il messaggio raw in una folder di output. 
        /// Restituisce il path completo del file creato (folder specificata + file)
        /// </summary>
        public static string Save(this MailMessage mailMessage, string folderDestination)
        {
            if (!Directory.Exists(folderDestination)) Directory.CreateDirectory(folderDestination);

            var guid = Guid.NewGuid();
            var path = Path.Combine(folderDestination, guid.ToString());
            Directory.CreateDirectory(path);

            var copier = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = path,
                EnableSsl = false
            };

            // salva eml nella directory specificata (non invia nulla in realtà)
            copier.Send(mailMessage);

            var fileCreated = Directory.GetFiles(path).FirstOrDefault();
            return fileCreated;
        }

        /// <summary>
        /// Confronta due date, prendendo in considerazione solo giorno, mese e anno
        /// </summary>
        public static bool EqualsByYearMonthDay(this DateTime date, DateTime dateToCompare)
        {
            if (date.Day == dateToCompare.Day && date.Year == dateToCompare.Year && date.Month == dateToCompare.Month)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Confronta due stringhe, escludendo eventuali null e ignorando maiuscole/minuscole
        /// </summary>
        public static bool EqualsSafeIgnoreCase(this string str, string strToCompare)
        {
            return str.ToStringSafe().Equals(strToCompare.ToStringSafe(), StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Esegue l'intersezione tra tutti gli insiemi
        /// </summary>
        public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> containerList)
        {
            IEnumerable<T> tempList = containerList.First();

            foreach (IEnumerable<T> list in containerList)
            {
                tempList = tempList.Intersect<T>(list);
            }

            return tempList;
        }

        //HACK: Modificato da come l'aveva scritto riccardo. E' giusto?
        /// <summary>
        /// Verifica se la stringa corrente contiene una delle stringhe passate
        /// </summary>
        public static bool ContainsOneOfStrings(this string testData, params string[] strings)
        {
            return strings.ToList().Exists(testData.Contains);
        }

        /// <summary>
        /// Verifica se la stringa corrente contiente tutte le stringhe passate
        /// </summary>
        public static bool ContainsAllOfStrings(this string testData, params string[] strings)
        {
            return strings.ToList().All(testData.Contains);
        }

        /// <summary>
        /// Verifica se la stringa corrente è uguale a una delle stringhe passate
        /// </summary>
        public static bool EqualsOneOfStrings(this string testData, params string[] strings)
        {
            return strings.ToList().Exists(testData.Equals);
        }

        /// <summary>
        /// Verifica se la stringa corrente è uguale a una delle stringhe passate
        /// </summary>
        public static bool EqualsOneOfStringsCaseInsensitive(this string testData, params string[] strings)
        {
            return strings.ToList().Exists(s => testData.Equals(s, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Restituisce il testo della data secondo il formato specificato, "-" altrimenti
        /// </summary>
        public static string ToDateTimeText(this string text, string format)
        {
            DateTime? dateTime = text.ToDateTime();
            return dateTime.ToDateTimeText(format, "-");
        }

        /// <summary>
        /// Restituisce il testo della data secondo il formato specificato, "-" altrimenti
        /// </summary>
        public static string ToDateTimeText(this DateTime? date, string format)
        {
            return date.ToDateTimeText(format, "-");
        }

        /// <summary>
        /// Restituisce il testo della data secondo il formato specificato, "-" altrimenti
        /// </summary>
        public static string ToDateTimeText(this DateTime? date, string format, string nullValue)
        {
            string result = String.Format("{0:" + format + "}", date);
            if (String.IsNullOrEmpty(result))
            {
                result = nullValue;
            }
            return result;
        }


        /// <summary>
        /// Restituisce il testo della data secondo il formato specificato, "-" altrimenti
        /// </summary>
        public static string ToDateTimeTextConvert(this string text, string sourceFormat, string targetFormat, string nullValue)
        {
            DateTime? dateTime = text.ToDateTime(sourceFormat);
            return dateTime.ToDateTimeText(targetFormat, nullValue);
        }

        /// <summary>
        /// Restituisce il ToString evitando eventuali NullReferenceException
        /// </summary>
        public static string ToStringSafe(this object obj, string nullText = "")
        {
            if (obj == null) return nullText;
            return obj.ToString();
        }

        /// <summary>
        /// Restituisce l'enumerato dal valore. Eccezione se l'enumerato non contiene il valore indicato
        /// </summary>
        public static T ToEnum<T>(this int integer)
        {
            return (T) Convert.ChangeType(integer, typeof (T));
        }

        /// <summary>
        /// Restituisce l'enumerato dal valore. Eccezione se l'enumerato non contiene il valore indicato
        /// </summary>
        public static T ToEnum<T>(this short smallInteger)
        {
            return (T)Convert.ChangeType(smallInteger, typeof(T));
        }

        /// <summary>
        /// Restituisce l'enumerato dal valore. Eccezione se l'enumerato non contiene il valore indicato
        /// </summary>
        public static T ToEnum<T>(this byte tinyInteger)
        {
            return (T)Convert.ChangeType(tinyInteger, typeof(T));
        }

        /// <summary>
        /// Restituisce l'istanza evitando eventuali NullReferenceException
        /// </summary>
        public static T NullSafe<T>(this T obj)
            where T : new()
        {
            if (obj == null) return new T();
            return obj;
        }

        /// <summary>
        /// ToLower senza NullReferenceException
        /// </summary>
        public static string ToLowerSafe(this string @string)
        {
            return @string.ToStringSafe().ToLower();
        }

        /// <summary>
        /// Converte un testo immesso da input tipicamente "sporco"
        /// con spazi non voluti o maiuscole non volute in una stringa pulita
        /// </summary>
        public static string FormattedSafe(this string @string)
        {
            if (@string == null) return null;
            var stringFormatted = @string.Trim();
            stringFormatted = stringFormatted.ToLower();
            return stringFormatted;
        }

        /// <summary>
        /// Casta solo se l'oggetto ha un valore
        /// </summary>
        public static T As<T>(this object obj, T defaultValue = default(T))
        {
            return (obj is T) ? (T)obj : defaultValue;
        }

        /// <summary>
        /// Se presenti i caratteri terminatori li elimina
        /// </summary>
        public static string NormalizeNullValue(this string s)
        {
            if (s == null) return null;
            var result = new StringBuilder();
            foreach (var c in s)
            {
                if (c != '\0')
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Restituisce Y o N a seconda del valore del booleano
        /// </summary>
        public static string ToStringYorN(this bool boolean)
        {
            return boolean ? "Y" : "N";
        }

        /// <summary>
        /// Restituisce Y o N a seconda del valore del booleano
        /// </summary>
        public static string ToStringYorN(this bool? boolean)
        {
            bool value = boolean.GetValueOrDefault();
            return value.ToStringYorN();
        }

        /// <summary>
        /// Restituisce Sì o No a seconda del valore del booleano
        /// </summary>
        public static string ToStringSiorNo(this string yOrN)
        {
            bool value = yOrN == "Y";
            return value.ToStringSiorNo();
        }

        /// <summary>
        /// Restituisce Sì o No a seconda del valore del booleano
        /// </summary>
        public static string ToStringSiorNo(this bool boolean)
        {
            return boolean ? "Sì" : "No";
        }

        /// <summary>
        /// Restituisce Sì o No a seconda del valore del booleano
        /// </summary>
        public static string ToString(this bool boolean, string yesText, string noText)
        {
            return boolean ? yesText : noText;
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        public static string ToStringCommaSeparated<TSource>(this IEnumerable<TSource> list, Func<TSource, string> selector)
        {
            var elements = list.Select(selector);
            return String.Join(",", elements.ToArray());
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        public static string ToStringCommaSeparated<TSource>(this IEnumerable<TSource> list)
        {
            return list.ToStringCommaSeparated(item => item.ToString());
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        public static string ToStringCommaSeparated(this IEnumerable list)
        {
            return list.Cast<IEnumerable<object>>().ToStringCommaSeparated();
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        //public static string ToStringCommaSeparated(this IEnumerable list, Func<object, string> selector)
        //{
        //    return list.Cast<IEnumerable<object>>().ToStringCommaSeparated(selector);
        //}

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        public static string ToStringCommaSeparated<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return ToStringWithFormat(dict, "[{0},{1}] ");
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista con un formato
        /// (esempio: "[{0},{1}] ")
        /// </summary>
        /// <param name="format">formato (esempio: "[{0},{1}] ")</param>
        public static string ToStringWithFormat<TKey, TValue>(this IDictionary<TKey, TValue> dict, string format)
        {
            var result = new StringBuilder();
            foreach (var pair in dict)
            {
                result.AppendFormat(format, pair.Key, pair.Value);
            }
            return result.ToString();
        }

        /// <summary>
        /// Restituisce una rappresentazione testuale della lista
        /// </summary>
        public static string ToStringCommaSeparated(this NameValueCollection dict)
        {
            StringBuilder result = new StringBuilder();
            foreach (var key in dict.AllKeys)
            {
                result.AppendFormat("[{0},{1}] ", key, dict[key]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Serializza in binario e codifica a base64
        /// </summary>
        public static string ToBinaryBase64(this object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memory = new MemoryStream();
            formatter.Serialize(memory, obj);
            byte[] data = memory.ToArray();
            string objSerialized = Convert.ToBase64String(data);
            return objSerialized;
        }

        /// <summary>
        /// Deserializza da binario in base64
        /// </summary>
        public static T ToObjectBase64<T>(this string base64)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] data = Convert.FromBase64String(base64);
            MemoryStream memory = new MemoryStream(data);
            object obj = formatter.Deserialize(memory);
            return (T)obj;
        }

        /// <summary>
        /// Copia uno stream in un altro stream notificandone il progresso
        /// </summary>
        public static void CopyTo(this Stream source, Stream destination, int bufferLength,
            Action<int> progressChanged)
        {
            var buffer = new byte[bufferLength];
            int bytesRead;
            int totalBytesRead = 0;
            int callbackBytesToOverflow = bufferLength;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                if (totalBytesRead > callbackBytesToOverflow)
                {
                    callbackBytesToOverflow += bufferLength;
                    progressChanged(totalBytesRead);
                }
            }
        }

        #region SecureString

        /// <summary>
        /// Converte in un SecureString immodificabile
        /// </summary>
        public static SecureString ToSecureString(this string s)
        {
            SecureString ss = new SecureString();
            foreach (char c in s.ToCharArray())
            {
               ss.AppendChar(c);
            }
            ss.MakeReadOnly();
            return ss;
        }

        /// <summary>
        /// Decifra la stringa.
        /// </summary>
        public static string ToCleanString(this SecureString ss)
        {
            if (ss == null) return "";
            IntPtr bstr = Marshal.SecureStringToBSTR(ss);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        #endregion

        /// <summary>
        /// Converte una coordinata in un radiante
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double ToRad(this double degree)
        {
            // Converts numeric degrees to radians 
            return degree * Math.PI / 180;
        }

        /// <summary>
        /// Converte un radiante in una coordinata
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToDeg(this double radian)
        {
            return radian * 180 / Math.PI;
        }

        /// <summary>
        /// Restituisce l'attributo del tipo specificato, se esiste
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this object obj)
            where TAttribute : Attribute
        {
            var type = obj.GetType();
            return GetAttribute<TAttribute>(type);
        }

        /// <summary>
        /// Restituisce l'attributo del tipo specificato, se esiste
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            var attribute = type.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(TAttribute));
            return attribute as TAttribute;
        } 

        /// <summary>
        /// Se il tipo è un Nullable di T restituisce il tipo T. Altrimenti se non lo è restituisce sé stesso
        /// </summary>
        public static Type GetNullableUnderlyingOrDefault(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType ?? type;
        }

        /// <summary>
        /// Restituisce il primo generic trovato, altrimenti sé stesso
        /// </summary>
        public static Type GetGenericUnderlyingOrDefault(this Type type)
        {
            var genericTypes = type.GetGenericArguments();
            return genericTypes.FirstOrDefault() ?? type;
        }

        /// <summary>
        /// Restituisce se la classe implementa l'interfaccia generica disinteressandosi
        /// del tipo concreto generico.
        /// 
        /// Quindi è possibile verificare se un type implementa IList&lt;&gt;
        /// </summary>
        public static bool ImplementsGenericInterface(this Type type, Type interfaceToCheck)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceToCheck);
        }
    }
}
