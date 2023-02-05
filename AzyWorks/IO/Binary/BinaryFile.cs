using System.IO;

namespace AzyWorks.IO.Binary
{
    public class BinaryFile
    {
        private FileManager _file;
        private BinaryHeader _fileHeader;
        private BinaryFileDataContainer _fileContainer;

        private bool _readSuccesfully;

        public BinaryFile(string path)
        {
            _file = new FileManager(path);
        }

        public bool ReadFile(bool throwIfNotExist = false)
        {
            _readSuccesfully = false;

            if (!_file.Exists)
            {
                if (throwIfNotExist)
                    throw new FileNotFoundException(_file.Path);

                return false;
            }

            using (var sourceStream = _file.OpenFile())
            using (var reader = new BinaryReader(sourceStream))
            {
                _fileHeader = reader.Read<BinaryHeader>();
            }

            if (_fileHeader is null)
                return false;

            var containerObject = _fileHeader.Objects[0];

            if (containerObject is null)
                return false;

            _fileContainer = BinaryHelpers.DeserializeObject(containerObject) as BinaryFileDataContainer;
            _readSuccesfully = true;

            return true;
        }

        public bool WriteFile()
        {
            if (_fileHeader is null)
                return false;

            if (_fileContainer is null)
                return false;

            var containerObject = BinaryHelpers.SerializeObject(_fileContainer);

            if (containerObject is null)
                return false;

            _fileHeader.Objects = new BinaryObject[1] { containerObject };

            using (var writer = new BinaryWriter(_file.OpenFile()))
            {
                writer.Write(_fileHeader);
            }

            return true;
        }

        public T GetData<T>(string key)
        {
            if (!TryGetDataByKey(key, out var data))
                return default;

            var value = BinaryHelpers.DeserializeObject(data);

            if (value is null)
                return default;

            return (T)value;
        }

        public bool WriteData(string key, object value)
        {
            if (_fileContainer is null)
                InitInMemory();

            var data = BinaryHelpers.SerializeObject(value);

            return _fileContainer.TryInsertData(key, data);
        }

        private bool TryGetDataByKey(string key, out BinaryObject binaryObject)
        {
            binaryObject = null;

            if (!_readSuccesfully)
                return false;

            return _fileContainer.TryGetData(key, out binaryObject);
        }

        private void InitInMemory()
        {
            _fileHeader = new BinaryHeader();
            _fileContainer = new BinaryFileDataContainer();
        }
    }
}