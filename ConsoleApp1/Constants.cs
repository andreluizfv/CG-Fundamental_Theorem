using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection.Metadata;
//C:\Users\andre\source\repos\ConsoleApp1\ConsoleApp1\imgToReshape.png
//C: \Users\andre\source\repos\ConsoleApp1\ConsoleApp1\bin\Debug\netcoreapp2.1\
namespace ConsoleApp1 {
    internal class PathsAndConstants {
        static string absPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        readonly public string bgPath = Path.Combine(absPath,@"background.png");
        readonly public string imgToReshapePath = Path.Combine(absPath, @"imgToReshape.png");
        readonly public string testPath = Path.Combine(absPath, @"test.png");

    }
}
