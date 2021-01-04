//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.TeamFoundation.Core.WebApi;
//using Microsoft.TeamFoundation.Core.WebApi.Types;
//using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
//using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
//using Microsoft.VisualStudio.Services.Common;
//using Microsoft.VisualStudio.Services.WebApi;

//namespace ReleaseNotes
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var personalAccessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN");

//            if (args.Length == 2)
//            {
//                var orgUrl = new Uri(args[0]);         // Organization URL, for example: https://dev.azure.com/fabrikam
//                int workItemId = int.Parse(args[1]);   // ID of a work item, for example: 12

//                // Create a connection
//                var connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));

//                // Show details a work item
//                ShowWorkItemDetails(connection, "Up.France.ODI", workItemId, CancellationToken.None).GetAwaiter().GetResult();
//            }
//            else
//            {
//                Console.WriteLine("Usage: ConsoleApp {orgUrl} {personalAccessToken} {workItemId}");
//            }
//        }

//        static private async Task ShowWorkItemDetails(VssConnection connection, string projectName, int workItemId, CancellationToken cancellationToken)
//        {
//            // Get an instance of the work item tracking client
//            var witClient = connection.GetClient<WorkItemTrackingHttpClient>();
//            //var workClient = connection.GetClient<WorkHttpClient>();

//            try
//            {
//                // Get the specified work item
//                var project = await GetTeamProjectByNameAsync(connection, projectName).ConfigureAwait(false);
//                var team = await GetTeamByNameAsync(connection, project.Id, "App - Financeur", cancellationToken).ConfigureAwait(false);

//                var teamCtx = new TeamContext(project.Id, team.Id);
//                //Microsoft.TeamFoundation.Work.WebApi.TimeFrame.Current
//                //var iterations = await workClient.GetTeamIterationsAsync(teamCtx, "Current", cancellationToken: cancellationToken).ConfigureAwait(false);
//                var q = new Wiql { Query = @"Select [System.Id], [System.Title], [System.State] From WorkItems
//                                             Where [System.WorkItemType] = 'User Story'
//                                             AND [Iteration Path] = @CurrentIteration
//                                             OR [Iteration Path] = @CurrentIteration - 1
//                                             order by [Microsoft.VSTS.Common.Priority] asc, [System.CreatedDate] desc" };
//                var res = await witClient.QueryByWiqlAsync(q, teamCtx, top: 1000, cancellationToken: cancellationToken).ConfigureAwait(false);
//                foreach (var item in res.WorkItems)
//                {
//                    var workitem = await witClient.GetWorkItemAsync(item.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
//                    if (workitem.Fields.Keys.Contains("System.BoardColumn"))
//                    {
//                        var boardColumn = (string)workitem.Fields["System.BoardColumn"];
//                        if (boardColumn == "Code review" || boardColumn == "Testing" || boardColumn == "Finished" || boardColumn == "Done")
//                        {
//                            Console.WriteLine(workitem.Fields["System.Title"]);
//                            //Console.WriteLine("{0}: {1}", workitem.Fields["System.Title"], workitem.Url);
//                        }
//                    }

//                    //TODO: Write to new wiki page




//                    //// Output the work item's field values
//                    //foreach (var field in workitem.Fields)
//                    //{
//                    //    Console.WriteLine("  {0}: {1}", field.Key, field.Value);
//                    //}
//                }
//            }
//            catch (AggregateException aex)
//            {
//                if (aex.InnerException is VssServiceException vssex)
//                {
//                    Console.WriteLine(vssex.Message);
//                }
//            }
//        }

//        public static async Task<TeamProjectReference> GetTeamProjectByNameAsync(VssConnection connection, string projectName)
//        {
//            var projectClient = connection.GetClient<ProjectHttpClient>();

//            var projects = await projectClient.GetProjects(ProjectState.All).ConfigureAwait(false);
//            while (projects.All(x => x.Name != projectName && !string.IsNullOrEmpty(projects.ContinuationToken)))
//            {
//                projects = await projectClient.GetProjects(ProjectState.All, continuationToken: projects.ContinuationToken).ConfigureAwait(false);
//            }
//            return projects.FirstOrDefault(x => x.Name == projectName);
//        }

//        public static async Task<WebApiTeam> GetTeamByNameAsync(VssConnection connection, Guid projectId, string teamName, CancellationToken cancellationToken)
//        {
//            var teamClient = connection.GetClient<TeamHttpClient>();
//            var teams = await teamClient.GetTeamsAsync(projectId.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false);
//            return teams.FirstOrDefault(x => x.Name == teamName);
//        }
//    }
//}
