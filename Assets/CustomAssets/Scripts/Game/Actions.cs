using System.Collections.Generic;
using UnityEngine;

namespace Game {

	public enum ActionTag {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack
	}

	public class Action {
		public delegate void ActionDelegate();
		public static readonly ActionDelegate nop = () => { };

		public readonly ActionTag Tag;
		public List<KeyCode> Keys = new List<KeyCode>(2);

		public ActionDelegate StartBehaviour = nop;
		public ActionDelegate WhileBehaviour = nop;
		public ActionDelegate FinishBehaviour = nop;
		public ActionDelegate NotPressedBehaviour = nop;

		public float TimeActionActive { get { return FinalizationTime - ActivationTime; } }
		public float TimeActionInactive { get { return Time.time - FinalizationTime; } }
		public float ActivationTime;
		public float FinalizationTime;

		public void DefineAllDefaults(ActionDelegate start, ActionDelegate during, ActionDelegate finish) {
			StartBehaviour = start;
			WhileBehaviour = during;
			FinishBehaviour = finish;
		}

		public Action SetAllNop() {
			DefineAllDefaults(nop, nop, nop);
			return this;
		}

		public Action(ActionTag tag, KeyCode k1) {
			this.Tag = tag;
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

		public Action GetAction(ActionTag tag) {
			return _actions.Find(action => action.Tag == tag);
		}

		public ActionManager AddAction(Action action) {
			_actions.Add(action);
			return this;
		}

	}
}
