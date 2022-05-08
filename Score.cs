using System;
using SQLite;

namespace SEOMonitor
{
    public class Score
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public DateTime MeasureDate { get; set; }
        public int Performance { get; set; }
        public string LargestContentfulPaint { get; set; }
        public string CumulativeLayoutShift { get; set; }
        public string FirstInputDelay { get; set; }
        public int BestPractices { get; set; }
        public int Accessibility { get; set; }
        public int SEO { get; set; }
        public int PWA { get; set; }
    }
}
