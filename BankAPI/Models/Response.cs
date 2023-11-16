using System.Net;

namespace BankAPI.Models
{
    public class Response
    {
        public string RequestId => $"{Guid.NewGuid().ToString()}";
        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object Data { get; set; }
    }
}
