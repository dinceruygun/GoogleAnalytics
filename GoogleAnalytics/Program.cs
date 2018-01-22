using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            AnalyticsReportingService reportingService;
            GoogleCredential credentials;

            using (var stream = new FileStream("your json file", FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly };
                var googleCredential = GoogleCredential.FromStream(stream);
                credentials = googleCredential.CreateScoped(scopes);
            }


            reportingService = new AnalyticsReportingService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials
            });



            var dateRange = new DateRange
            {
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EndDate = DateTime.Now.ToString("yyyy-MM-dd")
            };
            var sessions = new Metric
            {
                Expression = "ga:pageviews",
                Alias = "Sessions"
            };
            var date = new Dimension { Name = "ga:date" };

            var reportRequest = new ReportRequest
            {
                DateRanges = new List<DateRange> { dateRange },
                Dimensions = new List<Dimension> { date },
                Metrics = new List<Metric> { sessions },
                ViewId = "xxxxxxxx" // your view id
            };



            var getReportsRequest = new GetReportsRequest
            {
                ReportRequests = new List<ReportRequest> { reportRequest }
            };
            var batchRequest = reportingService.Reports.BatchGet(getReportsRequest);
            var response = batchRequest.Execute();



            var result = response.Reports.First().Data.Rows.Sum(r => int.Parse(r.Metrics.First().Values.First()));


        }
    }
}
