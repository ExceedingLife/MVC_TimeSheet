using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_TimeSh.Models;
using System.Net;

namespace MVC_TimeSh.Controllers
{
    //[Authorize(Roles = "Admin, SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private ApplicationDbContext context;

        public SuperAdminController()
        {
            context = new ApplicationDbContext();
        }

        // GET: SuperAdmin
        public ActionResult Dashboard()
        {
            var userCount = context.Users.Count();
            ViewBag.UsersCount = userCount;

            var roleCount = context.Roles.Count();
            ViewBag.RolesCount = roleCount;

            var uid = User.Identity.GetUserId();
            ViewBag.CurrentId = uid;

            var adminCount = AdminList().Count();
            ViewBag.AdminCount = adminCount;

            return View();
        }

        public ActionResult GetAllUsers()
        {
            var usersWithRole = (from user in context.Users
                                 from userRole in user.Roles
                                 join role in context.Roles on userRole.RoleId
                                 equals role.Id
                                 orderby user.DateCreated
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
                                 }).ToList();

            if (usersWithRole == null)
                return HttpNotFound();

            return View(usersWithRole);
        }


        //[ValidateAntiForgeryToken]
        //[HttpPost]
        public async Task<ActionResult> DeleteBySuper(string id)
        {
            if (ModelState.IsValid)
            {
                if(id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = await manager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var roleUser = await manager.GetRolesAsync(id);
                if(roleUser.Count() > 0)
                {
                    foreach (var r in roleUser.ToList())
                    {
                        var resultRole = await manager.RemoveFromRoleAsync(id, r);
                    }
                }
                var result = await manager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User Removed Successfully & User Role";
                    return RedirectToAction("Dashboard", "SuperAdmin");
                }
                else
                {
                    TempData["Error"] = "ERROR Removing User & User Role";
                    return RedirectToAction("Dashboard", "SuperAdmin");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }        

        public async Task<ActionResult> EditSuperAdmin(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = await manager.FindByIdAsync(id);
            if(user == null)
            {
                return HttpNotFound();
            }
            var userRoles = await manager.GetRolesAsync(user.Id);
            if (userRoles != null)
                //ViewBag.RoleEdit = userRoles.FirstOrDefault();
                // userRoles;
                TempData["EditRole"] = userRoles.FirstOrDefault();
            ViewBag.Roles = new SelectList(
                context.Roles.ToList(), "Name", "Name");
                //new SelectList(context.Roles.Where(u =>
               //!u.Name.Contains("SuperAdmin")).ToList(), "Name", "Name");

            return View(new UpdateUserViewModel()
            {
                UserId = user.Id,
                IdShortened = user.Id.Substring(0, 10),
                Email = user.Email,
                UserName = user.UserName,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                DateCreated = user.DateCreated,
                UserRoles = manager.GetRoles(user.Id).FirstOrDefault()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSuperAdmin([Bind]UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);            
                var user = manager.FindById(model.UserId);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.Birthday = model.Birthday;
                user.DateCreated = Convert.ToDateTime(model.DateCreated);
                string role = "";
                role = TempData["EditRole"].ToString();
                //role = ViewBag.RoleEdit;
                //string role = ViewBag.RoleEdit ?? "";
                manager.RemoveFromRole(user.Id, role);
                var roleResult =
                    manager.AddToRole(user.Id, model.UserRoles);
                var result =  manager.Update(user);
                if (result.Succeeded)
                {
                    context.SaveChanges();
                    TempData["Success"] = "User Updated Successfully";
                    return RedirectToAction("GetAllUsers", "SuperAdmin");
                }
                else
                {
                    TempData["ErrorRole"] = "Error adding User Role";
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                TempData["Error"] = "User Update Failed";
                return RedirectToAction("Dashboard");
            }            
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditSuperAdmin([Bind]UpdateUserViewModel model)
        //{
        //    var store = new UserStore<ApplicationUser>(context);
        //    var manager = new UserManager<ApplicationUser>(store);

        //    //ModelState.Remove("PhoneNumber");
        //    //ModelState.Remove("Birthday");
        //    //ModelState.Remove("Name");
        //    //ModelState.Remove("UserRoles");

        //    if (ModelState.IsValid)
        //    {
        //        var user = manager.FindById(model.UserId);
        //        if (user == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        user.Email = model.Email;
        //        user.UserName = model.UserName;
        //        user.Name = model.Name;
        //        user.PhoneNumber = model.PhoneNumber;
        //        user.Birthday = model.Birthday;
        //        user.DateCreated = Convert.ToDateTime(model.DateCreated);
        //        var roleResult =
        //            manager.AddToRole(user.Id, model.UserRoles);
        //        if (!roleResult.Succeeded)
        //        {
        //            TempData["ErrorRole"] = "Error adding User Role";
        //            return RedirectToAction("Dashboard");
        //        }
        //        manager.Update(user);
        //        context.SaveChanges();                
        //        TempData["Success"] = "User Updated Successfully";              
        //        return RedirectToAction("GetAllUsers", "SuperAdmin");
        //    }
        //    //foreach (ModelState modelState in ViewData.ModelState.Values)
        //    //{
        //    //    foreach (ModelError error in modelState.Errors)
        //    //    {
        //    //        List<ModelError> lsterrors = new List<ModelError>();
        //    //        lsterrors.Add(error);
        //    //    }
        //    //}
        //    TempData["Error"] = "User Update Failed";
        //    return RedirectToAction("Dashboard");
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditSuperAdmin([Bind]UpdateUserViewModel model)
        //{
        //        var user = await manager.FindByIdAsync(model.UserId);        
        //        var roleResult = 
        //            await manager.AddToRoleAsync(user.Id, model.UserRoles);
        //}


        public ActionResult RoleManager()
        {
            var roleList = context.Roles.OrderBy(r => r.Name).ToList()
                .Select(rr => new SelectListItem
                {
                    Value = rr.Name.ToString(),
                    Text = rr.Name
                }).ToList();
            ViewBag.Roles = roleList;

            var roleCount = context.Roles.Count();
            ViewBag.RolesCount = roleCount;

            var userWithRole = (from user in context.Users
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
            //ViewBag.UserRoles = userWithRole;

            return View(userWithRole);
        }

        // POST: /Roles/Create
        [HttpPost]
        public ActionResult CreateRole(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                TempData["Success"] = "Role Created Successfully";
                return RedirectToAction("RoleManager");
            }
            catch
            {
                return View();
            }
        }

        // DELETE ROLE
        public ActionResult DeleteRole(string rolename)
        {
            var selectedRole = context.Roles.Where(r => r.Name.Equals(rolename,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(selectedRole);
            context.SaveChanges();
            TempData["Error"] = "Role Removed Successfully";
            return RedirectToAction("RoleManager", "SuperAdmin");
        }

        // ASSIGN ADMINISTRATOR
        public ActionResult AssignAdmin()
        {
            //var lstAdmins = (from users in context.Users
            //                 from roles in users.Roles
            //                 join r in context.Roles
            //                 on roles.RoleId equals r.Id
            //                 where r.Name.Equals("Admin")
            //                 select new AssignRolesModel()
            //                 { }).ToList();
            AssignRolesModel model = new AssignRolesModel();
            model.lstAdmins = AdminList();
            model.lstUsers = UserList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignAdmin(AssignRolesModel model)
        {
            try
            {
                if(model.lstUsers == null)
                {
                    TempData["Error"] = "There are no Users to Assign Roles";
                    model.lstUsers = UserList();
                    model.lstAdmins = AdminList();
                    return View(model);
                }

                var selectedUsersCount = (from user in model.lstUsers
                                          where user.SelectedUsers == true
                                          select user).Count();
                if(selectedUsersCount == 0)
                {
                    TempData["Error"] = "You have not Selected any User to Assign Roles";
                    model.lstAdmins = AdminList();
                    model.lstUsers = UserList();
                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    List<UserModel> users = new List<UserModel>();
                    ApplicationUser au;
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new UserManager<ApplicationUser>(store);
                    model.CreatedBy = 1;
                    
                    foreach(var u in model.lstUsers)
                    {
                        if(u.SelectedUsers == true)
                        {
                            users.Add(u);
                        }
                    }

                    foreach(var u in users)
                    {
                        au = context.Users.Where(x => x.Id.Equals(u.UserId,
                            StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        manager.RemoveFromRole(au.Id, "User");
                        manager.AddToRole(au.Id, "Admin");
                    }

                    TempData["Success"] = "Roles Assigned Successfully";
                    return RedirectToAction("AssignAdmin");
                }
            }
            catch (Exception)
            {
                throw;
            }           

            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult RemoveAdmin(string userid)
        {
            ApplicationUser au = context.Users.Where(u => u.Id.Equals(userid,
                StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            
            manager.RemoveFromRole(au.Id, "Admin");
            manager.AddToRole(au.Id, "User");
            if (Request.IsAjaxRequest())
            {
                TempData["SuccessRole"] = "Role Removed Successfully";
                Json(Url.Action("AssignAdmin"));
                return RedirectToAction("AssignAdmin", "SuperAdmin");
            }
            
            //TempData["Success"] = "Roles Assigned Successfully";
            return RedirectToAction("AssignAdmin");
        }

        public List<AdminModel> AdminList()
        {
            var lstAdmin = (from u in context.Users
                            from r in u.Roles
                            join rr in context.Roles
                            on r.RoleId equals rr.Id
                            where rr.Name.Equals("Admin")
                            select new AdminModel()
                            {
                               UserId = u.Id,
                               Name = u.Name,
                               Username = u.UserName
                            }).ToList();

            List<AdminModel> listAdmins = lstAdmin;
            //lstAdmins.Add(new AdminModel)
            return listAdmins;
        }
        public List<UserModel> UserList()
        {
            var lstUser = (from u in context.Users
                            from r in u.Roles
                            join rr in context.Roles
                            on r.RoleId equals rr.Id
                            where rr.Name.Equals("User")
                            select new UserModel()
                            {
                                UserId = u.Id,
                                Name = u.Name,
                                Username = u.UserName,
                                SelectedUsers = false,
                                AssignToAdmin = "0"                                
                            }).ToList();

            List<UserModel> listUser = lstUser;
            //lstAdmins.Add(new AdminModel)
            return listUser;
        }

        public ActionResult ViewAdmins()
        {
            var lstAdmins = (from users in context.Users
                             from role in users.Roles
                             join roleL in context.Roles
                             on role.RoleId equals roleL.Id
                             where roleL.Name.Equals("Admin")
                             select new UserRolesMenuModel()
                             {
                                 UserId = users.Id,
                                 IdShortened = users.Id.Substring(0, 10),
                                 Name = users.Name,
                                 Role = roleL.Name,
                                 Username = users.UserName
                             }).ToList();
            return View(lstAdmins);
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
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = await manager.FindAsync(User.Identity.Name, model.CurrentPassword);

                if( (await manager.CheckPasswordAsync(user, model.CurrentPassword))==false)
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
                        string hashPass = manager.PasswordHasher.HashPassword(model.Password);
                        await manager.ChangePasswordAsync(user.Id, model.CurrentPassword, hashPass);
                        var result = await manager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            TempData["Success"] = "Password Updated Successfully";
                            return RedirectToAction("Dashboard", "SuperAdmin");
                        }
                        else
                        {
                            TempData["Error"] = "Password Update Unsuccessful";
                            return RedirectToAction("Dashboard", "SuperAdmin");
                        }
                    }
                }
            }
            else
            {
                TempData["Error"] = "Password Update Unsuccessful";
                return RedirectToAction("Dashboard", "SuperAdmin");
            }
        }
    }
}