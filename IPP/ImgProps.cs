using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace ImgProps
{
    public class ImgProps
    {
        private Image<Rgba32> _data;
        public int Width { get { return _data.Width; } }
        public int Height { get { return _data.Height; } }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }

        public int GetGreyIntensity(int x, int y) => (int)((
            _data[x, y].R +
            _data[x, y].G +
            _data[x, y].B
        ) / 3);

        public static ImgProps Resize(ImgProps img, int newWidth, int newHeight)
        {
            ResizeOptions options = new ResizeOptions();
            options.Sampler = KnownResamplers.NearestNeighbor;
            options.Size = new SixLabors.Primitives.Size();
        }

        //Save image
        public string Write(string filePath)
        {
            _data.Save($"{filePath}.png");
            return $"{filePath}.png";
        }

        //Constructors
        public ImgProps(string filePath)
        {
            _data = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
        }

        public ImgProps(Image<Rgba32> data)
        {
            _data = data.Clone();
        }

        public ImgProps(ImgProps img)
        {
            _data = img._data.Clone();
        }
    }
}