using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Utils
{
    public class DomainEmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                //VALIDAR SI TIENE DOMINIO UNILEVER
                MailAddress address = new MailAddress(value.ToString());
                string host = address.Host;

                var result = JsonConvert.DeserializeObject<MailBoxLayer>(SomeHelpers.VerifyEmail(value.ToString()));

                if (result.smtp_check != null)
                {
                    if (result.smtp_check.Value)
                    {
                        if (host.Trim().Equals("unilever.com"))
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("La dirección de correo electrónico debe ser corporativa [unilever.com]");
                        }
                    }
                    else
                    {
                        return new ValidationResult("La dirección de correo electrónico no existe...");
                    }

                }
                else
                {
                    return new ValidationResult("La dirección de correo electrónico no existe...");
                }
            }
            else
            {
                return new ValidationResult("La dirección de correo electrónico no fue ingresada...");
            }
        }
    }

    public class MailBoxLayer
    {
        public string email { get; set; }
        public string did_you_mean { get; set; }
        public string user { get; set; }
        public string domain { get; set; }
        public bool? format_valid { get; set; }
        public bool? mx_found { get; set; }
        public bool? smtp_check { get; set; }
        public bool? catch_all { get; set; }
        public bool? role { get; set; }
        public bool? disposable { get; set; }
        public bool? free { get; set; }
        public double? score { get; set; }
    }
}