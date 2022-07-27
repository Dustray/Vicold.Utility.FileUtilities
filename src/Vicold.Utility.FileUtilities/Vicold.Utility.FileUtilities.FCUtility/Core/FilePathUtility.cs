using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Core
{
    internal static class FilePathUtility
    {
        private static readonly int MIN_LEN = 5;
        private static readonly int MAX_LEN = 9;
        private static readonly string[] PRE_STR_HINT = { "ppv-", "ppv", "fc", "v-", "-", "v" }; // 顺序不可改
        private static readonly string[] OUT_EXT = { ".torrent", ".gif", ".jpg", ".png", ".mov", ".mov", ".tmp" };
        public static string? GetCodeFromLongStr(string longStr)
        {
            foreach (var exts in OUT_EXT)
            {
                if (longStr.ToLower().EndsWith(exts))
                {
                    return null;
                }
            }

            var minLen = 6;
            var maxLen = 9;
            IList<char> code = new List<char>(maxLen);
            string? result = null;
            int resultIndex = 999;
            int index = 0;
            int maxHintIndex = PRE_STR_HINT.Length;
            foreach (char ascii in longStr)
            {

                if (int.TryParse(ascii.ToString(), out int num))
                {
                    code.Add(ascii);
                }
                else
                {
                    if (code.Count < minLen || code.Count > maxLen)
                    {
                        code.Clear();
                    }
                    else
                    {
                        if (code.Count < minLen && code.Count > maxLen)
                        {
                            continue;
                        }

                        // 找到了
                        var hintIndex = CheckPreStrIndex(index - code.Count);
                        if (hintIndex < resultIndex)
                        {
                            result = new string(code.ToArray());
                        }

                        code.Clear();
                    }
                }

                index++;
            }

            if (result is not { } && code.Count >= minLen && code.Count <= maxLen)
            {
                result = new string(code.ToArray());
            }

            int CheckPreStrIndex(int searchIndex)
            {
                for (var i = 0; i < maxHintIndex; i++)
                {
                    var preStr = PRE_STR_HINT[i];

                    if (preStr.Length >= searchIndex + 1)
                    {
                        continue;
                    }

                    if (preStr is { })
                    {
                        var target = longStr.Substring(searchIndex - preStr.Length, preStr.Length);
                        if (target is { } && target.ToLower() == preStr)
                        {
                            return i;
                        }
                    }
                }

                return maxHintIndex;
            }

            return result;
        }

        /// <summary>
        /// 获取重复文件（size相同）
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static Dictionary<int, List<string>> GetDupCodesPathInFolderLoop(IList<string> folders)
        {
            // <code, <size, path[]>>
            Dictionary<int, Dictionary<long, List<string>>> result = new Dictionary<int, Dictionary<long, List<string>>>();
            foreach (var folder in folders)
            {
                Search(folder);
            }

            void Search(string thisDir)
            {
                //绑定到指定的文件夹目录
                DirectoryInfo dir = new DirectoryInfo(thisDir);

                //检索表示当前目录的文件和子目录
                FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

                //遍历检索的文件和子目录
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    if (fsinfo is FileInfo fileInfo)
                    {
                        if (fileInfo.Extension.ToLower() != ".mp4")
                        {
                            continue;
                        }

                        var newCode = GetCodeFromLongStr(fileInfo.Name);
                        if (newCode is { } && int.TryParse(newCode, out var code))
                        {
                            if (result.TryGetValue(code, out var sizeDict))
                            {
                                if (sizeDict.TryGetValue(fileInfo.Length, out var pathList))
                                {
                                    pathList.Add(fileInfo.FullName);
                                }
                                else
                                {
                                    sizeDict[fileInfo.Length] = new List<string>() { fileInfo.FullName };
                                }
                            }
                            else
                            {
                                result[code] = new Dictionary<long, List<string>>() { { fileInfo.Length, new List<string>() { fileInfo.FullName } } };
                            }
                        }
                    }
                }
            }

            var filteredResult = new Dictionary<int, List<string>>();
            foreach (var item in result)
            {
                foreach (var sizeItem in item.Value)
                {
                    if (sizeItem.Value.Count > 1)
                    {
                        if (filteredResult.TryGetValue(item.Key, out var pathList))
                        {
                            pathList.AddRange(sizeItem.Value);
                        }
                        else
                        {
                            filteredResult[item.Key] = sizeItem.Value;
                        }
                    }
                }
            }

            return filteredResult;
        }

        public static IEnumerable<KeyValuePair<int, string>> GetAllCodesPathInFolderLoop(string folder)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            Search(folder);
            void Search(string thisDir)
            {
                //绑定到指定的文件夹目录
                DirectoryInfo dir = new DirectoryInfo(thisDir);

                //检索表示当前目录的文件和子目录
                FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

                //遍历检索的文件和子目录
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    //判断是否为空文件夹　　
                    if (fsinfo is DirectoryInfo dirInfo)
                    {
                        var dirPath = dirInfo.FullName;
                        var newCode = GetCodeFromLongStr(dirInfo.Name);
                        if (newCode is { } && int.TryParse(newCode, out var code))
                        {
                            result[code] = dirPath;
                        }

                        Search(dirPath);
                    }
                    else if (fsinfo is FileInfo fileInfo)
                    {
                        if (fileInfo.Extension.ToLower() != ".mp4")
                        {
                            continue;
                        }

                        var newCode = GetCodeFromLongStr(fileInfo.Name);
                        if (newCode is { } && int.TryParse(newCode, out var code))
                        {
                            result[code] = fileInfo.FullName;
                        }
                    }
                }
            }

            return result;
        }

        public static IEnumerable<int> GetAllCodesInFolderLoop(string folder)
        {
            HashSet<int> result = new HashSet<int>();
            Search(folder);
            void Search(string thisDir)
            {
                //绑定到指定的文件夹目录
                DirectoryInfo dir = new DirectoryInfo(thisDir);

                //检索表示当前目录的文件和子目录
                FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

                //遍历检索的文件和子目录
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    //判断是否为空文件夹　　
                    if (fsinfo is DirectoryInfo dirInfo)
                    {
                        var dirPath = dirInfo.FullName;
                        var newCode = GetCodeFromLongStr(dirInfo.Name);
                        if (newCode is { } && int.TryParse(newCode, out var code))
                        {
                            result.Add(code);
                        }

                        Search(dirPath);
                    }
                    else if (fsinfo is FileInfo fileInfo)
                    {
                        if (fileInfo.Extension.ToLower() != ".mp4")
                        {
                            continue;
                        }

                        var newCode = GetCodeFromLongStr(fileInfo.Name);
                        if (newCode is { } && int.TryParse(newCode, out var code))
                        {
                            result.Add(code);
                        }
                    }
                }
            }

            return result;
        }

        public static bool TryToInteger(string? str, out int code)
        {
            if (str is { } && int.TryParse(str, out code))
            {
                return true;
            }

            code = 0;
            return false;
        }

        /// <summary>
        /// 全路径修改文件名
        /// </summary>
        /// <param name="oldFullName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string GetNewFileName(string oldFullName, string newName)
        {
            var dir = Path.GetDirectoryName(oldFullName);
            if (dir == null)
            {
                return oldFullName;
            }

            var ext = Path.GetExtension(oldFullName);
            var newFullName = Path.Combine(dir, newName + ext);
            return newFullName;
        }

        /// <summary>
        /// 全路径修改最末尾文件夹名称
        /// </summary>
        /// <param name="oldFullName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string GetNewDirPath(string oldFullName, string newName)
        {
            if (oldFullName[^1] == '\\' || oldFullName[^1] == '/')
            {
                oldFullName = oldFullName[..^1];
            }

            var dir = oldFullName[..oldFullName.LastIndexOf('\\')];
            var newFullName = Path.Combine(dir, newName);
            return newFullName;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="conflict"></param>
        /// <returns></returns>
        public static string MoveFile(string oldPath, string newPath, ref int conflict)
        {

            if (File.Exists(newPath))
            {
                var dir = Path.GetDirectoryName(newPath);
                if (dir == null)
                {
                    return oldPath;
                }

                var name = Path.GetFileNameWithoutExtension(newPath);
                Console.WriteLine("****冲突：" + name);
                var ext = Path.GetExtension(newPath);
                newPath = Path.Combine(dir, name + "-" + (conflict++) + ext);
            }

            File.Move(oldPath, newPath);
            return newPath;
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public static string MoveDir(string oldPath, string newPath, ref int dumCode)
        {
            if (Directory.Exists(newPath))
            {
                if (newPath[newPath.Length - 1] == '\\' || newPath[newPath.Length - 1] == '/')
                {
                    newPath = newPath.Substring(0, newPath.Length - 1);
                }

                var dir = newPath.Substring(0, newPath.LastIndexOf('\\'));
                var name = Path.GetFileNameWithoutExtension(newPath);
                Console.WriteLine("****冲突：" + name);
                newPath = Path.Combine(dir, name + "——" + (dumCode++));
            }

            Directory.Move(oldPath, newPath);
            return newPath;
        }

        public static string? GetFileNameFromDragEventArgs(System.Windows.DragEventArgs e)
        {
            var data = e.Data.GetData(System.Windows.DataFormats.FileDrop);
            var array = data as Array;
            if (array is { })
            {
                var value = array.GetValue(0);
                if (value is { })
                {
                    var fileName = value.ToString();
                    return fileName;
                }
            }
            return null;
        }
    }
}
