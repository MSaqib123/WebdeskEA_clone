using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebdeskEA.Models.ExternalModel;
using WebdeskEA.Models.DbModel;
using WebdeskEA.Models.BaseEntites;
using WebdeskEA.Models.MappingModel;

namespace WebdeskEA.DataAccess;

public partial class WebdeskEADBContext : IdentityDbContext<IdentityUser>//DbContext
{
    public WebdeskEADBContext()
    {
    }

    public WebdeskEADBContext(DbContextOptions<WebdeskEADBContext> options)
        : base(options)
    {
    }
    public DbSet<Customer> Customers{ get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<FinancialYear> financialYears { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<PkgType> PackageTypes { get; set; }
    public DbSet<PackagePermission> PackagePermissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Country> Countries{ get; set; }
    public DbSet<StateProvince> StateProvinces{ get; set; }
    public DbSet<City> Cities{ get; set; }
    public DbSet<TenantPermission> TenantPermissions{ get; set; }
    public DbSet<TenantType> TenantTypes { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<UserRight> UserRights { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public virtual DbSet<BusinessCategory> BusinessCategories { get; set; }
    public virtual DbSet<Coa> Coas { get; set; }
    public virtual DbSet<Coatype> Coatypes { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Brand> Brands { get; set; }
    
//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=SAQIB\\MSSQLSERVER01;Initial Catalog=WebdeskEA_20240706;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
