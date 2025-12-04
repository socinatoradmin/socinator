using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace GramDominatorUI.GDViews.InstaLikeComment
{
    public class MediaUnlikerBase : ModuleSettingsUserControl<MediaUnlikerViewModel, MediaUnlikerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.PostFilterModel.PostCategory.FilterPostCategory)
                if (Model.PostFilterModel.PostCategory.IgnorePostImages &&
                    Model.PostFilterModel.PostCategory.IgnorePostVideos &&
                    Model.PostFilterModel.PostCategory.IgnorePostAlbums)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please check maximum two options for Post Type filteration inside Post Filter category.");
                    return false;
                }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MediaUnlikerModel =
                        JsonConvert.DeserializeObject<MediaUnlikerModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new MediaUnlikerViewModel();
                ObjViewModel.MediaUnlikerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for MediaUnliker.xaml
    /// </summary>
    public partial class MediaUnliker : MediaUnlikerBase
    {
        private MediaUnliker()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: MediaUnlikerHeader,
                footer: MediaUnlikerFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unlike,
                moduleName: Enums.GdMainModule.LikeComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.MediaUnlikerVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.MediaUnlikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static MediaUnliker CurrentMediaUnliker { get; set; }

        /// <summary>
        ///     GetSingeltonObjectMediaUnliker is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise will return a new created object.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static MediaUnliker GetSingeltonObjectMediaUnliker()
        {
            return CurrentMediaUnliker ?? (CurrentMediaUnliker = new MediaUnliker());
        }
    }
}