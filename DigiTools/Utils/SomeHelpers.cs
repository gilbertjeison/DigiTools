using DigiTools.Dao;
using DigiTools.Database;
using DigiTools.Models;
using OfficeOpenXml;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Spire.Xls;
using Spire.Xls.Core;
using System.Drawing.Imaging;

namespace DigiTools.Utils
{
    public class SomeHelpers
    {
        static DaoEwo daoE = new DaoEwo();
        static DaoRepuestos daoR = new DaoRepuestos();
        static DaoPorque daoP = new DaoPorque();
        static DaoAcciones daoA = new DaoAcciones();
        static DaoTecnicos daoTec = new DaoTecnicos();
        static DaoUsuarios daoUSer = new DaoUsuarios();
        public static string ROL_SADMIN = "2b0bf2ba-c2a3-4f19-84d7-9288c0bc2895";
        public static string ROL_ADMIN = "d190e724-261a-49bb-8bbf-b24119c49a44";
        public static string ROL_MEC = "65b01f2a-0b46-4d0c-a227-304dc22e2f9d";

        static string ewo_images = "~/Content/images/ewo_images/";
        public static string IMG_PATH = "~/Content/images/v1/";

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

        static string noti_error = "<div style=\"background: #fff; min-height: 50px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2); position: relative; margin-bottom: 30px; -webkit-border-radius: 2px; -moz-border-radius: 2px; -ms-border-radius: 2px; border-radius: 2px;\"> "
            + " <div  style=\"color:#FF0040; padding: 20px; position: relative;  border-bottom: 1px solid rgba(204, 204, 204, 0.35); background-color: #03A9F4 !important;\"> "
            + " <h2 style=\"color:white; margin: 0;font-size: 18px;font-weight: normal;\"> "
            + " " + nombreE + " <small style=\"display: block; font-size: 12px; margin-top: 5px; color: white; line-height: 15px;\">Error del sistema en tiempo de ejecución</small> "
            + " </h2> "
            + " </div> "
            + " <div style=\"font-size: 14px; color: #555; padding: 20px;\"> "
            + "Revisar base de datos para conocer el problema. "
            + " </div> <small style=\"font-size:8px;\">Desarrollado por (gilbertjeison@gmail.com)</small>";




