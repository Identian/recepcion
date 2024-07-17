using Newtonsoft.Json;
using System.Text.Json;


namespace Domain.TokenContext
{
    public class CustomJwtTokenContext
    {
        [JsonProperty("enterpriseNit", Required = Required.Always)]
        public string EnterpriseNit { get; set; } = null!;

        [JsonProperty("enterpriseToken", Required = Required.Always)]
        public string EnterpriseToken { get; set; } = null!;

        [JsonProperty("entepriseId", Required = Required.Always)]
        public string EnterpiseId { get; set; } = null!;

        [JsonProperty("enterpriseschemeid", Required = Required.Always)]
        public string EnterpiseSchemeId { get; set; } = null!;
    }
}
