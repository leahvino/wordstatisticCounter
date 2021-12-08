using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Dto
{
    public class ResponseBase<T>
    {      
        public T Response { get; set; }
    }
}
