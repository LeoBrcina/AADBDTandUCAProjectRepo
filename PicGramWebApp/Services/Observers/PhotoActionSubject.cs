namespace PicGramWebApp.Services.Observers
{
    // Observer pattern: this subject notifies all registered observers whenever
    // an application action occurs (upload, edit, download, package change, etc.).
    public class PhotoActionSubject
    {
        private readonly List<IPhotoActionObserver> _observers;

        public PhotoActionSubject(IEnumerable<IPhotoActionObserver> observers)
        {
            _observers = observers.ToList();
        }

        public async Task NotifyAsync(PhotoActionEvent actionEvent)
        {
            foreach (var observer in _observers)
            {
                await observer.UpdateAsync(actionEvent);
            }
        }
    }
}