using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Shell32;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace IconeBox
{
    //Ressource static classe
    class R
    {
        public static Box[] BOX;
        public static Notif notif;
        public static bool isBoxReady = false;
        public static bool isDLReady = false;
        public static Rectangle screen;
        public static bool hardReboot = false;
        public static string I = @"\";
        //private static string serverPath = "";error


        static void ExecuteCommand(string command)
        {
            Debug.WriteLine("cmd ====> " + command);
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            //processInfo.RedirectStandardError = true;
            //processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }

        internal static void updateConf()
        {
            Thread update = new Thread(delegate()
            {
                R.notif.setNotif("", 'o');

                //get serveur path
                /*  StreamReader file = null;
                  try
                  {
                      file = new StreamReader("path_conf.txt");
                      Program.conf.pathConf = file.ReadToEnd();
                      R.serverPath = Program.conf.pathConf.Substring(0, Program.conf.pathConf.LastIndexOf('\\'));
                      file.Close();
                  }
                  catch
                  {
                      Debug.WriteLine("path serv introuvable");
                      R.notif.setNotif("path_conf.txt introuvable!", 'e');
                  }
                  */

                //getfile
                // if(file != null)
                cp(Program.PathServerIconBox + I + Program.groupe + I + "conf_init.txt", Program.app);

                //is file here
                StreamReader file = null;
                String newConf = null;
                bool isServConfHere = false;
                try
                {
                    file = new StreamReader(Program.app+"\\conf_init.txt");
                    newConf = file.ReadToEnd();
                    if (!newConf.Substring(0, 7).Equals("{\"pass\""))//verifie que c'est le bon fichier
                    {
                        isServConfHere = false;
                        R.notif.setNotif("conf_init.txt incompatible", 'e');
                    }
                    else
                    {
                        isServConfHere = true;
                    }
                    file.Close();
                }
                catch{ isServConfHere = false; 
                    R.notif.setNotif("Problème connexion Server\nupdateimpossible", 'w');
                }

                if(isServConfHere)//if dl work
                {
                    if (newConf.Equals(Program.confFile) && !hardReboot)//si rien de nouveau
                    {
                        File.Delete(Program.app+"\\conf_init.txt");
                        R.notif.setNotif("pas nouvelle de MAJ disponible.", 'o',1500);
                    }
                    else//si la config a changer
                    {
                        File.Delete(Program.app+"\\~conf_init.txt");
                        File.Move(Program.app+"\\conf_init.txt", Program.app+"\\~conf_init.txt");

                        foreach(Box b in R.BOX)
                        {
                            b.makeExit();
                        }
                        R.BOX = null;
                        hardReboot = false;
                        Program.start();
                    }
                }
                else
                {
                    //show message into notif ico
                }
            });update.Start();
            updateFolder();
        }

        static void updateFolder()
        {
            foreach(Box box in BOX)
            {
                if (Directory.Exists(box.folderPath))//si le dossier existe
                {
                    IEnumerable<string> listLocal = getFolderContent(Program.app + I+box.folderPath);
                    IEnumerable<string> listRemote = getFolderContent(Program.PathServerIconBox + I + Program.groupe);

                    IEnumerable<string> Tosup = listLocal.Except<string>(listRemote);
                    IEnumerable<string> Toadd = listRemote.Except<string>(listLocal);

                    supIcon(box, Tosup);
                    addIcon(box, Toadd);
                }
                else//si dossier inexistant
                {
                    cpd(Program.PathServerIconBox + I + Program.groupe+I+ box.folderPath, Program.app);
                }
            }
        }

        private static void addIcon(Box b, IEnumerable<string> toadd)
        {
            foreach (string ico in toadd)
            {
                cp(Program.PathServerIconBox + I+Program.groupe+I + b.folderPath + "\\" + ico, "ico\\"+b.folderPath);
            }
        }

        private static void supIcon(Box b, IEnumerable<string> tosup)
        {
            foreach (string ico in tosup)
            {
                File.Delete(Program.app+I+b.folderPath+I+ ico);
            }
        }


        static List<string> getFolderContent(string path)
        {
            List <string> rez = new List<string>();
            IEnumerable<string> list = Directory.EnumerateFiles(path);
            foreach (string s in list)
            {
                rez.Add(s.Substring(s.LastIndexOf('\\') + 1));
            }
            return rez;
        }

        public static void cp(string source, string dest)
        {
            ExecuteCommand("copy /y \"" + source + "\" \"" + dest + "\"");
        }

        public static void cpd(string source, string dest)
        {
            string folderToCopy = source.Substring(source.LastIndexOf("\\") + 1);
            
            //Directory.CreateDirectory(dest);
            ExecuteCommand("XCOPY /E /H /K /C /Y \"" + source + "\" \"" + dest + "\\" + folderToCopy + "\\\"");
        }


        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }

        

        public static string GetTargetPath(string filePath)
        {
            string targetPath = ResolveMsiShortcut(filePath);
            if (targetPath == null)
            {
                targetPath = ResolveShortcut(filePath);
            }

            return targetPath;
        }

        public static string GetInternetShortcut(string filePath)
        {
            string url = "";

            using (TextReader reader = new StreamReader(filePath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("URL="))
                    {
                        string[] splitLine = line.Split('=');
                        if (splitLine.Length > 0)
                        {
                            url = splitLine[1];
                            break;
                        }
                    }
                }
            }

            return url;
        }

        static string ResolveShortcut(string filePath)
        {
            // IWshRuntimeLibrary is in the COM library "Windows Script Host Object Model"
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

            try
            {
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                return shortcut.TargetPath;
            }
            catch (COMException)
            {
                // A COMException is thrown if the file is not a valid shortcut (.lnk) file 
                return null;
            }
        }

        static string ResolveMsiShortcut(string file)
        {
            StringBuilder product = new StringBuilder(NativeMethods.MaxGuidLength + 1);
            StringBuilder feature = new StringBuilder(NativeMethods.MaxFeatureLength + 1);
            StringBuilder component = new StringBuilder(NativeMethods.MaxGuidLength + 1);

            NativeMethods.MsiGetShortcutTarget(file, product, feature, component);

            int pathLength = NativeMethods.MaxPathLength;
            StringBuilder path = new StringBuilder(pathLength);

            NativeMethods.InstallState installState = NativeMethods.MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            if (installState == NativeMethods.InstallState.Local)
            {
                return path.ToString();
            }
            else
            {
                return null;
            }
        }

        static public void log(string log)
        {
            Debug.WriteLine(log);
        }

        private class NativeMethods
        {
            [DllImport("msi.dll", CharSet = CharSet.Auto)]
            public static extern uint MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

            [DllImport("msi.dll", CharSet = CharSet.Auto)]
            public static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

            public const int MaxFeatureLength = 38;
            public const int MaxGuidLength = 38;
            public const int MaxPathLength = 1024;

            public enum InstallState
            {
                NotUsed = -7,
                BadConfig = -6,
                Incomplete = -5,
                SourceAbsent = -4,
                MoreData = -3,
                InvalidArg = -2,
                Unknown = -1,
                Broken = 0,
                Advertised = 1,
                Removed = 1,
                Absent = 2,
                Local = 3,
                Source = 4,
                Default = 5
            }
        }

    }
}
