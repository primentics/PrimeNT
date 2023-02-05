namespace AzyWorks.Logging
{
    public class PersonalLog
    {
        private string _sourceName;

        public PersonalLog(string sourceName)
        {
            _sourceName = sourceName;
        }

        public void Info(object message)
            => Log.SendInfo(_sourceName, message);

        public void Warn(object message)
            => Log.SendWarn(_sourceName, message);

        public void Error(object message)
            => Log.SendError(_sourceName, message);

        public void Debug(object message)
            => Log.SendDebug(_sourceName, message);
    }
}