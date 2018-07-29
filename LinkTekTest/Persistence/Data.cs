using DataAccess;

namespace LinkTekTest.Persistence
{
    /// <summary>
    /// Provides singetons that interact with the data source.
    /// </summary>
    internal class Data
    {
        private static IDatabaseService _databaseService;
        private static Employees _employees;

        /// <summary>
        /// Gets the Employees singleton that interacts with employee data in the database.
        /// </summary>
        /// <returns>Employees singleton</returns>
        public static Employees Employees()
        {
            if (_employees != null)
                return _employees;

            InitialiseDatabase();

            return _employees ?? (_employees = new Employees(_databaseService));
        }

        private static void InitialiseDatabase()
        {
            if (_databaseService == null)
                _databaseService = new MsSql();
        }
    }
}
