using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarcodeApp.Models
{
    class API
    {
    }

    public class MemberRequest
    {
        public string first { get; set; }
        public string last { get; set; }
        public string email { get; set; }
    }
    public class Token
    {
        public string resourceUrl { get; set; }
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public LoginToken token { get; set; }
    }
    public class LoginToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }

    public class EmailAddress
    {
        public Response foundEmail { get; set; }
    }

    public class CreateNewMember
    {
        public Response memberCreated { get; set; }
    }

    public class MemberNote
    {
        public string MemberNumber { get; set; }
        public string Note { get; set; }
        public string NoteCreated { get; set; }
    }

    public class Response
    {
        public string Id { get; set; }
        public string Success { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
    }
}
