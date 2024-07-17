using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase;

public partial class MetadataContext : DbContext
{
    public MetadataContext()
    {
    }

    public MetadataContext(DbContextOptions<MetadataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatDatatypeReceptionMetadatum> CatDatatypeReceptionMetadata { get; set; }

    public virtual DbSet<CatReceptionExtensible> CatReceptionExtensibles { get; set; }

    public virtual DbSet<CatReceptionExtensibleListedValue> CatReceptionExtensibleListedValues { get; set; }

    public virtual DbSet<EnterpriseCatReceptionExtensible> EnterpriseCatReceptionExtensibles { get; set; }

    public virtual DbSet<InvoiceReception> InvoiceReceptions { get; set; }

    public virtual DbSet<InvoiceReceptionExtensible> InvoiceReceptionExtensibles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatDatatypeReceptionMetadatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cat_datatype_reception_metadata_pk1");

            entity.ToTable("cat_datatype_reception_metadata");

            entity.HasIndex(e => e.CodeMetadataDatatype, "cat_datatype_reception_metadata_uk1").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CodeMetadataDatatype).HasColumnName("code_metadata_datatype");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<CatReceptionExtensible>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cat_reception_extensible_pk");

            entity.ToTable("cat_reception_extensible");

            entity.HasIndex(e => e.CodeExtensible, "cat_reception_extensible_uk1").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CodeExtensible).HasColumnName("code_extensible");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IdCatExtensibleType).HasColumnName("id_cat_extensible_type");
            entity.Property(e => e.IdDatatypeReceptionMetadata).HasColumnName("id_datatype_reception_metadata");
            entity.Property(e => e.IdExtensibleType).HasColumnName("id_extensible_type");
            entity.Property(e => e.IdRoot).HasColumnName("id_root");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsChild).HasColumnName("is_child");
            entity.Property(e => e.IsListed).HasColumnName("is_listed");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.IdDatatypeReceptionMetadataNavigation).WithMany(p => p.CatReceptionExtensibles)
                .HasForeignKey(d => d.IdDatatypeReceptionMetadata)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_reception_extensible_fk2");

            entity.HasOne(d => d.IdRootNavigation).WithMany(p => p.InverseIdRootNavigation)
                .HasForeignKey(d => d.IdRoot)
                .HasConstraintName("cat_reception_extensible_fk3");
        });

        modelBuilder.Entity<CatReceptionExtensibleListedValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cat_reception_extensible_listed_values_pk");

            entity.ToTable("cat_reception_extensible_listed_values");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IdCodeExtensible).HasColumnName("id_code_extensible");
            entity.Property(e => e.IdEnterprise).HasColumnName("id_enterprise");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<EnterpriseCatReceptionExtensible>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("enterprise_cat_reception_extensible_pk");

            entity.ToTable("enterprise_cat_reception_extensible");

            entity.HasIndex(e => e.IdCatReceptionExtensible, "IX_enterprise_cat_reception_extensible_id_cat_reception_extensible");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IdCatReceptionExtensible).HasColumnName("id_cat_reception_extensible");
            entity.Property(e => e.IdEnterprise).HasColumnName("id_enterprise");
            entity.Property(e => e.IsRequired).HasColumnName("is_required");
            entity.Property(e => e.IsToSupplier).HasColumnName("is_to_supplier");
            entity.Property(e => e.Label)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("label");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.IdCatReceptionExtensibleNavigation).WithMany(p => p.EnterpriseCatReceptionExtensibles)
                .HasForeignKey(d => d.IdCatReceptionExtensible)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("enterprise_cat_reception_extensible_fk2");
        });

        modelBuilder.Entity<InvoiceReception>(entity =>
        {
            entity.ToTable("invoice_reception", tb =>
                {
                    tb.HasTrigger("invoice_reception_set_enterprise_consecutive_ins");
                    tb.HasTrigger("invoice_reception_set_enterprise_consecutive_upd");
                });

            entity.HasIndex(e => e.CreatedAt, "IX_invoice_reception_created_at");

            entity.HasIndex(e => new { e.Active, e.IdEnterpriseSupplier, e.DateReception }, "IX_invoice_reception_date_reception_id_enterprise_supplier_active");

            entity.HasIndex(e => e.IdEnterpriseIssues, "IX_invoice_reception_id_enterprise_issues");

            entity.HasIndex(e => new { e.IdEnterpriseSupplier, e.Active, e.DocumentId }, "IX_invoice_reception_id_enterprise_supplier_active_document_id");

            entity.HasIndex(e => new { e.Active, e.IdEnterpriseSupplier, e.IssueDatetime }, "IX_invoice_reception_issue_datetime_id_enterprise_supplier_active");

            entity.HasIndex(e => e.IssueDatetime, "IX_invoice_recetption_issue_datetime");

            entity.HasIndex(e => new { e.IdEnterpriseIssues, e.DocumentId, e.DocumentType }, "invoice_reception_uk1").IsUnique();

            entity.HasIndex(e => new { e.Active, e.IdEnterpriseSupplier, e.CreatedAt }, "ix_invoice_reception_active_supplier_created_at");

            entity.HasIndex(e => e.IdEnterpriseSupplier, "ix_invoice_reception_supplier");

            entity.HasIndex(e => new { e.DocumentId, e.IdEnterpriseSupplier, e.Active }, "ix_supplier_documentid");

            entity.Property(e => e.Id)
                .HasComment("Comentario para  Primary Key invoice_reception")
                .HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Accuse).HasColumnName("accuse");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.AllowanceTotalAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("allowance_total_amount");
            entity.Property(e => e.CalculationRate)
                .HasColumnType("decimal(15, 6)")
                .HasColumnName("calculation_rate");
            entity.Property(e => e.CaseNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("case_number");
            entity.Property(e => e.ChargeTotalAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("charge_total_amount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("currency");
            entity.Property(e => e.DateReception)
                .HasColumnType("datetime")
                .HasColumnName("date_reception");
            entity.Property(e => e.DeliveryNote)
                .HasColumnType("text")
                .HasColumnName("delivery_note");
            entity.Property(e => e.DocumentId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("document_id");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("document_type");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.EnterpriseConsecutive).HasColumnName("enterprise_consecutive");
            entity.Property(e => e.HasWarnings).HasColumnName("has_warnings");
            entity.Property(e => e.IdEnterpriseIssues).HasColumnName("id_enterprise_issues");
            entity.Property(e => e.IdEnterpriseSupplier).HasColumnName("id_enterprise_supplier");
            entity.Property(e => e.InfodatosRequiredNotSet).HasColumnName("infodatos_required_not_set");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.IssueDatetime)
                .HasColumnType("datetime")
                .HasColumnName("issue_datetime");
            entity.Property(e => e.IssueTime).HasColumnName("issue_time");
            entity.Property(e => e.LastConstraintStatus).HasColumnName("last_constraint_status");
            entity.Property(e => e.LineExtensionAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("line_extension_amount");
            entity.Property(e => e.MountTotal)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("mount_total");
            entity.Property(e => e.PartyIdentificationId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("party_identification_id");
            entity.Property(e => e.PartyName)
                .HasMaxLength(450)
                .IsUnicode(false)
                .HasColumnName("party_name");
            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");
            entity.Property(e => e.PersonFamilyname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("person_familyname");
            entity.Property(e => e.PersonFirstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("person_firstname");
            entity.Property(e => e.PersonMiddlename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("person_middlename");
            entity.Property(e => e.PrePaidAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("pre_paid_amount");
            entity.Property(e => e.ReceptionType).HasColumnName("reception_type");
            entity.Property(e => e.RegistrationName)
                .HasMaxLength(450)
                .IsUnicode(false)
                .HasColumnName("registration_name");
            entity.Property(e => e.SchemeId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("scheme_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StatusDian).HasColumnName("status_dian");
            entity.Property(e => e.StatusDianDatetime)
                .HasColumnType("datetime")
                .HasColumnName("status_dian_datetime");
            entity.Property(e => e.StatusDianMessage)
                .HasColumnType("text")
                .HasColumnName("status_dian_message");
            entity.Property(e => e.StatusDianValidation)
                .HasColumnType("text")
                .HasColumnName("status_dian_validation");
            entity.Property(e => e.TaxExclusiveAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("tax_exclusive_amount");
            entity.Property(e => e.TaxInclusiveAmount)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("tax_inclusive_amount");
            entity.Property(e => e.TechProviderPartyIdentificationId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tech_provider_party_identification_id");
            entity.Property(e => e.TechProviderSoftwareId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tech_provider_software_id");
            entity.Property(e => e.UblVersionId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("ubl_version_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Uuid)
                .HasMaxLength(96)
                .IsUnicode(false)
                .HasColumnName("uuid");
            entity.Property(e => e.WasSendReceive).HasColumnName("was_send_receive");
            entity.Property(e => e.WasSendResponse).HasColumnName("was_send_response");
        });

        modelBuilder.Entity<InvoiceReceptionExtensible>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoice_reception_cat_reception_extensible_pk");

            entity.ToTable("invoice_reception_extensible");

            entity.HasIndex(e => e.IdEnterpriseCatReceptionExtensible, "IX_invoice_reception_extensible_id_enterprise_cat_reception_extensible");

            entity.HasIndex(e => e.IdInvoiceReception, "IX_invoice_reception_extensible_id_invoice_reception");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IdEnterpriseCatReceptionExtensible).HasColumnName("id_enterprise_cat_reception_extensible");
            entity.Property(e => e.IdInvoiceReception).HasColumnName("id_invoice_reception");
            entity.Property(e => e.Internal1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("internal1");
            entity.Property(e => e.Internal2)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("internal2");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Value)
                .IsUnicode(false)
                .HasColumnName("value");

            entity.HasOne(d => d.IdEnterpriseCatReceptionExtensibleNavigation).WithMany(p => p.InvoiceReceptionExtensibles)
                .HasForeignKey(d => d.IdEnterpriseCatReceptionExtensible)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_reception_cat_reception_extensible_fk2");

            entity.HasOne(d => d.IdInvoiceReceptionNavigation).WithMany(p => p.InvoiceReceptionExtensibles)
                .HasForeignKey(d => d.IdInvoiceReception)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_reception_cat_reception_extensible_fk1");

        });
    }
}
