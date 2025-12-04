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

namespace GramDominatorUI.GDViews.Instachats
{
    public class
        SendMessageToFollowerBase : ModuleSettingsUserControl<SendMessageToFollowerViewModel, SendMessageToFollowerModel
        >
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
    }

    /// <summary>
    ///     Interaction logic for SendMessageToFollower.xaml
    /// </summary>
    public partial class SendMessageToFollower : SendMessageToFollowerBase
    {
        private static SendMessageToFollower ObjSendMessageToFollower;

        public SendMessageToFollower()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendMessageToFollowFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToFollower,
                moduleName: Enums.GdMainModule.InstaChat.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToFollowerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToFollowerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static SendMessageToFollower GetSingeltonSendMessageToFollower()
        {
            return ObjSendMessageToFollower ?? (ObjSendMessageToFollower = new SendMessageToFollower());
        }
        private bool IsEnableMultiSelect => ObjViewModel != null ?
            ObjViewModel.ShowMediaControl==Visibility.Visible : false;
        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                //opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                opf.Multiselect = IsEnableMultiSelect;
                if (opf.ShowDialog().Value)
                {
                    ObjViewModel.Model.MediaPath = opf.FileName;
                    ObjViewModel.Medias.Clear();
                    if (IsEnableMultiSelect)
                    {
                        foreach(var item in opf.FileNames)
                        {
                            ObjViewModel.Medias.Add(new MessageMediaInfo(item));
                        }
                    }else
                        ObjViewModel.Medias.Add(new MessageMediaInfo(ObjViewModel.Model.MediaPath));
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

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Video Files |*.mp4;";
                opf.Multiselect = IsEnableMultiSelect;
                if (opf.ShowDialog().Value)
                {
                    ObjViewModel.Model.MediaPath = opf.FileName;
                    ObjViewModel.Medias.Clear();
                    if (IsEnableMultiSelect)
                    {
                        foreach (var item in opf.FileNames)
                        {
                            ObjViewModel.Medias.Add(new MessageMediaInfo(item));
                        }
                    }
                    else
                        ObjViewModel.Medias.Add(new MessageMediaInfo(ObjViewModel.Model.MediaPath));
                    ObjViewModel.Model.Medias = ObjViewModel.Medias;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}