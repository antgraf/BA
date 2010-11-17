using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;

// Credits:
// XmlSerializableDictionary class is partially based on work of Aaron Skonnard (MSDN Magazine - June 2003) and Paul Welter (Paul Welter's Weblog)

namespace ExecutionActors
{
	public enum DictionaryValuesTypeAttribute
	{
		No,
		Core,
		Specific
	}

	/// <summary>
	/// XML serializable version of generic dictionary collection.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	[XmlRoot("dictionary")]
	public class XmlSerializableDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>, IXmlSerializable
	{
		#region IXmlSerializable Members
		/// <summary>
		/// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
		/// </summary>
		/// <returns>An <see cref="System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="WriteXml"/> method and consumed by the <see cref="ReadXml"/> method.</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			// TODO: add schema
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();
			if(wasEmpty) return;
			while(reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");
				reader.ReadStartElement("key");
				TKey key = (TKey)keySerializer.Deserialize(reader);
				reader.ReadEndElement();
				string typeName = reader.GetAttribute("type");
				Type type = string.IsNullOrEmpty(typeName) ? typeof(TValue) : Type.GetType(typeName, true);
				reader.ReadStartElement("value");
// ReSharper disable AssignNullToNotNullAttribute
				TValue value = (TValue)new XmlSerializer(type).Deserialize(reader);
// ReSharper restore AssignNullToNotNullAttribute
				reader.ReadEndElement();
				Add(key, value);
				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
			foreach(TKey key in Keys)
			{
				writer.WriteStartElement("item");
				writer.WriteStartElement("key");
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();
				writer.WriteStartElement("value");
				TValue value = this[key];
				switch(ValuesTypeAttribute)
				{
					case DictionaryValuesTypeAttribute.Core:
					{
						writer.WriteAttributeString("type", value.GetType().ToString());
						break;
					}
					case DictionaryValuesTypeAttribute.Specific:
					{
						writer.WriteAttributeString("type", value.GetType().AssemblyQualifiedName);
						break;
					}
				}
				new XmlSerializer(value.GetType()).Serialize(writer, value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}
		#endregion

		[DefaultValue(DictionaryValuesTypeAttribute.Core)]
		public DictionaryValuesTypeAttribute ValuesTypeAttribute { get; set; }
	}

	/// <summary>
	/// XML serializer of non-generic dictionary collection.
	/// </summary>
	public class DictionaryXmlSerializer : IXmlSerializable
	{
		private const string ns = "http://www.develop.com/xml/serialization";

		/// <summary>
		/// Dictionary collection to serialize.
		/// </summary>
		public readonly System.Collections.IDictionary Dictionary;

		/// <summary>
		/// Default constructor. Initializes <see cref="Dictionary"/> member with a new <see cref="System.Collections.Hashtable"/>.
		/// </summary>
		public DictionaryXmlSerializer()
		{
			Dictionary = new System.Collections.Hashtable();
		}

		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="dictionary">Dictionary collection to serialize.</param>
		public DictionaryXmlSerializer(System.Collections.IDictionary dictionary)
		{
			Dictionary = dictionary;
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("dictionary", ns);
			foreach(object key in Dictionary.Keys)
			{
				object value = Dictionary[key];
				writer.WriteStartElement("item", ns);
				writer.WriteElementString("key", ns, key.ToString());
				writer.WriteElementString("value", ns, value.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader reader)
		{
			reader.Read(); // move past container
			reader.ReadStartElement("dictionary");
			while(reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item", ns);
				string key = reader.ReadElementString("key", ns);
				string value = reader.ReadElementString("value", ns);
				reader.ReadEndElement();
				reader.MoveToContent();
				Dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
		/// </summary>
		/// <returns>An <see cref="System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="WriteXml"/> method and consumed by the <see cref="ReadXml"/> method.</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			// TODO: add schema
			return null;
		}
	}
}
