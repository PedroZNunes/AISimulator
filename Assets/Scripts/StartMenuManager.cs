using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour {

    [SerializeField] private GameObject QuitButton;

    #if UNITY_WEBGL
    private void Start() {
        QuitButton.SetActive( false );
    }
    #endif

    public void LoadScene (int index) {
        SceneManager.LoadScene (index);
    }

    public void LoadScene (string name) {
        SceneManager.LoadScene (name);
    }

    public void Quit () { 
        Application.Quit ();
    }
}
