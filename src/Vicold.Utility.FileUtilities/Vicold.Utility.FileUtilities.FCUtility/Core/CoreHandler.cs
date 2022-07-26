﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vicold.Utility.FileUtilities.FCUtility.Configuration;
using Vicold.Utility.FileUtilities.FCUtility.Configuration.Entities;
using Vicold.Utility.FileUtilities.FCUtility.Database;
using Vicold.Utility.FileUtilities.FCUtility.Database.Entities;
using Vicold.Utility.FileUtilities.FCUtility.Views.ViewModels;

namespace Vicold.Utility.FileUtilities.FCUtility.Core
{
    internal class CoreHandler : IDisposable
    {
        private Logger _logger;
        private DBDriver _dBDriver;
        private ConfigDriver _configDriver;
        private bool _syncing = false;

        public CoreHandler(Logger logger)
        {
            _logger = logger;
            _dBDriver = new DBDriver();
            _configDriver = new ConfigDriver();
        }

        public DBDriver DB => _dBDriver;

        public bool HasDatabaseChanged()
        {
            return _dBDriver.HasDataChanged;
        }

        public int GetAllDataCount()
        {
            return _dBDriver.CountAll();
        }

        public CustomConfig GetConfig()
        {
            return _configDriver.GetConfig();
        }

        public void SaveConfig()
        {
            _configDriver.Save();
        }
        public List<DBCountItemVM> GetLevelAndTypeDataCount()
        {
            var config = _configDriver.GetConfig();
            var mainPath = config.MainPath ?? string.Empty;
            var countResult = new List<DBCountItemVM>();

            Count(TypeType.Clean);
            Count(TypeType.CleanMask);
            Count(TypeType.Mosaic);

            var unknownCount = _dBDriver.Count(LevelType.Unset, TypeType.Unknown);
            if (unknownCount != 0)
            {
                countResult.Add(new DBCountItemVM($"未设置", unknownCount));
            }

            var all = GetAllDataCount();
            var otherCount = all;
            foreach (var v in countResult)
            {
                otherCount -= v.Count;
            }

            if (otherCount != 0)
            {
                countResult.Add(new DBCountItemVM($"其他", otherCount));
            }

            countResult.Add(new DBCountItemVM($"总和", all));

            void Count(TypeType type)
            {
                for (var i = LevelTypeInfo.MinTypeIndex; i <= LevelTypeInfo.MaxTypeIndex; i++)
                {
                    var level = LevelTypeInfo.GetType(i);

                    var count = _dBDriver.Count(level, type);
                    if (count != 0)
                    {
                        var dir = Path.Combine(mainPath, FileStrUtility.GetMainDirName(level, type));
                        countResult.Add(new DBCountItemVM($"{TypeTypeInfo.GetTypeName(type)}-{level}", count, dir));
                    }
                }
            }

            return countResult;
        }

        /// <summary>
        /// 同步数据库
        /// </summary>
        public Task SyncDatabase()
        {
            if (_syncing)
            {
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                _syncing = true;
                _logger.Log("[DB]", "文件与数据库同步开始");
                var config = _configDriver.GetConfig();

                if (!Directory.Exists(config.MainPath))
                {
                    _logger.Log("[DB]", $"主资源路径[{config.MainPath}]不存在");
                }

                var count = 0;
                // 遍历更新主路径
                for (var i = LevelTypeInfo.MinTypeIndex; i <= LevelTypeInfo.MaxTypeIndex; i++)
                {
                    var level = LevelTypeInfo.GetType(i);

                    SearchMain(level, TypeType.Clean);
                    SearchMain(level, TypeType.CleanMask);
                    SearchMain(level, TypeType.Mosaic);
                }

                _logger.Log("[DB]", $"已同步主资源库，共同步了{count}个文件");
                count = 0;
                // 遍历更新子路径
                if (config.SubPaths is { })
                {
                    foreach (var subPath in config.SubPaths)
                    {
                        if (subPath is { })
                        {
                            SearchSub(subPath);
                        }
                    }
                }

                _logger.Log("[DB]", $"已同步临时资源库，共同步了{count}个文件");

                void SearchMain(LevelType levelType, TypeType typeType)
                {
                    var codes = GetMainCodes(levelType, typeType);
                    if (codes is { })
                    {
                        count += codes.Count();
                        SearchAndSync(codes, levelType, typeType);
                    }
                }

                void SearchSub(string subPath)
                {
                    var codes = GetSubCodes(subPath);
                    if (codes is { })
                    {
                        count += codes.Count();
                        SearchAndSync(codes, LevelType.Unset, TypeType.Unknown);
                    }
                }

                _logger.Log("[DB]", "文件与数据库同步结束");
                _syncing = false;
            });
        }

