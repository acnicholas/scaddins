using Newtonsoft.Json;
using System.Collections.Generic;

namespace SCaddins.ExportSchedules.Models
{
    public class TeamMember
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("members")]
        public List<TeamMember> Members { get; set; }

        [JsonProperty("invitations")]
        public List<object> Invitations { get; set; }

        [JsonProperty("dashboard_url")]
        public string DashboardUrl { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class TeamsResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("results")]
        public List<Team> Results { get; set; }
    }
}