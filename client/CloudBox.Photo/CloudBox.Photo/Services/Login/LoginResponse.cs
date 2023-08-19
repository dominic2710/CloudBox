using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudBox.Photo.Services.Login
{
    public class LoginResponse:BaseResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }    
        public string Token { get; set; }
    }
}
