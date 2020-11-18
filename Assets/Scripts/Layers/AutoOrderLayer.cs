using UnityEngine;

// Ce composant ajuste automatique (en fonction de la position Y de l'objet) la valuer order in layer du sprite renderer a chaque frames

public class AutoOrderLayer : MonoBehaviour
{
    public Vector2 pivot; // Offset de la position pour plus de personnalisation
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 point = (Vector2)transform.position + pivot;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-point.y);
    }

    // Gizmos pour l'editeur
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)pivot, 2f);
    }
}
