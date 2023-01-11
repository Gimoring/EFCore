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
            var junpil = new Player() { Name = "Junpil" };
            var munyoung = new Player() { Name = "Munyoung" };
            var yujin = new Player() { Name = "Yujin" };

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

            // 하나만 넣을 경우 db.items.add(item);
            db.Items.AddRange(items);
            db.Guilds.Add(guild);
            db.SaveChanges();
        }

        // 1 + 2) 특정 길드에 있는 길드원들이 소지한 모든 아이템들을 보고 싶다!
        // 장점 : DB 접근 한 번으로 다 로딩 (JOIN)
        // 단점 : 다 필요한지 모르겠는데도 다 가져옴.
        public static void EagerLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write("> ");
            string name = Console.ReadLine();

            // AsNoTracking : ReadOnly의 의미 "읽기만 할 것이다" << Tracking Snapshot (이라고 db를 감시해서 변화가있는지 탐지하는 기능)을 무시하고 읽기만 한다!
            // include : Eager Loading (즉시 로딩) << 나중에 알아볼 것 TODO
            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds.AsNoTracking()
                    .Where(g => g.GuildName == name)
                    .Include(g => g.Members) // 그냥 Include는 1차원적. 여기서 g.Members만을 불러오는데 만약 player의 아이템까지 보고 싶다면 ThenInclude를 통해 접근 가능하다 (2차원적 접근)
                        .ThenInclude(p => p.Item)
                    .First(); //SELECT TOP 1

                foreach (Player player in guild.Members) // owner도 같이 로딩해주세요!
                {
                    Console.WriteLine($"ItemId : {player.Item.TemplateId},  Owner : {player.Name}");
                }
            }
        }

        // 장점 : 필요한 시점에 필요한 것만 로딩 가능
        // 단점 : DB 접근을 여러 번 해서 접근 비용이 비싸진다.
        public static void ExplicitLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                // Explicit Loading을 할 때에는 AsNoTracking을 붙여주면 안된다. => 에러
                Guild guild = db.Guilds
                    .Where(g => g.GuildName == name)
                    .First();

                // 명시적
                db.Entry(guild).Collection(g => g.Members).Load(); // guild 에 가서 g의 멤버들을 로드 해주세요!

                foreach (Player player in guild.Members)
                {
                    db.Entry(player).Reference(p => p.Item).Load(); // player의 item도 로드 해주세요!
                }

                foreach (Player player in guild.Members)
                {
                    Console.WriteLine($"TemplateId : {player.Item.TemplateId} Owner {player.Name}");
                }
            }
        }

        // 3) 특정 길드에 있는 길드원 수는?
        // 장점 : 필요한 정보만 쏘옥~ 빼서 로딩
        // 단점 : 일일히 select 안에 만들어줘야 함
        public static void SelectLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var info = db.Guilds
                    .Where(g => g.GuildName == name)
                    .MapGuildToDto()
                    .First();

                Console.WriteLine($"GuildName {info.Name}, MemberCount {info.MemberCount}");
            }
        }
    }
}
