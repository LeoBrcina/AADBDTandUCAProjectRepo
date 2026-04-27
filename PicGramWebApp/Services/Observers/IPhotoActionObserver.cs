namespace PicGramWebApp.Services.Observers
{
    public interface IPhotoActionObserver
    {
        Task UpdateAsync(PhotoActionEvent actionEvent);
    }
}