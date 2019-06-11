using HttpValuesMutator.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Console.WriteLine("OnActionExecuted");

            var attributes = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes;
            var mutationAttribute = attributes.FirstOrDefault(a => a.AttributeType.Name == nameof(MutateHttpBodyAttribute));
            if (mutationAttribute == null)
            {
                return;
            };

            foreach (var mutationType in mutationAttribute.NamedArguments)
            {
                switch (mutationType.MemberName)
                {
                    case nameof(MutateHttpBodyAttribute.IncomingType):
                        if (((Type)mutationType.TypedValue.Value).Name != typeof(TPreMutate).Name)
                        {
                            return;
                        }
                        break;
                    case nameof(MutateHttpBodyAttribute.OutgoingType):
                        if (((Type)mutationType.TypedValue.Value).Name != typeof(TPostMutate).Name)
                        {
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }

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
            Console.WriteLine("OnResourceExecuting");

            var attributes = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.CustomAttributes;
            var mutationAttribute = attributes.FirstOrDefault(a => a.AttributeType.Name == nameof(MutateHttpBodyAttribute));
            if (mutationAttribute == null)
            {
                return;
            };

            foreach (var mutationType in mutationAttribute.NamedArguments)
            {
                switch (mutationType.MemberName)
                {
                    case nameof(MutateHttpBodyAttribute.IncomingType):
                        if (((Type)mutationType.TypedValue.Value).Name != typeof(TPreMutate).Name)
                        {
                            return;
                        }
                        break;
                    case nameof(MutateHttpBodyAttribute.OutgoingType):
                        if (((Type)mutationType.TypedValue.Value).Name != typeof(TPostMutate).Name)
                        {
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }

            var pathArgs = context.RouteData.Values;
            foreach (var pathArg in pathArgs)
            {
                var propertyName = pathArg.Key;
                if (PropertiesToMutate.Contains(propertyName))
                {
                    var incomingVal = (TPreMutate)pathArg.Value;
                    var mutator = HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>.GetRequestMutator(propertyName);
                    var mutatedVal = mutator(incomingVal);
                    context.RouteData.Values[propertyName] = mutatedVal;
                }
            }

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
        {
            Console.WriteLine("OnActionExecuting");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine("OnResourceExecuted");
        }
    }
}
