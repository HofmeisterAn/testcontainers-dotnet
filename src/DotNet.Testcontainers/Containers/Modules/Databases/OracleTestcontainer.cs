namespace DotNet.Testcontainers.Containers
{
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class OracleTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal OracleTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => $"Data Source={this.Hostname}:{this.Port};User id={this.Username};Password={this.Password};";

    /// <summary>
    /// Executes a SQL script in the database container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to be executed.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    public override async Task<ExecResult> ExecScriptAsync(string scriptContent)
    {
      var tempScriptFile = this.GetTempScriptFile();
      await this.CopyFileAsync(tempScriptFile, Encoding.ASCII.GetBytes(scriptContent));
      var execScriptCommand = $"exit | $ORACLE_HOME/bin/sqlplus -S {this.Username}/{this.Password}@{this.Hostname} @{tempScriptFile}";
      return await this.ExecAsync(new[] { "/bin/sh", "-c", execScriptCommand });
    }
  }
}
