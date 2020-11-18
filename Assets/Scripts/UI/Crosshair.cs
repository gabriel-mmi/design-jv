using UnityEngine;

// Ce composant gere le reticule

public class Crosshair : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Met a jour la position du reticule
        Vector3 nextPos = Input.mousePosition;
        nextPos.z = 0;

        rectTransform.position = nextPos;
    }
}
