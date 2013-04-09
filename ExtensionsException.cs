using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System
{
    /// <summary>
    /// Extension method di utilità per Exception
    /// <remarks>
    /// EVENTUALI NUOVI EXTENSION METHOD DEVONO ESSERE SUFFICIENTEMENTE GENERICI E APPARTENERE ALLE REGION GIUSTE
    /// </remarks>
    /// </summary>
    public static class ExtensionsException
    {
        /// <summary>
        /// Un'eccezione può essere rilanciata tramite "throw;". In questo caso non perdo il
        /// contesto né lo stacktrace. In altri casi è necessario rilanciare proprio la
        /// "throw ex;" ma in questo caso lo stacktrace viene rimpiazzato con quello attuale.
        /// Per impedire questo, prima della rethrow invocare questo metodo o prima di passarlo
        /// a qualche metodo
        /// </summary>
        /// <param name="e">l'eccezione di cui preservare il contesto</param>
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public static void PreserveStackTrace(this Exception e)
        {
            var ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
            var mgr = new ObjectManager(null, ctx);
            var si = new SerializationInfo(e.GetType(), new FormatterConverter());

            e.GetObjectData(si, ctx);
            mgr.RegisterObject(e, 1, si); // prepare for SetObjectData
            mgr.DoFixups(); // ObjectManager calls SetObjectData

            // voila, e is unmodified save for _remoteStackTraceString
        }

    }
}
