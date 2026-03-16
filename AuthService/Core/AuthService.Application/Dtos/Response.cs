using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Dtos
{
    public class Response<T> where T :class
    {
        public T Data { get; set; }
        public bool Success { get; set; }

        public string Message { get; set; }
        public string ErrorCodes { get; set; }
    }
}
/*DATA türünün o classa bağlı olacak şekilde ayarlandı*/