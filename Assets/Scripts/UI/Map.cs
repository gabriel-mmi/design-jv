using UnityEngine;
using UnityEngine.SceneManagement;

// Ce composant n'est plus utilisé

public class Map : MonoBehaviour
{
    public void OpenScene (int id)
    {
        SceneManager.LoadScene(id);
    }
}
