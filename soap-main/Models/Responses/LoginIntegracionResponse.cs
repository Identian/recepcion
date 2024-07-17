using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WcfRecepcionSOAP.Models.Responses
{
    public class LoginIntegracionResponse
    {

        [JsonProperty("token", Required = Required.Always)]
        public string Token { get; set; }

        [JsonProperty("passwordExpiration", Required = Required.Always)]
        public DateTime PasswordExpiration { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);

        public static LoginIntegracionResponse FromJson(string data) => JsonConvert.DeserializeObject<LoginIntegracionResponse>(data);

    }
}