using System;
using System.Collections.Generic;
using Game.PlayerComponents;
using UnityEngine;

namespace Game {

	public delegate void ActionDelegate();
	
	public class Action<T> {
		public static readonly ActionDelegate nop = () => {};

		public readonly T Tag;
		public List<KeyCode> Keys { get { return _enabled ? _keys : _emptyKeys; } }

		public float TimeActionActive;
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

		public Action<T> Reset() {
			StartBehaviour = WhileBehaviour = FinishBehaviour = NotPressedBehaviour = nop;
			return this;
		}

		public ActionDelegate[] GetAllFunctions() {
			return new[] {StartBehaviour, WhileBehaviour, FinishBehaviour, NotPressedBehaviour};
		}

		public void SetAllFunctions(ActionDelegate[] functions) {
			StartBehaviour = functions[0];
			WhileBehaviour = functions[1];
			FinishBehaviour = functions[2];
			ForceFinishBehaviour = functions[3];
		}


		private ActionDelegate _startBehaviour = nop;
		private ActionDelegate _whileBehaviour = nop;
		private ActionDelegate _finishBehaviour = nop;
		private ActionDelegate _notPressedBehaviour = nop;

		private bool _enabled = true;
		private readonly List<KeyCode> _keys = new List<KeyCode>(2);
		private readonly List<KeyCode> _emptyKeys = new List<KeyCode>(0);

		public Action(T tag) {
			this.Tag = tag;
		}

		public Action<T> AddKey(KeyCode k) {
			Keys.Add(k);
			return this;
		}

		public void Enable() {
			if (_enabled) return;

			_enabled = true;

			foreach (KeyCode k in _keys) { // TODO: extract to ContinueFromForcedFinishedBehaviour delegate
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

	public class ActionManager<TEnum> where TEnum : struct, IConvertible, IComparable {
		public List<Action<TEnum>> Actions { get { return _actions; } }
		private readonly List<Action<TEnum>> _actions = new List<Action<TEnum>>();

		public Action<TEnum> GetAction(TEnum tag) {
			return _actions.Find(action => action.Tag.CompareTo(tag) == 0);
		}

		public void ResetAllActions() {
			foreach (Action<TEnum> action in _actions) {
				action.Reset();
			}
		}

		public ActionManager() {
			foreach (TEnum tag in Enum.GetValues(typeof(TEnum))) {
				AddAction(new Action<TEnum>(tag));
			}
		}

		private void AddAction(Action<TEnum> action) {
			_actions.Add(action);
		}

	}
}
