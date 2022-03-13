using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class RegisteredFragmentAttribute : Attribute {
  public RegisteredFragmentAttribute(float spawnWeight = 1) {
    SpawnWeight = spawnWeight;
  }
  public Type type;

  public float SpawnWeight { get; }
  private static List<RegisteredFragmentAttribute> cachedTypes;

  public static List<RegisteredFragmentAttribute> GetAllFragmentTypes<T>() where T : Fragment {
    if (cachedTypes == null) {
      cachedTypes = GetRegisteredTypesImpl().ToList();
      UnityEngine.Debug.Log(String.Join(", ", cachedTypes.Select(t => t.type.FullName)));
    }
    return cachedTypes.Where(attr => attr.type.IsSubclassOf(typeof(T))).ToList();
  }

  private static IEnumerable<RegisteredFragmentAttribute> GetRegisteredTypesImpl() {
    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    foreach (var assembly in assemblies) {
      foreach (Type type in assembly.GetTypes()) {
        var attributes = type.GetCustomAttributes(typeof(RegisteredFragmentAttribute), true);
        if (attributes.Length > 0) {
          var regAttribute = attributes[0] as RegisteredFragmentAttribute;
          regAttribute.type = type;
          yield return regAttribute;
        }
      }
    }
  }
}