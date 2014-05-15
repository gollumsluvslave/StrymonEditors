using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.Core;

using RITS.StrymonEditor.Models;
namespace PresetStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            ODataRoute route = config.Routes.MapODataServiceRoute("odata", "odata", GetModel());
            route.MapODataRouteAttributes(config);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        public static IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<DBPreset>("Presets");
            builder.EntitySet<DBTag>("Tags");
            builder.EntitySet<DBParameter>("Parameters");
            builder.EntitySet<DBPresetTag>("PresetTags");
            

            builder.EntityType<DBPreset>().HasKey<int>(x => x.PresetId);
            builder.EntityType<DBTag>().HasKey<int>(x => x.TagId);
            builder.EntityType<DBParameter>().HasKey<int>(x => x.ParameterId);
            builder.EntityType<DBPresetTag>().HasKey<int>(x => x.PresetTagId);
            builder.AddComplexType(typeof(PresetMetadata)).AddCollectionProperty(typeof(PresetMetadata).GetProperty("Tags"));
            
            builder.AddComplexType(typeof(Tag));
            
            builder.Function("GetSalesTaxRate").Returns<double>().Parameter<string>("state");

            builder.Action("SearchForPresets")
                    .Returns<List<PresetMetadata>>()
                    .Parameter<PresetSearch>("criteria");

            builder.Action("UploadPreset")
                    .Parameter<StrymonXmlPreset>("uploadPreset");
            return builder.GetEdmModel();
        }
    }
}
