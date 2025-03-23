using Bogus;
using CommandLine;
using GenerateData;
using System.Reflection;

static void WriteToCsv<T>(List<T> data, string filePath)
{
    if (data == null || !data.Any())
    {
        throw new ArgumentException("The data list is empty or null.");
    }

    using (var writer = new StreamWriter(filePath))
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString()?.Replace(",", " ") ?? ""); // Handle nulls and commas
            writer.WriteLine(string.Join(",", values));
        }
    }
}


Console.WriteLine("Data Generator for AI Day");

var domains = new List<string>
{
    "Life", "Non Life", "Banking", "Pension", "Mortgages"
};

var faker = new Faker();

var workflowStates = new List<string>
{
    "Draft ✏️",
    "In Review 🔍",
    "Pending Approval ⏳",
    "Approved ✅",
    "In Progress ⚙️",
    "On Hold ⏸️",
    "Needs Attention 🚨",
    "Sucessful ✅",
    "Closed 🔐",
    "Canceled ❌",
    "Archived 🗓"
};


static void Main(string[] args)
{
    Parser.Default.ParseArguments<Options>(args)
           .WithParsed<Options>(o =>
           {
               Console.WriteLine("Generating Fake Workflow");

               if (o.Verbose)
               {
                   Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                   Console.WriteLine("Quick Start Example! App is in Verbose mode!");
               }
               else
               {
                   Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                   Console.WriteLine("Quick Start Example!");
               }
               var features = WorkflowSim.SimulateEpics();
               WriteToCsv(features, o.Filename);
               Console.WriteLine("Done");

           });
}

