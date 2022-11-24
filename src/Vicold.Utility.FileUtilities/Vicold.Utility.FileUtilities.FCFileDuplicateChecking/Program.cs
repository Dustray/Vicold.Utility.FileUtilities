// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;


var ppo = "O:\\夸克\\走进生活";

// 遍历文件
var files = Directory.GetFiles(ppo, "*.*", SearchOption.AllDirectories);

foreach (var file in files)
{
    var newfile = file.Replace(".ra删除r", ".rar");
    // 重命名

    File.Move(file, newfile);
}


Console.WriteLine("#Start Search");
return;

string sourceDir = @"O:\2127\新1.txt";
string saveDir = @"O:\2127\新1_filter.txt";
string searchTargetFolder = @"Z:\WorldPlace\beautiful\FC2-PPV";


HashSet<int> hash = new HashSet<int>();
Rename(searchTargetFolder);
List<string> result = new List<string>();
using StreamReader sr = new StreamReader(sourceDir, System.Text.Encoding.Default);
string? line;
while ((line = sr.ReadLine()) != null)
{
    var newCode = GetFCFileCode(line);
    if (newCode != null)
    {
        if (!hash.TryGetValue(newCode.Value,out _))
        {
            result.Add(line);
        }
        else
        {
            Console.WriteLine($"重复：{newCode.Value}");
        }
    }
}
using StreamWriter sw = new StreamWriter(saveDir,false, System.Text.Encoding.UTF8);

foreach (var resultLine in result)
{
    sw.WriteLine(resultLine);
}

Console.WriteLine("#End Search");
Console.ReadLine();


void Rename(string thisDir)
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
            var newCode = GetFCFileCode(dirInfo.Name);
            if (newCode != null)
            {
                hash.Add(newCode.Value);
            }

            Rename(dirPath);
        }
        else if (fsinfo is FileInfo fileInfo)
        {
            var newCode = GetFCFileCode(fileInfo.Name);
            if (newCode != null)
            {
                hash.Add(newCode.Value);
            }
        }
    }
}



int? GetFCFileCode(string name)
{
    var minLen = 5;
    var maxLen = 9;
    IList<char> code = new List<char>(maxLen);
    bool isContinuous = false;
    foreach (char ascii in name)
    {

        if (int.TryParse(ascii.ToString(), out int num))
        {
            isContinuous = true;
            code.Add(ascii);
        }
        else
        {
            if (code.Count < minLen || code.Count > maxLen)
            {
                code.Clear();
                isContinuous = false;
            }
            else
            {
                break;
            }
        }
    }

    if (isContinuous && code.Count >= minLen && code.Count <= maxLen)
    {
        var codeStr = new string(code.ToArray());
        return int.Parse(codeStr);
    }
    else
    {
        return null;
    }
}