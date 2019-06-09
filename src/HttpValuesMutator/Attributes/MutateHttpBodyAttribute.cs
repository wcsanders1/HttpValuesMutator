using System;

namespace HttpValuesMutator.Attributes
{
    public class MutateHttpBodyAttribute : Attribute
    {
        public Type IncomingType { get; set; }
        public Type OutgoingType { get; set; }
    }
}
