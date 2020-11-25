﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GDrive.Anomalies.Library;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GFile = Google.Apis.Drive.v3.Data.File;

namespace GDrive.Anomalies
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] Scopes = { DriveService.Scope.DriveReadonly };

        private const string ApplicationName = "GDrive-Anomalies";
        private static readonly HashSet<GFile> _allGFiles = new HashSet<GFile>(new GFileStrictComparer());
        private static readonly HashSet<GFile> _allFolders = new HashSet<GFile>(new GFileStrictComparer());
        private static readonly HashSet<GFile> _allDuplicated = new HashSet<GFile>();

        private static async Task Main()
        {
            var credential = Connect("credentials-tunisien.json");

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            await ListFiles(service, pageSize: 20).ConfigureAwait(false);
            Console.WriteLine($"Total file count; {_allGFiles.Count}");
            Console.WriteLine($"Total folder count; {_allFolders.Count}");
            Console.WriteLine($"Total duplicated files count; {_allDuplicated.Count}");

            foreach (var item in _allDuplicated)
            {
                Console.WriteLine($"{item.Name}");
            }
            Console.ReadKey();
        }

        private static async Task<FileList> ListFiles(DriveService service, string nextPageToken = default, int pageSize = 10, string parentId = "", string spaces = "drive", CancellationToken cancellationToken = default)
        {
            try
            {
                var listRequest = service.Files.List();
                listRequest.PageSize = pageSize;
                //listRequest.Fields = "nextPageToken, files(id, name, description, size, mimeType, parents, trashed, createdTime, modifiedTime, parents)";
                listRequest.Fields = "*";
                listRequest.PageToken = nextPageToken;
                listRequest.Spaces = spaces;

                if (!string.IsNullOrEmpty(parentId))
                {
                    listRequest.Q = $"('{parentId}' in parents)";
                    Console.WriteLine($"parentid: {parentId}");
                }

                // List files.
                var fileListResponse = await listRequest.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                Console.WriteLine(string.Empty);

                if (fileListResponse.Files?.Count > 0)
                {
                    foreach (var file in fileListResponse.Files)
                    {
                        Console.WriteLine(file.ToText());
                        if (file.IsFolder())
                        {
                            Console.WriteLine(string.Empty);
                            await ListFiles(service, default, pageSize: pageSize, parentId: file.Id, cancellationToken: cancellationToken).ConfigureAwait(false);
                            _allFolders.Add(file);
                        }
                        else
                        {
                            if (!_allGFiles.Add(file))
                            {
                                _allDuplicated.Add(file);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(fileListResponse.NextPageToken))
                    {
                        try
                        {
                            Console.WriteLine(">> To the next page");
                            await ListFiles(service, fileListResponse.NextPageToken, pageSize, parentId, spaces, cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No files found.");
                }
                return fileListResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private static UserCredential Connect([NotNull] string credFileName)
        {
            UserCredential credential;
            using (var stream = new FileStream(credFileName, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                var credPath = "token-" + credFileName;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }
    }
}