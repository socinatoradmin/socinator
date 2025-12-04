#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class LiveChatViewModel : BindableBase, IDisposable
    {
        private readonly IGenericFileManager _genericFileManager;
        public SocialNetworks SocialNetworks { get; set; }

        public LiveChatViewModel(SocialNetworks networks)
        {
            SocialNetworks = networks;
            InitilizeDefaultValue(SocialNetworks);
            LiveChatModel.LstImages.CollectionChanged += images_CollectionChanged;
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            SendMessageCommand = new BaseCommand<object>(sender => true, SendMessageExecute);
            UserSelectionChangedCommand = new BaseCommand<object>(sender => true, UserSelectionChangedExecute);
            FriendSelectionChangedCommand = new BaseCommand<object>(sender => true, FriendSelectionChangedExecute);
            AttachFileCommand = new BaseCommand<object>(sender => true, AttachFileExecute);
            EmojiCommand = new BaseCommand<object>(sender => true, EmojiExecute);
            ClearChatListCommand = new BaseCommand<object>(sender => true, ClearFriendsExecute);
            InitilizeEmoji();
            UpdateFriendList();
        }

        private void images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LiveChatModel.ImageCount = LiveChatModel.LstImages.Count;
        }


        #region Command

        public ICommand SendMessageCommand { get; set; }
        public ICommand UserSelectionChangedCommand { get; set; }
        public ICommand FriendSelectionChangedCommand { get; set; }
        public ICommand AttachFileCommand { get; set; }
        public ICommand EmojiCommand { get; set; }
        public ICommand ClearChatListCommand { get; set; }

        #endregion

        #region Properties

        private bool _isPopupOpen;

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                if (value == _isPopupOpen)
                    return;
                SetProperty(ref _isPopupOpen, value);
            }
        }

        private ObservableCollection<Emoji> _lstEmojiSmileysAndPeople = new ObservableCollection<Emoji>();

        public ObservableCollection<Emoji> LstEmojiSmileysAndPeople
        {
            get => _lstEmojiSmileysAndPeople;
            set
            {
                if (value == _lstEmojiSmileysAndPeople)
                    return;
                SetProperty(ref _lstEmojiSmileysAndPeople, value);
            }
        }

        public CancellationTokenSource CancellationSource = new CancellationTokenSource();
        private LiveChatModel _liveChatModel = new LiveChatModel();

        public LiveChatModel LiveChatModel
        {
            get => _liveChatModel;
            set
            {
                if (value == _liveChatModel)
                    return;
                SetProperty(ref _liveChatModel, value);
            }
        }

        private List<DominatorAccountModel> _lstAccountModel = new List<DominatorAccountModel>();


        public List<DominatorAccountModel> LstAccountModel
        {
            get => _lstAccountModel;
            set
            {
                if (value == _lstAccountModel)
                    return;
                SetProperty(ref _lstAccountModel, value);
            }
        }

        #endregion

        #region Command Methods

        private void UserSelectionChangedExecute(object sender)
        {
            try
            {
                LstAccountModel = InstanceProvider.GetInstance<IAccountsFileManager>()
                    .GetAll(SocialNetworks);

                LiveChatModel.DominatorAccountModel = LstAccountModel.FirstOrDefault(x =>
                    x.UserName == LiveChatModel.SelectedAccount && x.AccountBaseModel.AccountNetwork == SocialNetworks);

                UpdateFriendList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FriendSelectionChangedExecute(object sender)
        {
            if (LiveChatModel.SenderDetails == null) return;
            try
            {
                CancelPriviousTask();

                var senders = _genericFileManager.GetModuleDetails<ChatDetails>(
                    FileDirPath.GetChatDetailFile(LiveChatModel.DominatorAccountModel.AccountBaseModel
                        .AccountNetwork)).Where(x => x.SenderId == LiveChatModel.SenderDetails.SenderId);

                senders = senders.OrderBy(x => x.MessageTime);
                Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstChat.Clear());
                // ReSharper disable once ConstantConditionalAccessQualifier
                senders?.ForEach(chat =>
                {
                    Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstChat.Add(chat));
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    CancelPriviousTask();
                    //Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstChat.Clear());

                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                        .UpdateCurrentChat(LiveChatModel, CancellationSource.Token);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void SendMessageExecute(object sender)
        {
            var messageType = !string.IsNullOrEmpty(LiveChatModel.TextMessage) &&
                              LiveChatModel.LstImages.Count > 0 ? ChatMessageType.TextAndMedia :
                !string.IsNullOrEmpty(LiveChatModel.TextMessage) ? ChatMessageType.Text : ChatMessageType.Media;


            SendMesage(LiveChatModel.TextMessage, LiveChatModel.LstImages.ToList().DeepCloneObject(),
                messageType);

            LiveChatModel.LstImages.Clear();

            LiveChatModel.TextMessage = string.Empty;
        }

        private void AttachFileExecute(object sender)
        {
            var filters = "Image Files | *.jpg; *.jpeg; *.png; *.gif";
            List<string> picPath = FileUtilities.GetImageOrVideo(true, filters);
            LiveChatModel.LstImages.AddRange(picPath);
            //if (picPath != null)
            //    SendMesage(picPath, ChatMessageType.Media);
        }

        private void EmojiExecute(object sender)
        {
            IsPopupOpen = true;
        }

        private void ClearFriendsExecute(object sender)
        {
            try
            {
                CancelPriviousTask();
                _genericFileManager.DeleteBinFiles(
                    FileDirPath.GetChatDetailFile(LiveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));

                _genericFileManager.DeleteBinFiles(
                    FileDirPath.GetFriendDetailFile(LiveChatModel.DominatorAccountModel.AccountBaseModel
                        .AccountNetwork));

                LiveChatModel.LstSender.Clear();
                LiveChatModel.LstChat.Clear();
                //LiveChatModel.LstImages.Clear();
                InitilizeDefaultValue(SocialNetworks);
                UpdateFriendList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Methods

        public void UpdateFriendList()
        {
            try
            {
                CancelPriviousTask();

                var senders = _genericFileManager.GetModuleDetails<SenderDetails>(
                    FileDirPath.GetFriendDetailFile(LiveChatModel.DominatorAccountModel.AccountBaseModel
                        .AccountNetwork)).Where(x => x.AccountId == LiveChatModel.DominatorAccountModel.AccountId);

                senders = senders.OrderByDescending(x => x.LastMessegeDateTime);
                Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstSender.Clear());
                // ReSharper disable once ConstantConditionalAccessQualifier
                senders?.ForEach(sender =>
                {
                    Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstSender.Add(sender));
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            ThreadFactory.Instance.Start(() =>
            {
                try
                {
                    CancelPriviousTask();
                    //LiveChatModel.DominatorAccountModel = LstAccountModel.FirstOrDefault(x => x.UserName == LiveChatModel.SelectedAccount);

                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                        .CloseBrowser(LiveChatModel, CancellationSource.Token);

                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                        .UpdateFriendList(LiveChatModel, CancellationSource.Token);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private async void SendMesage(string message, List<string> listImages, ChatMessageType chatMessageType)
        {
            try
            {
                CancelPriviousTask();

                if (!string.IsNullOrEmpty(message) || listImages.Count > 0)
                {
                    var isSent = await SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory()
                        .ChatFactory
                        .SendMessageToUser(LiveChatModel, message, listImages, chatMessageType,
                            CancellationSource.Token);

                    if (isSent)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks,
                            LiveChatModel.DominatorAccountModel.AccountBaseModel.UserName, "Chat",
                            "Successfully sent message!");
                    }
                    else
                    {
                        LiveChatModel.TextMessage = message;
                        LiveChatModel.LstImages.AddRange(listImages);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks,
                            LiveChatModel.DominatorAccountModel.AccountBaseModel.UserName, "Chat",
                            "Message sending fail");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void InitilizeEmoji()
        {
            LstEmojiSmileysAndPeople = new ObservableCollection<Emoji>
            {
                new Emoji("Grinning Face", "😀"),
                new Emoji("Beaming Face With Smiling Eyes", "😁"),
                new Emoji("Face With Tears of Joy", "😂"),
                new Emoji("Rolling on the Floor Laughing", "🤣"),
                new Emoji("Grinning Face With Big Eyes", "😃"),
                new Emoji("Grinning Face With Smiling Eyes", "😄"),
                new Emoji("Grinning Face With Sweat", "😅"),
                new Emoji("Grinning Squinting Face", "😆"),
                new Emoji("Winking Face", "😉"),
                new Emoji("Smiling Face With Smiling Eyes", "😊"),
                new Emoji("Face Savoring Food", "😋"),
                new Emoji("Smiling Face With Sunglasses", "😎"),
                new Emoji("Smiling Face With Heart-Eyes", "😍"),
                new Emoji("Face Blowing a Kiss", "😘"),
                new Emoji("Kissing Face", "😗"),
                new Emoji("Kissing Face With Smiling Eyes", "😙"),
                new Emoji("Kissing Face With Closed Eyes", "😚"),
                new Emoji("Smiling Face", "☺"),
                new Emoji("Slightly Smiling Face", "🙂"),
                new Emoji("Hugging Face", "🤗"),
                new Emoji("Star-Struck", "🤩"),
                new Emoji("Thinking Face", "🤔"),
                new Emoji("Face With Raised Eyebrow", "🤨"),
                new Emoji("Neutral Face", "😐"),
                new Emoji("Expressionless Face", "😑"),
                new Emoji("Face Without Mouth", "😶"),
                new Emoji("Face With Rolling Eyes", "🙄"),
                new Emoji("Smirking Face", "😏"),
                new Emoji("Persevering Face", "😣"),
                new Emoji("Sad but Relieved Face", "😥"),
                new Emoji("Face With Open Mouth", "😮"),
                new Emoji("Zipper-Mouth Face", "🤐"),
                new Emoji("Hushed Face", "😯"),
                new Emoji("Sleepy Face", "😪"),
                new Emoji("Tired Face", "😫"),
                new Emoji("Sleeping Face", "😴"),
                new Emoji("Relieved Face", "😌"),
                new Emoji("Face With Tongue", "😛"),
                new Emoji("Winking Face With Tongue", "😜"),
                new Emoji("Squinting Face With Tongue", "😝"),
                new Emoji("Drooling Face", "🤤"),
                new Emoji("Unamused Face", "😒"),
                new Emoji("Downcast Face With Sweat", "😓"),
                new Emoji("Pensive Face", "😔"),
                new Emoji("Confused Face", "😕"),
                new Emoji("Upside-Down Face", "🙃"),
                new Emoji("Money-Mouth Face", "🤑"),
                new Emoji("Astonished Face", "😲"),
                new Emoji("Frowning Face", "☹"),
                new Emoji("Slightly Frowning Face", "🙁"),
                new Emoji("Confounded Face", "😖"),
                new Emoji("Disappointed Face", "😞"),
                new Emoji("Worried Face", "😟"),
                new Emoji("Face With Steam From Nose", "😤"),
                new Emoji("Crying Face", "😢"),
                new Emoji("Loudly Crying Face", "😭"),
                new Emoji("Frowning Face With Open Mouth", "😦"),
                new Emoji("Anguished Face", "😧"),
                new Emoji("Fearful Face", "😨"),
                new Emoji("Weary Face", "😩"),
                new Emoji("Exploding Head", "🤯"),
                new Emoji("Grimacing Face", "😬"),
                new Emoji("Anxious Face With Sweat", "😰"),
                new Emoji("Face Screaming in Fear", "😱"),
                new Emoji("Flushed Face", "😳"),
                new Emoji("Zany Face", "🤪"),
                new Emoji("Dizzy Face", "😵"),
                new Emoji("Pouting Face", "😡"),
                new Emoji("Angry Face", "😠"),
                new Emoji("Face With Symbols on Mouth", "🤬"),
                new Emoji("Face With Medical Mask", "😷"),
                new Emoji("Face With Thermometer", "🤒"),
                new Emoji("Face With Head-Bandage", "🤕"),
                new Emoji("Nauseated Face", "🤢"),
                new Emoji("Face Vomiting", "🤮"),
                new Emoji("Sneezing Face", "🤧"),
                new Emoji("Smiling Face With Halo", "😇"),
                new Emoji("Cowboy Hat Face", "🤠"),
                new Emoji("Clown Face", "🤡"),
                new Emoji("Lying Face", "🤥"),
                new Emoji("Shushing Face", "🤫"),
                new Emoji("Face With Hand Over Mouth", "🤭"),
                new Emoji("Face With Monocle", "🧐"),
                new Emoji("Nerd Face", "🤓"),
                new Emoji("Smiling Face With Horns", "😈"),
                new Emoji("Angry Face With Horns", "👿"),
                new Emoji("Ogre", "👹"),
                new Emoji("Goblin", "👺"),
                new Emoji("Skull", "💀"),
                new Emoji("Ghost", "👻"),
                new Emoji("Alien", "👽"),
                new Emoji("Robot Face", "🤖"),
                new Emoji("Pile of Poo", "💩"),
                new Emoji("Grinning Cat Face", "😺"),
                new Emoji("Grinning Cat Face With Smiling Eyes", "😸"),
                new Emoji("Cat Face With Tears of Joy", "😹"),
                new Emoji("Smiling Cat Face With Heart-Eyes", "😻"),
                new Emoji("Cat Face With Wry Smile", "😼"),
                new Emoji("Kissing Cat Face", "😽"),
                new Emoji("Weary Cat Face", "🙀"),
                new Emoji("Crying Cat Face", "😿"),
                new Emoji("Pouting Cat Face", "😾"),
                new Emoji("Baby", "👶"),
                new Emoji("Boy", "👦"),
                new Emoji("Girl", "👧"),
                new Emoji("Man", "👨"),
                new Emoji("Woman", "👩"),
                new Emoji("Old Man", "👴"),
                new Emoji("Old Woman", "👵"),
                new Emoji("Man Health Worker", "👨‍⚕️"),
                new Emoji("Woman Health Worker", "👩‍⚕️"),
                new Emoji("Man Student", "👨‍🎓"),
                new Emoji("Woman Student", "👩‍🎓"),
                new Emoji("Man Judge", "👨‍⚖️"),
                new Emoji("Woman Judge", "👩‍⚖️"),
                new Emoji("Man Farmer", "👨‍🌾"),
                new Emoji("Woman Farmer", "👩‍🌾"),
                new Emoji("Man Cook", "👨‍🍳"),
                new Emoji("Woman Cook", "👩‍🍳"),
                new Emoji("Man Mechanic", "👨‍🔧"),
                new Emoji("Woman Mechanic", "👩‍🔧"),
                new Emoji("Man Factory Worker", "👨‍🏭"),
                new Emoji("Woman Factory Worker", "👩‍🏭"),
                new Emoji("Man Office Worker", "👨‍💼"),
                new Emoji("Woman Office Worker", "👩‍💼"),
                new Emoji("Man Scientist", "👨‍🔬"),
                new Emoji("Woman Scientist", "👩‍🔬"),
                new Emoji("Man Technologist", "👨‍💻"),
                new Emoji("Woman Technologist", "👩‍💻"),
                new Emoji("Man Singer", "👨‍🎤"),
                new Emoji("Woman Singer", "👩‍🎤"),
                new Emoji("Man Artist", "👨‍🎨"),
                new Emoji("Woman Artist", "👩‍🎨"),
                new Emoji("Man Pilot", "👨‍✈️"),
                new Emoji("Woman Pilot", "👩‍✈️"),
                new Emoji("Man Astronaut", "👨‍🚀"),
                new Emoji("Woman Astronaut", "👩‍🚀"),
                new Emoji("Man Firefighter", "👨‍🚒"),
                new Emoji("Woman Firefighter", "👩‍🚒"),
                new Emoji("Police Officer", "👮"),
                new Emoji("Man Police Officer", "👮‍♂️"),
                new Emoji("Woman Police Officer", "👮‍♀️"),
                new Emoji("Detective", "🕵"),
                new Emoji("Man Detective", "🕵️‍♂️"),
                new Emoji("Woman Detective", "🕵️‍♀️"),
                new Emoji("Guard", "💂"),
                new Emoji("Man Guard", "💂‍♂️"),
                new Emoji("Woman Guard", "💂‍♀️"),
                new Emoji("Construction Worker", "👷"),
                new Emoji("Man Construction Worker", "👷‍♂️"),
                new Emoji("Woman Construction Worker", "👷‍♀️"),
                new Emoji("Prince", "🤴"),
                new Emoji("Princess", "👸"),
                new Emoji("Person Wearing Turban", "👳"),
                new Emoji("Man Wearing Turban", "👳‍♂️"),
                new Emoji("Woman Wearing Turban", "👳‍♀️"),
                new Emoji("Man With Chinese Cap", "👲"),
                new Emoji("Woman With Headscarf", "🧕"),
                new Emoji("Bearded Person", "🧔"),
                new Emoji("Blond-Haired Person", "👱"),
                new Emoji("Blond-Haired Man", "👱‍♂️"),
                new Emoji("Blond-Haired Woman", "👱‍♀️"),
                new Emoji("Man in Tuxedo", "🤵"),
                new Emoji("Bride With Veil", "👰"),
                new Emoji("Pregnant Woman", "🤰"),
                new Emoji("Breast-Feeding", "🤱"),
                new Emoji("Baby Angel", "👼"),
                new Emoji("Santa Claus", "🎅"),
                new Emoji("Mrs. Claus", "🤶"),
                new Emoji("Woman Mage", "🧙‍♀️"),
                new Emoji("Man Mage", "🧙‍♂️"),
                new Emoji("Woman Fairy", "🧚‍♀️"),
                new Emoji("Man Fairy", "🧚‍♂️"),
                new Emoji("Woman Vampire", "🧛‍♀️"),
                new Emoji("Man Vampire", "🧛‍♂️"),
                new Emoji("Mermaid", "🧜‍♀️"),
                new Emoji("Merman", "🧜‍♂️"),
                new Emoji("Woman Elf", "🧝‍♀️"),
                new Emoji("Man Elf", "🧝‍♂️"),
                new Emoji("Woman Genie", "🧞‍♀️"),
                new Emoji("Man Genie", "🧞‍♂️"),
                new Emoji("Woman Zombie", "🧟‍♀️"),
                new Emoji("Man Zombie", "🧟‍♂️"),
                new Emoji("Person Frowning", "🙍"),
                new Emoji("Man Frowning", "🙍‍♂️"),
                new Emoji("Woman Frowning", "🙍‍♀️"),
                new Emoji("Person Pouting", "🙎"),
                new Emoji("Man Pouting", "🙎‍♂️"),
                new Emoji("Woman Pouting", "🙎‍♀️"),
                new Emoji("Person Gesturing No", "🙅"),
                new Emoji("Man Gesturing No", "🙅‍♂️"),
                new Emoji("Woman Gesturing No", "🙅‍♀️"),
                new Emoji("Person Gesturing OK", "🙆"),
                new Emoji("Man Gesturing OK", "🙆‍♂️"),
                new Emoji("Woman Gesturing OK", "🙆‍♀️"),
                new Emoji("Person Tipping Hand", "💁"),
                new Emoji("Man Tipping Hand", "💁‍♂️"),
                new Emoji("Woman Tipping Hand", "💁‍♀️"),
                new Emoji("Person Raising Hand", "🙋"),
                new Emoji("Man Raising Hand", "🙋‍♂️"),
                new Emoji("Woman Raising Hand", "🙋‍♀️"),
                new Emoji("Person Bowing", "🙇"),
                new Emoji("Man Bowing", "🙇‍♂️"),
                new Emoji("Woman Bowing", "🙇‍♀️"),
                new Emoji("Person Facepalming", "🤦"),
                new Emoji("Man Facepalming", "🤦‍♂️"),
                new Emoji("Woman Facepalming", "🤦‍♀️"),
                new Emoji("Person Shrugging", "🤷"),
                new Emoji("Man Shrugging", "🤷‍♂️"),
                new Emoji("Woman Shrugging", "🤷‍♀️"),
                new Emoji("Person Getting Massage", "💆"),
                new Emoji("Man Getting Massage", "💆‍♂️"),
                new Emoji("Woman Getting Massage", "💆‍♀️"),
                new Emoji("Person Getting Haircut", "💇"),
                new Emoji("Man Getting Haircut", "💇‍♂️"),
                new Emoji("Woman Getting Haircut", "💇‍♀️"),
                new Emoji("Person Walking", "🚶"),
                new Emoji("Man Walking", "🚶‍♂️"),
                new Emoji("Woman Walking", "🚶‍♀️"),
                new Emoji("Person Running", "🏃"),
                new Emoji("Man Running", "🏃‍♂️"),
                new Emoji("Woman Running", "🏃‍♀️"),
                new Emoji("Woman Dancing", "💃"),
                new Emoji("Man Dancing", "🕺"),
                new Emoji("People With Bunny Ears", "👯"),
                new Emoji("Men With Bunny Ears", "👯‍♂️"),
                new Emoji("Women With Bunny Ears", "👯‍♀️"),
                new Emoji("Woman in Steamy Room", "🧖‍♀️"),
                new Emoji("Man in Steamy Room", "🧖‍♂️"),
                new Emoji("Man in Suit Levitating", "🕴"),
                new Emoji("Speaking Head", "🗣"),
                new Emoji("Bust in Silhouette", "👤"),
                new Emoji("Busts in Silhouette", "👥"),
                new Emoji("Man and Woman Holding Hands", "👫"),
                new Emoji("Two Men Holding Hands", "👬"),
                new Emoji("Two Women Holding Hands", "👭")
            };
        }

        public void InitilizeDefaultValue(SocialNetworks socialNetworks)
        {
            var accountModelList = InstanceProvider.GetInstance<IAccountsFileManager>()
                .GetAll(socialNetworks).ToList();

            LiveChatModel.AccountNames = new ObservableCollection<string>(accountModelList
                .Where(x => x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName).ToList());

            if (LiveChatModel.AccountNames.Count > 0)
            {
                LiveChatModel.DominatorAccountModel = accountModelList[0];
                LiveChatModel.SelectedAccount = LiveChatModel.AccountNames.First();
            }

            try
            {
                LstAccountModel = accountModelList;

                LiveChatModel.DominatorAccountModel =
                    LstAccountModel.FirstOrDefault(x =>
                        x.AccountBaseModel.UserName == LiveChatModel.SelectedAccount);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        private void CancelPriviousTask()
        {
            CancellationSource.Cancel();
            CancellationSource.Dispose();
            CancellationSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (CancellationSource != null)
            {
                CancellationSource.Dispose();
                CancellationSource = null;
            }
        }
    }


    public class Emoji
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Emoji(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}