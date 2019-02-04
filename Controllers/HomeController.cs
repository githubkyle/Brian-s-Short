using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using NGP.Models;


namespace NEX.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {


            return View();
        }
        [HttpPost]
        public ActionResult About(Msgbox obj)
        {

            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "claimprojectngp@gmail.com";
                WebMail.Password = "Lenovo1!";

                //Sender email address.  
                WebMail.From = "SenderGamilId@gmail.com";


                //Send email  
                WebMail.Send(to: "claimprojectngp@gmail.com", subject: obj.EmailSubject, body: obj.EMailBody, cc: obj.EmailCC, bcc: obj.EmailBCC, isBodyHtml: true);
                ViewBag.Status = "Email Sent Successfully.";
            }
            catch (Exception)
            {
                ViewBag.Status = "Problem while sending email, Please check details.";

            }
            return View();
        }

            public ActionResult Register()
        {


            return View();
        }

        public ActionResult SignIn()
        {


            return View();
        }

        public ActionResult Home()
        {


            return View();
        }

        public ActionResult Events()
        {


            return View();
        }

        public ActionResult Theatrical()
        {


            return View();
        }

        public ActionResult Catalogue()
        {


            return View();
        }

        public ActionResult MyAccount()
        {
            return View();
        }

        public ActionResult CorporateG()
        {
            return View();
        }

        public ActionResult CommercialG()
        {
            return View();
        }

        public ActionResult TheatricalG()
        {
            return View();
        }
       
    }
}