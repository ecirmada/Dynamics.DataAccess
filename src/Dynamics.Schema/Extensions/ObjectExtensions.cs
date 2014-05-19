using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dynamics.Schema.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeObject<T>(this T toSerialize) where T : class
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());
            var textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        public static T DeSerializeToObject<T>(this string serializedString) where T: class
        {
            
            var xmlSerializer = new XmlSerializer(typeof(T));
            var textReader = new StringReader(serializedString);
            return (T) xmlSerializer.Deserialize(textReader);
        }
    }
}
