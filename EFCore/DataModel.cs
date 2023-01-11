using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore
{
    // 데이터 모델링 
    // - 클래스 이름 + Id 을 붙이는게 기본 컨벤션이다 
    // -- ex) class Player   + Player (Id) 
    // - 클래스 이름 = 테이블 이름 => Player
    [Table("Item")]
    public class Item
    {
        public int ItemId { get; set; } // DB에서 관리하는 고유 Id (PK)
        public int TemplateId { get; set; } // 따로 데이터시트에 관리하는 Id ex) 집행검Id => 101번 
        public DateTime CreateDate { get; set; }

        // 다른 클래스 참조 => FK 로 관리 => 링크를 걸어서 다른 애를 참조하는 것을 Navigation property 라고 함.
        public int OwnerId { get; set; }
        public Player Owner { get; set; }

    }
    public class Player
    {
        // class이름 + id => PK 
        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}
