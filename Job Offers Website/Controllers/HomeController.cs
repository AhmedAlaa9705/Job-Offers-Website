using Job_Offers_Website.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }
        public ActionResult Details(int JobId)
        {
            var job = db.Jobs.Find(JobId);
            if (job == null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = JobId;
            return View(job);
        }
        [Authorize]
        public ActionResult Apply()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Apply(string message)
        {
            var UserID = User.Identity.GetUserId();
            var JobID = (int)Session["JobId"];

            var check = db.ApplyForJobs.Where(a => a.JobId == JobID && a.UserId == UserID).ToList();

            if (check.Count < 1)
            {
                var job = new ApplyForJob();


                job.UserId = UserID;
                job.JobId = JobID;
                job.Message = message;
                job.ApplyDate = DateTime.Now;


                db.ApplyForJobs.Add(job);
                db.SaveChanges();
                ViewBag.Result = "تمت الاضافه بنجاح!";
            }
            else
            {
                ViewBag.Result = "المعذره لقد سبق وتقدمت الي هذه الوظيفه";
            }




            return View();
        }
        [Authorize]
        public ActionResult GetJobsByUserId()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = db.ApplyForJobs.Where(a => a.UserId == UserId);
            return View(Jobs.ToList());
        }
        [Authorize]
        public ActionResult DetailsOfJobs(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }

            return View(job);
        }
        [Authorize]
        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();
            var jobs = from app in db.ApplyForJobs
                       join Job in db.Jobs
                       on app.JobId equals Job.Id
                       where Job.User.Id == UserId
                       select app;
            return View(jobs.ToList());
        }
        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }
        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                job.ApplyDate = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobsByUserId");

            }
            return View(job);

        }
        public ActionResult Delete(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);

        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {

            var Myjob = db.ApplyForJobs.Find(job.Id);
            db.ApplyForJobs.Remove(Myjob);
            db.SaveChanges();
            return RedirectToAction("GetJobsByUserId");



        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            var mail = new MailMessage();
            var LoginInfo = new NetworkCredential("ahmedalaa9705@gmail.com", "1951997Ahmed");
            mail.From = new MailAddress(contact.Email);
            mail.To.Add(new MailAddress(contact.Email));
            mail.Subject = contact.Subject;
            mail.IsBodyHtml = true;
            string body = "اسم المرسل:" + contact.Name + "<br>" +
                           //"بريد المرسل:" + contact.Email + "<br>" +
                            "عنوان الرساله:" + contact.Subject + "<br>" +
                             "نص الرساله:<b>" + contact.message + "</b>";
            mail.Body = body;
            var SmtpClient = new SmtpClient("smtp.gmail.com", 587);
            SmtpClient.EnableSsl = true;
            SmtpClient.Credentials = LoginInfo;
            SmtpClient.Send(mail);
            return RedirectToAction("Index");

        }
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(string searchName)
        {
            var result = db.Jobs.Where(a => a.JobTitle.Contains(searchName)
             || a.JobContent.Contains(searchName)
             || a.Category.CategoryName.Contains(searchName)
             || a.Category.CategoryDescription.Contains(searchName)).ToList();
            return View(result);
        }
    }
}