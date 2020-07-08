using Microsoft.AspNetCore.Http;
using Rsk.AspNetCore.Fido.Dtos;

namespace IdS.Quickstart.Account
{
    public class FidoIdSModel : NonceModel
    {
        public string ReturnUrl { get; set; }
        public Base64FidoAuthenticationChallenge Challenge { get; set; }
    }
}