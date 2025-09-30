using System.Web.Script.Serialization;using System.Collections.Generic;using System.Collections;using System.Linq;using System;

namespace ColpatriaSAI.Negocio.Componentes
{
//    class EF2Javascript
//    {
        
//private static readonly Type[] _builtInTypes = new[]{
//typeof(bool),
//typeof(byte),
//typeof(sbyte),
//typeof(char),
//typeof(decimal),
//typeof(double),
//typeof(float),
//typeof(int),
//typeof(uint),
//typeof(long),
//typeof(ulong),
//typeof(short),
//typeof(ushort),
//typeof(string),
//typeof(DateTime),
//typeof(Guid),
//typeof(bool?),
//typeof(byte?),
//typeof(sbyte?),
//typeof(char?),
//typeof(decimal?),
//typeof(double?),
//typeof(float?),
//typeof(int?),
//typeof(uint?),
//typeof(long?),
//typeof(ulong?),
//typeof(short?),
//typeof(ushort?),
//typeof(DateTime?),
//typeof(Guid?)
//};
//public static IDictionary Serialize(object TargetObject,int currentDepth=1, List alreadyProccededObjects=null, int maxDepth=2)
//{
//if (alreadyProccededObjects == null) { alreadyProccededObjects = new List(); }
//alreadyProccededObjects.Add(TargetObject.GetHashCode());
//Type type = TargetObject.GetType();
//var properties = from p in type.GetProperties()
//where p.CanWrite &&
//_builtInTypes.Contains(p.PropertyType)
//select p;
//var result = properties.ToDictionary(
//property => property.Name,
//property => property.GetValue(TargetObject, null) == null ? "" : property.GetValue(TargetObject, null)
//);
//if (maxDepth > currentDepth)
//{
//var complexProperties = from p in type.GetProperties()
//where p.CanWrite &&
//p.CanRead &&
//!_builtInTypes.Contains(p.PropertyType) &&
//!alreadyProccededObjects.Contains(p.GetValue(TargetObject, null)== null ? 0 : p.GetValue(TargetObject, null).GetHashCode())
//select p;
//foreach (var property in complexProperties)
//{
//var js = new System.Web.Script.Serialization.JavaScriptSerializer();
//Dictionary newObj = (Dictionary)EFObjectConverter.Serialize(property.GetValue(TargetObject, null), currentDepth + 1, alreadyProccededObjects);
//result.Add(property.Name, newObj);
//}
//}
//return result;
    //}
    //}
    public class Ef2Javascript : JavaScriptConverter
    {
        private int _currentDepth = 1;
        private readonly int _maxDepth = 2;
        private readonly List<int> _processedObjects = new List<int>();
        private readonly Type[] _builtInTypes = new[]
                                                  {
                                                      typeof(bool),    
                                                      typeof(byte), 
                                                      typeof(sbyte), 
                                                      typeof(char), 
                                                      typeof(decimal),
                                                      typeof(double), 
                                                      typeof(float), 
                                                      typeof(int),  
                                                      typeof(uint), 
                                                      typeof(long), 
                                                      typeof(ulong), 
                                                      typeof(short),
                                                      typeof(ushort),
                                                      typeof(string),
                                                      typeof(DateTime),
                                                      typeof(Guid)
                                                  };
        public Ef2Javascript(int maxDepth = 2,
          Ef2Javascript parent = null)
      {
          _maxDepth = maxDepth;   
          if (parent != null)
          {
              _currentDepth += parent._currentDepth;
          }
      }  
      public override object Deserialize( IDictionary<string,object> dictionary, Type type, JavaScriptSerializer serializer)
      {
          return null;
      }    
      public override IDictionary<string,object> Serialize(object obj, JavaScriptSerializer serializer)
      {
          _processedObjects.Add(obj.GetHashCode()); 
          Type type = obj.GetType(); 
          var properties = from p in type.GetProperties()
                           where p.CanWrite &&
                           p.CanWrite &&
                           _builtInTypes.Contains(p.PropertyType)
                           select p;    var result = properties.ToDictionary( property => property.Name,
                               property => (Object)(property.GetValue(obj, null) == null 
                                   ? "":  property.GetValue(obj, null).ToString().Trim()));
          if (_maxDepth >= _currentDepth)
          {
              var complexProperties = from p in type.GetProperties() 
                                      where p.CanWrite &&
                                      p.CanRead &&
                                      !_builtInTypes.Contains(p.PropertyType) &&
                                      !_processedObjects.Contains(p.GetValue(obj, null) 
                                      == null ? 0 
                                      : p.GetValue(obj, null).GetHashCode()) 
                                      select p;
              foreach (var property in complexProperties) 
              {        var js = new JavaScriptSerializer();   
                  js.RegisterConverters(new List<JavaScriptConverter>
                                            {
                                                new Ef2Javascript(_maxDepth - _currentDepth, this)
                                            });        result.Add(property.Name, js.Serialize(property.GetValue(obj, null)));      }
          }    return result;
      }  public override IEnumerable<System.Type> SupportedTypes
      {
          get
          {
              return GetType().Assembly.GetTypes();
          }
      }
  }

    }

