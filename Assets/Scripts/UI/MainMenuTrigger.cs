using UnityEngine;

// Ce composant gere les trigger present dans les portes du menu principal

public class MainMenuTrigger : MonoBehaviour
{
    public string message;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            FindObjectOfType<MainMenu>().SendMessage(message);
        }
    }
}
