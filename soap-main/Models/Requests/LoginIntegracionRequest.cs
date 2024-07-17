using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WcfRecepcionSOAP.Models.Requests
{
    public class LoginIntegracionRequest
    {
        [JsonProperty("user", Required = Required.Always)]
        public string User { get; set; }

        [JsonProperty("pasword", Required = Required.Always)]
        public string Password { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

       
    }
}