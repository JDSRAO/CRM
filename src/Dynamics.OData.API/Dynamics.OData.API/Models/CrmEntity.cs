using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics.OData.API.Models
{
    public abstract class CrmEntity
    {
        public string EntityName { get; }

        public CrmEntity(string entityName)
        {
            EntityName = entityName;
        }
    }
}
