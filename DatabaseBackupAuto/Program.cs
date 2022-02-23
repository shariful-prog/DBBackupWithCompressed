using Ionic.Zip;
using System;
using System.IO;
using System.Reflection;

namespace DatabaseBackupAuto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connectionString = BasicSetting.ConnectionString;
                string actualDir = BasicSetting.BackupDirectory;
                string dbname = "hogevardedb";

                //Removing 7 days old database
                string[] files = Directory.GetFiles(actualDir);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTime < DateTime.Now.AddDays(-7))
                        fi.Delete();
                }


                // Backing UP database
                BackupService backupService = new BackupService(connectionString, actualDir);
                var tple = backupService.BackupDatabase(dbname);


                //Zipping file
                using (var zip = new ZipFile())
                {
                    zip.Password = BasicSetting.SecretKey;
                    zip.AddFile(tple.Item1, Path.GetFileName(tple.Item1));
                    zip.Save(actualDir + "\\" + tple.Item2 + ".zip");
                }
                FileInfo deletFile = new FileInfo(tple.Item1);
                deletFile.Delete();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }


    }
}
