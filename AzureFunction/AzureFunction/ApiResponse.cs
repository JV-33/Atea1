using System.Collections.Generic;

namespace AzureFunction
{
    public class ApiResponse
    {
        public int Count { get; set; }
        public List<ApiEntry> Entries { get; set; }
    }

    public class ApiEntry
    {
        public string API { get; set; }
        public string Description { get; set; }
        public string Auth { get; set; }
        public bool HTTPS { get; set; }
        public string Cors { get; set; }
        public string Link { get; set; }
        public string Category { get; set; }
    }
}