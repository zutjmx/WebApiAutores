using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace;
            var versionApi = nameSpaceController.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
