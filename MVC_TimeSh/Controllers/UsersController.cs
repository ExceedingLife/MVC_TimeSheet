using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_TimeSh.Models;
using MVC_TimeSh.Controllers;
using PagedList;

namespace MVC_TimeSh.Controllers
{
    //[Authorize(Roles = "Users, Developer")]
    [Authorize]
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

        public IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
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

        // GET: /User/Create/{id}
        [Authorize]
        public ActionResult CreateUser()
        {
            var model = new RegisterViewModel();
            model.Birthday = DateTime.Today.AddYears(-125);

            ViewBag.Roles = new SelectList(context.Roles.Where(u =>
           !u.Name.Contains("SuperAdmin")).ToList(), "Name", "Name");

            return View(model);
        }
        //POST: /User/Create/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    Birthday = model.Birthday,
                    DateCreated = Convert.ToDateTime(model.DateCreated)                
                };
                string hashPass = UserManager.PasswordHasher.HashPassword(model.Password);
                var result = await UserManager.CreateAsync(user, hashPass);
                TempData["Success"] = "User Created Successfully";
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                }
                return RedirectToAction("UsersTable");
                //return RedirectToAction("UserDashboard");
            }
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
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
                AuthManager.SignOut();

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

        // GET: Users List
        public ActionResult UsersTable(string sortOrder, string searchString,
                                       string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CreateSortParam = String.IsNullOrEmpty(sortOrder) ? "CreateSortDesc" : "";
            ViewBag.IdSortParam = sortOrder == "IdSort" ? "IdSortDesc" : "IdSort";
            ViewBag.NameSortParam = sortOrder == "NameSort" ? "NameSortDesc" : "NameSort";
            ViewBag.BdaySortParam = sortOrder == "BdaySort" ? "BdaySortDesc" : "BdaySort";
            ViewBag.EmailSortParam = sortOrder == "EmailSort" ? "EmailSortDesc" : "EmailSort";
            ViewBag.UnameSortParam = sortOrder == "UnameSort" ? "UnameSortDesc" : "UnameSort";
            ViewBag.RoleSortParam = sortOrder == "RoleSort" ? "RoleSortDesc" : "RoleSort";

            var usersWithRole = (from user in context.Users
                                 from userRole in user.Roles
                                 join role in context.Roles on userRole.RoleId
                                 equals role.Id
                                 //orderby user.DateCreated
                                 select new UsersViewModel()
                                 {
                                     UserId = user.Id,
                                     IdShortened = user.Id.Substring(0, 10),
                                     Name = user.Name,
                                     Birthday = user.Birthday.ToString(),
                                     PhoneNumber = user.PhoneNumber,
                                     Email = user.Email,
                                     UserName = user.UserName,
                                     DateCreated = user.DateCreated.ToString(),
                                     Role = role.Name
                                 });            
            if (usersWithRole == null)
                return HttpNotFound();

            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                usersWithRole = usersWithRole.Where(
                                    u => u.UserName.Contains(searchString)
                                      || u.Email.Contains(searchString)
                                      || u.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "IdSort":
                    usersWithRole = usersWithRole.OrderBy(u => u.IdShortened);
                    break;
                case "IdSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.IdShortened);
                    break;
                case "NameSort":
                    usersWithRole = usersWithRole.OrderBy(u => u.Name);
                    break;
                case "NameSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.Name);
                    break;
                case "BdaySort":
                    usersWithRole = usersWithRole.OrderBy(u => u.Birthday);
                    break;
                case "BdaySortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.Birthday);
                    break;
                case "EmailSort":
                    usersWithRole = usersWithRole.OrderBy(u => u.Email);
                    break;
                case "EmailSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.Email);
                    break;
                case "UnameSort":
                    usersWithRole = usersWithRole.OrderBy(u => u.UserName);
                    break;
                case "UnameSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.UserName);
                    break;
                case "RoleSort":
                    usersWithRole = usersWithRole.OrderBy(u => u.Role);
                    break;
                case "RoleSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.Role);
                    break;
                case "CreateSortDesc":
                    usersWithRole = usersWithRole.OrderByDescending(u => u.DateCreated);
                    break;
                default:
                    usersWithRole = usersWithRole.OrderBy(u => u.DateCreated);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            //return View(usersWithRole.ToList());
            return View(usersWithRole.ToPagedList(pageNumber, pageSize));
        }

        // GET: /User/ChangePassword/{id}
        public ActionResult ChangePassword()
        {
            var user = User.Identity.GetUserName();
            UpdatePasswordModel update = new UpdatePasswordModel()
            {
                Username = user
            };

            return View(update);
        }
        // POST: /User/ChangePassword/{id}
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

        // GET: /User/UserDetails/{id} (Modal ajax())
        public async Task<ActionResult> UserDetails(string userId)
        {
            try
            {
                if(userId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var user = await UserManager.FindByIdAsync(userId);
                UsersViewModel model = new UsersViewModel()
                {
                    UserId = user.Id,
                    IdShortened = user.Id.Substring(0, 10),
                    Birthday = user.Birthday.ToString(),
                    Name = user.Name,
                    DateCreated = user.DateCreated.ToString(),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName

                };
                return PartialView("UserDetails", model);
            }
            catch(Exception ex)
            {
                //Console.Write(ex.Message);
                TempData["Error"] = "Error retrieving User Details";
                return RedirectToAction("UserDashboard", "Users");
            }
        }

    }
}