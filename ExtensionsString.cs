using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Extension method di utilità per String
    /// <remarks>
    /// EVENTUALI NUOVI EXTENSION METHOD DEVONO ESSERE SUFFICIENTEMENTE GENERICI E APPARTENERE ALLE REGION GIUSTE
    /// </remarks>
    /// </summary>
    public static class ExtensionsString
    {
        private static readonly Regex WebUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EmailExpression = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Restituisce se ha un formato conforme a un url
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsWebUrl(this string target)
        {
            return !string.IsNullOrEmpty(target) && WebUrlExpression.IsMatch(target);
        }

        /// <summary>
        /// Restituisce se ha un formato conforme a un email
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsEmail(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }

        /// <summary>
        /// Restituisce l'esito del classico String.Format. Utile poiché più semplice da leggere
        /// e permette la concatenazione
        /// </summary>
        [DebuggerStepThrough]
        public static string FormatWith(this string target, params object[] args)
        {
            //Check.Argument.IsNotEmpty(target, "target");

            return string.Format(Thread.CurrentThread.CurrentCulture, target, args);
        }

        /// <summary>
        /// In caso di errore restituisce la stessa stringa senza i marker per gli argomenti
        /// </summary>
        public static string StringFormatSafe(this string format, params object[] args)
        {
            try
            {
                return String.Format(format, args);
            }
            catch (Exception)
            {
                const string regex = "{[0-9]+}";
                return Regex.Replace(format, regex, "");
            }
        }

        /// <summary>
        /// Restituisce l'hash md5 della stringa
        /// </summary>
        [DebuggerStepThrough]
        public static string HashMD5(this string target)
        {
            //Check.Argument.IsNotEmpty(target, "target");

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.Unicode.GetBytes(target);
                byte[] hash = md5.ComputeHash(data);

                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Restituisce il wrap della stringa alla posizione specificata.
        /// Da quella posizione in poi concatena "..."
        /// </summary>
        /// <param name="target">la stringa di interesse</param>
        /// <param name="index">la posizione da cui wrappare</param>
        /// <returns>stringa wrappata</returns>
        [DebuggerStepThrough]
        public static string WrapAt(this string target, int index)
        {
            const int DotCount = 3;

            //Check.Argument.IsNotEmpty(target, "target");
            //Check.Argument.IsNotNegativeOrZero(index, "index");

            return (target.Length <= index) ? target : string.Concat(target.Substring(0, index - DotCount), new string('.', DotCount));
        }

        /// <summary>
        /// A partire da una stringa che rispetta il formato Guid crea
        /// il corrispondente oggetto Guid. In caso di formato non rispettato o
        /// di errori viene restituito un Guid vuoto.
        /// </summary>
        [DebuggerStepThrough]
        public static Guid ToGuid(this string target)
        {
            Guid result = Guid.Empty;

            if ((!string.IsNullOrEmpty(target)) && (target.Trim().Length == 22))
            {
                string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");

                try
                {
                    byte[] base64 = Convert.FromBase64String(encoded);

                    result = new Guid(base64);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Converte la stringa in un enumerato del tipo specificato e restituisce
        /// il risultato della conversione o il valore passato per parametro
        /// </summary>
        /// <param name="target">la stringa di interesse</param>
        /// <typeparam name="T">il tipo dell'enumerato da convertire</typeparam>
        /// <param name="defaultValue">il valore di default da restituire in caso di errori</param>
        [DebuggerStepThrough]
        public static T ToEnum<T>(this string target, T defaultValue = default(T))
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(target))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), target.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Se la stringa contiene numerici
        /// </summary>
        public static bool HasDigits(this string s)
        {
            if (s == null) return false;
            foreach (var c in s)
            {
                if (Char.IsDigit(c)) return true;
            }
            return false;
        }

        /// <summary>
        /// Tronca la stringa ai caratteri specificati
        /// </summary>
        public static string Truncate(this string s, int length)
        {
            if (String.IsNullOrEmpty(s) || s.Length < length) return s ?? String.Empty;
            return s.Substring(0, length);
        }

        /// <summary>
        /// Ritorna il numero minimo di modifiche elementari che consentono di trasformare una stringa in un'altra (Distanza di Levenshtein)
        /// </summary>
        /// ///
        /// <param name="target">la stringa di interesse</param>
        /// <param name="compareString">La stringa da cofrontare</param>
        public static int GetLevenshteinDistance(this string target, string compareString)
        {
            int n = target.Length; //length of s
            int m = compareString.Length; //length of t
            int[,] d = new int[n + 1, m + 1]; // matrix
            int cost; // cost

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    cost = (compareString.Substring(j - 1, 1) == target.Substring(i - 1, 1) ? 0 : 1);
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        /// <summary>
        /// Confronta due stringhe e restituisce il rapporto di ugualianza
        /// </summary>
        /// <param name="target">la stringa di interesse</param>
        /// <param name="compareString">La stringa da cofrontare</param>
        /// <returns>Rapporto da 0 a 1. 1 Se le stringhe sono uguali</returns>
        public static double GetSimilarity(this string target, string compareString)
        {
            if (string.IsNullOrEmpty(target) && string.IsNullOrEmpty(compareString))
                return 1;

            int levenshteinDistance = target.GetLevenshteinDistance(compareString);
            double ratio = (double)(levenshteinDistance * 100) /
                           (target.Length > compareString.Length
                                ? target.Length
                                : compareString.Length);
            var probability = 1 - (ratio / 100);
            return probability;
        }

    }
}
