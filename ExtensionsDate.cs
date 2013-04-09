using System;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Extension method di utilità per Date
    /// <remarks>
    /// EVENTUALI NUOVI EXTENSION METHOD DEVONO ESSERE SUFFICIENTEMENTE GENERICI E APPARTENERE ALLE REGION GIUSTE
    /// </remarks>
    /// </summary>
    public static class ExtensionsDate
    {

        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 999);

        /// <summary>
        /// Verifica se la data è valida secondo range di date minime e massime
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsValid(this DateTime target)
        {
            return (target >= MinDate) && (target <= MaxDate);
        }

    }
}
