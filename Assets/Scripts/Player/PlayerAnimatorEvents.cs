using System.Collections.Generic;
using UnityEngine;

// Ce composant permet de recuperer les callbacks de l'Animator du joueur et d'effectuer les actions necessaires
// par exemple : joueur un son de pas

public class PlayerAnimatorEvents : MonoBehaviour
{
    public List<AudioClip> stepSounds = new List<AudioClip>();

    private AudioSource source;

    private void Start()
    {
        source = GetComponentInParent<AudioSource>();

    }

    // Cette procedure est appelé par l'Animator du joueur.
    // Elle permet de jouer un son de pas aleatoire
    public void PlayStepSound ()
    {
        source.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Count)]);
    }
}
