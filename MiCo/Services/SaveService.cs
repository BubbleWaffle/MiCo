using MiCo.Data;
using MiCo.Helpers;

namespace MiCo.Services
{
    public class SaveService
    {
        private readonly MiCoDbContext _context;

        public SaveService(MiCoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method used to cancel report by removing from the database
        /// </summary>
        /// <param name="id">User account id passing by URL</param>
        /// <returns>Helper reporting success or error</returns>
        public async Task<ResultHelper> JusticeSave(int id)
        {
            var reportToRemove = await _context.reports.FindAsync(id);

            if (reportToRemove != null) 
            {
                _context.reports.Remove(reportToRemove);

                await _context.SaveChangesAsync();

                return new ResultHelper(true, "Report deleted successfully!");
            }

            return new ResultHelper(false, "This report doesn't exist!");
        }
    }
}
