using System;
using System.Text.RegularExpressions;

namespace Fenton.Forensics
{
    public class FolderChurn
    {
        public string ItemName { get; set; }

        public int Count { get; set; }

        public int Score { get; set; }

        public int Percentile { get; set; }

        public string DisplayName
        {
            get
            {
                var folderName = ItemName;

                if (folderName.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    folderName = folderName.Substring(folderName.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase) + 1);
                }

                folderName = Regex.Replace(folderName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", "&shy;$0");
                return folderName.Replace(".", "&shy;.");
            }
        }
    }
}