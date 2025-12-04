using AutoMapper;
using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using RedditDominatorCore.RDModel;

namespace RedditDominatorUI.AutoMapping
{
    // ReSharper disable once UnusedMember.Global
    public class RedditMapperProfile : Profile
    {
        public RedditMapperProfile()
        {
            CreateMap<InteractedUsers, RedditUser>()
                .ForMember(a => a.CommentKarma, a => a.MapFrom(b => b.CommentKarma))
                .ForMember(a => a.Username, a => a.MapFrom(b => b.InteractedUsername))
                .ForMember(a => a.Id, a => a.MapFrom(b => b.InteractedUserId))
                .ForMember(a => a.AccountIcon, a => a.MapFrom(b => b.AccountIcon))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForMember(a => a.DisplayName, a => a.MapFrom(b => b.DisplayName))
                .ForMember(a => a.DisplayNamePrefixed, a => a.MapFrom(b => b.DisplayNamePrefixed))
                .ForMember(a => a.DisplayText, a => a.MapFrom(b => b.DisplayText))
                .ForMember(a => a.HasUserProfile, a => a.MapFrom(b => b.HasUserProfile))
                .ForMember(a => a.IsEmployee, a => a.MapFrom(b => b.IsEmployee))
                .ForMember(a => a.IsFollowing, a => a.MapFrom(b => b.IsFollowing))
                .ForMember(a => a.IsGold, a => a.MapFrom(b => b.IsGold))
                .ForMember(a => a.IsMod, a => a.MapFrom(b => b.IsMod))
                .ForMember(a => a.IsNsfw, a => a.MapFrom(b => b.IsNsfw))
                .ForMember(a => a.PrefShowSnoovatar, a => a.MapFrom(b => b.PrefShowSnoovatar))
                .ForMember(a => a.PostKarma, a => a.MapFrom(b => b.PostKarma))
                .ForMember(a => a.Url, a => a.MapFrom(b => b.Url));
        }
    }
}