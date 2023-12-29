using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;
using MiCo.Models.ViewModels;

namespace MiCo.Services
{
    public class ProfileReportService
    {
        private readonly MiCoDbContext _context;

        public ProfileReportService(MiCoDbContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Method used to report specific user
        /// </summary>
        /// <param name="id_reported_user">Id reported user</param>
        /// <param name="id_reporting_user">Id reporting user (currently logged user)</param>
        /// <param name="model">View model passing report data</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> ProfileReport(int id_reported_user, int? id_reporting_user, ProfileReportViewModel model)
        {
            int non_nullable_id = id_reporting_user ?? default(int); //Convert nullable int value to non-nullable

            var existing_report = _context.reports
                .Where(r => r.id_reporting_user == non_nullable_id && r.id_reported_user == id_reported_user && 
                r.report_date >= DateTimeOffset.Now.AddDays(-1))
                .FirstOrDefault();

            if (existing_report != null)
                return new ResultHelper(false, "You have recently reported this user! Take a break.");

            if (string.IsNullOrWhiteSpace(model.reason))
                return new ResultHelper(false, "You have to enter reason!");

            if (model.reason.Length > 300)
                return new ResultHelper(false, "Your reason is too long!");

            var newReport = new Reports
            {
                id_reported_user = id_reported_user,
                id_reporting_user = non_nullable_id,
                reason = model.reason,
                report_date = DateTimeOffset.Now
            };

            _context.reports.Add(newReport);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "We received your report!");
        }
    }
}
