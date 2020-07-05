using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
namespace VprUnpack
{

	struct STATIC_BIN {
		static public byte[] ManifestV5 = {
			0x56 ,0x42 ,0x49 ,0x4e ,0x05 ,0x00 ,0x14 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x05 ,0x00 ,
			0x00 ,0x00 ,0x01 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0x40 ,0x02 ,0x00 ,0x00 ,0x00 ,0x00 ,
			0x00 ,0x01 ,0x03 ,0x21 ,0x01 ,0x00 ,0x0d ,0x56 ,0x65 ,0x6e ,0x74 ,0x75 ,0x7a ,0x2e ,0x4b ,0x65 ,
			0x72 ,0x6e ,0x65 ,0x6c ,0x31 ,0x01 ,0x00 ,0x22 ,0x01 ,0x00 ,0x24 ,0x56 ,0x65 ,0x6e ,0x74 ,0x75 ,
			0x7a ,0x2e ,0x4b ,0x65 ,0x72 ,0x6e ,0x65 ,0x6c ,0x2e ,0x49 ,0x4f ,0x2e ,0x53 ,0x69 ,0x6e ,0x67 ,
			0x6c ,0x65 ,0x53 ,0x63 ,0x65 ,0x6e ,0x65 ,0x4d ,0x61 ,0x6e ,0x69 ,0x66 ,0x65 ,0x73 ,0x74 ,0x04 ,
			0x11 ,0x01 ,0x00 ,0x00 ,0x00 ,0x32 ,0x01 ,0x00 ,0x05 ,0x01 ,0x05 ,0x01 ,0x1e ,0x1f ,0x02 ,0x56 ,
			0x45 ,0x4e ,0x44
		};
		static public byte[] ManifestV6 = {
			0x56 ,0x42 ,0x49 ,0x4e ,0x05 ,0x00 ,0x14 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x06 ,0x00 ,
			0x00 ,0x00 ,0x01 ,0x00 ,0x00 ,0x00 ,0x04 ,0x00 ,0x00 ,0x00 ,0x40 ,0x02 ,0x00 ,0x00 ,0x00 ,0x00 ,
			0x00 ,0x01 ,0x03 ,0x21 ,0x01 ,0x00 ,0x0d ,0x56 ,0x65 ,0x6e ,0x74 ,0x75 ,0x7a ,0x2e ,0x4b ,0x65 ,
			0x72 ,0x6e ,0x65 ,0x6c ,0x31 ,0x01 ,0x00 ,0x22 ,0x01 ,0x00 ,0x24 ,0x56 ,0x65 ,0x6e ,0x74 ,0x75 ,
			0x7a ,0x2e ,0x4b ,0x65 ,0x72 ,0x6e ,0x65 ,0x6c ,0x2e ,0x49 ,0x4f ,0x2e ,0x53 ,0x69 ,0x6e ,0x67 ,
			0x6c ,0x65 ,0x53 ,0x63 ,0x65 ,0x6e ,0x65 ,0x4d ,0x61 ,0x6e ,0x69 ,0x66 ,0x65 ,0x73 ,0x74 ,0x04 ,
			0x11 ,0x01 ,0x00 ,0x00 ,0x00 ,0x32 ,0x01 ,0x00 ,0x05 ,0x01 ,0x05 ,0x01 ,0x1e ,0x1f ,0x02 ,0x56 ,
			0x45 ,0x4e ,0x44
		};

	}




	class VentuzProjectBuilding
	{
		Dictionary<int, Uri> uirs = new Dictionary<int, Uri>();
		DirectoryInfo dir;
		uint ver = 5;
		public VentuzProjectBuilding(string workdir)
		{
			string urifile = workdir + "\\UriList";
			dir = new DirectoryInfo(Path.GetDirectoryName(urifile));
			BinaryReader br = new BinaryReader(new FileStream(urifile,FileMode.Open));
			uint magic = br.ReadUInt32();
			br.BaseStream.Position += 10;
			ver = br.ReadUInt32();
			switch (ver)
			{
				case 5:
					br.BaseStream.Position += 113;
					break;
				case 6:
					br.BaseStream.Position += 116;
					break;
				default:
					return;
			}

			bool getNexturi()
			{
				byte tmp = br.ReadByte();
				if (tmp != 0xb1) return false;
				int index = br.ReadInt32();
				string name = br.ReadString();
				uirs[index - 1] = new Uri(name.Substring(1));
				return true;
			}
			while (getNexturi())
			{

			}
			br.Close();
		}
		public void Building()
		{
			Directory.SetCurrentDirectory(dir.FullName);
			foreach (int ind in uirs.Keys)
			{
                try
                {
                    createFile(ind);
                }
                catch (Exception)
                {

                }
			}
            try
            {
			    createVZP();
            }
            catch (Exception)
            {

            }
			dir.CreateSubdirectory(".unpack");
			File.Move("Manifest", ".unpack/Manifest");
			File.Move("Snapshot", ".unpack/Snapshot");
			File.Move("UriList", ".unpack/UriList");
		}
		void createVZP()
		{
			string path = "Manifest";
			string outpath = "Project.vzp";
            byte[] manifestData = File.ReadAllBytes(path);
            int startind = -1;
            for (int i = 0; i < manifestData.Length-4; i++)
            {
                if (
                    manifestData[i] == 0x3c         &&
                    manifestData[i + 1] == 0x56     &&
                    manifestData[i + 2] == 0x65     &&
                    manifestData[i + 3] == 0x6e
                    )
                {
                    startind = i;
                    break;
                }
            }
            if (startind==-1)return;

            int endind = -1;
            for (int i = startind; i < manifestData.Length; i++)
            {
                if (manifestData[i] == 0)
                {
                    endind = i;
                    break;
                }
            }
            if (endind == -1)return;


			BinaryWriter bw = new BinaryWriter(new FileStream(outpath, FileMode.Create));
            bw.Write(manifestData,startind,endind-startind);
			bw.Close();
		}
		void createFile(int fileInd)
		{
			Uri uri = uirs[fileInd];
			string fullPath = uri.ToString();
			if (fullPath.StartsWith("ventuz://"))
			{
				string rPath = fullPath.Substring(9).Split('#')[0].Trim();
				string subDir = Path.GetDirectoryName(rPath);
				string fileName = Path.GetFileName(rPath);
				string fileExt = Path.GetExtension(rPath);
				dir.CreateSubdirectory(subDir);
				if (fileExt == ".vzs")
				{
					string tempDir = subDir+"\\"+System.Guid.NewGuid().ToString("N");
					dir.CreateSubdirectory(tempDir);	
					File.Move(fileInd.ToString(), tempDir + "\\Scene");
					File.WriteAllBytes(tempDir + "\\Manifest", ver==6?STATIC_BIN.ManifestV6: STATIC_BIN.ManifestV5);
					File.Copy("Snapshot", tempDir + "\\Snapshot");
					File.Copy("UriList", tempDir + "\\UriList");
					new FastZip().CreateZip(fileName, tempDir, false, "");
					File.Move(fileName, rPath);
					Directory.Delete(tempDir,true);
				}
				else
				{
					File.Move(fileInd.ToString(), rPath);
				}

			}
		}
	}
}
