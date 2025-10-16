using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IReportService
    {
        // Admin report queries
        IEnumerable<NewsArticle> GetNewsStatisticsByDateRange(DateTime startDate, DateTime endDate, string? keyword = null, int page = 1, int pageSize = 20);
        IEnumerable<object> GetCategoryStatistics(DateTime startDate, DateTime endDate);
        IEnumerable<object> GetAuthorStatistics(DateTime startDate, DateTime endDate);
        object GetOverallStatistics(DateTime startDate, DateTime endDate);
    }
}
