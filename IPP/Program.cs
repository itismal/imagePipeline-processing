using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace IPP
{
    class Program
    {
        static void Main(string[] args)
        {
            ArgsProcessing processArgs = new ArgsProcessing(args);

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