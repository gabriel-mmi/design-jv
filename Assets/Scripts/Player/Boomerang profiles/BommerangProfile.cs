using UnityEngine;

// Ce ScriptableObject permet de definir les stats de chaques boomerangs (Bois, Fer, Or et Diamant)

[CreateAssetMenu(fileName = "Boomerang profile", menuName = "ScriptableObjects/Bommerang profile", order = 1)]
public class BommerangProfile : ScriptableObject
{
    public float damages;
    public float speed;
    public float returnDistance; // Distance a laquelle le boomerang doit revenir vers le joueur
    public Sprite sprite;
}
