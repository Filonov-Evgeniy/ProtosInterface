using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtosInterface.Extensions
{
    public static class DbSetExtensions
    {
        public static int GetLastId<T>(this DbSet<T> dbSet) where T : class
        {
            return dbSet
                .OrderByDescending(x => EF.Property<int>(x, "Id"))
                .Select(x => EF.Property<int>(x, "Id"))
                .FirstOrDefault();
        }
    }
}
