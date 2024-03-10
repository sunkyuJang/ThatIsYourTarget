using System;
using System.Reflection;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace JExtentioner
{
    public static class TypeExtentioner
    {
        public static string GetVariableName(this object variable)
        {
            var type = variable.GetType();
            var name = type.GetField(variable.ToString()).Name;

            return name;
        }
        public static string GetVariableName(System.Enum variable)
        {
            return variable.GetType().Name;
        }

        public static string PrintEnumVariableName<T>(T enumVariable)
        {
            return nameof(enumVariable);
        }

        private static string GetVariableName(Type type, object variable)
        {
            // 필드 정보 가져오기
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // 필드 정보에서 변수 이름 찾기
            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(variable) == variable)
                {
                    return field.Name;
                }
            }

            // 열거형 멤버를 확인 (열거형에 특화된 검사)
            if (type.IsEnum)
            {
                string[] names = type.GetEnumNames();
                foreach (string name in names)
                {
                    if (Enum.Parse(type, name).Equals(variable))
                    {
                        return name;
                    }
                }
            }

            // 재귀적으로 상위 클래스 검사
            if (type.BaseType != null)
            {
                return GetVariableName(type.BaseType, variable);
            }

            return string.Empty;
        }
    }
}
