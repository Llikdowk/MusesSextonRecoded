
public static class DebugMsg {
	public delegate void DebugLog(string msg);
	private const string sep = " <color=grey>##</color> ";

	public static void ComponentNotFound(DebugLog log, System.Type component, string extraInfo = "") {
		log("Component <b>" + component + "</b> not found" + sep + extraInfo);
	}

	public static void GameObjectNotFound(DebugLog log, string gameObjectName, string extraInfo = "") {
		log("GameObject with name <b>" + gameObjectName + "</b> not found" + sep + extraInfo);
	}

	public static void NoExistantInteraction(DebugLog log) {
		log("Trying to remove a non-existant interaction");
	}
}
