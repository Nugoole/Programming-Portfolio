using ALT.CVL;
using ALT.CVL.Core;

using ALT.CVL.SICK;

using Microsoft.Win32;



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Test
{
    delegate void OnImageGrabbedFunc(int width, int height, IntPtr bufferPtr);

    class Param
    {
        public string ParamName { get; set; }
        public string Value { get; set; }

    }
    class Program
    {
        static int countToSpace = 0;

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        [DllImport("BaslerCPP.dll")]
        internal static extern void AttachOnImageGrabbedDelegate(OnImageGrabbedFunc func);
        [DllImport("BaslerCPP.dll")]
        internal static extern void ConnectAndGrabOne(out int width, out int height, out IntPtr buffer);
        [DllImport("BaslerCPP.dll")]
        internal static extern void Init();
        [DllImport("BaslerCPP.dll")]
        internal static extern void Destroy();

        static Stopwatch watch;
        static void OnImageGrabbedFunction(int width, int height, IntPtr bufferPtr)
        {
            byte[] buffer = new byte[width * height];
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            //var colorPalette = bmp.Palette;
            //for (int i = 0; i < 256; i++)
            //{
            //    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
            //}
            //bmp.Palette = colorPalette;

            watch.Start();
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            Marshal.Copy(bufferPtr, buffer, 0, width * height);
            CopyMemory(bmpdata.Scan0, bufferPtr, (uint)(width * height));
            bmp.UnlockBits(bmpdata);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            bmp.Save(@"C:\Users\MSI\Desktop\testImage.bmp", ImageFormat.Bmp);
        }

        static void Main(string[] args)
        {
            SICKGrabber grabber = new SICKGrabber();
            //grabber.Connect(grabber.AvailableCameras[1]);

            //Node<Param> CurrentNode = null;
            //Tree<Param> paramTree = new Tree<Param>();
            //Console.ReadLine();


            //grabber.GetCurrentParameterTree(UsersetID.USER_SET_3);

            //Console.ReadLine();

            grabber.LoadImage(@"C:\Users\MSI\Desktop\Images\calibration zig arrow calibrated.dat");
            grabber.Load2DImage(@"C:\Users\MSI\Desktop\sick setup 20220119\FOV.dat");

            var frame = Sick.GenIStream.IFrame.Load(@"C:\Users\MSI\Desktop\sick setup 20220119\FOV.dat");
            grabber.Calibrate(@"C:\Users\MSI\Desktop\sick setup 20220119\21400039_20220119\CalibrationResult.json", frame);

            Console.ReadLine();


            //var stringLines = File.ReadAllLines(@"C:\Users\MSI\Desktop\parameter.csv");
            //foreach (var item in stringLines)
            //{
            //    if (!item.Equals("#Root") && CurrentNode == null)
            //        continue;

            //    if (CurrentNode == null)
            //        CurrentNode = paramTree.MainNode;

            //    if (item.StartsWith("#"))
            //    {
            //        if (CurrentNode.ParentNode != null)
            //            CurrentNode = CurrentNode.ParentNode;

            //        CurrentNode.AddChild(new Param { ParamName = item.TrimStart(new char[] { '#' }), Value = null });
            //        CurrentNode = CurrentNode.ChildNodes.Last();
            //    }
            //    else
            //    {
            //        var splitString = item.Split(',');


            //        var newParam = new Param { ParamName = splitString[0], Value = splitString[1] };
            //        CurrentNode.AddChild(newParam);
            //    }
            //}
            //List<string> paramsString = new List<string>();
            //foreach (var item in paramTree)
            //{
            //    if (item != null)
            //        paramsString.Add(item.ParamName);
            //}


            //File.WriteAllLines(@"C:\Users\MSI\Desktop\param.txt", paramsString);
            //Console.ReadLine();
            //watch = new Stopwatch();


            //Console.ReadLine();
            //IntPtr bufferPtr = new IntPtr();


            //Init();
            //AttachOnImageGrabbedDelegate(OnImageGrabbedFunction);
            //ConnectAndGrabOne(out int width, out int height, out bufferPtr);



            //Destroy();
            //Console.ReadLine();


            //milTest a = new milTest();

            //a.GrabAndBinarize();

            //Console.ReadLine();









            //XmlDocument xml = new XmlDocument();
            //xml.Load(@"C:\Users\MSI\Downloads\GenICam_Package_2021.02\SFNC\SFNC_Device_Reference_Version_2_6_0_Schema_1_1_xml.xml");


            //var groups = xml.DocumentElement.ChildNodes.Cast<XmlNode>().Where(x=>x.LocalName.Equals("Group"));
            //var names = groups.Select(x => x.InnerText);
            //var paramNodes = new TreeNode<string>("Root");

            //var rootCategories = groups.Where(x => x.Attributes[0].Value.Equals("RootCategory")).FirstOrDefault();
            //var subCategories = groups.Where(x => x.Attributes[0].Value.Equals("SubCategories")).FirstOrDefault();

            //var rootPFeatures = rootCategories.ChildNodes[0].ChildNodes.Cast<XmlNode>().Where(x => x.LocalName.Equals("pFeature"));
            ////Root Categories
            //foreach (XmlNode item in rootPFeatures)
            //{
            //    paramNodes.AddChild(item.InnerText);
            //}

            //foreach (var child in paramNodes.ChildNodes)
            //{
            //    var category = subCategories.Cast<XmlNode>().First(x => x.Attributes["Name"].Value.Equals(child.Content));
            //    foreach (XmlNode node in category.ChildNodes.Cast<XmlNode>().Where(x=>x.LocalName.Equals("pFeature")))
            //    {
            //        child.AddChild(node.InnerText);
            //    }
            //}







            //Console.ReadLine();

            //var cameras = CameraConnector.Instance.GetAllAvailableCameras();
            //var camera = cameras[0].Connect();

            //Bitmap bitmap = new Bitmap(1024, 1024, PixelFormat.Format8bppIndexed);
            //var colorPalette = bitmap.Palette;
            //for (int i = 0; i < 256; i++)
            //{
            //    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
            //}
            //bitmap.Palette = colorPalette;
            //byte[] pixelBytes = new byte[1024 * 1024];



            //for (int k = 0; k < 16; k++)
            //{
            //    for (int i = 0; i < 1024 / 16; i++)
            //    {
            //        for (int j = 0; j < 1024; j++)
            //        {
            //            pixelBytes[j + (i + 1024 / 16 * k) * 1024] = (byte)(k * 16);
            //        }
            //    }
            //}


            //var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //Marshal.Copy(pixelBytes, 0, bitmapData.Scan0, pixelBytes.Length);
            //bitmap.UnlockBits(bitmapData);

            //bitmap.Save(@"C:\Users\MSI\Desktop\image.bmp", ImageFormat.Bmp);

            //Console.ReadLine();


            //TcpClient client = new TcpClient("192.168.10.100", 23);

            //if (client.Connected)
            //    Console.WriteLine("Connected!");

            //var stream = client.GetStream();


            //Thread thread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        byte[] readByte = new byte[1000];
            //        stream.Read(readByte, 0, readByte.Length);
            //        Console.WriteLine($"{DateTime.Now.ToString("HH-mm-ss:fff")} | {Encoding.ASCII.GetString(readByte).Trim('\0')}");
            //    }
            //});
            //thread.Start();

            //while (true)
            //{
            //    if (Console.ReadLine() == "t")
            //        for (int i = 0; i < 30; i++)
            //        {
            //            Console.WriteLine("Send Text : ");
            //            //var strToSend = Console.ReadLine();
            //            //if (strToSend == "exit")
            //            //    break;

            //            var strToSend = "||>trigger on" + Environment.NewLine;
            //            Console.Write($"{DateTime.Now.ToString("HH-mm-ss:fff")} | {strToSend}");
            //            var sendBytes = Encoding.ASCII.GetBytes(strToSend);
            //            stream.Write(sendBytes, 0, sendBytes.Length);

            //            Task.Delay(500).Wait();
            //        }
            //}
            
        }


    }
}
