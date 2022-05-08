using System;
using System.Collections.Generic;
using System.Configuration;
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
            string path = ConfigurationManager.AppSettings["DatabaseFile"];
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


        Per Abby

            The list of pages for success metrics looks good. I think we’ll just need to make these changes:
-	Add community page (ex. https://www.richmondamerican.com/california/inland-empire-new-homes/chino/gardenside-at-the-preserve)
-	Add plan page (ex. https://www.richmondamerican.com/colorado/denver-metro-new-homes/arvada/haskins-station/anika) 
-	Add QMI page  (ex. https://www.richmondamerican.com/nevada/las-vegas-new-homes/henderson/marble-mesa-at-lake-las-vegas/arabelle/30030000-0021 this QMI is available August 2022, but we may need to find a new QMI URL every once in a while. Or if it changes too often, we can stop tracking QMI pages)
-	Add agent blog (we probably don’t need /blog/about us, so we can replace that one with https://www.richmondamerican.com/agent-blog/) 

            https://www.richmondamerican.com
https://www.richmondamerican.com/campaigns/1457
https://www.richmondamerican.com/covid-19
https://www.richmondamerican.com/arizona
https://www.richmondamerican.com/arizona/phoenix-new-homes
https://www.richmondamerican.com/design-a-home
https://www.richmondamerican.com/about-us
https://www.richmondamerican.com/career-center
https://www.richmondamerican.com/contact-us
https://www.richmondamerican.com/real-estate-agents-center
https://www.richmondamerican.com/guides/homebuying
https://www.richmondamerican.com/new-home-events
https://www.richmondamerican.com/blog
https://www.richmondamerican.com/blog/about-us
       
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

