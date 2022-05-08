using System;
using SQLite;

namespace SEOMonitor
{
    public class Page
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

    }
}
