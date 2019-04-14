using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC_TimeSh.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.DateTime),
            DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
            ApplyFormatInEditMode = true)]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }

        [Required]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    // Adding new Models for Database TimesheetDB
    public class AdminModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
    public class UserModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool SelectedUsers { get; set; }
        public string AssignToAdmin { get; set; }
    }
    public class AssignRolesModel
    {
        public List<AdminModel> lstAdmins { get; set; }
        [Required(ErrorMessage = "Choose Admin")]
        public string UserId { get; set; }
        public List<UserModel> lstUsers { get; set; }
        public int? AssignToAdmin { get; set; }
        public int? CreatedBy { get; set; }
    }
    public class UserRolesMenuModel
    {
        public string UserId { get; set; }
        [Display(Name = "User ID")]
        public string IdShortened { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
    public class UsersViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User ID")]
        public string IdShortened { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Birthday { get; set; }
        public string DateCreated { get; set; }
    }
    public class UpdateUserViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User ID")]
        public string IdShortened { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "Your Name")]
        public string Name { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        //"{0:yyyy/mm/dd}",
        [DataType(DataType.Date),
            DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
            ApplyFormatInEditMode = true)]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Required]
        [Display(Name = "User Roles")]
        public string UserRoles { get; set; }
    }
    public class UpdatePasswordModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password may be Incorrect")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class Project
    {
        [Key]
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }
        [Required]
        [Display(Name = "Project Name")]        
        public string ProjectName { get; set; }
        [Display(Name = "Nature of Industry")]
        public string NatureOfIndustry { get; set; }
        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }
    }
    public class ProjectUpdateModel
    {
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }
        [Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [Display(Name = "Nature of Industry")]
        public string NatureOfIndustry { get; set; }
        [Required]
        [Display(Name = "Project Code")]
        public string ProjectCode { get; set; }
    }

    public class TimeSheetMaster
    {
        [Key]
        public int TimeSheetMasterId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TotalHours { get; set; }
        public string UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int TimeSheetStatus { get; set; }
        public string Comment { get; set; }
    }
    public class TimeSheetMasterModel
    {
        [Display(Name = "Time Sheet Master ID")]
        public int TimeSheetMasterId { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime ToDate { get; set; }
        [Display(Name = "Total Hours")]
        public int? TotalHours { get; set; }
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Month Submitted")]
        public string SubmitedMonth { get; set; }
        [Display(Name = "Status")]
        public int TimeSheetStatus { get; set; }
        public string Comment { get; set; }
    }
    public class TimeSheetDetails
    {
        [Key]
        public int TimeSheetId { get; set; }
        public string DaysOfWeek { get; set; }
        public int? Hours { get; set; }
        public DateTime? Period { get; set; }
        public int? ProjectId { get; set; }
        public string UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int TimeSheetMasterId { get; set; }
    }
    public class TimeSheetDetailsModel
    {
        [Display(Name = "Time Sheet ID")]
        public int TimeSheetId { get; set; }
        [Display(Name = "Day of Week")]
        public string DaysOfWeek { get; set; }
        [Display(Name = "Hours")]
        public int? Hours { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Time Sheet Master ID")]
        public int TimeSheetMasterId { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
    }
    public class TimeSheetProjectsModel
    {
        [DataType(DataType.Date),
         DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",
         ApplyFormatInEditMode = true)]
        public DateTime Proj1 { get; set; }

        public DateTime Proj2 { get; set; }
        public DateTime Proj3 { get; set; }
        public DateTime Proj4 { get; set; }
        public DateTime Proj5 { get; set; }
        public DateTime Proj6 { get; set; }
        public DateTime Proj7 { get; set; }

        /*  PROJECT 1   */
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d1 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d2 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d3 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d4 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d5 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d6 { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w1d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal1 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc1 { get; set; }

        /*  PROJECT 2   */
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d1 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d2 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d3 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d4 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d5 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d6 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w2d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal2 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc2 { get; set; }

        /*  PROJECT 3   */
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d1 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d2 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d3 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d4 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d5 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d6 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w3d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal3 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc3 { get; set; }

        /*  PROJECT 4   */
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d1 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d2 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d3 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d4 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d5 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d6 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w4d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal4 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc4 { get; set; }

        /*  PROJECT 5   */
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d1 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d2 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d3 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d4 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d5 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d6 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w5d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal5 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc5 { get; set; }

        /*  PROJECT 6   */
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d1 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d2 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d3 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d4 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d5 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d6 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        [Range(0, 24, ErrorMessage = "0 to 24")]
        public int? P1w6d7 { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter Only Numbers")]
        public int? ProjTotal6 { get; set; }
        [StringLength(50, ErrorMessage = "Text Length Exceeds Limit")]
        public string ProjDesc6 { get; set; }

        public string DaysOfWeek1 { get; set; }
        public string DaysOfWeek2 { get; set; }
        public string DaysOfWeek3 { get; set; }
        public string DaysOfWeek4 { get; set; }
        public string DaysOfWeek5 { get; set; }
        public string DaysOfWeek6 { get; set; }
        public string DaysOfWeek7 { get; set; }

        [Required(ErrorMessage = "Choose Project")]
        public int? ProjectId1 { get; set; }
        public int? ProjectId2 { get; set; }
        public int? ProjectId3 { get; set; }
        public int? ProjectId4 { get; set; }
        public int? ProjectId5 { get; set; }
        public int? ProjectId6 { get; set; }
        public int? ProjectId7 { get; set; }
    }
}
