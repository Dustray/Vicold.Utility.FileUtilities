using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Database
{
    internal class DBBackupTool
    {
        private static readonly string _dbFile = DBDriver.ConnectionString;
        public static readonly string _backupPath = AppDomain.CurrentDomain.BaseDirectory + @"backup";
        public static void BackupDatabase()
        {
            var filePath = Path.Combine(_backupPath, $"data_bak_{DateTime.Now:yyyyMMddHHmmss}.db");
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
            File.Copy(_dbFile, filePath, true);
            DeleteOldBakFile();
        }

        private static void DeleteOldBakFile()
        {
            var files = Directory.GetFiles(_backupPath);
            var now = DateTime.Now;
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("data_bak_"))
                {
                    var fileTime = DateTime.ParseExact(fileName.Substring(9, 14), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    if (now.Subtract(fileTime).TotalDays > 7)
                    {
                        File.Delete(file);
                    }
                }
            }
        }
    }
}
