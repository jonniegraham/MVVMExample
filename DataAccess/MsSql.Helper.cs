using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess
{
    /// <summary>
    /// Helper functions and fields for building queries.
    /// </summary>
    public partial class MsSql
    {
        private readonly Dictionary<Type, SqlDbType> _dbTypes = new Dictionary<Type, SqlDbType>()
        {
            {typeof(string), SqlDbType.NVarChar },
            {typeof(int), SqlDbType.Int }
        };

        /// <summary>
        /// Constructs MS SQL Server connection string from settings in the LinkTekTest.exe.config file.
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                var appSettings = new Utils.AppSettings();
                try
                {
                    return $"Data Source={appSettings.GetSetting("Data Source")};" +
                        $"Initial Catalog={appSettings.GetSetting("Initial Catalog")};" +
                        $"Integrated Security={appSettings.GetSetting("Integrated Security")};" +
                        $"User Id={appSettings.GetSetting("User Id")};" +
                        $"Password={appSettings.GetSetting("Password")};Connect Timeout=0";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Constructs 'WHERE clause' from column_name-value pairs, i.e. " WHERE attr1=@attr1 AND attr2=@attr2 AND ...".
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columnValuePairs">Column-value pairs describing WHERE condtions</param>
        private static void AppendWhereClause(ref string query, Dictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                return;

            query += " WHERE " + columnValuePairs.First().Key + "=@" + columnValuePairs.First().Key;
            query = columnValuePairs.Skip(1).Aggregate(query, (current, columnValuePair) => current + (" AND " + columnValuePair.Key + "=@" + columnValuePair.Key));
        }

        /// <summary>
        /// Constructs 'SET clause' from column_name-value pairs, i.e. " SET attr1=@attr1, attr2=@attr2, ...".
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columnValuePairs">Column names and their values</param>
        private void AppendSetStatement(ref string query, Dictionary<string, object> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                throw new ArgumentException("columnValuePairs cannot be null or empty.");

            query += " SET " + columnValuePairs.First().Key + "=@" + columnValuePairs.First().Key;
            query = columnValuePairs.Skip(1).Aggregate(query, (current, columnValuePair) => current + (", " + columnValuePair.Key + "=@" + columnValuePair.Key));
        }

        /// <summary>
        /// Adds parameter-values to the SqlCommand.
        /// These values will replace the value-placeholders in the query string.
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <param name="columnValuePairs">Column names and their values</param>
        private void AddParameters(SqlCommand command, Dictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null)
                return;

            foreach (var columnValuePair in columnValuePairs)
            {
                command.Parameters.Add($"@{columnValuePair.Key}", GetDbType(columnValuePair.Value));
                command.Parameters[$"@{columnValuePair.Key}"].Value = columnValuePair.Value;
            }
        }

        /// <summary>
        /// Get SqlDbType corresponding to C# data type
        /// </summary>
        /// <param name="value">Value of whose C# type to inspect</param>
        /// <returns>SqlDbType corresponding to C# type</returns>
        private SqlDbType GetDbType(dynamic value)
        {
            Type type = value.GetType();
            if (!_dbTypes.ContainsKey(type))
                throw new ArgumentException($"C# type '{type}' is not mapped to any SqlDbType.");
            return _dbTypes[type];
        }

        /// <summary>
        /// Creates a comma-separated list of column names, i.e. "attr1, attr2, ..."
        /// </summary>
        /// <param name="columnValuePairs">Column names and their values</param>
        /// <returns>Comma-separated list of column names</returns>
        private static string ListColumnNames(Dictionary<string, dynamic> columnValuePairs)
        {
            if (columnValuePairs == null || columnValuePairs.Count == 0)
                throw new ArgumentException("columnValuePairs cannot be null or empty.");

            var listedColumnNames = "";
            listedColumnNames += columnValuePairs.First().Key;
            return columnValuePairs.Skip(1).Aggregate(listedColumnNames, (current, columnValuePair) => current + (", " + columnValuePair.Key));
        }

        /// <summary>
        /// Creates a comma-separated list of value-placeholders, i.e. "@attr1, @attr2, ..."
        /// </summary>
        /// <param name="columnValuePairs">Column names and their values</param>
        /// <returns>Comma-separated list of value-placeholders</returns>
        private static string ListValuePlaceholders(Dictionary<string, dynamic> columnValuePairs)
        {
            return "@" + Utils.Replace(@",\s*", ", @", ListColumnNames(columnValuePairs));
        }
    }
}
