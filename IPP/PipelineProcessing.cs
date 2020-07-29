using System;
using System.Collections.Generic;
using System.IO;
using ImageProps;
using NodeAbstraction;
using Nodes;

namespace PipeDataProcessing
{
    public class NodeException : Exception
    {
        public NodeException(string message) : base(message) { }
    }

    public class PipelineProcessing
    {
        //declarations
        private static string _pipeFileName;
        private static string _imgInput;
        private static string _outputDir;
        private static bool _isVerbose;
        private static bool _isSaveAll;

        private static ImgProps img = null;
        private static string[] pipeDataArray;

        //public
        public static void ArgsData(string pipeFileName, string imgInput, string outputDir)
        {
            _pipeFileName = pipeFileName;
            _imgInput = imgInput;
            _outputDir = outputDir;
        }

        public static void OptArgsData(bool isVerbose, bool isSaveAll)
        {
            _isVerbose = isVerbose;
            _isSaveAll = isSaveAll;
        }
        
        //private
        public static void Process()
        {
            if (!Directory.Exists(_outputDir))
                Directory.CreateDirectory(_outputDir);

            BaseNode endNode = ParsePipeLine();
            if (endNode == null)
            {
                Console.WriteLine("EndNode is null. Failed");
                return;
            }

            if (Directory.Exists(_imgInput))
            {
                int counter = 0;
                string[] filePaths;
                filePaths = Directory.GetFiles(_imgInput);

                for (int i = 0; i < filePaths.Length; i++)
                {
                    counter++;
                    try
                    {
                        img = new ImgProps(filePaths[i]);
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Directory not found");
                        return;
                    }
                    ImgProps outputImg = endNode.GetOutput(img, _outputDir, _isVerbose, _isSaveAll);

                    if (outputImg == null)
                    {
                        Console.WriteLine("Pipeline failed");
                        return;
                    }

                    BaseNode.imgNum++;

                    if (_isSaveAll == false)
                        outputImg.Write(_outputDir + "/final");
                }
            }
            else
            {
                try
                {
                    img = new ImgProps(_imgInput);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found");
                    return;
                }
                ImgProps outputImg = endNode.GetOutput(img, _outputDir, _isVerbose, _isSaveAll);

                if (outputImg == null)
                {
                    Console.WriteLine("Pipeline failed");
                    return;
                }

                if (_isSaveAll == false)
                    outputImg.Write(_outputDir + "/final");
            }
        }

        private static BaseNode ParsePipeLine()
        {
            try
            {
                pipeDataArray = File.ReadAllLines(_pipeFileName);
                //Process the pipe file data
                return AnalyzePipeData();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Pipe File not found. Use '-help' for help");
                return null;
            }
            catch (NodeException error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }

        private static BaseNode AnalyzePipeData()
        {
            List<string> nodeDataList = new List<string>();
            List<string> splittedNodeData = new List<string>();

            //List to store parent class type objects
            List<BaseNode> nodeName = new List<BaseNode>();

            for (int i = 0; i < pipeDataArray.Length; i++)
            {
                //Ignores the lines containing '#' (Comments in pipe file)
                if (pipeDataArray[i].Contains("#"))
                    continue;
                //Ignores the empty lines in pipe file
                else if (pipeDataArray[i] == "")
                    continue;
                else if (pipeDataArray[i].Contains(" "))
                    nodeDataList.AddRange(pipeDataArray[i].Split(" "));
                else
                    nodeDataList.Add(pipeDataArray[i]);
            }

            for (int i = 0; i < nodeDataList.Count; i++)
            {
                splittedNodeData.AddRange(nodeDataList[i].Split("="));
            }

            for (int i = 0; i < splittedNodeData.Count; i++)
            {
                if (splittedNodeData[i] == "node")
                {
                    //Create a noise object if noise node is present in pipe file
                    if (splittedNodeData[i + 1] == "noise")
                    {
                        if (splittedNodeData[i + 2] == "noisePercent")
                        {
                            //Check if noise percentage is between 0 and 1 (both inclusive)
                            if (float.Parse(splittedNodeData[i + 3]) > 1.0f || float.Parse(splittedNodeData[i + 3]) < 0.0f)
                                throw new NodeException("Enter correct noise percentage value.\nNoise percent should in between 0 and 1 (both inclusive)");
                            else
                                nodeName.Add(new N_Noise(float.Parse(splittedNodeData[i + 3])));
                        }
                        //Check if pipe file contains 'noisePercent' parameter
                        else
                            throw new NodeException("Check the pipe file if 'noisePercent' exist exactly as mentioned.");
                    }
                    //Create a convolve object if convolve node is present in pipe file
                    else if (splittedNodeData[i + 1] == "convolve")
                    {
                        if (splittedNodeData[i + 2].Contains("kernel"))
                        {
                            //Check if kernel only contains of the following kernel types: edge, blur or sharpen
                            if (splittedNodeData[i + 3] == "edge")
                                nodeName.Add(new N_Convolve(splittedNodeData[i + 3]));
                            else if (splittedNodeData[i + 3] != "blur")
                                nodeName.Add(new N_Convolve(splittedNodeData[i + 3]));
                            else if (splittedNodeData[i + 3] != "sharpen")
                                nodeName.Add(new N_Convolve(splittedNodeData[i + 3]));
                            else
                                throw new NodeException("Enter correct kernel type.\nEither one of these: 'edge', 'blur' or 'sharpen'");
                        }
                        //Check if pipe file contains 'kernel' parameter
                        else
                            throw new NodeException("Check the pipe file if 'kernel' exist exactly as mentioned.");
                    }
                    //Create a vignette object if vignette node is present in pipe file
                    else if (splittedNodeData[i + 1] == "vignette")
                        nodeName.Add(new N_Vignette());
                    else
                    {
                        throw new NodeException("Node not found");
                    }
                }
            }

            //Connect the nodes in order
            for (int i = 0; i < nodeName.Count - 1; i++)
            {
                nodeName[i + 1].SetInput(nodeName[i]);
            }

            //Return the last node created
            return nodeName[nodeName.Count - 1];
        }
    }
}