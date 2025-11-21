
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Interface;
using ExcelImporter.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.Users
{
    /// <summary>
    /// Page model for the users list page
    /// </summary>
    public class IndexModel : AdminPageModel
    {
        private readonly IUsersService _usersService;
        private readonly IExcelExporter _excelExporter;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IUsersService usersService,
            IExcelExporter excelExporter,
            ILogger<IndexModel> logger)
        {
            _usersService = usersService;
            _excelExporter = excelExporter;
            _logger = logger;
        }

        /// <summary>
        /// List of users to display
        /// </summary>
        public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();

        /// <summary>
        /// Filter parameters for searching and pagination
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public UserFilterDto Filter { get; set; } = new UserFilterDto();

        /// <summary>
        /// Total count of users matching the filter
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total number of users in the system
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Total number of admin users
        /// </summary>
        public int TotalAdmins { get; set; }

        /// <summary>
        /// Total number of regular users
        /// </summary>
        public int TotalRegularUsers { get; set; }

        /// <summary>
        /// Total number of manager users
        /// </summary>
        public int TotalManagers { get; set; }

        /// <summary>
        /// Handles GET requests to display the users list
        /// </summary>
        public async Task OnGetAsync()
        {
            // Get filtered users
            var (users, totalCount) = await _usersService.SearchAsync(Filter);
            Users = users;
            TotalCount = totalCount;

            // Calculate pagination
            TotalPages = (int)Math.Ceiling(totalCount / (double)Filter.PageSize);

            // Get statistics
            TotalUsers = await _usersService.GetTotalCountAsync();
            var admins = await _usersService.GetByRoleAsync("Admin");
            var regularUsers = await _usersService.GetByRoleAsync("User");
            var managers = await _usersService.GetByRoleAsync("Manager");

            TotalAdmins = admins.Count();
            TotalRegularUsers = regularUsers.Count();
            TotalManagers = managers.Count();
        }

        /// <summary>
        /// Export users/clients to Excel
        /// </summary>
        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            try
            {
                var fileBytes = await _excelExporter.ExportClientsAsync();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users to Excel");
                TempData["Error"] = "Failed to export users to Excel.";
                return RedirectToPage();
            }
        }
    }
}