        public static async Task SendGridAsync(int type, string mail, string nombre)
        {
            //type 1 = registro, 2 = aprobación, 3 = Eliminación

            var apiKey = ConfigurationManager.AppSettings["SendGridApi"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("info@digitoolsunilever.com", "Información del sistema");
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

            if (type == 4)
            {
                htmlContent = noti_error;
            }
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            Debug.WriteLine("Respuesta de correo electrónico " + response.StatusCode);
        }

        public static async Task SendGridAsync(string mail, string nombre, string callBU)
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridApi"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("info@digitoolsunilever.com", "Información del sistema");
            var subject = "Recuperación de contraseña";
            var to = new EmailAddress(mail, nombre);
            var plainTextContent = "Sistema de herramientas digitales Unilever";

            string htmlContent = "<div style=\"border: solid 1px #03a9f4; background: #fff; min-height: 50px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2); position: relative; margin-bottom: 30px; -webkit-border-radius: 2px; -moz-border-radius: 2px; -ms-border-radius: 2px; border-radius: 2px;\"> "
            + " <div  style=\"color:#FF0040; padding: 20px; position: relative;  border-bottom: 1px solid rgba(204, 204, 204, 0.35); background-color: #03A9F4 !important;\"> "
            + " <h2 style=\"color:white; margin: 0;font-size: 18px;font-weight: normal;\"> "
            + " " + nombre + " <small style=\"display: block; font-size: 12px; margin-top: 5px; color: white; line-height: 15px;\">Procedimiento para recuperación de contraseña [DIGITOOLS]...</small> "
            + " </h2> "
            + " </div> "
            + " <div style=\"font-size: 14px; color: #555; padding: 20px;\"> "
            + "" + callBU + ""
            + " </div> <small style=\"font-size:8px;\">Desarrollado por (gilbertjeison@gmail.com)</small>";

                        
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            Debug.WriteLine("Respuesta de correo electrónico " + response.StatusCode);
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

        public static async Task<string> GenerateEwoFile(int id_ewo)
        {
            try
            {
                //OBTENER INFORMACION DE FORMATO 
                KpiViewModel km = await daoE.GetEwoDesc(id_ewo);
                var rep_Utils = await daoR.GetRepUtils(id_ewo);
                var pqs = await daoP.GetPorques(id_ewo);
                var lista_acc = await daoA.GetActionsList(id_ewo);

                //QUEBRAR PROCESO DE EXCEL
                //foreach (Process clsProcess in Process.GetProcesses())
                //{
                //    if (clsProcess.ProcessName.Equals("EXCEL"))
                //    {
                //        clsProcess.Kill();
                //        break;
                //    }
                //}


                string filename = HttpContext.Current.Server.MapPath("~/Content/formats/FEU.XLSX");
                string nfilename = HttpContext.Current.Server.MapPath("~/Content/formats/FORMATO_EWO.XLSX");

                //ESCRIBIR EN ARCHIVO ECXEL
                FileInfo file = new FileInfo(filename);
                FileInfo fileN = new FileInfo(nfilename);
                
                if (fileN.Exists)
                {
                    if (!IsFileLocked(fileN))
                    {
                        fileN.Delete();
                    }
                    else
                    {
                        return "-2";
                    }
                }
               

                file.CopyTo(nfilename);

                using (ExcelPackage excelPackage = new ExcelPackage(fileN))
                {
                    var ws = excelPackage.Workbook.Worksheets["EWO"];

                    ws.Cells["A3"].Value = km.Consecutivo;
                    ws.Cells["B3"].Value = km.AreaLinea;
                    ws.Cells["C3"].Value = km.Equipo;
                    ws.Cells["E3"].Value = km.Fecha.ToShortDateString();
                    ws.Cells["F3"].Value = km.NumAviso;
                    ws.Cells["G3"].Value = km.DiligenciadoPor;
                    ws.Cells["H3"].Value = km.TipoAveria;
                    ws.Cells["J3"].Value = km.Turno;

                    ws.Cells["A6"].Value = km.HrNotAveD;
                    ws.Cells["B6"].Value = km.HrIniRepD;
                    ws.Cells["C6"].Value = km.TEspIniTec;
                    ws.Cells["D6"].Value = km.TDiagn;
                    ws.Cells["E6"].Value = km.TEspRep;
                    ws.Cells["F6"].Value = km.TRepCamP;
                    ws.Cells["G6"].Value = km.PruTieArr;
                    ws.Cells["H6"].Value = km.HrFinRepEntD;
                    ws.Cells["I6"].Value = km.TiempoTotal;

                    using (FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath(ewo_images + km.PathImage1), FileMode.Open))
                    {
                        //IMAGEN 1
                        var eImg = ws.Drawings.AddPicture("image1", Image.FromStream(fileStream, true, true));
                        eImg.From.Column = 0;
                        eImg.From.Row = 7;
                        eImg.SetSize(170, 220);

                        // 2x2 px space for better alignment
                        eImg.From.ColumnOff = Pixel2MTU(2);
                        eImg.From.RowOff = Pixel2MTU(60);
                    }

                    //IMAGEN 2
                    if (km.PathImage2 != null && km.PathImage2.Length > 0)
                    {
                        using (FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath(ewo_images + km.PathImage2), FileMode.Open))
                        {
                            var eImg2 = ws.Drawings.AddPicture("image2", Image.FromStream(fileStream, true, true));
                            eImg2.From.Column = 2;
                            eImg2.From.Row = 7;
                            eImg2.SetSize(220, 220);

                            // 2x2 px space for better alignment
                            eImg2.From.ColumnOff = Pixel2MTU(20);
                            eImg2.From.RowOff = Pixel2MTU(60);
                        }
                    }


                    ws.Cells["A16"].Value = km.DescImg1;
                    ws.Cells["C16"].Value = km.DescImg2;
                    ws.Cells["F8"].Value = km.DescripcionAveria;

                    //CAMBIO DE REPUESTO O AJUSTE
                    string a = "", c = "";
                    if (km.Accion == 0) { a = "X"; } else { c = "X"; }
                    ws.Cells["F12"].Value = "( " + c + "  ) Cambio de Componente                    (  " + a + "  ) Ajuste";

                    if (rep_Utils.Count > 0)
                    {
                        for (int row = 15; row < rep_Utils.Count + 15; row++)
                        {
                            for (int cols = 6; cols < 10; cols++)
                            {
                                if (cols == 6)
                                {
                                    ws.SetValue(row, cols, rep_Utils[row - 15].codigo_sap);
                                }
                                if (cols == 7)
                                {
                                    ws.SetValue(row, cols, rep_Utils[row - 15].descripcion);
                                }
                                if (cols == 8)
                                {
                                    ws.SetValue(row, cols + 1, rep_Utils[row - 15].cantidad_ewo);
                                }
                                if (cols == 9)
                                {
                                    ws.SetValue(row, cols + 1, "$" + rep_Utils[row - 15].costo);
                                }
                            }
                        }
                    }

                    //5G CHECK
                    if (km.GembaOkB)
                    {
                        ws.Cells["J22"].Value = "X";
                    }
                    else
                    {
                        ws.Cells["I22"].Value = "X";
                    }

                    if (km.GembutsuOkB)
                    {
                        ws.Cells["J23"].Value = "X";
                    }
                    else
                    {
                        ws.Cells["I23"].Value = "X";
                    }

                    if (km.GenjitsuOkB)
                    {
                        ws.Cells["J24"].Value = "X";
                    }
                    else
                    {
                        ws.Cells["I24"].Value = "X";
                    }

                    if (km.GenriOkB)
                    {
                        ws.Cells["J25"].Value = "X";
                    }
                    else
                    {
                        ws.Cells["I25"].Value = "X";
                    }

                    if (km.GensokuOkB)
                    {
                        ws.Cells["J26"].Value = "X";
                    }
                    else
                    {
                        ws.Cells["I26"].Value = "X";
                    }

                    //5G TEXT
                    ws.Cells["C22"].Value = km.GembaDesc;
                    ws.Cells["C23"].Value = km.GembutsuDesc;
                    ws.Cells["C24"].Value = km.GenjitsuDesc;
                    ws.Cells["C25"].Value = km.GenriDesc;
                    ws.Cells["C26"].Value = km.GensokuDesc;

                    //5W + 1H
                    ws.Cells["C28"].Value = km.QueDesc;
                    ws.Cells["C29"].Value = km.DondeDesc;
                    ws.Cells["C30"].Value = km.CuandoDesc;
                    ws.Cells["C31"].Value = km.QuienDesc;
                    ws.Cells["C32"].Value = km.CualDesc;
                    ws.Cells["C33"].Value = km.ComoDesc;

                    ws.Cells["A35"].Value = km.FenomenoDesc;

                    //CUADRO PORQUE PORQUE
                    if (pqs.Count > 0)
                    {
                        for (int row = 38; row < pqs.Count + 38; row++)
                        {
                            for (int cols = 1; cols <= 10; cols += 2)
                            {
                                if (cols == 1)
                                {
                                    ws.SetValue(row, cols, pqs[row - 38].porque_1);
                                }
                                if (cols == 3)
                                {
                                    ws.SetValue(row, cols, pqs[row - 38].porque_2);
                                }
                                if (cols == 5)
                                {
                                    ws.SetValue(row, cols, pqs[row - 38].porque_3);
                                }
                                if (cols == 7)
                                {
                                    ws.SetValue(row, cols, pqs[row - 38].porque_4);
                                }
                                if (cols == 9)
                                {
                                    ws.SetValue(row, cols, pqs[row - 38].porque_5);
                                }
                            }
                        }
                    }

                    using (FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath(ewo_images + km.PathImagePQ1), FileMode.Open))
                    {
                        //IMAGEN 3
                        var eImg3 = ws.Drawings.AddPicture("image3", Image.FromStream(fileStream, true, true));
                        eImg3.From.Column = 0;
                        eImg3.From.Row = 42;
                        eImg3.SetSize(320, 235);

                        // 2x2 px space for better alignment
                        eImg3.From.ColumnOff = Pixel2MTU(60);
                        eImg3.From.RowOff = Pixel2MTU(30);
                    }


                    //IMAGEN 4
                    if (km.PathImagePQ2 != null && km.PathImagePQ2.Length > 0)
                    {
                        using (FileStream fileStream = new FileStream(HttpContext.Current.Server.MapPath(ewo_images + km.PathImagePQ2), FileMode.Open))
                        {
                            var eImg4 = ws.Drawings.AddPicture("image4", Image.FromStream(fileStream, true, true));
                            eImg4.From.Column = 5;
                            eImg4.From.Row = 42;
                            eImg4.SetSize(320, 235);

                            // 2x2 px space for better alignment
                            eImg4.From.ColumnOff = Pixel2MTU(60);
                            eImg4.From.RowOff = Pixel2MTU(30);
                        }

                    }


                    //DESC IMAGE XQ
                    ws.Cells["A55"].Value = km.DescImgPQ1;
                    ws.Cells["F55"].Value = km.DescImgPQ2;

                    ws.Cells["B64"].Value = km.FchUltimoMttoD.ToShortDateString();
                    ws.Cells["H64"].Value = km.FchProxMttoD.ToShortDateString();


                    //CUADRO PORQUE PORQUE
                    if (lista_acc.Count > 0)
                    {
                        for (int row = 88; row < lista_acc.Count + 88; row++)
                        {
                            for (int cols = 1; cols <= 4; cols++)
                            {
                                if (cols == 1)
                                {
                                    ws.SetValue(row, cols + 1, lista_acc[row - 88].accion);
                                }
                                if (cols == 2)
                                {
                                    ws.SetValue(row, cols + 5, lista_acc[row - 88].tipo_accion);
                                }
                                if (cols == 3)
                                {
                                    var names = daoUSer.GetUserAsync(lista_acc[row - 88].responsable).Result;
                                    ws.SetValue(row, cols + 5, names.Nombres + " " + names.Apellidos);
                                }
                                if (cols == 4)
                                {
                                    ws.SetValue(row, cols + 6, lista_acc[row - 88].fecha.Value.ToShortDateString());
                                }
                            }
                        }
                    }

                    //RESPONSABLES
                    ws.Cells["A103"].Value = km.IdTecMattInv;
                    ws.Cells["F103"].Value = km.IdOpersInv;
                    ws.Cells["A105"].Value = km.IdAnaElab;
                    ws.Cells["C105"].Value = km.FchAnaElab;
                    ws.Cells["D105"].Value = km.IdContMedDef;
                    ws.Cells["G105"].Value = km.FchDefConMed;
                    ws.Cells["J105"].Value = km.FchEjeVal;
                    ws.Cells["H105"].Value = km.IdEjeValPor;

                    var shape = ws.Drawings.AddShape("ff", eShapeStyle.Ellipse);
                    shape.Fill.Transparancy = 70;
                    shape.SetSize(110, 290);

                    ExcelRange rango1 = ws.Cells[69, 1];
                    ExcelRange rango2 = ws.Cells[69, 2];
                    ExcelRange rango3 = ws.Cells[69, 4];
                    ExcelRange rango4 = ws.Cells[69, 5];
                    ExcelRange rango5 = ws.Cells[69, 7];
                    ExcelRange rango6 = ws.Cells[69, 9];

                    switch (km.CicloRaiz)
                    {
                        case 1:
                            shape.SetPosition(72, 0, 0, 0);                                                   
                            break;
                        case 2:
                            shape.SetPosition(72, 0, 1, 40);
                            break;
                        case 3:
                            shape.SetPosition(72, 0, 3, 0);
                            break;
                        case 4:
                            shape.SetPosition(72, 0, 4, 40);
                            break;
                        case 5:
                            shape.SetPosition(72, 0, 6, 22);
                            break;
                        case 6:
                            shape.SetPosition(72, 0, 8, 0);
                            break;
                    }


                //float pri = Convert.ToSingle(rango1.);
                //float seg = Convert.ToSingle(rango2.Left) + 28f;
                //float ter = Convert.ToSingle(rango3.Left);
                //float cua = Convert.ToSingle(rango4.Left) + 37f;
                //float qui = Convert.ToSingle(rango5.Left) + 10f;
                //float sex = Convert.ToSingle(rango6.Left);

                excelPackage.SaveAs(fileN);
                }

