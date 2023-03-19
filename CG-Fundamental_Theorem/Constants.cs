using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection.Metadata;
using System.Data.SqlTypes;
using MathNet.Numerics.LinearAlgebra;
//C:\Users\andre\source\repos\ConsoleApp1\ConsoleApp1\imgToReshape.png
//C: \Users\andre\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp2.1\
namespace ConsoleApp1
{
    internal class PathsAndConstants
    {
        static string absPath =
            Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

        public readonly string bgPath = Path.Combine(absPath, @"background.png");
        public readonly string texturePath = Path.Combine(absPath, @"imgToReshape.png");
        public readonly string finalPath = Path.Combine(absPath, @"final.png");
        public readonly string testPath = Path.Combine(absPath, @"test.png");

        //step 2: choosing quadrilateral`s area
        /// <summary>
        /// array of coordinates {x1, y1, x2, y2, x3, y3, x4, y4}
        /// (x1,y1) top left, (x2,y2) top right
        /// (x3,y3) bottom left, (x4,y4) bottom right
        /// </summary>
        public readonly Matrix<double> aimPoints = Matrix<double>.Build.DenseOfArray(new double [,] {
            {212, 218},
            {438, 172},
            {191, 411},
            {429, 389}});
    }
}
