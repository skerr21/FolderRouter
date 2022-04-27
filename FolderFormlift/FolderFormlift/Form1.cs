using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace FolderFormlift
{

    public partial class Form1 : Form
    {
        public string watcherPath;
        public string moveToPath;
        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            using (StreamReader r = new StreamReader(@"E:\repos\FolderFormlift\FolderFormlift\FileInformation.json"))
            {

                if (r.Peek() != -1)
                {

                    string json = r.ReadToEnd();
                    List<FileArch> fileArchs = JsonConvert.DeserializeObject<List<FileArch>>(json);
                    foreach (FileArch item in fileArchs)
                    {
                        listBox1.Items.Add(item.extName + " " + item.monitorLocation + " " + item.moveLocation);
                        moveToPath = item.moveLocation;
                        GenerateWatchers(item.monitorLocation, item.extName, moveToPath);
                    }
                }



            }
            

            
        }


        private void GenerateWatchers(string path, string filter, string destPath)
        {
            var watcher = new FileSystemWatcher(path,filter);
            
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            //watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            //watcher.Deleted += OnDeleted;
            //watcher.Renamed += OnRenamed;
            //watcher.Error += OnError;

            watcher.Filter = "*";
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

        

        }
    
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            
            try
            {
                // Set a variable to the My Documents path.
                //Thread.Sleep(5000);
                DirectoryInfo dirPath;
                DirectoryInfo destPath;
                moveToPath = moveToPath + @"\" + e.Name;
                dirPath = new DirectoryInfo(e.FullPath);
                destPath = new DirectoryInfo(moveToPath);


                string[] fileEntries = Directory.GetFileSystemEntries(e.FullPath);
                foreach (string fileEntry in fileEntries)
                {
                    

                    if (fileEntry.Contains(".cbr") | fileEntry.Contains(".cbz"))
                    {


                       
                                CopyAll(dirPath, destPath);

                        
                    } else if (fileEntry.Contains(".epub") | fileEntry.Contains(".mobi"))
                    {
                        try
                        {

                            CopyAll(dirPath, destPath);
                            //Directory.Delete(e.FullPath, true);
                        }
                        catch
                        {
                            Console.WriteLine(e.FullPath + " already exists");
                        }
                    }
                    
                }
                //Console.WriteLine($"{files.Count().ToString()} files found.");
            }
            catch (UnauthorizedAccessException uAEx)
            {
                Console.WriteLine(uAEx.Message + "1");
            }
            catch (PathTooLongException pathEx)
            {
                Console.WriteLine(pathEx.Message + "2");
            }
            
        }


        //static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        //{
        //    // Get information about the source directory
        //    var dir = new DirectoryInfo(sourceDir);

        //    // Check if the source directory exists
        //    if (!dir.Exists)
        //        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        //    // Cache directories before we start copying
        //    DirectoryInfo[] dirs = dir.GetDirectories();

        //    // Create the destination directory
        //    Directory.CreateDirectory(destinationDir);

        //    // Get the files in the source directory and copy to the destination directory
        //    foreach (FileInfo file in dir.GetFiles())
        //    {

        //        string targetFilePath = Path.Combine(destinationDir, file.Name);
        //        file.CopyTo(targetFilePath);
        //    }

        //    // If recursive and copying subdirectories, recursively call this method
        //    if (recursive)
        //    {
        //        foreach (DirectoryInfo subDir in dirs)
        //        {
        //            string newDestinationDir = Path.Combine(destinationDir, @"\"+ subDir.Name);
        //            CopyDirectory(subDir.FullName, newDestinationDir, true);
        //        }
        //    }
        //}

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }

            Directory.Delete(source.FullName, true);
        }
    

    private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            watcherPath = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            moveToPath = folderBrowserDialog2.SelectedPath;
        }



        private void button4_Click(object sender, EventArgs e)
        {
            //string textToAdd;
            string textToAdd = textBox1.Text;

            //if(textToAdd.Length > 0)

            //{
            //    listBox1.Items.Add(textToAdd);
            List<FileArch> archs = new List<FileArch>();
            //    archs.Add(new FileArch()
            //    {
            //        extName = textToAdd, monitorLocation = watcherPath, moveLocation = moveToPath
            //    });
            //    string json = JsonConvert.SerializeObject(archs.ToArray());
            //    System.IO.File.AppendAllText(@"E:\repos\FolderFormlift\FolderFormlift\FileInformation.json", json);
            //} 
            var filePath = @"E:\repos\FolderFormlift\FolderFormlift\FileInformation.json";
            
            // Read existing json data
            var jsonData = System.IO.File.ReadAllText(filePath);
            // De-serialize to object or create new list
            var archList = JsonConvert.DeserializeObject<List<FileArch>>(jsonData)
                                  ?? new List<FileArch>();

            // Add any new employees
            archList.Add(new FileArch()
            {
                extName = textToAdd,
                monitorLocation = watcherPath,
                moveLocation = moveToPath
                
                
            });
            textToAdd += " " + watcherPath + " " + moveToPath;
            
            // Update json data string
            jsonData = JsonConvert.SerializeObject(archList);
            System.IO.File.WriteAllText(filePath, jsonData);


        }


    }
}

