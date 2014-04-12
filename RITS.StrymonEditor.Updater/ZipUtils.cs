using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using System.Text;

namespace RITS.StrymonEditor.Updater
{
    public static class ZipUtils
    {
        public static void ExtractZip(string zipPath, string targetFolder,Action<int,string> startDelegate=null, Action<int,string,string> progressDelegate=null)
        {
            int counter = 0;
            ZipFile zf = null;
            string tmpFolder = Path.Combine(targetFolder, "AutoUpdateFiles");
            Directory.CreateDirectory(tmpFolder);
            foreach (var f in Directory.EnumerateFiles(tmpFolder))
            {
                File.Delete(f);
            }
            try
            {
                FileStream fs = File.OpenRead(zipPath);
                zf = new ZipFile(fs);
                if(startDelegate!=null) startDelegate(Convert.ToInt32(zf.Count),"Install");
                foreach (ZipEntry zipEntry in zf)
                {
                    counter++;
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    if (progressDelegate != null) progressDelegate(counter, entryFileName,"Installing...");
                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);
                    String fullZipToPath = Path.Combine(targetFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);
                    try
                    {
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                    catch (IOException ex)
                    {
                        RITS.StrymonEditor.Logging.StaticLogger.Error(ex);
                        Directory.CreateDirectory(tmpFolder);
                        string tempPath = Path.Combine(tmpFolder, Path.GetFileName(fullZipToPath));
                        using (FileStream streamWriter = File.Create(tempPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }

            }
        }
        // Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        //
        public static void ZipFolder(string targetPath, string sourceFolder, Action<int,string> startDelegate = null, Action<int, string,string> progressDelegate = null)
        {
            FileStream fsOut = File.Create(targetPath);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            int folderOffset = sourceFolder.Length + (sourceFolder.EndsWith("\\") ? 0 : 1);
            CompressFolder(sourceFolder, zipStream, folderOffset,startDelegate, progressDelegate);
            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }


        public static void ExtractFile(string zipPath, string sourceFileName, string targetPath)
        {
            using (var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                using (var zf = new ZipFile(fs))
                {
                    var ze = zf.GetEntry(sourceFileName);
                    if (ze == null)
                    {
                        throw new ArgumentException(sourceFileName, "not found in Zip");
                    }
                    byte[] buffer = new byte[4096];
                    using (var s = zf.GetInputStream(ze))
                    {
                        using (FileStream streamWriter = File.Create(targetPath))
                        {
                            StreamUtils.Copy(s, streamWriter, buffer);
                        }
                    }
                }
}        }

        // Recurses down the folder structure
        //
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, Action<int,string> startDelegate, Action<int, string,string> progressDelegate)
        {
            string[] files = Directory.GetFiles(path);
            if (startDelegate != null) startDelegate(files.Count(),"Backup");
            int counter = 0; 
            foreach (string filename in files)
            {
                if (Path.GetExtension(filename) == ".log") continue;
                counter++;
                FileInfo fi = new FileInfo(filename);
                if (progressDelegate != null) progressDelegate(counter, filename, "Backup...");

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);
                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                // TODO make hack configurable
                if (!folder.EndsWith("PreviousVersions"))
                {
                    CompressFolder(folder, zipStream, folderOffset, startDelegate, progressDelegate);
                }
            }
        }
    }
}
