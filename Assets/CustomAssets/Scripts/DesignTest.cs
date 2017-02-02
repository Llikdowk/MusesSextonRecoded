using UnityEngine;
using Input = UnityEngine.Input;

namespace Test {

	public interface Behaviour {
	}

	public class DesignTest : MonoBehaviour {

		public string Behaviour;


		void Start() {
			Debug.Log("DesignTest init");
		}

		void Update() {
			/*
			if(Input.GetButtonDown("Fire1")) {
				Debug.Log(Input.mousePosition);
			}
			*/

		}
	}
}
