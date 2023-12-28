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
