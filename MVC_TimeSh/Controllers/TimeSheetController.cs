using System;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;

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

        //GET: /TimeSheet/UserTimeSheets/{id}
        public async Task<ActionResult> UserTimeSheets(string id)
        {
            var userTimeSheets = (from times in context.TimeSheetMaster
                                  where times.UserId == id
                                  select new TimeSheetMasterModel()
                                  {
                                      TimeSheetMasterId = times.TimeSheetMasterId,
                                      FromDate = ((DateTime)times.FromDate),
                                      ToDate = ((DateTime)times.ToDate),
                                      TimeSheetStatus = times.TimeSheetStatus,
                                      TotalHours = times.TotalHours,
                                      Comment = times.Comment,
                                      DateCreated = ((DateTime)times.DateCreated),
                                      UserId = times.UserId.ToString(),
                                      IdShortened = times.UserId.Substring(0,10)
                                  });
            if (userTimeSheets == null)
                return HttpNotFound();

            return View(await userTimeSheets.ToListAsync());
        }
        //GET: /TimeSheet/UserTimeDetails/{uid}{tid}
        public async Task<ActionResult> UserTimeDetails(string uid, int tid)
        {
            var tmaster = await context.TimeSheetMaster.Where(t =>
                          t.TimeSheetMasterId == tid).FirstOrDefaultAsync();
            var tdetail = await context.TimeSheetDetails.Where(t => 
                          t.TimeSheetMasterId == tmaster.TimeSheetMasterId).FirstOrDefaultAsync();
            var project = await context.Projects.Where(p => 
                          p.ProjectId == tdetail.ProjectId).FirstOrDefaultAsync();

            var userTimeDetails = (from t in context.TimeSheetMaster
                                   join d in context.TimeSheetDetails
                                   on t.TimeSheetMasterId equals d.TimeSheetMasterId
                                   where t.UserId == uid
                                   select new TimeSheetDetailsModel()
                                   {
                                       TimeSheetMasterId = t.TimeSheetMasterId,
                                       Sunday = d.Sunday,
                                       Monday = d.Monday,
                                       Tuesday = d.Tuesday,
                                       Wednesday = d.Wednesday,
                                       Thursday = d.Thursday,
                                       Friday = d.Friday,
                                       Saturday = d.Saturday,
                                       Hours = t.TotalHours,
                                       ProjectName = project.ProjectName,
                                       ProjectId = project.ProjectId,
                                       UserId = t.UserId,
                                       Comment = t.Comment
                                   });
            if (userTimeDetails == null)
                return HttpNotFound();

            return View(await userTimeDetails.FirstOrDefaultAsync());
        }
        //GET: /TimeSheet/UserEditTimeSheet/{uid}{mid}{did}
        public async Task<ActionResult> UserEditTimeSheet(string uid, int did)
        {
            //var tmaster = await context.TimeSheetMaster.Where(t =>
            //              t.TimeSheetMasterId == mid).FirstOrDefaultAsync();
            //var tdetail = await context.TimeSheetDetails.Where(t =>
            //              t.TimeSheetMasterId == tmaster.TimeSheetMasterId).FirstOrDefaultAsync();
            //var project = await context.Projects.Where(p =>
            //              p.ProjectId == tdetail.ProjectId).FirstOrDefaultAsync();

            var td = await context.TimeSheetDetails.Where(t => 
                           t.TimeSheetId == did).FirstOrDefaultAsync();
            var proj = await context.Projects.Where(p => 
                             p.ProjectId == td.ProjectId).FirstOrDefaultAsync();

            var timeSheet = (from t in context.TimeSheetMaster
                             join d in context.TimeSheetDetails
                             on t.TimeSheetMasterId equals d.TimeSheetMasterId
                             where d.TimeSheetId == did
                             select new TimeSheetDetailsModel()
                             {
                                 TimeSheetMasterId = t.TimeSheetMasterId,
                                 Sunday = d.Sunday,
                                 Monday = d.Monday,
                                 Tuesday = d.Tuesday,
                                 Wednesday = d.Wednesday,
                                 Thursday = d.Thursday,
                                 Friday = d.Friday,
                                 Saturday = d.Saturday,
                                 Hours = t.TotalHours,
                                 ProjectName = proj.ProjectName,
                                 ProjectId = proj.ProjectId,
                                 UserId = t.UserId,
                                 Comment = t.Comment
                             });
            if (timeSheet == null)
                return HttpNotFound();

            return View(await timeSheet.FirstOrDefaultAsync());
        }

        // GET: /TimeSheet/AddTimeSheet
        public ActionResult AddTimeSheet()
        {
            ViewBag.ProjectsSelect = new SelectList(
                context.Projects.ToList(), "ProjectId", "ProjectName");

            var uid = User.Identity.GetUserId();
            ViewBag.CurrentId = uid;

            return View();
        }
        // POST: /TimeSheet/AddTimeSheet{Model}
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
                //if (ModelState.IsValid) {
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
                 *  context.SaveChanges();      */                 
                context.SaveChanges();
                    TempData["SuccessMaster"] = "TimeSheetMaster Created Successfully";
                    //Create TimeSheetDetails
                    CreateTimeSheetDetails(masterModel, timeSheetModel);

                /*  Need to add seperate Dates to each Project */
                TimeSheetMaster masterModel2 = new TimeSheetMaster();
                masterModel2.FromDate = timeSheetModel.Proj1;
                masterModel2.ToDate = timeSheetModel.Proj1.AddDays(7);
                masterModel2.TotalHours = timeSheetModel.ProjTotal2;
                masterModel2.UserId = User.Identity.GetUserId();
                masterModel2.DateCreated = DateTime.Now;
                masterModel2.TimeSheetStatus = 1;
                masterModel2.Comment = timeSheetModel.ProjDesc2;
                if(masterModel2.TotalHours != null)
                {
                    context.TimeSheetMaster.Add(masterModel2);
                    context.SaveChanges();
                    TempData["SuccessMaster"] = "2 TimeSheetMasters have been created Successfully";
                    Create2ndTimeSheetDetails(masterModel2, timeSheetModel);
                }
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

        /* Created off TimeSheetMaster ~ TimeSheetDetails */
        public void CreateTimeSheetDetails(TimeSheetMaster master,
               TimeSheetProjectsModel projectsModel)
        {
            // CURRENTLY SETUP FOR ONLY 1 TIME DETAILS RECORD AT A TIME.
            // WILL SETUP FOR MULTIPLE TIME DETAIL RECORDS EVENTUALLY.
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
                detailsModel.Saturday = projectsModel.P1w1d7;

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
        /* Created off TimeSheetMaster ~ TimeSheetDetails */
        public void Create2ndTimeSheetDetails(TimeSheetMaster master,
               TimeSheetProjectsModel projectsModel)
        {   // METHOD FOR CREATING A 2ND TIMESHEETDETAIL RECORD
            try
            {
                var detailsModel2 = new TimeSheetDetails();
                detailsModel2.Hours = master.TotalHours;
                /* PERIOD */
                detailsModel2.ProjectId = (int)projectsModel.ProjectId2;
                detailsModel2.UserId = master.UserId;
                detailsModel2.DateCreated = master.DateCreated;
                detailsModel2.TimeSheetMasterId = master.TimeSheetMasterId;
                detailsModel2.Sunday = Convert.ToInt32(projectsModel.P1w2d1);
                detailsModel2.Monday = Convert.ToInt32(projectsModel.P1w2d2);
                detailsModel2.Tuesday = Convert.ToInt32(projectsModel.P1w2d3);
                detailsModel2.Wednesday = Convert.ToInt32(projectsModel.P1w2d4);
                detailsModel2.Thursday = Convert.ToInt32(projectsModel.P1w2d5);
                detailsModel2.Friday = Convert.ToInt32(projectsModel.P1w2d6);
                detailsModel2.Saturday = Convert.ToInt32(projectsModel.P1w2d7);
                context.TimeSheetDetails.Add(detailsModel2);
                context.SaveChanges();
                TempData["SuccessDetails"] = "2 TimeSheetDetails Created Successfully";
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                TempData["ErrorDetails"] = "The 2nd TimeSheetDetail Created Unsuccessfully";
            }
        }

        /*  CREATE METHOD TO CHECK DATE WEEK FOR USER   */

        // GET: TimeSheet/DisplayAllTimeSheetDetails{masterid}
        public ActionResult DisplayAllTimeSheetDetails(int masterid)
            {
            var masterModel = context.TimeSheetMaster.Where(w => 
                              w.TimeSheetMasterId.Equals(masterid)).FirstOrDefault();

            var detailM = context.TimeSheetDetails.Where(t =>
                          t.TimeSheetMasterId.Equals(masterModel.TimeSheetMasterId))
                          .FirstOrDefault();

            var project = context.Projects.Where(p => p.ProjectId==detailM.ProjectId)
                          .FirstOrDefault();


            var details = (from master in context.TimeSheetMaster
                           join detail in context.TimeSheetDetails
                           on master.TimeSheetMasterId equals detail.TimeSheetMasterId
                           where master.TimeSheetMasterId == masterid
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
        // POST: TimeSheet/DeleteTimeSheet{masterId}
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
                context.SaveChanges();
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
        [HttpPost, ActionName("DeleteTimeSh")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTimeData(int mid, int did)
        {
            if(mid < 0 || did < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var masterData = context.TimeSheetMaster.Where(t =>
                t.TimeSheetMasterId.Equals(mid)).FirstOrDefault();
            var detailData = context.TimeSheetDetails.Where(t =>
                t.TimeSheetMasterId.Equals(mid) && t.TimeSheetId.Equals(did))
                .FirstOrDefault();
            if(masterData == null)
            {
                TempData["ErrorMaster"] = "TimeSheet Master Delete was Unsuccessful";
                return RedirectToAction("UserTimeDetails");
            }
            if(detailData == null)
            {
                TempData["ErrorDetails"] = "TimeSheet Details Delete was Unsuccessful";
                return RedirectToAction("TimeSheetList");
            }
            try
            {
                context.TimeSheetDetails.Remove(detailData);
                context.SaveChanges();
                TempData["SuccessDetails"] = "TimeSheet Details Deleted Successfully";
                var chk = context.TimeSheetDetails.Where(x => 
                          x.TimeSheetMasterId.Equals(mid)).FirstOrDefault();
                if(chk == null)
                {
                    context.TimeSheetMaster.Remove(masterData);
                    context.SaveChanges();
                    TempData["SuccessMaster"] = "TimeSheet Master Deleted Successfully";
                    return RedirectToAction("UserTimeDetails");
                }
                else
                {
                    return RedirectToAction("UserTimeDetails");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                TempData["Error"] = "TimeSheet Delete was Unsuccessful";
                return RedirectToAction("UserTimeDetails");
            }
        }


        /* To-do: Join on User + TimeSheet but currently not enouph timesheets */
        // GET: TimeSheet/ExportTimeSheetToExcel
        public ActionResult ExportTimeSheetToExcel()
        {
            var users = (from user in context.Users
                         select new SelectListItem()
                         {
                             Value = user.Id.ToString(),
                             Text = user.UserName
                         }).ToList();
            ViewBag.TimeUsers = users;

            var allTimes = (from time in context.TimeSheetMaster
                                //join detail in context.TimeSheetDetails 
                                //on time.TimeSheetMasterId 
                                //equals detail.TimeSheetMasterId 
                            orderby time.FromDate
                            select new SelectListItem()
                            {
                                Value = time.TimeSheetMasterId.ToString(),
                                Text = time.FromDate + " - " + time.ToDate
                            }).ToList();
            ViewBag.TimeSheets = allTimes;

            return View();
        }

        /* WILL SETUP FOR SPECIFIC USERS + TIMES WHEN MORE TIMES ARE ADDED */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportTimeSheetToExcel(string id, int timeId)
        {
            try
            {
                //var user = await UserManager.FindByIdAsync(id);//
                var timeMaster = await context.TimeSheetMaster.Where(t =>
                                 t.TimeSheetMasterId == timeId).FirstOrDefaultAsync();
                var timeDetail = await context.TimeSheetDetails.Where(d =>
                                 d.TimeSheetMasterId.Equals(timeMaster.TimeSheetMasterId))
                                 .FirstOrDefaultAsync();
                var project = await context.Projects.Where(p => p.ProjectId == 
                              timeDetail.ProjectId).FirstOrDefaultAsync();
                ExportTimeSheetModel model = new ExportTimeSheetModel()
                {
                    UserId = "1505USERID",//user.Id,
                    IdShortened = "876543",//user.Id.Substring(0, 10),
                    Username = "USERNAME",//user.UserName,
                    TimeSheetMasterId = timeMaster.TimeSheetMasterId,
                    DateCreated = timeMaster.DateCreated.ToString(),
                    FromDate = timeMaster.FromDate.ToString(),
                    ToDate = timeMaster.ToDate.ToString(),
                    TotalHours = timeMaster.TotalHours,
                    Comment = timeMaster.Comment,
                    Sunday = timeDetail.Sunday,
                    Monday = timeDetail.Monday,
                    Tuesday = timeDetail.Tuesday,
                    Wednesday = timeDetail.Wednesday,
                    Thursday = timeDetail.Thursday,
                    Friday = timeDetail.Friday,
                    Saturday = timeDetail.Saturday,
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName
                };

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlBook = xlApp.Workbooks.Add(System.Reflection.Missing.Value);
                Excel.Worksheet xlSheet = xlBook.ActiveSheet;
                Excel.Range xlRange;
                xlSheet.Name = "Time Records";

                xlSheet.Cells[1, 1] = "User ID";
                xlRange = xlSheet.Cells[1, 1];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[1, 2] = "Username";
                xlRange = xlSheet.Cells[1, 2];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[1, 4] = "From Date";
                xlRange = xlSheet.Cells[1, 4];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[1, 5] = "To Date";
                xlRange = xlSheet.Cells[1, 5];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[1, 6] = "Date Created";
                xlRange = xlSheet.Cells[1, 6];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[2, 1] = model.IdShortened;
                xlSheet.Cells[2, 2] = model.Username;
                xlSheet.Cells[2, 4] = model.FromDate;
                xlSheet.Cells[2, 5] = model.ToDate;
                xlSheet.Cells[2, 6] = model.DateCreated;

                xlSheet.Cells[5, 1] = "Project ID";
                xlRange = xlSheet.Cells[5, 1];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 2] = "Project Name";
                xlRange = xlSheet.Cells[5, 2];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 3] = "Sunday";
                xlRange = xlSheet.Cells[5, 3];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 4] = "Monday";
                xlRange = xlSheet.Cells[5, 4];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 5] = "Tuesday";
                xlRange = xlSheet.Cells[5, 5];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 6] = "Wednesday";
                xlRange = xlSheet.Cells[5, 6];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 7] = "Thursday";
                xlRange = xlSheet.Cells[5, 7];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 8] = "Friday";
                xlRange = xlSheet.Cells[5, 8];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 9] = "Saturday";
                xlRange = xlSheet.Cells[5, 9];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 10] = "Total Hours";
                xlRange = xlSheet.Cells[5, 10];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[5, 11] = "Description";
                xlRange = xlSheet.Cells[5, 11];
                xlRange.Font.Bold = true;
                xlRange.BorderAround2();
                xlSheet.Cells[6, 1] = model.ProjectId;
                xlSheet.Cells[6, 2] = model.ProjectName;
                xlSheet.Cells[6, 3] = model.Sunday;
                xlSheet.Cells[6, 4] = model.Monday;
                xlSheet.Cells[6, 5] = model.Tuesday;
                xlSheet.Cells[6, 6] = model.Wednesday;
                xlSheet.Cells[6, 7] = model.Thursday;
                xlSheet.Cells[6, 8] = model.Friday;
                xlSheet.Cells[6, 9] = model.Saturday;
                xlSheet.Cells[6, 10] = model.TotalHours;
                xlSheet.Cells[6, 11] = model.Comment;

                xlSheet.Columns.AutoFit();
                xlSheet.Range["A1:F1"].Interior.Color = Color.LawnGreen;
                xlSheet.Range["A5:K5"].Interior.Color = Color.Yellow;

                string filePath = @"C:\Users\extre_000\Documents\GitHub\MVC_TimeSh\MVC_TimeSh\Content\ExcelExport\";
                string fileName = "Time" + model.TimeSheetMasterId + ".xlsx";
                xlBook.SaveAs(Path.Combine(filePath,fileName));
                xlBook.Close();
                xlApp.Quit();

                // xlApp.Workbooks.Open(Path.Combine(filePath, fileName));
                //workbook.LoadFromFile(@"..\..\D",ExcelVersion.Version97to2003
                //var workbook = xlBook.Open("~/Content/ExcelExport/Time.xlsx");
                //xlApp.Application.Workbooks.Open("~/Content/ExcelExport/" + fileName);

                return RedirectToAction("TimeSheetList");
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                TempData["Error"] = "TimeSheet Export was Unsuccessful";
                return RedirectToAction("TimeSheetList");
            }
        }
    }
}