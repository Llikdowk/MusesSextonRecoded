using System.Collections.Generic;
using UnityEngine;

namespace Game.Actions {

	public class Action {
		public delegate void ActionDelegate();
		public static readonly ActionDelegate nop = () => { };

		public List<KeyCode> Keys = new List<KeyCode>(2);
		public ActionDelegate StartBehaviour = nop;
		public ActionDelegate WhileBehaviour = nop;
		public ActionDelegate FinishBehaviour = nop;


		public void DefineAllDefaults(ActionDelegate start, ActionDelegate during, ActionDelegate finish) {
			StartBehaviour = start;
			WhileBehaviour = during;
			FinishBehaviour = finish;
		}

		public void SetAllNop() {
			DefineAllDefaults(nop, nop, nop);
		}

		public Action(KeyCode k1) {
			Keys.Add(k1);
		}

		public Action(KeyCode k1, KeyCode k2) {
			Keys.Add(k1);
			Keys.Add(k2);
		}

	}

	public class ActionManager {
		public List<Action> Actions { get { return _actions; } }
		private readonly List<Action> _actions = new List<Action>();

		public ActionManager() {
			var use = new Action(KeyCode.E);
			use.DefineAllDefaults(()=> Debug.Log("use start"), () => Debug.Log("use while"), () => Debug.Log("use finish"));

			_actions.Add(use);
		}

	}
}
