using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace ConsoleMThreads.Entity
{

    public partial class ZhirenResponse
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
    public partial class ZhirenResponse
    {
        public static ZhirenResponse[] FromJson(string json) => 
        JsonConvert.DeserializeObject<ZhirenResponse[]>(json, Converter.Settings);
      
    }

    public static class Serialize
    {
        public static string ToJson(this ZhirenResponse[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
