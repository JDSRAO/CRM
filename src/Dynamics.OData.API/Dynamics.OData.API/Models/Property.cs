using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamics.OData.API.Models
{
    public class Property
    {
        public string Name { get; }

        public Type Type { get; }

        public int TypeMask { get; }

        public object CrmValue => crmValue;

        private object crmValue { get; set; }

        public Property(string name, object value, Type propertyType, string entityPluralName = null, bool isMappedToMultipleEntities = false, string entityName = null, int typeMask = 1)
        {
            Type = propertyType;
            switch (propertyType)
            {
                case Type.LookUp:
                    if (isMappedToMultipleEntities)
                    {
                        if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityPluralName))
                        {
                            throw new ArgumentNullException("Entity name or entity plural name is missing");
                        }
                        else
                        {
                            Name = $"{name}_{entityName}@odata.bind";
                        }
                    }
                    else
                    {
                        Name = $"{name}@odata.bind";
                    }

                    if (string.IsNullOrEmpty(entityPluralName))
                    {
                        throw new ArgumentNullException("Entity plural name missing");
                    }
                    crmValue = $"/{entityPluralName}({value})";

                    break;
                case Type.PartyListItem:
                    if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityPluralName))
                    {
                        throw new ArgumentNullException("Entity name or entity plural name is missing");
                    }
                    else
                    {
                        Name = $"partyid_{entityName}@odata.bind"; ;
                        crmValue = $"/{entityPluralName}({value})";
                    }
                    TypeMask = typeMask;
                    break;
                case Type.PartyList:
                    Name = name;
                    crmValue = value;
                    break;
                case Type.Other:
                    Name = name;
                    crmValue = value;
                    break;
                default:
                    throw new InvalidOperationException("CRM type not implemented");
            }
        }

        public static JObject CreatePartyListFieldItem(string key, string value, int typeMask = 1)
        {
            JObject party = new JObject();
            party[key] = value;
            party["participationtypemask"] = typeMask;
            return party;
        }

    }

    public enum Type
    {
        LookUp,
        PartyListItem,
        PartyList,
        Other
    }
}
