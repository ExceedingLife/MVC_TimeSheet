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
                //if (ModelState.IsValid)
                //{
                    TimeSheetMaster masterModel = new TimeSheetMaster();
                    masterModel.FromDate = timeSheetModel.Proj1;
                    masterModel.ToDate = timeSheetModel.Proj1.AddDays(7);
                    /* NEED HRS 4 WEEK/END MONTH/NEW MONTH */

                    masterModel.TotalHours = timeSheetModel.ProjTotal1;
                    masterModel.UserId = User.Identity.GetUserId();
                    masterModel.DateCreated = DateTime.Now;
                    /* MONTH SUBMITTED */

                    masterModel.TimeSheetStatus = 1;
                    masterModel.Comment = timeSheetModel.ProjDesc1;
                    context.TimeSheetMaster.Add(masterModel);

                //var detailsModel = CreateTimeSheetDetails(masterModel, timeSheetModel);
                //context.TimeSheetDetails.Add(detailsModel);
                /*  possible way to insert multiple records with ID
                 *  using (var context = new MyDbContext) {
                 *  context.Clients.Add(client);
                 *  clientDetails.ClientId = client.Id;
                 *  context.ClientDetails.Add(clientDetails);
                 *  context.SaveChanges();
                 */
                context.SaveChanges();
                    TempData["SuccessMaster"] = "TimeSheetMaster Created Successfully";

                    //Create TimeSheetDetails
                    CreateTimeSheetDetails(masterModel, timeSheetModel);                  
                    
                    return RedirectToAction("TimeSheetList", "TimeSheet");
                //}
               // TempData["Error"] = "TimeSheet Create Was Unsuccessful";
               // return RedirectToAction("TimeSheetList", "TimeSheet");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                TempData["ErrorMaster"] = "TimeSheet Create Was Unsuccessful";
                return RedirectToAction("TimeSheetList", "TimeSheet");
            }
        }

        /* TimeSheetDetails */
        public void CreateTimeSheetDetails(TimeSheetMaster master,
               TimeSheetProjectsModel projectsModel)
        {
            // CURRENTLY SETUP FOR ONLY 1 TIME DETAILS RECORD AT A TIME.
            // WILL SETUP FOR MULTIPLE TIMEDETAIL RECORDS EVENTUALLY.
            try
            {
                var detailsModel = new TimeSheetDetails();
                /* DAYS OF WEEK ?? */

                detailsModel.Hours = master.TotalHours;
                /* Period   ?  detailsModel.Period = "" */

                detailsModel.ProjectId = (int)projectsModel.ProjectId1;
                detailsModel.UserId = master.UserId;
                detailsModel.DateCreated = master.DateCreated;
                detailsModel.TimeSheetMasterId = master.TimeSheetMasterId;
                detailsModel.Sunday = projectsModel.P1w1d1;
                detailsModel.Monday = projectsModel.P1w1d2;
                detailsModel.Tuesday = projectsModel.P1w1d3;
                detailsModel.Wednesday = projectsModel.P1w1d4;
                detailsModel.Thursday = projectsModel.P1w1d5;
                detailsModel.Friday = projectsModel.P1w1d6;
                detailsModel.Sunday = projectsModel.P1w1d7;

                context.TimeSheetDetails.Add(detailsModel);
                context.SaveChanges();

                TempData["SuccessDetails"] = "TimeSheetDetails Created Successfully";
                //return RedirectToAction("TimeSheetList", "TimeSheet");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                TempData["ErrorDetails"] = "TimeSheetDetails Created Unsuccessful";
                //return RedirectToAction("TimeSheetList", "TimeSheet");
            }        
        }

        /*  CREATE METHOD TO CHECK DATE WEEK FOR USER   */

        public ActionResult DisplayAllTimeSheetDetails(int masterid)
            {
            var masterModel = context.TimeSheetMaster.Where(w => 
                              w.TimeSheetMasterId.Equals(masterid)).FirstOrDefault();

            var detailM = context.TimeSheetDetails.Where(t =>
                          t.TimeSheetMasterId.Equals(masterModel.TimeSheetMasterId)).FirstOrDefault();//.ToList();
            //var detail1 = detailM.FirstOrDefault();

            var project = context.Projects.Where(p => p.ProjectId==detailM.ProjectId).FirstOrDefault();


            var details = (from master in context.TimeSheetMaster
                           join detail in context.TimeSheetDetails
                           on master.TimeSheetMasterId equals detail.TimeSheetMasterId
                           //from proj in context.Projects where proj.ProjectId = detail.ProjectId
                           select new TimeSheetDetailsModel()
                           {
                               Sunday = detail.Sunday,
                               Monday = detail.Monday,
                               Tuesday = detail.Tuesday,
                               Wednesday = detail.Wednesday,
                               Thursday = detail.Thursday,
                               Friday = detail.Friday,
                               Saturday = detail.Saturday,
                               Hours = detail.Hours,
                               Comment = master.Comment,
                               ProjectName = project.ProjectName
                           }).FirstOrDefault();

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTimeSheet(int masterId)
        {
            if (masterId < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var timeSheetMaster = context.TimeSheetMaster.Where(t => 
                                  t.TimeSheetMasterId.Equals(masterId)).FirstOrDefault();
            var timeSheetDetails = context.TimeSheetDetails.Where(t => 
                                   t.TimeSheetMasterId.Equals(
                                   timeSheetMaster.TimeSheetMasterId)).FirstOrDefault();
            if(timeSheetMaster == null)
            {
                TempData["ErrorMaster"] = "TimeSheet Master Delete was Unsuccessful";
                return RedirectToAction("TimeSheetList");
            }
            if(timeSheetDetails == null)
            {
                TempData["ErrorDetails"] = "TimeSheet Details Delete was Unsuccessful";
                return RedirectToAction("TimeSheetList");
            }
            try
            {
                context.TimeSheetDetails.Remove(timeSheetDetails);
                context.TimeSheetMaster.Remove(timeSheetMaster);
                context.SaveChangesAsync();
                TempData["SuccessMaster"] = "TimeSheet Master Deleted Successfully";
                TempData["SuccessDetails"] = "TimeSheet Details Deleted Successfully";
                return RedirectToAction("TimeSheetList");
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                TempData["Error"] = "TimeSheet Delete was Unsuccessful";
                return RedirectToAction("TimeSheetList");
            }
        }

    }
}