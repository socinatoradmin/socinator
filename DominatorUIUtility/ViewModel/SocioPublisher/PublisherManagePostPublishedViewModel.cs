using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using EmbeddedBrowser;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostPublishedViewModel : PublisherPostlistBaseViewModel
    {
        public PublisherManagePostPublishedViewModel()
        {
            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);
            PublishedDetailsCommand = new BaseCommand<object>(PublishedDetailsCanExecute, PublishedDetailsExecute);
            ReportCommand = new BaseCommand<object>(ReportCanExecute, ReportExecute);
        }


        #region Properties

        public ICommand ExportCommand { get; set; }
        public ICommand PublishedDetailsCommand { get; set; }
        public ICommand ReportCommand { get; set; }

        #endregion

        #region Export

        private bool ExportCanExecute(object sender)
        {
            return true;
        }

        private void ExportExecute(object sender)
        {
            // Get the selected posts from published section of the campaign
            var selectedPost = PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

            if (selectedPost.Count != 0)
            {
                // Get the export path of the post
                var exportPath = FileUtilities.GetExportPath();
                var filename = $"{exportPath}\\Export-{ConstantVariable.GetDateTime()}.csv";
                var header =
                    "Account Name,Campaign Name,Destination,Destination Url,Description,Published,Successful,Published Date,Link";

                FileUtilities.AddHeaderToCsv(filename, header);

                foreach (var post in selectedPost) ExportPosts(post, filename);

                #region Commenting Sections

                //if (!string.IsNullOrEmpty(exportPath))
                //{
                //    //var header =
                //    //    "Post Description,MediaList,ShareUrl,ExpiredTime,Published,Running Status";


                //    //  FileUtilities.AddHeaderToCsv(filename, header);
                //    try
                //    {
                //        TextWriter writer = new StreamWriter(filename);

                //        var csv = new CsvWriter(writer);

                //        #region Write Csv Header

                //        csv.WriteField("Post Description");
                //        csv.WriteField("MediaList");
                //        csv.WriteField("ShareUrl");
                //        csv.WriteField("ExpiredTime");
                //        csv.WriteField("Published");
                //        csv.WriteField("Running Status");

                //        #endregion

                //        csv.NextRecord();

                //        selectedPost.ForEach(post =>
                //        {
                //            var mediaUrls = string.Empty;
                //            post.MediaList.ForEach(x => { mediaUrls += x + ConstantVariable.Separator; });

                //            var updateMediaUrl = string.Empty;
                //            if (!string.IsNullOrEmpty(mediaUrls))
                //            {
                //                updateMediaUrl = mediaUrls.Substring(0,
                //                    mediaUrls.LastIndexOf(ConstantVariable.Separator, StringComparison.Ordinal));
                //            }
                //            #region Write Csv Record

                //            csv.WriteField(Utilities.ReplaceUniCode(post.PostDescription));
                //            csv.WriteField(updateMediaUrl);
                //            csv.WriteField(post.ShareUrl);
                //            csv.WriteField(post.ExpiredTime);
                //            csv.WriteField($"\"{post.PublishedTriedAndSuccessStatus}\"");
                //            csv.WriteField(post.PostRunningStatus);

                //            #endregion
                //            csv.NextRecord();
                //        });
                //        writer.Close();
                //        csv.Flush();
                //    }
                //    catch (Exception ex)
                //    {
                //        ex.DebugLog();
                //    }
                //}
                //else
                //{
                //    Dialog.ShowDialog("Warning", "Please select path to export.");
                //} 

                #endregion
            }
            else
            {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    "LangKeySelectPostToExport".FromResourceDictionary());
            }
        }

        #endregion


        #region  PublishedDetails

        private bool PublishedDetailsCanExecute(object sender)
        {
            return true;
        }

        private void PublishedDetailsExecute(object sender)
        {
            try
            {
                var currentData = sender as PublisherPostlistModel;

                if (currentData?.LstPublishedPostDetailsModels.Count != 0)
                {
                    var dialog = new Dialog();
                    var publishedPostDetails = new PublishedPostDetails(currentData);
                    var window = dialog.GetMetroWindow(publishedPostDetails,
                        "LangKeyPublishedDetails".FromResourceDictionary());
                    window.ShowDialog();
                }
                else
                {
                    Dialog.ShowDialog("LangKeyPublishedDetails".FromResourceDictionary(),
                        "LangKeyNoDetailsAvailable".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Report

        private bool ReportCanExecute(object sender)
        {
            return true;
        }

        private void ReportExecute(object sender)
        {
            try
            {
                var currentData = sender as PublisherPostlistModel;
                if (currentData?.LstPublishedPostDetailsModels.Count != 0)
                {
                    var exportPath = FileUtilities.GetExportPath();

                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        var header =
                            "Account Name,Campaign Name,Destination,Destination Url,Description,Published,Successful,Published Date,Link";

                        var filename = $"{exportPath}\\{currentData?.PostId}.csv";

                        FileUtilities.AddHeaderToCsv(filename, header);

                        currentData?.LstPublishedPostDetailsModels.ForEach(post =>
                        {
                            var newpostDescription = "\"" + post.Description.Replace("\"", "\"\"") + "\"";


                            var csvData = post.AccountName + "," + post.CampaignName + "," + post.Destination + "," +
                                          post.DestinationUrl + "," +
                                          newpostDescription + ","
                                          + post.IsPublished + "," + post.Successful + "," +
                                          post.PublishedDate.ToString(CultureInfo.InvariantCulture) + "," +
                                          post.Link;

                            //var csvData = post.AccountName + "," + post.CampaignName + "," + post.Destination + "," + post.DestinationUrl + "," +
                            //              post.Description.Replace("\r\n", "<n>") + ","
                            //              + post.IsPublished + "," + post.Successful + "," + post.PublishedDate.ToString(CultureInfo.InvariantCulture) + "," +
                            //              post.Link;
                            using (var streamWriter = new StreamWriter(filename, true))
                            {
                                streamWriter.WriteLine(csvData);
                            }
                        });
                    }
                    else
                    {
                        Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                            "LangKeySelectPathToExport".FromResourceDictionary());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void ExportPosts(PublisherPostlistModel currentData, string filename)
        {
            try
            {
                if (currentData?.LstPublishedPostDetailsModels.Count != 0)
                    currentData?.LstPublishedPostDetailsModels.ForEach(post =>
                    {
                        var newpostDescription = "\"" + post.Description.Replace("\"", "\"\"") + "\"";

                        var csvData = post.AccountName + "," + post.CampaignName + "," + post.Destination + "," +
                                      post.DestinationUrl + "," +
                                      newpostDescription + ","
                                      + post.IsPublished + "," + post.Successful + "," +
                                      post.PublishedDate.ToString(CultureInfo.InvariantCulture) + "," +
                                      post.Link;
                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                        }
                    });
                else
                    Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeySelectPathToExport".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }

    public class PublishedPostDetailsViewModel : BindableBase
    {
        public PublishedPostDetailsViewModel()
        {
            ViewInBrowserCommand = new BaseCommand<object>(ViewInBrowserCanExecute, ViewInBrowserExecute);
        }


        #region Command

        public ICommand ViewInBrowserCommand { get; set; }

        #endregion

        #region Properties

        private PublisherPostlistModel _publisherPostlist = new PublisherPostlistModel();

        public PublisherPostlistModel PublisherPostlist
        {
            get => _publisherPostlist;
            set
            {
                if (value == _publisherPostlist)
                    return;
                SetProperty(ref _publisherPostlist, value);
            }
        }

        #endregion

        #region Methods

        private async void ViewInBrowserExecute(object sender)
        {
            var currentPost = (PublishedPostDetailsModel) sender;
            var dominatorAccountModel = InstanceProvider.GetInstance<IAccountCollectionViewModel>().GetCopySync()
                .FirstOrDefault(x => x.AccountId == currentPost.AccountId);
            if(dominatorAccountModel != null && !string.IsNullOrEmpty(currentPost.Link))
            {
                if(dominatorAccountModel.AccountBaseModel.AccountNetwork == DominatorHouseCore.Enums.SocialNetworks.Instagram && ConstantVariable.UsePuppeteer)
                {
                    var browserWindow = new PuppeteerBrowserActivity(dominatorAccountModel, targetUrl: currentPost.Link);
                    await browserWindow.LaunchBrowserAsync();
                }
                else
                {
                    var browserWindow = new BrowserWindow(dominatorAccountModel, currentPost.Link, true);
                    if (dominatorAccountModel.BrowserCookies?.Count > 0)
                        await browserWindow.BrowserSetCookie();
                    else
                        await browserWindow.SetCookie();
                    browserWindow.Show();
                }
            }
        }

        private bool ViewInBrowserCanExecute(object sender)
        {
            return true;
        }

        #endregion
    }
}