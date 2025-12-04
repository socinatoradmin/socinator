using DominatorHouseCore.Models;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.GDModel
{
    public class IgPublisherModel
    {
        public bool isVideo { get; set; }
        public bool isPostAsStory { get; set; }
        public bool isPostAsReels { get; set; }
        public bool isLocationName { get; set; }
        public string geoLocation { get; set; }
        public string thumbnailFilePath { get; set; }
        public string convertedMediaFilePath { get; set; }

        public List<string> ImageAlbumList = new List<string>();

        public List<string>videoAlbumList=new List<string>();
        public string tagLocationDetails { get; set; }

        public  List<InstagramUser> lstTagUserIds { get; set; } =new List<InstagramUser>();

        public List<string> lstThumbnailVideo = new List<string>();

        public ObservableCollection<string> CheckedImagePath =new ObservableCollection<string>();

        public List<string> checkedVideoPath=new List<string>();

        public List<string>lstVideoThumbnail=new List<string>();
        public bool isAlbum { get; set; }
        public bool IsImageAlbum { get; set; }
        public string mediaPath { get; set; }

        public string fullCaption { get; set; }
    }
}
