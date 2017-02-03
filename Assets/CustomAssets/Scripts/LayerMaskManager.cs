using System;
using System.Collections.Generic;
using UnityEngine;

public enum Layer {
	Player
}

public class LayerMaskManager {
	private static LayerMaskManager _instance;
	private readonly Dictionary<string, int> _map;

	public static LayerMaskManager GetInstance() {
		return _instance ?? (_instance = new LayerMaskManager());
	}

	private LayerMaskManager() {
		_map = new Dictionary<string, int>();
		foreach (Layer item in Enum.GetValues(typeof(Layer))) {
			AddLayer(item);
		}
	}

	public void AddLayer(Layer layer) {
		string name = layer.ToString();
		_map.Add(name, LayerMask.NameToLayer(name));
	}

	public int GetLayer(Layer layer) {
		return _map[layer.ToString()];
	}

}
