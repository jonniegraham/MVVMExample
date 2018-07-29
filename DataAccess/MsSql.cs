using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class MsSql : IDatabaseService
    {
        /// <summary>
        /// Select Row(s) from a database table.
        /// </summary>
        /// <param name="tableName">Name of database table</param>
        /// <param name="whereConditions">Column-value pairs describing WHERE condtions</param>
        /// <returns>List of rows</returns>
        public Task<List<Dictionary<string, dynamic>>> SelectRowsAsync(string tableName, Dictionary<string, dynamic> whereConditions)
        {
            var query = $"SELECT * FROM {tableName}";
            AppendWhereClause(ref query, whereConditions);
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    AddParameters(command, whereConditions);
                    sqlConnection.Open();
                    var reader = command.ExecuteReader();
                    var rows = new List<Dictionary<string, dynamic>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, dynamic>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        rows.Add(row);
                    }
                    return Task.FromResult(rows);
                }
            }
            catch (SqlException sqlException)
            {
                HandleSqlException(sqlException);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Insert a database row.
        /// </summary>
        /// <param name="tableName">Name of database table</param>
        /// <param name="columnValuePairs">New values</param>
        /// <returns>Primary key of newly inserted row</returns>
        public async Task<dynamic> InsertRowAsync(string tableName, Dictionary<string, dynamic> columnValuePairs)
        {
            var query = $"INSERT INTO {tableName}({ListColumnNames(columnValuePairs)}) " +
                $"VALUES({ListValuePlaceholders(columnValuePairs)}) SELECT SCOPE_IDENTITY()";
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    AddParameters(command, columnValuePairs);
                    sqlConnection.Open();

                    return await command.ExecuteScalarAsync();
                }
            }
            catch (SqlException sqlException)
            {
                HandleSqlException(sqlException);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public Task<bool> ExistsAsync(string tableName, Dictionary<string, dynamic> whereConditions)
        {
            bool returnValue = false;

            var query = $"SELECT 1 FROM {tableName}";
            AppendWhereClause(ref query, whereConditions);
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    AddParameters(command, whereConditions);
                    sqlConnection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                        returnValue = true;
                }
            }
            catch(SqlException sqlException)
            {
                HandleSqlException(sqlException);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// Update a database row.
        /// </summary>
        /// <param name="tableName">Name of database table</param>
        /// <param name="columnValuePairs">New values</param>
        /// <param name="whereConditions">Column_name-value pairs describing the row(s) to update</param>
        /// <returns></returns>
        public Task UpdateRowAsync(string tableName, Dictionary<string, dynamic> columnValuePairs,
            Dictionary<string, dynamic> whereConditions)
        {
            var query = $"UPDATE {tableName}";
            AppendSetStatement(ref query, columnValuePairs);
            AppendWhereClause(ref query, whereConditions);
            try
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    AddParameters(command, columnValuePairs);
                    AddParameters(command, whereConditions);
                    sqlConnection.Open();
                    command.ExecuteScalar();
                }
            }
            catch (SqlException sqlException)
            {
                HandleSqlException(sqlException);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.CompletedTask;
        }

        private void HandleSqlException(SqlException sqlException)
        {
            if (sqlException.Number == 134)
                throw new ArgumentException("(SQL Exception #134) Cannot update a field that is used in the WHERE clause.");
            if (sqlException.Number == 207)
                throw new ArgumentException("SQL Exception #207: Invalid column table name.");
            if (sqlException.Number == 208)
                throw new ArgumentException("SQL Exception #208: Invalid database table name.");

            Console.Write($"SQL Exception #{sqlException.Number}: {sqlException.Message}");
        }
    }
}