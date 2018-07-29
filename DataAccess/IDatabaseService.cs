using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess
{
    /// <summary>
    /// Local interface to persistence layer.
    /// </summary>
    public interface IDatabaseService
    {
        Task<List<Dictionary<string, dynamic>>> SelectRowsAsync(string tableName,
            Dictionary<string, dynamic> whereConditions);
        Task<dynamic> InsertRowAsync(string tableName, Dictionary<string, dynamic> columnValuePairs);
        Task<bool> ExistsAsync(string tableName, Dictionary<string, dynamic> whereConditions);
        Task UpdateRowAsync(string tableName, Dictionary<string, dynamic> columnValuePairs, Dictionary<string, dynamic> whereConditions);
    }
}
