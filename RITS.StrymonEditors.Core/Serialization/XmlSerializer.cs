using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RITS.StrymonEditor.Serialization
{
    /// <summary>
    /// Generic XmlSerializer; provides various methods to serialize and deserialize types marked with the Serializable attribute
    /// </summary>
    /// <typeparam name="T">The type to serialize.</typeparam>
    public class XmlStreamSerializer<T> : IDisposable
    {
        #region public methods

        /// <summary>
        /// Deserialize an xml stream
        /// </summary>
        /// <param name="stream">The XML stream.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        public T DeserializeStream(Stream stream)
        {
            return this.FromStream(stream);
        }

        /// <summary>
        /// Serialize the object to a stream
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A stream containing the object.</returns>
        public Stream SerializeToStream(T obj)
        {
            return this.ToStream(obj);
        } 
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.serializer = null;
            }
        }
        #endregion

        #region private methods


        /// <summary>
        /// Convert the object to a stream.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>A stream containing the object</returns>
        private Stream ToStream(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                this.serializer.Serialize(ms, obj);
                return ms;
            }
        }

        /// <summary>
        /// Get an object from an XML stream.
        /// </summary>
        /// <param name="stream">The XML stream.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        private T FromStream(Stream stream)
        {
            return (T)this.serializer.Deserialize(stream);
        } 
        #endregion

        #region private fields
        /// <summary>
        /// The XML serializer
        /// </summary>
        private XmlSerializer serializer = new XmlSerializer(typeof(T)); 
        #endregion
    }
}
