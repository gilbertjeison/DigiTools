using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiTools.Utils
{
    public class SomeHelpers
    {
        static string nombreE;

        static string noti_reg = "<div style=\"background: #fff; min-height: 50px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2); position: relative; margin-bottom: 30px; -webkit-border-radius: 2px; -moz-border-radius: 2px; -ms-border-radius: 2px; border-radius: 2px;\"> "
            + " <div  style=\"color:#2EFE2E; padding: 20px; position: relative;  border-bottom: 1px solid rgba(204, 204, 204, 0.35); background-color: #03A9F4 !important;\"> "
            + " <h2 style=\"color:white; margin: 0;font-size: 18px;font-weight: normal;\"> "
            + " " + nombreE + " <small style=\"display: block; font-size: 12px; margin-top: 5px; color: white; line-height: 15px;\">Registro satisfactorio al sistema [DIGITOOLS]...</small> "
            + " </h2> "
            + " </div> "
            + " <div style=\"font-size: 14px; color: #555; padding: 20px;\"> "
            + "Su registro está en estado de <strong>pendiente por aprobación</strong>, por este medio se le informa cuando haya sido aprobado por un administrador del sistema. "
            + " </div> <small style=\"font-size:8px;\">Desarrollado por (gilbertjeison@gmail.com)</small>";

        static string noti_apr = "<div style=\"background: #fff; min-height: 50px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2); position: relative; margin-bottom: 30px; -webkit-border-radius: 2px; -moz-border-radius: 2px; -ms-border-radius: 2px; border-radius: 2px;\"> "
            + " <div  style=\"color:#2196F3; padding: 20px; position: relative;  border-bottom: 1px solid rgba(204, 204, 204, 0.35); background-color: #03A9F4 !important;\"> "
            + " <h2 style=\"color:white; margin: 0;font-size: 18px;font-weight: normal;\"> "
            + " " + nombreE + " <small style=\"display: block; font-size: 12px; margin-top: 5px; color: white; line-height: 15px;\">Aprobación satisfactoria en el sistema [DIGITOOLS]...</small> "
            + " </h2> "
            + " </div> "
            + " <div style=\"font-size: 14px; color: #555; padding: 20px;\"> "
            + "Su usuario está en estado de <strong>Aprobado</strong>, ya puede iniciar en el sistema. "
            + " </div> <small style=\"font-size:8px;\">Desarrollado por (gilbertjeison@gmail.com)</small>";

        static string noti_elim = "<div style=\"background: #fff; min-height: 50px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2); position: relative; margin-bottom: 30px; -webkit-border-radius: 2px; -moz-border-radius: 2px; -ms-border-radius: 2px; border-radius: 2px;\"> "
            + " <div  style=\"color:#FF0040; padding: 20px; position: relative;  border-bottom: 1px solid rgba(204, 204, 204, 0.35); background-color: #03A9F4 !important;\"> "
            + " <h2 style=\"color:white; margin: 0;font-size: 18px;font-weight: normal;\"> "
            + " " + nombreE + " <small style=\"display: block; font-size: 12px; margin-top: 5px; color: white; line-height: 15px;\">Su aprobación ha sido rechazada en el sistema [DIGITOOLS]...</small> "
            + " </h2> "
            + " </div> "
            + " <div style=\"font-size: 14px; color: #555; padding: 20px;\"> "
            + "Su usuario está en estado de <strong>Rechazado</strong>, no puede iniciar en el sistema. "
            + " </div> <small style=\"font-size:8px;\">Desarrollado por (gilbertjeison@gmail.com)</small>";


        public static async Task SendGridAsync(int type, string mail, string nombre)
        {
            //type 1 = registro, 2 = aprobación, 3 = Eliminación

            var apiKey = ConfigurationManager.AppSettings["SendGridApi"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("gilbertjeison@gmail.com", "Jeison Desarrollador");
            var subject = "Notificación de registro al sistema";
            var to = new EmailAddress(mail, nombre);
            var plainTextContent = "Sistema de herramientas digitales Unilever";

            string htmlContent = "";

            nombreE = nombre;

            if (type == 1)
            {
                htmlContent = noti_reg;
            }
            if (type == 2)
            {
                htmlContent = noti_apr;
            }

            if (type == 3)
            {
                htmlContent = noti_elim;
            }

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            System.Diagnostics.Debug.WriteLine("Respuesta de correo electrónico " + response.StatusCode);
        }

        public static string VerifyEmail(string email)
        {         

            var apiKey = ConfigurationManager.AppSettings["Mailboxlayer"];
            var client = new RestClient("https://apilayer.net/api/check?access_key="+apiKey+"&email="+email+"");
            var request = new RestRequest(Method.GET);

            //request.AddHeader("authorization", "Bearer " + apiKey);
            //request.AddParameter("access_key ", apiKey, ParameterType.RequestBody);
            //request.AddParameter("email", email, ParameterType.RequestBody);

            var response = client.Execute(request);

            return response.Content.ToString();           
           
        }
    }
}