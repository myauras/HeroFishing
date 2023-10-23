using System;
using System.Reflection;
using UnityEngine;
namespace Scoz.Func {
    public static class DebugUtils {
        public static string ObjToStr(object _obj) {
            if (_obj == null)
                return "null";

            Type type = _obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            FieldInfo[] fields = type.GetFields();
            string result = $"{type.Name} {{\n";

            foreach (var prop in properties) {
                result += $"  {prop.Name}: {prop.GetValue(_obj, null)},\n";
            }

            foreach (var field in fields) {
                result += $"  {field.Name}: {field.GetValue(_obj)},\n";
            }

            result += "}";

            return result;
        }
    }
}