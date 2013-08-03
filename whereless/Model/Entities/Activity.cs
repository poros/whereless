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


        private int _id;
        private int _idPlace;
        private string _name;
        private string _argument;
        private string _pathfile;
        private ActivityType _type;
        

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
                if (value.IsNotEmpty())
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
                if (value.IsNotEmpty())
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


        public virtual int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public virtual int IdPlace
        {
            get { return _idPlace; }
            protected set { _idPlace = value; }
        }


        public Activity()
        {
        }


        public Activity(string name)
        {
            _name = name;
        }

        public Activity(int idA, int idP, string name, string path, string args, string t)
        {
            _id = idA;
            _idPlace = idP;
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
            if (Type == ActivityType.ExeFile)
            {
                StartExe(Pathfile, Argument);
            }
            else
            {
                if (Type == ActivityType.Wallpaper)
                {
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
                }
                else
                {
                    if (Type == ActivityType.BatchFile)
                    {
                        StartBatch(Pathfile);
                    }

                }
            }
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
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
            catch (InvalidOperationException ioe)
            {
                return;
            }
            catch (Win32Exception w32e)
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
            catch (Win32Exception w32e)
            {
                return;
            }
        }


        public virtual void ChangeWallpaper(string file, WallpaperStyle s)
        {
            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
           
            try
            {
                if (s == WallpaperStyle.Stretched)
                {
                    rkWallPaper.SetValue(@"WallpaperStyle", 2.ToString());
                    rkWallPaper.SetValue(@"TileWallpaper", 0.ToString());
                }
                else if (s == WallpaperStyle.Centered)
                {
                    rkWallPaper.SetValue(@"WallpaperStyle", 1.ToString());
                    rkWallPaper.SetValue(@"TileWallpaper", 0.ToString());
                }
                else
                {
                    rkWallPaper.SetValue(@"WallpaperStyle", 1.ToString());
                    rkWallPaper.SetValue(@"TileWallpaper", 1.ToString());
                }
            }
            catch (System.NullReferenceException nre)
            {
                return;
            }

            //Set wallpaper 
            SystemParametersInfo(20, 0, file, 0x01 | 0x02);
                       
            rkWallPaper.Close();
        }



    }
}
