using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore
{

    // DB 관계 모델링
    // 1:1
    // 1:다
    // 다:다

    // 데이터 모델링 
    // - 'Entity 클래스' 이름 + Id 을 붙이는게 기본 컨벤션이다 
    // -- ex) class Player   + Player (Id) 
    // - 클래스 이름 = 테이블 이름 => Player
    // 1:다 관계에서 '다'인 쪽에서 FK를 가지고 있는데 어떤 플레이어에 소속되어있는지 FK로 참조한다.
    [Table("Item")]
    public class Item
    {
        public int ItemId { get; set; } // DB에서 관리하는 고유 Id (PK)
        public int TemplateId { get; set; } // 따로 데이터시트에 관리하는 Id ex) 집행검Id => 101번 
        public DateTime CreateDate { get; set; }

        // 다른 클래스 참조 => FK 로 관리 => 링크를 걸어서 다른 애를 참조하는 것을 Navigation property 라고 함.
        // public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public Player Owner { get; set; }
    }
    
    // 플레이어 n 아이템 n
    [Table("Player")]
    public class Player
    {
        // class이름 + id => PK 
        public int PlayerId { get; set; }
        public string Name { get; set; }

        public Item Item { get; set; }
        public Guild Guild { get; set; }
    }

    // 길드 n 플레이어 m
    [Table("Guild")]
    public class Guild
    {
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Members { get; set; }
    }

    // DTO (Data Transfer Object)
    // Table로서 존재하는 것이 아니라 데이터를 추출할 때 중간 단계에서 사용하는 클래스임
    // -역할 : DB에 접근하는 애를 컨텐츠에 넘기기전에 재가공
    public class GuildDto
    {
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }

}
