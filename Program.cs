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

                        byte emissionVal = (byte) (40 <= pixel.B ? (pixel.B - 39) * (255.0/215.0) : 0);
                        emission.SetPixel(x, y, Color.FromArgb(emissionVal,emissionVal,emissionVal));

                        byte undyedMetalVal = (byte) (pixel.A <= 32 ? pixel.A * (255.0/32.0) : 255);
                        undyedMetalness.SetPixel(x, y, Color.FromArgb(undyedMetalVal,undyedMetalVal,undyedMetalVal));

                        byte dyemaskVal = (byte) (pixel.A <= 40 ? 0 : 255);
                        dyemaskThreshold.SetPixel(x, y, Color.FromArgb(dyemaskVal,dyemaskVal,dyemaskVal));

                        byte wearmaskVal = (byte) (48 <= pixel.A ? (pixel.A - 48) * (255.0/207.0) : 0);
                        wearmask.SetPixel(x, y, Color.FromArgb(wearmaskVal,wearmaskVal,wearmaskVal));
                    }
                }

                if (!Directory.Exists(Path.GetFileNameWithoutExtension(args[0])+"_split"))
                    Directory.CreateDirectory(Path.GetFileNameWithoutExtension(args[0])+"_split");

                ambientOcclusion.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_AO.png"));
                smoothness.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_Smoothness.png"));
                transparency.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_Transparency.png"));
                emission.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_Emission.png"));
                undyedMetalness.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_UndyedMetalness.png"));
                dyemaskThreshold.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_DyemaskThreshold.png"));
                wearmask.Save(Path.Combine(Path.GetFileNameWithoutExtension(args[0])+"_split", Path.GetFileNameWithoutExtension(args[0]) + "_Wearmask.png"));
            }

            else if (Directory.Exists(args[0]))
            {
                string directoryName = args[0].Substring(Path.GetDirectoryName(args[0]).Length);
                string textureName = directoryName.Substring(1,directoryName.Length-7);
                
                Bitmap ambientOcclusion = null;
                Bitmap smoothness = null;
                Bitmap transparency = null;
                Bitmap emission = null;
                Bitmap undyedMetalness = null; 
                Bitmap dyemaskThreshold = null;
                Bitmap wearmask = null;
                
                foreach (string texture in Directory.GetFiles(args[0]))
                {
                    switch (Path.GetFileNameWithoutExtension(texture).Substring(textureName.Length+1))
                    {
                        case ("AO"):
                            ambientOcclusion = new Bitmap(texture, false);
                            Console.WriteLine("AO read");
                            break;
                        case ("Smoothness"):
                            smoothness = new Bitmap(texture, false);
                            Console.WriteLine("Smoothness read");
                            break;
                        case ("Transparency"):
                            transparency = new Bitmap(texture, false);
                            Console.WriteLine("Transparency read");
                            break;
                        case ("Emission"):
                            emission = new Bitmap(texture, false);
                            Console.WriteLine("Emission read");
                            break;
                        case ("UndyedMetalness"):
                            undyedMetalness = new Bitmap(texture, false);
                            Console.WriteLine("Undyed metalnes read");
                            break;
                        case ("DyemaskThreshold"):
                            dyemaskThreshold = new Bitmap(texture, false);
                            Console.WriteLine("Dyemask threshold");
                            break;
                        case ("Wearmask"):
                            wearmask = new Bitmap(texture, false);
                            Console.WriteLine("Wearmask read");
                            break;
                    }
                }

                Bitmap gstack = new Bitmap(ambientOcclusion.Width, ambientOcclusion.Height);

                for (int x=0; x<gstack.Width; x++)
                {
                    for (int y=0; y<gstack.Height; y++)
                    {
                        byte red = ambientOcclusion.GetPixel(x,y).R;

                        byte green = smoothness.GetPixel(x,y).R;

                        byte blue = (byte) (emission.GetPixel(x,y).R > 0 ? emission.GetPixel(x,y).R * 215.0/255.0 + 40.0 : transparency.GetPixel(x,y).R * 32.0/255.0);

                        byte alpha = (byte) (wearmask.GetPixel(x,y).R > 0 ? wearmask.GetPixel(x,y).R * 207.0/255 + 48.0 : dyemaskThreshold.GetPixel(x,y).R > 0 ? dyemaskThreshold.GetPixel(x,y).R * 8.0/255.0 + 36.0 : undyedMetalness.GetPixel(x,y).R * 32.0/255.0);

                        Color pixel = Color.FromArgb(alpha,red,green,blue);

                        gstack.SetPixel(x,y,pixel);

                    }
                }
                
                gstack.Save(Path.Combine(Path.GetDirectoryName(args[0]),textureName+"_Combined.png"));
            }
        }
    }
}