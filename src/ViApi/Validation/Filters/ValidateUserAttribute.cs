using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ViApi.Contollers;
using ViApi.Services.Repository;
using ViApi.Types.API;

namespace ViApi.Validation.Filters
{
    [AttributeUsage(AttributeTargets.Method)]

    public class ValidateUserAttribute : TypeFilterAttribute
    {
        public ValidateUserAttribute(): base(typeof(ValidateUserFilter)) { }

        private class ValidateUserFilter : Attribute, IAsyncActionFilter
        {
            readonly IRepository _repository;
            public ValidateUserFilter(IRepository repository)
            {
                _repository = repository;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                Guid userGuid = context.HttpContext.GetGuidOrDefaultFromRequest();
                if (userGuid != default && (await _repository.IsUserExists(userGuid) is true))
                {
                    await next();
                }
                else
                {
                    context.Result = new ObjectResult(new ViApiResponse<Guid>(userGuid, false, "Переданный Guid определен как пустой."));
                }
            }
        }
    }
}
