using System;
using System.Collections.Specialized;

namespace ColpatriaSAI.Seguridad.Proveedores
{
  sealed internal class ProviderUtility
  {
    /// <summary>
    /// traer la default nombre de applicacion 
    /// </summary>
    /// <returns></returns>
    internal static string GetDefaultAppName()
    {
      string defPath;
      try
      {
        string vPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
        if (string.IsNullOrEmpty(vPath))
        {
          vPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
          int num1 = vPath.IndexOf('.');
          if (num1 != -1)
          {
            vPath = vPath.Remove(num1);
          }
        }
        if (string.IsNullOrEmpty(vPath))
        {
          return "/";
        }
        defPath = vPath;
      }
      catch
      {
        defPath = "/";
      }
      return defPath;
    }

    /// <summary>
    /// devolver un valor de una colección de un valor predeterminado si no esta en la colección 
   /// </summary>
    /// <param name="config">la colección</param>
    /// <param name="valueName">the value a traer</param>
    /// <param name="defaultValue">el valor default</param>
    /// <returns>un valor de la colección o el valor predeterminado</returns>
    internal static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
    {
      bool result;
      string valueToParse = config[valueName];
      if (valueToParse == null)
      {
        return defaultValue;
      }
      if (bool.TryParse(valueToParse, out result))
      {
        return result;
      }
      throw new Exception("Value must be boolean");
    }

    /// <summary>
    /// devolver un valor de una colección de un valor predeterminado si no esta en la colección
    /// </summary>
    /// <param name="config">la colección</param>
    /// <param name="valueName">el nombre del valor en la colección</param>
    /// <param name="defaultValue">el valor default</param>
    /// <param name="zeroAllowed">si el cero es permitido</param>
    /// <param name="maxValueAllowed">cual els el valor mas largo permitido</param>
    /// <returns>a value</returns>
    internal static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
    {
      int result;
      string valueToParse = config[valueName];
      if (valueToParse == null)
      {
        return defaultValue;
      }
      if (!int.TryParse(valueToParse, out result))
      {
        if (zeroAllowed)
        {
          throw new Exception("Value must be non negative integer");
        }
        throw new Exception("Value must be positive integer");
      }
      if (zeroAllowed && (result < 0))
      {
        throw new Exception("Value must be non negative integer");
      }
      if (!zeroAllowed && (result <= 0))
      {
        throw new Exception("Value must be positive integer");
      }
      if ((maxValueAllowed > 0) && (result > maxValueAllowed))
      {
        throw new Exception("Value too big");
      }
      return result;
    }
  }
}
