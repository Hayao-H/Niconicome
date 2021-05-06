using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml
{
    static class Xmlparser
    {
        /// <summary>
        /// オブジェクトをXML形式でシリアル化する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize<T>(T data, XmlWriterSettings? settings = null)
        {
            var serializer = new XmlSerializer(typeof(T));
            if (settings is null)
            {
                settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true
                };
            }
            var emptyns = new XmlSerializerNamespaces();
            emptyns.Add(string.Empty, string.Empty);
            var stringWriter = new StringWriter();
            var writer = XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(writer, data, emptyns);

            return stringWriter.ToString();
        }

        /// <summary>
        /// XML形式の文字列をデシリアライズする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T? Deserialize<T>(string content)
        {
            using var stream = new StringReader(content);
            var serializer = new XmlSerializer(typeof(T));

            T? data = (T?)serializer.Deserialize(stream);

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new InvalidOperationException("シリアライズしようとしているコンテンツはnullです。");
            }
        }
    }
}
