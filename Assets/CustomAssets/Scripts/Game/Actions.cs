using System.Collections.Generic;
using UnityEngine;

namespace Game {

	public enum ActionTag {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack
	}

	public class Action {
		public delegate void ActionDelegate();
		public static readonly ActionDelegate nop = () => {};

		public readonly ActionTag Tag;
		public List<KeyCode> Keys { get { return _enabled ? _keys : _emptyKeys; } }

		public float TimeActionActive { get { return FinalizationTime - ActivationTime; } }
		public float TimeActionInactive { get { return Time.time - FinalizationTime; } }
		public float ActivationTime;
		public float FinalizationTime;

		public ActionDelegate StartBehaviour {
			get { return _enabled ? _startBehaviour : nop; }
			set { _startBehaviour = value; }
		}
		public ActionDelegate WhileBehaviour {
			get { return _enabled ? _whileBehaviour : nop; }
			set { _whileBehaviour = value; }
		}
		public ActionDelegate FinishBehaviour {
			get { return _enabled ? _finishBehaviour : nop; }
			set { _finishBehaviour = value; }
		}

		public ActionDelegate NotPressedBehaviour {
			get { return _enabled ? _notPressedBehaviour : nop; }
			set { _notPressedBehaviour = value; }
		}

		public ActionDelegate ForceFinishBehaviour = nop;

		private ActionDelegate _startBehaviour = nop;
		private ActionDelegate _whileBehaviour = nop;
		private ActionDelegate _finishBehaviour = nop;
		private ActionDelegate _notPressedBehaviour = nop;

		private bool _enabled = true;
		private readonly List<KeyCode> _keys = new List<KeyCode>(2);
		private readonly List<KeyCode> _emptyKeys = new List<KeyCode>(0);

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

		public void Enable() {
			if (_enabled) return;

			_enabled = true;
			foreach (KeyCode k in _keys) {
				if (Input.GetKey(k)) {
					StartBehaviour();
					break;
				}
			}
		}

		public void Disable() {
			if (!_enabled) return;

			ForceFinishBehaviour();
			_enabled = false;
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
