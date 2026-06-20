namespace KarachiEstateHub.ViewModels
{
    public class SampleDataStatusViewModel
    {
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public int TotalSampleProperties { get; set; }
        public int ActiveVisibleSampleProperties { get; set; }
        public int PendingSampleProperties { get; set; }
        public int DraftSampleProperties { get; set; }
        public int RejectedSampleProperties { get; set; }
        public int MissingUserReferences { get; set; }
        public int MissingTypeReferences { get; set; }
        public int MissingLocationReferences { get; set; }
    }
}
