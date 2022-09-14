using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace CityApp.Web.Infrastructure
{
    public interface IDeploymentEnvironment
    {
        string DeploymentId { get; }
    }

    /// <summary>
    /// Thanks, Microsoft: https://github.com/aspnet/live.asp.net
    /// </summary>
    public class DeploymentEnvironment : IDeploymentEnvironment
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<DeploymentEnvironment>();

        private readonly string _contentRoot;
        private string _commitSha;

        public DeploymentEnvironment(IHostingEnvironment hostingEnv)
        {
            _contentRoot = hostingEnv.ContentRootPath;
        }

        public string DeploymentId
        {
            get
            {
                if (_commitSha == null)
                {
                    LoadCommitSha();
                }

                return _commitSha;
            }
        }

        private void LoadCommitSha()
        {
            var kuduActiveDeploymentPath = Path.GetFullPath(Path.Combine(_contentRoot, "..", "deployments", "active"));
            try
            {
                if (File.Exists(kuduActiveDeploymentPath))
                {
                    _logger.Debug("Kudu active deployment file found, using it to set DeploymentID");
                    _commitSha = $"{File.ReadAllText(kuduActiveDeploymentPath)} (kudu)";
                }
                else
                {
                    _logger.Debug("Kudu active deployment file not found, using git to set DeploymentID");

                    var git = Process.Start(new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = "rev-parse HEAD",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    });

                    var gitOut = "";
                    while (!git.StandardOutput.EndOfStream)
                    {
                        gitOut += git.StandardOutput.ReadLine();
                    }

                    gitOut += " (local)";

                    git.WaitForExit();

                    if (git.ExitCode != 0)
                    {
                        _logger.Debug("Problem using git to set deployment ID:\r\n  git exit code: {0}\r\n git output: {1}", git.ExitCode, _commitSha);
                        _commitSha = "(Could not determine deployment ID)";
                    }
                    else
                    {
                        _commitSha = gitOut;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error determining deployment ID");
                _commitSha = "(Error determining deployment ID)";
            }
        }
    }
}
