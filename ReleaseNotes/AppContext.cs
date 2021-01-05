using System;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReleaseNotes
{
    internal class AppContext
    {
        public const string PAT_NAME = "SYSTEM_ACCESSTOKEN";
        public const string BoardColumnName = "System.BoardColumn";

        public Uri OrgUrl { get; set; }
        public string VssProjectName { get; set; } = "Up.France.ODI";
        public string ReleaseNotesProjectName { get; set; } = "Uptimise";
        public string TeamName { get; set; } = "App - Financeur";
        public string PageReleaseNotePath { get; set; } = "Home/Applications/Uptimise/ReleaseNotes/";
        public TeamContext TeamContext { get; set; }
        public VssConnection Connection { get; set; }

        public TeamProjectReference TeamProjectReference { get; set; }
        public WebApiTeam WebApiTeam { get; set; }
        public bool DryRun { get; internal set; }
    }
}
