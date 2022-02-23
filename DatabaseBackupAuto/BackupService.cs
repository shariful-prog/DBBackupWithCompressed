using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseBackupAuto
{
    public class BackupService
    {
        private readonly string _connectionString;
        private readonly string _backupFolderFullPath;

        public BackupService(string connectionString, string backupFolderFullPath)
        {
            _connectionString = connectionString;
            _backupFolderFullPath = backupFolderFullPath;
        }
        public Tuple<string, string> BackupDatabase(string databaseName)
        {
            var tple = BuildBackupPathWithFilename(databaseName);
            string filePath = tple.Item1;
            string fileName = tple.Item2;



            using (var connection = new SqlConnection(_connectionString))
            {
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", databaseName, filePath);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return Tuple.Create(filePath, fileName);
        }


        private Tuple<string, string> BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyyMMdd"));

            return Tuple.Create(Path.Combine(_backupFolderFullPath, filename), filename);
        }
    }
}
