using System;
using System.Diagnostics;
using ImageProps;

namespace NodeAbstraction
{
    public abstract class BaseNode
    {
        //Create a counter to name the saved image
        public static int imgNum = 0;

        //Create a Stopwatch class object
        readonly Stopwatch watch = new Stopwatch();

        //Create an abstract methods for nodes to inherit from this base class
        public abstract ImgProps Process(ImgProps input);

        //Create an abstract property to get node names
        public abstract string NodeName { get; }

        //Set previous node as null by default for first node
        BaseNode prevNode = null;

        public void SetInput(BaseNode node)
        {
            prevNode = node;
        }

        public ImgProps GetOutput(ImgProps input, string saveDir = "", bool logging = false, bool isSaveAll = false)
        {
            //Insert output of a node as input to its connected node
            if (prevNode != null)
            {
                try
                {
                    input = prevNode.GetOutput(input, saveDir, logging,  isSaveAll);
                }
                catch (StackOverflowException)
                {
                    Console.WriteLine("Stack limit reached - pipeline is too long");
                    return null;
                }
                imgNum++;
            }

            //Measure the processing time
            watch.Start();
            ImgProps output = Process(input);
            watch.Stop();

            if (isSaveAll == true)
                //save all the intermidiate and final nodes using counter as name
                output.Write(saveDir + "/" + imgNum.ToString());

            if (logging == true)
            {
                //Log data corresponding to processed node
                Console.WriteLine("Node name: " + NodeName);
                Console.WriteLine("Processing time: {0}ms", watch.ElapsedMilliseconds);
                Console.WriteLine("Size of input image: " + input.ToString());
                Console.WriteLine("Size of output image: " + output.ToString() + "\n");
            }

            //if (isDirectory == false)
            //{
            //    if (prevNode == null)
            //    {
            //        //Reset counter if all node/s are processed
            //        imgNum = 0;
            //    }
            //}

            return output;
        }
    }
}