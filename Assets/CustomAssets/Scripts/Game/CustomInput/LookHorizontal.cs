
using UnityEngine;
using Game.PlayerComponents;

namespace Game.CustomInput {
	public class LookHorizontal : MonoBehaviour {

		[Range(0.0f, 200.0f)]
		public float Sensitivity = 100.0f;

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Sensitivity;
			transform.Rotate(0, speed, 0, Space.Self);
		}

	}
}
