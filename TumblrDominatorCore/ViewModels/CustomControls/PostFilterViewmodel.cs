using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.ViewModels.CustomControls
{
    public class PostFilterViewmodel : BindableBase
    {
        private string _blacklistInput = string.Empty;


        private PostFilterModel _postfilterModel = new PostFilterModel();

        private string _whitelistInput = string.Empty;

        public PostFilterViewmodel()
        {
            CmdBlacklistCaption = new BaseCommand<object>(sender => true, BlacklistInputBox_GetInput);
            CmdWhitelistCaption = new BaseCommand<object>(sender => true, WhitelistInputBox_GetInput);
        }

        public ICommand CmdWhitelistCaption { get; set; }
        public ICommand CmdBlacklistCaption { get; set; }

        public PostFilterModel PostfilterModel
        {
            get => _postfilterModel;
            set
            {
                if ((_postfilterModel == null) & (_postfilterModel == value))
                    return;
                SetProperty(ref _postfilterModel, value);
            }
        }

        public string WhitelistInput
        {
            get => _whitelistInput;

            set
            {
                _whitelistInput = value;
                SetProperty(ref _whitelistInput, value);
            }
        }

        public string BlacklistInput
        {
            get => _blacklistInput;

            set
            {
                _blacklistInput = value;
                SetProperty(ref _blacklistInput, value);
            }
        }

        public void WhitelistInputBox_GetInput(object sender)
        {
            PostfilterModel.CaptionWhitelist = WhitelistInput;
            PostfilterModel.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(WhitelistInput, "\r\n").ToList());
            GlobusLogHelper.log.Info("Caption Whitelist saved successfully");
        }


        public void BlacklistInputBox_GetInput(object sender)
        {
            PostfilterModel.CaptionBlacklists = WhitelistInput;
            PostfilterModel.RestrictedPostCaptionList =
                new ObservableCollection<string>(Regex.Split(WhitelistInput, "\r\n").ToList());
            GlobusLogHelper.log.Info("Caption blacklist saved successfully");
        }
    }
}