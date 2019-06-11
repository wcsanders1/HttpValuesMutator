using System;

namespace HttpValuesMutator.Models
{
    public class MutationObject<TPreMutate, TPostMutate>
    {
        public Func<TPreMutate, TPostMutate> OnRequest { get; set; }
        public Func<TPostMutate, TPreMutate> OnResponse { get; set; }
    }
}
