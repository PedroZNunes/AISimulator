using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour {

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
