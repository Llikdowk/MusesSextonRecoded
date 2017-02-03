
using UnityEngine;

namespace Game.CustomInput {
	public class LookVertical : MonoBehaviour {

		[Range(0.0f, 200.0f)]
		public float sensitivity = 100.0f;

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

			transform.Rotate(-speed, 0, 0, Space.Self); // TODO clamp angle
		}

	}
}
