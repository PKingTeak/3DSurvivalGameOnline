using UnityEngine;

public static class ValueChecker
{
    public static bool IsNull(object owner,object target,string variableName)
    {
        bool isNull = target switch
        {
            UnityEngine.Object unityObject => unityObject == null,
            null => true,
            _ => false
        };

        if (!isNull)
        {
            return false;
        }

        string className = owner?.GetType().Name ?? "Unknown";
        string objectName = GetObjectName(owner);

        Debug.LogError(
            $"[{objectName}/{className}] '{variableName}' 값이 NULL입니다.",
            owner as UnityEngine.Object
        );

        return true;
    }

    private static string GetObjectName(object owner)
    {
        if (owner is Component component)
        {
            return component.gameObject.name;
        }

        if (owner is GameObject gameObject)
        {
            return gameObject.name;
        }

        if (owner is UnityEngine.Object unityObject)
        {
            return unityObject.name;
        }

        return "Unknown";
    }
}