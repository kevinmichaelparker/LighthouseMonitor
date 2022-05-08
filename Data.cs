using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SEOMonitor
{
    public static class Data
    {
        private readonly static SQLiteConnection database;

        static Data()
        {
            string path = Environment.CurrentDirectory + "scores.db";
            database = new SQLiteConnection(path);
        }

        public static void Create()
        {
            database.CreateTable<Site>();
            database.CreateTable<Page>();
            database.CreateTable<Score>();
        }

        public static void Initialize()
        {
            database.Execute("UPDATE page SET Url = ?", "https://ra-blazor.cyberlancersseo.com");
            
            /*
            database.DeleteAll<Score>();
            database.DeleteAll<Page>();
            database.DeleteAll<Site>();

            Guid siteId = Guid.NewGuid();
            database.Insert(new Site { Id = siteId, Name = "Richmond American", Url = "https://richmondamerican.com" });
            
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://ra-blazor.cyberlancersseo.com" });
            
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/arizona/phoenix-new-homes" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/campaigns/1457" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/covid-19" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/arizona" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/arizona/phoenix-new-homes" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/design-a-home" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/about-us" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/career-center" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/contact-us" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/real-estate-agents-center" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/guides/homebuying" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/new-home-events" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/blog" });
            database.Insert(new Page { Id = Guid.NewGuid(), SiteId = siteId, Url = "https://www.richmondamerican.com/blog/about-us" });
            */
        }

        public static List<Site> GetSites()
        {
            return database.Table<Site>().OrderBy(s => s.Name).ToList();
        }

        public static List<Page> GetPages(Site site)
        {
            return database.Table<Page>().Where(p => p.SiteId == site.Id).OrderBy(p => p.Url).ToList();
        }

        public static List<Score> GetSummary(Site site)
        {
            string query = @"SELECT 
                p.SiteId AS PageId,
                s.MeasureDate AS MeasureDate,
                MIN(s.Performance) AS Performance,
                MAX(s.LargestContentfulPaint) AS LargestContentfulPaint,
                MAX(s.CumulativeLayoutShift) AS CumulativeLayoutShift,
                MAX(s.FirstInputDelay) AS FirstInputDelay,
                MIN(s.Accessibility) AS Accessibility,
                MIN(s.BestPractices) AS BestPractices,
                MIN(s.SEO) AS SEO,
                MIN(s.PWA) AS PWA
            FROM
                page p JOIN
                score s ON s.PageId = p.Id
            WHERE
                p.SiteId = ?
            GROUP BY
                p.SiteId,
                s.MeasureDate
            ORDER BY
                s.MeasureDate DESC";

            var scores = database.Query<Score>(query, site.Id).ToList();

            return scores;
        }

        public static int AddScore(Score score)
        {
            return database.Insert(score);
        }
    }
}

