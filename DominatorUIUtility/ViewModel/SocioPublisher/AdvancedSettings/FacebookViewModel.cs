using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.ViewModel.SocioPublisher.AdvancedSettings
{
    public class FacebookViewModel : BindableBase
    {
        private FacebookModel _facebookModel = new FacebookModel();

        public FacebookViewModel()
        {
            SelectFriendsCommand = new BaseCommand<object>(SelectFriendsCanExecute, SelectFriendsCommandExecute);
            SelectPagesCommand = new BaseCommand<object>(SelectPagesCanExecute, SelectPagesCommandExecute);
            SelectMentionCommand = new BaseCommand<object>(SelectMentionCanExecute, SelectMentionCommandExecute);
            SaveFriendCommad = new BaseCommand<object>(SavePagesCanExecute, SaveFriendExecute);
            SavePageCommad = new BaseCommand<object>(SaveFriendCanExecute, SavePageExecute);
            SaveMentionCommad = new BaseCommand<object>(SaveMentionCanExecute, SaveMentionCommandExecute);
        }

        public ICommand SelectMentionCommand { get; set; }

        public ICommand SelectFriendsCommand { get; set; }

        public ICommand SelectPagesCommand { get; set; }

        public ICommand SaveFriendCommad { get; set; }

        public ICommand SavePageCommad { get; set; }

        public ICommand SaveMentionCommad { get; set; }

        public FacebookModel FacebookModel
        {
            get => _facebookModel;
            set
            {
                if (_facebookModel == value)
                    return;
                SetProperty(ref _facebookModel, value);
            }
        }

        private void SaveMentionCommandExecute(object obj)
        {
            try
            {
                var mentionUrlList = Regex.Split(FacebookModel.CustomMentionUser, "\r\n");
                mentionUrlList.ForEach(x => { FacebookModel.ListCustomMentionUser.Add(x); });

                FacebookModel.ListCustomMentionUser = FacebookModel.ListCustomMentionUser.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectMentionCommandExecute(object obj)
        {
            var model = FacebookModel.SelectFriendsDetailsModelForMention;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Page, FbEntityTypes.Group, FbEntityTypes.CustomDestination
            };


            var selectAccountDetailsControl =
                FacebookModel.SelectFriendsDetailsModelForMention.AccountFriendsPair.Count == 0
                    ? new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages", true)
                    : new SelectAccountDetailsControl(hiddenColumnList, model, true);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl,
                "LangKeySelectAccountDetails".FromResourceDictionary());

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    FacebookModel.AccountMentionPair.Clear();
                    FacebookModel.ListCustomMentionUser.Clear();

                    model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountMentionpair = model.AccountFriendsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            FacebookModel.AccountMentionPair.AddRange(accountMentionpair);
                            FacebookModel.ListCustomMentionUser.AddRange(accountMentionpair.Select(z => z.Value)
                                .ToList());
                            FacebookModel.ListCustomMentionUser =
                                FacebookModel.ListCustomMentionUser.Distinct().ToList();
                            FacebookModel.ListCustomMentionUser.ForEach(z =>
                            {
                                if (!FacebookModel.CustomMentionUser.Contains(z))
                                    FacebookModel.CustomMentionUser += z + "\r\n";
                            });
                        }
                        else if (model.AccountFriendsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "",
                                "LangKeyDestiationSelectedButAccountNot".FromResourceDictionary());
                        }
                    });


                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();

            FacebookModel.SelectFriendsDetailsModelForMention = selectAccountDetailsControl.GetSelectAccountModel();
        }

        private void SavePageExecute(object sender)
        {
            try
            {
                var pageUrlList = Regex.Split(FacebookModel.CustomPageUrl, "\r\n");
                pageUrlList.ForEach(x => { FacebookModel.ListCustomPageUrl.Add(x); });

                FacebookModel.ListCustomPageUrl = FacebookModel.ListCustomPageUrl.Distinct().ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SaveFriendExecute(object sender)
        {
            try
            {
                FacebookModel.ListCustomTaggedUser.Clear();
                var taggedUserList = Regex.Split(FacebookModel.CustomTaggedUser, "\r\n");
                taggedUserList.ForEach(x => { FacebookModel.ListCustomTaggedUser.Add(x); });

                FacebookModel.ListCustomTaggedUser = FacebookModel.ListCustomTaggedUser.Distinct().ToList();

                FacebookModel.AccountFriendsPair.RemoveAll(x =>
                    FacebookModel.ListCustomTaggedUser.FirstOrDefault(y => y == x.Value) == null);

                FacebookModel.SelectFriendsDetailsModel.AccountFriendsPair.RemoveAll(x =>
                    FacebookModel.ListCustomTaggedUser.FirstOrDefault(y => y == x.Value) == null);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectFriendsCommandExecute(object obj)
        {
            var model = FacebookModel.SelectFriendsDetailsModel;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Page, FbEntityTypes.Group, FbEntityTypes.CustomDestination
            };

            var selectAccountDetailsControl = FacebookModel.SelectFriendsDetailsModel.AccountFriendsPair.Count == 0
                ? new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages")
                : new SelectAccountDetailsControl(hiddenColumnList, model);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl,
                "LangKeySelectAccountDetails".FromResourceDictionary());

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    FacebookModel.AccountFriendsPair.Clear();
                    FacebookModel.ListCustomTaggedUser.Clear();

                    model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountFriendspair = model.AccountFriendsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            FacebookModel.AccountFriendsPair.AddRange(accountFriendspair);
                            FacebookModel.ListCustomTaggedUser.AddRange(
                                accountFriendspair.Select(z => z.Value).ToList());
                            FacebookModel.ListCustomTaggedUser = FacebookModel.ListCustomTaggedUser.Distinct().ToList();
                            FacebookModel.ListCustomTaggedUser.ForEach(z =>
                            {
                                if (!FacebookModel.CustomTaggedUser.Contains(z))
                                    FacebookModel.CustomTaggedUser += z + "\r\n";
                            });
                        }
                        else if (model.AccountFriendsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "",
                                "LangKeyDestiationSelectedButAccountNot".FromResourceDictionary());
                        }
                    });


                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();

            FacebookModel.SelectFriendsDetailsModel = selectAccountDetailsControl.GetSelectAccountModel();
        }


        private void SelectPagesCommandExecute(object obj)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var model = FacebookModel.SelectPageDetailsModel;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Friend, FbEntityTypes.Group, FbEntityTypes.CustomDestination
            };


            selectAccountDetailsControl = FacebookModel.SelectPageDetailsModel.AccountPagesBoardsPair.Count == 0
                ? new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, "Pages", true)
                : new SelectAccountDetailsControl(hiddenColumnList, model, true);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl,
                "LangKeySelectAccountDetails".FromResourceDictionary());

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    FacebookModel.AccountPagesBoardsPair.Clear();
                    FacebookModel.ListCustomPageUrl.Clear();

                    model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model.ListSelectDestination.ForEach(x =>
                    {
                        var accountPagespair = model.AccountPagesBoardsPair.Where(y => y.Key == x.AccountId).ToList();

                        if (x.IsAccountSelected)
                        {
                            FacebookModel.AccountPagesBoardsPair.AddRange(accountPagespair);
                            FacebookModel.ListCustomPageUrl.AddRange(accountPagespair.Select(z => z.Value).ToList());
                            FacebookModel.ListCustomPageUrl = FacebookModel.ListCustomPageUrl.Distinct().ToList();
                            FacebookModel.ListCustomPageUrl.ForEach(z =>
                            {
                                if (!FacebookModel.CustomPageUrl.Contains(z))
                                    FacebookModel.CustomPageUrl += z + "\r\n";
                            });
                        }
                        else if (model.AccountPagesBoardsPair.Any(y => y.Key == x.AccountId))
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, x.AccountName, "",
                                "LangKeyDestiationSelectedButAccountNot".FromResourceDictionary());
                        }
                    });


                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();

            FacebookModel.SelectPageDetailsModel = selectAccountDetailsControl.GetSelectAccountModel();
        }

        private bool SelectFriendsCanExecute(object arg)
        {
            return true;
        }

        private bool SelectMentionCanExecute(object arg)
        {
            return true;
        }

        private bool SelectPagesCanExecute(object arg)
        {
            return true;
        }

        private bool SavePagesCanExecute(object arg)
        {
            return true;
        }

        private bool SaveFriendCanExecute(object arg)
        {
            return true;
        }

        private bool SaveMentionCanExecute(object arg)
        {
            return true;
        }
    }
}