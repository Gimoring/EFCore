﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore
{
    public static class Extensions
    {
        // IEnumerable (LINQ to object / LINQ to XML 쿼리)
        // IQueryable (LINQ to SQL 쿼리)
        public static IQueryable<GuildDto> MapGuildToDto(this IQueryable<Guild> guild)
        {
            return guild.Select(g => new GuildDto()
            {
                Name = g.GuildName,
                MemberCount = g.Members.Count()
            });
        }
    }
}