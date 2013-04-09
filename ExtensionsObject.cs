using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace System
{
    /// <summary>
    /// Extension method di utilità per Object
    /// <remarks>
    /// EVENTUALI NUOVI EXTENSION METHOD DEVONO ESSERE SUFFICIENTEMENTE GENERICI E APPARTENERE ALLE REGION GIUSTE
    /// </remarks>
    /// </summary>
    public static class ExtensionsObject
    {
        /// <summary>
        /// Clona l'oggetto. Il tipo dell'oggetto deve essere serializzabile.
        /// </summary>
        /// <returns>L'oggetto copiato</returns>
        public static T Clone<T>(this T item)
        {
            if ((object)item != null && !item.Equals(default(T)))
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter(null,
                         new StreamingContext(StreamingContextStates.Clone));
                    formatter.Serialize(stream, item);
                    stream.Seek(0, SeekOrigin.Begin);
                    var result = (T)formatter.Deserialize(stream);
                    return result;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Crea una rappresentazione xml dell'oggetto. La classe dell'oggetto deve
        /// dichiarare l'attributo XmlSerializable 
        /// </summary>
        public static string ToXml<T>(this T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Crea una rappresentazione xml dell'oggetto. La classe dell'oggetto deve
        /// dichiarare l'attributo XmlSerializable 
        /// </summary>
        public static void ToXml<T>(this T obj, Stream outputStream)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(outputStream, obj);
        }

        /// <summary>
        /// Crea l'oggetto dall'xml
        /// </summary>
        public static T ToObject<T>(this string xml)
        {
            if (xml == null) return default(T);
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringReader(xml))
            {
                var objDeserialized = serializer.Deserialize(sw);
                return (T)objDeserialized;
            }
        }

        /// <summary>
        /// Crea l'oggetto dall'xml
        /// </summary>
        public static T ToObject<T>(this string xml, XmlRootAttribute rootAttribute)
        {
            if (xml == null) return default(T);
            var serializer = new XmlSerializer(typeof(T), rootAttribute);
            using (var sw = new StringReader(xml))
            {
                var objDeserialized = serializer.Deserialize(sw);
                return (T)objDeserialized;
            }
        }

        /// <summary>
        /// Crea l'oggetto dall'xml
        /// </summary>
        public static T ToObjectOrDefault<T>(this string xml, XmlRootAttribute rootAttribute = null)
        {
            try
            {
                if (rootAttribute == null)
                    return xml.ToObject<T>();
                else
                    return xml.ToObject<T>(rootAttribute);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Ritorna l'array di bytes da uno stream e reimposta il seek originale
        /// </summary>
        public static byte[] GetBytes(this Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// Crea l'oggetto dall'xml
        /// </summary>
        public static T ToObject<T>(this Stream stream)
        {
            if (stream == null) return default(T);
            var serializer = new XmlSerializer(typeof(T));
            var objDeserialized = serializer.Deserialize(stream);
            return (T)objDeserialized;
        }

        /// <summary>
        /// Crea una rappresentazione xml dell'oggetto. La classe dell'oggetto deve
        /// dichiarare l'attributo XmlSerializable 
        /// </summary>
        public static string ToXml(this object obj, Type type)
        {
            var serializer = new XmlSerializer(type);
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Restituisce l'oggeto serializzato in Binario
        /// </summary>
        public static byte[] ToBinary(this object obj)
        {
            if (!obj.GetType().IsSerializable)
                throw new ArgumentException("The type must be serializable.", "obj");

            MemoryStream stream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Close();
            return stream.GetBuffer();
        }

        /// <summary>
        /// Restituisce lo stream aperto
        /// </summary>
        public static Stream ToBinaryStream(this object obj)
        {
            if (!obj.GetType().IsSerializable)
                throw new ArgumentException("The type must be serializable.", "obj");

            MemoryStream stream = new MemoryStream();
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Crea l'oggetto dai byte
        /// </summary>
        public static T ToObject<T>(this byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BinaryFormatter bFormatter = new BinaryFormatter();
            var objectDeserialied = (T)bFormatter.Deserialize(stream);
            stream.Close();
            return objectDeserialied;
        }

        #region PerformanceCounterCategory

        /// <summary>
        /// Add a Counter in existing performance category
        /// </summary>
        /// <param name="counters">Counters to Add</param>
        /// <param name="preservedOldValues">Preserve values of old counters</param>
        public static void Add(this PerformanceCounterCategory target, IEnumerable<CounterCreationData> counters, bool preservedOldValues)
        {
            if(string.IsNullOrEmpty(target.CategoryName) || !PerformanceCounterCategory.Exists(target.CategoryName))
                throw new Exception("Category not found");

            var oldCounters = new CounterCreationDataCollection();
            if(preservedOldValues)
            {
                var preservedValues = target.GetCounters().ToDictionary(currentCounter => currentCounter.CounterName, 
                    currentCounter => new {
                        Name = currentCounter.CounterName,
                        Help = currentCounter.CounterHelp,
                        Type = currentCounter.CounterType, 
                        Value = currentCounter.RawValue
                    }
                    );

                preservedValues.Values.ForEach(value => oldCounters.Add(new CounterCreationData()
                                                                         {
                                                                             CounterHelp = value.Help,
                                                                             CounterName = value.Name,
                                                                             CounterType = value.Type
                                                                         }));
                oldCounters.AddRange(counters.ToArray());
                PerformanceCounterCategory.Create(target.CategoryName, target.CategoryHelp,PerformanceCounterCategoryType.MultiInstance, oldCounters);

                foreach (var preservedValue in preservedValues)
                {
                    var performanceCounter = new PerformanceCounter(target.CategoryName, preservedValue.Key, false);
                    performanceCounter.RawValue = preservedValue.Value.Value;
                }
            }
            else
            {
                oldCounters.AddRange(counters.ToArray());
                PerformanceCounterCategory.Create(target.CategoryName, target.CategoryHelp, PerformanceCounterCategoryType.MultiInstance, oldCounters);
            }
        }

        #endregion

        #region Drawing
        
        public static Image Resize(this Image img, short thumbWidthMax, short thumbHeightMax)
        {
            short width, height;
            double ratio = (double)img.Width / img.Height;

            if (ratio > 1)
            {
                width = thumbWidthMax;
                height = (short)(thumbWidthMax / ratio);
            }
            else
            {
                height = thumbHeightMax;
                width = (short)(thumbHeightMax * ratio);
            }

            Image result = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
            return result;
        }

        /// <summary>
        /// Ritorna un memory stream aperto in posizione 0 dell'immagine
        /// </summary>
        /// <param name="img">Immagine da serializzare</param>
        /// <param name="format">Se null di default image format è Jpeg</param>
        /// <returns></returns>
        public static MemoryStream GetMemoryStream(this Image img, ImageFormat format = null)
        {
            var memoryStream = new MemoryStream();
            if (format == null)
                format = ImageFormat.Jpeg;

            img.Save(memoryStream, format);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        #endregion
    }

}