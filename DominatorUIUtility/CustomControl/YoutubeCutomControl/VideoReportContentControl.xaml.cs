using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl.YoutubeCutomControl
{
    public partial class VideoReportContentControl
    {
        private static readonly RoutedEvent AddCommentToListEvent =
            EventManager.RegisterRoutedEvent("AddCommentToListChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(VideoReportContentControl));

        // Using a DependencyProperty as the backing store for ReportDetails.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportDetailsProperty =
            DependencyProperty.Register("ReportDetails", typeof(ManageReportVideosContentModel),
                typeof(VideoReportContentControl), new PropertyMetadata());

        // Using a DependencyProperty as the backing store for ListReportDetailsModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListReportDetailsModelProperty =
            DependencyProperty.Register("ListReportDetailsModel",
                typeof(ObservableCollection<ManageReportVideosContentModel>), typeof(VideoReportContentControl),
                new PropertyMetadata(new ObservableCollection<ManageReportVideosContentModel>()));

        public static readonly DependencyProperty SplitTextByNextLineProperty =
            DependencyProperty.Register("SplitTextByNextLine", typeof(bool), typeof(VideoReportContentControl),
                new PropertyMetadata(new bool()));

        // Using a DependencyProperty as the backing store for AddReportDetailsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddReportDetailsCommandProperty =
            DependencyProperty.Register("AddReportDetailsCommand", typeof(ICommand), typeof(VideoReportContentControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(VideoReportContentControl),
                new PropertyMetadata());

        private bool _isUncheckfromList;

        public VideoReportContentControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            AddReportDetailsCommand = new BaseCommand<object>(sender => true, AddCommentsExecute);
            //CmbReportOption.ItemsSource = new List<string>
            //{
            //    "Sexual content", "Violent or repulsive content", "Hateful or abusive content","Harassment or bullying",
            //    "Harmful dangerous acts","Misinformation","Child abuse", "Promotes terrorism", "Spam or misleading",
            //    "Infringes my rights","Captions issue"
            //};
            CmbReportOption.ItemsSource = new List<string>
            {
                "Sexual content", "Violent or repulsive content", "Hateful or abusive content","Harassment or bullying",
                "Harmful or dangerous acts","Suicide, self-harm, or eating disorders","Misinformation", "Child abuse", "Promotes terrorism",
                "Spam or misleading"
            };
        }

        public VideoReportContentControl(int optionIndex, int subOptionIndex) : this()
        {
            CmbReportOption.SelectedIndex = optionIndex;
            CmbReportSubOption.SelectedIndex = subOptionIndex;
            SplitByNextLine.Visibility = Visibility.Collapsed;
        }


        public ManageReportVideosContentModel ReportDetails
        {
            get => (ManageReportVideosContentModel)GetValue(ReportDetailsProperty);
            set => SetValue(ReportDetailsProperty, value);
        }

        public ObservableCollection<ManageReportVideosContentModel> ListReportDetailsModel
        {
            get => (ObservableCollection<ManageReportVideosContentModel>)GetValue(ListReportDetailsModelProperty);
            set => SetValue(ListReportDetailsModelProperty, value);
        }

        public bool SplitTextByNextLine
        {
            get => (bool)GetValue(SplitTextByNextLineProperty);
            set => SetValue(SplitTextByNextLineProperty, value);
        }

        public bool Isupdated { get; set; }

        public ICommand AddReportDetailsCommand
        {
            get => (ICommand)GetValue(AddReportDetailsCommandProperty);
            set => SetValue(AddReportDetailsCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler AddCommentToListChanged
        {
            add => AddHandler(AddCommentToListEvent, value);
            remove => RemoveHandler(AddCommentToListEvent, value);
        }

        private void AddCommentToListEventHandler()
        {
            var routedEventArgs = new RoutedEventArgs(AddCommentToListEvent);
            RaiseEvent(routedEventArgs);
        }


        private void chkQuery_Checked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, true);
        }

        private void chkQuery_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(sender, false);
        }

        private void CheckUncheckAll(object sender, bool IsChecked)
        {
            try
            {
                var currentQuery = ((QueryContent)(sender as CheckBox).DataContext).Content.QueryValue;
                if (!ReportDetails.LstQueries.Skip(1).All(x => x.IsContentSelected))
                    if (!IsChecked)
                    {
                        _isUncheckfromList = true;
                        ReportDetails.LstQueries[0].IsContentSelected = false;
                    }

                if (ReportDetails.LstQueries.Skip(1).All(x => x.IsContentSelected))
                {
                    _isUncheckfromList = false;
                    ReportDetails.LstQueries[0].IsContentSelected = IsChecked;
                }

                if (_isUncheckfromList)
                {
                    _isUncheckfromList = false;
                    return;
                }

                if (currentQuery == "All" || currentQuery == "Default")
                {
                    _isUncheckfromList = false;
                    ReportDetails.LstQueries.ToList().Select(query =>
                    {
                        query.IsContentSelected = IsChecked;
                        return query;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddCheckedQueryToList()
        {
            ReportDetails.SelectedQuery.Clear();
            ReportDetails.LstQueries.ToList().ForEach(query =>
            {
                if (query.IsContentSelected)
                    ReportDetails.SelectedQuery.Add(query);
            });
        }

        private void AddCommentsExecute(object sender)
        {
            //if (string.IsNullOrEmpty(Comments.CommentText))
            //{
            //    Dialog.ShowDialog("Warning", "Please type some comment !!");
            //    return;LangKeyWarning
            //}
            if (!ReportDetails.LstQueries.Any(x => x.IsContentSelected))
            {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(), "Please select atleast one query.");
                return;
            }

            AddCheckedQueryToList();
            if (btnAddCommentToList.Content.ToString() == "LangKeyUpdate".FromResourceDictionary())
            {
                ListReportDetailsModel.Select(x =>
                {
                    if (x.CommentId == ReportDetails.CommentId)
                    {
                        x.CommentText = ReportDetails.CommentText;
                        x.LstQueries = ReportDetails.LstQueries;
                        x.SelectedQuery = ReportDetails.SelectedQuery;
                        x.ReportOption = CmbReportOption.SelectedIndex;
                        x.ReportSubOption = CmbReportSubOption.SelectedIndex;
                        x.ReportText = CmbReportOption.SelectedItem as string;
                    }

                    return x;
                }).ToList();
                ReportDetails.SelectedQuery.Remove(
                    ReportDetails.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
                ReportDetails.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();
                ReportDetails.ReportOption = CmbReportOption.SelectedIndex;
                ReportDetails.ReportText = CmbReportOption.SelectedItem as string;
                ReportDetails.ReportSubOption = CmbReportSubOption.SelectedIndex;
                Isupdated = true;
                Dialog.CloseDialog(this);
            }
            else
            {
                AddCommentToListEventHandler();
            }
        }

        private void CmbReportOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var getList = SetSubOptions((sender as ComboBox).SelectedIndex);
                CmbReportSubOption.ItemsSource = getList;
                if (getList != null)
                    CmbReportSubOption.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private List<string> SetSubOptions(int optionIndex)
        {
            //CmbReportSubOption.Visibility = Visibility.Visible;
            var list = new List<string>();
            switch (optionIndex)
            {
                //Sexual Content Sub-options
                case 0:
                    list = new List<string>
                    {
                        "Graphic sexual activity", "Nudity", "Suggestive, but without nudity",
                        "Content involving minors", "Abusive title or description", "Other sexual content"
                    };
                    break;
                // Violence and Repulsive Content Sub-options
                case 1:
                    list = new List<string> { "Adults fighting", "Physical attack", "Youth violence", "Animal abuse" };
                    break;
                // Hateful or Abusive Content Sub-options
                case 2:
                    list = new List<string>
                    {
                        "Promotes hatred or violence", "Abusing vulnerable individuals",
                        "Abusive title or description"
                    };
                    break;
                //Harassment or Bullying sub-options
                case 3:
                    list = new List<string>
                    {
                        "This is harassing me", "This is harassing someone else"
                    };
                    break;
                // Harmful or dangerous acts sub-options
                case 4:
                    list = new List<string>
                    {
                        "Pharmaceutical or drug abuse", "Abuse of fire or explosives", "Suicide or self injury",
                        "Other dangerous acts"
                    };
                    break;
                // Misinformation sub-options
                case 5:
                // Child Abuse sub-options
                case 6:
                // Promotes terrorism sub-options
                case 7:
                    CmbReportSubOption.Visibility = Visibility.Hidden;
                    list?.Clear();
                    return null;
                // Spam or misleading sub-options
                case 8:
                    list = new List<string>
                    {
                        "Mass advertising", "Pharmaceutical drugs for sale", "Misleading text", "Misleading thumbnail",
                        "Scams or fraud"
                    };
                    break;
                // Infringes my rights sub-options
                case 9:
                    list = new List<string>
                    {
                        "Copyright issue", "Privacy issue", "Trademark infringement", "Defamation", "Counterfeit",
                        "Other legal issue"
                    };
                    break;
                // Captions issue sub-options
                case 10:
                    list = new List<string>
                        {"Captions are missing (CVAA)", "Captions are inaccurate", "Captions are abusive"};
                    break;
            }

            return list;
        }
    }
}