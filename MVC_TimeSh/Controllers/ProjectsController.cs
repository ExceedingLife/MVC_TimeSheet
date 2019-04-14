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
    public class ProjectsController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;

        public ProjectsController()
        {
            context = new ApplicationDbContext();
        }
        public ProjectsController(ApplicationUserManager userManager, 
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

        // GET: Projects
        public ActionResult ProjectListAll()
        {
            var projectList = (from projects in context.Projects
                               select new
                               {
                                   Id = projects.ProjectId,
                                   Name = projects.ProjectName,
                                   Code = projects.ProjectCode,
                                   Industry = projects.NatureOfIndustry
                               }).ToList().Select(proj => new Project()
                               {
                                   ProjectId = proj.Id,
                                   ProjectName = proj.Name,
                                   ProjectCode = proj.Code,
                                   NatureOfIndustry = proj.Industry
                               });
            if(projectList == null)
                return HttpNotFound();

            return View(projectList);
        }

        // GET: CreateProject
        public ActionResult CreateProject()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProject(Project model)
        {
            if (ModelState.IsValid)
            {

                context.Projects.Add(model);
                await context.SaveChangesAsync();
                TempData["Success"] = "Project Created Successfully";
                return RedirectToAction("ViewProjects", "SuperAdmin");
            }
            TempData["Error"] = "Project Create Was Unsuccessful";
            return RedirectToAction("ViewProjects", "SuperAdmin");
        }
        
        //  SUPER ADMINISTRATOR PROJECT EDITS ONLY

        public ActionResult EditProject(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = context.Projects.Where(p =>
                p.ProjectId.Equals(id)).FirstOrDefault();
            if (project == null)
            {
                return HttpNotFound();
            }
            //"~/Views/Projects/EditProject.cshtml"
            return View("EditProject", new ProjectUpdateModel()
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectCode = project.ProjectCode,
                NatureOfIndustry = project.NatureOfIndustry
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject([Bind]ProjectUpdateModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var project = context.Projects.Where(id => id.ProjectId
                .Equals(model.ProjectId)).FirstOrDefault();
                project.ProjectName = model.ProjectName;
                project.ProjectCode = model.ProjectCode;
                project.NatureOfIndustry = model.NatureOfIndustry;
                try
                {
                    context.Entry(project).State = EntityState.Modified;
                    context.SaveChanges();
                    TempData["Success"] = "Project Updated Successfully";
                    return RedirectToAction("ViewProjects", "SuperAdmin");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Project Update Error";
                    return RedirectToAction("ViewProjects", "SuperAdmin");
                }
            }
            TempData["Error"] = "Project Update Error";
            return RedirectToAction("ViewProjects", "SuperAdmin");
        }

        public ActionResult DeleteProject(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = context.Projects.Where(p =>
                p.ProjectId.Equals(id)).FirstOrDefault();
            if (project == null)
            {
                return HttpNotFound();
            }
            return View("DeleteProject", new ProjectUpdateModel()
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectCode = project.ProjectCode,
                NatureOfIndustry = project.NatureOfIndustry
            });
        }
        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProjectConfirmed(ProjectUpdateModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var project = context.Projects.Where(id => id.ProjectId
                .Equals(model.ProjectId)).FirstOrDefault();
                try
                {
                    context.Projects.Remove(project);
                    context.SaveChanges();
                    TempData["Success"] = "Project Deleted Successfully";
                    return RedirectToAction("ViewProjects", "SuperAdmin");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Project Delete Unsuccessful";
                    return RedirectToAction("ViewProjects", "SuperAdmin");
                }
            }
            TempData["Error"] = "Project Delete Unsuccessful";
            return RedirectToAction("ViewProjects", "SuperAdmin");
        }
        
    }
}