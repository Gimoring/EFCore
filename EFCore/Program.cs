using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace EFCore
{
    public class Program
    {
        // 초기화 시간이 오래 걸릴 수도 있음.
        static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                Console.WriteLine("Db Initialized");
            }

            
        }
        static void Main(string[] args)
        {
            InitializeDB(forceReset: true);
        }
    }
}

