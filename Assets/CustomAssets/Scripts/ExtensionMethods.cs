
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
}
