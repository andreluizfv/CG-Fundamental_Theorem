using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using BigGustave;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using MathNet.Numerics.LinearAlgebra;

namespace ConsoleApp1 {
    class Program {
        const int x = 0, y = 1;
        static void Main(string[] args) {
            //step one: choosing images
            PathsAndConstants constants = new PathsAndConstants();
            Png backGround = openFunc(constants.bgPath), texture = openFunc(constants.texturePath);
            
            applyTexture(backGround, texture, constants.finalPath, constants.aimPoints);
            Png test = openFunc(constants.testPath);
            var builder = PngBuilder.FromPng(test);
            var black = new Pixel(0, 0, 0);
            builder.SetPixel(200,0,200, 1, 1);
            
            
            //Pixel topLeft = test.GetPixel(0, 0);
            //Console.WriteLine(topLeft.R + ", " + topLeft.G + "," + topLeft.B);
        }
        /// <summary>
        /// put the texture in the desired area
        /// </summary>
        /// <param name="backGround"> png image where the texture will be placed</param>
        /// <param name="texture">png image of the texture</param>
        /// <param name="finalPath">path to save the background embed with the texture</param>
        /// <param name="aimPoints">4 dots that delimit the background area to place the texture</param>
        private static void applyTexture(Png backGround, Png texture, string finalPath, Matrix<Double> aimPoints)
        {
            var textureDim = Matrix<double>.Build.DenseOfArray(new double[,]
                {{0, 0}, {texture.Width-1, 0}, {0, texture.Height-1}, {texture.Width-1, texture.Height-1 }});
            var T = projectiveTransformationFinder(aimPoints, textureDim);
            var builder = PngBuilder.FromPng(backGround);
            var aimPixelCoord = Vector<double>.Build.Dense(0);
            for (int i = 0; i < backGround.Width; i++) {
                for (int j = 0; j < backGround.Height; j++){
                    if (isInRegion(i,j,aimPoints)) {
                        aimPixelCoord = T.Solve(Vector<double>.Build.Dense(new double[] { i, j, 1 }));
                        paintPixel(builder, aimPixelCoord, texture);
                    }
                }
            }

            saveImage(builder, finalPath);
        }

        private static bool isInRegion(int x, int y, Matrix<double> quadrilateral)
        {
            throw new NotImplementedException();
        }

        private static void paintPixel(PngBuilder builder, Vector<double> aimPixelCoord, Png texture)
        {
            var newColor = texture.GetPixel(Convert.ToUInt16(aimPixelCoord[x]), Convert.ToUInt16(aimPixelCoord[x]));
        }


