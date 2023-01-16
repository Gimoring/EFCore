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


        // Update 3단계
        // 1) Tracked Entity를 얻어 온다.
        // 2) Entity 클래스의 property를 변경 (set)
        // 3) SaveChanges 호출

        // (Connected vs Disconnected) Update
        // Disconnected : Update 단계가 한 번에 쭉 일어나지 않고, 끊기는 경우
        // (REST API 등) ex) 먼저 데이터를 불러와서 보여주고 나~중에 업데이트 하는 식
        // 처리하는 2가지 방법
        // 1) Reload 방식. 필요한 정보만 보내서, 1-2-3 Step
        // 2) Full Update 방식. 모든 정보를 다 보내고 받아서, 아예 Entity를 다시 만들고 통으로 Update

        public static void ShowGuilds()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.MapGuildToDto())
                {
                    Console.WriteLine($" guild id : {guild.GuildId}, guild name : {guild.Name}, guild mCount : {guild.MemberCount}");
                }
            }
        }
        // 장점 : 최소한의 정보로 Update 가능 (일반적으로 리로드 방식 사용)
        // 단점 : Read 두 번 한다.
        public static void UpdateByReload()
        {
            ShowGuilds();

            // 외부(클라) 에서 수정 원하는 데이터의 ID / 정보 넘겨줬다고 가정
            Console.WriteLine("Input GuildId");
            Console.WriteLine("> ");
            int id = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Input GuildName");
            Console.WriteLine("> ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                Guild guild = db.Find<Guild>(id);
                guild.GuildName = name;

                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ----");
            ShowGuilds();
        }

        public static string MakeUpdateJsonStr()
        {
            var jsonStr = "(\"GuildId\":1, \"GuildName\":\"Hello\", \"Members\":null)";
            return jsonStr;
        }

        // 장점 : DB에 다시 Read할 필요 없이 바로 Update
        // 단점 : 모든 정보 필요, 보안 문제 (상대를 신용할 때 사용)
        public static void UpdateByFull()
        {
            ShowGuilds();
            string jsonStr = MakeUpdateJsonStr();
            Guild guild = JsonConvert.DeserializeObject<Guild>(jsonStr);

            using (AppDbContext db = new AppDbContext())
            {
                db.Guilds.Update(guild);
                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ----");
            ShowGuilds();
        }


    }
}
