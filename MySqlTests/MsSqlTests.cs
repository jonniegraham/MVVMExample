using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DataAccess.Tests
{
    [TestClass()]
    public class MsSqlTests
    {
        private IDatabaseService databaseService;

        [TestInitialize]
        public void Initialize()
        {
            databaseService = new MsSql();
        }

        [TestMethod()]
        public void SelectRowsAsyncTest()
        {
            databaseService.UpdateRowAsync(tableName: "employee", whereConditions: new Dictionary<string, dynamic>
            {
                { "first_name", "john" }
            }, columnValuePairs: new Dictionary<string, dynamic>
            {
                {"last_name", "peter" }
            });
        }
    }
}