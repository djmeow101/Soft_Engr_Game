using UnityEngine;
using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Playgame()
    {
        SceneManager.LoadScene(1);   
    }

    // Update is called once per frame
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
