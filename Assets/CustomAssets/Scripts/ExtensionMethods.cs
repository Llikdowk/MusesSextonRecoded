
using UnityEngine;

public static class ExtensionMethods {

	/// <summary>
	/// Sets local position and rotation to 0
	/// </summary>
	/// <param name="transform"></param>
	public static void LocalReset(this Transform transform) {
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	/// <summary>
	/// Tries to get the component. It is added if it cannot be found. 
	/// </summary>
	/// <typeparam name="ComponentType"></typeparam>
	/// <param name="gameObject"></param>
	/// <returns></returns>
	public static ComponentType GetOrAddComponent<ComponentType>(this GameObject gameObject) where ComponentType : Component {
		ComponentType component = gameObject.GetComponent<ComponentType>() ?? gameObject.AddComponent<ComponentType>();
		return component;
	}
}
