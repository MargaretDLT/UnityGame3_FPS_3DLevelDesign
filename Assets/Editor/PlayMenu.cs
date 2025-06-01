using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/**
 * How to use this script:
 *     Drop it into the Assets > Editor Folder and restart your project.
 *     This script modifies the PLAY  button (for playing a Scene, not in the Suck It Up! menu). 
 *     
 * What this script does:
 *     When you press the PLAY button a popup will ask to save the current work in the Scene file.
 *     - If you click "Save", the current Scene is saved and the MainMenu Scene loads and plays.
 *     - If you click "Don't Save" the MainMenu Scene loads and plays and you lose your changes. 
 *     - If you click "Cancel" you return to your Scene in and nothing plays. 
 * , 
 *     ... when you press PLAY again, playing stops and the script reloads the Scene you were working on as the active editor scene.
 */


// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class PlayMenu
{
	private static string currentScene;

	// register an event handler when the class is initialized
	static PlayMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayButton;
    }

    private static void OnPlayButton(PlayModeStateChange state)
    {
        //Debug.Log("I am here");

		// handle turn on PLAY button - save unsaved scene or stop
		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
		{
			//Debug.Log("trying playmode");
			currentScene = EditorSceneManager.GetActiveScene().path;
			EditorPrefs.SetString("CurrentScene", currentScene);
			Debug.Log("Current Scene = " + currentScene);

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				//Debug.Log("loading & playing main menu");
				EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Single);
				EditorApplication.isPlaying = true;
			}
			else
			{
				//Debug.Log("User cancelled save before play");
				EditorApplication.isPlaying = false;
			}
		}

		// PLAY button pressed, stop playing
		if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
		{
			//Debug.Log("Exiting playmode");
			EditorApplication.isPlaying = false;
		}

		// restore saved current scene when not playing anymore
		if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
		{
			//Debug.Log("Open previous scene");
			EditorSceneManager.OpenScene(EditorPrefs.GetString("CurrentScene"), OpenSceneMode.Single);
		}
    }
}