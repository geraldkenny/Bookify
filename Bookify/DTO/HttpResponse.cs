using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookify.DTO
{
    public class HttpResponseDTO
    {
        public string StatusCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class HttpResponse<T, T2>
    {
        public HttpResponse(int statusCode, string responseMessage, dynamic data )
        {
            StatusCode = statusCode;
            ResponseMessage = responseMessage;
            Data = data;
        }
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
        [JsonProperty("response_message")]
        public string ResponseMessage { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

    }

}
