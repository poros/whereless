using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;

using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace whereless.Model.Entities
{

    

    public class Activity
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);



        public enum ActivityType : int
        {
            Wallpaper,
            BatchFile,
            ExeFile
        }

        public enum WallpaperStyle : int
        {
            Tiled,
            Centered,
            Stretched
        }


        
        private string _name;
        private string _argument; // if exe file -> args; if wallpaper -> style
        private string _pathfile;
        private ActivityType _type;

        public virtual Location Location { get; set; } // Reference for Inverse(). Causes problems, but saves an update.


        public virtual int Id { get; protected set; }

        public virtual string Name 
        {
            get { return _name; }
            set { _name = value; } 
        }

        public virtual string Argument
        {
            get{ return _argument; }
            set
            {
                if (value !=null)
                {
                    _argument = value;
                }
                
            }
        }

        public virtual string Pathfile
        {
            get { return _pathfile; }
            set
            {
                if (value != null)
                {
                    _pathfile = value;
                }
            }
        }

        
        public virtual ActivityType Type
        {
            get { return _type; }
            set { _type = value; }
        }


       
        public Activity()
        {
        }


        public Activity(string name)
        {
            _name = name;
        }

        public Activity(string name, string path, string args, string t)
        {

            _name = name;
            _pathfile = path;
            _argument = args;

            if (t.CompareTo("Wallpaper") == 0)
            {
                _type = ActivityType.Wallpaper;
            }
            else
            {
                if (t.CompareTo("ExeFile") == 0)
                {
                    _type = ActivityType.ExeFile;
                }
                else
                {
                    if (t.CompareTo("BatchFile") == 0)
                    {
                        _type = ActivityType.BatchFile;
                    }
                }
            }
        }



        public virtual void Start()
        {
            switch (Type)
            {
                case ActivityType.ExeFile:
                    StartExe(Pathfile, Argument);
                    break;

                case ActivityType.BatchFile:
                    StartBatch(Pathfile);
                    break;

                case ActivityType.Wallpaper:
                    WallpaperStyle s;
                    if (Argument.CompareTo("Tiled") == 0)
                    {
                        s = WallpaperStyle.Tiled;
                    }
                    else
                    {
                        if (Argument.CompareTo("Centered") == 0)
                        {
                            s = WallpaperStyle.Centered;
                        }
                        else
                        {
                            s = WallpaperStyle.Stretched;
                        }
                    }
                    ChangeWallpaper(Pathfile, s);
                    break;

            }
        }

        public virtual void Stop()
        {
            string exe = Path.GetFileNameWithoutExtension(Pathfile);
            Process[] processes = Process.GetProcessesByName(exe);
            foreach (Process proc in processes)
            {
                proc.Kill();
            }
        }



        public override string ToString()
        {
            return (this.GetType().Name + ": " + "Name = " + Name);
        }



        public virtual void StartExe(string file, string args)
        {
            Process a = new Process();

            try
            {
                a.EnableRaisingEvents = false;
                a.StartInfo.FileName = file;
                a.StartInfo.Arguments = args;
                a.Start();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            catch (Win32Exception )
            {
                return;
            }

        }


        public virtual void StartBatch(string file)
        {
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch (Win32Exception)
            {
                return;
            }
        }


        public virtual void ChangeWallpaper(string file, WallpaperStyle s)
        {

            if (File.Exists(file) == false)
            {
                return;
            }

            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);

            if (rkWallPaper != null)
            {
                switch (s)
                {
                    case WallpaperStyle.Stretched:
                        rkWallPaper.SetValue(@"WallpaperStyle", 2.ToString());
                        rkWallPaper.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WallpaperStyle.Centered:
                        rkWallPaper.SetValue(@"WallpaperStyle", 1.ToString());
                        rkWallPaper.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WallpaperStyle.Tiled:
                        rkWallPaper.SetValue(@"WallpaperStyle", 1.ToString());
                        rkWallPaper.SetValue(@"TileWallpaper", 1.ToString());
                        break;
                    default:
                        //unreachable
                        break;
                }

                //Set wallpaper 
                SystemParametersInfo(20, 0, file, 0x01 | 0x02);
                
                rkWallPaper.Close();
            }
        }
    }
}