        public void SearchAndSync(IEnumerable<KeyValuePair<int, string>>? codes, LevelType levelType, TypeType typeType)
        {
            if (codes is { })
            {
                foreach (var code in codes)
                {
                    var existCode = _dBDriver.Search(code.Key);
                    if (existCode is { } // code不为空
                    && existCode.FilePath is { } // 文件路径不为空
                    && File.Exists(existCode.FilePath)  // 文件存在
                    && existCode.FilePath != code.Value) // 文件路径不一致 
                    {
                        _logger.Log("[DB]", $"Code[{code.Key}]文件已存在，请查看文件是否重复：");
                        _logger.Log("[DB]", $"  已存在文件：{existCode.FilePath}");
                        _logger.Log("[DB]", $"  新查到文件：{code.Value}");
                    }

                    _dBDriver.InsertOrUpdate(new CodeTable()
                    {
                        Code = code.Key,
                        Level = levelType,
                        Type = typeType,
                        FilePath = code.Value
                    });
                }
            }
        }

        public void SearchAndSync(IEnumerable<int>? codes, LevelType levelType, TypeType typeType)
        {
            if (codes is { })
            {
                foreach (var code in codes)
                {
                    _dBDriver.InsertOrUpdate(code, levelType, typeType);
                }
            }
        }

        /// <summary>
        /// 获取主路径中文件的代码
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<int, string>>? GetMainCodes(LevelType levelName, TypeType typeName)
        {
            var config = _configDriver.GetConfig();
            if (config.MainPath is { })
            {
                var dir = Path.Combine(config.MainPath, FileStrUtility.GetMainDirName(levelName, typeName));
                if (!Directory.Exists(dir))
                {
                    return null;
                }

                var codes = FilePathUtility.GetAllCodesPathInFolderLoop(dir);
                return codes;
            }

            return null;
        }

        /// <summary>
        /// 获取支路径中文件的代码
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<int, string>>? GetSubCodes(string subPath)
        {
            if (!Directory.Exists(subPath))
            {
                _logger.Log("[DB]", $"临时资源路径[{subPath}]不存在");
                return null;
            }
            var codes = FilePathUtility.GetAllCodesPathInFolderLoop(subPath);
            return codes;
        }

        /// <summary>
        /// 重命名所有文件
        /// </summary>
        /// <param name="loop"></param>
        public void RenameAllFile(bool loop = true)
        {
            var config = _configDriver.GetConfig();
            if (config.MainPath is { })
            {
                RenameFileFormat(config.MainPath, loop);
            }

            // 遍历更新子路径
            if (config.SubPaths is { })
            {
                foreach (var subPath in config.SubPaths)
                {
                    if (subPath is { })
                    {
                        RenameFileFormat(subPath, loop);
                    }
                }
            }
        }


        /// <summary>
        /// 链接去重
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IList<string> LinkDuplicateRemoval(IList<string> source)
        {
            var result = new List<string>(source.Count);
            foreach (var link in source)
            {
                var code = FilePathUtility.GetCodeFromLongStr(link);
                if (FilePathUtility.TryToInteger(code, out var codeInt))
                {
                    if (_dBDriver.Search(codeInt) is not { })
                    {
                        result.Add(link);
                    }
                }
            }

            return result;
        }

