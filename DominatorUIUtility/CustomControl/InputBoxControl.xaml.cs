using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for InputBoxControl.xaml
    /// </summary>
    public partial class InputBoxControl
    {
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(InputBoxControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for InputCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputCollectionProperty =
            DependencyProperty.Register("InputCollection", typeof(List<string>), typeof(InputBoxControl),
                new PropertyMetadata(new List<string>()));


        private static readonly RoutedEvent GetInputClickEvent = EventManager.RegisterRoutedEvent("GetInputClick",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InputBoxControl));

        public static readonly DependencyProperty SaveVisiblityProperty =
            DependencyProperty.Register("SaveVisiblity", typeof(Visibility), typeof(InputBoxControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty ImportVisiblityProperty =
            DependencyProperty.Register("ImportVisiblity", typeof(Visibility), typeof(InputBoxControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty RefreshVisiblityProperty =
            DependencyProperty.Register("RefreshVisiblity", typeof(Visibility), typeof(InputBoxControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMarkText", typeof(string), typeof(InputBoxControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(InputBoxControl));

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(InputBoxControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InputBoxControl),
                new FrameworkPropertyMetadata());

        public InputBoxControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            SaveVisiblity = Visibility.Visible;
            ImportVisiblity = Visibility.Visible;
            RefreshVisiblity = Visibility.Visible;
            InputCollection = new List<string>();
            Height = 80;
        }

        public string InputText
        {
            get => (string) GetValue(InputTextProperty);
            set => SetValue(InputTextProperty, value);
        }


        public List<string> InputCollection
        {
            get => (List<string>) GetValue(InputCollectionProperty);
            set => SetValue(InputCollectionProperty, value);
        }

        public Visibility SaveVisiblity
        {
            get => (Visibility) GetValue(SaveVisiblityProperty);
            set => SetValue(SaveVisiblityProperty, value);
        }

        public Visibility ImportVisiblity
        {
            get => (Visibility) GetValue(ImportVisiblityProperty);
            set => SetValue(ImportVisiblityProperty, value);
        }

        public Visibility RefreshVisiblity
        {
            get => (Visibility) GetValue(RefreshVisiblityProperty);
            set => SetValue(RefreshVisiblityProperty, value);
        }

        public string WaterMarkText
        {
            get => (string) GetValue(WaterMarkProperty);
            set => SetValue(WaterMarkProperty, value);
        }


        public ICommand SaveCommand
        {
            get => (ICommand) GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        public ICommand ClearCommand
        {
            get => (ICommand) GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        ///     Create a RoutedEventHandler for query clicks
        /// </summary>
        public event RoutedEventHandler GetInputClick
        {
            add => AddHandler(GetInputClickEvent, value);
            remove => RemoveHandler(GetInputClickEvent, value);
        }

        private void BtnImportBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = FileUtilities.FileBrowseAndReader();
                if (list.Count == 0)
                    return;
                if (string.IsNullOrWhiteSpace(InputText))
                    InputCollection.Clear();
                foreach (var text in list)
                    if (!InputCollection.Contains(text))
                        InputCollection.Add(text);

                InputText = string.Empty;

                var tmpLstInputs = InputCollection;


                ThreadFactory.Instance.Start(() =>
                {
                    var cache = new CacheText
                    {
                        Limit = tmpLstInputs.Count
                    };

                    for (var counter = 0; counter < tmpLstInputs.Count; counter++)
                    {
                        var input = tmpLstInputs[counter];
                        input = counter == 0 ? input : "\r\n" + input;

                        cache.AddToCache(input);

                        //if (cache.AddToCache(input))
                        //    continue;
                        //AddTextToInputBox(cache.GetCacheText());

                        // System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500));
                    }

                    AddTextToInputBox(cache.GetCacheText());

                    GlobusLogHelper.log.Info("Text file content uploaded successfully!");
                    ToasterNotification.ShowSuccess("Text file content uploaded successfully!");
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddTextToInputBox(string inputText)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { InputText += inputText; }, DispatcherPriority.Background);
            else
                InputText += inputText;
        }

        private void BtnRefereshBlacklistsText_OnClick(object sender, RoutedEventArgs e)
        {
            InputCollection.Clear();
            InputText = string.Empty;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            return;
            if (!mainRTB.IsFocused)
                return;

            var start = mainRTB
                .CaretPosition; // this is the variable we will advance to the left until a non-letter character is found
            var end = mainRTB
                .CaretPosition; // this is the variable we will advance to the right until a non-letter character is found

            var stringBeforeCaret =
                start.GetTextInRun(LogicalDirection
                    .Backward); // extract the text in the current run from the caret to the left
            var stringAfterCaret =
                start.GetTextInRun(LogicalDirection
                    .Forward); // extract the text in the current run from the caret to the left

            if (((sender as Button).Background as SolidColorBrush).Color == Colors.LightGreen)
                ((Button) sender).Background = new SolidColorBrush(Colors.Transparent);
            if (((sender as Button).Background as SolidColorBrush).Color == Colors.Transparent)
                ((Button) sender).Background = new SolidColorBrush(Colors.LightGreen);

            InputText = StringFromRichTextBox(mainRTB);
        }

        private string StringFromRichTextBox(RichTextBox rtb)
        {
            //var tr = new System.Windows.Documents.TextRange(mainRTB.Document.ContentStart,
            //                     mainRTB.Document.ContentEnd);
            //var ms = new System.IO.MemoryStream();
            //tr.Save(ms, DataFormats.Xaml);
            //string xamlText = System.Text.ASCIIEncoding.Default.GetString(ms.ToArray());

            var textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );


            //string strRegex = @"<b>(?<X>.*?)</b>";
            //var myRegexOptions = System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline;
            //var myRegex = new System.Text.RegularExpressions.Regex(strRegex, myRegexOptions);
            //string strTargetString = "Hellow world is my <b>first</b> application in <b>computer</b> world.";
            //string NewString = strTargetString;
            //foreach (System.Text.RegularExpressions.Match myMatch in myRegex.Matches(strTargetString))
            //{
            //    if (myMatch.Success)
            //    {
            //        NewString = NewString.Replace(myMatch.ToString(), myMatch.ToString().ToUpper());
            //    }
            //}
            //InputText = NewString;
            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ClearCommand?.Execute(sender);
            }
            catch { }
        }
    }
}