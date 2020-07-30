using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ArgumentProcessing
{
    public class ArgsProcessing
    {
        private static string[] _args;
        private static bool mandatoryArgsStat;

        public static bool InitialCheck()
        {
            if (_args.Contains("-help") && _args.Length > 1)
            {
                Console.WriteLine("Use only -help to view help information");
                return false;
            }
            else if (_args[0] == "-help")
            {
                HelpArgs();
                return false;
            }
            else
            {
                mandatoryArgsStat = CheckMandatoryArgument();
                if (mandatoryArgsStat == false)
                    return false;
            }

            return false;
        }

        private static void HelpArgs()
        {
            string executionInfo = @"Usage: img-processor [options] -pipe <path> -input <path> -output <path>
            Required parameters:
                -pipe <path> : the path to the pipe txt file
                -input <path> : the path to the input image or image directory
                -output <path> : the path to the output(file or directory)
                (must be a directory if -saveall is enabled or - input is a directory)
                                             
            Options:
                -verbose : use this option to enable verbose logging
                -saveall : use this option to save all intermediate images
                -help : display this help";

            Console.WriteLine(executionInfo);
        }

        private static bool CheckMandatoryArgument()
        {
            if (!_args.Contains("-pipe"))
            {
                Console.WriteLine("'-pipe' argument not found.\nRecommendation: Use '-help'.");
                return false;
            }
            else if (!_args.Contains("-input"))
            {
                Console.WriteLine("'-input' argument not found.\nRecommendation: Use '-help'.");
                return false;
            }
            else if (!_args.Contains("-output"))
            {
                Console.WriteLine("'-output' argument not found.\nRecommendation: Use '-help'.");
                return false;
            }
            else
            {
                Console.WriteLine("Mandatory arguments present.\n");
                return true;
            }
        }

        public string GetArgsValue(string argument)
        {
            int position;
            string argsNameError;

            if (argument == "-pipe")
            {
                position = Array.IndexOf(_args, argument);
                argsNameError = CheckArgumentName(argument, position);

                if (argsNameError != null)
                {
                    Console.WriteLine(argsNameError);
                    return null;
                }
                else
                    return _args[position + 1];
            }
            else if (argument == "-input")
            {
                position = Array.IndexOf(_args, argument);
                argsNameError = CheckArgumentName(argument, position);

                if (argsNameError != null)
                {
                    Console.WriteLine(argsNameError);
                    return null;
                }
                else
                    return _args[position + 1];
            }
            else if (argument == "-output")
            {
                position = Array.IndexOf(_args, "-output");
                try
                {
                    if (_args[position + 1] == "-input" || _args[position + 1] == "-pipe" || _args[position + 1] == "-verbose" || _args[position + 1] == "-saveall")
                        return AppContext.BaseDirectory;
                    else
                        return _args[position + 1];
                }
                catch (IndexOutOfRangeException)
                {
                    return AppContext.BaseDirectory;
                }
            }
            return null;
        }

        private static string CheckArgumentName(string argument, int position)
        {
            try
            {
                if (argument == "-pipe")
                {
                    if (_args[position + 1] == "-input" || _args[position + 1] == "-output" || _args[position + 1] == "-verbose" || _args[position + 1] == "-saveall")
                    {
                        return "Please insert pipe file name";
                    }
                }
                else if (argument == "-input")
                {
                    if (_args[position + 1] == "-pipe" || _args[position + 1] == "-output" || _args[position + 1] == "-verbose" || _args[position + 1] == "-saveall")
                    {
                        return "Please insert name of input file or directory";
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                if (argument == "-pipe")
                {
                    return "Please insert pipe file name";
                }
                else if (argument == "-input")
                {
                    return "Please insert name of input file or directory";
                }
            }
            return null;
        }

        public bool CheckOptionalArgs(string argument)
        {
            if (_args.Contains(argument))
                return true;

            return false;
        }

        public ArgsProcessing(string[] args)
        {
            _args = args;
        }
    }
}