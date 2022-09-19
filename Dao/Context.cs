using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CUGOJ.Admin_Server.Dao;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Core> Cores { get; set; } = null!;
    public DbSet<Auth> Auths { get; set; } = null!;
    public DbSet<SysInfo> SysInfos { get; set; } = null!;
    public DbSet<Host> Hosts { get; set; } = null!;
    public string DbPath { get; }
    public Context()
    {
        DbPath = "./data/data.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }


    public static void CreateTable()
    {
        using var context = new Context();
        context.Database.EnsureCreated();
    }

}

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public int Role { get; set; } = 0;
    public int Status { get; set; } = 1;

    public string Token { get; set; } = null!;
}

public class Core
{
    public long Id { get; set; }
    public string Host { get; set; } = null!;
    public string IP { get; set; } = null!;
    public string Env { get; set; } = null!;
    public int Port { get; set; } = 0;
    public string LogPath { get; set; } = null!;
    public string TracePath { get; set; } = null!;
    public string MysqlPath { get; set; } = null!;
    public string RedisPath { get; set; } = null!;
    public string Neo4jPath { get; set; } = null!;
    public string RabbitmqPath { get; set; } = null!;
    public int Status { get; set; } = 1;
    public string Branch { get; set; } = null!;
}

public class Auth
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long CoreId { get; set; }
    public int Status { get; set; } = 1;
}

[Index(nameof(Env))]
public class SysInfo
{
    public long Id { get; set; }
    public string Env { get; set; } = string.Empty;
    public string LogPath { get; set; } = null!;
    public string TracePath { get; set; } = null!;
    public string MysqlPath { get; set; } = null!;
    public string RedisPath { get; set; } = null!;
    public string Neo4jPath { get; set; } = null!;
    public string RabbitmqPath { get; set; } = null!;
}

[Index(nameof(Name))]
public class Host
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string HostIP { get; set; } = null!;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}