
using UnityEngine;

namespace Game.CustomInput {
	public class LookHorizontal : MonoBehaviour {

		[Range(0.0f, 200.0f)]
		public float sensitivity = 100.0f;

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
			transform.Rotate(0, speed, 0, Space.Self);
		}

	}
}
