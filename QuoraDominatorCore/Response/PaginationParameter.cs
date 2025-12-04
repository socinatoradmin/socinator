namespace QuoraDominatorCore.Response
{
    public class PaginationParameter
    {
        public string PaginationID { get; set; }
        public string EndCursorPosition { get; set; }
        public bool HasNextPage { get; set; }
    }
}
