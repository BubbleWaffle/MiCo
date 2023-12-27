using MiCo.Data;
using MiCo.Helpers;
using MiCo.Models;

namespace MiCo.Services
{
    public class ProfileReportService
    {
        private readonly MiCoDbContext _context;

        public ProfileReportService(MiCoDbContext context) 
        {
            _context = context;
        }

        public async Task<ResultHelper> ProfileReport(int id_reported_user, int? id_reporting_user, string reason)
        {
            int non_nullable_id = id_reporting_user ?? default(int); //Convert nullable int value to non-nullable

            var existingReport = _context.reports
                .Where(r => r.id_reporting_user == non_nullable_id && r.id_reported_user == id_reported_user && 
                r.report_date >= DateTimeOffset.Now.AddDays(-1))
                .FirstOrDefault();

            if (existingReport != null)
                return new ResultHelper(false, "You have recently reported this user! Take a break.");

            if (string.IsNullOrWhiteSpace(reason))
                return new ResultHelper(false, "You have to enter reason!");

            /* Creat report object */
            var newReport = new Reports
            {
                id_reported_user = id_reported_user,
                id_reporting_user = non_nullable_id,
                reason = reason,
                report_date = DateTimeOffset.Now
            };

            _context.reports.Add(newReport);
            await _context.SaveChangesAsync();

            return new ResultHelper(true, "We received your report!");
        }
    }
}
