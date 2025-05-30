using Microsoft.EntityFrameworkCore;
using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces.Repositories.Gamestore;
using Storefy.Services.Data;

namespace Storefy.Services.Repositories.Gamestore;

/// <summary>
/// Repository to manage the operations of Platform entities using a database context.
/// </summary>
/// <inheritdoc cref="IPlatformRepository{T}"/>
public class PlatformRepository : IPlatformRepository<Platform>
{
    private readonly StorefyDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The data base context
    /// to be used by the repository.</param>
    public PlatformRepository(StorefyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Platform> Add(CreateUpdatePlatformDto platformDto)
    {
        var platformExist = await _dbContext.Platforms
            .AnyAsync(p => p.Type == platformDto.Platform.Type);

        if (!platformExist)
        {
            var createdPlatform = new Platform
            {
                Id = Guid.NewGuid().ToString(),
                Type = platformDto.Platform.Type,
            };

            await _dbContext.AddAsync(createdPlatform);
            await _dbContext.SaveChangesAsync();

            return createdPlatform;
        }

        throw new InvalidOperationException("This platform already exist!");
    }

    /// <inheritdoc />
    public async Task<Platform> Delete(Platform entity)
    {
        _dbContext.Platforms.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    /// <inheritdoc />
    public async Task<Platform> GetById(string id)
    {
        var platform = await _dbContext.Platforms
            .FirstOrDefaultAsync(p => p.Id == id);

        return platform ??
            throw new InvalidOperationException("This platform doesn't exist!");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetAll()
    {
        return await _dbContext.Platforms.ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Platform> Update(CreateUpdatePlatformDto platformDto)
    {
        var updatedPlatform = await _dbContext.Platforms
            .FindAsync(platformDto.Platform.Id);

        if (updatedPlatform != null)
        {
            var outdatedPlatform = new Platform
            {
                Id = updatedPlatform.Id,
                Type = updatedPlatform.Type,
                Games = updatedPlatform.Games,
            };
            updatedPlatform.Type = platformDto.Platform.Type;
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(outdatedPlatform);
            return updatedPlatform;
        }

        throw new InvalidOperationException("Cannot update this platform.");
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetPlatformsByGameAlias(string gameAlias, string languageCode)
    {
        var selectedPlatform = await _dbContext.Platforms
            .Include(g => g.Games)
            .Where(g => g.Games
            .Any(game => game.Key
            .Equals(gameAlias)))
            .ToListAsync();

        if (languageCode != "en")
        {
            var platformTranslations = await _dbContext.PlatformTranslations
                .Where(gt => gt.Language.LanguageCode == languageCode)
                .ToListAsync();

            foreach (var platform in selectedPlatform)
            {
                var translation = platformTranslations.FirstOrDefault(pt => pt.PlatformId == platform.Id);

                if (translation != null)
                {
                    platform.Type = translation.Type;
                }
            }
        }

        return selectedPlatform;
    }
}
