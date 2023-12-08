using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static string watchedDirectory;
    static string logFilePath;

    static void Main()
    {
        ConfigureMonitoring();
        using (FileSystemWatcher watcher = new FileSystemWatcher(watchedDirectory))
        {
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
            watcher.Created += OnFileOrDirectoryCreated;
            watcher.Deleted += OnFileOrDirectoryDeleted;
            watcher.Renamed += OnFileOrDirectoryRenamed;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Отслеживание запущено. Для выхода нажмите любую клавишу.");
            Console.ReadKey();
        }
    }

    static void ConfigureMonitoring()
    {
        Console.Write("Введите путь к отслеживаемой директории: ");
        watchedDirectory = Console.ReadLine();
        if (!Directory.Exists(watchedDirectory))
        {
            Console.WriteLine("Указанная директория не существует. Завершение программы.");
            Environment.Exit(1);
        }
        Console.Write("Введите путь к лог-файлу: ");
        logFilePath = Console.ReadLine();
    }

    static void OnFileOrDirectoryCreated(object sender, FileSystemEventArgs e)
    {
        LogChange($"Создан {GetFileSystemType(e)}: {e.FullPath}");
    }

    static void OnFileOrDirectoryDeleted(object sender, FileSystemEventArgs e)
    {
        LogChange($"Удален {GetFileSystemType(e)}: {e.FullPath}");
    }

    static void OnFileOrDirectoryRenamed(object sender, RenamedEventArgs e)
    {
        LogChange($"Переименован {GetFileSystemType(e)}: {e.OldFullPath} -> {e.FullPath}");
    }

    static string GetFileSystemType(FileSystemEventArgs e)
    {
        return e is FileEventArgs ? "файл" : "директория";
    }

    static void LogChange(string change)
    {
        try
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {change}\n");
            Console.WriteLine(change);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }
}

class FileEventArgs : FileSystemEventArgs
{
    public FileEventArgs(WatcherChangeTypes changeType, string directory, string name) : base(changeType, directory, name) { }
}
