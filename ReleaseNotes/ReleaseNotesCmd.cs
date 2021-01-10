﻿using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Threading;

namespace ReleaseNotes
{
    [Command(
        Name = "rnotes",
        OptionsComparison = StringComparison.InvariantCultureIgnoreCase,
        Description = "rnotes a CLI to create auto generate Release notes from azure devops")]
    [HelpOption("-?")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class ReleaseNotesCmd
    {
        private readonly ILogger<ReleaseNotesCmd> _logger;
        private readonly IConsole _console;
        private readonly IReleaseNotesService _releaseNotesService;

        public ReleaseNotesCmd(ILogger<ReleaseNotesCmd> logger, IConsole console, IReleaseNotesService releaseNotesService)
        {
            _logger = logger;
            _console = console;
            _releaseNotesService = releaseNotesService;
        }

        public async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            if (!Uri.TryCreate(OrgUrl, UriKind.Absolute, out var uri))
            {
                _logger.LogError($"Invalid organization url '{OrgUrl}'");
                app.ShowHelp();
                return 1;
            }

            var appContext = new AppContext
            {
                OrgUrl = uri,
                TeamName = TeamName,
                PageReleaseNotePath = PageReleaseNotePath,
                VssProjectName = VssProjectName,
                ReleaseNotesProjectName = ReleaseNotesProjectName,
                ReleaseNoteVersion = ReleaseNoteVersion,
                Query = Query,
                DryRun = DryRun,
                IterationOffset = IterationOffset
            };

            // Create a connection
            appContext.Connection = new VssConnection(appContext.OrgUrl, new VssBasicCredential(string.Empty, PAT));

            await _releaseNotesService.UpdateOrCreateReleaseNotes(appContext, cancellationToken).ConfigureAwait(false);
            return 0;
        }

        [Option("-o|--organization", "Organization url", CommandOptionType.SingleValue)]
        public string OrgUrl { get; } = Environment.GetEnvironmentVariable("System.CollectionUri");

        [Option("-p|--project", "Azure devops project name", CommandOptionType.SingleValue)]
        public string VssProjectName { get; set; } = Environment.GetEnvironmentVariable("System.TeamProject");

        [Option(Description = "Wiki project name", ShortName = "r")]
        public string ReleaseNotesProjectName { get; set; }

        [Option("-t|--team", "Wiki team name", CommandOptionType.SingleValue)]
        public string TeamName { get; set; }

        [Option(Description = "Personal access token", ShortName = "x")]
        public string PAT { get; set; } = Environment.GetEnvironmentVariable(AppContext.PAT_NAME);

        [Option(Description = "Release notes page path", ShortName = "n")]
        public string PageReleaseNotePath { get; set; }

        [Option("-q|--query", "Query Id Or Path: used to retreive release notes work items", CommandOptionType.SingleValue)]
        public string Query { get; set; }

        [Option("-v|--version", "Overrite release notes version", CommandOptionType.SingleValue)]
        public string ReleaseNoteVersion { get; set; }

        [Option("-i|--iteration", "Iteration offset (ex: +1, -1)", CommandOptionType.SingleValue)]
        public string IterationOffset { get; set; } = "0";

        [Option("-d|--dry", "If true, the release will be displayed on console", CommandOptionType.NoValue)]
        public bool DryRun { get; set; } = false;

        private static string GetVersion()
            => typeof(ReleaseNotesCmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}