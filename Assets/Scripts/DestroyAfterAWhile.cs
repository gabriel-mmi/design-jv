using UnityEngine;

// Ce composant permet de detruire un objet apres un certain temps

public class DestroyAfterAWhile : MonoBehaviour
{
    // Propriétés
    public float lifeTime;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
