using DataBaseConnection.CommonDatabaseConnection.Tables.Account;
using System.Data.Entity;

namespace SQLite.CodeFirst
{
    //public class ModelConfiguration
    //{
    //    public static void Configure(DbModelBuilder modelBuilder)
    //    {
    //       ConfigureAccountdataBaseEntity(modelBuilder);
    //    }

    //    #region Commented
    //    //private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
    //    //{
    //    //    modelBuilder.Entity<Team>().ToTable("Base.MyTable")
    //    //        .HasRequired(t => t.Coach)
    //    //        .WithMany()
    //    //        .WillCascadeOnDelete(false);

    //    //    modelBuilder.Entity<Team>()
    //    //        .HasRequired(t => t.Stadion)
    //    //        .WithRequiredPrincipal()
    //    //        .WillCascadeOnDelete(true);
    //    //} 
    //    #endregion

    //    public static void ConfigureAccountdataBaseEntity(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<FeedInfoes>();
    //        modelBuilder.Entity<Friendships>();
    //        modelBuilder.Entity<DailyStatitics>();
    //        modelBuilder.Entity<InteractedPosts>();
    //        modelBuilder.Entity<InteractedUsers>();
    //        modelBuilder.Entity<UnfollowedUsers>();
    //    }

    //}
}
