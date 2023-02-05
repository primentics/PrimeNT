using System.Text;

namespace AzyWorks.Configuration
{
    public class ConfigWriter
    {
        private StringBuilder _buffer = new StringBuilder();

        private string _curKey;

        public void Write(string value)
        {
            _buffer.AppendLine($"= {_curKey} =");
            _buffer.AppendLine($"{value}");
        }


        public void WriteDescription(string description)
        {
            _buffer.AppendLine($"# {description}");
        }

        public void SetOnKey(string key)
        {
            _curKey = key;
        }

        public void FinishKey()
        {
            _buffer.AppendLine();
            _curKey = null;
        }

        public void ResetBuffer()
            => _buffer.Clear();

        public string GetBuffer()
        {
            var buffer = _buffer.ToString();

            ResetBuffer();

            return buffer;
        }
    }
}