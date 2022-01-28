// (C) Copyright 2017 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins
{
    using System.Collections.Generic;

    public class Author
    {
        // ReSharper disable once InconsistentNaming
        public string login { get; set; }

        // ReSharper disable once InconsistentNaming
        public int id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string avatar_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string gravatar_id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string html_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string followers_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string following_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string gists_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string starred_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string subscriptions_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string organizations_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string repos_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string events_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string received_events_url { get; set; }

        // ReSharper disable once InconsistentNaming
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string type { get; set; }

        // ReSharper disable once InconsistentNaming
        public bool site_admin { get; set; }
    }

    public class Uploader
    {
        // ReSharper disable once InconsistentNaming
        public string login { get; set; }

        // ReSharper disable once InconsistentNaming
        public int id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string avatar_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string gravatar_id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string html_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string followers_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string following_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string gists_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string starred_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string subscriptions_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string organizations_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string repos_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string events_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string received_events_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string type { get; set; }

        // ReSharper disable once InconsistentNaming
        public bool site_admin { get; set; }
    }

    public class Asset
    {
        // ReSharper disable once InconsistentNaming
        public string url { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string browser_download_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public int id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string name { get; set; }

        // ReSharper disable once InconsistentNaming
        public string label { get; set; }

        // ReSharper disable once InconsistentNaming
        public string state { get; set; }

        // ReSharper disable once InconsistentNaming
        public string content_type { get; set; }

        // ReSharper disable once InconsistentNaming
        public int size { get; set; }

        // ReSharper disable once InconsistentNaming
        public int download_count { get; set; }

        // ReSharper disable once InconsistentNaming
        public string created_at { get; set; }

        // ReSharper disable once InconsistentNaming
        public string updated_at { get; set; }

        // ReSharper disable once InconsistentNaming
        public Uploader uploader { get; set; }
    }

    public class LatestVersion
    {
        // ReSharper disable once InconsistentNaming
        public string url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string html_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string assets_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string upload_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string tarball_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public string zipball_url { get; set; }

        // ReSharper disable once InconsistentNaming
        public int id { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string tag_name { get; set; }

        // ReSharper disable once InconsistentNaming
        public string target_commitish { get; set; }

        // ReSharper disable once InconsistentNaming
        public string name { get; set; }

        // ReSharper disable once InconsistentNaming
        public string body { get; set; }

        // ReSharper disable once InconsistentNaming
        public bool draft { get; set; }

        // ReSharper disable once InconsistentNaming
        public bool prerelease { get; set; }

        // ReSharper disable once InconsistentNaming
        public string created_at { get; set; }

        // ReSharper disable once InconsistentNaming
        public string published_at { get; set; }

        // ReSharper disable once InconsistentNaming
        public Author author { get; set; }

        // ReSharper disable once InconsistentNaming
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<Asset> assets { get; set; }
    }
}
