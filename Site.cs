using System;
using SQLite;

namespace SEOMonitor
{
    public class Site
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
