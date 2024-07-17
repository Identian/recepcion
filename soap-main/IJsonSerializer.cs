
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace WcfRecepcionSOAP
{
    public interface IJsonSerializer : ISerializer, IDeserializer
    {

    }
}