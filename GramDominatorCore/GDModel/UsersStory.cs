using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.GDModel
{
    public class UsersStory
    {    
        public List<UsersPostStory> LstMedia { get; set; } = new List<UsersPostStory>();
        public string UserId { get; set; }
    }
    public class UsersPostStory
    {
        public string UserId { get; set; }
        public string UserMediaId { get; set; }
        public string PostTime { get; set; }
        public string currentTime { get; set; }
        public string PostId { get; set; }
        public string MediaType { get; set; }
    }
}
