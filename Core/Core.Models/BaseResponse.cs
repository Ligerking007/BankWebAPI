using System;
namespace Core.Models
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }

}
