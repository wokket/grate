using System;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;

    private const string Bug232Sql = @"
ALTER DATABASE {{DatabaseName}} SET READ_COMMITTED_SNAPSHOT ON;
GO
ALTER DATABASE {{DatabaseName}} SET ALLOW_SNAPSHOT_ISOLATION ON;
GO";

    [Test]
    public async Task Bug232_Timeout_14_Regression()
    {
        // V1.4 regressed something, trying to repro

        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[RunAfterCreateDatabase]?.Path ?? throw new Exception("Config Fail")));

        WriteSql(path, "token.sql", Bug232Sql);

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        // Now drop it and do it again
        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Drop = true
        };

        await using (var migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }

    }


}
