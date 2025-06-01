using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // when the BACK key is pressed
        if(Input.GetKeyDown(KeyCode.Backspace))
		{
            SceneManager.LoadScene(0);      // load the MENU scene at index 0
        }
        
    }
}
