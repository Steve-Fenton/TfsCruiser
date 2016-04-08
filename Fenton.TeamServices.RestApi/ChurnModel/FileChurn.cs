using System.IO;
using System.Text.RegularExpressions;

namespace Fenton.TeamServices.BuildRestApi
{
    public class FileChurn
    {
        public string ItemName { get; set; }

        public int Version { get; set; }

        public int Count { get; set; }

        public int Score { get; set; }

        public int Percentile { get; set; }

        public string DisplayName
        {
            get
            {
                var fileName = Path.GetFileName(ItemName);
                fileName = Regex.Replace(fileName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", "&shy;$0");
                return fileName.Replace(".", "&shy;.");
            }
        }

        public string FolderName
        {
            get
            {
                return ItemName.Substring(0, ItemName.Length - DisplayName.Length);
            }
        }
    }
}