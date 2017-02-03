using System.Collections.Generic;
using UnityEngine;

namespace Game {

	public enum ActionTag {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack
	}

	public class Action {
		public delegate void ActionDelegate();
		public static readonly ActionDelegate nop = () => { };

		public readonly ActionTag tag;
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

		public Action(ActionTag tag, KeyCode k1) {
			this.tag = tag;
			Keys.Add(k1);
		}

		public Action(ActionTag type, KeyCode k1, KeyCode k2) 
			: this(type, k1) { 
			Keys.Add(k2);
		}

	}

	public class ActionManager {
		public List<Action> Actions { get { return _actions; } }
		private readonly List<Action> _actions = new List<Action>();

		private static ActionManager _instance;
		public static ActionManager GetInstance() {
			return _instance ?? (_instance = new ActionManager());
		}

		private ActionManager() {

			_actions.Add(new Action(ActionTag.MoveForward, KeyCode.W));
			_actions.Add(new Action(ActionTag.MoveLeft, KeyCode.A));
			_actions.Add(new Action(ActionTag.MoveBack, KeyCode.S));
			_actions.Add(new Action(ActionTag.MoveRight, KeyCode.D));
			_actions.Add(new Action(ActionTag.Use, KeyCode.E));
		}

		public Action GetAction(ActionTag tag) {
			return _actions.Find(action => action.tag == tag);
		}


	}
}
