using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using Newtonsoft.Json;

namespace VelzonModerna.Services
{

    public interface IPaginationService
    {
        PagedResult<T> AddPaginationMetadata<T>(PagedResult<T> models) where T : class;
    }

    public class PaginationService : IPaginationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaginationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public PagedResult<T> AddPaginationMetadata<T>(PagedResult<T> models) where T : class
        {
            var httpResponse = _httpContextAccessor?.HttpContext?.Response;

            var metadata = new
            {
                models.TotalResults,
                models.PageIndex,
                models.PageSize,
                models.TotalPages,
                models.HasPrevious,
                models.HasNext
            };

            httpResponse?.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            httpResponse?.Headers.Add("X-Pagination-Result", $"Retornando {models.TotalResults} Registros do Banco de Dados.");

            return models;
        }
    }
}
