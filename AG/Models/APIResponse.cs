using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard.Entity.Common
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse()
        {
            Errors = new List<Errors>();
        }
        public bool IsSuccess { get; set; }
        public List<Errors> Errors { get; set; }
        public T Data { get; set; }
    }

    public class Errors
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}