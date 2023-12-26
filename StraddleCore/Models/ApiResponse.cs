using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models
{
    public class ApiResponse<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public int Status { get; set; }
        public string? Reason { get; set; }

        //T can be a class or List of class List<class> or null
        public T? Data { get; set; }
    }
}
