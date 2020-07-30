using System;
using ArgumentProcessing;
using PipeDataProcessing;

namespace IPP
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments entered. Use -help.");
                return;
            }

            ArgsProcessing processArgs = new ArgsProcessing(args);

            if (ArgsProcessing.InitialCheck())
            {
                string pipeFileName = processArgs.GetArgsValue("-pipe");
                string imgInput = processArgs.GetArgsValue("-input");
                string outputDir = processArgs.GetArgsValue("-output");

                if (pipeFileName == null || imgInput == null || outputDir == null)
                {
                    Console.WriteLine("Use -help to see usage");
                    return;
                }

                bool isVerbose = processArgs.CheckOptionalArgs("-verbose");
                bool isSaveAll = processArgs.CheckOptionalArgs("-saveall");

                if (isSaveAll == true)
                {
                    if (outputDir == null || outputDir == "")
                    {
                        Console.WriteLine("Output directory name must be specified if '-saveall' argument is provided");
                        return;
                    }
                }

                PipelineProcessing.ArgsData(pipeFileName, imgInput, outputDir);
                PipelineProcessing.OptArgsData(isVerbose, isSaveAll);
                PipelineProcessing.Process();
            }   
        }
    }
}