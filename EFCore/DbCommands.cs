using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore
{
    public class DbCommands
    {
        // 초기화 시간이 오래 걸릴 수도 있음.
        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                CreateTestData(db);
                Console.WriteLine("Db Initialized");
            }
        }

        public static void CreateTestData(AppDbContext db)
        {
            var player = new Player()
            {
                Name = "Junpil"
            };

            List<Item> items = new List<Item>()
            { 
                new Item()
                {
                    TemplateId = 101,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = new Player () {Name = "Munyoung"},
                }
            };
            // 하나만 넣을 경우 db.items.add(item);
            db.Items.AddRange(items);
            db.SaveChanges();
        }

        public static void ReadAll()
        {
            // AsNoTracking : ReadOnly의 의미 "읽기만 할 것이다" << Tracking Snapshot (이라고 db를 감시해서 변화가있는지 탐지하는 기능)을 무시하고 읽기만 한다!
            // include : Eager Loading (즉시 로딩) << 나중에 알아볼 것 TODO
            using (var db = new AppDbContext())
            {
                foreach (Item item in db.Items.AsNoTracking().Include(i => i.Owner)) // owner도 같이 로딩해주세요!
                {
                    Console.WriteLine($"TemplateId : {item.TemplateId} Owner : {item.Owner.Name} Created : {item.CreateDate}");
                }
            }
        }

        // 특정 플레이어가 소지한 아이템들의 CreateDate를 수정
        public static void UpdateDate()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                            .Where(i => i.Owner.Name == name);

                foreach (Item item in items)
                {
                    item.CreateDate = DateTime.Now;
                }

                db.SaveChanges();
            }

            ReadAll();
        }

        public static void DeleteItem()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                            .Where(i => i.Owner.Name == name);

                db.Items.RemoveRange(items);

                db.SaveChanges();
            }

            ReadAll();
        }
    }
}
