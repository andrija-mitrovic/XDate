using System.ComponentModel.DataAnnotations;

namespace XDate.BackEnd.Dtos
{
    public class UserForLoginDto
    {    
         
        public string Username {get;set;}
        public string Password {get;set;}
    }
}