using Microsoft.EntityFrameworkCore;
using KnowledgeTestingSystem.Web.Data.Models;

namespace KnowledgeTestingSystem.Web.Data;

/// <summary>
/// Единый DbContext для всей системы KnowledgeTestingSystem.
/// Объединяет сущности из ранее разделённых БД: usersandalgos + users.
/// </summary>
public partial class KnowledgeTestingDbContext : DbContext
{
    public KnowledgeTestingDbContext(DbContextOptions<KnowledgeTestingDbContext> options)
        : base(options)
    {
    }

    // === Task/Algo entities (из usersandalgos) ===
    public virtual DbSet<Algorithm> Algorithms { get; set; }
    public virtual DbSet<AlgorithmStep> AlgorithmSteps { get; set; }
    public virtual DbSet<Test> Tests { get; set; }
    public virtual DbSet<TestInputData> TestInputData { get; set; }
    public virtual DbSet<TrackedVariable> TrackedVariables { get; set; }
    public virtual DbSet<TestStepResponse> TestStepResponses { get; set; }

    // === User entities (из users) ===
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<Session> Sessions { get; set; }
    public virtual DbSet<SessionTest> SessionTests { get; set; }
    public virtual DbSet<Grade> Grades { get; set; }
    public virtual DbSet<SolutionsByUser> SolutionsByUsers { get; set; }
    public virtual DbSet<SolutionsByProgram> SolutionsByPrograms { get; set; }
    public virtual DbSet<VariablesSolutionsByUser> VariablesSolutionsByUsers { get; set; }
    public virtual DbSet<VariablesSolutionsByProgram> VariablesSolutionsByPrograms { get; set; }
    public virtual DbSet<UserTestAbility> UserTestAbilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =============================================
        // === ALGORITHMS & TESTS (usersandalgos tables) ===
        // =============================================

