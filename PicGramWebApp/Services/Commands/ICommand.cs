namespace PicGramWebApp.Services.Commands
{
    public interface ICommand<T>
    {
        Task<T> ExecuteAsync();
    }
}