using System;

namespace QuizApplication.Database
{
    public interface IDbFactory<out T> where T : CommonDbContext
    {
        T GetReadWrite();
        T GetReadOnly();
    }
}