        static Png openFunc(string path) {

            using (var stream = File.OpenRead(path)) {
                Png img = Png.Open(stream);
                return img;
            }
            //Pixel pixel  = img.GetPixel(img.Width - 1, img.Height);
            //pixel.R
        }
/* To solve step 3, we need:
 *  We have the equations:
 * x1,y1 = background coordinates to apply texture from constants
 * X1,Y1 = texture image dimensions
 * a b c * X1 = lambda1 * x1 => aX1 + bY1 + c = x1 => X1*a + Y1*b + 1*c + 0*d + 0*e + 0*f + 0*g + 0*h + 0*i + 0*l2 + 0*l3 + 0*l4 = x1
 * d e f   Y1     (1)     y1    dX1 + eY1 + f = y1 => 0*a + 0*b + 0*c + X1*d + Y1*e + 1*f + 0*g + 0*h + 0*i + 0*l2 + 0*l3 + 0*l4 = y1
 * g h i   1              1     gX1 + hY1 + i = 1  => 0*a + 0*b + 0*c + 0*d + 0*e + 0*f + X1*g + Y1*h + 1*i + 0*l2 + 0*l3 + 0*l4 = 1
 *
 * a b c * X2 = lambda2 * x2 => aX2 + bY2 + c = l2*x2 => X2*a + Y2*b + 1*c + 0*d + 0*e + 0*f + 0*g + 0*h + 0*i - x2*l2 + 0*l3 + 0*l4 = 0
 * d e f   Y2             y2    dX2 + eY2 + f = l2*y2 => 0*a + 0*b + 0*c + X2*d + Y2*e + 1*f + 0*g + 0*h + 0*i - y2*l2 + 0*l3 + 0*l4 = 0
 * g h i   1              1     gX2 + hY2 + i = l2    => 0*a + 0*b + 0*c + 0*d + 0*e + 0*f + X2*g + Y2*h + 1*i - 1*l2 + 0*l3 + 0*l4 = 0
 *
 * a b c * X3 = lambda3 * x3 => aX3 + bY3 + c = l3*x3 => X3*a + Y3*b + 1*c + 0*d + 0*e + 0*f + 0*g + 0*h + 0*i + 0*l2 - x3*l3 + 0*l4 = 0
 * d e f   Y3             y3    dX3 + eY3 + f = l3*y3 => 0*a + 0*b + 0*c + X3*d + Y3*e + 1*f + 0*g + 0*h + 0*i + 0*l2 - y3*l3 + 0*l4 = 0
 * g h i   1              1     gX3 + hY3 + i = l3    => 0*a + 0*b + 0*c + 0*d + 0*e + 0*f + X3*g + Y3*h + 1*i + 0*l2 - 1*l3 + 0*l4 = 0
 *
 * a b c * X4 = lambda4 * x4 => aX4 + bY4 + c = l4*x4 => X4*a + Y4*b + 1*c + 0*d + 0*e + 0*f + 0*g + 0*h + 0*i + 0*l2 + 0*l3 - x4*l4 = 0
 * d e f   Y4             y4    dX4 + eY4 + f = l4*y4 => 0*a + 0*b + 0*c + X4*d + Y4*e + 1*f + 0*g + 0*h + 0*i + 0*l2 + 0*l3 - y4*l4 = 0
 * g h i   1              1     gX4 + hY4 + i = l4    => 0*a + 0*b + 0*c + 0*d + 0*e + 0*f + X4*g + Y4*h + 1*i + 0*l2 + 0*l3 - 1*l4 = 0
 *
 * =>
 *
 *  ( X1 Y1 1  0  0  0  0  0  0  0  0  0  ) (  a )  ( x1)
 *  ( 0  0  0  X1 Y1 1  0  0  0  0  0  0  ) (  b )  ( y1)
 *  ( 0  0  0  0  0  0  X1 Y1 1  0  0  0  ) (  c )  ( 1 )
 *  ( X2 Y2 1  0  0  0  0  0  0 -x2 0  0  ) (  d )  ( 0 )
 *  ( 0  0  0  X2 Y2 1  0  0  0 -y2 0  0  ) (  e )  ( 0 )
 *  ( 0  0  0  0  0  0  X2 Y2 1 -1  0  0  ) (  f ) =( 0 )
 *  ( X3 Y3 1  0  0  0  0  0  0  0 -x3 0  ) (  g )  ( 0 )
 *  ( 0  0  0  X3 Y3 1  0  0  0  0 -y3 0  ) (  h )  ( 0 )
 *  ( 0  0  0  0  0  0  X3 Y3 1  0 -1  0  ) (  i )  ( 0 )
 *  ( X4 Y4 1  0  0  0  0  0  0  0  0 -x4 ) ( l2 )  ( 0 )
 *  ( 0  0  0  X4 Y4 1  0  0  0  0  0 -y4 ) ( l3 )  ( 0 )
 *  ( 0  0  0  0  0  0  X4 Y4 1  0  0 -1  ) ( l4 )  ( 0 )
 */
//
/// <summary>
/// Find the matrix T that maps pixels of the area designed by aim points in pixels in texture
/// </summary>
/// <param name="aimPoints"> area designed in the back ground, in the shape {{x1,y1} {x2,y2} .. {x4,y4}}</param>
/// <param name="texture"> matrix with 4 pixels in each edge of the texture, in the shape  in the shape {{x1,y1} {x2,y2}
/// .. {x4,y4}}, x1 y1 -> 0,0, x2,y2 -> width,0 ... </param>
/// <returns></returns>
        static Matrix<double> projectiveTransformationFinder(Matrix<double> aimPoints, Matrix<double> texture){
            var A = Matrix<double>.Build.DenseOfArray(new double[,] { //math explained in read.me
                { texture[0,x], texture[0,y], 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, texture[0,x], texture[0,y], 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, texture[0,x], texture[0,y], 1, 0, 0, 0 },
                { texture[1,x], texture[1,y], 1, 0, 0, 0, 0, 0, 0, -aimPoints[1,x], 0, 0 },
                { 0, 0, 0, texture[1,x], texture[1,y], 1, 0, 0, 0, -aimPoints[1,y], 0, 0 },
                { 0, 0, 0, 0, 0, 0, texture[1,x], texture[1,y], 1, -1, 0, 0 },
                { texture[2,x], texture[2,y], 1, 0, 0, 0, 0, 0, 0, 0, -aimPoints[2,x], 0 },
                { 0, 0, 0, texture[2,x], texture[2,y], 1, 0, 0, 0, 0, -aimPoints[2,y], 0 },
                { 0, 0, 0, 0, 0, 0, texture[2,x], texture[2,y], 1, 0, -1, 0 },
                { texture[3,x], texture[3,y], 1, 0, 0, 0, 0, 0, 0, 0, 0, -aimPoints[3,x] },
                { 0, 0, 0, texture[3,x], texture[3,y], 1, 0, 0, 0, 0, 0, -aimPoints[3,y] },
                { 0, 0, 0, 0, 0, 0, texture[3,x], texture[3,y], 1, 0, 0, -1}, });
            var b = Vector<double>.Build.Dense(new double[] {
                aimPoints[0,x], aimPoints[0,y], 1, 0, 0, 0, 0, 0, 0, 0, 0, 0});
            var s = A.Solve(b); //step 3: solve the transformation to texture->image (it's the 3x3 inside)
            return Matrix<double>.Build.DenseOfArray(new double[,]
                {{s[0], s[1], s[2]}, {s[3], s[4], s[5]}, {s[6], s[7], s[8]}}).Inverse(); //step 4: solve image->texture
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

