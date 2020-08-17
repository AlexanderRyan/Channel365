using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Channel365.Data.Entities;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Channel365.Web.Areas.Identity.Pages.Account.Manage
{
    public class SettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IEmailSender emailSender;

        public SettingsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.emailSender = emailSender;
        }
        public string Username { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public Uri ImageUrl { get; set; }

        public string Email { get; set; }

        public bool IsFullNameSet { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Display(Name = "Username")]
            public string NewUsername { get; set; }

            [Display(Name = "First name")]
            public string SetFirstName { get; set; }

            [Display(Name = "Last name")]
            public string SetLastName { get; set; }

            [Display(Name = "Upload new profile image")]
            public Uri NewImageUrl { get; set; }

            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await userManager.GetUserNameAsync(user);
            var email = await userManager.GetEmailAsync(user);
            var phoneNumber = await userManager.GetPhoneNumberAsync(user);

            var imageUrl = GetCurrentImageUrl(user);
            var firstName = GetUserFirstName(user);
            var lastName = GetUserLastName(user);

            FirstName = firstName;
            LastName = lastName;
            ImageUrl = imageUrl;
            Email = email;

            Input = new InputModel
            {
                NewUsername = userName,
                SetFirstName = firstName,
                SetLastName = lastName,
                NewImageUrl = imageUrl,
                NewEmail = email,
                PhoneNumber = phoneNumber
            };

            IsFullNameSet = ConfirmFullNameSet(user);
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userName = await userManager.GetUserNameAsync(user);
            if (Input.NewUsername != userName)
            {
                var setNewUsername = await userManager.SetUserNameAsync(user, Input.NewUsername);
                if (!setNewUsername.Succeeded)
                {
                    var userId = await userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred changing username for user with ID '{userId}'.");
                }
            }

            var userImageUrl = GetCurrentImageUrl(user);
            if (Input.NewImageUrl != userImageUrl && Input.NewImageUrl != null)
            {
                user.UserImageUrl = Input.NewImageUrl;
                await context.SaveChangesAsync();
            }

            var phoneNumber = await userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await userManager.GetUserIdAsync(user);

                var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);

                await emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSetPersonalDetailsAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (!string.IsNullOrEmpty(Input.SetFirstName))
            {
                user.FirstName = Input.SetFirstName;
            }
            if (!string.IsNullOrEmpty(Input.SetLastName))
            {
                user.LastName = Input.SetLastName;
            }

            var fullNameSet = ConfirmFullNameSet(user);

            if (fullNameSet)
            {
                await context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        private Uri GetCurrentImageUrl(ApplicationUser user)
        {
            var currentImageUrl = user.UserImageUrl;

            return currentImageUrl;
        }
        private bool ConfirmFullNameSet(ApplicationUser user)
        {
            return user.FirstName != null && user.LastName != null;
        }
        private string GetUserLastName(ApplicationUser user)
        {
            return user.LastName;
        }
        private string GetUserFirstName(ApplicationUser user)
        {
            return user.FirstName;
        }
    }
}
