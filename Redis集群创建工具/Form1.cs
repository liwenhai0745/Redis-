using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Redis集群创建工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {


            //1.处理主
            //string MasterConfig = textBox1.Text;
            DoIPConfig(textBox1.Text);

            //2.处理从
            foreach (var item in textBox2.Text.Split('\n'))
            {
                DoIPConfig(item.Replace("\r",""),true);

            }


            //3.处理哨兵

        }

        private void DoIPConfig(string ipConfig, bool isSlave = false,bool issentinel=false) {
            string[] arr = ipConfig.Split(':');
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Resources\\Redis-x64-3.2.100");
            string newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\OutPut\\Redis-x64-3.2.100-"+arr[1]);


            FileHelper.CopyFile(path, newPath);
            string strContent = "";
            if (isSlave) {

                string[] master = textBox1.Text.Split(':');

                //如果是从,就要绑定主  # slaveof <masterip> <masterport>
                strContent = File.ReadAllText(newPath + "\\redis.windows.conf");
                strContent = Regex.Replace(strContent, "# slaveof <masterip> <masterport>", "# slaveof <masterip> <masterport> "+Environment.NewLine+$"slaveof {master[0]} {master[1]}");
                File.WriteAllText(newPath + "\\redis.windows.conf", strContent);
            }

            //替换端口
            //port 6379
            strContent = File.ReadAllText(newPath + "\\redis.windows.conf");
            strContent = Regex.Replace(strContent, "port 6379", "port "+arr[1]);
            File.WriteAllText(newPath + "\\redis.windows.conf", strContent);

            //生成启动bat
            File.WriteAllText(newPath+ "\\startRedisServer.bat", "@echo off"+Environment.NewLine+"redis-server.exe redis.windows.conf"+Environment.NewLine+"@pause");


            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\OutPut\\start" + arr[1]+".cmd"), "@echo off"+Environment.NewLine+ "cd Redis-x64-3.2.100-" + arr[1] + "" + Environment.NewLine+"startRedisServer.bat");


          

            BuildSentinel(arr);

        }

        private void BuildSentinel(string[] arr)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Resources\\Redis-x64-3.2.100");
            string dest = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\OutPut\\Sentinel\\" + arr[1]);
            FileHelper.CopyFile(path, dest);

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }


            string[] master = textBox1.Text.Split(':');

            //生成哨兵信息
            string SentinetConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Resources\\sentinel.txt"));
            string strContent = Regex.Replace(SentinetConfig, "6379", master[1]);
            strContent = Regex.Replace(SentinetConfig, "22222", "2"+ arr[1]);
            strContent = Regex.Replace(strContent, "127.0.0.1", master[0]);
            File.WriteAllText(dest + "\\sentinel.conf", strContent);


            //生成启动bat
            File.WriteAllText(dest + "\\startRedisSentinel.bat", "@echo off" + Environment.NewLine + "redis-server.exe sentinel.conf --sentinel " + Environment.NewLine + "@pause");


            //
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\OutPut\\Sentinel\\startrRedisSentinel" + arr[1] + ".cmd"), "@echo off" + Environment.NewLine + "cd " + arr[1] + "" + Environment.NewLine + "startRedisSentinel.bat");


        }
    }
}
