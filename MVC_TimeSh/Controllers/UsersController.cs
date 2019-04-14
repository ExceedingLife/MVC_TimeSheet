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
using System.Web.Hosting;

namespace MVC_TimeSh.Controllers
{
    
    public class UsersController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext context;

        public UsersController()
        {
            context = new ApplicationDbContext();
        }
        public UsersController(ApplicationUserManager userManager, 
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
                return _signInManager ?? HttpContext
                    .GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext
                    .GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: User Dashboard.
        [Authorize]
        public ActionResult UserDashboard()
        {
            var userCount = context.Users.Count();
            ViewBag.UsersCount = userCount;
            
            var uid = User.Identity.GetUserId();
            ViewBag.CurrentId = uid;

            return View();
        }

        // Get: /User/Edit/{id}
        [Authorize]
        public async Task<ActionResult> EditUser(string id)
        {
            if (id == null)
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
                    TempData["Success"] = "Your Account Updated Successfully";
                    return RedirectToAction("UserDashboard");
                }
                else
                {
                    TempData["Error"] = "Your Account Update Unsuccessful";
                    return RedirectToAction("UserDashboard");
                }
            }
            else
            {
                TempData["Error"] = "Your Account Update Unsuccessful";
                return RedirectToAction("UserDashboard");
            }
        }

        // GET: /User/Delete/{id}
        [Authorize]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (id == null)
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
        // POST: /User/Delete/{id}
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]//(string id)
        public async Task<ActionResult> DeleteConfirmed(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {//(id==null)//
                if(model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //AccountController controller = new AccountController();
                //(id);//
                var user = await UserManager.FindByIdAsync(model.UserId);
                var logins = user.Logins;
                var roles = await UserManager.GetRolesAsync(user.Id);
                if(logins != null)
                {
                    foreach(var login in logins)
                    {
                        await UserManager.RemoveLoginAsync(login.UserId,
                            new UserLoginInfo(
                                login.LoginProvider, login.ProviderKey));
                    }
                }
                // Supposed to only be 1 role but just in case -
                if(roles.Count() > 0)
                {
                    foreach(var r in roles)
                    {
                        var deleteRole = await UserManager.RemoveFromRoleAsync(user.Id, r);
                    }
                }
                
                //ApplicationManager.
                // controller.LogOff();
                //WebSecurity.Logout() doesnt find.
                //FormsAuthentication.SignOut();
                //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                //Roles.DeleteCookie();
                //Request.GetOwinContext.
                //Session.Clear();
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User Deleted Successfully";
                    return RedirectToAction("UsersTable", "Users");
                }                
            }
            TempData["Error"] = "User Delete Unsuccessful";
            return RedirectToAction("UserDashboard", "Users");
        }

        // GET: Users
        public ActionResult UsersTable()
        {
            //var lstUsers = context.Users.ToList();
            //List<Customer> data = await query.ToListAsync();
            var usersWithRole = (from user in context.Users
                                 from userRole in user.Roles
                                 join role in context.Roles on userRole.RoleId
                                 equals role.Id
                                 orderby user.DateCreated
                                 select new UsersViewModel()
                                 {
                                     UserId = user.Id,
                                     IdShortened = user.Id.Substring(0,10),
                                     Name = user.Name,
                                     Birthday = user.Birthday.ToString(),
                                     PhoneNumber = user.PhoneNumber,
                                     Email = user.Email,
                                     UserName = user.UserName,
                                     DateCreated = user.DateCreated.ToString(),
                                     Role = role.Name
                                 }).ToList();

            if (usersWithRole == null)
                return HttpNotFound();

            return View(usersWithRole);
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
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(User.Identity.Name, model.CurrentPassword);

                if ((await UserManager.CheckPasswordAsync(user, model.CurrentPassword)) == false)
                {
                    TempData["Error"] = "Incorrect Password!";
                    return View();
                }
                else
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        TempData["Error"] = "Passwords DO NOT match!";
                        return View();
                    }
                    else
                    {
                        string hashPass = UserManager.PasswordHasher.HashPassword(model.Password);
                        await UserManager.ChangePasswordAsync(user.Id, model.CurrentPassword, hashPass);
                        var result = await UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            TempData["Success"] = "Password Updated Successfully";
                            return RedirectToAction("UserDashboard", "Users");
                        }
                        else
                        {
                            TempData["Error"] = "Password Update Unsuccessful";
                            return RedirectToAction("UserDashboard", "Users");
                        }
                    }
                }
            }
            else
            {
                TempData["Error"] = "Password Update Unsuccessful";
                return RedirectToAction("UserDashboard", "Users");
            }
        }
    }
}