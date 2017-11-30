using System;
using System.IO;
using System.Text;

namespace EM
{

    public static class FileHelper
    {

        public static void DeleteIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// 读取本地文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件内容</returns>
        public static string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            string result;
            using (StreamReader streamReader = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
            {
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
                result = text;
            }
            return result;
        }

        public static void WriteFile(string filePath, string content,bool append)
        {
            FileStream fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(content);
            sw.Dispose();
            fs.Dispose();
        }
        /// <summary>
        /// 新增或者覆盖文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">内容</param>
        /// <param name="ifCover">是否直接覆盖</param>
        public static void CreateFile(string fileName, string filePath, string content, bool ifCover = false)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath += "\\" + fileName;
            if (!File.Exists(filePath) || ifCover)
            {
                FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1);
                sw.Write(content);//开始写入值
                sw.Close();
                fs1.Close();
            }
        }
        public static void WriteLog(string logPath, string content)
        {
            try
            {
                string fileContent = ReadFile(logPath);
                fileContent = content + fileContent;
                FileStream fs = new FileStream(logPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("UTF-8"));
                sw.Write(fileContent);
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception e)
            {

            }
        }
        public static string GetFileContentWithNolock(string filename)
        {
            string content = string.Empty;
            string filepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase  + filename;
            using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }

        public static int GetFileCount(string path)
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }
            else
            {
                string[] fileList = System.IO.Directory.GetFileSystemEntries(path);
                return fileList.Length;
            }
        }
    }
}
