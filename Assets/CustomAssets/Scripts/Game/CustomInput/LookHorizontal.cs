
using UnityEngine;
using UnityEngine.UI;

namespace Game.CustomInput {
	public class LookHorizontal : MonoBehaviour {

		[Range(0.0f, 100.0f)]
		public float sensitivity = 75.0f;

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
			transform.Rotate(0, speed, 0, Space.Self);
		}

	}
}
