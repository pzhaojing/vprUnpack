using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;


namespace VprUnpack
{
	class Program
	{

		static void VPR_Unpack (string vpr)
		{
			string path = vpr;
			string dir = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
			Directory.CreateDirectory(dir);
			(new FastZip()).ExtractZip(path, dir, "");

			VentuzProjectBuilding ventuz = new VentuzProjectBuilding(dir);
			ventuz.Building();
		}

		static void Main(string[] args)
		{
            Console.WriteLine("VPR UnPack 1.0 - For Ventuz5  Ventuz6");
			string cd =  Directory.GetCurrentDirectory();
			string[] files = args;
			if (files.Length == 0)
			{
				files = Directory.GetFiles(cd, "*.vpr");
			}

			for (int i = 0; i < files.Length; i++)
			{
				Directory.SetCurrentDirectory(cd);
				Console.WriteLine("unpack[{0}/{1}] : {2}", i+1, files.Length,files[i]);
				try
				{
					VPR_Unpack(files[i]);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
				
			}
		}
	}
}
