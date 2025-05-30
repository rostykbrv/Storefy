using Storefy.BusinessObjects.Dto;
using Storefy.BusinessObjects.Models.GameStoreSql;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;

namespace Storefy.Services.Services;

/// <summary>
/// Implementation of the services needed to manipulate and retrieve information for Platform entities in the repository.
/// </summary>
/// <inheritdoc cref="IPlatformService"/>
public class PlatformService : IPlatformService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used by the platform repository.</param>
    public PlatformService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Platform> AddPlatform(CreateUpdatePlatformDto platformDto)
    {
        return await _unitOfWork.PlatformRepository.Add(platformDto);
    }

    /// <inheritdoc />
    public async Task<Platform> DeletePlatform(string id)
    {
        var platform = await _unitOfWork.PlatformRepository.GetById(id);

        return await _unitOfWork.PlatformRepository.Delete(platform);
    }

    /// <inheritdoc />
    public async Task<Platform> GetPlatformById(string platformId)
    {
        return await _unitOfWork.PlatformRepository.GetById(platformId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return await _unitOfWork.PlatformRepository.GetAll();
    }

    /// <inheritdoc />
    public async Task<Platform> UpdatePlatform(CreateUpdatePlatformDto platformDto)
    {
        return await _unitOfWork.PlatformRepository.Update(platformDto);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Platform>> GetPlatformsByGame(string gameAlias, string languageCode)
    {
        return await _unitOfWork.PlatformRepository.GetPlatformsByGameAlias(gameAlias, languageCode);
    }
}
