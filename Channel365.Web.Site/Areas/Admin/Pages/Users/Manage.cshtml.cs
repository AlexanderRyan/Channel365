using Channel365.Data.Entities;
using Channel365.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Channel365.Web.Areas.Admin.Pages.Users
{
    public class ManageModel : PageModel
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<ManageModel> logger;

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public ManageModel(
            ApplicationDbContext context,
            ILogger<ManageModel> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await context.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetFindMatchAsync(string search)
        {
            IList<ApplicationUser> users = new List<ApplicationUser>();

            if (search != null && search.IndexOf(':') == 1)
            {
                var result = search.Split(':');

                switch (result[0])
                {
                    case "@":
                        users = await context.Users
                                .AsNoTracking()
                                .Where(x => x.Email.Contains(result[1]))
                                .ToListAsync();
                        break;

                    case "?":
                        users = await context.Users
                            .AsNoTracking()
                            .Where(x => x.Id == result[1])
                            .ToListAsync();
                        break;
                }
            }
            else
            {
                if (search == null)
                {
                    users = await context.Users
                        .AsNoTracking()
                        .OrderBy(x => x.UserName)
                        .ToListAsync();
                }
                else
                {
                    users = await context.Users
                        .AsNoTracking()
                        .Where(x => x.UserName.Contains(search))
                        .OrderBy(x => x.UserName)
                        .ToListAsync();
                }
            }

            return new JsonResult(users);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            string statusMessage;

            if (user == null)
            {
                statusMessage = "Error: Specified resource could not be located. (Video Id: )" + id;
                logger.LogInformation($"User with ID {id} could not be found.");
                return new JsonResult(statusMessage);
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            statusMessage = id + " successfully removed.";
            logger.LogInformation($"User with ID {id} has been removed from the database.");
            return new JsonResult(statusMessage);
        }

        public async Task<IActionResult> OnPostEditAsync(string id, string userName, string firstName, string lastName, string email, string phoneNr)
        {
            var userToEdit = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            string statusMessage;

            if (userToEdit == null)
            {
                statusMessage = $"Error: unable to locate specified resource. User-id: {id}";
                logger.LogWarning($"Error locating User. (id: {id})");

                return new JsonResult(statusMessage);
            }

            userToEdit.UserName = userName ?? userToEdit.UserName;
            userToEdit.FirstName = firstName ?? userToEdit.FirstName;
            userToEdit.LastName = lastName ?? userToEdit.LastName;
            userToEdit.Email = email ?? userToEdit.Email;
            userToEdit.PhoneNumber = phoneNr ?? userToEdit.PhoneNumber;

            context.Entry(userToEdit).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (context.Users.Any(x => x.Id == id))
                {
                    statusMessage = $"Concurrency Exception - User-id ({id}).";
                    logger.LogError($"Concurrency Exception - User-id ({id}).");
                    return new JsonResult("");
                }
                else
                    throw;
            }

            statusMessage = $"{id} successfully edited.";
            logger.LogInformation($"User updated. (id: {id})");
            return new JsonResult(statusMessage);
        }
        public async Task<IActionResult> OnGetReturnEditedAsync(string id)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                logger.LogError($"Error retrieving element from database: {id}");
                return NotFound("Error: Unable to located specified resource.");
            }

            return new JsonResult(user);
        }
    }
}
