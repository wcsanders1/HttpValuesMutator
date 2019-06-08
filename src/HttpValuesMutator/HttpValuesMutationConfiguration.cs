using System;
using System.Collections.Generic;

namespace HttpValuesMutator
{
    public static class HttpBodyMutatorConfiguration<TPreMutate, TPostMutate>
    {
        private static Dictionary<string, MutationObject> Mutators { get; set; } =
            new Dictionary<string, MutationObject>();

        public static void SetMutator(string properyName, Func<TPreMutate, TPostMutate> onRequest,
            Func<TPostMutate, TPreMutate> onResponse)
        {
            var mutationObj = new MutationObject
            {
                OnRequest = onRequest,
                OnResponse = onResponse
            };

            if (!Mutators.ContainsKey(properyName))
            {
                Mutators.Add(properyName, mutationObj);

                return;
            }

            Mutators[properyName] = mutationObj;
        }

        public static Func<TPreMutate, TPostMutate> GetRequestMutator(string propertyName)
        {
            if (!Mutators.TryGetValue(propertyName, out var mutator))
            {
                return null;
            }

            return mutator.OnRequest;
        }

        public static Func<TPostMutate, TPreMutate> GetResponseMutator(string propertyName)
        {
            if (!Mutators.TryGetValue(propertyName, out var mutator))
            {
                return null;
            }

            return mutator.OnResponse;
        }

        private class MutationObject
        {
            public Func<TPreMutate, TPostMutate> OnRequest { get; set; }
            public Func<TPostMutate, TPreMutate> OnResponse { get; set; }
        }
    }
}
