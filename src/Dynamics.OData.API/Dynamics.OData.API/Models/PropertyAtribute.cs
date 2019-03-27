using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics.OData.API.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyAtribute : Attribute
    {
        public string Name => name;
        public Type PropertyType => type;
        public string EntityPluralName => entityPluralName;
        public bool IsMappedToMultipleEntities => isMappedToMultipleEntities;
        public string EntityName => entityName;
        public int TypeMask => typeMask;


        private string name { get; set; }
        private Type type { get; set; }
        private string entityPluralName { get; set; }
        private bool isMappedToMultipleEntities { get; set; }
        private string entityName { get; set; }
        private int typeMask { get; set; }



        public PropertyAtribute(string name, Type propertyType, string entityPluralName = null, bool isMappedToMultipleEntities = false, string entityName = null, int typeMask = 1)
        {
            this.name = name;
            this.type = propertyType;
            this.entityPluralName = entityPluralName;
            this.isMappedToMultipleEntities = isMappedToMultipleEntities;
            this.entityName = entityName;
            this.typeMask = typeMask;
        }
    }
}
