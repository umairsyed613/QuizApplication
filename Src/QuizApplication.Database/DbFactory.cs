using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace QuizApplication.Database
{
    public class DbFactory<T> : IDbFactory<T> where T : CommonDbContext
    {
        private readonly string _connectionString;
        private readonly DbConnection _outerConnection;
        private readonly DbTransaction _transaction;

        public DbFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbFactory(DbConnection connection, DbTransaction transaction)
        {
            _outerConnection = connection ?? throw new ArgumentNullException(nameof(connection));
            _transaction = transaction;
        }

        public T GetReadOnly()
        {
            var context = GetReadWrite();
            context.ReadOnlyMode = true;
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.LazyLoadingEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        public T GetReadWrite()
        {
            var options = new DbContextOptionsBuilder<T>();

            options = _connectionString != null ? options.UseSqlite(_connectionString) : options.UseSqlite(_outerConnection);

            var conn = (T) Activator.CreateInstance(typeof(T), options.Options);

            if (_transaction != null) { conn.Database.UseTransaction(_transaction); }

            conn.ChangeTracker.AutoDetectChangesEnabled = true;

            return conn;
        }
    }

    public interface IDbWithTransactionFactory
    {
        Task<IDbWithTransactionConnection> Create();
    }

    public class DbWithTransactionFactory : IDbWithTransactionFactory
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _dbConnection;

        public DbWithTransactionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbWithTransactionFactory(SqliteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IDbWithTransactionConnection> Create()
        {
            var connection = _connectionString != null ? new SqliteConnection(_connectionString) : _dbConnection;
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
            return new DbWithTransactionConnection(connection, transaction);
        }
    }

    public interface IDbWithTransactionConnection : IDisposable
    {
        IDbFactory<T> FactoryFor<T>() where T : CommonDbContext;
        void Commit();

        SqliteConnection Connection { get; }
        SqliteTransaction Transaction { get; }
    }

    public sealed class DbWithTransactionConnection : IDbWithTransactionConnection
    {
        internal DbWithTransactionConnection(SqliteConnection connection, SqliteTransaction transaction)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public SqliteConnection Connection { get; }
        public SqliteTransaction Transaction { get; }

        public IDbFactory<T> FactoryFor<T>() where T : CommonDbContext
        {
            return new DbFactory<T>(Connection, Transaction);
        }

        public void Dispose()
        {
            Transaction.Dispose();
            Connection.Dispose();
        }

        public void Commit() => Transaction.Commit();
    }
}
