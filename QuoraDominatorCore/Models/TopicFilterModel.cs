using DominatorHouseCore.Utility;
using System.Windows;

namespace QuoraDominatorCore.Models
{
    public class TopicFilterModel:BindableBase
    {
        private bool _filtertopic;
        private bool _filteranswers;
        private bool _filterquestions;
        private bool _saveCloseButtonVisible;
        private bool _IsCheckedTopicFilter;
        private Visibility _IsVisibleAnswerFilter;
        private RangeUtilities _topicsrange=new RangeUtilities(0,1000);
        private RangeUtilities _questionsrange=new RangeUtilities(0,1000);
        private RangeUtilities _answersrange=new RangeUtilities(0,1000);
        public bool SaveCloseButtonVisible
        {
            get => _saveCloseButtonVisible;
            set
            {
                if (value == _saveCloseButtonVisible) return;
                SetProperty(ref _saveCloseButtonVisible, value);
            }
        }
        public Visibility IsVisibleAnswerFilter
        {
            get => _IsVisibleAnswerFilter;
            set
            {
                if (value == _IsVisibleAnswerFilter) return;
                SetProperty(ref _IsVisibleAnswerFilter, value);
            }
        }
        public bool IsCheckedTopicFilter
        {
            get => _IsCheckedTopicFilter;
            set
            {
                if (value == _IsCheckedTopicFilter) return;
                SetProperty(ref _IsCheckedTopicFilter, value);
            }
        }
        public bool FilterTopicsCount
        {
            get => _filtertopic;
            set
            {
                if (value == _filtertopic) return;
                SetProperty(ref _filtertopic, value);
            }
        }
        public bool FilterQuestions
        {
            get => _filterquestions;
            set
            {
                if (value == _filterquestions) return;
                SetProperty(ref _filterquestions, value);
            }
        }
        public bool FilterAnswers
        {
            get => _filteranswers;
            set
            {
                if (value == _filteranswers) return;
                SetProperty(ref _filteranswers, value);
            }
        }
        public RangeUtilities TopicsCount
        {
            get => _topicsrange;
            set
            {
                if (value == _topicsrange) return;
                SetProperty(ref _topicsrange, value);
            }
        }
        public RangeUtilities QuestionsCount
        {
            get => _questionsrange;
            set
            {
                if (value == _questionsrange) return;
                SetProperty(ref _questionsrange, value);
            }
        }
        public RangeUtilities AnswersCount
        {
            get => _answersrange;
            set
            {
                if (value == _answersrange) return;
                SetProperty(ref _answersrange, value);
            }
        }
    }
}
