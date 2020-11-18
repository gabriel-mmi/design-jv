using UnityEngine;

// Ce composant oriente sur l'axe z l'objet en direction de la souris

public class LookAtMouse : MonoBehaviour
{
    // Propriétés
    public bool isActive = true; // Rotate or not to mouse
    public float speed; // Rotation speed

    void Update()
    {
        if (isActive)
        {
            // Obtenir la position de la souris
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Calculer l'angle a parcourir
            float angle = Vector2.SignedAngle(Vector2.right, (mousePos - (Vector2)transform.position));
            angle += 180;
            // Effectuer la rotation
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, angle, speed * Time.deltaTime));
        }
    }
}
