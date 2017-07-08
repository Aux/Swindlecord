using System;
using System.Collections.Generic;
using System.Text;

namespace Swindlecord
{
    public class PostedTweet
    {
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public string Content { get; set; }
    }
}
