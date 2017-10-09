// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var data = Company.FromJson(jsonString);
//
// For POCOs visit quicktype.io?poco
//
namespace QuickType
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Company
    {
        /// <summary>
        /// 母公司标识符
        /// </summary>
        [JsonProperty("company_uuid")]
        public string CompanyUuid { get; set; }

        [JsonProperty("registered_address")]
        public string RegisteredAddress { get; set; }

        [JsonProperty("boss")]
        public string Boss { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        /// <summary>
        /// 分公司标识符
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid { get; set; }
    }


    public partial class Company
    {
        public static Company FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Company>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Company self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
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
