using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_TimeSh.Models;

namespace MVC_TimeSh.Controllers
{
    public class TimeSheetController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;

        public TimeSheetController()
        {
            context = new ApplicationDbContext();
        }
        public TimeSheetController(ApplicationUserManager userManager,
               ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            context = new ApplicationDbContext();
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: TimeSheetList
        public ActionResult TimeSheetList()
        {
            var timeSheets = (from times in context.TimeSheetMaster
                              select new TimeSheetMasterModel()
                              {
                                  TimeSheetMasterId = times.TimeSheetMasterId,
                                  FromDate = ((DateTime)times.FromDate),
                                  ToDate = ((DateTime)times.ToDate),
                                  TimeSheetStatus = times.TimeSheetStatus,
                                  TotalHours = times.TotalHours,
                                  Comment = times.Comment,
                                  DateCreated = ((DateTime)times.DateCreated),
                                  UserId = times.UserId.ToString()
                              }).ToList();
            if (timeSheets == null)
                return HttpNotFound();
            return View(timeSheets);
        }

        // GET: AddTimeSheet
        public ActionResult AddTimeSheet()
        {
            ViewBag.ProjectsSelect = new SelectList(
                context.Projects.ToList(), "ProjectId", "ProjectName");

            var uid = User.Identity.GetUserId();
            ViewBag.CurrentId = uid;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTimeSheet(TimeSheetProjectsModel timeSheetModel)
        {
            try
            {
                if(timeSheetModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    TimeSheetMaster masterModel = new TimeSheetMaster();
                    masterModel.FromDate = timeSheetModel.Proj1;
                    /* NEED HRS 4 WEEK/END MONTH/NEW MONTH */

                    masterModel.TotalHours = timeSheetModel.ProjTotal1;
                    masterModel.UserId = User.Identity.GetUserId();
                    masterModel.DateCreated = DateTime.Now;
                    /* MONTH SUBMITTED */

                    masterModel.TimeSheetStatus = 1;
                    masterModel.Comment = timeSheetModel.ProjDesc1;
                    context.TimeSheetMaster.Add(masterModel);
                    var detailsModel = CreateTimeSheetDetails(masterModel, timeSheetModel);
                    context.TimeSheetDetails.Add(detailsModel);
                    context.SaveChanges();

                    TempData["SuccessMaster"] = "TimeSheetMaster Created Successfully";
                    TempData["SuccessDetails"] = "TimeSheetDetails Created Successfully";
                    return RedirectToAction("TimeSheetList", "TimeSheet");
                }
                TempData["Error"] = "TimeSheet Create Was Unsuccessful";
                return RedirectToAction("TimeSheetList", "TimeSheet");
            }
            catch (Exception)
            {
                TempData["Error"] = "TimeSheet Create Was Unsuccessful";
                return RedirectToAction("TimeSheetList", "TimeSheet");
            }
        }


        public TimeSheetDetails CreateTimeSheetDetails(TimeSheetMaster master, 
               TimeSheetProjectsModel projectsModel)
        {
            //projectsModel.proji
            var detailsModel = new TimeSheetDetails();
            /* DAYS OF WEEK ?? */
            detailsModel.Hours = master.TotalHours;
            /* Period   ?  detailsModel.Period = "" */
            
            detailsModel.ProjectId = (int)projectsModel.ProjectId1;

            detailsModel.UserId = master.UserId;
            detailsModel.DateCreated = master.DateCreated;
            detailsModel.TimeSheetMasterId = master.TimeSheetMasterId;
            //var prodName = context.Projects.Where(p => 
            //    p.ProjectId.Equals(projectsModel.ProjectId1)).FirstOrDefault();
            //detailsModel.ProjectName = prodName.ProjectName;

            return detailsModel;
        }
    }
}