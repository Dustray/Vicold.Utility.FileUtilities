// See https://aka.ms/new-console-template for more information
Console.WriteLine("#Start Rename");

//string sourceDir = "./DemoFiles/";
string sourceDir = @"Y:\Bone\115d2\auto_create@2022.6.25 20.35.39 (491)";
string targetFormat = "FC-PPV-{0}";

string nongoalsFileDir = "NoneGoalFiles";
bool isLoop = true;

int dumCode = 0;

Rename(Path.GetFullPath(sourceDir),0);

Console.WriteLine("#End Rename");
Console.ReadLine();

void Rename(string thisDir, int back)
{
    //绑定到指定的文件夹目录
    DirectoryInfo dir = new DirectoryInfo(thisDir);

    //检索表示当前目录的文件和子目录
    FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

    //遍历检索的文件和子目录
    int index = 0;
    int conflict = 1;
    foreach (FileSystemInfo fsinfo in fsinfos)
    {
        //判断是否为空文件夹　　
        if (fsinfo is DirectoryInfo dirInfo)
        {
            var dirPath = dirInfo.FullName;
            var newCode = GetFCFileCode(dirInfo.Name);
            if (newCode != null)
            {
                var newName = ToNewName(newCode);
                var newPath = ToNewDirPath(dirInfo.FullName, newName);
                if (newPath.ToUpper() != dirInfo.FullName.ToUpper())
                {
                    dirPath = MoveDir(dirInfo.FullName, newPath);
                    Console.WriteLine($"{empty(back)}[{index}]Renamed Dir: {newCode}");
                }
            }


            //递归调用
            if (isLoop)
            {
                Rename(dirPath, back+1);
            }
        }
        else if (fsinfo is FileInfo fileInfo)
        {
            var newCode = GetFCFileCode(fileInfo.Name);
            if ("1546423" == newCode)
            {

            }
            string? newPath = null;
            if (newCode != null)
            {
                var newName = ToNewName(newCode);
                newPath = ToNewFilePath(fileInfo.FullName, newName);
                if (newPath.ToUpper() != fileInfo.FullName.ToUpper())
                {
                    MoveFile(fileInfo.FullName, newPath,ref conflict);
                }

                Console.WriteLine($"{empty(back)}[{index}]Renamed File: {newCode}");
            }
            else
            {
                // 垃圾桶
                var fileDir = Path.GetDirectoryName(fileInfo.FullName);
                var nongoalsDir = fileDir + "/" + nongoalsFileDir;
                if (!Directory.Exists(nongoalsDir))
                {
                    Directory.CreateDirectory(nongoalsDir);
                }

                newPath = nongoalsDir + "/" + fileInfo.Name;
                fileInfo.MoveTo(newPath);
                Console.WriteLine($"{empty(back)}[{index}]Trash File: {newPath}");
            }

        }
        index++;
    }

}

string? GetFCFileCode(string name)
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
        return new string(code.ToArray());
    }
    else
    {
        return null;
    }
}

string ToNewName(string code)
{
    return string.Format(targetFormat, code);
}

string ToNewFilePath(string oldFullName, string newName)
{
    var dir = Path.GetDirectoryName(oldFullName);
    var ext = Path.GetExtension(oldFullName);
    var newFullName = Path.Combine(dir, newName + ext);
    return newFullName;
}

string MoveFile(string oldPath, string newPath,ref int conflict)
{

    if (File.Exists(newPath))
    {
        var dir = Path.GetDirectoryName(newPath);
        var name = Path.GetFileNameWithoutExtension(newPath);
        Console.WriteLine("****冲突：" + name);
        var ext = Path.GetExtension(newPath);
        newPath = Path.Combine(dir, name + "-" + (conflict++) + ext);
    }

    File.Move(oldPath, newPath);
    return newPath;
}

string ToNewDirPath(string oldFullName, string newName)
{
    if (oldFullName[oldFullName.Length - 1] == '\\' || oldFullName[oldFullName.Length - 1] == '/')
    {
        oldFullName = oldFullName.Substring(0, oldFullName.Length - 1);
    }

    var dir = oldFullName.Substring(0, oldFullName.LastIndexOf('\\'));
    var newFullName = Path.Combine(dir, newName);
    return newFullName;
}

string MoveDir(string oldPath, string newPath)
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

string empty(int len)
{
    len *= 4;
    char[] chars = new char[len];
    for(int i = 0; i < len; i++)
    {
        chars[i] = '-';
    }
    return new string(chars);
}