                //SPIRE FREE
                Workbook wb = new Workbook();
                wb.LoadFromFile(nfilename);
                Worksheet ws2 = wb.Worksheets[0];
                ICheckBox chk = ws2.CheckBoxes[km.CausaRaiz-1];
                chk.CheckState = CheckState.Checked;

                wb.Save();
                wb.Dispose();
                //NETOFFICE
                //Excel.Application app = new Excel.Application();
                //app.DisplayAlerts = false;
                //app.Visible = false;

                //Excel.Workbook wb = app.Workbooks.Open(nfilename);
                //Excel.Worksheet ws2 = (Excel.Worksheet)wb.Worksheets[1];

                //Excel.OptionButton opt = (Excel.OptionButton)ws2.OptionButtons(km.CausaRaiz);
                //opt.Value = true;

                //Excel.Range rango1 = ws2.Cells[69, 1];
                //Excel.Range rango2 = ws2.Cells[69, 2];
                //Excel.Range rango3 = ws2.Cells[69, 4];
                //Excel.Range rango4 = ws2.Cells[69, 5];
                //Excel.Range rango5 = ws2.Cells[69, 7];
                //Excel.Range rango6 = ws2.Cells[69, 9];

                //float pri = Convert.ToSingle(rango1.Left);
                //float seg = Convert.ToSingle(rango2.Left) + 28f;
                //float ter = Convert.ToSingle(rango3.Left);
                //float cua = Convert.ToSingle(rango4.Left) + 37f;
                //float qui = Convert.ToSingle(rango5.Left) + 10f;
                //float sex = Convert.ToSingle(rango6.Left);

