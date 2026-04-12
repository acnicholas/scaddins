using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCaddins.ExportSchedules.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ExportSchedules.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://backend.nullcarbon.dk";
        private static readonly HttpClient _httpClient;

        // Static constructor to initialize the shared HttpClient
        static ApiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5) // Increased to 5 minutes (300 seconds)
            };
        }

        /// <summary>
        /// Sets the authorization token for API requests
        /// </summary>
        public static void SetAuthToken(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// Get all teams for the authenticated user
        /// </summary>
        public static async Task<List<Team>> GetTeams(string accessToken)
        {
            SetAuthToken(accessToken);

            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/teams/api/teams/");

                await EnsureSuccessStatusCodeWithDetailedErrorAsync(
                    response, "Failed to get teams");

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new List<Team>();
                }

                var token = JToken.Parse(content);

                if (token.Type == JTokenType.Array)
                {
                    return token.ToObject<List<Team>>() ?? new List<Team>();
                }

                var teamsResponse = token.ToObject<TeamsResponse>();
                return teamsResponse?.Results ?? new List<Team>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving teams: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all buildings for a team
        /// </summary>
        public static async Task<List<Building>> GetBuildings(string accessToken, string teamSlug)
        {
            if (string.IsNullOrEmpty(teamSlug))
            {
                throw new ArgumentException("Team slug cannot be empty", nameof(teamSlug));
            }

            SetAuthToken(accessToken);

            try
            {
                // Modified URL to include page=all parameter to get all buildings at once
                var response = await _httpClient.GetAsync($"{BaseUrl}/lca/{teamSlug}/building/?page=all");

                await EnsureSuccessStatusCodeWithDetailedErrorAsync(
                    response, $"Failed to get buildings for team '{teamSlug}'");

                var content = await response.Content.ReadAsStringAsync();

                // When using page=all, the response is a direct array of buildings, not wrapped in a results property
                var buildings = JsonConvert.DeserializeObject<List<Building>>(content);
                return buildings ?? new List<Building>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving buildings: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all reports for a building
        /// </summary>
        public static async Task<List<Report>> GetReports(string accessToken, string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId))
            {
                throw new ArgumentException("Building ID cannot be empty", nameof(buildingId));
            }

            SetAuthToken(accessToken);

            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/lca/{buildingId}/reports/");

                await EnsureSuccessStatusCodeWithDetailedErrorAsync(
                    response, $"Failed to get reports for building '{buildingId}'");

                var content = await response.Content.ReadAsStringAsync();
                var reportsResponse = JsonConvert.DeserializeObject<ReportsResponse>(content);
                return reportsResponse.Results ?? new List<Report>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving reports: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Upload an Excel file to a report with decimal separator specification
        /// </summary>
        public static async Task<bool> UploadExcelFile(
            string accessToken,
            string reportId,
            byte[] fileData,
            string fileName,
            string decimalSeparator = ".",
            string processor = "Revit",
            bool noKeynote = false)
        {
            if (string.IsNullOrEmpty(reportId))
            {
                throw new ArgumentException("Report ID cannot be empty", nameof(reportId));
            }

            if (fileData == null || fileData.Length == 0)
            {
                throw new ArgumentException("File data cannot be empty", nameof(fileData));
            }

            SetAuthToken(accessToken);

            try
            {
                // Show file size in the debug output
                System.Diagnostics.Debug.WriteLine($"Uploading file {fileName}, size: {fileData.Length / 1024} KB");
                System.Diagnostics.Debug.WriteLine($"Using decimal separator: {decimalSeparator}");
                System.Diagnostics.Debug.WriteLine($"No keynote flag: {noKeynote}");

                using (var content = new MultipartFormDataContent())
                {
                    // Add the excel_file parameter
                    var fileContent = new ByteArrayContent(fileData);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    content.Add(fileContent, "excel_file", fileName);

                    // Add the processor parameter
                    content.Add(new StringContent(processor), "processor");

                    // Add the decimal_separator parameter
                    content.Add(new StringContent(decimalSeparator), "decimal_separator");

                    // Add the no_keynote parameter
                    content.Add(new StringContent(noKeynote.ToString().ToLowerInvariant()), "no_keynote");

                    // Add progress reporting for large uploads
                    System.Diagnostics.Debug.WriteLine("Starting upload...");
                    var response = await _httpClient.PostAsync($"{BaseUrl}/lca/report/{reportId}/batch-upload", content);
                    System.Diagnostics.Debug.WriteLine("Upload completed");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Upload failed with status {response.StatusCode}: {errorContent}");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uploading Excel file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper method to provide more detailed error information
        /// </summary>
        private static async Task EnsureSuccessStatusCodeWithDetailedErrorAsync(
            HttpResponseMessage response, string context)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"{context}: {response.StatusCode}. Details: {errorContent}");
            }
        }
    }
}
