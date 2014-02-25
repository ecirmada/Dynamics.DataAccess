using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Dynamics.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : Entity, new()
    {
        private readonly IOrganizationService organizationService;

        public Repository(IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        public T Get(Guid id, params Expression<Func<T, object>>[] include)
        {
            return Get(id, new ColumnSet(true), include);
        }

        public T Get(Guid id, ColumnSet columnSet, params Expression<Func<T, object>>[] include)
        {
            return Get(new T { Id = id }.ToEntityReference(), columnSet, include);
        }

        public T Get(EntityReference er, params Expression<Func<T, object>>[] include)
        {
            return Get(er, new ColumnSet(true), include);
        }

        public T Get(EntityReference er, ColumnSet columnSet, params Expression<Func<T, object>>[] include)
        {
            if (organizationService == null)
            {
                return null;
            }

            var e = organizationService.Retrieve(er.LogicalName, er.Id, columnSet).ToEntity<T>();
            if (include != null && include.Length > 0)
            {
                HydrateProperties(e, include);
            }
            return e;
        }

        public IEnumerable<T> Find(T e, params Expression<Func<T, object>>[] include)
        {
            if (organizationService == null)
            {
                return null;
            }

            var queryExpression = new QueryExpression(e.LogicalName);

            // Apply the condition operators to the attributes passed in
            foreach (var attributeName in e.Attributes.Keys)
            {
                ConditionExpression condition;
                var attribute = e.Attributes[attributeName];
                if (attribute is EntityReference)
                {
                    condition = new ConditionExpression(attributeName, ConditionOperator.Equal, (attribute as EntityReference).Id);
                }
                else if (attribute is OptionSetValue)
                {
                    condition = new ConditionExpression(attributeName, ConditionOperator.Equal, (attribute as OptionSetValue).Value);
                }
                else
                {
                    condition = new ConditionExpression(attributeName, ConditionOperator.Equal, attribute);
                }
                queryExpression.Criteria.AddCondition(condition);
            }
            queryExpression.ColumnSet = new ColumnSet(true);

            // If you want to page the data for large datasets you could add a take count parameter and
            // then limit the results returned. Not implemented by default, but leaving here for reference
            //if (takeCount > 0)
            //{
            //    queryExpression.PageInfo = new PagingInfo { Count = takeCount, PageNumber = 1 };
            //}

            // Execute and return the results
            List<T> entities = organizationService.RetrieveMultiple(queryExpression).Entities.Select(o => o.ToEntity<T>()).ToList();
            entities.ForEach(o => HydrateProperties(o, include));
            return entities;
        }

        public IEnumerable<T> FindByAttribute(string attributeName, object attributeValue, params Expression<Func<T, object>>[] include)
        {
            var entity = new T();
            entity[attributeName] = attributeValue;
            return Find(entity, include);
        }
        
        public IEnumerable<T> FindByRelationship(Guid parentId, string relationshipName, string parentKeyName, string childKeyName, params Expression<Func<T, object>>[] include)
        {
            if (organizationService == null)
            {
                return null;
            }
            var queryExpression = new QueryExpression
            {
                EntityName = (new T()).LogicalName,
                ColumnSet = new ColumnSet(true)
            };
            var linkEntity = new LinkEntity
            {
                LinkToEntityName = relationshipName,
                LinkToAttributeName = childKeyName,
                LinkFromEntityName = queryExpression.EntityName,
                LinkFromAttributeName = childKeyName
            };
            linkEntity.LinkCriteria.AddCondition(parentKeyName, ConditionOperator.Equal, parentId);
            queryExpression.LinkEntities.Add(linkEntity);
            List<T> entities = organizationService.RetrieveMultiple(queryExpression).Entities.Select(e => e.ToEntity<T>()).ToList();
            entities.ForEach(o => HydrateProperties(o, include));
            return entities;
        }
        
        public IQueryableRepository AsQueryable()
        {
            if (organizationService == null)
            {
                return null;
            }
            return new QueryableRepository(new OrganizationServiceContext(organizationService));
        }

        public void Create(T e)
        {
            if (organizationService == null)
            {
                return;
            }
            e.Id = organizationService.Create(e);
        }

        public void Update(T e)
        {
            if (organizationService == null)
            {
                return;
            }
            organizationService.Update(e);
        }

        public void Delete(T e)
        {
            if (organizationService == null)
            {
                return;
            }
            organizationService.Delete(e.LogicalName, e.Id);
        }

        public IEnumerable<T> FindByAttributeWithInclude(string attributeName, object attributeValue, params Expression[] include)
        {
            return FindByAttribute(attributeName, attributeValue, include.ToCorrectedExpressions<T>());
        }

        public IEnumerable<T> FindByRelationshipWithInclude(Guid parentId, string relationshipName, string parentKeyName, string childKeyName,
                                                               params Expression[] include)
        {
            return FindByRelationship(parentId, relationshipName, parentKeyName, childKeyName, include.ToCorrectedExpressions<T>());
        }

        public T GetWithInclude(EntityReference er, params Expression[] include)
        {
            return Get(er, include.ToCorrectedExpressions<T>());
        }

        public void HydrateProperties(T e, params Expression<Func<T, object>>[] include)
        {
            if (e == null)
            {
                return;
            }

            foreach (var expression in include)
            {
                ExpressionDetail expressionDetail = expression.ToExpressionDetail();
                if (expressionDetail != null)
                {
                    var propertyInfo = typeof (T).GetProperties().FirstOrDefault(p => p.Name == expressionDetail.PropertyName);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.PropertyType == typeof (Entity) || propertyInfo.PropertyType.BaseType == typeof (Entity))
                        {
                            // Get the child item using the property info
                            var child = GetChildByProperty(e, propertyInfo, expressionDetail.IncludeExpressions);
                            if (child != null)
                            {
                                propertyInfo.SetValue(e, child);
                            }
                        }
                        if (propertyInfo.PropertyType.GetInterfaces().Contains(typeof (IEnumerable)))
                        {
                            // Get the child results using the property info
                            var results = GetChildrenByProperty(e, propertyInfo, expressionDetail.IncludeExpressions);
                            if (results != null)
                            {
                                propertyInfo.SetValue(e, results);
                            }
                        }
                    }
                }
            }
        }

        private object GetChildByProperty(T entity, PropertyInfo propertyInfo, params Expression[] include)
        {
            // Don't accept null objects
            if (entity == null || propertyInfo == null)
            {
                return null;
            }

            // Ensure we have a lookup attribute decorator to allow us to discover the entity reference attribute on the entity
            var lookupAttribute = (LookupReferenceNameAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(LookupReferenceNameAttribute));
            if (lookupAttribute == null)
            {
                return null;
            }

            // Get the value of the entity reference field property that is set in the lookup reference attribute
            var allPropertyInfo = typeof(T).GetProperties();
            var lookupPropertyInfo = allPropertyInfo.First(p => p.Name == lookupAttribute.FieldName);
            var childReference = (EntityReference)lookupPropertyInfo.GetValue(entity);

            // If the child reference isn't set then we cannot hydrate!
            if (childReference == null)
            {
                return null;
            }

            // Create a concrete repository for the type of child object
            var concreteRepository = propertyInfo.PropertyType.CreateConcreteRepository(organizationService);

            // Invoke the GetByAttribute method to retrieve the list of related child items
            MethodInfo getMethod = concreteRepository.GetType().GetMethod("GetWithInclude");

            var parametersArray = new object[] { childReference, include };
            return getMethod.Invoke(concreteRepository, parametersArray);
        }

        private object GetChildrenByProperty(T entity, PropertyInfo propertyInfo, params Expression[] include)
        {
            // Don't accept null objects
            if (entity == null || propertyInfo == null)
            {
                return null;
            }

            // Ensure we have a foreign key attribute to allow us to discover the parent key attribute on the entity
            var foreignKeyAttribute = (ForeignKeyOnChildAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ForeignKeyOnChildAttribute));
            if (foreignKeyAttribute != null)
            {
                // Create a concrete repository for the type of child object
                Type templateType = propertyInfo.PropertyType.GenericTypeArguments[0];
                var concreteRepository = templateType.CreateConcreteRepository(organizationService);

                // Invoke the GetByAttribute method to retrieve the list of related child items
                MethodInfo findMethod = concreteRepository.GetType()
                    .GetMethod("FindByAttributeWithInclude");
                var parametersArray = new object[]
                                          {
                                              foreignKeyAttribute.LogicalName, 
                                              entity.Id, 
                                              include
                                          };
                return findMethod.Invoke(concreteRepository, parametersArray);
            }

            var relationshipLinkAttribute = (RelationshipLinkAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(RelationshipLinkAttribute));
            if (relationshipLinkAttribute != null)
            {
                // Create a concrete repository for the type of child object
                Type templateType = propertyInfo.PropertyType.GenericTypeArguments[0];
                var concreteRepository = templateType.CreateConcreteRepository(organizationService);

                // Invoke the GetByAttribute method to retrieve the list of related child items
                MethodInfo findMethod = concreteRepository.GetType()
                    .GetMethod("FindByRelationshipWithInclude");
                //MethodInfo findMethod = concreteRepository.GetType()
                //    .GetMethod("FindByRelationship", new[] { typeof(Guid), typeof(string), typeof(string), typeof(string) });
                var parametersArray = new object[]
                                          {
                                              entity.Id, 
                                              relationshipLinkAttribute.RelationshipName, 
                                              relationshipLinkAttribute.ParentKeyName,
                                              relationshipLinkAttribute.ChildKeyName, 
                                              include
                                          };
                return findMethod.Invoke(concreteRepository, parametersArray);
            }
            return null;
        }
    }

    public class ExpressionDetail
    {
        public string PropertyName { get; set; }
        public Expression[] IncludeExpressions { get; set; }
    }

    public class RelationshipLinkAttribute : Attribute
    {
        public string RelationshipName { get; set; }
        public string ParentKeyName { get; set; }
        public string ChildKeyName { get; set; }
    }

    public class ForeignKeyOnChildAttribute : Attribute
    {
        public string LogicalName { get; set; }
    }

    public class LookupReferenceNameAttribute : Attribute
    {
        public string FieldName { get; set; }
    }

    public static class EnumerableExtensions
    {
        public static object Include<T1>(this IEnumerable<T1> items, params Expression<Func<T1, object>>[] include)
        {
            return items;
        }
    }

    public static class EntityExtensions
    {
        public static object Include<T1>(this T1 e, params Expression<Func<T1, object>>[] include)
        {
            return e;
        }
    }

    public static class TypeExtensions
    {
        public static object CreateConcreteRepository(this Type type, IOrganizationService service)
        {
            var repositoryType = typeof(Repository<>);
            Type repositoryTypeWithGeneric = repositoryType.MakeGenericType(new[] { type });
            return Activator.CreateInstance(repositoryTypeWithGeneric, service);
        }

        //public static T As<T>(this Operator c) where T : struct
        //{
        //    return (T)Enum.Parse(typeof(T), c.ToString(), false);
        //}

    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, Object>>[] ToCorrectedExpressions<T>(this Expression[] expressions)
        {
            Expression<Func<T, object>>[] correctedExpressions = null;
            if (expressions != null)
            {
                correctedExpressions = expressions.Select(e => (Expression<Func<T, object>>)e).ToArray();
            }
            return correctedExpressions;
        }

        public static ExpressionDetail ToExpressionDetail<T>(this Expression<Func<T, object>> expression)
        {
            if (expression.Body as MemberExpression != null)
            {
                var memberExpression = expression.Body as MemberExpression;

                return new ExpressionDetail
                                           {
                                               PropertyName = memberExpression.Member.Name,
                                               IncludeExpressions = null
                                           };
            }
            if (expression.Body as MethodCallExpression != null)
            {
                var methodCallExpression = expression.Body as MethodCallExpression;
                var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
                Expression[] includeExpressions = null;
                if (methodCallExpression.Method.Name == "Include")
                {
                    //Expression operand = ((methodCallExpression.Arguments[1] as NewArrayExpression).Expressions[0] as UnaryExpression).Operand;



                    // TODO - Casting to the wrong generic. When the expression is parsed you are actually parsing something like this:
                    //
                    // Get(id, e1 => e1.Entity2.Include(e2 => e2.SomeOtherEntity))
                    //
                    // So what we actually need to is pull out e2, not e1 (e1 will be our main generic T... e2 is embedded... 
                    //    ... might require some reflection... meh...)
                    includeExpressions = (methodCallExpression.Arguments[1] as NewArrayExpression)
                        .Expressions.Select(
                            e => (e as UnaryExpression).Operand)
                            .ToArray();
                }



                // TODO - Using the wrong generic here! When the expression is parsed you are actually parsing something like this:
                //
                // Get(id, e1 => e1.Entity2.Include(e2 => e2.SomeOtherEntity))
                //
                // So what we actually need to is pull out e2, not e1 (e1 will be our main generic T... e2 is embedded... 
                //    ... might require some reflection... meh...)
                return new ExpressionDetail
                           {
                               PropertyName = memberExpression.Member.Name,
                               IncludeExpressions = includeExpressions
                           };
            }
            return null;
        }
    }
}