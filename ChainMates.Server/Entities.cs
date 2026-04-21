
using ChainMates.Server;
using ChainMates.Server.DTOs.Notification.Info;
using EFCore.NamingConventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Xml.Linq;

namespace ChainMates.Server
{
    public class Author 
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public List<Story> Stories { get; set; } = new();
        public List<Segment> Segments { get; set; } = new();
        public List<ModerationAssignment> ModerationAssignments { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

        public List<AuthorRelation> PrimaryRelations { get; set; }
        public List<AuthorRelation> SecondaryRelations { get; set; }
        public List<CircleAssignment> CircleAssignments { get; set; } = new();

        public List<Notification> ReceivedNotifications { get; set; } = new();
        //public List<Notification> InstigatedNotifications { get; set; } = new();
    }
    public class Story
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int? CircleId { get; set; }
        public string? Title { get; set; } = "";
        public int? MaxSegments { get; set; } = null;
        public int? MaxSegmentLength { get; set; } = null;
        public int? MinSegmentLength { get; set; } = null;
        public int? MaxBranches { get; set; } = null;
        public bool? IsItMature { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public Author Author { get; set; }
        public Circle? Circle { get; set; }
        public List<Segment> Segments { get; set; } = new();
        public List<StoryComment> Comments { get; set; } = new();

    }
    public class Segment
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";
        public int SegmentStatusId { get; set; } = 1;
        public int AuthorId { get; set; }
        public int StoryId { get; set; }
        public int? PreviousSegmentId { get; set; } = null;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public SegmentStatus SegmentStatus { get; set; }
        public Author Author { get; set; }
        public Story Story { get; set; }
        public Segment? PreviousSegment { get; set; }
        public List<Segment> FollowingSegments { get; set; } = new();
        public List<ModerationAssignment> ModerationAssignments { get; set; } = new();
        public List<SegmentComment> Comments { get; set; } = new();
    }
    public class SegmentStatus
    {
        public int Id { get; set; }
        public string description { get; set; }
        public List<Segment> Segments { get; set; }
    }
    public class ModerationAssignment
    {
        public int AuthorId { get; set; }
        public int SegmentId { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Author Author { get; set; }
        public Segment Segment { get; set; }
    }
    public class Comment
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public int CommentTypeId { get; set; }
        public int CommentStatusId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Author Author { get; set; }
        public CommentType CommentType { get; set; }
        public CommentStatus CommentStatus { get; set; }
        public StoryComment? StoryComment { get; set; }
        public SegmentComment? SegmentComment { get; set; }
        public CommentComment? CommentComment { get; set; }
        public List<CommentComment> ChildComments { get; set; }
    }
    public class CommentStatus
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
    }
    public class CommentType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<Comment> Comments { get; set; }
    }
    public class StoryComment
    {
        public int CommentId { get; set; }
        public int CommentTypeId { get; set; }
        public int ParentStoryId { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> ChildComments { get; set; }
        public Story ParentStory { get; set; }
    }
    public class SegmentComment
    {

        public int CommentId { get; set; }
        public int CommentTypeId { get; set; }
        public int ParentSegmentId { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> ChildComments { get; set; }
        public Segment ParentSegment { get; set; }
    }
    public class CommentComment
    {
        public int CommentId { get; set; }
        public int CommentTypeId { get; set; }
        public int ParentCommentId { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> ChildComments { get; set; }
        public Comment ParentComment { get; set; }
    }

    public class SegmentTrace
    {
        public int FinalSegmentId { get; set; }
        public int FinalAuthorId { get; set; }
        public int FinalSegmentStatusId { get; set; }
        public int EarlierSegmentId { get; set; }
        public int EarlierSegmentContent { get; set; }
        public int EarlierSegmentAuthorId { get; set; }

    }

    public class JoinableSegmentByAuthor
    {
        public int AuthorId { get; set; }
        public int SegmentId { get; set; }
    }

    public class ModeratableSegmentByAuthor
    {
        public int AuthorId { get; set; }
        public int SegmentId { get; set; }
    }

    public class AuthorRelation { 
        public int AuthorId { get; set; }
        public int RelatedAuthorId { get; set; }
        public int AuthorRelationTypeId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Author Author { get; set; }
        public Author RelatedAuthor { get; set; }
        public AuthorRelationType AuthorRelationType { get; set; }
    }

    public class AuthorRelationType     {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<AuthorRelation> AuthorRelations { get; set; }
    }

    public class Circle {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public List<CircleAssignment> CircleAssignments { get; set; }

        public List<Story> Stories { get; set; }
    }

    public class CircleAssignment {
        public int CircleId { get; set; }
        public int AuthorId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Circle Circle { get; set; }
        public Author Author { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public int NotificationTypeId { get; set; }
        public int RecipientAuthorId { get; set; }
        //public int InstigatorAuthorId { get; set; }
        public JsonDocument Info { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public NotificationType NotificationType { get; set; }
        public Author RecipientAuthor { get; set; }
        //public Author InstigatorAuthor { get; set; }

    }

    public class NotificationType
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public List<Notification> Notifications { get; set; } = new();

        
        }
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Author { get; set; }
        public DbSet<Story> Story { get; set; }
        public DbSet<Segment> Segment { get; set; }
        public DbSet<SegmentStatus> SegmentStatus { get; set; }
        public DbSet<ModerationAssignment> ModerationAssignment { get; set; }


        public DbSet<SegmentTrace> SegmentTrace { get; set; }
        public DbSet<JoinableSegmentByAuthor> JoinableSegmentByAuthor { get; set; }
        public DbSet<ModeratableSegmentByAuthor> ModeratableSegmentByAuthor { get; set; }


        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentStatus> CommentStatus { get; set; }
        public DbSet<CommentType> CommentType { get; set; }
        public DbSet<StoryComment> StoryComment { get; set; }
        public DbSet<SegmentComment> SegmentComment { get; set; }
        public DbSet<CommentComment> CommentComment { get; set; }


        public DbSet<AuthorRelation> AuthorRelation { get; set; }
        public DbSet<AuthorRelationType> AuthorRelationType { get; set; }


        public DbSet<Circle> Circle { get; set; }
        public DbSet<CircleAssignment> CircleAssignment { get; set; }


        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationType> NotificationType { get; set; }

        //public DbSet<SegmentCommentBySegment> SegmentCommentBySegment { get; set; }
        //public DbSet<SegmentCommentByComment> SegmentCommentByComment { get; set; }

        //public DbSet<CommentCommentBySegment> CommentCommentBySegment { get; set; }

        //public DbSet<SegmentCommentCommentByComment> SegmentCommentCommentByComment { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = "Host=localhost;Database=postgres;Username=postgres;Password=postgres";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            dataSourceBuilder.EnableDynamicJson();

            var dataSource = dataSourceBuilder.Build();

            options.UseNpgsql(dataSource)
                   .UseSnakeCaseNamingConvention();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("chain_mates");

            modelBuilder.Entity<Author>();

            modelBuilder.Entity<Story>(
                nestedBuilder =>
                {
                    nestedBuilder
                        .HasOne(s => s.Author)
                        .WithMany(a => a.Stories)
                        .HasForeignKey(s => s.AuthorId)
                        .OnDelete(DeleteBehavior.Cascade);

                    nestedBuilder
                        .HasOne(s => s.Circle)
                        .WithMany(a => a.Stories)
                        .HasForeignKey(s => s.CircleId)
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Cascade);

                });

            modelBuilder.Entity<Segment>(
                nestedBuilder =>
                {
                    nestedBuilder
                        .HasOne(s => s.Author)
                        .WithMany(a => a.Segments)
                        .HasForeignKey(s => s.AuthorId)
                        .OnDelete(DeleteBehavior.Cascade);

                    nestedBuilder
                        .HasOne(s => s.Story)
                        .WithMany(st => st.Segments)
                        .HasForeignKey(s => s.StoryId)
                        .OnDelete(DeleteBehavior.Cascade);

                    nestedBuilder
                        .HasOne(s => s.PreviousSegment)
                        .WithMany(s2 => s2.FollowingSegments)
                        .HasForeignKey(s => s.PreviousSegmentId)
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Restrict);

                    nestedBuilder
                        .HasOne(s => s.SegmentStatus)
                        .WithMany(ss => ss.Segments)
                        .HasForeignKey(s => s.SegmentStatusId)
                        .OnDelete(DeleteBehavior.Restrict);
                });


             modelBuilder.Entity<Comment>(
                 nestedBuilder =>
                 {
                    nestedBuilder
                        .HasKey(c => c.Id);

                    nestedBuilder
                        .HasAlternateKey(c => new { c.Id, c.CommentTypeId });

                    nestedBuilder
                        .HasOne(c => c.Author)
                        .WithMany(a => a.Comments)
                        .HasForeignKey(c => c.AuthorId)
                        .OnDelete(DeleteBehavior.Restrict);

                    nestedBuilder
                        .HasOne(c => c.CommentStatus)
                        .WithMany(cs => cs.Comments)
                        .HasForeignKey(c => c.CommentStatusId)
                        .OnDelete(DeleteBehavior.Restrict);

                    nestedBuilder
                        .HasOne(c => c.CommentType)
                        .WithMany(ct => ct.Comments)
                        .HasForeignKey(c => c.CommentTypeId)
                        .OnDelete(DeleteBehavior.Restrict);
                        });

             modelBuilder.Entity<StoryComment>(
                nestedBuilder =>
                {
                    nestedBuilder
                        .HasKey(sc => new { sc.CommentId, sc.CommentTypeId });

                    nestedBuilder
                        .HasOne(sc => sc.ParentStory)
                        .WithMany(s => s.Comments)
                        .HasForeignKey(sc => sc.ParentStoryId)
                        .OnDelete(DeleteBehavior.Restrict);

                    nestedBuilder
                        .HasOne(sc => sc.Comment)
                        .WithOne(c => c.StoryComment)
                        .HasPrincipalKey<Comment>(c => new { c.Id, c.CommentTypeId })
                        .HasForeignKey<StoryComment>(sc => new { sc.CommentId, sc.CommentTypeId })
                        .IsRequired()
                        .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<SegmentComment>(
                    nestedBuilder =>
                    {
                        nestedBuilder
                            .HasKey(sc => new { sc.CommentId, sc.CommentTypeId });

                        nestedBuilder
                            .HasOne(sc => sc.ParentSegment)
                            .WithMany(s => s.Comments)
                            .HasForeignKey(sc => sc.ParentSegmentId)
                            .OnDelete(DeleteBehavior.Restrict);

                        nestedBuilder
                            .HasOne(sc => sc.Comment)
                            .WithOne(c => c.SegmentComment)
                            .HasPrincipalKey<Comment>(c => new { c.Id, c.CommentTypeId })
                            .HasForeignKey<SegmentComment>(sc => new { sc.CommentId, sc.CommentTypeId })
                            .IsRequired()
                            .OnDelete(DeleteBehavior.Restrict);
                    });

                modelBuilder.Entity<CommentComment>(
                    nestedBuilder =>
                    {
                        nestedBuilder
                            .HasKey(cc => new { cc.CommentId, cc.CommentTypeId });

                        nestedBuilder
                            .HasOne(cc => cc.ParentComment)
                            .WithMany(c => c.ChildComments)
                            .HasForeignKey(cc => cc.CommentId)
                            .OnDelete(DeleteBehavior.Restrict);

                        nestedBuilder
                            .HasOne(sc => sc.Comment)
                            .WithOne(c => c.CommentComment)
                            .HasPrincipalKey<Comment>(c => new { c.Id, c.CommentTypeId })
                            .HasForeignKey<CommentComment>(sc => new { sc.CommentId, sc.CommentTypeId })
                            .IsRequired()
                            .OnDelete(DeleteBehavior.Restrict);
                    });

                modelBuilder.Entity<ModerationAssignment>(
                    nestedBuilder =>
                    {
                        nestedBuilder
                            .HasKey(ma => new { ma.AuthorId, ma.SegmentId });

                        nestedBuilder
                            .HasOne(ma => ma.Segment)
                            .WithMany(s => s.ModerationAssignments)
                            .HasForeignKey(ma => ma.SegmentId);

                        nestedBuilder
                            .HasOne(ma => ma.Author)
                            .WithMany(s => s.ModerationAssignments)
                            .HasForeignKey(ma => ma.AuthorId);
                    });

                    modelBuilder.Entity<AuthorRelation>(
                        nestedBuilder =>
                        {
                            nestedBuilder
                            .HasKey(ar => new { ar.AuthorId, ar.RelatedAuthorId });

                            nestedBuilder
                            .HasOne(ar => ar.Author)
                            .WithMany(a => a.PrimaryRelations)
                            .HasForeignKey(ar => ar.AuthorId)
                            .OnDelete(DeleteBehavior.Restrict);

                            nestedBuilder
                             .HasOne(ar => ar.RelatedAuthor)
                             .WithMany(a => a.SecondaryRelations)
                             .HasForeignKey(ar => ar.RelatedAuthorId)
                             .OnDelete(DeleteBehavior.Restrict);

                            nestedBuilder
                             .HasOne(ar => ar.AuthorRelationType)
                             .WithMany(art => art.AuthorRelations)
                             .HasForeignKey(ar => ar.AuthorRelationTypeId)
                             .OnDelete(DeleteBehavior.Restrict);
                        });

                    modelBuilder.Entity<CircleAssignment>(
                        nestedBuilder =>
                        {
                            nestedBuilder
                                .HasKey(ca => new { ca.CircleId, ca.AuthorId });
                            
                            nestedBuilder
                                .HasOne(cm => cm.Circle)
                                .WithMany(c => c.CircleAssignments)
                                .HasForeignKey(cm => cm.CircleId);

                            nestedBuilder
                                .HasOne(cm => cm.Author)
                                .WithMany(a => a.CircleAssignments)
                                .HasForeignKey(cm => cm.CircleId);
                        });

                    modelBuilder.Entity<Notification>(
                            nestedBuilder =>
                            {
                                nestedBuilder
                                    .Property(n => n.Info)
                                    .HasColumnType("jsonb");

                                nestedBuilder
                                    .HasOne(n => n.NotificationType)
                                    .WithMany(nt => nt.Notifications)
                                    .HasForeignKey(n => n.NotificationTypeId);

                                nestedBuilder
                                    .HasOne(n => n.RecipientAuthor)
                                    .WithMany(ra => ra.ReceivedNotifications)
                                    .HasForeignKey(n => n.RecipientAuthorId);

                                //nestedBuilder
                                //    .HasOne(n => n.InstigatorAuthor)
                                //    .WithMany(ia => ia.InstigatedNotifications)
                                //    .HasForeignKey(n => n.InstigatorAuthorId);
                            });
                

                    modelBuilder.Entity<SegmentTrace>()
                        .HasNoKey()
                        .ToView("segment_trace");

                    modelBuilder.Entity<JoinableSegmentByAuthor>()
                        .HasNoKey()
                        .ToView("joinable_segment_by_author");

                    modelBuilder.Entity<ModeratableSegmentByAuthor>()
                        .HasNoKey()
                        .ToView("moderatable_segment_by_author");

                    //        modelBuilder.Entity<SegmentCommentBySegment>()
                    //.HasNoKey();

                    //        modelBuilder.Entity<SegmentCommentByComment>()
                    //.HasNoKey();

                    //        modelBuilder.Entity<CommentCommentBySegment>()
                    //.HasNoKey();

                    //        modelBuilder.Entity<SegmentCommentCommentByComment>()
                    //.HasNoKey();
                }
        }
    }