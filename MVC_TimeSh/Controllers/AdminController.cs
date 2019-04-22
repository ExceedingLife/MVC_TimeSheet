using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_TimeSh.Controllers;
using MVC_TimeSh.Models;
using System.Data.Entity;

namespace MVC_TimeSh.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;

        public AdminController()
        {
            context = new ApplicationDbContext();
        }
        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            context = new ApplicationDbContext();
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: Admin
        public ActionResult Dashboard()
        {
            var userCount = context.Users.Count();
            ViewBag.UsersCount = userCount;

            var uid = User.Identity.GetUserId();
            ViewBag.CurrentId = uid;

            var roleCount = context.Roles.Count();
            ViewBag.RolesCount = roleCount;

            return View();
        }

        // GET: /User/Edit/{id}
        public async Task<ActionResult> EditUser(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRole = await UserManager.GetRolesAsync(user.Id);

            return View(new UpdateUserViewModel()
            {
                UserId = user.Id,
                IdShortened = user.Id.Substring(0, 10),
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                DateCreated = user.DateCreated,
                Email = user.Email,
                UserName = user.UserName,
                UserRoles = userRole.FirstOrDefault()
            });
        }
        // POST: /User/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser([Bind]UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.Birthday = model.Birthday;
                user.DateCreated = model.DateCreated;
                user.Email = model.Email;
                user.UserName = model.UserName;
                //context.Entry(user).State = EntityState.Modified;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await context.SaveChangesAsync();
                    TempData["Success"] = "User Updated Successfully";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["Error"] = "User Update Unsuccessful";
                    return RedirectToAction("Dashboard");
                }                
            }
            else
            {
                TempData["Error"] = "User Update Unsuccessful";
                return RedirectToAction("Dashboard");
            }
        }

        public ActionResult ChangePassword()
        {
            var user = User.Identity.GetUserName();
            UpdatePasswordModel update = new UpdatePasswordModel()
            {
                Username = user
            };

            return View(update);
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(UpdatePasswordModel model)
        {
            var user = await UserManager.FindAsync(User.Identity.Name, model.CurrentPassword);

            if(!UserManager.CheckPassword(user, model.CurrentPassword))
            {
                TempData["Error"] = "Incorrect Password!";
                return View();
            }
            else
            {
                if(model.Password != model.ConfirmPassword)
                {
                    TempData["Error"] = "Passwords DO NOT match!";
                    return View();
                }
                else
                {
                    string hashIt = UserManager.PasswordHasher.HashPassword(model.Password);
                    await UserManager.ChangePasswordAsync(user.Id, model.CurrentPassword,
                            hashIt);
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Password Updated Successfully";
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        TempData["Error"] = "Password Update Unsuccessful";
                        return RedirectToAction("Dashboard", "Admin");
                    }
                }
            }
        }

        public ActionResult RoleManager()
        {
            //var roleList = context.Roles.ToList()
            //.Select(r => new SelectListItem{Value = r.Name.ToString(),
            //Text = r.Name}).ToList();//ViewBag.Roles = roleList;
            GetRolesAsSelectListItem();
            var userList = context.Users.ToList();
            //var nonSuperAdmins = (from user in context.Users
            //from ur in user.Roles join role in context.Roles
            //on ur.RoleId equals role.Id where !role.Name.Contains("SuperAdmin")
            //select new SelectListItem(){Value = user.UserName.ToString(),
            //Text = user.UserName}).ToList();//ViewBag.NotSuper = nonSuperAdmins;
            GetNonSuperAdminAsSelectListItem();
            GetRolesNotSuperSelectList();

            var userWithRoles = (from user in context.Users
                                 from role in user.Roles
                                 join r in context.Roles
                                 on role.RoleId equals r.Id
                                 orderby user.UserName
                                 select new UserRolesMenuModel()
                                 {
                                     UserId = user.Id,
                                     IdShortened = user.Id.Substring(0, 10),
                                     Name = user.Name,
                                     Username = user.UserName,
                                     Role = r.Name
                                 }).ToList();
            return View(userWithRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetUserRole(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                var user = await UserManager.FindByNameAsync(username);
                var roles = await UserManager.GetRolesAsync(user.Id);
                // ?? ?? or a different way
                ViewBag.RolesForUser = roles;
                TempData["Success"] = "Roles Retrieved Successfully";
                GetRolesAsSelectListItem();
                GetNonSuperAdminAsSelectListItem();
                GetAllUsersList();
                GetRolesNotSuperSelectList();
               
                //return RedirectToAction("RoleManager");
                return View("RoleManager");
                //return View();
            }
            else
            {
                TempData["Error"] = "Role Retrieval Unsuccessful";
                return RedirectToAction("RoleManager");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserRole(string username, string role)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(role))
            {
                var user = await UserManager.FindByNameAsync(username);
                var currentRole = await UserManager.GetRolesAsync(user.Id);
                await UserManager.RemoveFromRoleAsync(user.Id, currentRole.FirstOrDefault());
                await UserManager.AddToRoleAsync(user.Id, role);
                TempData["Success"] = "Role Successfully Changed.";
                GetRolesAsSelectListItem();
                GetNonSuperAdminAsSelectListItem();
                GetAllUsersList();
                GetRolesNotSuperSelectList();
                return View("RoleManager");
            }
            else
            {
                TempData["Error"] = "Role Change Unsuccessful!";
                return RedirectToAction("RoleManager");
            }
            
        }
        public void GetRolesAsSelectListItem()
        {
            var roleList = context.Roles.ToList()
                .Select(r => new SelectListItem
                {
                    Value = r.Name.ToString(),
                    Text = r.Name
                }).ToList();
            ViewBag.Roles = roleList;
        }
        public void GetRolesNotSuperSelectList()
        {           
            var roleList = (from role in context.Roles
                            where !role.Name.Contains("SuperAdmin")
                            select new SelectListItem()
                            {
                                Value = role.Name.ToString(),
                                Text = role.Name
                            }).ToList();
            ViewBag.AdminChangeRoles = roleList;
            //ViewBag.AdminChangeRoles = new SelectList(context.Roles.Where(u =>
            // !u.Name.Contains("SuperAdmin")).ToList(), "Name", "Name");
        }
        public void GetNonSuperAdminAsSelectListItem()
        {
            var nonSuperAdmins = (from user in context.Users
                                  from ur in user.Roles
                                  join role in context.Roles
                                  on ur.RoleId equals role.Id
                                  where !role.Name.Contains("SuperAdmin")
                                  select new SelectListItem()
                                  {
                                      Value = user.UserName.ToString(),
                                      Text = user.UserName
                                  }).ToList();
            ViewBag.NotSuper = nonSuperAdmins;
        }
        public void GetAllUsersList()
        {
            var userWithRoles = (from user in context.Users
                                 from role in user.Roles
                                 join r in context.Roles
                                 on role.RoleId equals r.Id
                                 orderby r.Name
                                 select new UserRolesMenuModel()
                                 {
                                     UserId = user.Id,
                                     IdShortened = user.Id.Substring(0, 10),
                                     Name = user.Name,
                                     Username = user.UserName,
                                     Role = r.Name
                                 }).ToList();
            ViewBag.UserNrole = userWithRoles;
        }
    }
}