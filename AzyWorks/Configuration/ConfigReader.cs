using AzyWorks.Pooling;

using System;
using System.Linq;
using System.Text;

namespace AzyWorks.Configuration
{
    public class ConfigReader
    {
        private ConfigHandler _handler;

        private string[] _buffer;

        private string _curLine;
        private string _curKey;
        private string _curValue;

        private int _curLinePos;
        private int _movedPos;

        public string CurrentKey { get => _curKey; }
        public string CurrentValue { get => _curValue; }

        public ConfigReader(ConfigHandler handler)
        {
            _handler = handler;
        }

        public bool ReadConfig()
        {
            while (MoveToNextLine())
            {
                if (string.IsNullOrWhiteSpace(_curLine))
                    continue;

                if (_curLine.StartsWith("#"))
                    continue;

                if (_curLine.Count(x => x == '=') != 2)
                    continue;

                ReadKey();
                ReadValue();

                var config = _handler._configs.FirstOrDefault(x => x.Name == CurrentKey);

                if (config != null)
                {
                    if (_handler._converter.TryConvert(this, config.ValueType, out var result))
                    {
                        config.SetValue(result);
                    }
                }
                else
                {
                    Log.SendWarn("Config Reader", $"Missing config key: {CurrentKey}");
                }
            }

            return false;
        }

        public bool MoveToNextLine()
        {
            _curLinePos++;

            if (_curLinePos >= _buffer.Length)
            {
                return false;
            }

            try
            {
                _curLine = _buffer[_curLinePos];
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadKey()
        {
            if (_curLine.Count(x => x == '=') != 2)
                throw new InvalidOperationException($"_curLine ({_curLine}) is not a valid key!");

            var firstIndex = _curLine.IndexOf('=');
            var lastIndex = _curLine.LastIndexOf('=');

            if (firstIndex == -1 || lastIndex == -1)
                throw new InvalidOperationException($"_curLine is invalid. ({firstIndex}/{lastIndex})");

            _curKey = _curLine
                .Replace("=", "")
                .Trim();
        }

        private void ReadValue()
        {
            var sb = PoolManager.Get<StringBuilder>();

            _movedPos = _curLinePos + 1;

            while (TryPeekNextLine(_movedPos, out var line))
            {
                if (string.IsNullOrWhiteSpace(line))
                    break;

                if (line.StartsWith("#"))
                    break;

                if (line.Count(x => x == '=') == 2)
                    break;

                sb.AppendLine(line);

                _movedPos++;
            }

            _curValue = sb.ToString().Trim();
            _curLinePos = _movedPos;

            PoolManager.Return(sb);
        }

        private bool TryPeekNextLine(int pos, out string line)
        {
            if (pos >= _buffer.Length)
            {
                line = null;
                return false;
            }

            try
            {
                line = _buffer[pos];
                return true;
            }
            catch
            {
                line = null;
                return false;
            }
        }

        public void SetBuffer(string[] buffer)
        {
            if (_buffer is null)
                _buffer = new string[buffer.Length];

            if (buffer.Length > _buffer.Length)
                Array.Resize(ref _buffer, buffer.Length);

            Array.Copy(buffer, _buffer, buffer.Length);
        }

        public void ClearBuffer()
        {
            if (_buffer is null)
                return;

            Array.Clear(_buffer, 0, _buffer.Length);
        }
    }
}