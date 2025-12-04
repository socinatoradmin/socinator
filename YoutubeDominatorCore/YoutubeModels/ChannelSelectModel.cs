using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YoutubeDominatorCore.YoutubeModels
{
    public class ChannelSelectModel : INotifyPropertyChanged
    {
        private string _destinationId;
        private string _destinationName;

        private ObservableCollection<ChannelDestinationSelectModel> _listSelectDestination =
            new ObservableCollection<ChannelDestinationSelectModel>();


        /// <summary>
        ///     To specify the destination Id
        /// </summary>
        [ProtoMember(1)]
        public string DestinationId
        {
            get => _destinationId;
            set
            {
                if (_destinationId == value)
                    return;
                _destinationId = value;
                OnPropertyChanged(nameof(DestinationId));
            }
        }


        /// <summary>
        ///     To specify the destination name
        /// </summary>
        [ProtoMember(2)]
        public string DestinationName
        {
            get => _destinationName;
            set
            {
                if (_destinationName == value)
                    return;
                _destinationName = value;
                OnPropertyChanged(nameof(DestinationName));
            }
        }

        /// <summary>
        ///     To hold all destination list which holds all group,page count both selected and total
        /// </summary>
        [ProtoMember(10)]
        public ObservableCollection<ChannelDestinationSelectModel> ListSelectDestination
        {
            get => _listSelectDestination;
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     To assign the default for the destination
        /// </summary>
        /// <returns>returns as filled default value of  <see cref="BoardCreateDestinationModel" /></returns>
        public static ChannelSelectModel DestinationDefaultBuilder()
        {
            return new ChannelSelectModel
            {
                DestinationName = $"Default-{ConstantVariable.GetDateTime()}",
                DestinationId = Utilities.GetGuid()
                //AccountPagesBoardsPair = new List<Tuple<string, KeyValuePair<string, List<RepinQueryContent>>, string>>(),
                //AccountGroupPair = new List<KeyValuePair<string, string>>(),
                //SelectedAccountIds = new List<string>(),
                //CustomDestinations = new List<KeyValuePair<string, PublisherCustomDestinationModel>>(),
                //CreatedDate = DateTime.Now,
                //DestinationDetailsModels = new List<PublisherDestinationDetailsModel>()
            };
        }
    }
}