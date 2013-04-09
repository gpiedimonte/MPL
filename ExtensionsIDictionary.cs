using System;
using System.Collections.Generic;

namespace System
{
    public static class ExtensionsIDictionary
    {

        /// <summary>
        /// Restituisce il valore associato alla chiave. Se non viene trovato
        /// nulla allora aggiunge il valore specificato al dizionario
        /// </summary>
        /// <param name="dictionary">il dizionario di interesse</param>
        /// <param name="key">la chiave da ricercare o da aggiungere</param>
        /// <param name="valueToReturnIfNull">il valore da aggiungere se la ricerca non da esito positivo</param>
        /// <returns>il valore restituito o creato</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue valueToReturnIfNull)
        {
            TValue value;
            if (!dictionary.ContainsKey(key))
            {
                value = valueToReturnIfNull;
                dictionary.Add(key, value);
            }
            else
            {
                value = dictionary[key];
            }
            return value;
        }

        /// <summary>
        /// Restituisce il valore associato alla chiave. Se non viene trovato
        /// nulla allora aggiunge una nuova istanza del valore al dizionario
        /// </summary>
        /// <param name="dictionary">il dizionario di interesse</param>
        /// <param name="key">la chiave da ricercare o da aggiungere</param>
        /// <returns>il valore restituito o creato</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
           where TValue : new()
        {
            return GetOrCreate(dictionary, key, new TValue());
        }

    }

    public static class ExtensionsQueue
    {
        /// <summary>
        /// Ritorna l'oggetto in coda. Se la coda è vuota ritorna il default value
        /// </summary>
        public static TValue DequeueOrDefault<TValue>(this Queue<TValue> queue, TValue defaultValue = default(TValue))
        {
            try
            {
                return queue.Dequeue();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
