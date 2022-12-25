using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";

    [Test]
    public async Task Bug283_Syntax_Error_On_Create()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]?.Path ?? throw new Exception("Config Fail")));
        var config = Context.GetConfiguration(db, parent, knownFolders);

        WriteSql(path, "script0001.tables.sql", Bug283ReproSql);

        await using var migrator = Context.GetMigrator(config);
        await migrator.Migrate();
    }

    private static string Bug283ReproSql => @"
CREATE TABLE actor (
actor_id numeric NOT NULL ,
first_name VARCHAR(45) NOT NULL,
last_name VARCHAR(45) NOT NULL,
last_update DATE NOT NULL,
CONSTRAINT pk_actor PRIMARY KEY (actor_id)
);";
}
