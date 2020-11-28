using System;
namespace Consumer.Api.Models
{

    public class ErrorResponse
    {
        public string Message { get; set; }
        public ErrorResponse(Exception ex)
        {
            Message = ex.Message;
        }
    }
}