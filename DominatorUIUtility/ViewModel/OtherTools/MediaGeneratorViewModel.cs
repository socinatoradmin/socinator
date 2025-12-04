using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherTools
{
    public class MediaGeneratorViewModel : BaseTabViewModel, IOtherToolsViewModel
    {
        public MediaGeneratorViewModel() : base("LangKeyMediaGenerator", "MediaGeneratorControlTemplate")
        {
            BrowseCommand = new DelegateCommand(BrowseExecute);
            CopyCmd = new DelegateCommand<object>(Copy);
            LstFile = new ObservableCollection<string>();
        }

        public ICommand BrowseCommand { get; }
        public ICommand CopyCmd { get; }
        public ObservableCollection<string> LstFile { get; }

        private void Copy(object filePaths)
        {
            try
            {
                var filesPath = new StringBuilder();
                if (filePaths != null)
                {
                    var data = filePaths as IEnumerable<object>;
                    if (!data.Any())
                    {
                        ToasterNotification.ShowWarning("LangKeySelectAtLeastOnePathToCopy".FromResourceDictionary());
                        return;
                    }

                    data.ForEach(x =>
                    {
                        filesPath.Append(x.ToString());
                        filesPath.AppendLine();
                    });
                    filesPath.Remove(filesPath.Length - 1, 1);
                    Clipboard.SetData(DataFormats.Text, filesPath.ToString());
                    ToasterNotification.ShowSuccess("LangKeyFilesPathCopied".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BrowseExecute()
        {
            try
            {
                var filters =
                    "Image Files |*.jpg;*.jpeg;*.png;*.gif|Videos Files |*.dat; *.wmv; *.3g2; *.3gp;*.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                    " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm|All file |*.*";
                var picPath = FileUtilities.GetImageOrVideo(true, filters);
                if (picPath != null)
                    foreach (var pic in picPath)
                        if (LstFile.All(x => x != pic))
                            LstFile.Add(pic);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}