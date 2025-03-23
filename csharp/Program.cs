using Bogus;
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

Console.WriteLine("Generating Fake Workflow");

var features = WorkflowSim.SimulateEpics();

WriteToCsv(features, @"C:\temp\features.csv");

Console.WriteLine("Done");
