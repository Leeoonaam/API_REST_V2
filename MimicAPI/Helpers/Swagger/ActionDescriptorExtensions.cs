﻿using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace MimicAPI.Helpers.Swagger
{
    public static class ActionDescriptorExtensions
    {
        public static ApiVersionModel GetApiVersion(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor?.Properties.Where((kvp) => ((Type)kvp.Key).Equals(typeof(ApiVersionModel))).Select(kvp => kvp.Value as ApiVersionModel).FirstOrDefault();
        }
    }
}
