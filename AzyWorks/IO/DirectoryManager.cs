using AzyWorks.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzyWorks.IO
{
    public class DirectoryManager 
    {
        private DirectoryInfo _dirInfo;

        public DirectoryManager(string directoryPath) 
        {
            _dirInfo = new DirectoryInfo(directoryPath);
        }

        public string Path { get => _dirInfo.FullName; }
        public string Name { get => _dirInfo.Name; }

        public bool Exists { get => _dirInfo.Exists; }

        public DateTime LastWriteTime { get => _dirInfo.LastWriteTime; set => Directory.SetLastWriteTime(Path, value); }
        public DateTime LastAccessTime { get => _dirInfo.LastAccessTime; set => Directory.SetLastAccessTime(Path, value); }
        public DateTime CreationTime { get => _dirInfo.CreationTime; set => Directory.SetCreationTime(Path, value); }

        public DirectoryManager Parent { get => new DirectoryManager(_dirInfo.Parent.FullName); }
        public DirectoryManager Root { get => new DirectoryManager(_dirInfo.Root.FullName); }

        public List<DirectoryManager> GetDirectories()
        {
            return _dirInfo.EnumerateDirectories().Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<DirectoryManager> GetDirectories(string searchPattern)
        {
            return _dirInfo.EnumerateDirectories(searchPattern).Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<DirectoryManager> GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return _dirInfo.EnumerateDirectories(searchPattern, searchOption).Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles()
        {
            return _dirInfo.EnumerateFiles().Select(x => new FileManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles(string searchPattern)
        {
            return _dirInfo.EnumerateFiles(searchPattern).Select(x => new FileManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles(string searchPattern, SearchOption searchOption) 
        {
            return _dirInfo.EnumerateFiles(searchPattern, searchOption).Select(x => new FileManager(x.FullName)).ToList();
        }

        public void Create()
        {
            if (!Exists)
                _dirInfo.Create();
        }

        public void Delete()
        {
            if (Exists)
                _dirInfo.Delete(true);
        }

        public void CreateSubdirectory(string path)
        {
            _dirInfo.CreateSubdirectory(path);
        }

        public void Move(string path)
        {
            _dirInfo.MoveTo(path);
        }
    }
}