        // Algorithm
        modelBuilder.Entity<Algorithm>(entity =>
        {
            entity.HasKey(e => e.AlgoId);
            entity.ToTable("algorithms");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.SrcPath).HasColumnName("src_path");
            entity.Property(e => e.PicPath).HasColumnName("pic_path").HasDefaultValueSql("''::text");
            entity.Property(e => e.AlgoName).HasColumnName("algo_name").HasDefaultValueSql("''::text");
        });

        // AlgorithmStep
        modelBuilder.Entity<AlgorithmStep>(entity =>
        {
            entity.HasKey(e => new { e.AlgoStep, e.AlgoId });
            entity.ToTable("algorithmsteps");
            entity.HasIndex(e => new { e.AlgoId, e.AlgoStep }, "AK_algorithmsteps_algo_id_algo_step").IsUnique();

            entity.Property(e => e.AlgoStep).HasColumnName("algo_step");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficult).HasColumnName("difficult");

            entity.HasOne(d => d.Algo).WithMany(p => p.AlgorithmSteps).HasForeignKey(d => d.AlgoId);
        });

        // Test
        modelBuilder.Entity<Test>(entity =>
        {
            entity.ToTable("tests");
            entity.HasIndex(e => e.AlgoId, "IX_tests_algo_id");

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Difficult).HasColumnName("difficult");
            entity.Property(e => e.SolvedCount).HasColumnName("solved_count");
            entity.Property(e => e.TestName).HasColumnName("test_name");
            entity.Property(e => e.UnsolvedCount).HasColumnName("unsolved_count");

            entity.HasOne(d => d.Algo).WithMany(p => p.Tests).HasForeignKey(d => d.AlgoId);
        });

        // TestInputData
        modelBuilder.Entity<TestInputData>(entity =>
        {
            entity.HasKey(e => new { e.TestId, e.VarName });
            entity.ToTable("testinputdata");

            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.VarType).HasColumnName("var_type");
            entity.Property(e => e.VarValue).HasColumnName("var_value");
        });

        // TrackedVariable
        modelBuilder.Entity<TrackedVariable>(entity =>
        {
            entity.HasKey(e => new { e.Sequence, e.AlgoId, e.AlgoStep });
            entity.ToTable("trackedvariables");
            entity.HasIndex(e => new { e.AlgoId, e.AlgoStep }, "IX_trackedvariables_algo_id_algo_step").IsUnique();

            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.AlgoStep).HasColumnName("algo_step");
            entity.Property(e => e.LineNumber).HasColumnName("line_number");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.VarType).HasColumnName("var_type");

            entity.HasOne(d => d.AlgorithmStep).WithMany(p => p.TrackedVariables)
                .HasPrincipalKey(p => new { p.AlgoId, p.AlgoStep })
                .HasForeignKey(d => new { d.AlgoId, d.AlgoStep });
        });

        // TestStepResponse
        modelBuilder.Entity<TestStepResponse>(entity =>
        {
            entity.HasKey(e => new { e.TestId, e.AlgoStep, e.AlgoId });
            entity.ToTable("teststepresponses");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.AlgoStep).HasColumnName("algo_step");
            entity.Property(e => e.AlgoId).HasColumnName("algo_id");
            entity.Property(e => e.CorrectCount).HasColumnName("correct_count");
            entity.Property(e => e.IncorrectCount).HasColumnName("incorrect_count");
        });

        // =============================================
        // === USERS & SESSIONS (users tables) ===
        // =============================================

        // Group
        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("groups");
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(e => e.GroupId, "IX_Users_group_id");
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IconPath).HasColumnName("icon_path");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.Group).WithMany(p => p.Users).HasForeignKey(d => d.GroupId);
        });

        // Session
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(e => e.SessionId);

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.DateFinish).HasColumnName("date_finish");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.Difficult).HasColumnName("difficult");
            entity.Property(e => e.SessionType).HasColumnName("session_type");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasMany(d => d.Groups).WithMany(p => p.Sessions)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupSession",
                    r => r.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                    l => l.HasOne<Session>().WithMany().HasForeignKey("SessionId"),
                    j =>
                    {
                        j.HasKey("SessionId", "GroupId");
                        j.ToTable("groupsessions");
                        j.HasIndex(new[] { "GroupId" }, "IX_GroupSessions_group_id");
                        j.IndexerProperty<int>("SessionId").HasColumnName("session_id");
                        j.IndexerProperty<int>("GroupId").HasColumnName("group_id");
                    });
        });

        // SessionTest
        modelBuilder.Entity<SessionTest>(entity =>
        {
            entity.ToTable("sessiontests");
            entity.HasKey(e => new { e.SessionId, e.TestId });

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.Session).WithMany(p => p.SessionTests).HasForeignKey(d => d.SessionId);
        });

        // Grade
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("grades");
            entity.HasKey(e => new { e.UserId, e.SessionId });
            entity.HasIndex(e => e.SessionId, "IX_Grades_session_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Datetime).HasColumnName("datetime");
            entity.Property(e => e.Mark).HasColumnName("mark");

            entity.HasOne(d => d.Session).WithMany(p => p.Grades).HasForeignKey(d => d.SessionId);
            entity.HasOne(d => d.User).WithMany(p => p.Grades).HasForeignKey(d => d.UserId);
        });

        // SolutionsByUser
        modelBuilder.Entity<SolutionsByUser>(entity =>
        {
            entity.ToTable("solutionsbyusers");
            entity.HasKey(e => new { e.SessionId, e.UserId, e.UserStep, e.UserLineNumber, e.OrderNumber, e.TestId });
            entity.HasIndex(e => e.UserId, "IX_SolutionsByUsers_UserId");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserStep).HasColumnName("user_step");
            entity.Property(e => e.UserLineNumber).HasColumnName("user_line_number");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.StepDifficult).HasColumnName("step_difficult");

            entity.HasOne(d => d.Session).WithMany(p => p.SolutionsByUsers).HasForeignKey(d => d.SessionId);
            entity.HasOne(d => d.User).WithMany(p => p.SolutionsByUsers).HasForeignKey(d => d.UserId);
        });

        // SolutionsByProgram
        modelBuilder.Entity<SolutionsByProgram>(entity =>
        {
            entity.ToTable("solutionsbyprograms");
            entity.HasKey(e => new { e.SessionId, e.TestId, e.ProgramStep, e.ProgramLineNumber, e.OrderNumber });

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.ProgramStep).HasColumnName("program_step");
            entity.Property(e => e.ProgramLineNumber).HasColumnName("program_line_number");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.StepDifficult).HasColumnName("step_difficult");

            entity.HasOne(d => d.Session).WithMany(p => p.SolutionsByPrograms).HasForeignKey(d => d.SessionId);
        });

        // VariablesSolutionsByUser
        modelBuilder.Entity<VariablesSolutionsByUser>(entity =>
        {
            entity.ToTable("variables_solutions_by_users");
            entity.HasKey(e => new { e.UserStep, e.UserLineNumber, e.OrderNumber, e.TestId, e.VarName });

            entity.Property(e => e.UserStep).HasColumnName("user_step");
            entity.Property(e => e.UserLineNumber).HasColumnName("user_line_number");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.VarValue).HasColumnName("var_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        // VariablesSolutionsByProgram
        modelBuilder.Entity<VariablesSolutionsByProgram>(entity =>
        {
            entity.ToTable("variables_solutions_by_programs");
            entity.HasKey(e => new { e.ProgramStep, e.ProgramLineNumber, e.OrderNumber, e.TestId, e.VarName });

            entity.Property(e => e.ProgramStep).HasColumnName("program_step");
            entity.Property(e => e.ProgramLineNumber).HasColumnName("program_line_number");
            entity.Property(e => e.OrderNumber).HasColumnName("order_number");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.VarName).HasColumnName("var_name");
            entity.Property(e => e.VarValue).HasColumnName("var_value");
        });

        // UserTestAbility
        modelBuilder.Entity<UserTestAbility>(entity =>
        {
            entity.ToTable("usertestabilities");
            entity.HasKey(e => new { e.UserId, e.TestId });

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.Ability).HasColumnName("ability");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
