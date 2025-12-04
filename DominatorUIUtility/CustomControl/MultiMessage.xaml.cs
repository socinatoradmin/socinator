using System.Windows;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for MultiMessage.xaml
    /// </summary>
    public partial class MultiMessage
    {
        public MultiMessage()
        {
            InitializeComponent();
            ChatControl.DataContext = MultiMessagesModel;
        }

        public MultiMessagesModel MultiMessagesModel { get; set; } = new MultiMessagesModel();

        private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (e.OldValue != null)
            {
                if (e.OldValue > e.NewValue)
                {
                    for (var i = e.OldValue; i > e.NewValue; i--)
                        MultiMessagesModel.LstMessages.RemoveAt(MultiMessagesModel.LstMessages.Count - 1);
                }
                else if (e.OldValue < e.NewValue)
                {
                    MultiMessagesModel.LstMessages.Clear();
                    for (var i = 0; i < e.NewValue; i++)
                        MultiMessagesModel.LstMessages.Add("");
                }
            }
            else
            {
                MultiMessagesModel.LstMessages.Clear();
                for (var i = 0; i < e.NewValue; i++)
                    MultiMessagesModel.LstMessages.Add("");
            }
        }
    }
}