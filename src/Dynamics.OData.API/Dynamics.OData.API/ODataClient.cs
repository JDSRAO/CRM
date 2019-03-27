using Dynamics.OData.API.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;
using Xrm.Tools.WebAPI.Results;

namespace Dynamics.OData.API
{
    public class ODataClient
    {
        string CrmApiUrl { get; set; }
        CRMWebAPI ApiClient { get; set; }
        string AccessToken { get; set; }

        public ODataClient(string crmUrl, string accessToken, double crmVersion = 9.1)
        {
            this.CrmApiUrl = $"{crmUrl}/api/data/v{crmVersion}/";
            AccessToken = accessToken;
            ApiClient = new CRMWebAPI(CrmApiUrl, this.AccessToken);
        }

        public async Task<ExpandoObject> Get(string entityCollectionName, Guid recordId, CRMGetListOptions QueryOptions = null)
        {
            var result = await ApiClient.Get(entityCollectionName, recordId, QueryOptions);
            return result;
        }

        public async Task<T> Get<T>(string entityCollectionName, Guid recordId, CRMGetListOptions QueryOptions = null)
        {
            var result = await ApiClient.Get<T>(entityCollectionName, recordId, QueryOptions);
            return result;
        }

        public async Task<CRMGetListResult<ExpandoObject>> GetList(string url, CRMGetListOptions QueryOptions = null)
        {
            var result = await ApiClient.GetList(url, QueryOptions);
            return result;
        }

        public async Task<Guid> Create(string entityCollectionName, object data)
        {
            var id = await ApiClient.Create(entityCollectionName, data);
            return id;
        }

        public async Task<Guid> Create<T>(T data) where T : CrmEntity
        {
            var crmData = CreateCrmRecordModelFromPropertyAttributes(data);
            var id = await ApiClient.Create(data.EntityName, crmData);
            return id;
        }

        public async Task<List<CRMBatchResultItem>> Create(string entityCollectionName, object[] data)
        {
            var recordIds = await ApiClient.Create(entityCollectionName, data);
            return recordIds.ResultItems;
        }

        public static IDictionary<string, object> CreateCrmRecordModelFromProperties(params Property[] properties)
        {
            var crmRecord = new ExpandoObject() as IDictionary<string, object>;
            JArray parties = new JArray();
            Property partyListField = null;
            foreach (var property in properties)
            {
                switch (property.Type)
                {
                    case Models.Type.LookUp:
                    case Models.Type.Other:
                        crmRecord[property.Name] = property.CrmValue;
                        break;
                    case Models.Type.PartyListItem:
                        var party = Property.CreatePartyListFieldItem(property.Name, property.CrmValue.ToString(), property.TypeMask);
                        parties.Add(party);
                        break;
                    case Models.Type.PartyList:
                        partyListField = property;
                        break;
                    default:
                        break;
                }
            }

            if (partyListField != null)
            {
                crmRecord[partyListField.Name] = parties;
            }

            return crmRecord;
        }

        public static IDictionary<string, object> CreateCrmRecordModelFromPropertyAttributes(object model)
        {
            var modelProperties = GetPropertyInfo(model);
            var crmRecord = CreateCrmRecordModelFromProperties(modelProperties.ToArray());
            return crmRecord;
        }

        private static List<Property> GetPropertyInfo(object model)
        {
            PropertyInfo[] modelProperties;
            List<Property> properties = new List<Property>();
            var flags = BindingFlags.Public | BindingFlags.Instance;
            modelProperties = model.GetType().GetProperties(flags);
            foreach (var property in modelProperties)
            {
                if (property.CanRead && property.GetValue(model) != null)
                {
                    var attributeInfo = property.GetCustomAttribute<PropertyAtribute>();
                    if (attributeInfo != null)
                    {
                        var propertyInfo = new Property(attributeInfo.Name, property.GetValue(model), attributeInfo.PropertyType, attributeInfo.EntityPluralName, attributeInfo.IsMappedToMultipleEntities, attributeInfo.EntityName, attributeInfo.TypeMask);
                        properties.Add(propertyInfo);
                    }
                }
            }

            return properties;
        }
    }
}
