using System;

namespace PinDominatorCore.PDModel
{
    public abstract class PinterestAction
    {
        public PinterestUser GetUser()
        {
            var user = this as User;
            if (user != null)
                return user.PinterestUser;
            var post = this as Post;
            if (post != null)
                return post.PinterestPin.User;
            throw new ArgumentException("ScrapeResult has to be of type User");
        }

        public sealed class Post : PinterestAction
        {
            public Post(PinterestPin pinterestPin)
            {
                PinterestPin = pinterestPin;
            }

            public PinterestPin PinterestPin { get; set; }
        }

        public sealed class User : PinterestAction
        {
            public User(PinterestUser pinterestUser)
            {
                PinterestUser = pinterestUser;
            }

            public PinterestUser PinterestUser { get; set; }
        }
    }
}