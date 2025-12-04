using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    ///     Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///     Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///     Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl;assembly=DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///     You will also need to add a project reference from the project where the XAML file lives
    ///     to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:SocinatorTextBox />
    /// </summary>
    [TemplatePart(Name = TextEditorPart, Type = typeof(TextBox))]
    [TemplatePart(Name = SuggestionPopUp, Type = typeof(Popup))]
    [TemplatePart(Name = SelectionListBox, Type = typeof(ListBox))]
    public class SocinatorTextBox : Control
    {
        static SocinatorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SocinatorTextBox),
                new FrameworkPropertyMetadata(typeof(SocinatorTextBox)));
        }

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));

            var postDescriptionText = Template.FindName(TextEditorPart, this) as TextBox;

            if (!_postDescription.Equals(postDescriptionText))
            {
                if (postDescriptionText != null)
                {
                    postDescriptionText.PreviewKeyDown -= OnEditorKeyDown;
                    postDescriptionText.LostFocus -= OnEditorLostFocus;
                    postDescriptionText.TextChanged -= OnEditorTextChanged;
                }

                _postDescription = postDescriptionText;

                if (postDescriptionText != null)
                {
                    postDescriptionText.LostFocus += OnEditorLostFocus;
                    postDescriptionText.TextChanged += OnEditorTextChanged;
                    postDescriptionText.PreviewKeyDown += OnEditorKeyDown;
                }
            }

            GotFocus += SocinatorTextBox_GotFocus;

            var popUpControl = Template.FindName(SuggestionPopUp, this) as Popup;

            if (!_postUp.Equals(popUpControl))
            {
                if (popUpControl != null)
                {
                    popUpControl.Opened -= OnPopupOpened;
                    popUpControl.Closed -= OnPopupClosed;
                }

                _postUp = popUpControl;

                if (popUpControl != null)
                {
                    popUpControl.StaysOpen = false;
                    popUpControl.Opened += OnPopupOpened;
                    popUpControl.Closed += OnPopupClosed;
                }
            }

            ItemsSelector = Template.FindName(SelectionListBox, this) as Selector;

            if (ItemsSelector != null)
            {
                SelectionAdapter = new SelectionAdapter(ItemsSelector);
                SelectionAdapter.Commit += OnSelectionAdapterCommit;
                SelectionAdapter.Cancel += OnSelectionAdapterCancel;
                SelectionAdapter.SelectionChanged += OnSelectionAdapterSelectionChanged;
                ItemsSelector.PreviewMouseDown += ItemsSelector_PreviewMouseDown;
            }
        }

        #endregion

        #region SuggestionsAdapter

        private class SuggestionsAdapter
        {
            #region Constructors

            public SuggestionsAdapter(SocinatorTextBox socinatorText)
            {
                _socinatorText = socinatorText;
            }

            #endregion

            #region Fields

            private readonly SocinatorTextBox _socinatorText;
            private string _filter;

            #endregion

            #region Methods

            public void GetSuggestions(string searchText)
            {
                try
                {
                    _filter = searchText;
                    _socinatorText.IsLoading = true;
                    var suggestionParameterizedThread = new ParameterizedThreadStart(GetSuggestionsAsync);
                    var suggestionThread = new Thread(suggestionParameterizedThread);
                    suggestionThread.Start(new object[] {searchText, _socinatorText.SuggestionProvider});
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            private void DisplaySuggestions(IEnumerable suggestions, string filter)
            {
                try
                {
                    if (_filter != filter)
                        return;
                    if (!_socinatorText.IsDropDownOpen)
                        return;
                    _socinatorText.IsLoading = false;
                    _socinatorText.ItemsSelector.ItemsSource = suggestions;
                    _socinatorText.IsDropDownOpen = _socinatorText.ItemsSelector.HasItems;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            private void GetSuggestionsAsync(object param)
            {
                var args = param as object[];
                var searchText = Convert.ToString(args?[0]);
                var provider = args?[1] as ISuggestionProvider;
                var list = provider?.GetSuggestions(searchText);
                _socinatorText.Dispatcher.BeginInvoke(new Action<IEnumerable, string>(DisplaySuggestions),
                    DispatcherPriority.Background,
                    list,
                    searchText);
            }

            #endregion
        }

        #endregion

        #region Properties

        public const string TextEditorPart = "PART_Editor";
        public const string SuggestionPopUp = "PART_Popup";
        public const string SelectionListBox = "PART_Selector";

        public string Watermark
        {
            get => (string) GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(SocinatorTextBox),
                new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SocinatorTextBox),
                new PropertyMetadata(string.Empty));

        public bool IsDropDownOpen
        {
            get => (bool) GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SocinatorTextBox),
                new PropertyMetadata(false));


        public DataTemplate MacroTemplate
        {
            get => (DataTemplate) GetValue(MacroTemplateProperty);
            set => SetValue(MacroTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for MacroTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MacroTemplateProperty =
            DependencyProperty.Register("MacroTemplate", typeof(DataTemplate), typeof(SocinatorTextBox),
                new PropertyMetadata(null));


        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(SocinatorTextBox));


        public bool IsLoading
        {
            get => (bool) GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(SocinatorTextBox),
                new PropertyMetadata(false));


        public ISuggestionProvider SuggestionProvider
        {
            get => (ISuggestionProvider) GetValue(SuggestionProviderProperty);
            set => SetValue(SuggestionProviderProperty, value);
        }

        // Using a DependencyProperty as the backing store for MacrosSuggestionProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuggestionProviderProperty =
            DependencyProperty.Register("SuggestionProvider", typeof(ISuggestionProvider), typeof(SocinatorTextBox),
                new FrameworkPropertyMetadata(null));


        public object LoadingContent
        {
            get => GetValue(LoadingContentProperty);
            set => SetValue(LoadingContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for LoadingContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingContentProperty =
            DependencyProperty.Register("LoadingContent", typeof(object), typeof(SocinatorTextBox),
                new PropertyMetadata(null));


        public string DisplayMember
        {
            get => (string) GetValue(DisplayMemberProperty);
            set => SetValue(DisplayMemberProperty, value);
        }

        // Using a DependencyProperty as the backing store for DisplayMember.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register("DisplayMember", typeof(string), typeof(SocinatorTextBox),
                new PropertyMetadata(string.Empty));


        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SocinatorTextBox),
                new FrameworkPropertyMetadata(null, OnSelectedItemChanged));


        public int Delay
        {
            get => (int) GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(SocinatorTextBox), new PropertyMetadata(200));


        public TextWrapping TextWrapping
        {
            get => (TextWrapping) GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(SocinatorTextBox),
                new PropertyMetadata(TextWrapping.NoWrap));

        public double TextWidth
        {
            get => (double) GetValue(TextWidthProperty);
            set => SetValue(TextWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(SocinatorTextBox),
                new PropertyMetadata(double.NaN));

        public double TextHeight
        {
            get => (double) GetValue(TextHeightProperty);
            set => SetValue(TextHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextHeightProperty =
            DependencyProperty.Register("TextHeight", typeof(double), typeof(SocinatorTextBox),
                new PropertyMetadata(double.NaN));

        public BindingEvaluator BindingEvaluator { get; set; }

        public DispatcherTimer FetchTimer { get; set; }

        public string Filter { get; set; }

        public SelectionAdapter SelectionAdapter { get; set; }

        private TextBox _postDescription = new TextBox();

        private Popup _postUp = new Popup();

        public Selector ItemsSelector { get; set; }

        private bool _isUpdatingText;

        private bool _selectionCancelled;

        private SuggestionsAdapter _suggestionsAdapter { get; set; }

        #endregion


        #region Events

        #region Pop Open and Close

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (!_selectionCancelled)
                OnSelectionAdapterCommit();
        }

        private void OnSelectionAdapterCommit()
        {
            if (ItemsSelector.SelectedItem != null)
            {
                SelectedItem = ItemsSelector.SelectedItem;
                _isUpdatingText = true;
                _postDescription.Text = _postDescription.Text.ApplyMacros(_postDescription.CaretIndex,
                    GetDisplayText(ItemsSelector.SelectedItem));
                SetSelectedItem(ItemsSelector.SelectedItem);
                _isUpdatingText = false;
                IsDropDownOpen = false;
                _postDescription.SelectionStart = _postDescription.Text.Length;
                _postDescription.SelectionLength = 0;
            }
        }

        private void OnSelectionAdapterCancel()
        {
            _isUpdatingText = true;
            _postDescription.Text = _postDescription.Text.ApplyMacros(_postDescription.CaretIndex,
                GetDisplayText(ItemsSelector.SelectedItem));
            _postDescription.SelectionStart = _postDescription.Text.Length;
            _postDescription.SelectionLength = 0;
            _isUpdatingText = false;
            IsDropDownOpen = false;
            _selectionCancelled = true;
        }

        private string GetDisplayText(object dataItem)
        {
            if (BindingEvaluator == null) BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));
            if (dataItem == null) return string.Empty;
            if (string.IsNullOrEmpty(DisplayMember)) return dataItem.ToString();
            return BindingEvaluator.Evaluate(dataItem);
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            _selectionCancelled = false;
            ItemsSelector.SelectedItem = SelectedItem;
        }

        #endregion


        private void SocinatorTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _postDescription?.Focus();
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (SelectionAdapter == null)
                return;

            if (IsDropDownOpen)
                SelectionAdapter.HandleKeyDown(e);
            else
                IsDropDownOpen = e.Key == Key.Down || e.Key == Key.Up;
        }

        private void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
                IsDropDownOpen = false;
        }

        private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText)
                return;
            if (FetchTimer == null)
            {
                FetchTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(Delay)
                };
                FetchTimer.Tick += OnFetchTimerTick;
            }

            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            SetSelectedItem(null);
            if (_postDescription.Text.Length > 0)
            {
                if (!_postDescription.Text.IsGetMacros())
                {
                    if (IsDropDownOpen)
                    {
                        IsDropDownOpen = false;
                        _postDescription.SelectionStart = _postDescription.Text.Length;
                        _postDescription.SelectionLength = 0;
                    }

                    return;
                }

                IsLoading = true;
                IsDropDownOpen = true;
                ItemsSelector.ItemsSource = null;
                FetchTimer.IsEnabled = true;
                FetchTimer.Start();
            }
            else
            {
                IsDropDownOpen = false;
            }
        }

        private void SetSelectedItem(object item)
        {
            _isUpdatingText = true;
            SelectedItem = item;
            _isUpdatingText = false;
        }

        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SocinatorTextBox socinatorTextBox;
            socinatorTextBox = d as SocinatorTextBox;
            if (socinatorTextBox != null)
                if ((socinatorTextBox._postDescription != null) & !socinatorTextBox._isUpdatingText)
                {
                    socinatorTextBox._isUpdatingText = true;
                    socinatorTextBox._postDescription.Text = socinatorTextBox._postDescription.Text.ApplyMacros(
                        socinatorTextBox._postDescription.CaretIndex,
                        socinatorTextBox.BindingEvaluator.Evaluate(e.NewValue));
                    socinatorTextBox._isUpdatingText = false;
                }
        }

        private void OnFetchTimerTick(object sender, EventArgs e)
        {
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            if (SuggestionProvider != null && ItemsSelector != null)
            {
                var getFirstSubString = _postDescription.Text.Substring(0, _postDescription.CaretIndex);
                var startIndexOfCurrentWord = getFirstSubString.LastIndexOf("{", StringComparison.Ordinal);
                if (startIndexOfCurrentWord == -1)
                    return;

                var getFilter = getFirstSubString.Substring(startIndexOfCurrentWord);

                Filter = getFilter;

                if (_suggestionsAdapter == null) _suggestionsAdapter = new SuggestionsAdapter(this);
                _suggestionsAdapter.GetSuggestions(Filter);
            }
        }

        private void ItemsSelector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var posItem = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (posItem == null)
                return;
            if (!ItemsSelector.Items.Contains(posItem))
                return;
            ItemsSelector.SelectedItem = posItem;
            OnSelectionAdapterCommit();
        }

        private void OnSelectionAdapterSelectionChanged()
        {
            _isUpdatingText = true;
            _postDescription.Text = _postDescription.Text.ApplyMacros(_postDescription.CaretIndex,
                GetDisplayText(ItemsSelector.SelectedItem));
            _postDescription.SelectionStart = _postDescription.Text.Length;
            _postDescription.SelectionLength = 0;
            ScrollToSelectedItem();
            _isUpdatingText = false;
        }


        private void ScrollToSelectedItem()
        {
            var listBox = ItemsSelector as ListBox;
            if (listBox?.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }

        #endregion
    }

    public interface ISuggestionProvider
    {
        #region Public Methods

        IEnumerable GetSuggestions(string filter);

        #endregion Public Methods
    }

    public class BindingEvaluator : FrameworkElement
    {
        #region Constructors

        public BindingEvaluator(Binding binding)
        {
            ValueBinding = binding;
        }

        #endregion

        #region Methods

        public string Evaluate(object dataItem)
        {
            DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }

        #endregion

        #region Fields

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string),
            typeof(BindingEvaluator), new FrameworkPropertyMetadata(string.Empty));

        #endregion

        #region Properties

        public string Value
        {
            get => (string) GetValue(ValueProperty);

            set => SetValue(ValueProperty, value);
        }

        public Binding ValueBinding { get; set; }

        #endregion
    }

    public class SelectionAdapter
    {
        #region Constructors

        public SelectionAdapter(Selector selector)
        {
            SelectorControl = selector;

            // initialize event for mouse up
            SelectorControl.PreviewMouseUp += OnSelectorMouseDown;
        }

        #endregion

        #region Properties

        public Selector SelectorControl { get; set; }

        #endregion

        #region Events

        /// <summary>
        ///     To specify selection popup has closed with cancel the replace event of the textbox
        /// </summary>
        public delegate void CancelEventHandler();

        /// <summary>
        ///     To specify selection popup has closed with commit the replace event of the textbox
        /// </summary>
        public delegate void CommitEventHandler();

        /// <summary>
        ///     To specify the whenever selection has been changed in popup
        /// </summary>
        public delegate void SelectionChangedEventHandler();

        /// <summary>
        ///     Object of <see cref="SelectionAdapter.CancelEventHandler" />
        /// </summary>
        public event CancelEventHandler Cancel;

        /// <summary>
        ///     Object of <see cref="SelectionAdapter.CommitEventHandler" />
        /// </summary>
        public event CommitEventHandler Commit;

        /// <summary>
        ///     Object of <see cref="SelectionAdapter.SelectionChangedEventHandler" />
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion

        #region Methods

        public void HandleKeyDown(KeyEventArgs key)
        {
            switch (key.Key)
            {
                case Key.Down:
                    IncrementSelection();
                    break;
                case Key.Up:
                    DecrementSelection();
                    break;
                case Key.Escape:
                    Cancel?.Invoke();
                    break;
                case Key.Enter:
                case Key.Tab:
                    Commit?.Invoke();
                    break;
            }
        }

        private void DecrementSelection()
        {
            if (SelectorControl.SelectedIndex == -1)
            {
                SelectorControl.SelectedIndex = 0;
            }
            else
            {
                if (SelectorControl.SelectedIndex == 0)
                    return;
                SelectorControl.SelectedIndex -= 1;
                SelectionChanged?.Invoke();
            }
        }

        private void IncrementSelection()
        {
            if (SelectorControl.SelectedIndex == SelectorControl.Items.Count - 1)
            {
                SelectorControl.SelectedIndex = -1;
            }
            else
            {
                if (SelectorControl.SelectedIndex >= SelectorControl.Items.Count)
                    return;
                SelectorControl.SelectedIndex += 1;
                SelectionChanged?.Invoke();
            }
        }

        private void OnSelectorMouseDown(object sender, MouseButtonEventArgs e)
        {
            Commit?.Invoke();
        }

        #endregion
    }
}