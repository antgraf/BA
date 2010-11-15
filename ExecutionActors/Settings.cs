using System.Text;
using System.Xml.Serialization;
using System.IO;
using System;

namespace ExecutionActors
{
	public class Settings : XmlSerializableDictionary<string, object>
	{
		public string XmlSerialize(Type[] types = null)
		{
			XmlSerializer xmler = types == null ? new XmlSerializer(typeof(Settings)) : new XmlSerializer(typeof(Settings), types);
			MemoryStream stream = new MemoryStream();
			xmler.Serialize(stream, this);
			byte[] buf = stream.ToArray();
			return Encoding.UTF8.GetString(buf);
		}

		public static Settings XmlDeserialize(string xml, Type[] types = null)
		{
			XmlSerializer xmler = types == null ? new XmlSerializer(typeof(Settings)) : new XmlSerializer(typeof(Settings), types);
			byte[] buf = Encoding.UTF8.GetBytes(xml);
			MemoryStream stream = new MemoryStream(buf, false);
			return (Settings)xmler.Deserialize(stream);
		}
	}
}
