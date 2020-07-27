using System;
using SixLabors.ImageSharp.PixelFormats;
using ImageProps;
using NodeAbstraction;

namespace Nodes
{
    public class N_Greyscale : BaseNode
    {
        public override string NodeName
        {
            get { return string.Format("GreyScale"); }
        }

        public override ImgProps Process(ImgProps input)
        {
            ImgProps output = ImgProps.ToGrayscale(input);
            return output;
        }
    }

    public class N_Pixelate : BaseNode
    {
        public override string NodeName
        {
            get { return string.Format("Pixelate"); }
        }

        private int _width;
        private int _height;

        public N_Pixelate(int newWidth, int newHeight)
        {
            _width = newWidth;
            _height = newHeight;
        }

        public override ImgProps Process(ImgProps input)
        {
            ImgProps output = ImgProps.Resize(input, _width, _height);
            output = ImgProps.Resize(output, input.Width, input.Height);
            return output;
        }
    }

    public class N_Noise : BaseNode
    {
        private float _decimalPercent;

        public override string NodeName
        {
            get { return string.Format("Noise ({0}%)", _decimalPercent * 100); }
        }

        public override ImgProps Process(ImgProps input)
        {
            ImgProps noisedImage = new ImgProps(input.Width, input.Height);

            Random random = new Random();

            const int highestByte = 255;

            int noisedData;

            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    noisedData = random.Next(-(int)(_decimalPercent * highestByte), +(int)(_decimalPercent * highestByte));

                    noisedImage[i, j] = new Rgba32(
                        ImageUtility.RestrictOverflow(input[i, j].R + noisedData),
                        ImageUtility.RestrictOverflow(input[i, j].G + noisedData),
                        ImageUtility.RestrictOverflow(input[i, j].B + noisedData),
                        input[i, j].A
                    );
                }
            }

            return noisedImage;
        }

        public N_Noise(float decimalPercent)
        {
            _decimalPercent = decimalPercent;
        }
    }

    public class N_Vignette : BaseNode
    {
        public override string NodeName
        {
            get { return string.Format("Vignette"); }
        }

        public override ImgProps Process(ImgProps input)
        {
            double centerX = input.Width / 2;
            double centerY = input.Height / 2;
            double maxDistance = Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(centerY, 2));

            double distX;
            double distY;
            double distance;

            double brightness;

            ImgProps vignetteImage = new ImgProps(input.Width, input.Height);

            for (int i = 0; i < input.Width; i++)
            {
                for (int j = 0; j < input.Height; j++)
                {
                    distX = centerX - i;
                    distY = centerY - j;

                    distance = (int)(Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2)));

                    brightness = Math.Pow(((maxDistance - distance) / maxDistance), 2);

                    vignetteImage[i, j] = new Rgba32(
                        ImageUtility.RestrictOverflow((int)(input[i, j].R * brightness)),
                        ImageUtility.RestrictOverflow((int)(input[i, j].G * brightness)),
                        ImageUtility.RestrictOverflow((int)(input[i, j].B * brightness)),
                        input[i, j].A
                    );
                }
            }

            return vignetteImage;
        }
    }

    public class N_Convolve : BaseNode
    {
        private double[,] selectedKernel;
        //Store kernel type
        private readonly string _kernelType;

        public override string NodeName
        {
            get { return string.Format("Convolution ({0})", _kernelType); }
        }

        public override ImgProps Process(ImgProps input)
        {
            if (_kernelType == "edge")
                selectedKernel = ImageUtility.edge;
            else if (_kernelType == "sharpen")
                selectedKernel = ImageUtility.sharpen;
            else if (_kernelType == "blur")
                selectedKernel = ImageUtility.blur;

            int imageWidth = input.Width;
            int imageheight = input.Height;
            int kernelWidth = selectedKernel.GetLength(0);
            int kernelHeight = selectedKernel.GetLength(1);
            int offset = (kernelWidth - 1) / 2;

            ImgProps convolved = new ImgProps(imageWidth - offset, imageheight - offset);

            for (int i = offset; i < input.Width - offset; i++)
            {
                for (int j = offset; j < input.Height - offset; j++)
                {
                    double red = 0;
                    double green = 0;
                    double blue = 0;

                    for (int a = 0; a < kernelWidth; a++)
                    {
                        for (int b = 0; b < kernelHeight; b++)
                        {
                            red += input[i + a - (offset), j + b - (offset)].R * selectedKernel[a, b];
                            green += input[i + a - (offset), j + b - (offset)].G * selectedKernel[a, b];
                            blue += input[i + a - (offset), j + b - (offset)].B * selectedKernel[a, b];
                        }
                    }

                    red = ImageUtility.RestrictOverflow((int)red);
                    green = ImageUtility.RestrictOverflow((int)green);
                    blue = ImageUtility.RestrictOverflow((int)blue);


                    convolved[i, j] = new Rgba32((byte)red, (byte)green, (byte)blue);
                }
            }
            return convolved;
        }

        public N_Convolve(string kernelType)
        {
            _kernelType = kernelType;
        }
    }

    public static class ImageUtility
    {
        private const int lowerLimit = 0;
        private const int upperLimit = 255;

        public static byte RestrictOverflow(int intensity)
        {
            if (intensity < lowerLimit)
                return (byte)lowerLimit;
            else if (intensity > upperLimit)
                return (byte)upperLimit;

            return (byte)intensity;
        }

        public static readonly double[,] edge = new double[,]
        {
            { 0, 1, 0 },
            { 1, -4, 1 },
            { 0, 1, 0 }
        };

        public static readonly double[,] sharpen = new double[,]
        {
            { 0, -1, 0 },
            { -1, 5, -1 },
            { 0, -1, 0 }
        };

        public static readonly double[,] blur = new double[,]
        {
            { (double)1/256, (double)4/256, (double)6/256, (double)4/256, (double)1/256 },
            { (double)4/256, (double)16/256, (double)24/256, (double)16/256, (double)4/256 },
            { (double)6/256, (double)24/256, (double)36/256, (double)24/256, (double)6/256 },
            { (double)4/256, (double)16/256, (double)24/256, (double)16/256, (double)4/256 },
            { (double)1/256, (double)4/256, (double)6/256, (double)4/256, (double)1/256 }
        };
    }
}