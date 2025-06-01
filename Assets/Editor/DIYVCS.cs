using UnityEngine;
using UnityEditor;

// How to use this script:
// 1.  Download and save the file.
// 2.  Add to your project. You can just drag it into your script folder. 
// 3.  Select the Assets menu and there is a new item added at the end: Backup Project to UnityPackage.
// 4.  Select that item. A DIYYCS Editor window open.s
// 5.  You can change the name; for example, in a way that let's you remember what you backed up!
// 6.  The default save location is your project folder. You can use / to prefix the name with an *existing* folder. 
// 7.  Uncheck "Include Project Settings" if you are thinking of importing / adding this to a different existing project.
// 8.  Uncheck packages if you only want to export what's in your Assets. This depends on what custom packages you have. 
// 9.  Click Create backup. Your beautiful backup will be created. A folder opens on the file. Notice that the name is time-stamped.
// 10. To import: Use Assets > Import Package > Custom Package... (as usual)

public class DIYVCS : EditorWindow
{
	static string projectName;
	static bool bSettings;
	static bool bPackages;

	[MenuItem("Assets/Backup Project to UnityPackage")]
	public static void ShowWindow()
	{
		string[] s = Application.dataPath.Split('/');
		projectName = s[s.Length - 2];
		bSettings = true;
		bPackages = true;

		GetWindow<DIYVCS>("DIYVCS");
	}

	public static void exportPackage()
	{
		string[] projectContent = new string[] { "Assets", "ProjectSettings", "Packages" };

		if (bSettings == true && bPackages == true)
		{
			projectContent = new string[] { "Assets", "ProjectSettings", "Packages/manifest.json" };
		}
		else
		{
			if (bSettings == true && bPackages == false)
			{
				projectContent = new string[] { "Assets", "ProjectSettings" };
			}
			else
			{
				if (bSettings == false && bPackages == true)
				{
					projectContent = new string[] { "Assets", "Packages/manifest.json" };
				}
				else
				{
					if (bSettings == false && bPackages == false)
					{
						projectContent = new string[] { "Assets" };
					}
				}
			}
		}

		AssetDatabase.ExportPackage(projectContent, projectName + System.DateTime.UtcNow.ToString("yyyyMMddhhmmss") + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
		Debug.Log("Project Exported to project root folder with UTC datetimecode with " + projectContent.Length + " project folders.");
	}

	// draw the GUI window
	private void OnGUI()
	{
		GUILayout.Label("Backup Project to UnityPackage", EditorStyles.boldLabel);

		projectName = EditorGUILayout.TextField("Project Name", projectName);

		bSettings = GUILayout.Toggle(bSettings, "Include ProjectSettings");
		bPackages = GUILayout.Toggle(bPackages, "Include Packages");

		if (GUI.Button(new Rect(10, 100, 150, 30), "Create Backup"))
		{
			exportPackage();
		}
	}
}