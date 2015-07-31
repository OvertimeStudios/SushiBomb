using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GameController myScript = target as GameController;

		myScript.sushisToWin = EditorGUILayout.IntField ("Sushis To Win", myScript.sushisToWin);

		myScript.isEditor = GUILayout.Toggle(myScript.isEditor, "Testing Mode " + ((myScript.isEditor) ? "Enabled" : "Disabled"));
		
		if(myScript.isEditor)
			myScript.startLevel = EditorGUILayout.IntField("Start Level", myScript.startLevel);
		
	}
}