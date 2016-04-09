namespace Fenton.Rest
{
    using System.ComponentModel;
    using System.Configuration;

    public static class Config
    {
        public static T GetType<T>(string key, T defaultValue = default(T))
        {
            T result = defaultValue;
            var item = ConfigurationManager.AppSettings[key];
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (item != null && converter != null && converter.IsValid(item))
            {
                result = (T)converter.ConvertFromString(item);
            }

            return result;
        }
    }
}
