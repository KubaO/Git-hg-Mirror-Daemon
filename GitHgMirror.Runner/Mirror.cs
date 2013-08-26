﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitHgMirror.CommonTypes;

namespace GitHgMirror.Runner
{
    class Mirror : IDisposable
    {
        private readonly Settings _settings;
        private readonly EventLog _eventLog;

        private readonly CommandRunner _commandRunner = new CommandRunner();


        public Mirror(Settings settings, EventLog eventLog)
        {
            _settings = settings;
            _eventLog = eventLog;
        }


        public void MirrorRepositories(MirroringConfiguration configuration)
        {
            try
            {
                var cloneDirectoryPath = Path.Combine(_settings.RepositoriesDirectoryPath, ToDirectoryName(configuration.HgCloneUri) + " - " + ToDirectoryName(configuration.GitCloneUri));
                var quotedHgCloneUrl = configuration.HgCloneUri.ToString().EncloseInQuotes();
                var quotedGitCloneUrl = configuration.GitCloneUri.ToString().EncloseInQuotes();

                if (!Directory.Exists(cloneDirectoryPath))
                {
                    Directory.CreateDirectory(cloneDirectoryPath);
                    RunCommandAndLogOutput("hg clone --noupdate " + quotedHgCloneUrl + " " + cloneDirectoryPath.EncloseInQuotes() + "");
                }

                RunCommandAndLogOutput("cd \"" + cloneDirectoryPath + "\"");
                RunCommandAndLogOutput(Path.GetPathRoot(cloneDirectoryPath).Replace("\\", string.Empty)); // Changing directory to other drive if necessary
                
                if (configuration.Direction == MirroringDirection.GitToHg)
                {
                    RunCommandAndLogOutput("hg pull " + quotedGitCloneUrl);
                    RunCommandAndLogOutput("hg push " + quotedHgCloneUrl);
                }
            }
            catch (CommandException ex)
            {
                throw new MirroringException(String.Format("An exception occured while mirroring the repositories {0} and {1} in direction {2}", configuration.HgCloneUri, configuration.GitCloneUri, configuration.Direction), ex);
            }
        }

        public void Dispose()
        {
            _commandRunner.Dispose();
        }


        private void RunCommandAndLogOutput(string command)
        {
            _eventLog.WriteEntry(_commandRunner.RunCommand(command));
        }


        private static string ToDirectoryName(Uri cloneUri)
        {
            return cloneUri.Host.Replace("_", "__") + "_" + cloneUri.PathAndQuery.Replace("_", "__").Replace('/', '_');
        }
    }
}