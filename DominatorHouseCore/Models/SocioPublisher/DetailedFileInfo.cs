namespace DominatorHouseCore.Models.SocioPublisher
{
    public class DetailedFileInfo
    {
        /// <summary>
        ///     To specify the Index id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     To specify the respetive file details value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id">Index id</param>
        /// <param name="value">Respetive file details value</param>
        public DetailedFileInfo(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}