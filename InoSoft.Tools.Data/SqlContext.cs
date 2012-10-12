﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace InoSoft.Tools.Data
{
    /// <summary>
    /// Context for executing SQL queries.
    /// </summary>
    /// <remarks>
    /// Works synchronously, but in separate thread. Only one thread and one execution queue are created,
    /// so single context can operate only single SQL query at the same time. For simultaneous access
    /// miltiple contexts must be used. Note that some versions of MSSQL can't operate queries in parallel.
    /// In this case using single context for the whole application is recommended.
    /// This context is asynchronous wrapper of EF 4.1 DbContext and its best advantage is that it works in single
    /// thread, as mentioned above. This technique solves problem when EntityConnection reconnects each time
    /// it's used in a different thread.
    /// </remarks>
    public class SqlContext : AsyncProcessor<SqlBatch>, ISqlContext, IDisposable
    {
        private readonly SqlConnection _sqlConnection;

        /// <summary>
        /// Creates SqlContext.
        /// </summary>
        /// <param name="connectionString">SQL connection string, which context will use.</param>
        public SqlContext(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlConnection.Open();
            Start();
        }

        /// <summary>
        /// Executes SQL command, which returns array of elements.
        /// </summary>
        /// <param name="elementType">Type of elements or null if we don't need to return query result.</param>
        /// <param name="sql">SQL query string.</param>
        /// <param name="parameters">Optional named parameters.</param>
        public Array Execute(Type elementType, string sql, params object[] parameters)
        {
            // Create encapsulated query and push it into queue.
            var query = new SqlQuery
            {
                ElementType = elementType,
                Sql = sql,
                Parameters = parameters
            };
            var batch = new SqlBatch { Queries = new[] { query } };
            EnqueueItem(batch);

            // Wait until query is executed.
            batch.WaitSignal();

            if (query.Exception != null)
            {
                // Rethrow the exception if it occured.
                throw query.Exception;
            }

            return query.Result;
        }

        /// <summary>
        /// Executes SQL query, which returns nothing.
        /// </summary>
        /// <param name="sql">SQL query string.</param>
        /// <param name="parameters">Optional named parameters.</param>
        public void Execute(string sql, params object[] parameters)
        {
            Execute(null, sql, parameters);
        }

        /// <summary>
        /// Executes SQL command, which returns array of elements.
        /// </summary>
        /// <param name="sql">SQL query string.</param>
        /// <param name="parameters">Optional named parameters.</param>
        public T[] Execute<T>(string sql, params object[] parameters)
        {
            return Execute(typeof(T), sql, parameters).Cast<T>().ToArray();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _sqlConnection.Dispose();
        }

        /// <summary>
        /// Processes batches of SQL queries from queue.
        /// </summary>
        /// <param name="item">Encapsulated batch of queries.</param>
        protected override void ProcessItem(SqlBatch item)
        {
            foreach (var query in item.Queries)
            {
                try
                {
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        command.CommandText = query.Sql;
                        command.Parameters.AddRange(query.Parameters);

                        if (query.ElementType != null)
                        {
                            using (var reader = command.ExecuteReader(CommandBehavior.KeyInfo))
                            {
                                var properties = new List<PropertyInfo>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var prop = query.ElementType.GetProperty(reader.GetName(i));
                                    properties.Add(prop);
                                }

                                var result = new ArrayList();
                                if (SqlTypeHelper.IsSqlType(query.ElementType))
                                {
                                    while (reader.Read())
                                    {
                                        var sqlValue = reader.GetValue(0);
                                        result.Add(sqlValue == DBNull.Value ? null : sqlValue);
                                    }
                                }
                                else
                                {
                                    while (reader.Read())
                                    {
                                        var resultItem = Activator.CreateInstance(query.ElementType);
                                        for (int i = 0; i < properties.Count; i++)
                                        {
                                            if (properties[i] != null)
                                            {
                                                var sqlValue = reader.GetValue(i);
                                                properties[i].SetValue(resultItem, sqlValue == DBNull.Value ? null : sqlValue, null);
                                            }
                                        }
                                        result.Add(resultItem);
                                    }
                                }

                                query.Result = result.ToArray(query.ElementType);
                            }
                        }
                        else
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    query.Exception = ex;
                    break;
                }
            }
            item.Signal();
        }
    }
}