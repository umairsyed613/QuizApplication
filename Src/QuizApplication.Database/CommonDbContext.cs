using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuizApplication.Database
{
    public class CommonDbContext : DbContext
    {
        private bool _readOnlyMode;

        public bool ReadOnlyMode
        {
            get => _readOnlyMode;
            set
            {
                if (_readOnlyMode && !value) { throw new InvalidOperationException("Can't make readonly db context writable"); }

                _readOnlyMode = value;
            }
        }

        public CommonDbContext(DbContextOptions options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            FailIfReadOnly();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            FailIfReadOnly();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void FailIfReadOnly()
        {
            if (ReadOnlyMode) { throw new InvalidOperationException("Don't save readonly db context"); }
        }
    }
}