        public void RenameFileFormat(string root, bool isLoop, bool isMoveUnformatFile = false)
        {
            var renameCount = 0;
            var config = _configDriver.GetConfig();
            Rename(root, 0);

            void Rename(string thisDir, int back)
            {
                //绑定到指定的文件夹目录
                DirectoryInfo dir = new DirectoryInfo(thisDir);

                //检索表示当前目录的文件和子目录
                FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

                //遍历检索的文件和子目录
                int index = 0;
                int conflict = 1;
                int dumCode = 0;
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    //判断是否为空文件夹　　
                    if (fsinfo is DirectoryInfo dirInfo)
                    {
                        var dirPath = dirInfo.FullName;
                        if (!IsRenamed(dirInfo.Name)) // 是否已经重命名过
                        {
                            var code = FilePathUtility.GetCodeFromLongStr(dirInfo.Name);
                            if (FilePathUtility.TryToInteger(code, out var newCode))
                            {
                                var newName = GetNewFileName(newCode);
                                var newPath = FilePathUtility.GetNewDirPath(dirPath, newName);
                                if (newPath.ToUpper() != dirPath.ToUpper())
                                {
                                    dirPath = FilePathUtility.MoveDir(dirPath, newPath, ref dumCode);
                                }
                            }
                        }
                        //递归调用
                        if (isLoop)
                        {
                            Rename(dirPath, back + 1);
                        }
                    }
                    else if (fsinfo is FileInfo fileInfo)
                    {
                        if (!FilePathUtility.RENAME_EXT.Contains(fileInfo.Extension.ToLower()))
                        {
                            continue;
                        }

                        var fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                        if (!IsRenamed(fileName)) // 是否已经重命名过
                        {
                            var code = FilePathUtility.GetCodeFromLongStr(fileInfo.Name);
                            string? newPath = null;
                            if (FilePathUtility.TryToInteger(code, out var newCode))
                            {
                                var newName = GetNewFileName(newCode);
                                newPath = FilePathUtility.GetNewFileName(fileInfo.FullName, newName);
                                if (newPath.ToUpper() != fileInfo.FullName.ToUpper())
                                {
                                    FilePathUtility.MoveFile(fileInfo.FullName, newPath, ref conflict);
                                    renameCount++;
                                }
                            }
                            else if (isMoveUnformatFile) // 将不符合命名规范的文件放到单独的垃圾桶目录
                            {
                                // 垃圾桶
                                var fileDir = Path.GetDirectoryName(fileInfo.FullName);
                                var nongoalsDir = fileDir + "/" + config?.UnknownFolderName ?? "NoneGoalFiles";
                                if (!Directory.Exists(nongoalsDir))
                                {
                                    Directory.CreateDirectory(nongoalsDir);
                                }

                                newPath = nongoalsDir + "/" + fileInfo.Name;
                                fileInfo.MoveTo(newPath);
                            }
                        }
                    }

                    index++;
                }
            }

            bool IsRenamed(string fileName)
            {
                var config = _configDriver.GetConfig();
                if (config is { })
                {
                    var formatStart = string.Format(config.StandardFormat ?? "", string.Empty);
                    var formatLen = formatStart.Length;
                    if (fileName.Length >= formatLen && fileName.ToUpper().StartsWith(formatStart.ToUpper()))
                    {
                        if (fileName.Length - formatLen > 1)
                        {
                            if (fileName[formatLen] == '-')
                            {
                                var numStr = fileName.Substring(formatLen + 1, fileName.Length - formatLen);
                                if (int.TryParse(numStr, out var num))
                                {
                                    return true; // 有序号并且格式正确
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (fileName.Length - formatLen == 1)
                        {
                            return false;
                        }
                        else
                        {
                            return true; // 没有序号
                        }
                    }
                }

                return false;
            }
        }

        public string GetNewFileName(int code)
        {
            var config = _configDriver.GetConfig();
            if (config.StandardFormat is { })
            {
                return string.Format(config.StandardFormat, code);
            }

            return code.ToString();
        }

        public void WriteUnknownCodeToDatebase(IList<string> links) => WriteLinkCodeToDatebase(links);

        private void WriteLinkCodeToDatebase(IList<string> links, LevelType levelType = LevelType.Unset, TypeType typeType = TypeType.Unknown, bool updateIfExist = false)
        {
            foreach (var link in links)
            {
                var codeStr = FilePathUtility.GetCodeFromLongStr(link);
                if (FilePathUtility.TryToInteger(codeStr, out var code))
                {
                    if (updateIfExist)
                    {
                        _dBDriver.InsertOrUpdate(code, levelType, typeType);
                    }
                    else
                    {
                        _dBDriver.InsertIfNotExist(code, levelType, typeType);
                    }
                }
            }
        }


        public void Dispose()
        {
            _dBDriver.Dispose();
            _configDriver.Dispose();
        }
    }
}
