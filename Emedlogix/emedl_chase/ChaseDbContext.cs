using emedl_chase.DbModel;
using emedl_chase.Cure_Model;
using Microsoft.EntityFrameworkCore;


namespace emedl_chase
{
    public class ChaseDbContext:DbContext
    {
        public ChaseDbContext(DbContextOptions<ChaseDbContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DenialClaimLine>(entity =>
            {
                entity.ToTable("denialclaim_line"); // DB table name

                // Primary key definition
                entity.HasKey(e => e.Id); // 

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ClaimId).HasColumnName("claim_id");
                entity.Property(e => e.ClaimLineId).HasColumnName("claim_line_id");
                entity.Property(e => e.MemberId).HasColumnName("member_id");
                entity.Property(e => e.MemberName).HasColumnName("member_name");
                entity.Property(e => e.MemberAge).HasColumnName("member_age");
                entity.Property(e => e.MemberGender).HasColumnName("member_gender");
                entity.Property(e => e.ProviderId).HasColumnName("provider_id");
                entity.Property(e => e.ProviderNpi).HasColumnName("provider_npi");
                entity.Property(e => e.Insurance).HasColumnName("insurance");
                entity.Property(e => e.CobDetails).HasColumnName("cob_details");
                entity.Property(e => e.DosFrom).HasColumnName("dos_from");
                entity.Property(e => e.DosTo).HasColumnName("dos_to");
                entity.Property(e => e.Pos).HasColumnName("pos");
                entity.Property(e => e.Cpt).HasColumnName("cpt");
                entity.Property(e => e.Modifiers).HasColumnName("modifiers");
                entity.Property(e => e.Units).HasColumnName("units");
                entity.Property(e => e.Icds).HasColumnName("icds");
                entity.Property(e => e.AllowedAmount).HasColumnName("allowed_amount");
                entity.Property(e => e.PaidAmount).HasColumnName("paid_amount");
                entity.Property(e => e.DeniedAmount).HasColumnName("denied_amount");
            });

            modelBuilder.Entity<SaveAsDraft>(entity =>
            {
                entity.ToTable("saveasdraft");

                // Primary key definition
                entity.HasKey(e => e.id);


            });


            modelBuilder.Entity<classificationrules>(entity =>
            {
                entity.HasKey(e => e.RuleId)
                    .HasName("idx_16405_primary");

                entity.ToTable("classificationrules", schema: "public");

                entity.Property(e => e.RuleId).HasColumnName("ruleid");


                entity.Property(e => e.AgeRelated)
                    .HasMaxLength(100)
                    .HasColumnName("agerelated");

                entity.Property(e => e.AltKeyword)
                    .HasMaxLength(250)
                    .HasColumnName("altkeyword");

                entity.Property(e => e.Associatedcondition1)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition1");

                entity.Property(e => e.Associatedcondition2)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition2");

                entity.Property(e => e.Associatedcondition3)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition3");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("code")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.ComplicationProcess)
                    .HasMaxLength(250)
                    .HasColumnName("complicationprocess");

                entity.Property(e => e.Conditions)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("conditions")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.Context)
                    .HasMaxLength(250)
                    .HasColumnName("context");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.Descs).HasColumnName("descs");

                entity.Property(e => e.Externalcontext)
                    .HasMaxLength(250)
                    .HasColumnName("externalcontext");

                entity.Property(e => e.FamilialAutoimmune)
                    .HasMaxLength(250)
                    .HasColumnName("familialautoimmune");

                entity.Property(e => e.GenderRelated)
                    .HasMaxLength(100)
                    .HasColumnName("genderrelated");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isdeleted")
                    .HasDefaultValueSql("'0'::smallint");

                entity.Property(e => e.IsNonHCC)
                    .HasColumnName("isnonhcc")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsStndAlone).HasColumnName("isstndalone");

                entity.Property(e => e.Local_Descs).HasColumnName("local_descs");

                entity.Property(e => e.LocationBodypart)
                    .HasMaxLength(250)
                    .HasColumnName("locationbodypart");

                entity.Property(e => e.LocationBodySystem)
                    .HasMaxLength(250)
                    .HasColumnName("locationbodysystem");

                entity.Property(e => e.LocationLaterality)
                    .HasMaxLength(250)
                    .HasColumnName("locationlaterality");

                entity.Property(e => e.LocationOrgan)
                    .HasMaxLength(250)
                    .HasColumnName("locationorgan");

                entity.Property(e => e.LocationPosition)
                    .HasMaxLength(250)
                    .HasColumnName("locationposition");

                entity.Property(e => e.LocationTissue)
                    .HasMaxLength(250)
                    .HasColumnName("locationtissue");

                entity.Property(e => e.Locprop)
                    .HasMaxLength(250)
                    .HasColumnName("locprop");

