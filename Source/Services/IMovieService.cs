namespace MovieWebApplication.Services
{
    public interface IMovieService
    {
        public Task<TResult> GetAndDeserializeAsync<TResult>(string endpoint);
    }
}