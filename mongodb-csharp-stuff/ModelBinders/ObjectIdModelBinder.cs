namespace Mcs.ModelBinders
{
    using System.Diagnostics;
    using System.Web.Http.Controllers;
    using System.Web.Http.ModelBinding;

    using MongoDB.Bson;

    public class ObjectIdModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
   
            if (bindingContext.ModelType != typeof(ObjectId))
            {
                return false;
            }

            var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (val == null)
            {
                return false;
            }

            var id = val.RawValue as string;

            if (id == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, "Wrong value type");

                return false;
            }

            ObjectId result;

            if (ObjectId.TryParse(id, out result))
            {
                bindingContext.Model = result;
                return true;
            }

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert value to ObjectId");

            return false;
        }
    }
}
