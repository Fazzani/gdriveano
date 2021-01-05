using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReleaseNotes
{
    internal class ReleaseNotesService : IReleaseNotesService
    {
        private readonly ILogger<ReleaseNotesService> _logger;
        private readonly AppOptions _appOption;

        public ReleaseNotesService(ILogger<ReleaseNotesService> logger, IOptions<AppOptions> appAption)
        {
            _logger = logger;
            _appOption = appAption?.Value;
        }

        public async Task UpdateOrCreateReleaseNotes(AppContext appContext, CancellationToken cancellationToken = default)
        {
            var witClient = appContext.Connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                appContext.TeamProjectReference = await GetTeamProjectByNameAsync(appContext).ConfigureAwait(false);
                appContext.WebApiTeam = await GetTeamByNameAsync(appContext.Connection, appContext.TeamProjectReference.Id, teamName: appContext.TeamName, cancellationToken).ConfigureAwait(false);

                appContext.TeamContext = new TeamContext(appContext.TeamProjectReference.Id, appContext.WebApiTeam.Id);
                var notes = await GetWorkItems(witClient, appContext, cancellationToken).ToListAsync().ConfigureAwait(false);

                _logger.LogInformation($"{notes.Count} notes was retreived");

                string pageContent = await GenerateContent(notes, appContext.ReleaseNotesProjectName, cancellationToken: cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("A new content generated");

                if (appContext.DryRun)
                {
                    Console.WriteLine(pageContent);
                }
                else
                {
                    var pagePath = $"{appContext.PageReleaseNotePath}{DateTime.Now:dd-MM-yyyy}";
                    _logger.LogInformation($"Creating new release notes page at {pagePath}");
                    var pageResponse = await Wiki.Wiki.GetOrCreateWikiPage(appContext.Connection, appContext.TeamProjectReference.Id, pagePath).ConfigureAwait(false);
                    var wikiResponse = await Wiki.Wiki.EditWikiPageById(appContext.Connection, appContext.TeamProjectReference.Id, pageResponse.Page.Id.Value, new MemoryStream(Encoding.UTF8.GetBytes(pageContent ?? ""))).ConfigureAwait(false);
                    _logger.LogInformation($"New Release notes page created here {wikiResponse.Page.RemoteUrl}");
                }
            }
            catch (AggregateException aex)
            {
                _logger.LogError(aex, aex.Message);
            }
        }

        public async Task<string> GenerateContent(List<object> notes, string projectName = "Uptimize", string version = "1.0.0", CancellationToken cancellationToken = default)
        {
            Handlebars.RegisterTemplate("Note", _appOption.NoteTpl);
            var hbs = await File.ReadAllTextAsync("release.hbs", cancellationToken).ConfigureAwait(false);
            var tpl = Handlebars.Compile(hbs);
            var data = new
            {
                ProjectName = projectName,
                Date = DateTime.Now.ToShortDateString(),
                Version = version,
                Notes = notes
            };
            return tpl(data);
        }

        public async IAsyncEnumerable<object> GetWorkItems(WorkItemTrackingHttpClient witClient,
                                                                   AppContext appContext,
                                                                   [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var q = new Wiql { Query = _appOption.Query };

            var res = await witClient.QueryByWiqlAsync(q, appContext.TeamContext, top: 1000, cancellationToken: cancellationToken).ConfigureAwait(false);

            foreach (var item in res.WorkItems)
            {
                var workitem = await witClient.GetWorkItemAsync(item.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (workitem.Fields.Keys.Contains(AppContext.BoardColumnName))
                {
                    var boardColumn = (string)workitem.Fields[AppContext.BoardColumnName];
                    if (boardColumn == BoardColumns.Testing || boardColumn == BoardColumns.Finished || boardColumn == BoardColumns.Done)
                    {
                        yield return new
                        {
                            Title = workitem.Fields["System.Title"].ToString(),
                            workitem.Id,
                            Url = workitem.Links.Links["html"] is ReferenceLink link ? link.Href : string.Empty
                        };
                    }
                }
            }
        }

        public async Task<TeamProjectReference> GetTeamProjectByNameAsync(AppContext appContext)
        {
            var projectClient = appContext.Connection.GetClient<ProjectHttpClient>();

            var projects = await projectClient.GetProjects(ProjectState.All).ConfigureAwait(false);
            while (projects.All(x => x.Name != appContext.VssProjectName && !string.IsNullOrEmpty(projects.ContinuationToken)))
            {
                projects = await projectClient.GetProjects(ProjectState.All, continuationToken: projects.ContinuationToken).ConfigureAwait(false);
            }
            return projects.FirstOrDefault(x => x.Name == appContext.VssProjectName);
        }

        public static async Task<WebApiTeam> GetTeamByNameAsync(VssConnection connection, Guid projectId, string teamName, CancellationToken cancellationToken)
        {
            var teamClient = connection.GetClient<TeamHttpClient>();
            var teams = await teamClient.GetTeamsAsync(projectId.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
            return teams.FirstOrDefault(x => x.Name == teamName);
        }
    }

    static class BoardColumns
    {
        public const string CodeReview = "Code review";
        public const string Testing = "Testing";
        public const string Finished = "Finished";
        public const string Done = "Done";
    }
}
