using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido.Models;

namespace IdS.Quickstart.Account
{
    public class RegistrationViewModel : NonceModel
    {
        public Base64FidoRegistrationChallenge FidoRegistrationChallenge { get; set; }
    }
}