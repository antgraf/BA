using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ExecutionActors
{
	public class Settings : XmlSerializableDictionary<string, object>
	{
		public string XmlSerialize()
		{
			XmlSerializer xml = new XmlSerializer(typeof(Settings)/*, new Type[] { typeof(another_type) }*/);
			MemoryStream stream = new MemoryStream();
			xml.Serialize(stream, this);
			byte[] buf = stream.ToArray();
			return Encoding.UTF8.GetString(buf);
		}

		public static Settings XmlDeserialize(string xml)
		{
			XmlSerializer xmler = new XmlSerializer(typeof(Settings)/*, new Type[] { typeof(another_type) }*/);
			byte[] buf = Encoding.UTF8.GetBytes(xml);
			MemoryStream stream = new MemoryStream(buf, false);
			return (Settings)xmler.Deserialize(stream);
		}
	}
}
