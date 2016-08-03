using Kitty.Db;
using System;
using System.Data;
using System.Data.Common;
#if DEBUG
using System.Diagnostics;
#endif
using System.Threading.Tasks;

namespace Kitty.Core.Browser.History
{
    public class FirefoxHistoryCollector : IBrowserHistoryCollector
    {
        private IConnectionProvider inputProvider;
        private IConnectionProvider outputProvider;

        public FirefoxHistoryCollector(IConnectionProvider inputProvider, IConnectionProvider outputProvider)
        {
            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;
        }

        #region places

        private void CreatePlaceTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE places (url LONGVARCHAR, visit_count INTEGER DEFAULT 0)";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectPlaceTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT url, visit_count FROM moz_places";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreatePlaceTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO places(url, visit_count) VALUES(@url,@visit_count)";

                        var urlParameter = outputCommand.CreateParameter();
                        urlParameter.ParameterName = "url";
                        urlParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(urlParameter);
                        var visitCountParameter = outputCommand.CreateParameter();
                        visitCountParameter.ParameterName = "visit_count";
                        visitCountParameter.DbType = DbType.Int32;
                        outputCommand.Parameters.Add(visitCountParameter);

                        while (reader.Read())
                        {
                            string url = reader.GetString(0);
                            int visitCount = reader.GetInt32(1);

                            urlParameter.Value = url;
                            visitCountParameter.Value = visitCount;
                            outputCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        #region searchs

        private void CreateSearchTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE searchs (input LONGVARCHAR NOT NULL)";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectInputHistoryTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT input FROM moz_inputhistory";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreateSearchTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO searchs(input) VALUES(@input)";

                        var inputParameter = outputCommand.CreateParameter();
                        inputParameter.ParameterName = "input";
                        inputParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(inputParameter);

                        while (reader.Read())
                        {
                            string input = reader.GetString(0);

                            inputParameter.Value = input;
                            outputCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        public Task<bool> CollectAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var inputConnection = inputProvider.CreateConnection())
                    using (var outputConnection = outputProvider.CreateConnection())
                    {
                        inputConnection.Open();
                        outputConnection.Open();
                        CollectPlaceTable(inputConnection, outputConnection);
                        CollectInputHistoryTable(inputConnection, outputConnection);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                    return false;
                }
                return true;
            });
        }
    }
}
