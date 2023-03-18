using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using BigGustave;
using Microsoft.VisualBasic;
namespace ConsoleApp1 {
    class Program {
        public XmlDocument constants = new XmlDocument();

        static void Main(string[] args) {
            PathsAndConstants constants = new PathsAndConstants();
            Png backGround = openFunc(constants.bgPath);
            Png imgToReshape = openFunc(constants.imgToReshapePath);
           
        }
        void loadConstants() {
            constants.PreserveWhitespace = true;
            try { constants.Load("XMLFile1.xml"); }
            catch(System.IO.FileNotFoundException) {
                constants.LoadXml("<error> error <error>");
            }
        }
        static Png openFunc(string path) {

            using (var stream = File.OpenRead(path)) {
                Png img = Png.Open(stream);
                return img;
            }
            //Pixel pixel  = img.GetPixel(img.Width - 1, img.Height);
            //pixel.R
        }
    }





}
/*
 * Png test = openFunc(constants.testPath);
            Pixel botRight = test.GetPixel(test.Width - 1, test.Height - 1);
            Pixel botLeft = test.GetPixel(0, test.Height - 1);
            Pixel topRight = test.GetPixel(test.Height - 1,0);
            Pixel topLeft = test.GetPixel(0, 0);
            Console.WriteLine("bot Right: " + botRight.R +","+ botRight.G + "," + "," + botRight.B);
            Console.WriteLine("bot Left: " + botLeft.R + "," + botLeft.G + "," + botLeft.B);
            Console.WriteLine("top Right: " + topRight.R + "," + topRight.G + "," + topRight.B);
            Console.WriteLine("top Left: " + topLeft.R + "," + topLeft.G + "," + topLeft.B);
*/

