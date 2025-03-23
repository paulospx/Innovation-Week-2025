using CommandLine;

namespace GenerateData
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
        [Option('f', "filename", Required = false, HelpText = "CSV filename to create", Default ="C:\\temp\\feature.csv")]
        public required string Filename { get; set; }
    }
}
