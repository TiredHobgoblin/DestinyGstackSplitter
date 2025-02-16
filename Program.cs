using System;
using System.IO;
using System.Drawing;

namespace DestinyGstackSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(args[0]))
            {
                Bitmap gstack = new Bitmap(args[0], false);
                Bitmap ambientOcclusion = new Bitmap(gstack.Width, gstack.Height);
                Bitmap smoothness = new Bitmap(gstack.Width, gstack.Height);
                Bitmap transparency = new Bitmap(gstack.Width, gstack.Height);
                Bitmap emission = new Bitmap(gstack.Width, gstack.Height);
                Bitmap undyedMetalness = new Bitmap(gstack.Width, gstack.Height); 
                Bitmap dyemaskThreshold = new Bitmap(gstack.Width, gstack.Height);
                Bitmap wearmask = new Bitmap(gstack.Width, gstack.Height);

                for (int x=0; x<gstack.Width; x++)
                {
                    for (int y=0; y<gstack.Height; y++)
                    {
                        Color pixel = gstack.GetPixel(x,y);

                        ambientOcclusion.SetPixel(x, y, Color.FromArgb(pixel.R,pixel.R,pixel.R));

                        smoothness.SetPixel(x, y, Color.FromArgb(pixel.G,pixel.G,pixel.G));

                        byte transparencyVal = (byte) (pixel.B <= 32 ? pixel.B * (255.0/32.0) : 255);
                        transparency.SetPixel(x, y, Color.FromArgb(transparencyVal,transparencyVal,transparencyVal));

                        byte emissionVal = (byte) (40 <= pixel.A ? 0 : (pixel.B - 40) * (255.0/215.0));
                        emission.SetPixel(x, y, Color.FromArgb(emissionVal,emissionVal,emissionVal));

                        byte undyedMetalVal = (byte) (pixel.A <= 32 ? pixel.A * (255.0/32.0) : 255);
                        undyedMetalness.SetPixel(x, y, Color.FromArgb(undyedMetalVal,undyedMetalVal,undyedMetalVal));

                        byte dyemaskVal = (byte) (pixel.A <= 40 ? 0 : 255);
                        dyemaskThreshold.SetPixel(x, y, Color.FromArgb(dyemaskVal,dyemaskVal,dyemaskVal));

                        byte wearmaskVal = (byte) (48 <= pixel.A ? 0 : (pixel.A - 48) * (255.0/207.0));
                        wearmask.SetPixel(x, y, Color.FromArgb(wearmaskVal,wearmaskVal,wearmaskVal));
                    }
                }

                if (!Directory.Exists("Output"))
                    Directory.CreateDirectory("Output");

                ambientOcclusion.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_AO.png"));
                smoothness.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_Smoothness.png"));
                transparency.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_Transparency.png"));
                emission.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_Emission.png"));
                undyedMetalness.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_UndyedMetalness.png"));
                dyemaskThreshold.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_DyemaskThreshold.png"));
                wearmask.Save(Path.Combine("Output", Path.GetFileNameWithoutExtension(args[0]) + "_Wearmask.png"));
            }
        }
    }
}
