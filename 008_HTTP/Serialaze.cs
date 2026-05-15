using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace _008_HTTP
{
    public class Serializer
    {
        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(input))
            {
                return (T)xmlSerializer.Deserialize(sr);
            }
        }
    }

}
