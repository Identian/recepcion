﻿namespace CapaDominio.Response
{
    public class ResponseGeneral : IResponseGeneral
    {
        public string? token_type { get; set; }

        public string? scope { get; set; }

        public string? expires_in { get; set; }

        public string? ext_expires_in { get; set; }

        public string? access_token { get; set; }

        public string? refresh_token { get; set; }
    }
}
