using System;

namespace GramDominatorCore.GDModel
{
    public abstract class InstagramAction
    {
        public InstagramUser GetUser()
        {
            User user = this as User;
            if (user != null)
                return user.InstagramUser;
            var post = this as Post;
            if (post != null)
                return post.InstagramPost.User;
            throw new ArgumentException("ScrapeResult has to be of type User");
        }

        public sealed class Post : InstagramAction
        {
            public Post(InstagramPost instagramPost)
            {
                InstagramPost = instagramPost;
            }

            public InstagramPost InstagramPost { get; set; }
        }

        public sealed class User : InstagramAction
        {
            public User(InstagramUser instagramUser)
            {
                InstagramUser = instagramUser;
            }

            public InstagramUser InstagramUser { get; set; }
        }
    }
}
