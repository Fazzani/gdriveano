using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReleaseNotes
{
    class Program
    {
        private const string ProjectName = "Up.France.ODI";
        private const string BoardColumnName = "System.BoardColumn";
        private const string TeamName = "App - Financeur";
        private const string PAT_NAME = "SYSTEM_ACCESSTOKEN";
        private const string PageReleaseNotePath = "Home/Applications/Uptimise/ReleaseNotes/";

        static void Main(string[] args)
        {
            var personalAccessToken = Environment.GetEnvironmentVariable(PAT_NAME);

            if (args.Length == 2)
            {
                var orgUrl = new Uri(args[0]);         // Organization URL, for example: https://dev.azure.com/fabrikam

                // Create a connection
                var connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));

                // Show details a work item
                ShowWorkItemDetails(connection, ProjectName, CancellationToken.None).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Usage: ConsoleApp {orgUrl}");
            }
        }

        static private async Task ShowWorkItemDetails(VssConnection connection, string projectName, CancellationToken cancellationToken)
        {
            // Get an instance of the work item tracking client
            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                // Get the specified work item
                var project = await GetTeamProjectByNameAsync(connection, projectName).ConfigureAwait(false);
                var team = await GetTeamByNameAsync(connection, project.Id, teamName: TeamName, cancellationToken).ConfigureAwait(false);

                var teamCtx = new TeamContext(project.Id, team.Id);
                var q = new Wiql { Query = @"Select [System.Id], [System.Title], [System.State] From WorkItems
                                             Where [System.WorkItemType] = 'User Story'
                                             AND [Iteration Path] = @CurrentIteration
                                             OR [Iteration Path] = @CurrentIteration - 1
                                             order by [Microsoft.VSTS.Common.Priority] asc, [System.CreatedDate] desc" };

                var res = await witClient.QueryByWiqlAsync(q, teamCtx, top: 1000, cancellationToken: cancellationToken).ConfigureAwait(false);
                var notes = new List<object>();

                foreach (var item in res.WorkItems)
                {
                    var workitem = await witClient.GetWorkItemAsync(item.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (workitem.Fields.Keys.Contains(BoardColumnName))
                    {
                        var boardColumn = (string)workitem.Fields[BoardColumnName];
                        if (boardColumn == BoardColumns.Testing || boardColumn == BoardColumns.Finished || boardColumn == BoardColumns.Done)
                        {
                            notes.Add(new
                            {
                                Title = workitem.Fields["System.Title"].ToString(),
                                workitem.Id,
                                Url = workitem.Links.Links["html"] is ReferenceLink link ? link.Href : string.Empty
                            });
                        }
                    }
                }

                Handlebars.RegisterTemplate("Note", "[{{{Id}}}-{{{Title}}}]({{{Url}}})");
                var hbs = await File.ReadAllTextAsync("release.hbs").ConfigureAwait(false);
                var tpl = Handlebars.Compile(hbs);
                var data = new
                {
                    ProjectName = "Uptimize",
                    Date = DateTime.Now.ToShortDateString(),
                    Version = "1.0.0",
                    Notes = notes
                };
                var result = tpl(data);
                Console.WriteLine("");
                Console.WriteLine("========================================");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(result);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("========================================");

                var pagePath = $"{PageReleaseNotePath}{DateTime.Now:dd-MM-yyyy}";
                Console.WriteLine(pagePath);

                var pageResponse = await Wiki.Wiki.GetOrCreateWikiPage(connection, project.Id, pagePath).ConfigureAwait(false);
                await Wiki.Wiki.EditWikiPageById(connection, project.Id, pageResponse.Page.Id.Value, new MemoryStream(Encoding.UTF8.GetBytes(result ?? ""))).ConfigureAwait(false);
            }
            catch (AggregateException aex)
            {
                if (aex.InnerException is VssServiceException vssex)
                {
                    Console.WriteLine(vssex.Message);
                }
            }
        }

        public static async Task<TeamProjectReference> GetTeamProjectByNameAsync(VssConnection connection, string projectName)
        {
            var projectClient = connection.GetClient<ProjectHttpClient>();

            var projects = await projectClient.GetProjects(ProjectState.All).ConfigureAwait(false);
            while (projects.All(x => x.Name != projectName && !string.IsNullOrEmpty(projects.ContinuationToken)))
            {
                projects = await projectClient.GetProjects(ProjectState.All, continuationToken: projects.ContinuationToken).ConfigureAwait(false);
            }
            return projects.FirstOrDefault(x => x.Name == projectName);
        }

        public static async Task<WebApiTeam> GetTeamByNameAsync(VssConnection connection, Guid projectId, string teamName, CancellationToken cancellationToken)
        {
            var teamClient = connection.GetClient<TeamHttpClient>();
            var teams = await teamClient.GetTeamsAsync(projectId.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
            return teams.FirstOrDefault(x => x.Name == teamName);
        }
    }

    public static class BoardColumns
    {
        public const string CodeReview = "Code review";
        public const string Testing = "Testing";
        public const string Finished = "Finished";
        public const string Done = "Done";
    }
}
