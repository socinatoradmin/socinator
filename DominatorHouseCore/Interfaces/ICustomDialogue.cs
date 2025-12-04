using MahApps.Metro.Controls.Dialogs;
using System.Windows;
namespace DominatorHouseCore.Interfaces
{
    public interface ICustomDialogue
    {
        MessageDialogResult ShowCustomDialog(string title, string message, string affirmativeText, string negativeText);
        MessageDialogResult ShowDialog(string title, string message);
        string ShowModalInputExternal(string title, string message, string affirmativeText, string negativeText,string DefaultText);
        Window GetWindow(string title, object windowContent,bool WithoutClose=false);
    }
}
