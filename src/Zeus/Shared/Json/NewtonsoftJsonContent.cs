using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Zeus.v2.Shared.Json
{
    public class NewtonsoftJsonContent : StringContent
    {
        public NewtonsoftJsonContent(object instance) 
            : base(Serialize(instance), Encoding.UTF8, "application/json")
        { }

        public NewtonsoftJsonContent(object instance, JsonSerializerSettings settings) :
            base(Serialize(instance, settings), Encoding.UTF8, "application/json")
        { }

        public static string Serialize(object instance, JsonSerializerSettings settings = null)
        {
            return settings == null
                ? JsonConvert.SerializeObject(instance, Formatting.Indented)
                : JsonConvert.SerializeObject(instance, Formatting.Indented, settings);
        }
    }
}