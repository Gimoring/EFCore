using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
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

        // 이미 존재하는 사용자를 연동하려면?
        // 1) Tracked Instance (추적되고 있는 객체)를 얻어와서
        // 2) 데이터 연결

        public static void CreateTestData(AppDbContext db)
        {
            var junpil = new Player() { Name = "Junpil" };
            var munyoung = new Player() { Name = "Munyoung" };
            var yujin = new Player() { Name = "Yujin" };

            // 1) Detached
            Console.WriteLine(db.Entry(yujin).State);

            List<Item> items = new List<Item>()
            { 
                new Item()
                {
                    TemplateId = 101,
                    CreateDate = DateTime.Now,
                    Owner = junpil
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = yujin
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = munyoung
                }
            };

            Guild guild = new Guild()
            {
                GuildName = "wrsoft",
                Members = new List<Player>() { munyoung, yujin, junpil }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);
            db.SaveChanges();

        }

        public static void ShowItems()
        {
            using (AppDbContext db = new AppDbContext())
            {
                // 1) 혹시라도 설정에서 삭제 필터링된 item들을 보고 싶다면
                // foreach (var item in db.Items.Include(i => i.Owner).IgnoreQueryFilters().ToList())
                foreach (var item in db.Items.Include(i => i.Owner).ToList())
                {
                    // 2)
                    // if (item.softdeleted) { ... } else { ...}

                    if (item.Owner == null)
                        Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                    else
                        Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) OwnerId({item.Owner.PlayerId} Owner({item.Owner.Name}))");
                }
            }
        }

        public static void ShowGuild()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.Include(g=> g.Members).ToList())
                {
                    Console.WriteLine($"GuildId({guild.GuildId}) GuildName({guild.GuildName}) MemberCount({guild.Members.Count})");
                }
            }
        }


    }
}
