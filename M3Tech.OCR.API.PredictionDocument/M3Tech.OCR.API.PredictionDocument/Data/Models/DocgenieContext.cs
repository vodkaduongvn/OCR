using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument.Data.Models;

public partial class DocgenieContext : DbContext
{
    public DocgenieContext()
    {
    }

    public DocgenieContext(DbContextOptions<DocgenieContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<CallType> CallTypes { get; set; }

    public virtual DbSet<Consumer> Consumers { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentCategorizationLog> DocumentCategorizationLogs { get; set; }

    public virtual DbSet<DocumentCategory> DocumentCategories { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Name=ConnectionStrings:DocgenieDBContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PRIMARY");

            entity.ToTable("calls");

            entity.HasIndex(e => e.CallTypeId, "call_type_idx");

            entity.HasIndex(e => e.ConsumerId, "consumer_idx");

            entity.Property(e => e.CallId).HasColumnName("call_id");
            entity.Property(e => e.CallReceivedDatetime)
                .HasColumnType("datetime")
                .HasColumnName("call_received_datetime");
            entity.Property(e => e.CallRespondedDatetime)
                .HasColumnType("datetime")
                .HasColumnName("call_responded_datetime");
            entity.Property(e => e.CallTypeId).HasColumnName("call_type_id");
            entity.Property(e => e.ConsumerId).HasColumnName("consumer_id");
            entity.Property(e => e.ErrorFound)
                .HasColumnType("text")
                .HasColumnName("error_found");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.JsonProvided)
                .HasColumnType("text")
                .HasColumnName("json_provided");
            entity.Property(e => e.JsonReceived)
                .HasColumnType("text")
                .HasColumnName("json_received");

            entity.HasOne(d => d.CallType).WithMany(p => p.Calls)
                .HasForeignKey(d => d.CallTypeId)
                .HasConstraintName("calls_ibfk_2");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Calls)
                .HasForeignKey(d => d.ConsumerId)
                .HasConstraintName("calls_ibfk_1");
        });

        modelBuilder.Entity<CallType>(entity =>
        {
            entity.HasKey(e => e.CallTypeId).HasName("PRIMARY");

            entity.ToTable("call_types");

            entity.Property(e => e.CallTypeId).HasColumnName("call_type_id");
            entity.Property(e => e.CallNameEn)
                .HasMaxLength(255)
                .HasColumnName("call_name_en");
            entity.Property(e => e.CallNameFr)
                .HasMaxLength(255)
                .HasColumnName("call_name_fr");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PRIMARY");

            entity.ToTable("consumers");

            entity.Property(e => e.ConsumerId).HasColumnName("consumer_id");
            entity.Property(e => e.ConsumerNameEn)
                .HasMaxLength(255)
                .HasColumnName("consumer_name_en");
            entity.Property(e => e.ConsumerNameFr)
                .HasMaxLength(255)
                .HasColumnName("consumer_name_fr");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PRIMARY");

            entity.ToTable("documents");

            entity.HasIndex(e => e.DocumentCategoryId, "document_category_idx");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.CategorizationDatetime)
                .HasColumnType("datetime")
                .HasColumnName("categorization_datetime");
            entity.Property(e => e.DocumentCategoryId).HasColumnName("document_category_id");
            entity.Property(e => e.DocumentName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("document_name");
            entity.Property(e => e.EncodedContent)
                .IsRequired()
                .HasColumnType("text")
                .HasColumnName("encoded_content");
            entity.Property(e => e.ExtractionFinishDatetime)
                .HasColumnType("datetime")
                .HasColumnName("extraction_finish_datetime");
            entity.Property(e => e.ExtractionStartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("extraction_start_datetime");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.LastCategorizationLogId).HasColumnName("last_categorization_log_id");
            entity.Property(e => e.ServerAddress)
                .HasMaxLength(255)
                .HasColumnName("server_address");
            entity.Property(e => e.ServerFolder)
                .HasMaxLength(255)
                .HasColumnName("server_folder");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.Documents)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documents_ibfk_1");
        });

        modelBuilder.Entity<DocumentCategorizationLog>(entity =>
        {
            entity.HasKey(e => e.CategorizationLogId).HasName("PRIMARY");

            entity.ToTable("document_categorization_logs");

            entity.HasIndex(e => e.DocumentCategoryId, "document_category_idx");

            entity.HasIndex(e => e.DocumentId, "document_idx");

            entity.Property(e => e.CategorizationLogId).HasColumnName("categorization_log_id");
            entity.Property(e => e.CategorizationConfidence).HasColumnName("categorization_confidence");
            entity.Property(e => e.CategorizationModelCalledDatetime)
                .HasColumnType("datetime")
                .HasColumnName("categorization_model_called_datetime");
            entity.Property(e => e.CategorizationModelResponseDatetime)
                .HasColumnType("datetime")
                .HasColumnName("categorization_model_response_datetime");
            entity.Property(e => e.CategorizationProbability).HasColumnName("categorization_probability");
            entity.Property(e => e.CategorizationType)
                .HasColumnType("enum('Creation','Update','Unknown')")
                .HasColumnName("categorization_type");
            entity.Property(e => e.DocumentCategoryId).HasColumnName("document_category_id");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
            entity.Property(e => e.UpdateSource)
                .HasMaxLength(255)
                .HasColumnName("update_source");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.DocumentCategorizationLogs)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("document_categorization_logs_ibfk_2");

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentCategorizationLogs)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("document_categorization_logs_ibfk_1");
        });

        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.HasKey(e => e.DocumentCategoryId).HasName("PRIMARY");

            entity.ToTable("document_categories");

            entity.HasIndex(e => e.DocumentNbcIdentifier, "document_nbc_identifier").IsUnique();

            entity.Property(e => e.DocumentCategoryId).HasColumnName("document_category_id");
            entity.Property(e => e.DocumentCategoryNameEn)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("document_category_name_en");
            entity.Property(e => e.DocumentCategoryNameFr)
                .HasMaxLength(255)
                .HasColumnName("document_category_name_fr");
            entity.Property(e => e.DocumentNbcIdentifier)
                .HasMaxLength(10)
                .HasColumnName("document_nbc_identifier");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_deleted");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion)
                .IsRequired()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PRIMARY");

            entity.ToTable("labels");

            entity.HasIndex(e => e.DocumentCategoryId, "document_category_id");

            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.DocumentCategoryId).HasColumnName("document_category_id");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.Labels)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("labels_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
