using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpValuesMutator
{
    public class HttpValuesMutationFilter<TPreMutate, TPostMutate> : IResourceFilter, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {}

        public void OnResourceExecuted(ResourceExecutedContext context)
        {}

        
    }
}
