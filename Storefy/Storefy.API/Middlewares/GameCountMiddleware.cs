using Storefy.Interfaces.Services;

namespace Storefy.API.Middlewares;

/// <summary>
/// Middleware count games and add header to responce on each request with total number of games.
/// </summary>
public class GameCountMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameCountMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate for the next piece of middleware.</param>
    public GameCountMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invoke middleware for processing.
    /// This method gets game service from request service, fetches total count of games
    /// and append it to the HTTP response header before processing next middleware.
    /// </summary>
    /// <param name="context">The HttpContext for the current request and response.</param>
    /// <returns>A System.Threading.Tasks.Task for Middleware processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var gameService = context.RequestServices.GetService<IGameService>();
        var allGames = await gameService.GetAll();
        var gamesCount = allGames.Count();
        context.Response.Headers.Add("x-total-numbers-of-games", gamesCount.ToString());
        await _next(context);
    }
}
