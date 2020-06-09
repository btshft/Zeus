using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Shared.Validation
{
    internal class CompositeValidationResult : ValidationResult
    {
        public CompositeValidationResult(string errorMessage)
            : base(errorMessage) { }

        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames) { }

        protected CompositeValidationResult(ValidationResult validationResult)
            : base(validationResult) { }

        public List<ValidationResult> InnerResults { get; }
            = new List<ValidationResult>();
    }
}