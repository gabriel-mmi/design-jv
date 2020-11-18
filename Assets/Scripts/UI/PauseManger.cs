using UnityEngine;

// Ce composant gere la menu pause

public class PauseManger : MonoBehaviour
{
    // Ouverture du menu (appelé par PlayerBehaviour)
    public void OpenMenu()
    {
        Time.timeScale = 0; // Ecoulement du temps arreté
        transform.GetChild(0).gameObject.SetActive(true); // Affichage du l'interace
    }

    // Lorsque le joueur clic sur le bouton "Reprendre"
    public void OnResume()
    {
        Time.timeScale = 1; // Reprise de l'ecoulement normal
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Lorsque le joueur clic sur le bouton "Quitter"
    public void OnQuit()
    {
        Application.Quit();
    }
}
