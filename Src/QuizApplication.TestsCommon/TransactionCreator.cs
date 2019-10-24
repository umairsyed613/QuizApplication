using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using QuizApplication.Database;

namespace QuizApplication.TestsCommon
{
    public static class TransactionCreator
    {
        public static IDbWithTransactionFactory NoCommitFactory()
        {
            return new NoCommitDbFactory();
        }

        public static IDbWithTransactionFactory Clone(this IDbWithTransactionConnection conn)
        {
            return new FactoryThatReturnsExistingConnection(conn);
        }

        public class NoCommitDbFactory : IDbWithTransactionFactory
        {
            public async Task<IDbWithTransactionConnection> Create()
            {
                var connection = InitializeDb("DataSource=:memory:");
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                Debug.Print("Creating No Commit Factory");
                return new NoCommitConnection(connection, transaction, true);
            }

            private SqliteConnection InitializeDb(string connectionString)
            {
                if (!System.IO.File.Exists("001-CreateTables.sql")) { return null; }

                var data = File.ReadAllText("001-CreateTables.sql");

                if (string.IsNullOrWhiteSpace(data))
                {
                    return null;
                }

                var connection = new SqliteConnection(connectionString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = data;
                command.ExecuteNonQuery();

                return connection;
            }
        }

        public class NoCommitConnection : IDbWithTransactionConnection
        {
            private readonly bool _doDispose;

            public NoCommitConnection(SqliteConnection connection, SqliteTransaction transaction, bool doDispose)
            {
                Connection = connection ?? throw new ArgumentNullException(nameof(connection));
                Transaction = transaction;
                _doDispose = doDispose;
            }

            public SqliteConnection Connection { get; }
            public SqliteTransaction Transaction { get; }

            private static object _fac = null;

            public IDbFactory<T> FactoryFor<T>() where T : CommonDbContext
            {
                if (_fac == null)
                {
                    Debug.Print("Creating DbContext");
                    _fac = new DbFactory<T>(Connection, Transaction);
                }

                return (IDbFactory<T>) _fac;
            }

            public void Dispose()
            {
                if (!_doDispose) { return; }

                Connection.Dispose();
                Transaction?.Dispose();
                _fac = null;
            }

            public void Commit()
            {
                Debug.Print("Commiting No Commit Factory");
                // Don't do anything here
            }
        }

        public class FactoryThatReturnsExistingConnection : IDbWithTransactionFactory
        {
            private readonly IDbWithTransactionConnection _existingConnection;

            public FactoryThatReturnsExistingConnection(IDbWithTransactionConnection conn)
            {
                _existingConnection = conn;
            }

            public Task<IDbWithTransactionConnection> Create()
            {
                var t = new NoCommitConnection(_existingConnection.Connection, _existingConnection.Transaction, false);
                return Task.FromResult<IDbWithTransactionConnection>(t);
            }
        }
    }
}
