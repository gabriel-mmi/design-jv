using UnityEngine;

// Ce composant ajuste automatique (en fonction de la position Y de l'objet) la valuer order in layer du sprite renderer AU START

public class AutoLayerOrderOnce : MonoBehaviour
{
    public Vector2 pivot;

    void Start()
    {
        Vector2 point = (Vector2)transform.position + pivot;
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-point.y);
    }

    // Gizmos pour l'editeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)pivot, 2f);
    }
}
