using _01_Data.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _01_Data.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    #region Tables
    public DbSet<T3IdentityUser> T3IdentityUser { get; set; } = default!;
    public DbSet<T3IdentityUserRole> T3IdentityUserRole { get; set; } = default!;
    public DbSet<T3IdentityRole> T3IdentityRole { get; set; } = default!;
    public DbSet<T3IdentityRoleHierarchy> T3IdentityRoleHierarchy { get; set; } = default!;
    public DbSet<T3IdentityClaim> T3IdentityClaim { get; set; } = default!;
    public DbSet<T3Module> T3Module { get; set; } = default!;
    public DbSet<T3ModuleHierarchy> T3ModuleHierarchy { get; set; } = default!;
    public DbSet<T3Item> T3Item { get; set; } = default!;
    public DbSet<T3ItemHierarchy> T3ItemHierarchy { get; set; } = default!;
    public DbSet<T3Location> T3Location { get; set; } = default!;
    public DbSet<T3LocationHierarchy> T3LocationHierarchy { get; set; } = default!;
    public DbSet<T3LocationItem> T3LocationItem { get; set; } = default!;
    public DbSet<T3ProcessType> T3ProcessType { get; set; } = default!;
    public DbSet<T3ProcessTypeItem> T3ProcessTypeItem { get; set; } = default!;
    public DbSet<T3ProcessTypeModule> T3ProcessTypeModule { get; set; } = default!;
    public DbSet<T3Template> T3Template { get; set; }
    public DbSet<T3TemplatePanel> T3TemplatePanel { get; set; }
    public DbSet<T3TemplateApprover> T3TemplateApprover { get; set; }
    public DbSet<T3Property> T3Property { get; set; }
    public DbSet<T3PropertyPanel> T3PropertyPanel { get; set; }
    public DbSet<T3PropertyTemplate> T3PropertyTemplate { get; set; }
    public DbSet<T3Form> T3Form { get; set; }
    public DbSet<T3FormField> T3FormField { get; set; }
    public DbSet<T3FormFieldValue> T3FormFieldValue { get; set; }
    public DbSet<T3FormResource> T3FormResource { get; set; }
    public DbSet<T3FormResourceItem> T3FormResourceItem { get; set; }
    public DbSet<T3ShiftType> T3ShiftType { get; set; }
    public DbSet<T3Protocol> T3Protocol { get; set; }
    public DbSet<T3ProtocolItem> T3ProtocolItem { get; set; }
    public DbSet<T3ShiftTypeBreak> T3ShiftTypeBreak { get; set; }
    public DbSet<T3ShiftTypeCategory> T3ShiftTypeCategory { get; set; }
    public DbSet<T3ShiftTypeDay> T3ShiftTypeDay { get; set; }
    public DbSet<T3ShiftTypeLocation> T3ShiftTypeLocation { get; set; }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
        base.OnConfiguring(optionsBuilder);
    } 
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in builder.Model.GetEntityTypes())
        {
            Microsoft.EntityFrameworkCore.Metadata.IMutableProperty? rowVersion = entityType.FindProperty("RowVersion");
            if (rowVersion != null)
            {
                rowVersion.IsConcurrencyToken = true;
            }

            Microsoft.EntityFrameworkCore.Metadata.IMutableProperty? idProperty = entityType.FindProperty("Id");
            if (idProperty != null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                idProperty.SetDefaultValueSql("NEWSEQUENTIALID()");
            }
        }

        #region IdetityConfigurations

        _ = builder.Entity<T3IdentityUser>(entity =>
        {
            _ = entity.Property(p => p.UserId).HasDefaultValue("");
            _ = entity.Property(p => p.FirstName).HasDefaultValue("");
            _ = entity.Property(p => p.LastName).HasDefaultValue("");
            _ = entity.Property(p => p.StartPage).HasDefaultValue("");
            _ = entity.Property(p => p.Barcode).HasDefaultValue("");
            _ = entity.Property(p => p.IsActive).HasDefaultValue(0);
            _ = entity.Property(p => p.PhotoUrl).HasDefaultValue("");
        });

        _ = builder.Entity<T3IdentityUserRole>(entity =>
        {
            _ = entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            _ = entity.HasOne(p => p.User).WithMany(p => p.ListRoles)
                                          .HasForeignKey(p => p.UserId)
                                          .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasOne(p => p.Role).WithMany(p => p.ListUsers)
                                          .HasForeignKey(p => p.RoleId)
                                          .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasIndex(p => p.UserId);
            _ = entity.HasIndex(p => p.RoleId);
            _ = entity.Navigation(e => e.Role).AutoInclude(); 
        });

        _ = builder.Entity<T3IdentityRole>(entity =>
        {
            _ = entity.Property(p => p.Name).HasDefaultValue("");
            _ = entity.Property(p => p.StartPage).HasDefaultValue("");
            _ = entity.Property(p => p.IsActive).HasDefaultValue(0);
            _ = entity.Property(p => p.IsTeam).HasDefaultValue(0);
            _ = entity.Property(p => p.IsDepartment).HasDefaultValue(0);
        });

        builder.Entity<T3IdentityRoleHierarchy>(entity =>
        {
            // Artık tekil Id PK (BaseEntity.Id). Çift PK yerine unique index:
            entity.HasIndex(h => new { h.ParentId, h.ChildId }).IsUnique();

            entity.HasOne(h => h.Parent)
                  .WithMany(r => r.ListChilds)
                  .HasForeignKey(h => h.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.Child)
                  .WithMany(r => r.ListParents)
                  .HasForeignKey(h => h.ChildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Navigation(h => h.Parent).AutoInclude();
            entity.Navigation(h => h.Child).AutoInclude();
        });


        _ = builder.Entity<T3IdentityClaim>(entity =>
        {
            _ = entity.Property(p => p.Type).HasDefaultValue("");
            _ = entity.Property(p => p.Value).HasDefaultValue("");
            _ = entity.Property(p => p.PermissionType).HasDefaultValue(0);

            _ = entity.HasOne(p => p.User).WithMany(p => p.ListClaims)
                                          .HasForeignKey(p => p.UserId)
                                          .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasOne(p => p.Role).WithMany(p => p.ListClaims)
                                         .HasForeignKey(p => p.RoleId)
                                         .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasIndex(p => p.UserId);
            _ = entity.HasIndex(p => p.RoleId);
        });

        #endregion

        #region ModuleConfigurations

        _ = builder.Entity<T3Module>(entity =>
        {
            _ = entity.Property(p => p.Name).HasDefaultValue("");
            _ = entity.Property(p => p.PageText).HasDefaultValue("");
            _ = entity.Property(p => p.ColorBack).HasDefaultValue("");
            _ = entity.Property(p => p.ColorFore).HasDefaultValue("");

            _ = entity.HasOne(m => m.Template)
                      .WithOne(t => t.Module)
                      .HasForeignKey<T3Template>(t => t.Id)
                      .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(m => m.Form)
                      .WithOne(f => f.Module)
                      .HasForeignKey<T3Form>(f => f.Id)
                      .OnDelete(DeleteBehavior.Restrict);

            _ = entity.Property(x => x.Name)
                      .HasMaxLength(256) 
                      .UseCollation("Turkish_CI_AS");

            _ = entity.HasIndex(x => x.Name)
                      .HasDatabaseName("UX_T3Module_Name")
                      .IsUnique();
        });

        _ = builder.Entity<T3ModuleHierarchy>(entity =>
        {
            _ = entity.HasOne(p => p.Parent).WithMany(p => p.ListChilds)
                                            .HasForeignKey(p => p.ParentId)
                                            .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasOne(p => p.Child).WithMany(p => p.ListParents)
                                           .HasForeignKey(p => p.ChildId)
                                           .OnDelete(DeleteBehavior.Restrict);
            _ = entity.HasIndex(p => p.ParentId);
            _ = entity.HasIndex(p => p.ChildId);
            _= entity.Navigation(e => e.Parent).AutoInclude();
            _= entity.Navigation(e => e.Child).AutoInclude();
        });
         
        _ = builder.Entity<T3Item>(entity =>
        {
            _ = entity.HasIndex(p => p.ModuleId);
            _ = entity.HasIndex(p => p.ModuleTypeId);
            _ = entity.Property(p => p.Name).HasDefaultValue("");

            _ = entity.HasOne(p => p.Module)
                      .WithMany(p => p.ListItems)
                      .HasForeignKey(p => p.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.ModuleType)
                      .WithMany(p => p.ListModuleTypeItems)
                      .HasForeignKey(p => p.ModuleTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(p => p.Location)
                      .WithMany(p => p.ListItems)
                      .HasForeignKey(p => p.LocationId)
                      .OnDelete(DeleteBehavior.Cascade);
             
            _ = entity.HasOne(i => i.Template)
                      .WithOne()  
                      .HasForeignKey<T3Template>(t => t.Id)
                      .HasPrincipalKey<T3Item>(i => i.Id)
                      .OnDelete(DeleteBehavior.Restrict);
             
            _ = entity.HasOne(i => i.Form)
                      .WithOne(f => f.Item)
                      .HasForeignKey<T3Form>(f => f.Id)
                      .HasPrincipalKey<T3Item>(i => i.Id)
                      .OnDelete(DeleteBehavior.Restrict);
        });


        builder.Entity<T3ItemHierarchy>(entity =>
        {
            // Tekil Id PK, (ParentId, ChildId) unique
            entity.HasIndex(h => new { h.ParentId, h.ChildId }).IsUnique();

            entity.HasOne(h => h.Parent)
                  .WithMany(i => i.ListChilds)
                  .HasForeignKey(h => h.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.Child)
                  .WithMany(i => i.ListParents)
                  .HasForeignKey(h => h.ChildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Navigation(h => h.Parent).AutoInclude();
            entity.Navigation(h => h.Child).AutoInclude();
        });


        _ = builder.Entity<T3ProcessType>(entity =>
        {
            _ = entity.Property(p => p.Name).HasDefaultValue("");
            _ = entity.Property(p => p.Barcode).HasDefaultValue("");
            _ = entity.Property(p => p.ColorBack).HasDefaultValue("");
            _ = entity.Property(p => p.ColorFore).HasDefaultValue("");
        });

        _ = builder.Entity<T3ProcessTypeItem>(entity =>
        {
            _ = entity.HasKey(ur => new { ur.ItemId, ur.TypeId });

            _ = entity.HasOne(p => p.Item)
                      .WithMany(p => p.ListProcessTypes)
                      .HasForeignKey(p => p.ItemId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.ProcessType)
                      .WithMany(p => p.ListItems)
                      .HasForeignKey(p => p.TypeId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasIndex(p => p.ItemId);
            _ = entity.HasIndex(p => p.TypeId);
        });

        _ = builder.Entity<T3ProcessTypeModule>(entity =>
        {
            _ = entity.HasKey(ur => new { ur.ModuleId, ur.TypeId });

            _ = entity.HasOne(p => p.Module)
                      .WithMany(p => p.ListProcessTypes)
                      .HasForeignKey(p => p.ModuleId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.ProcessType)
                      .WithMany(p => p.ListModules)
                      .HasForeignKey(p => p.TypeId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasIndex(p => p.ModuleId);
            _ = entity.HasIndex(p => p.TypeId);
        });

        #endregion

        #region LocationConfigurations

        _ = builder.Entity<T3Location>(entity =>
        {
            _ = entity.Property(p => p.Name).HasDefaultValue("");
            _ = entity.Property(p => p.Barcode).HasDefaultValue("");
        });

        _ = builder.Entity<T3LocationHierarchy>(entity =>
        {
            _ = entity.HasOne(p => p.Parent).WithMany(p => p.ListChilds)
                                            .HasForeignKey(p => p.ParentId)
                                            .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.Child).WithMany(p => p.ListParents)
                                           .HasForeignKey(p => p.ChildId)
                                           .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasIndex(p => p.ParentId);
            _ = entity.HasIndex(p => p.ChildId);
            _ = entity.Navigation(e => e.Parent).AutoInclude();
            _ = entity.Navigation(e => e.Child).AutoInclude();
        });

        _ = builder.Entity<T3LocationItem>(entity =>
        {
            _ = entity.HasOne(p => p.Item).WithMany(p => p.ListLocations)
                                            .HasForeignKey(p => p.ItemId)
                                            .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.Location).WithMany(p => p.ListLocationItems)
                                           .HasForeignKey(p => p.LocationId)
                                           .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasIndex(p => p.LocationId);
            _ = entity.HasIndex(p => p.ItemId);
        });

        #endregion

        #region TemplateConfigurations

        _ = builder.Entity<T3Template>(entity =>
        {
            _ = entity.HasOne(p => p.Module).WithOne(p => p.Template)
                                            .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(p => p.Item).WithOne(p => p.Template)
                                          .OnDelete(DeleteBehavior.Cascade);
        });


        _ = builder.Entity<T3TemplateApprover>(entity =>
        {
            _ = entity.HasOne(p => p.Template).WithMany(p => p.ListApprovers)
                                              .HasForeignKey(p => p.TemplateId)
                                              .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.User).WithMany(p => p.ListApproveTemplates)
                                           .HasForeignKey(p => p.UserId)
                                           .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(p => p.Role).WithMany(p => p.ListApproveTemplates)
                                          .HasForeignKey(p => p.RoleId)
                                          .OnDelete(DeleteBehavior.Cascade);
        });

        _ = builder.Entity<T3TemplatePanel>(entity =>
        {
            _ = entity.HasOne(p => p.Template).WithMany(p => p.ListPanels)
                                              .HasForeignKey(p => p.TemplateId)
                                              .OnDelete(DeleteBehavior.Cascade);

            _ = entity.Property(p => p.Name).HasDefaultValue("");
        });

        _ = builder.Entity<T3Property>(entity =>
        {
            _ = entity.Property(p => p.Name).HasDefaultValue("");
            _ = entity.Property(p => p.DisplayText).HasDefaultValue("");
            _ = entity.Property(p => p.Pattern).HasDefaultValue("");
            _ = entity.Property(p => p.FileTypes).HasDefaultValue("");
            _ = entity.Property(p => p.ExtField).HasDefaultValue("");

            _ = entity.HasOne(p => p.FormResource)
                      .WithMany(r => r.ListProperties)
                      .HasForeignKey(p => p.FormResourceId)
                      .OnDelete(DeleteBehavior.Cascade);
        });

        _ = builder.Entity<T3PropertyPanel>()
                   .HasKey(pfp => new { pfp.PropertyFieldId, pfp.PanelId });

        _ = builder.Entity<T3PropertyPanel>()
                   .HasOne(pfp => pfp.PropertyField)
                   .WithMany(pf => pf.ListPanels)
                   .HasForeignKey(pfp => pfp.PropertyFieldId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3PropertyPanel>()
                   .HasOne(pfp => pfp.Panel)
                   .WithMany(tp => tp.ListPropertyFields)
                   .HasForeignKey(pfp => pfp.PanelId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3PropertyTemplate>()
                   .HasOne(pft => pft.PropertyField)
                   .WithMany(pf => pf.ListTemplates)
                   .HasForeignKey(pft => pft.PropertyFieldId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3PropertyTemplate>()
                   .HasOne(pft => pft.Template)
                   .WithMany(t => t.ListPropertyFields)
                   .HasForeignKey(pft => pft.TemplateId)
                   .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region FormConfigurations

        _ = builder.Entity<T3Form>()
                   .HasOne(t => t.Template)
                   .WithMany(t => t.ListForms)
                   .HasForeignKey(t => t.TemplateId)
                   .OnDelete(DeleteBehavior.Restrict);

        _ = builder.Entity<T3Form>()
                   .HasOne(t => t.CreateUser)
                   .WithMany(t => t.ListFormCreates)
                   .HasForeignKey(t => t.CreateUserId)
                   .OnDelete(DeleteBehavior.Restrict);

        _ = builder.Entity<T3Form>()
                   .HasOne(t => t.ApprovedUser)
                   .WithMany(t => t.ListFormApproveds)
                   .HasForeignKey(t => t.ApprovedUserId)
                   .OnDelete(DeleteBehavior.Restrict);

        _ = builder.Entity<T3FormField>()
                   .HasOne(t => t.Form)
                   .WithMany(t => t.ListFormFields)
                   .HasForeignKey(t => t.FormId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3FormField>()
                   .HasOne(t => t.PropertyField)
                   .WithMany(t => t.ListFormFields)
                   .HasForeignKey(t => t.PropertyFieldId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3FormFieldValue>()
                   .HasOne(t => t.FormField)
                   .WithMany(t => t.ListValues)
                   .HasForeignKey(t => t.FormFieldId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3FormFieldValue>()
                   .HasOne(t => t.CreateUser)
                   .WithMany(t => t.ListFormFieldValue)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region ProtocolConfigurations

        _ = builder.Entity<T3Protocol>()
                   .HasOne(mh => mh.ProcessType)
                   .WithMany(m => m.ListProtocols)
                   .HasForeignKey(mh => mh.ProcessTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3ProtocolItem>()
                   .HasKey(mh => new { mh.ProtocolId, mh.ItemId, mh.LocationId });

        _ = builder.Entity<T3ProtocolItem>()
                   .HasOne(mh => mh.Protocol)
                   .WithMany(m => m.ListProtocolItems)
                   .HasForeignKey(mh => mh.ProtocolId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3ProtocolItem>()
                   .HasOne(mh => mh.Item)
                               .WithMany(m => m.ListProtocols)
                   .HasForeignKey(mh => mh.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);

        _ = builder.Entity<T3ProtocolItem>()
                   .HasOne(mh => mh.Location)
                   .WithMany(m => m.ListProtocolItems)
                   .HasForeignKey(mh => mh.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region ShiftConfigurations

        _ = builder.Entity<T3ShiftTypeCategory>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).HasDefaultValue("");
            _ = entity.Property(e => e.Description).HasDefaultValue("");
        });

        _ = builder.Entity<T3ShiftType>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).HasDefaultValue("");
            _ = entity.Property(e => e.Description).HasDefaultValue("");
        });

        _ = builder.Entity<T3ShiftTypeDay>(entity =>
        {
            _ = entity.HasIndex(e => e.ShiftTypeId, "IX_T3ShiftTypeDay_ShiftTypeId");
            _ = entity.HasOne(d => d.ShiftType).WithMany(p => p.ListDays)
                      .HasForeignKey(d => d.ShiftTypeId)
                      .HasConstraintName("FK_ShiftTypeDay_ShiftType");
        });

        _ = builder.Entity<T3ShiftTypeBreak>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).HasDefaultValue("");
            _ = entity.Property(e => e.Description).HasDefaultValue("");

            _ = entity.HasOne(d => d.ShiftTypeDay)
                      .WithMany(p => p.T3ShiftTypeBreaks)
                      .HasForeignKey(d => d.ShiftTypeDayId);
        });

        _ = builder.Entity<T3ShiftTypeLocation>(entity =>
        {
            _ = entity.HasOne(st => st.Location)
                      .WithMany(st => st.ListShiftTypes)
                      .HasForeignKey(st => st.LocationId)
                      .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(st => st.ShiftType)
                      .WithMany(st => st.ListLocations)
                      .HasForeignKey(st => st.ShiftTypeId)
                      .OnDelete(DeleteBehavior.Cascade);
        });

        _ = builder.Entity<T3Shift>(entity =>
        {
            _ = entity.HasOne(rh => rh.Location)
                      .WithMany(r => r.ListShifts)
                      .HasForeignKey(rh => rh.LocationId)
                      .OnDelete(DeleteBehavior.Cascade);
        });

        _ = builder.Entity<T3ShiftBreak>(entity =>
        {
            _ = entity.HasOne(rh => rh.Shift)
                .WithMany(r => r.ListBreaks)
                .HasForeignKey(rh => rh.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion
    }
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
        _ = optionsBuilder.UseSqlServer("Server=.;Initial Catalog=DBProskon;User ID=t3;Password=X-XP+45qAZrc+VmYLVB;Connect Timeout=60;MultipleActiveResultSets=True;TrustServerCertificate=true;");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
