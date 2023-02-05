using AzyWorks.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzyWorks.IO
{
    public class DirectoryManager : DisposableObject
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
            ThrowIfDisposed();

            return _dirInfo.EnumerateDirectories().Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<DirectoryManager> GetDirectories(string searchPattern)
        {
            ThrowIfDisposed();

            return _dirInfo.EnumerateDirectories(searchPattern).Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<DirectoryManager> GetDirectories(string searchPattern, SearchOption searchOption)
        {
            ThrowIfDisposed();

            return _dirInfo.EnumerateDirectories(searchPattern, searchOption).Select(x => new DirectoryManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles()
        {
            ThrowIfDisposed();

            return _dirInfo.EnumerateFiles().Select(x => new FileManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles(string searchPattern)
        {
            ThrowIfDisposed();

            return _dirInfo.EnumerateFiles(searchPattern).Select(x => new FileManager(x.FullName)).ToList();
        }

        public List<FileManager> GetFiles(string searchPattern, SearchOption searchOption) 
        {
            ThrowIfDisposed();

            return _dirInfo.EnumerateFiles(searchPattern, searchOption).Select(x => new FileManager(x.FullName)).ToList();
        }

        public void Create()
        {
            ThrowIfDisposed();

            if (!Exists)
                _dirInfo.Create();
        }

        public void Delete()
        {
            ThrowIfDisposed();

            if (Exists)
                _dirInfo.Delete(true);
        }

        public void CreateSubdirectory(string path)
        {
            ThrowIfDisposed();

            _dirInfo.CreateSubdirectory(path);
        }

        public void Move(string path)
        {
            ThrowIfDisposed();

            _dirInfo.MoveTo(path);
        }

        public override void Dispose()
        {
            base.Dispose();

            _dirInfo = null;
        }
    }
}
