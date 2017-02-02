
using UnityEngine;

namespace Game.CustomInput {
	public class LookVertical : MonoBehaviour {

		[Range(0.0f, 100.0f)]
		public float sensitivity = 75.0f;

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

			transform.Rotate(-speed, 0, 0, Space.Self);
		}

	}
}