                //switch (km.CicloRaiz)
                //{
                //    case 1:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            pri, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //    case 2:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            seg, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //    case 3:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            ter, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //    case 4:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            cua, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //    case 5:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            qui, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //    case 6:
                //        ws2.Shapes.AddShape(MsoAutoShapeType.msoShapeOval,
                //            sex, 1735, 80, 200).Fill.Transparency = 0.60f;
                //        break;
                //}

                //wb.Save();
                //wb.Close();

                return nfilename;
            }
            catch (Exception ex)
            {
                string err = "Error al generar formato ewo (SOMEHELPERS): " + ex.ToString();
                Trace.WriteLine(err);
                
                //REPORTAR ERROR EN LA BASE DE DATOS
                await DaoExcepcion.AddExceptionAsync(
                    new excepciones()
                    {
                        codigo_error = -1,
                        codigo_usuario = HttpContext.Current.User.Identity.Name ?? "No definido",
                        descripcion = err,
                        fecha = SomeHelpers.GetCurrentTime()
                    });
                return "-1";

            }            
        }

        protected static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }

        private static void SaveImageEwoServer(HttpPostedFileBase file)
        {
            string nameAndLocation = ewo_images + file.FileName;
            file.SaveAs(HttpContext.Current.Server.MapPath(nameAndLocation));
        }

        public static string GetCausaRaizDesc(int index)
        {
            string res = "";
            switch (index)
            {
                case 1:
                    res = "Factores externos [FI]";
                    break;
                case 2:
                    res = "Falta de Conocimiento [PD]";
                    break;
                case 3:
                    res = "Falta de Diseño [FI]";
                    break;
                case 4:
                    res = "Falta de Mantenimiento [PM]";
                    break;
                case 5:
                    res = "Condiciones Sub estandar de operación [PD]";
                    break;
                case 6:
                    res = "Falta de Condiciones básicas [AA]";
                    break;
            }

            return res;
        }

        public static DateTime GetCurrentTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "SA Pacific Standard Time");
            return _localTime;
        }

        public static Image resizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = (newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new System.Drawing.Rectangle(destX, destY, destWidth, destHeight),
                new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }

    }
}