using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;

namespace SEOMonitor
{
    class Program
    {
        /*
        static void CrawlSite(string url)
        {
            //Pages.Add(url);
            Console.WriteLine(url);
            var client = new HttpClient();
            var html = client.GetStringAsync(url).Result;
            var document = new HtmlDocument();
            document.LoadHtml(html);
            foreach (var element in document.DocumentNode.Descendants("a"))
            {
                var href = element.Attributes["href"]?.Value.ToLower();

                if (href == null ||
                    href.StartsWith("tel:") ||
                    href.StartsWith("#") ||
                    href.StartsWith("mailto:") ||
                    href.EndsWith(".jpg") ||
                    href.EndsWith(".pdf"))
                {
                    continue;
                }
                else if (href.StartsWith("/"))
                {
                    href = Site + href;
                }
                else if (href.StartsWith("http"))
                {
                    if (!href.StartsWith(Site))
                        continue; //Skip external sites
                }
                else
                {
                    href = url + "/" + href;
                }

                if (href.EndsWith("/"))
                    href = href.Substring(0, href.Length - 1);
                
                if (!Pages.Contains(href))
                {
                    CrawlSite(href);
                }
                
            }
        }
*/
        static void RunLighthouse(Page page, string filename)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/c lighthouse {page.Url} --output=json --output-path={filename} --chrome-flags=--headless";
            startInfo.CreateNoWindow = false;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
        }

        static int GetScore(JsonNode node, string category)
        {
            node = node["categories"]?[category]?["score"];

            if (node == null)
                return 0;

            try
            {
                return (int)(node.GetValue<decimal>() * 100);
            }
            catch
            {
                return 0;
            }

        }
        static string GetValue(JsonNode node, string category)
        {
            node = node["audits"]?[category]?["displayValue"];

            if (node == null)
                return String.Empty;

            return node.ToString();
        }

        static Score ParseLighthouse(Page page, string filename, DateTime measureDate)
        {
            var scores = new List<Score>();
            var file = File.ReadAllText(filename);
            var node = JsonNode.Parse(file);
            
            return new Score()
            {
                Id = Guid.NewGuid(),
                PageId = page.Id,
                MeasureDate = measureDate,
                Performance = GetScore(node, "performance"),
                LargestContentfulPaint = GetValue(node, "largest-contentful-paint"),
                CumulativeLayoutShift = GetValue(node, "cumulative-layout-shift"),
                FirstInputDelay = GetValue(node, "max-potential-fid"),
                BestPractices = GetScore(node, "best-practices"),
                SEO = GetScore(node, "seo"),
                Accessibility = GetScore(node, "accessibility"),
                PWA = GetScore(node, "pwa")
            };
        }

        static void SaveReport(StringBuilder output, string filename)
        {
            using (var file = new StreamWriter(filename, false))
            {
                file.Write(output.ToString());
            }
        }

        static void Main(string[] args)
        {
            try
            {
                string lighthouseFile = @"C:\Projects\SEOMonitor\Reports\output.json";
                string filename = @"C:\Projects\SEOMonitor\Reports\SuccessMetrics.csv";
                StringBuilder output = new StringBuilder();
                DateTime measureDate = DateTime.Now;

                //CreateExcel
                //CreateWorksheet1
                //CreateWorksheet2

                output.AppendLine("Url,Date,Performance,Accessibility,Best Practices,SEO,PWA,Largest Contentful Paint,Cumulative Layout Shift,First Input Delay");

                foreach (var site in Data.GetSites())
                {
                    foreach (var page in Data.GetPages(site))
                    {
                        RunLighthouse(page, lighthouseFile);
                        var score = ParseLighthouse(page, lighthouseFile, measureDate);
                        Data.AddScore(score);
                        //Worksheet 2. Add Score for this page
                    }
                    foreach (var summary in Data.GetSummary(site))
                    {
                        output.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\n", site.Url, summary.MeasureDate, summary.Performance, summary.Accessibility, summary.BestPractices, summary.SEO, summary.PWA, summary.LargestContentfulPaint, summary.CumulativeLayoutShift, summary.FirstInputDelay);
                    }
                    
                }
                SaveReport(output, filename);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }
        }
    }
}
