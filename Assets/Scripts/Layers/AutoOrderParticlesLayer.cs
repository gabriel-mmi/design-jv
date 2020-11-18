using UnityEngine;

// Ce composant est semblable au composant AutoOrderLayer mais pour les particules

public class AutoOrderParticlesLayer : MonoBehaviour
{
    public SpriteRenderer relativeTo;
    private ParticleSystemRenderer particleSystemRenderer;

    void Start()
    {
        particleSystemRenderer = (ParticleSystemRenderer)GetComponent<ParticleSystem>().GetComponent<Renderer>();
    }

    void Update()
    {
        particleSystemRenderer.sortingOrder = Mathf.RoundToInt(relativeTo.sortingOrder - 10);
    }

}
