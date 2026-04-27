using PicGramWebApp.Models;

namespace PicGramWebApp.Services.Search
{
    // Search service: separates query execution from the controller so that
    // the Builder creates criteria and this service applies them to the data source.
    public interface IPhotoSearchService
    {
        List<Photo> Search(PhotoSearchCriteria criteria);
    }
}