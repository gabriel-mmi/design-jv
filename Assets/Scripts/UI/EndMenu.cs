using UnityEngine;
using UnityEngine.SceneManagement;

// Ce composant gere l'ecran de fin du la premiere version du jeu
// Il redirige joueur vers le menu principal ou lui permet de quitter le jeu

public class EndMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    // Detruit la sauvegarde et en crée une nouvelle
    public void OnRestartGame ()
    {
        SaveManager.instance.DeleteSave();
        OnMainMenu();
    }

    // Revienir au menu principal
    public void OnMainMenu ()
    {
        SceneManager.LoadScene(0);
    }

    // Quitter le jeu
    public void OnQuit ()
    {
        Application.Quit();
    }
}
