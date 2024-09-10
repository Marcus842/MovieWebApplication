namespace MovieWebApplication.Models
{
    public class HomeViewModel
    {
        public OmdbResponseModel? ResponseModel { get; set; }
        public string PageIndex { get; set; }
        public string MovieTitle { get; set; }
    }
}
