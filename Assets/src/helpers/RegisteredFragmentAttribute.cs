using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class RegisteredFragmentAttribute : Attribute {
  private static List<Type> cachedTypes;
  public static List<Type> GetAllFragmentTypes() {
    if (cachedTypes == null) {
      cachedTypes = GetRegisteredTypesImpl().ToList();
      UnityEngine.Debug.Log(String.Join(", ", cachedTypes.Select(t => t.FullName)));
    }
    return cachedTypes;
  }

  private static IEnumerable<Type> GetRegisteredTypesImpl() {
    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    foreach (var assembly in assemblies) {
      foreach (Type type in assembly.GetTypes()) {
        if (type.GetCustomAttributes(typeof(RegisteredFragmentAttribute), true).Length > 0) {
          yield return type;
        }
      }
    }
  }
}