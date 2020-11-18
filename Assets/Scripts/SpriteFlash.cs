using System.Collections;
using UnityEngine;

// Ce composant active le shader de flash lorsque la procedure Flash est appelée
// par ILHAM EFFENDI

public class SpriteFlash : MonoBehaviour
{
    // Propriétés
    public Color flashColor; // Couleur de flash
    public float flashDuration;

    Material mat;

    private IEnumerator flashCoroutine;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    private void Start()
    {
        mat.SetColor("_FlashColor", flashColor);
    }

    // Quand Flash est appelée effectue l'animation de flash
    public void Flash ()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = DoFlash();
        StartCoroutine(flashCoroutine);
    }

    private IEnumerator DoFlash()
    {
        float lerpTime = 0;

        while (lerpTime < flashDuration)
        {
            lerpTime += Time.deltaTime;
            float perc = lerpTime / flashDuration;

            SetFlashAmount(1f - perc);
            yield return null;
        }
        SetFlashAmount(0);
    }

    private void SetFlashAmount(float flashAmount)
    {
        mat.SetFloat("_FlashAmount", flashAmount);
    }
}
