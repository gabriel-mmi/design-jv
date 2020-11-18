using UnityEngine;
using UnityEngine.SceneManagement;

// Ce composant gere l'ecran de mort

public class DeathScreen : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true; // Affiche le cursuer
    }

    // Lorsque le bouton "Rejouer" est préssé
    public void OnRestart ()
    {
        if (SaveManager.instance)
        {
            // Charge la derniere scene
            SceneManager.LoadScene(SaveManager.instance.sceneId);
        }
    }
}
