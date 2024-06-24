﻿using Microsoft.EntityFrameworkCore;
using MimicAPI.Models;

namespace MimicAPI.Data
{
    public class Context : DbContext
    {
        //construtor padrão para todo contexto
        public Context(DbContextOptions<Context>options) : base(options)
        {
                
        }

        //propriedade - tabela
        public DbSet<palavra> Palavras { get; set; }
    }
}
