
using Bogus;

namespace GenerateData
{
    public class WorkflowSim
    {
        public static Feature GenerateEpic(int id,
            string title, 
            string description, 
            string owner,
            string developer,
            string tester,
            string sme, 
            string changeBoard,
            int devDuration, 
            int testDuration, 
            int uatDuration, 
            int prodDuration,
            DateTime started
            )
        {
            var epic = new Feature
            {
                FeatureId = id,
                FeatureGuid = Guid.NewGuid(),
                Title = title,
                Description = description,
                ChangeBoard = changeBoard,
                Developer = developer,
                Tester = tester,
                SubjectMatterExpert = sme,
                Owner = owner,
                Started = started,
                DevelopmentFinished = started.AddSeconds((double)devDuration),
                DurationDevInSeconds = devDuration,
                DurationTestInSeconds = testDuration,
                TestFinished = started.AddSeconds((double)(devDuration+testDuration)),
                DurationAcceptanceInSeconds = uatDuration,
                SmeFinished = started.AddSeconds((double)(devDuration + testDuration+uatDuration)),
                DurationProductionInSeconds = prodDuration,
                ProductionReleases = started.AddSeconds((double)(devDuration + testDuration + uatDuration + prodDuration)),
                Closed = started.AddSeconds((double)(devDuration + testDuration + uatDuration + prodDuration))
            };
            return epic;
        }

        static DateTime GenerateRandomDate(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            TimeSpan timeSpan = endDate - startDate;
            int randomDays = random.Next(0, (int)timeSpan.TotalDays + 1); // Include end date
            return startDate.AddDays(randomDays);
        }

        public static List<Feature> SimulateEpics() {
            var result = new List<Feature>();
            var environments = new List<string> { "Dev", "Test", "UAT", "Prod" };
            var statuses = new List<string> { "Success", "Failed", "In Progress", "Pending", "Canceled" };
            var random = new Random();
            var featureList = new List<string> { "Feature 1", "Feature 2", "Feature 3", "Feature 4", "Feature 5" };


            for (int i =0; i < 1000; i++)
            {
                var init = GenerateRandomDate(
                    new DateTime(2025, 01, 01),
                    new DateTime(2025, 12, 31)
                    );


                int devDuration = random.Next(3600 * 24, 3600 * 24 * 3);
                int testDuration = random.Next(360 * 24, 3600 * 24 * 5);
                int uatDuration = random.Next(3600 * 24, 3600 * 24 * 5);
                int prodDuration = random.Next(3600 * 24, 3600 * 24 * 5);

                var feature = GenerateEpic(
                    i,
                    new Faker().Person.FirstName,
                    new Faker().Lorem.Lines(1),
                    new Faker().Person.LastName,
                    new Faker().Person.FirstName,
                    new Faker().Person.LastName,
                    new Faker().Person.LastName,
                    new Faker().Person.LastName,
                    devDuration,
                    testDuration,
                    uatDuration,
                    prodDuration,
                    init
                    );

                result.Add(feature);
            }


            
            // foreach(var int i)
            foreach(var env in environments)
            {
                var duration = random.Next(60, 3600);
                
            }
            return result;
        }
    }

    public class Feature {
        public int FeatureId{get;set;}
        public Guid FeatureGuid  {get;set;}
        public required string Title {get;set;}
        public required string Description {get;set;}
        public required string Owner{get;set;}
        public DateTime Started {get;set;}
        public int DurationDevInSeconds { get;set;}
        public required string Developer { get; set; }
        public DateTime DevelopmentFinished {get;set;}
        public required string Tester {get;set;}
        public int DurationTestInSeconds {get;set;}       
        public DateTime TestFinished {get;set;}
        public required string SubjectMatterExpert {get;set;}
        public int DurationAcceptanceInSeconds {get;set;}
        public DateTime SmeFinished {get;set;}
        public required string ChangeBoard {get;set;}
        public int DurationProductionInSeconds{get;set;}
        public  DateTime ProductionReleases {get;set;}
        public DateTime Closed {get;set;}
    }

}
