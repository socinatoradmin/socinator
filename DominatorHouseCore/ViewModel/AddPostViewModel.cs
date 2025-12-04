#region

using System;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class AddPostViewModel : BindableBase
    {
        public AddPostViewModel()
        {
            Enum.GetNames(typeof(DayOfWeek)).ToList().ForEach(day =>
            {
                AddPostModel.JobConfigurations.Weekday.Add(new ContentSelectGroup
                {
                    Content = day
                });
            });
            AddPostModel.OtherConfiguration.MakeImagesUniqueStatus.Add("Medium");
        }

        private AddPostModel _addPostModel = new AddPostModel();

        public AddPostModel AddPostModel
        {
            get => _addPostModel;
            set
            {
                if ((_addPostModel == null) & (_addPostModel == value))
                    return;
                SetProperty(ref _addPostModel, value);
            }
        }
    }
}