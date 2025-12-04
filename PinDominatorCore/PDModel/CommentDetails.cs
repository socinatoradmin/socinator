using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.PDModel
{
    public class CommentDetails
    {
        public PinterestUser Commentor {  get; set; }
        public string Comment { get; set; }
        public string CommentId {  get; set; }
    }
}
