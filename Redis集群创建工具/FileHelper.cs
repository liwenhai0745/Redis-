using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis集群创建工具
{
    public class FileHelper
    {
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sources">源路径</param>
        /// <param name="dest">新路径</param>
        public static void CopyFile(string sources, string dest)
        {
            DirectoryInfo dinfo = new DirectoryInfo(sources);
            if (!Directory.Exists(dest)) {
                Directory.CreateDirectory(dest);
            }
            //注，这里面传的是路径，并不是文件，所以不能保含带后缀的文件                
            foreach (FileSystemInfo f in dinfo.GetFileSystemInfos())
            {
                //目标路径destName = 目标文件夹路径 + 原文件夹下的子文件(或文件夹)名字                
                //Path.Combine(string a ,string b) 为合并两个字符串                     
                String destName = Path.Combine(dest, f.Name);
                if (f is FileInfo)
                {
                    //如果是文件就复制       
                    File.Copy(f.FullName, destName, true);//true代表可以覆盖同名文件                     
                }
                else
                {
                    //如果是文件夹就创建文件夹然后复制然后递归复制              
                    Directory.CreateDirectory(destName);
                    CopyFile(f.FullName, destName);
                }
            }
        }
    }
}
