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
    public class XmlSerializer<T> : IDisposable
    {
        #region public methods
        /// <summary>
        /// Deserialize an XmlDocument
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Meh!")]
        public T DeserializeXmlDocument(XmlDocument doc)
        {
            return this.FromXmlNode(doc);
        }
        
        /// <summary>
        /// Deserialize an XmlElement
        /// </summary>
        /// <param name="elem">The XML element.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Meh!")]
        public T DeserializeXmlElement(XmlElement elem)
        {
            return this.FromXmlNode(elem);
        }

        /// <summary>
        /// Deserialize string
        /// </summary>
        /// <param name="xml">The XML as a string.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        public T DeserializeString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return this.FromXmlNode(doc);
        }

        /// <summary>
        /// Deserialize an xml file
        /// </summary>
        /// <param name="path">The path to the XML file.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>.
        /// </returns>
        public T DeserializeFile(string path)
        {
            using (XmlTextReader reader = new XmlTextReader(path))
            {
                return (T)this.serializer.Deserialize(reader);
            }
        }

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
        /// Serialize the object to an XmlDocument
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An XML document.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Meh!")]
        public XmlDocument SerializeToXmlDocument(T obj)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.ToXml(obj));
            return doc;
        }

        /// <summary>
        /// Serialize the object to an XmlElement
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An XML element</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Meh!")]
        public XmlElement SerializeToXmlElement(T obj)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this.ToXml(obj));
            return doc.DocumentElement;
        }

        /// <summary>
        /// Serialize the object to a string
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The object as an XML string.</returns>
        public string SerializeToString(T obj)
        {
            return this.ToXml(obj);
        }

        /// <summary>
        /// Serialize the object to an xml file
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="path">The path to the XML file.</param>
        public void SerializeToFile(T obj, string path)
        {
            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {
                this.serializer.Serialize(fstream, obj);
            }
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
        /// Converts from an XML node.
        /// </summary>
        /// <param name="node">The XML node.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        private T FromXmlNode(XmlNode node)
        {
            using (XmlNodeReader reader = new XmlNodeReader(node))
            {
                return (T)this.serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Convert the object to XML.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>An XML string.</returns>
        private string ToXml(T obj)
        {
            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlTextWriter tw = new XmlTextWriter(sw);

                this.serializer.Serialize(tw, obj);

                return sw.ToString();
            }
        }

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
