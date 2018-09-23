using System;
using System.Collections.Generic;
using System.Text;

namespace RssCatalog.Model
{
    public class Blog
    {
        public Guid Id { get; set; }
        public string BlogName { get; set; }
        public string Url { get; set; }
        public string FeedUrl { get; set; }
        public string FeedType { get; set; }
    }
}
