using Newtonsoft.Json;

namespace FGMM.SDK.Core.RPC
{
    public class Serializer
    {
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);
        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
