using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace HttpValuesMutator
{
    public class HttpValuesMutationFilter<TPreMutate, TPostMutate> : IResourceFilter, IActionFilter
    {
        private HashSet<string> PropertiesToMutate { get; set; }

        public HttpValuesMutationFilter()
        {
            PropertiesToMutate = HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>.GetPropertiesToMutate();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var resultValue = context.Result as ObjectResult;
            var valueAsJObject = JObject.FromObject(resultValue.Value);

            foreach (var property in valueAsJObject)
            {
                var propertyName = property.Key;
                if (PropertiesToMutate.Contains(propertyName))
                {
                    var mutator = HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>.GetResponseMutator(propertyName);
                    var outGoingVal = valueAsJObject[propertyName].ToObject<TPostMutate>();
                    var mutatedVal = mutator(outGoingVal);
                    valueAsJObject[propertyName] = JToken.FromObject(mutatedVal);
                }
            }

            ((ObjectResult)context.Result).Value = valueAsJObject;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var originalBody = new StreamReader(httpContext.Request.Body).ReadToEnd();
            var bodyAsJObject = JObject.Parse(originalBody);

            foreach (var property in bodyAsJObject)
            {
                var propertyName = property.Key;
                if (PropertiesToMutate.Contains(propertyName))
                {
                    var mutator = HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>.GetRequestMutator(propertyName);
                    var incomingVal = bodyAsJObject[propertyName].ToObject<TPreMutate>();
                    var mutatedVal = mutator(incomingVal);
                    bodyAsJObject[propertyName] = JToken.FromObject(mutatedVal);
                }
            }

            var serializedBody = JsonConvert.SerializeObject(bodyAsJObject);
            var newBodyContent = new StringContent(serializedBody, Encoding.UTF8, "application/json");
            var newStreamBody = newBodyContent.ReadAsStreamAsync().GetAwaiter().GetResult();

            httpContext.Request.Body = newStreamBody;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {}

        public void OnResourceExecuted(ResourceExecutedContext context)
        {}
    }
}
