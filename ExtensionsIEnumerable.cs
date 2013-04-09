using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Extension method di utilità per IEnumerable
    /// <remarks>
    /// EVENTUALI NUOVI EXTENSION METHOD DEVONO ESSERE SUFFICIENTEMENTE GENERICI E APPARTENERE ALLE REGION GIUSTE
    /// </remarks>
    /// </summary>
    public static class ExtensionsIEnumerable
    {
        /// <summary>
        /// Check if two enumerable has the same content (order insensitive)
        /// </summary>
        public static bool EqualsContent<T>(this IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            if (enumerable1 == null || enumerable2 == null) return false;
            var coordinatesCount = enumerable1.Count();
            var intersectCount = enumerable1.Intersect(enumerable2).Count();
            return coordinatesCount == intersectCount;
        }

        /// <summary>
        /// Itera gli elementi eseguendo un'operazione su ogni elemento iterato
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> current, Action<T> iterationAction)
        {
            foreach (var obj in current)
            {
                iterationAction(obj);
            }
            return current;
        }

        /// <summary>
        /// Restituisce un elemento a caso o default se la lista è vuota
        /// </summary>
        public static T RandomOrDefault<T>(this IEnumerable<T> current, Random random = null)
        {
            if(random == null) random = new Random();
            var randomIndex = random.Next(0, current.Count());
            return current.ElementAtOrDefault(randomIndex);
        }

        /// <summary>
        /// Crea un array specificando il tipo dinamico.
        /// Se per esempio ho un IEnumerable di object ma voglio un
        /// array che dinamicamente è un array di Person allora qui lo
        /// specifico
        /// </summary>
        public static Array ToArray<T>(this IEnumerable<T> source, Type dynamicType)
        {
            var array = Array.CreateInstance(dynamicType, source.Count());
            int index = 0;
            foreach (var item in source)
            {
                array.SetValue(item, index);
                index++;
            }
            return array;
        }

    }
}
