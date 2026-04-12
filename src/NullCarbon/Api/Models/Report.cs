using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SCaddins.ExportSchedules.Models
{
    public class CreatedBy
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("avatar")]
        public object Avatar { get; set; }
    }

    public class Report
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("building")]
        public Building Building { get; set; }

        [JsonProperty("created_by")]
        public object created_by { get; set; }  // Changed from int? to object to handle different types

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("gwp_total")]
        public double GwpTotal { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ReportsResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public object Next { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("results")]
        public List<Report> Results { get; set; }
    }
}