using HttpValuesMutator.Models;
using System;
using System.Collections.Generic;

namespace HttpValuesMutator
{
    public static class HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>
    {
        private static Dictionary<string, MutationObject<TPreMutate, TPostMutate>> Mutators { get; set; } =
            new Dictionary<string, MutationObject<TPreMutate, TPostMutate>>(StringComparer.InvariantCultureIgnoreCase);

        private static HashSet<string> PropertiesToMutate { get; set; } =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public static void SetMutator(string properyName, Func<TPreMutate, TPostMutate> onRequest,
            Func<TPostMutate, TPreMutate> onResponse)
        {
            var mutationObj = new MutationObject<TPreMutate, TPostMutate>
            {
                OnRequest = onRequest,
                OnResponse = onResponse
            };

            if (!Mutators.ContainsKey(properyName))
            {
                PropertiesToMutate.Add(properyName);
                Mutators.Add(properyName, mutationObj);

                return;
            }

            Mutators[properyName] = mutationObj;
        }

        internal static HashSet<string> GetPropertiesToMutate()
        {
            return PropertiesToMutate;
        }

        internal static Func<TPreMutate, TPostMutate> GetRequestMutator(string propertyName)
        {
            if (!Mutators.TryGetValue(propertyName, out var mutator))
            {
                return null;
            }

            return mutator.OnRequest;
        }

        internal static Func<TPostMutate, TPreMutate> GetResponseMutator(string propertyName)
        {
            if (!Mutators.TryGetValue(propertyName, out var mutator))
            {
                return null;
            }

            return mutator.OnResponse;
        }
    }
}