                entity.Property(e => e.Manifestation)
                    .HasMaxLength(250)
                    .HasColumnName("manifestation");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.NamePlace)
                    .HasMaxLength(250)
                    .HasColumnName("nameplace");

                entity.Property(e => e.OtherSpecific)
                    .HasMaxLength(250)
                    .HasColumnName("otherspecific");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Qualityspecificity)
                    .HasMaxLength(250)
                    .HasColumnName("qualityspecificity");

                entity.Property(e => e.Reviewed).HasColumnName("reviewed");

                entity.Property(e => e.RuleNo)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("ruleno");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.SequelaeHistory)
                    .HasMaxLength(250)
                    .HasColumnName("sequelaehistory");

                entity.Property(e => e.Severity)
                    .HasMaxLength(250)
                    .HasColumnName("severity");

                entity.Property(e => e.Source).HasColumnName("source");

                entity.Property(e => e.StageEncounter)
                    .HasMaxLength(100)
                    .HasColumnName("stageencounter");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Timing)
                    .HasMaxLength(250)
                    .HasColumnName("timing");

                entity.Property(e => e.Units)
                    .HasMaxLength(250)
                    .HasColumnName("units");

                entity.Property(e => e.UnSpecified)
                    .HasMaxLength(250)
                    .HasColumnName("unspecified");

                entity.Property(e => e.ValidFromDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("validfromdate");

                entity.Property(e => e.ValidToDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("validtodate");



            });

            //modelBuilder.Entity<Mailverify>(entity =>

            //{
            //    entity.ToTable("mailverify");
            //    entity.Property(e => e.Id).HasColumnName("Id");

            //    entity.Property(e => e.UserId)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //    entity.Property(e => e.CreatedOn);

            //    entity.Property(e => e.UUID)
            //    .IsRequired();

            //    entity.Property(e => e.Status)
            //    .HasMaxLength(100)
            //    .IsRequired();



            //});

            modelBuilder.Entity<classificationrules_NonReview>(entity =>
            {
                entity.ToTable("classificationrules_nonreview", schema: "public");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AgeRelated)
                    .HasMaxLength(100)
                    .HasColumnName("agerelated");

                entity.Property(e => e.AltKeyword)
                    .HasMaxLength(250)
                    .HasColumnName("altkeyword");

                entity.Property(e => e.Associatedcondition1)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition1");

                entity.Property(e => e.Associatedcondition2)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition2");

                entity.Property(e => e.Associatedcondition3)
                    .HasMaxLength(250)
                    .HasColumnName("associatedcondition3");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("code")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.ComplicationProcess)
                    .HasMaxLength(250)
                    .HasColumnName("complicationprocess");

                entity.Property(e => e.Conditions)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("conditions")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.Context)
                    .HasMaxLength(250)
                    .HasColumnName("context");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.Descs).HasColumnName("descs");

                entity.Property(e => e.Externalcontext)
                    .HasMaxLength(250)
                    .HasColumnName("externalcontext");

                entity.Property(e => e.FamilialAutoimmune)
                    .HasMaxLength(250)
                    .HasColumnName("familialautoimmune");

                entity.Property(e => e.GenderRelated)
                    .HasMaxLength(100)
                    .HasColumnName("genderrelated");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isdeleted")
                    .HasDefaultValueSql("'0'::smallint");

                entity.Property(e => e.IsNonHCC)
                    .HasColumnName("isnonhcc")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsStndAlone).HasColumnName("isstndalone");

                entity.Property(e => e.Local_Descs).HasColumnName("local_descs");

                entity.Property(e => e.LocationBodypart)
                    .HasMaxLength(250)
                    .HasColumnName("locationbodypart");

                entity.Property(e => e.LocationBodySystem)
                    .HasMaxLength(250)
                    .HasColumnName("locationbodysystem");

                entity.Property(e => e.LocationLaterality)
                    .HasMaxLength(250)
                    .HasColumnName("locationlaterality");

                entity.Property(e => e.LocationOrgan)
                    .HasMaxLength(250)
                    .HasColumnName("locationorgan");

                entity.Property(e => e.LocationPosition)
                    .HasMaxLength(250)
                    .HasColumnName("locationposition");

                entity.Property(e => e.LocationTissue)
                    .HasMaxLength(250)
                    .HasColumnName("locationtissue");

                entity.Property(e => e.Locprop)
                    .HasMaxLength(250)
                    .HasColumnName("locprop");

                entity.Property(e => e.Manifestation)
                    .HasMaxLength(250)
                    .HasColumnName("manifestation");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.NamePlace)
                    .HasMaxLength(250)
                    .HasColumnName("nameplace");

                entity.Property(e => e.OtherSpecific)
                    .HasMaxLength(250)
                    .HasColumnName("otherspecific");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Qualityspecificity)
                    .HasMaxLength(250)
                    .HasColumnName("qualityspecificity");

                entity.Property(e => e.Reviewed).HasColumnName("reviewed");

                entity.Property(e => e.RuleId).HasColumnName("ruleid");

                entity.Property(e => e.RuleNo)
                    .HasMaxLength(200)
                    .HasColumnName("ruleno");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.SequelaeHistory)
                    .HasMaxLength(250)
                    .HasColumnName("sequelaehistory");

                entity.Property(e => e.Severity)
                    .HasMaxLength(250)
                    .HasColumnName("severity");

                entity.Property(e => e.Source).HasColumnName("source");

                entity.Property(e => e.StageEncounter)
                    .HasMaxLength(100)
                    .HasColumnName("stageencounter");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.Timing)
                    .HasMaxLength(250)
                    .HasColumnName("timing");

                entity.Property(e => e.Units)
                    .HasMaxLength(250)
                    .HasColumnName("units");

                entity.Property(e => e.UnSpecified)
                    .HasMaxLength(250)
                    .HasColumnName("unspecified");

                entity.Property(e => e.ValidFromDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("validfromdate");

                entity.Property(e => e.ValidToDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("validtodate");
            });

            modelBuilder.Entity<excludes>(entity =>
            {
                entity.ToTable("excludes", schema: "public");

                entity.HasIndex(e => e.IncludeCode, "idx_16417_excludes_code");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.ExcludeCode)
                    .HasMaxLength(450)
                    .HasColumnName("excludecode");

                entity.Property(e => e.IncludeCode)
                    .HasMaxLength(45)
                    .HasColumnName("includecode");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");
            });

            //modelBuilder.Entity<excludes>(entity =>
            //{
            //    entity.ToTable("excludes_old");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.Createdby).HasColumnName("createdby");

            //    entity.Property(e => e.Createddate)
            //        .HasColumnType("timestamp with time zone")
            //        .HasColumnName("createddate");

            //    entity.Property(e => e.Excludecode)
            //        .IsRequired()
            //        .HasMaxLength(45)
            //        .HasColumnName("excludecode")
            //        .HasDefaultValueSql("''::character varying");

            //    entity.Property(e => e.Includecode)
            //        .IsRequired()
            //        .HasMaxLength(45)
            //        .HasColumnName("includecode")
            //        .HasDefaultValueSql("''::character varying");

            //    entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

            //    entity.Property(e => e.Modifieddate)
            //        .HasColumnType("timestamp with time zone")
            //        .HasColumnName("modifieddate");
            //});

            modelBuilder.Entity<icd>(entity =>
            {
                entity.ToTable("icd", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EffectiveDate).HasColumnName("id");
                entity.Property(e => e.EffectiveDate).HasColumnName("effectivedate");
                entity.Property(e => e.TermDate).HasColumnName("termdate");
                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Hcc)
                    .HasMaxLength(45)
                    .HasColumnName("hcc");

                entity.Property(e => e.Icdcode)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("icdcode");
            });

            modelBuilder.Entity<manualexcludecode>(entity =>
            {
                entity.ToTable("manualexcludecode", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.HCCCode)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("hcccode");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.RemovedHCC)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("removedhcc");
            });

            modelBuilder.Entity<manualincludecode>(entity =>
            {
                entity.ToTable("manualincludecode", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalHCC)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("additionalhcc");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.HCCCode)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("hcccode");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");
            });

            modelBuilder.Entity<medication>(entity =>
            {
                entity.ToTable("meatterms", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.IsDelete).HasColumnName("isdelete");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasColumnName("meatterm");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");
            });

            modelBuilder.Entity<medCondition>(entity =>
            {
                entity.ToTable("medcondition", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDelete).HasColumnName("isdelete");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<medication>(entity =>
            {
                entity.ToTable("medication", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasColumnName("drugname");

                entity.Property(e => e.IsDelete).HasColumnName("isdelete");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");
            });

            modelBuilder.Entity<negation>(entity =>
            {
                entity.ToTable("negation", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value");
            });

            //modelBuilder.Entity<ruleinclusion>(entity =>
            //{
            //    entity.ToTable("ruleinclusion", schema: "public");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.CreatedBy).HasColumnName("createdby");

            //    entity.Property(e => e.CreatedDate)
            //        .HasColumnType("timestamp with time zone")
            //        .HasColumnName("createddate");

            //    entity.Property(e => e.IncludeId).HasColumnName("includeid");

            //    entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

            //    entity.Property(e => e.ModifiedDate)
            //        .HasColumnType("timestamp with time zone")
            //        .HasColumnName("modifieddate");

            //    entity.Property(e => e.RuleId).HasColumnName("ruleid");
            //});

            modelBuilder.Entity<rulewithoutwords>(entity =>
            {
                entity.ToTable("rulewithoutwords", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");

                entity.Property(e => e.RuleId).HasColumnName("ruleid");

                entity.Property(e => e.Word)
                    .IsRequired()
                    .HasColumnName("word");

                entity.Property(e => e.IsCodeLevel)
                    .HasColumnName("iscodelevel");

                entity.Property(e => e.IsDelete)
                   .HasColumnName("isdelete");

                entity.Property(e => e.Code)
                   .HasColumnName("code");

                entity.HasOne(d => d.Classificationrule)
                  .WithMany(p => p.Rulewithoutwords)
                  .HasForeignKey(d => d.RuleId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<section>(entity =>
            {
                entity.ToTable("section", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CharacterCnt).HasColumnName("charactercnt");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.Heading)
                    .HasMaxLength(500)
                    .HasColumnName("heading");

                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("text");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'Conclusive'::character varying");
            });

            modelBuilder.Entity<symptoms>(entity =>
            {
                entity.ToTable("symptoms", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDelete).HasColumnName("isdelete");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");

                entity.Property(e => e.SymptomCode)
                    .IsRequired()
                    .HasColumnName("symptomcode");
            });


            modelBuilder.Entity<wordconversion>(entity =>
            {
                entity.ToTable("wordconversion", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("nextval('wordconversion_id_seq'::regclass)");

                entity.Property(e => e.Abbreviation)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("abbreviation");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.Expansion)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("expansion");

                entity.Property(e => e.IsBeforeApplied)
                    .HasColumnName("isbeforeapplied")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.SourceType)
                    .HasMaxLength(50)
                    .HasColumnName("sourcetype")
                    .HasDefaultValueSql("'Abbreviation'::character varying");
            });

            modelBuilder.Entity<meatterms>(entity =>
            {
                entity.ToTable("meatterms", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.IsDelete).HasColumnName("isdelete");

                entity.Property(e => e.MEATTerm)
                    .IsRequired()
                    .HasColumnName("meatterm");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(45)
                    .HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifiedon");
            });


            modelBuilder.Entity<combinationcode>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ex_Code1)
                .HasColumnName("ex_code1")
                   .HasMaxLength(45);

                entity.Property(e => e.Ex_Code2)
                 .HasColumnName("ex_code2")
                   .HasMaxLength(45);

                entity.Property(e => e.Ex_Code3)
                 .HasColumnName("ex_code3")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code1)
                 .HasColumnName("com_code1")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code2)
                 .HasColumnName("com_code2")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code3)
                 .HasColumnName("com_code3")
                   .HasMaxLength(45);

                entity.Property(e => e.CreatedOn)
                .HasColumnName("createdon").HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                 .HasColumnName("createdby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn)
                 .HasColumnName("modifiedon")
                .HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                .HasColumnName("modifiedby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.effectivedate)
                 .HasColumnName("effectivedate")
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.termdate)
                 .HasColumnName("termdate")
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.IsDelete)
       .HasColumnName("isdelete");

            });

            modelBuilder.Entity<categorymaster>(entity =>
            {
                entity.ToTable("categorymaster", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                .HasColumnName("name")
                   .HasMaxLength(100).IsRequired();

                entity.Property(e => e.Year)
                .HasColumnName("year")
                .HasColumnType("SMALLINT").IsRequired();

                entity.Property(e => e.IsDelete)
           .HasColumnName("isdelete");

                entity.Property(e => e.IsRxHcc)
       .HasColumnName("isrxhcc");


                entity.Property(e => e.CreatedOn)
                 .HasColumnName("createdon")
                .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                .HasColumnName("createdby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn)
                  .HasColumnName("modifiedon")
                .HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                 .HasColumnName("modifiedby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<hccversioncategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryID)
                .HasColumnName("categoryid")
                .IsRequired();

                entity.Property(e => e.CreatedOn)
                 .HasColumnName("createdon")
                .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                 .HasColumnName("createdby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                 .HasColumnName("code")
                    .HasMaxLength(45);

                entity.Property(e => e.HccCode)
                 .HasColumnName("hcccode")
                    .HasMaxLength(45);

                entity.Property(e => e.Description)
                 .HasColumnName("description");

                entity.HasOne(d => d.Categorymaster)
                  .WithMany(p => p.Hccversioncategories)
                  .HasForeignKey(d => d.CategoryID)
                  .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<hccinclusion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryID)
                .HasColumnName("categoryid")
                .IsRequired();

                entity.Property(e => e.HccCode)
                .HasColumnName("hcccode")
                .HasMaxLength(45).
                IsRequired();

                entity.Property(e => e.InclusionCode)
                .HasColumnName("inclusioncode")
                .HasMaxLength(45).IsRequired();

                entity.Property(e => e.CreatedOn)
                .HasColumnName("createdon")
                .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                .HasColumnName("createdby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.HasOne(d => d.Categorymaster)
                  .WithMany(p => p.Hccinclusions)
                  .HasForeignKey(d => d.CategoryID)
                  .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<organization>(entity =>
            {
                entity.ToTable("organization", schema: "public");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.OrgName)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.OrgType)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();


                entity.Property(e => e.City)
                .HasMaxLength(100);


                entity.Property(e => e.State)
                .HasMaxLength(100);


                entity.Property(e => e.Country)
                .HasMaxLength(50);


                entity.Property(e => e.ZipCode)
                .HasMaxLength(50);

                entity.Property(e => e.CreatedBy)
                .HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp with time zone");

                entity.Property(e => e.ModifiedBy)
                   .HasMaxLength(50);

                entity.Property(e => e.ModifiedOn)
               .HasColumnType("timestamp with time zone");


                entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();




            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", schema: "public");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.LastName)
               .HasMaxLength(100);

                entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.PhoneNo)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();

                entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.AltPhoneNo)
                .HasMaxLength(50);

                entity.Property(e => e.CreatedBy)
                .HasMaxLength(50);

                entity.Property(e => e.CreatedBy)
                .HasMaxLength(50);

                entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DOJ)
                .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DOT)
                .HasColumnType("timestamp with time zone");

                entity.Property(e => e.ModifiedBy)
                   .HasMaxLength(50);

                entity.Property(e => e.ModifiedOn)
               .HasColumnType("timestamp with time zone");
            });


            modelBuilder.Entity<jobmaster>(entity =>
            {
                entity.HasKey(e => e.JobId);
                entity.Property(e => e.JobId).HasColumnName("jobid");
                entity.Property(e => e.Jobdate).HasColumnName("jobdate");
                entity.Property(e => e.DocsCount).HasColumnName("docscount");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedDate).HasColumnName("createddate");
                entity.Property(e => e.OrganisationId).HasColumnName("organisationid");
                entity.Property(e => e.JobStartTime).HasColumnName("jobstarttime");
                entity.Property(e => e.JobEndTime).HasColumnName("jobendtime");
                entity.Property(e => e.SuccessCount).HasColumnName("successcount");
                entity.Property(e => e.FailiureCount).HasColumnName("failiurecount");
                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<documentMaster>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.JobId).HasColumnName("jobid");
                entity.Property(e => e.DocId).HasColumnName("docid");
                entity.Property(e => e.DocName).HasColumnName("docname");
                entity.Property(e => e.DocDesc).HasColumnName("docdesc");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.TemplateId).HasColumnName("templateid");
                entity.Property(e => e.OrganisationId).HasColumnName("organisationid");
                entity.Property(e => e.FilePath).HasColumnName("filepath");
                entity.Property(e => e.ActualDocumentName).HasColumnName("actualDocumentName");

                entity.HasOne(d => d.Jobmaster)
                 .WithMany(p => p.DocumentMasters)
                 .HasForeignKey(d => d.JobId)
                 .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<documenResultLineItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.JobDetailId).HasColumnName("jobdetailid");
                entity.Property(e => e.SentenceId).HasColumnName("sentenceid");
                entity.Property(e => e.RuleNo).HasColumnName("ruleno");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.Code).HasColumnName("code");
                entity.Property(e => e.Section).HasColumnName("section");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CodeType).HasColumnName("codetype");
                entity.Property(e => e.isnonhcc).HasColumnName("isnonhcc");
                entity.Property(e => e.PageNo).HasColumnName("pageno");

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.DocumenResultLineItems)
                  .HasForeignKey(d => d.JobDetailId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<doc_page_sentence>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DocumentId).HasColumnName("document_id");
                entity.Property(e => e.FullText).HasColumnName("full_text");
                entity.Property(e => e.PageNo).HasColumnName("page_no");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");
                entity.Property(e => e.ModifiedOn).HasColumnName("modifiedon");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.JobId).HasColumnName("jobid");
                entity.Property(e => e.StartedOn).HasColumnName("started_on");
                entity.Property(e => e.EndOn).HasColumnName("end_on");
                entity.Property(e => e.DosList).HasColumnName("dos_list");
                entity.Property(e => e.DOS).HasColumnName("dos");

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.doc_page_sentences)
                  .HasForeignKey(d => d.DocumentId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<documenDefaultCodeExtract>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.JobDetailId).HasColumnName("jobdetailid");
                entity.Property(e => e.SentenceId).HasColumnName("sentenceid");
                entity.Property(e => e.RuleNo).HasColumnName("ruleno");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.Code).HasColumnName("code");
                entity.Property(e => e.Section).HasColumnName("section");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CodeType).HasColumnName("codetype");
                entity.Property(e => e.isnonhcc).HasColumnName("isnonhcc");
                entity.Property(e => e.PageNo).HasColumnName("pageno");

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.DocumenDefaultCodeExtracts)
                  .HasForeignKey(d => d.JobDetailId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<doc_Demographic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientName).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.PatientType).HasMaxLength(100);
                entity.Property(e => e.MemberID).HasMaxLength(100);

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.Demographics)
                  .HasForeignKey(d => d.Doc_MasterID)
                  .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<documentExtract>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.DocumentMaster)
                 .WithMany(p => p.documentExtracts)
                 .HasForeignKey(d => d.Doc_MasterID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //modelBuilder.Entity<documentAssignment>(entity =>
            //{
            //    entity.HasKey(e => e.ID);

            //    entity.Property(e => e.Status).HasMaxLength(100);
            //    entity.HasOne(d => d.DocumentMaster)
            //     .WithMany(p => p.DocumentAssignments)
            //     .HasForeignKey(d => d.DocumentID)
            //     .OnDelete(DeleteBehavior.Restrict);
            //});

            modelBuilder.Entity<finalhcc>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.DocumentMaster)
                 .WithMany(p => p.Finalhccs)
                 .HasForeignKey(d => d.DocmentMasterID)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Categorymaster)
                 .WithMany(p => p.Finalhccs)
                 .HasForeignKey(d => d.VersionCategoryID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<documentversionhistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.DocumentMaster)
                 .WithMany(p => p.Documentversionhistories)
                 .HasForeignKey(d => d.Doc_MasterID)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.User)
                 .WithMany(p => p.Documentversionhistories)
                 .HasForeignKey(d => d.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<codeChangeLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Type).HasMaxLength(20);
                entity.Property(e => e.UserType).HasMaxLength(20);

                entity.HasOne(d => d.DocumentMaster)
                 .WithMany(p => p.CodeChangeLogs)
                 .HasForeignKey(d => d.Doc_MasterID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<task>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Task).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.HasOne(a => a.RuleTask).WithOne(b => b.Task)
        .HasForeignKey<ruleTask>(e => e.TaskId);

            });

            modelBuilder.Entity<ruleTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<ruleTaskLineItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(20);

                entity.HasOne(d => d.RuleTask)
                 .WithMany(p => p.RuleTaskLineItems)
                 .HasForeignKey(d => d.RuleTaskId)
                 .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<dosExcel>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<meatSentence>(entity =>
            {
                entity.ToTable("meatSentence", schema: "public");

                entity.Property(e => e.Id).HasColumnName("Id").IsRequired();

                entity.Property(e => e.Code)
                    .IsRequired().HasColumnName("Code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("CreatedBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("CreatedOn");

                entity.Property(e => e.PageNo).HasColumnName("PageNo").IsRequired();

                entity.Property(e => e.Dos)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("Dos");

                entity.Property(e => e.Sentence).HasColumnName("Sentence").IsRequired();

                entity.Property(e => e.SentenceId)

                    .HasColumnName("SentenceId").IsRequired();

                entity.Property(e => e.DocId)
                    .HasColumnName("DocId").IsRequired();

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.meatSentences)
                  .HasForeignKey(d => d.DocId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.IsBestSentence).HasColumnName("isbestsentence");




            });


            modelBuilder.Entity<doc_CodeSentences>(entity =>
            {
                entity.ToTable("doc_CodeSentences", schema: "public");

                entity.Property(e => e.Id).HasColumnName("Id").IsRequired();

                entity.Property(e => e.Code)
                    .IsRequired().HasColumnName("Code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("CreatedBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("CreatedOn");

                entity.Property(e => e.PageNo).HasColumnName("PageNo").IsRequired();

                entity.Property(e => e.Dos)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("Dos");

                entity.Property(e => e.Sentence).HasColumnName("Sentence").IsRequired();

                entity.Property(e => e.SentenceId)

                    .HasColumnName("SentenceId").IsRequired();

                entity.Property(e => e.DocId)
                    .HasColumnName("DocId").IsRequired();

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.CodeSentences)
                  .HasForeignKey(d => d.DocId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.IsBestSentence).HasColumnName("isbestsentence");



            });

            modelBuilder.Entity<doc_MutualExclues>(entity =>
            {
                entity.ToTable("doc_MutualExclues", schema: "public");

                entity.Property(e => e.Id).HasColumnName("Id").IsRequired();

                entity.Property(e => e.Code)
                    .IsRequired().HasColumnName("Code");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(45)
                    .HasColumnName("CreatedBy");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("CreatedOn");

                entity.Property(e => e.Dos)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("Dos");

                entity.Property(e => e.DocId)
                    .HasColumnName("DocId").IsRequired();

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.CodeMutualExcludes)
                  .HasForeignKey(d => d.DocId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<alreadyexistmembercodes>(entity =>
            {
                entity.ToTable("alreadyexistmembercodes", schema: "public");

                entity.Property(e => e.id).HasColumnName("id").IsRequired();

                entity.HasOne(d => d.DocumentMaster)
                  .WithMany(p => p.Alreadyexistmembercodes)
                  .HasForeignKey(d => d.docmasterid)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<abbreviationwithout_word>(entity =>
            {
                entity.ToTable("abbreviationwithout_word", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("nextval('abbreviationwithout_word_id_seq'::regclass)");

                entity.Property(e => e.AbbreviationId)
                    .IsRequired()
                    .HasColumnName("abbreviationid");

                entity.Property(e => e.Withoutword).HasColumnName("withoutword");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isdeleted")
                    .HasDefaultValueSql("false");

                entity.HasOne(d => d.Abbreviation)
                .WithMany(p => p.AbbreviationWithoutWord)
                .HasForeignKey(d => d.AbbreviationId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<negationwithout_word>(entity =>
            {
                entity.ToTable("negationwithout_word", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("nextval('negationwithout_word_id_seq'::regclass)");

                entity.Property(e => e.NegationId)
                    .IsRequired()
                    .HasColumnName("negationid");

                entity.Property(e => e.Withoutword).HasColumnName("withoutword");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isdeleted")
                    .HasDefaultValueSql("false");

                entity.HasOne(d => d.Negation)
                .WithMany(p => p.NegationWithout_Word)
                .HasForeignKey(d => d.NegationId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<feedbackloop>(entity =>
            {
                entity.ToTable("feedbackloop", schema: "public");

                entity.Property(e => e.id).HasColumnName("id").HasDefaultValueSql("nextval('feedbackloop_id_seq'::regclass)");

                entity.Property(e => e.sentencetxt)
                    .HasColumnName("sentencetxt");

                entity.Property(e => e.code).HasColumnName("code");
                entity.Property(e => e.codetype).HasColumnName("codetype");
                entity.Property(e => e.actionby).HasColumnName("actionby");
                entity.Property(e => e.rule).HasColumnName("rule");
                entity.Property(e => e.createdby).HasColumnName("createdby");
                entity.Property(e => e.sentenceid).HasColumnName("sentenceid");
                entity.Property(e => e.jobid).HasColumnName("jobid");
                entity.Property(e => e.instancetype).HasColumnName("instancetype");
                entity.Property(e => e.createddate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");
                entity.Property(e => e.modifiedby).HasColumnName("modifiedby");
                entity.Property(e => e.modifieddate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

            });

            modelBuilder.Entity<researchanalyst>(entity =>
            {
                entity.ToTable("researchanalyst", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.createdby)
                    .HasColumnName("createdby");

                entity.Property(e => e.createdon)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");


                entity.Property(e => e.jresponse)
                   .HasColumnName("jresponse")
                   .HasColumnType("json");

                entity.Property(e => e.assigneddate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("assigneddate");

            });

            modelBuilder.Entity<ra_taskwithrule>(entity =>
            {
                entity.ToTable("ra_taskwithrule", schema: "public");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.ra_taskid).HasColumnName("ra_taskid");

                entity.Property(e => e.sourceid).HasColumnName("sourceid");

                entity.Property(e => e.sourcetype).HasColumnName("sourcetype");

                entity.Property(e => e.ruleno).HasColumnName("ruleno");

                entity.Property(e => e.createdby)
                    .HasColumnName("createdby");

                entity.Property(e => e.createdon)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

            });

            modelBuilder.Entity<fl_auditlog>(entity =>
            {
                entity.ToTable("fl_auditlog", schema: "public");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.ra_taskid)
                    .HasColumnName("ra_taskid");

                entity.Property(e => e.createdon)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

                entity.Property(e => e.reason)
                   .HasColumnName("reason");


            });

            modelBuilder.Entity<auditLog>(entity =>

            {
                entity.ToTable("audit_log", schema: "public");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasColumnName("id");
                entity.Property(e => e.UserID).HasColumnName("userid");
                entity.Property(e => e.RoleId).HasColumnName("roleid");
                entity.Property(e => e.Controller).HasColumnName("controller");
                entity.Property(e => e.Action).HasColumnName("action");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.IP).HasColumnName("ip");
                entity.Property(e => e.UserAgent).HasColumnName("useragent");
                entity.Property(e => e.Value1).HasColumnName("value1");
                entity.Property(e => e.Value2).HasColumnName("value2");



            });

            modelBuilder.Entity<dosterms>(entity =>
            {
                entity.ToTable("dosterms", schema: "public");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.modifieddate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.value)
                .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value");

                entity.Property(e => e.isdeleted)
                   .HasColumnName("isdeleted");
            });

            modelBuilder.Entity<Otp>(entity =>
            {
                entity.ToTable("mailotp", schema: "public");
                entity.Property(e => e.ID).HasColumnName("id").IsRequired();
                entity.Property(e => e.Otptype)
                    .HasColumnName("otptype");

                entity.Property(e => e.IsUsed).HasColumnName("isused");
                entity.Property(e => e.Otpvalue).HasColumnName("otpvalue");
                entity.Property(e => e.UserId).HasColumnName("userid");
                entity.Property(e => e.SendTo).HasColumnName("sendto");
                entity.Property(e => e.Purpose).HasColumnName("purpose");
                entity.Property(e => e.IsSent).HasColumnName("issent");
                entity.Property(e => e.CreatedTime)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdtime");
                entity.Property(e => e.ExpiryTime)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("expirytime");

            });

            modelBuilder.Entity<standfordcondition>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.HasOne(d => d.DocumentMaster)
                 .WithMany(p => p.standfordconditions)
                 .HasForeignKey(d => d.doc_masterid)
                 .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<risk_age_gender_score_master>(entity =>
            {
                entity.ToTable("risk_score_agegenderscore", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<risk_hcc_counts_score>(entity =>
            {
                entity.ToTable("risk_score_hcccountscore", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<risk_hcc_interaction>(entity =>
            {
                entity.ToTable("risk_score_hccinteraction", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<risk_hcc_score_list>(entity =>
            {
                entity.ToTable("risk_score_hccscorelist", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<risk_county_rate_master>(entity =>
            {
                entity.ToTable("risk_county_rate_master", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });


            modelBuilder.Entity<icd_condtions_mapping>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("icd_condtions_mapping", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id");

            });

            modelBuilder.Entity<charts_log_table>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("charts_log_table", schema: "public");
                entity.Property(e => e.id).HasColumnName("id");

            });

            modelBuilder.Entity<charts_log_table_step2>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("charts_log_table_step2", schema: "public");
                entity.Property(e => e.id).HasColumnName("id");

            });

            modelBuilder.Entity<charts_log_table_step3>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.ToTable("charts_log_table_step3", schema: "public");
                entity.Property(e => e.id).HasColumnName("id");

            });


            modelBuilder.Entity<rulewithoutwordsforicdmapping>(entity =>
            {
                entity.ToTable("rulewithoutwordsforicdmapping", schema: "public");

                entity.HasKey(e => e.id);
            });


            modelBuilder.Entity<icd_specificity>(entity =>
            {
                entity.ToTable("icd_specificity", schema: "public");
                entity.HasKey(e => e.id);
            });


            modelBuilder.Entity<icd_relation_mapping>(entity =>
            {
                entity.ToTable("icd_relation_mapping", schema: "public");
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<cptcodes>(entity =>
            {
                entity.ToTable("cpt_code", schema: "public");
                entity.HasKey(e => e.id);
            });


            modelBuilder.Entity<cpt_rules>(entity =>
            {
                entity.ToTable("cpt_rules", schema: "public");
                entity.HasKey(e => e.id);
            });
            modelBuilder.Entity<application_config>(entity =>
            {
                entity.ToTable("application_config", schema: "public");
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<icd_section_specificity>(entity =>
            {
                entity.ToTable("icd_section_specificity", schema: "public");
                entity.HasKey(e => e.id);
            });
            //modelBuilder.Entity<TOTP>(entity =>

            //{
            //    entity.ToTable("totp");
            //    entity.Property(e => e.Id).HasColumnName("Id");

            //    entity.Property(e => e.UserId)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //    entity.Property(e => e.UpdatedOn);

            //    entity.Property(e => e.UUId)
            //    .IsRequired();

            //    entity.Property(e => e.Status)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //    entity.Property(e => e.CreatedOn)
            //    .IsRequired();

            //    entity.Property(e => e.FactorId)
            //  .IsRequired();



            //});


            //modelBuilder.Entity<Security>(entity =>

            //{
            //    entity.ToTable("security");
            //    entity.Property(e => e.Id).HasColumnName("Id");

            //    entity.Property(e => e.UserName)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //    entity.Property(e => e.Password);


            //    entity.Property(e => e.Status)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //    entity.Property(e => e.IpAddress)
            //    .IsRequired();

            //    entity.Property(e => e.RetryCount).HasColumnName("REtryCount");

            //    entity.Property(e => e.Deatails);



            //});



            //modelBuilder.Entity<UserLog>(entity =>

            //{
            //    entity.ToTable("userlog");
            //    entity.Property(e => e.Id).HasColumnName("Id");

            //    entity.Property(e => e.Token)
            //    .HasMaxLength(100)
            //    .IsRequired();


            //    entity.Property(e => e.UpdatedOn);
            //    entity.Property(e => e.UserId);




            //});
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<chart_master>(entity =>
            {
                entity.ToTable("chart_master", schema: "public");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Sentence).HasColumnName("sentence");
                entity.Property(e => e.Unique_Id).HasColumnName("unique_id");
                entity.Property(e => e.Date_Of_Service).HasColumnName("date_of_service").HasColumnType("timestamp without time zone");
                entity.Property(e => e.Created_By).HasColumnName("created_by");
                entity.Property(e => e.Created_On).HasColumnName("created_on").HasColumnType("timestamp without time zone");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.ExpectedIcds).HasColumnName("expected_icds");
                entity.Property(e => e.Date_Of_Birth).HasColumnName("date_of_birth");
                entity.Property(e => e.Gender).HasColumnName("gender");
                entity.Property(e => e.CptCode).HasColumnName("cpt_code");
                entity.Property(e => e.Ref_No).HasColumnName("ref_no");

                entity.HasOne(d => d.Client_Uploads)
                .WithMany(p => p.chart_masters)
                .HasForeignKey(d => d.client_uploads_id)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.Organization)
               .WithMany(p => p.chart_master)
               .HasForeignKey(d => d.org_id)
               .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<chart_conditions_lineitem>(entity =>
            {
                entity.ToTable("chart_conditions_lineitem", schema: "public");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Chart_Master_Id).HasColumnName("chart_master_id");
                entity.Property(e => e.Condition).HasColumnName("condition");
                entity.Property(e => e.Icd_Code).HasColumnName("icd_code");
                entity.Property(e => e.Source_Type).HasColumnName("source_type");
                entity.Property(e => e.Created_By).HasColumnName("created_by");
                entity.Property(e => e.Created_On).HasColumnName("created_on").HasColumnType("timestamp without time zone");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.Sentence).HasColumnName("sentence");
                entity.Property(e => e.Rule_Id).HasColumnName("rule_id");
                entity.Property(e => e.Section).HasColumnName("section");

                entity.HasOne(d => d.chart_Master)
                 .WithMany(p => p.chart_Conditions_Lineitems)
                 .HasForeignKey(d => d.Chart_Master_Id)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.CptDetails)
                 .WithMany(p => p.chart_Conditions_Lineitems)
                 .HasForeignKey(d => d.cptDetailsId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<role_master>(entity =>
            {
                entity.ToTable("role_master", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();
                entity.Property(e => e.Role)
                    .HasColumnName("role");

                entity.Property(e => e.isdelete).HasColumnName("isdelete");
                entity.Property(e => e.createdby).HasColumnName("createdby");
                entity.Property(e => e.createdon).HasColumnName("createdon");
                entity.Property(e => e.modifiedby).HasColumnName("modifiedby");
                entity.Property(e => e.modifiedon).HasColumnName("modifiedon");
                entity.Property(e => e.createdon)
                    .HasColumnType("timestamp with time zone");
                entity.Property(e => e.modifiedon)
                    .HasColumnType("timestamp with time zone");

            });

            modelBuilder.Entity<user_organizatation_role>(entity =>
            {
                entity.ToTable("user_organizatation_role", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();
                entity.Property(e => e.OrganizatationId)
                    .HasColumnName("organizatation_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedOn).HasColumnName("createdon");
                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");
                entity.Property(e => e.ModifiedOn).HasColumnName("modifiedon");
                entity.Property(e => e.Target).HasColumnName("target");
                entity.Property(e => e.QA).HasColumnName("qa");
                entity.Property(e => e.IsDelete).HasColumnName("isdelete");
                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamp with time zone");
                entity.Property(e => e.ModifiedOn)
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(d => d.RoleMaster)
                .WithMany(p => p.user_Organizatation_Roles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Organization)
                .WithMany(p => p.user_Organizatation_Roles)
                .HasForeignKey(d => d.OrganizatationId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.User)
                .WithMany(p => p.user_Organizatation_Roles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<stopwords>(entity =>
            {
                entity.ToTable("stopwords", schema: "public");
                entity.HasNoKey();
            });

            modelBuilder.Entity<speciality>(entity =>
            {
                entity.ToTable("speciality", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();


                entity.HasOne(d => d.organization)
                .WithMany(p => p.specialityList)
                .HasForeignKey(d => d.clientid)
                .OnDelete(DeleteBehavior.Restrict);

            });

            //modelBuilder.Entity<staticconfiguration_code>(entity =>
            //{
            //    entity.ToTable("staticconfiguration");
            //    entity.HasKey(e => e.id);
            //});
            modelBuilder.Entity<client_uploads>(entity =>
            {
                entity.ToTable("client-uploads", schema: "public");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

                entity.HasOne(d => d.Organization)
                .WithMany(p => p.client_uploads)
                .HasForeignKey(d => d.org_id)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Speciality)
                .WithMany(p => p.client_Uploads)
                .HasForeignKey(d => d.specialties)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CptDetails>(entity =>
            {
                entity.ToTable("CptDetails", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();


                entity.HasOne(d => d.chart_Master)
                .WithMany(p => p.CptDetails)
                .HasForeignKey(d => d.chart_master_id)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Mod>(entity =>
            {
                entity.ToTable("mods", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();


            });

            modelBuilder.Entity<CptClassifyRules>(entity =>
            {
                entity.ToTable("CptClassifyRules", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();


            });

            modelBuilder.Entity<CptWithWord>(entity =>
            {
                entity.ToTable("cptwithword", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();


                entity.HasOne(d => d.CptClassifyRules)
                  .WithMany(p => p.WithWord)
                  .HasForeignKey(d => d.CptRuleId)
                  .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<LeftCptWithOutWord>(entity =>
            {
                entity.ToTable("leftcptwithoutword", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();

                entity.HasOne(d => d.CptClassifyRules)
                  .WithMany(p => p.WithOutWord)
                  .HasForeignKey(d => d.CptRuleId)
                  .OnDelete(DeleteBehavior.Restrict);




            });

            modelBuilder.Entity<MedicalNecessity>(entity =>
            {
                entity.ToTable("medicalnecessity", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();


            });

            modelBuilder.Entity<CptRuleWithoutWords>(entity =>
            {
                entity.ToTable("cptrulewithoutword", schema: "public");
                entity.Property(e => e.Id).HasColumnName("id").IsRequired();


            });

            modelBuilder.Entity<ErrorSummary>(entity =>
            {
                entity.ToTable("ErrorSummary", schema: "public");
                entity.Property(e => e.id).HasColumnName("Id").IsRequired();

                entity.HasOne(d => d.chart_Master)
                  .WithMany(p => p.ErrorSummaryList)
                  .HasForeignKey(d => d.chart_master_id)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MUED>(entity =>
            {
                entity.ToTable("mued", schema: "cure");
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    //.ValueGeneratedOnAdd() // ✅ Auto-Increment
                    .IsRequired();

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("created_on")
                    .HasDefaultValueSql("NOW()") // ✅ Default timestamp
                    .IsRequired();

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.ModifiedOn).HasColumnName("modified_on");
                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            });


            modelBuilder.Entity<LcdNotCoveredSource>(entity =>
            {

                entity.ToTable("lcdnotcoveredsource", schema: "cure"); // Mapping to PostgreSQL Table

                entity.HasKey(e => e.Id); // Primary Key

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd(); // Auto-increment

                entity.Property(e => e.LcdId)
                    .HasColumnName("lcd_id")
                    .HasColumnType("bigint");

                entity.Property(e => e.HcpcCodeId)
                    .HasColumnName("hcpc_code_id")
                    .HasColumnType("text");

                entity.Property(e => e.HcpcCodeGroup)
                    .HasColumnName("hcpc_code_group")
                    .HasColumnType("integer");

                entity.Property(e => e.ArticleId)
                    .HasColumnName("article_id")
                    .HasColumnType("varchar(100)")
                    .HasMaxLength(100);

                entity.Property(e => e.Icd10CodeId)
                    .HasColumnName("icd10_code_id")
                    .HasColumnType("text");

                entity.Property(e => e.Icd10NoncoveredGroup)
                    .HasColumnName("icd10_noncovered_group")
                    .HasColumnType("integer");

                entity.Property(e => e.Paragraph)
                    .HasColumnName("paragraph")
                    .HasColumnType("text");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("varchar(50)")
                    .HasMaxLength(50);

                entity.Property(e => e.StatusUpdateOn)
                    .HasColumnName("statusupdateon")
                    .HasColumnType("timestamp");

                entity.Property(e => e.HcpcParagraph)
                    .HasColumnName("hcpc_paragraph")
                    .HasColumnType("text");

                entity.Property(e => e.CompletedOn)
                    .HasColumnName("completed_on")
                    .HasColumnType("timestamptz"); // timestamp with time zone

                entity.Property(e => e.CompletedBy)
                    .HasColumnName("completed_by")
                    .HasColumnType("integer");

                entity.Property(e => e.AssignedOn)
                    .HasColumnName("assigned_on")
                    .HasColumnType("timestamptz");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("integer");

                entity.Property(e => e.AssignId)
                    .HasColumnName("assign_id")
                    .HasColumnType("bigint");
            });



            modelBuilder.Entity<LcdSourceData>(entity =>
            {
                entity.ToTable("lcdsourcedata", schema: "cure"); // Mapping to PostgreSQL Table

                entity.HasKey(e => e.id);

                entity.Property(e => e.id)
                    .HasColumnName("id") // Mapping Column Name
                    .IsRequired();

                entity.Property(e => e.LcdId)
                    .HasColumnName("lcd_id");
                entity.Property(e => e.HcpcCodeId)
                    .HasColumnName("hcpc_code_id")
                    .HasColumnType("text");

                entity.Property(e => e.HcpcCodeGroup)
                    .HasColumnName("hcpc_code_group")
                    .HasColumnType("text");

                entity.Property(e => e.ArticleId)
                    .HasColumnName("article_id");

                entity.Property(e => e.Icd10CodeId)
                    .HasColumnName("icd10_code_id")
                    .HasColumnType("text");

                entity.Property(e => e.Icd10CoveredGroup)
                    .HasColumnName("icd10_covered_group")
                    .HasColumnType("integer");

                entity.Property(e => e.Paragraph)
                    .HasColumnName("paragraph")
                    .HasColumnType("text");
            });


            modelBuilder.Entity<jwt_token>(entity =>
            {
                entity.ToTable("jwt_token", schema: "cure");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();


            });

            modelBuilder.Entity<userdaterangesampling>(entity =>
            {
                entity.ToTable("userdaterangesampling");

                entity.HasOne(d => d.User_Organizatation_Role)
                 .WithMany(p => p.userdaterangesamplings)
                 .HasForeignKey(d => d.user_organizatation_role_id)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<usertarget>(entity =>
            {
                entity.ToTable("usertarget");

                entity.HasOne(d => d.User_Organizatation_Role)
                 .WithMany(p => p.userDateRangeTarget)
                 .HasForeignKey(d => d.user_organizatation_role_id)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<targets>(entity =>
            {
                entity.ToTable("targets");

                entity.HasOne(d => d.usertarger)
                 .WithMany(p => p.Targets)
                 .HasForeignKey(d => d.TargetId)
                 .OnDelete(DeleteBehavior.Restrict);

            });



            modelBuilder.Entity<ncci_master_data>(entity =>
            {
                entity.ToTable("ncci_master_data", schema: "cure");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<article_lcd_user_assign>(entity =>
            {
                entity.ToTable("article_lcd_user_assign", schema: "cure");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });
            modelBuilder.Entity<ArticleLcdUserAssignNcovered>(entity =>
            {
                entity.ToTable("article_lcd_user_assign_ncovered", schema: "cure");
                entity.Property(e => e.id).HasColumnName("id").IsRequired();

            });

            modelBuilder.Entity<Mailverify>(entity =>

            {
                entity.ToTable("mailverify");
                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.userid)
                .HasMaxLength(100)
                .IsRequired();

                entity.Property(e => e.created_on);



            });

            modelBuilder.Entity<TypeOfDeliverable>(entity =>
            {
                entity.ToTable("TypeOfDeliverable");

            });

            modelBuilder.Entity<ClientOrganizations>(entity =>
            {
                entity.ToTable("ClientOrganizations");

            });


            modelBuilder.Entity<rule_inclusion>(entity =>
            {
                entity.ToTable("rule_inclusion", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created_on");

                entity.Property(e => e.IncludeId).HasColumnName("includes");

                entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modified_on");

                entity.Property(e => e.RuleId).HasColumnName("rule_id");
            });

            modelBuilder.Entity<patient_history>(entity =>
            {
                entity.ToTable("patient_history"); // DB table name

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();

                entity.Property(e => e.PatientId).HasColumnName("Patient ID");
                entity.Property(e => e.PatientName).HasColumnName("Patient Name");
                entity.Property(e => e.PatientFirstName).HasColumnName("Patient First Name");
                entity.Property(e => e.PatientDOB).HasColumnName("Patient DOB");
                entity.Property(e => e.PatientGender).HasColumnName("Patient Gender");
                entity.Property(e => e.PatientAge).HasColumnName("Patient Age");
                entity.Property(e => e.PatientLastName).HasColumnName("Patient Last Name");
                entity.Property(e => e.PrimaryInsuranceName).HasColumnName("Primary Insurance Name");
                entity.Property(e => e.PatientMiddleIntial).HasColumnName("Patient Middle Initial");
                entity.Property(e => e.Type).HasColumnName("Type");

            });

            modelBuilder.Entity<ClientTarget>(entity =>
            {
                entity.ToTable("ClientTarget", schema: "public");
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();


                entity.HasOne(d => d.clientOrganization)
                .WithMany(p => p.ClientTargetList)
                .HasForeignKey(d => d.clientOrgId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<cptcodewithoutword>(entity =>
            {
                entity.ToTable("cptcodewithoutword", schema: "public");
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();


                entity.HasOne(d => d.CptRules)
                .WithMany(p => p.CptCodeWithOutWord)
                .HasForeignKey(d => d.RuleId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ra_cpttaskwithrule>(entity =>
            {
                entity.ToTable("ra_cpttaskwithrule", schema: "public");

                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.ra_taskid).HasColumnName("ra_taskid");

                entity.Property(e => e.sourceid).HasColumnName("sourceid");

                entity.Property(e => e.sourcetype).HasColumnName("sourcetype");

                entity.Property(e => e.ruleno).HasColumnName("ruleno");

                entity.Property(e => e.createdby)
                    .HasColumnName("createdby");

                entity.Property(e => e.createdon)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createdon");

            });

            modelBuilder.Entity<cptcombination>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ex_Code1)
                .HasColumnName("ex_code1")
                   .HasMaxLength(45);

                entity.Property(e => e.Ex_Code2)
                 .HasColumnName("ex_code2")
                   .HasMaxLength(45);

                entity.Property(e => e.Ex_Code3)
                 .HasColumnName("ex_code3")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code1)
                 .HasColumnName("com_code1")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code2)
                 .HasColumnName("com_code2")
                   .HasMaxLength(45);

                entity.Property(e => e.Com_Code3)
                 .HasColumnName("com_code3")
                   .HasMaxLength(45);

                entity.Property(e => e.CreatedOn)
                .HasColumnName("createdon").HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                 .HasColumnName("createdby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn)
                 .HasColumnName("modifiedon")
                .HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                .HasColumnName("modifiedby")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.effectivedate)
                 .HasColumnName("effectivedate")
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.termdate)
                 .HasColumnName("termdate")
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.IsDelete)
       .HasColumnName("isdelete");

            });

            modelBuilder.Entity<cptmanualexcludecode>(entity =>
            {
                entity.ToTable("cptmanualexcludecode", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.CptCode)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("cptcode");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.RemovedCPT)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("removedcpt");
            });

            modelBuilder.Entity<cptmanualincludecode>(entity =>
            {
                entity.ToTable("cptmanualincludecode", schema: "public");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalCpt)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("additionalcpt");

                entity.Property(e => e.CreatedBy).HasColumnName("createdby");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("createddate");

                entity.Property(e => e.CptCode)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("cptcode");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("modifieddate");
            });

            modelBuilder.Entity<cpt_termtoterm>(entity =>
            {
                entity.ToTable("cpt_termtoterm", schema: "public");
                entity.Property(e => e.id).HasColumnName("id").ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<charge_capture>(entity =>
            {
                entity.ToTable("charge_capture", schema: "public");
                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.provider)
                .HasMaxLength(100);
              

                entity.Property(e => e.practice)
               .HasMaxLength(100);

                entity.Property(e => e.patient_name)
                .HasMaxLength(100);


                entity.Property(e => e.patient_id)
                .HasMaxLength(50);


                entity.Property(e => e.claim_id)
                .HasMaxLength(50);


                entity.Property(e => e.encounter_id)
                .HasMaxLength(50);

                entity.Property(e => e.cpt)
                .HasMaxLength(100);
               

                entity.Property(e => e.dos)
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.claim_date)
                .HasColumnType("timestamp without time zone");

                entity.Property(e => e.created_on)
                .HasColumnType("timestamp without time zone");


                entity.Property(e => e.isdelete)
       .HasColumnName("isdelete");

                entity.Property(e => e.file_name)
               .HasMaxLength(100);
                entity.Property(e => e.state)
               .HasMaxLength(100);
                entity.Property(e => e.org_id)
               .HasMaxLength(10);

                entity.Property(e => e.location)
              .HasMaxLength(100);

            });

        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
