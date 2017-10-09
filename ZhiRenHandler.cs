using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleMThreads
{
    class ZhiRenHandler { 



        public static string attendance_punches = "/api/v2/attendance_punches";
        static RestClient client;
        static HMACSHA256 hmac;
        public static void init() {
            client = new RestClient(AppUtils.GetZhiRenUrl());
      
             hmac = new HMACSHA256(Encoding.ASCII.GetBytes(AppUtils.GetZhiRenSecretKey()));
        }

        public static void sendUserVerified(string userId, string time) {
            var requst = new RestRequest(attendance_punches, Method.POST);
            var json = new JsonObject();
            json.Add("empno", userId);
            json.Add("punched_at", time);
            var jsonArray = new JsonArray();
            jsonArray.Add(json);
            var payload = new JsonObject();
            payload.Add("attendances", jsonArray);

            Console.WriteLine(payload.ToString());
            GetCommonRequest(requst, payload.ToString());
        }

        //zhiren api 测试
        public static void GetSubCompany() {
            var requst = new RestRequest("/api/v2/sub_companies", Method.GET);
            GetCommonRequest(requst, "");
        }

        //网络请求统一走这里
        private static void GetCommonRequest(RestRequest request, String payload) {
            request.AddQueryParameter("access_key", AppUtils.GetZhiRenAccessKey());
            var tonce = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
            request.AddQueryParameter("tonce", tonce);
            request.AddQueryParameter("payload", payload);
            
            string string_to_sign = request.Method + request.Resource + AppUtils.GetZhiRenAccessKey() + tonce + payload;
            Console.WriteLine(string_to_sign);
            byte[] b = Encoding.ASCII.GetBytes(string_to_sign);
            string finalSignature = HashEncode(hmac.ComputeHash(b));
            request.AddQueryParameter("signature", finalSignature);
            Console.WriteLine(finalSignature);
        
            request.AddHeader("x-zhiren-signature", finalSignature);
            // easy async support
            client.ExecuteAsync(request, response => {
                Console.WriteLine(response.Content);
            });

        }
       

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
