using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore
{
    // EF Core 작동 순서
    // 1) DbContext 만들 때
    // 2) DbSet<T> 를 찾는다.
    // 3) 모델링 class를 분석해서, 컬럼을 찾는다.
    // 4) 모델링 class에서 참조하는 다른 class가 있으면, 걔도 분석.
    // 5) OnModelCreating 함수 호출 (추가 설정 = override)
    // 6) 최종 : 데이터베이스의 전체 모델링 구조를 내부 메모리에 들고 있다.
    public class AppDbContext : DbContext
    {
        // 사용할 테이블 연결
        // - 테이블은 Item이란 이름으로 되어있고 DataModel에 있는 Item 클래스를 참고하라.
        // DbSet<Item> -> EF Core에게 알려준다
        // - Item이라는 DB Table 이 있는데, 세부적인 칼럼/키 정보는 Item 클래스를 참고하라.
        public DbSet<Item> Items { get; set; }
        public DbSet<EventItem> EventItems { get; set; }

        // 간접적으로 접근하는 테이블은 굳이 만들어주지 않아도 알아서 EF Core에게 알려준다. => player 만들 필요 없음.
        public DbSet<Player> Players { get; set; }
        public DbSet<Guild> Guilds { get; set; }

        // DB ConnectionString
        // 어떤 DB를 어떻게 연결해라 (각종 설정, authorization 등)
        // 보통 외부 파일에 빼가지고 프로그램이 실행될 때 로드해서 붙는다 (보안 문제)
        public const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EfCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 앞으로 Item Entity에 접근할 때 항상 사용되는 모델 레벨(데이터모델)의 필터링
            // 필터를 무시하고 싶으면 IngnoreQueryFilters 옵션 추가
            builder.Entity<Item>().HasQueryFilter(i => i.SoftDeleted == false);

            builder.Entity<Player>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("Index_Person_Name")
                .IsUnique();
        }
    }
}
