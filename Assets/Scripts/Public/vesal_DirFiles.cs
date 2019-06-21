using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace VesalCommon
{
    public class Vesal_DirFiles
    {
        public static string findFileInPath(string path, string fileName,bool isRecursion = true)
        {
            string lowFileName = fileName.ToLower() + "";
            DirectoryInfo dif = new DirectoryInfo(path);
            FileSystemInfo[] fsis = dif.GetFileSystemInfos();
            for (int i = 0; i < fsis.Length; i++)
            {
                FileSystemInfo tmp = fsis[i];
                if (tmp is DirectoryInfo && isRecursion)
                {
                    return findFileInPath(tmp.FullName, lowFileName);
                }
                if (get_file_name_from_full_path(tmp.FullName).ToLower() == lowFileName)
                {
                    return get_dir_from_full_path(tmp.FullName);
                }
            }
            return "";
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XmlDocument loadXml(string path, string fileName)
        {

            XmlDocument doc = new XmlDocument();
            string filepath;
            filepath = path + fileName;
            if (filepath.EndsWith(".xml.xml")) {
                filepath = filepath.Replace(".xml.xml", ".xml");
            }
            if (!File.Exists(filepath))
            {
                UnityEngine.Debug.Log("loadXml 文件不存在:" + filepath);
                return null;
            }
            else
            {
                doc.Load(filepath);
            }
            return doc;
        }

        /// <summary>
        /// 解压App压缩包至App id 命名的文件夹下，并返回该文件夹下第一个子文件夹名称
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="firstDirName"></param>
        public static void ExtractAppData(ref string basePath, ref string firstDirName)
        {
            Vesal_DirFiles.DelectDir(basePath);
            Vesal_DirFiles.CreateDir(basePath  + "/");
            Vesal_DirFiles.UnZip(basePath , basePath  + "/", true);
            basePath = basePath +  "/";
            firstDirName = Vesal_DirFiles.GetFirstDirInDir(basePath);
        }
     
        /// <summary>
        /// 获得目标文件夹下第一个子文件夹名词
        /// </summary>
        /// <param name="searchDirPath"></param>
        /// <returns></returns>
        public static string GetFirstDirInDir(string searchDirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(searchDirPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    return i.Name;
                }
            }
            return "";
        }
        /// <summary>
        /// 删除目录下所有子文件夹(递归)
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDirFiles(string srcPath,List<string> filterDirNames = null)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        if (srcPath == i.FullName)
                        {
                            continue;
                        }
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        UnityEngine.Debug.Log(subdir.Name);
                        if (filterDirNames != null && filterDirNames.Contains(subdir.Name)) {
                            continue;
                        }
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                        DelFile(i.FullName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 删除目录下所有子文件夹(递归)
        /// </summary>
        /// <param name="srcPath"></param>
        public static void DelectDir(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        if (srcPath == i.FullName)
                        {
                            continue;
                        }
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static bool UnZip(string sourcePath, string targetPath, bool overWrite = true)
        {
            StreamReader streamReader = new StreamReader(sourcePath, Encoding.Default);
            using (ZipInputStream s = new ZipInputStream(streamReader.BaseStream))
            {
                //File.OpenRead(sourcePath)
                //s.setEncoding("UTF-8");
                ZipEntry theEntry;
                
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    theEntry.IsUnicodeText = true;
                    string directoryName = Path.GetDirectoryName(theEntry.Name);

                    //UnityEngine.Debug.Log("aaa:" + theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(targetPath + directoryName);
                    }
                    string filePath = targetPath + theEntry.Name;
                    if (File.Exists(filePath))
                    {
                        if (overWrite)
                        {

                            File.Delete(filePath);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(filePath))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return true;

            //if (ZipFile.IsZipFile(sourcePath))
            //{
            //    try
            //    {
            //        ZipFile file = new ZipFile(sourcePath, Encoding.Default);
            //        if (!Directory.Exists(targetPath))
            //        {
            //            Directory.CreateDirectory(targetPath);
            //        }
            //        file.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);
            //        file.Dispose();
            //    }
            //    catch (Exception e)
            //    {
            //        UnityEngine.Debug.Log(e.Message);
            //        UnityEngine.Debug.Log(e.StackTrace);
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
            //return true;
        }
        public static IEnumerator UnZipAsync(string sourcePath, string targetPath, Action<float> callProgress, bool overWrite = true)
        {
            targetPath = targetPath.Replace("\\", "/");
            if (!targetPath.EndsWith("/")) {
                targetPath = targetPath + "/";
            }
            CreateDir(get_dir_from_full_path(targetPath));
            StreamReader streamReader = new StreamReader(sourcePath, Encoding.Default);
            long sum = streamReader.BaseStream.Length;
            using (ZipInputStream s = new ZipInputStream(streamReader.BaseStream))
            {
                ZipEntry theEntry;
                
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (callProgress!=null) {
                        callProgress(Mathf.Min((float)s.Position / (float)sum,0.999F));
                    }
                    yield return null;
                    theEntry.IsUnicodeText = true;
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(targetPath + directoryName);
                    }
                    string filePath = targetPath + theEntry.Name;
                    if (File.Exists(filePath))
                    {
                        if (overWrite)
                        {

                            File.Delete(filePath);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (fileName != String.Empty)
                    {
                        
                        using (FileStream streamWriter = File.Create(filePath))
                        {

                            int size = 1024;
                            byte[] data = new byte[1024*1024*4];
                            float countRead = 0;
                            //UnityEngine.Debug.Log("theEntry.Position:" + s.Position);
                            //UnityEngine.Debug.Log("theEntry.CompressedSize:"+ theEntry.CompressedSize);
                            //UnityEngine.Debug.Log("theEntry.Size:" + theEntry.Size);
                            //UnityEngine.Debug.Log("----sum:" + sum);
                            //sssum += theEntry.CompressedSize;
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    countRead += size;
                                    streamWriter.Write(data, 0, size);
                                    if (callProgress != null)
                                    {
                                        //换算成压缩后的下载比例
                                        //sum为theEntry.CompressedSize以及文件夹压缩后大小的总和
                                        callProgress(Mathf.Min(((s.Position) + (theEntry.CompressedSize/theEntry.Size) * countRead / theEntry.Size) / (float)sum,0.999f));
                                        yield return null;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                          
                           
                        }
                        
                    }
                }
                if (callProgress != null)
                {
                    //UnityEngine.Debug.Log(countRead);
                    //UnityEngine.Debug.Log(sum);
                   
                    callProgress(1.0F);
                }
            }
            //yield return null;
        }
        public static IEnumerator unzipInThread (string sourcePath, string targetPath, Action callback = null) {
            Thread thread = new Thread (() => {
                Vesal_DirFiles.UnZip (sourcePath, targetPath);
            });

            thread.Start ();
            yield return null;
            while (thread.ThreadState != System.Threading.ThreadState.Stopped) {
                yield return null;
            }
            if (callback != null) {
                callback ();
            }
        }

        public static bool ZipDirectory(string sourcePath, string targetZipFile, string fileFilter = @"+\.nothingToFilter$", string dirFilter = @"+\.nothingToFilter$")
        {
            try {
                CompressionZip(targetZipFile, 0, 1024, new List<string> { sourcePath });
            }
            catch (Exception e) {
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
                return false;
            }
            //try
            //{
            //    var fastZip = new FastZip();
            //    fastZip.CreateEmptyDirectories = true;
            //    //fastZip.RestoreAttributesOnExtract = true;

            //    fastZip.RestoreDateTimeOnExtract = true;
            //    UnityEngine.Debug.Log(sourcePath);
            //    fastZip.CreateZip(targetZipFile, sourcePath, true, fileFilter, dirFilter);
                
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.Log(e.Message);
            //    UnityEngine.Debug.Log(e.StackTrace);
            //    return false;
            //}
            return true;
        }

        public static String get_file_name_from_full_path(String path)
        {
            int pos = path.LastIndexOf("/");
            if (pos < 0)
            {
                pos = path.LastIndexOf("\\");
                if (pos < 0)
                    return path;

                return path.Substring(pos + 1);
            }

            return path.Substring(pos + 1);
        }

        public static String get_dir_from_full_path(String path)
        {
            int pos = path.LastIndexOf("/");
            if (pos < 0)
            {
                pos = path.LastIndexOf("\\");
                if (pos < 0)
                    return path;

                return path.Substring(0, pos + 1);
            }

            return path.Substring(0, pos + 1);
        }

        public static String get_name_suffix(String full_name)
        {
            int pos = full_name.LastIndexOf(".");
            if (pos < 0)
            {
                return full_name;
            }
            return full_name.Substring(pos + 1);
        }

        public static String remove_name_suffix(String full_name)
        {
            int pos = full_name.LastIndexOf(".");
            if (pos < 0)
            {
                return full_name;
            }

            return full_name.Substring(0, pos);
        }

        public static void CreateDir(String dir)
        {
            try
            {
                if (!Directory.Exists(dir)) //如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception e){
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
            }
        }

        public static List<string> GetAllDirInfoFromPath(string path)
        {
            List<string> list = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dirinfo = dir.GetDirectories();

            if(dirinfo.Length==0)
            {
                FileInfo[]  info_list= dir.GetFiles();
                for (int i = 0; i < info_list.Length; i++)
                {
                    list.Add(info_list[i].FullName);
                }
            }
            return list;
        }


        public static bool if_exit_directory(String dir)
        {
            if (Directory.Exists(dir))
            {
                return true;
            }
            else
                return false;
        }

        public static void DelFile(String file_path)
        {
            try
            {
                if (File.Exists(file_path))
                {
                    File.Delete(file_path);
                }
            }
            catch { }
        }
        public static void DeleteFolder(string filePath)
        {
           // string srcPath = filePath.Substring(0, filePath.Length - 4);
           
            try
            {
                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
                Directory.Delete(filePath);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void ClearFolder(string filePath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }               
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void SaveAbfile(string dest_path, bool decryptFlag)
        {
            string temp_path="";
            temp_path= dest_path;

            if (!File.Exists(temp_path))
                return;
            string abtemp = PublicClass.tempPath + "Abtemptemp.ab";
            //解压文件并删除源文件
            if (decryptFlag)
            {
                VesalDecrypt.encrypt.DecryptFile(temp_path, abtemp, "Vesal17788051918");
                DelFile(temp_path);
            }
            else
            {
                File.Move(temp_path, abtemp );
            }

            FileStream FileIn = new FileStream(abtemp, System.IO.FileMode.Open);
            byte[] data = new byte[1024];

            int readLen = FileIn.Read(data, 0, data.Length);

            //将此1024字节写入数据库的abfile表中
            //var local_db = new DbRepository<Abfiles>();
            //local_db.DataService("vesali.db");
            //local_db.CreateTable();
            //Abfiles file = new Abfiles { file_name = get_file_name_from_full_path(dest_path), head = Convert.ToBase64String(data) };
            //Abfiles res = local_db.SelectOne<Abfiles>((temp) =>
            //{
            //    if (temp.file_name == file.file_name)
            //        return true;
            //    else
            //        return false;
            //});

            //if (res == null)
            //{
            //    //insert
            //    local_db.Insert(file);
            //}
            //else
            //{
            //    //update
            //    local_db.Update(file);
            //}

            //local_db.Close();

            ////将此剩余字节写入dest_path

            //FileStream FileOut = new FileStream(dest_path, System.IO.FileMode.Create);
            //while (true)
            //{
            //    readLen = FileIn.Read(data, 0, data.Length);
            //    if (readLen <= 0)
            //        break;

            //    FileOut.Write(data, 0, readLen);
            //}
            //FileIn.Close();
            //FileOut.Close();

            //DelFile(abtemp);

        }


        public static void SaveAbfile(string temp_path, string dest_path, bool decryptFlag,bool isDeleteSource=true)
        {

            if (!File.Exists(temp_path))
                return;
            string abtemp = PublicClass.tempPath + "Abtemptemp.ab";
            //解压文件并删除源文件
            if (decryptFlag)
            {
                VesalDecrypt.encrypt.DecryptFile(temp_path, abtemp, "Vesal17788051918");
                DelFile(temp_path);
            }
            else
            {
                abtemp = temp_path;
            }
            FileStream FileIn = new FileStream(abtemp, System.IO.FileMode.Open);
            FileStream FileOut = new FileStream(dest_path, System.IO.FileMode.Create);
            //try
            //{
            //    byte[] data = new byte[1024];

            //    int readLen = FileIn.Read(data, 0, data.Length);

            //    //将此1024字节写入数据库的abfile表中
            //    var local_db = new DbRepository<Abfiles>();
            //    local_db.DataService("vesali.db");
            //    local_db.CreateTable();
            //    Abfiles file = new Abfiles { file_name = get_file_name_from_full_path(dest_path), head = Convert.ToBase64String(data) };
            //    Abfiles res = local_db.SelectOne<Abfiles>((temp) =>
            //    {
            //        if (temp.file_name == file.file_name)
            //            return true;
            //        else
            //            return false;
            //    });

            //    if (res == null)
            //    {
            //        //insert
            //        local_db.Insert(file);
            //    }
            //    else
            //    {
            //        //update
            //        local_db.Update(file);
            //    }

            //    local_db.Close();

            //    //将此剩余字节写入dest_path

            //    while (true)
            //    {
            //        readLen = FileIn.Read(data, 0, data.Length);
            //        if (readLen <= 0)
            //            break;

            //        FileOut.Write(data, 0, readLen);
            //    }
            //    FileIn.Close();
            //    FileOut.Close();

            //    if (isDeleteSource)
            //        DelFile(abtemp);

            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.Log(e.Message);
            //    FileIn.Close();
            //    FileOut.Close();
            //}

        }

        public static void GetAbfile_Synchronize(string from_path, string temp_path)
        {
            //if (!File.Exists(from_path))
            //{
            //    UnityEngine.Debug.LogError("!File.Exists : " + from_path);
            //    return;
            //}
            //var local_db = new DbRepository<Abfiles>();
            //local_db.DataService("vesali.db");
            //Abfiles res = local_db.SelectOne<Abfiles>((temp) => {
            //    if (temp.file_name == get_file_name_from_full_path(from_path))
            //        return true;
            //    else
            //        return false;
            //});
            //if (res == null)
            //{
            //    UnityEngine.Debug.LogError("res == null");
            //    local_db.Close();
            //    return;
            //}

            //byte[] data = new byte[1024];
            //data = Convert.FromBase64String(res.head);

            //FileStream FileOut = new FileStream(temp_path, System.IO.FileMode.Create);

            //FileOut.Write(data, 0, 1024);

            //FileStream FileIn = new FileStream(from_path, System.IO.FileMode.Open);
            ////将此剩余字节写入temp_path
            //int readLen;
            //while (true)
            //{
            //    readLen = FileIn.Read(data, 0, data.Length);
            //    if (readLen <= 0)
            //        break;

            //    FileOut.Write(data, 0, readLen);
            //}

            //FileOut.Close();
            //FileIn.Close();
        }


        public static void GetAbfile2(string from_path, string temp_path)
        {
            //byte[] data = new byte[1024];
            //if (!File.Exists(from_path))
            //    return;

            //var local_db = new DbRepository<Abfiles>();
            //local_db.DataService("vesali.db");
            //Abfiles res = local_db.SelectOne<Abfiles>((temp) =>
            //{
            //    if (temp.file_name == get_file_name_from_full_path(from_path))
            //        return true;
            //    else
            //        return false;
            //});
            //if (res == null)
            //{
            //    local_db.Close();
            //    return;
            //}
            //local_db.Close();
            //byte[]data1 = Convert.FromBase64String(res.head);
            //FileStream FileOut = new FileStream(temp_path, System.IO.FileMode.Create,System.IO.FileAccess.ReadWrite, FileShare.ReadWrite);
            //FileOut.Write(data1, 0, data1.Length);

            //FileStream FileIn = new FileStream(from_path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            ////将此剩余字节写入temp_path
            //int readLen;
            //while (true)
            //{
            //    readLen = FileIn.Read(data, 0, data.Length);
            //    if (readLen <= 0)
            //        break;
            //    FileOut.Write(data, 0, readLen);
            //}
            //FileOut.Flush();
            //FileIn.Close();
            //FileOut.Close();

            ////            vesal_log.vesal_write_log("load model phase 2 start: " + DateTime.Now.Ticks);

        }

        //public static byte[] GetAbfile(string from_path, string temp_path)
        //{
        //    byte[] data = new byte[1024];
        //    if (!File.Exists(from_path))
        //        return data;

        //    var local_db = new DbRepository<Abfiles>();
        //    local_db.DataService("vesali.db");
        //    Abfiles res = local_db.SelectOne<Abfiles>((temp) =>
        //    {
        //        if (temp.file_name == get_file_name_from_full_path(from_path))
        //            return true;
        //        else
        //            return false;
        //    });
        //    if (res == null)
        //    {
        //        local_db.Close();
        //        return data;
        //    }
        //    data = Convert.FromBase64String(res.head);
        //    byte[] data2 = File.ReadAllBytes(from_path);
        //    byte[] result = new byte[data.Length + data2.Length];
        //    Array.Copy(data, result, data.Length);
        //    Array.Copy(data2, 0, result, data.Length, data2.Length);
        //    vesal_log.vesal_write_log("load model phase 2 start: " + DateTime.Now.Ticks);
        //    local_db.Close();
        //    return result;
        //}

        //异步copy 单个文件
        public static IEnumerator Vesal_FileCopy(string scr_path, string des_path, Action callback)
        {
            UnityEngine.Debug.Log(scr_path);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (!scr_path.StartsWith("file://")) {
                scr_path = "file://" + scr_path;
            }
#endif
            UnityEngine.Debug.Log("________scr_path:" + scr_path);
            WWW www = new WWW(scr_path);
            yield return www;
            des_path = des_path.Replace("\\","/");
            // UnityEngine.Debug.Log("_______des_path:"+des_path);
            try {
                if (!string.IsNullOrEmpty(www.error))
                {
                    vesal_log.vesal_write_log("www.error:" + www.error);
                    UnityEngine.Debug.Log("www.error:" +www.error);
                }
                else
                {
                    UnityEngine.Debug.Log(Vesal_DirFiles.get_dir_from_full_path(des_path));
                    if (File.Exists(des_path))
                    {
                        // UnityEngine.Debug.Log("File.Exists(des_path):"+ File.Exists(des_path));
                        File.Delete(des_path);
                    }
                    Vesal_DirFiles.CreateDir(Vesal_DirFiles.get_dir_from_full_path(des_path));
                    FileStream fsDes = File.Create(des_path);
                    fsDes.Write(www.bytes, 0, www.bytes.Length);
                    fsDes.Flush();
                    fsDes.Close();
                    callback();
                }
                www.Dispose();
            }
            catch (Exception e) {
                UnityEngine.Debug.Log("Exception:" + e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
            }
        }

        public static void Vesal_DirCopy(string scr_path, string des_path)
        {
            try
            {
                string[] files = Directory.GetFiles(scr_path);
                foreach (string fn in files)
                {
                    string new_fn = fn.Replace("\\", "/");
                    string name = Vesal_DirFiles.get_file_name_from_full_path(new_fn);
                    File.Copy(fn, des_path + "/" + name, true);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 将对象转换为byte数组
        /// </summary>
        /// <param name="obj">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        /// <summary>
        /// 将byte数组转换成对象
        /// </summary>
        /// <param name="buff">被转换byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static object Bytes2Object(byte[] buff)
        {
            object obj;
            using (MemoryStream ms = new MemoryStream(buff))
            {
                IFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }
            return obj;
        }

        //序列化对象为string
        public static string SerializeClassObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }
        //反序列化对象为实例化类
        public static T DeserializeObject<T>(string i)
        {
            // JsonSerializer serializer = new JsonSerializer();
            // StringReader sr = new StringReader(i);
            // object o = serializer.Deserialize(new JsonTextReader(sr),typeof(T));
            T o = JsonConvert.DeserializeObject<T>(i);
            return o;
        }


        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="ofloderPath">要压缩的文件目录</param>
        /// <param name="zos">压缩后的文件</param>
        /// <param name="floderPath">压缩后存放的文件目录</param>
        public static void ZipFloder(string ofloderPath, ZipOutputStream zos, string floderPath)
        {
            Crc32 crc = new Crc32();

            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = UTF8.CodePage;
            foreach (FileSystemInfo item in new DirectoryInfo(floderPath).GetFileSystemInfos())
            {
                if (Directory.Exists(item.FullName))
                {
                    ZipFloder(ofloderPath, zos, item.FullName);
                }
                else if (File.Exists(item.FullName))//如果是文件
                {
                    DirectoryInfo oDir = new DirectoryInfo(ofloderPath);
                    string fullName2 = new FileInfo(item.FullName).FullName;
                    string path = oDir.Name + fullName2.Substring(oDir.FullName.Length, fullName2.Length - oDir.FullName.Length);//获取相对目录
                    FileStream fs = File.OpenRead(fullName2);
                    byte[] bts = new byte[fs.Length];
                    fs.Read(bts, 0, bts.Length);
                    ZipEntry ze = new ZipEntry(path);
                    ze.DateTime = DateTime.Now;
                    ze.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(bts);
                    ze.Crc = crc.Value;
                    zos.PutNextEntry(ze);             //为压缩文件流提供一个容器
                    zos.Write(bts, 0, bts.Length);  //写入字节
                }
            }
        }
        static Encoding UTF8 = Encoding.Default;
        /// <summary>
        /// 压缩文件
        /// </summary>     
        /// <param name="zipedFilePath">压缩后的文件(初始时不存在)</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <param name="blockSize">每次写入的内存大小</param>
        /// <returns></returns>
        public static bool CompressionZip(string zipedFilePath, int compressionLevel, int blockSize, List<string> AbsolutePaths)
        {
            bool result = true;
            FileStream fs = null;
            try
            {

                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = UTF8.CodePage;
                FileStream ZipFile = File.Create(zipedFilePath);                // 创建压缩文件  
                ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
                ZipStream.SetLevel(compressionLevel);             // 设置压缩级别  

                foreach (string path in AbsolutePaths)
                {
                    //如果是目录
                    if (Directory.Exists(path))
                    {
                        ZipFloder(path, ZipStream, path);
                    }
                    else if (File.Exists(path))//如果是文件
                    {
                        fs = File.OpenRead(path);
                        ZipEntry ze = new ZipEntry(new FileInfo(path).Name);
                        ZipStream.PutNextEntry(ze);             //为压缩文件流提供一个容器
                        byte[] bts = new byte[blockSize];
                        int size = fs.Read(bts, 0, bts.Length);    // 每次读入指定大小  
                        ZipStream.Write(bts, 0, size);
                        while (size < fs.Length)       // 保证文件被全部写入  
                        {
                            int sizeRead = fs.Read(bts, 0, bts.Length);
                            ZipStream.Write(bts, 0, sizeRead);
                            size += sizeRead;
                        }
                    }
                }
                ZipStream.Finish(); // 结束压缩
                ZipStream.Close();
                if (fs != null)
                {
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                result = false;

            }
            return result;
        }
        /// <summary>
        /// 按行读取文本配置
        /// </summary>     
        /// <param name="txt_file_path">txt配置文件路径</param>
        /// <returns>返回行数组</returns>
        public static List<string> ReadFileWithLine (string txt_file_path)
        {
            try
            {
                List<string> _list = new List<string>(); 
                FileStream fs = new FileStream(txt_file_path, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = null;
                while ((strLine = sr.ReadLine()) != null)
                {
                    _list.Add(strLine.Replace(",","").Trim());
                }
                //关闭流
                sr.Close();
                fs.Close();
                return _list;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                UnityEngine.Debug.Log(e.StackTrace);
                return null;
            }
        }
    
        public static bool isLowIphoneDevices()
        {
            bool IsLowIphoneDevice = false;
	        string modelStr = SystemInfo.deviceModel;
#if UNITY_IOS
            // iphone5:"iPhone5,1","iPhone5,2" iphone5c:"iPhone5,3","iPhone5,4" iphone5s:"iPhone6,1","iPhone6,2"
            // iphone6:"iPhone7,2" iphone6 plus:"iPhone7,1" iphone6s:"iPhone8,1"
            // iPhoneX:"iPhone10,3","iPhone10,6"  iPhoneXR:"iPhone11,8"  iPhoneXS:"iPhone11,2"  iPhoneXS Max:"iPhone11,6"
            IsLowIphoneDevice = modelStr.Equals("iPhone5,1") || modelStr.Equals("iPhone5,2") || modelStr.Equals("iPhone5,3") ||
             modelStr.Equals("iPhone5,4") || modelStr.Equals("iPhone6,1") || modelStr.Equals("iPhone6,2")|| 
             modelStr.Equals("iPhone7,1")|| modelStr.Equals("iPhone7,2")|| modelStr.Equals("iPhone8,1");
        #endif
            return IsLowIphoneDevice;
        }

        //public List<T> Readjson(string json_path)
        //{
        //    string jsonfile = "C:/Users/Raytine/AppData/Roaming/Vesal_unity_PC/AcuSearchTree.json";

        //    using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
        //    {
        //        using (JsonTextReader reader = new JsonTextReader(file))
        //        {
        //            JObject o = (JObject)JToken.ReadFrom(reader);
        //            Dictionary<string, object> r = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
        //            if (r.ContainsKey("AcuList"))
        //            {
        //                return JsonConvert.DeserializeObject<List<T>>(r["AcuList"].ToString());
        //            }
        //        }
        //    }
        //    return null;
        //}
    }

}