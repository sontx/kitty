using System;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
#if DEBUG
using System.Diagnostics;
#endif

namespace Kitty.Db
{
    internal sealed class ChromeHistoryCollector : IBrowserHistoryCollector
    {
        private IConnectionProvider inputProvider;
        private IConnectionProvider outputProvider;

        public ChromeHistoryCollector(IConnectionProvider inputProvider, IConnectionProvider outputProvider)
        {
            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;
        }
        
        private DateTime GetFriendlyDateTime(long milliseconds)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            var time = posixTime.AddMilliseconds(milliseconds / 1000 - 11644473600000);
            return time;
        }

        #region downloads

        private void CreateDownloadTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE downloads (target_path LONGVARCHAR NOT NULL,total_bytes INTEGER NOT NULL,end_time DATETIME NOT NULL,referrer VARCHAR NOT NULL)";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectDownloadTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT target_path, end_time, total_bytes, referrer FROM downloads";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreateDownloadTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO downloads(target_path, end_time, total_bytes, referrer) VALUES(@target_path,@end_time,@total_bytes,@referrer)";

                        var targetPathParameter = outputCommand.CreateParameter();
                        targetPathParameter.ParameterName = "target_path";
                        targetPathParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(targetPathParameter);
                        var endTimeParameter = outputCommand.CreateParameter();
                        endTimeParameter.ParameterName = "end_time";
                        endTimeParameter.DbType = DbType.DateTime;
                        outputCommand.Parameters.Add(endTimeParameter);
                        var totalBytesParameter = outputCommand.CreateParameter();
                        totalBytesParameter.ParameterName = "total_bytes";
                        totalBytesParameter.DbType = DbType.Int64;
                        outputCommand.Parameters.Add(totalBytesParameter);
                        var referrerParameter = outputCommand.CreateParameter();
                        referrerParameter.ParameterName = "referrer";
                        referrerParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(referrerParameter);

                        while (reader.Read())
                        {
                            string targetPath = reader.GetString(0);
                            long endTime = reader.GetInt64(1);
                            long totalBytes = reader.GetInt64(2);
                            string referrer = reader.GetString(3);

                            targetPathParameter.Value = targetPath;
                            endTimeParameter.Value = GetFriendlyDateTime(endTime);
                            totalBytesParameter.Value = totalBytes;
                            referrerParameter.Value = referrer;
                            outputCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        #region downloads_url_chains

        private void CreateDownloadUrlChainsTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE downloads_url_chains (id INTEGER NOT NULL,chain_index INTEGER NOT NULL,url LONGVARCHAR NOT NULL, PRIMARY KEY (id, chain_index) )";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectDownloadUrlChainsTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT * FROM downloads_url_chains";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreateDownloadUrlChainsTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO downloads_url_chains VALUES(@id,@chain_index,@url)";

                        var idParameter = outputCommand.CreateParameter();
                        idParameter.ParameterName = "id";
                        idParameter.DbType = DbType.Int32;
                        outputCommand.Parameters.Add(idParameter);
                        var chainIndexParameter = outputCommand.CreateParameter();
                        chainIndexParameter.ParameterName = "chain_index";
                        chainIndexParameter.DbType = DbType.Int16;
                        outputCommand.Parameters.Add(chainIndexParameter);
                        var urlParameter = outputCommand.CreateParameter();
                        urlParameter.ParameterName = "url";
                        urlParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(urlParameter);

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int chainIndex = reader.GetInt32(1);
                            string url = reader.GetString(2);

                            idParameter.Value = id;
                            chainIndexParameter.Value = chainIndex;
                            urlParameter.Value = url;
                            outputCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        #region keywork_search

        private void CreateKeyworkSearchTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE keyword_search (term LONGVARCHAR NOT NULL)";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectKeyworkSearchChainsTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT term FROM keyword_search_terms";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreateKeyworkSearchTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO keyword_search VALUES(@term)";

                        var termParameter = outputCommand.CreateParameter();
                        termParameter.ParameterName = "term";
                        termParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(termParameter);

                        while (reader.Read())
                        {
                            string term = reader.GetString(0);
                            termParameter.Value = term;
                            outputCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        #endregion

        #region urls

        private void CreateUrlTable(DbConnection outputConnection)
        {
            using (var outputCommand = outputConnection.CreateCommand())
            {
                outputCommand.CommandText = "CREATE TABLE urls(url LONGVARCHAR,title LONGVARCHAR,visit_count INTEGER DEFAULT 0 NOT NULL,last_visit_time DATETIME NOT NULL)";
                outputCommand.ExecuteNonQuery();
            }
        }

        private void CollectUrlTable(DbConnection inputConnection, DbConnection outputConnection)
        {
            using (var inputCommand = inputConnection.CreateCommand())
            using (var transaction = outputConnection.BeginTransaction())
            {
                using (var outputCommand = outputConnection.CreateCommand())
                {
                    inputCommand.CommandText = "SELECT url, title, visit_count, last_visit_time FROM urls";
                    using (var reader = inputCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        CreateUrlTable(outputConnection);

                        outputCommand.CommandText = "INSERT INTO urls VALUES(@url, @title, @visit_count, @last_visit_time)";

                        var urlParameter = outputCommand.CreateParameter();
                        urlParameter.ParameterName = "url";
                        urlParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(urlParameter);
                        var titleParameter = outputCommand.CreateParameter();
                        titleParameter.ParameterName = "title";
                        titleParameter.DbType = DbType.String;
                        outputCommand.Parameters.Add(titleParameter);
                        var visitCountParameter = outputCommand.CreateParameter();
                        visitCountParameter.ParameterName = "visit_count";
                        visitCountParameter.DbType = DbType.Int32;
                        outputCommand.Parameters.Add(visitCountParameter);
                        var lastVisitTimeParameter = outputCommand.CreateParameter();
                        lastVisitTimeParameter.ParameterName = "last_visit_time";
                        lastVisitTimeParameter.DbType = DbType.DateTime;
                        outputCommand.Parameters.Add(lastVisitTimeParameter);

                        while (reader.Read())
                        {
                            string url = reader.GetString(0);
                            string title = reader.GetString(1);
                            int visitCount = reader.GetInt32(2);
                            long lastVisitTime = reader.GetInt64(3);

                            urlParameter.Value = url;
                            titleParameter.Value = title;
                            visitCountParameter.Value = visitCount;
                            lastVisitTimeParameter.Value = GetFriendlyDateTime(lastVisitTime);
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
                        CollectDownloadTable(inputConnection, outputConnection);
                        CollectDownloadUrlChainsTable(inputConnection, outputConnection);
                        CollectKeyworkSearchChainsTable(inputConnection, outputConnection);
                        CollectUrlTable(inputConnection, outputConnection);
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
