using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Zeus.Shared.Json
{
    public sealed class FromNewtonsoftJsonBodyAttribute : ModelBinderAttribute
    {
        public FromNewtonsoftJsonBodyAttribute() 
            : base(typeof(NewtonsoftJsonModelBinder))
        {
            BindingSource = BindingSource.Body;
        }
    }
}
