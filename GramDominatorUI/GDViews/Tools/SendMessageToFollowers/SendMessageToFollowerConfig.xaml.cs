using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Instachat;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace GramDominatorUI.GDViews.Tools.SendMessageToFollowers
{
    public class
        SendMessageToFollowerConfigBase : ModuleSettingsUserControl<SendMessageToFollowerViewModel,
            SendMessageToFollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check message(s) has provided or not
            if (string.IsNullOrEmpty(Model.TextMessage))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add atleast one message and then click on save button to save the message");
                return false;
            }

            if (ObjViewModel.SendMessageToFollowerModel.ManageMessagesModel.LstQueries.Count > 0)
                foreach (var queryContent in Model.ManageMessagesModel.LstQueries)
                    if (queryContent.Content.QueryValue != "All")
                    {
                        ManageMessagesModel mangeMessagesModel = null;
                        try
                        {
                            mangeMessagesModel = ObjViewModel.SendMessageToFollowerModel.LstDisplayManageMessageModel
                                .Where(x =>
                                    x.SelectedQuery.Any(y =>
                                        y.Content.QueryType == queryContent.Content.QueryType &&
                                        y.Content.QueryValue == queryContent.Content.QueryValue)).ToList()
                                .GetRandomItem();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (mangeMessagesModel == null)
                        {
                            Dialog.ShowDialog(this, "Input Error",
                                $"please input at least one message for Querytype [ {queryContent.Content.QueryType} ] with Queryvalue [ {queryContent.Content.QueryValue} ]");
                            return false;
                        }
                    }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendMessageToFollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<SendMessageToFollowerModel>(ObjViewModel.Model,
                            true);
                else
                    ObjViewModel = new SendMessageToFollowerViewModel();
                ObjViewModel.SendMessageToFollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToFollowerConfig.xaml
    /// </summary>
    public partial class SendMessageToFollowerConfig : SendMessageToFollowerConfigBase
    {
        public SendMessageToFollowerConfig()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendMessageToFollower,
                Enums.GdMainModule.InstaChat.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.SendMessageToFollowerVideoTutorialsLink;
        }

        private static SendMessageToFollowerConfig CurrentSendMessageToFollowerConfig { get; set; }

        public static SendMessageToFollowerConfig GetSingeltonObjectSendMessageToFollowerConfig()
        {
            return CurrentSendMessageToFollowerConfig ??
                   (CurrentSendMessageToFollowerConfig = new SendMessageToFollowerConfig());
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                opf.Multiselect = true;
                if (opf.ShowDialog().Value)
                {
                    ObjViewModel.Model.MediaPath = opf.FileName;
                    foreach(var item in opf.FileNames)
                    {
                        ObjViewModel.Medias.Add(new MessageMediaInfo(item));
                    }
                    ObjViewModel.Model.Medias = ObjViewModel.Medias;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Video Files |*.mp4;";
                opf.Multiselect = true;
                if (opf.ShowDialog().Value)
                {
                    ObjViewModel.Model.MediaPath = opf.FileName;
                    foreach (var item in opf.FileNames)
                    {
                        ObjViewModel.Medias.Add(new MessageMediaInfo(item));
                    }
                    ObjViewModel.Model.Medias = ObjViewModel.Medias;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.Model.MediaPath = "";
        }
    }
}