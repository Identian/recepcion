using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WcfRecepcionSOAP
{
    public class RecepcionJsonSerializer : IJsonSerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public RecepcionJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            this._serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static RecepcionJsonSerializer Default => new RecepcionJsonSerializer(new Newtonsoft.Json.JsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}