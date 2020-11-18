/* Author : Raphaël Marczak - 2018/2020, for MIAMI Teaching (IUT Tarbes) and MMI Teaching (IUT Bordeaux Montaigne)
 * 
 * This work is licensed under the CC0 License. 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Ce composant gere les dialogue avec les NPC

public class DialogManager : MonoBehaviour 
{
    public Text nameText; // Nom du NPC
    public Text lineText; // Phrase du NPC
    public AudioClip letterClip; // Son a jouer a chaque lettre

    [HideInInspector] public bool inDialog = false;

    private int currentLine = 0; // Index de la ligne actuelle
    private List<string> dialogLines = new List<string>(); // Lignes a afficher
    private PlayerBehavior player;
    private AudioSource source;

    private bool canNextLine = true;

    // Commence la dialog, procédure appelée par la classe NPC
    public void StartDialog (string name, List<string> lines)
    {
        if(player == null)
        {
            source = GetComponent<AudioSource>();
            player = PlayerBehavior.instance.GetComponent<PlayerBehavior>();
        }

        if(!inDialog)
        {
            // Reset des variables (au cas ou elles auraient mal ete reset a la fin du dernier dialogue)
            gameObject.SetActive(true);
            inDialog = true;

            player.canAttack = false;
            player.canMove = false;

            currentLine = 0;
            dialogLines = lines;

            nameText.text = name;

            // Effet d'ecriture
            StopCoroutine("WriteEffect");
            StartCoroutine(WriteEffect(lineText, dialogLines[currentLine]));
        }
    }

    // Passer a la ligne suivante quand j'appuie sur espace
    public void NextLine ()
    {
        if (canNextLine)
        {
            if (currentLine < dialogLines.Count - 1)
            {
                currentLine++;
                StopCoroutine("WriteEffect");
                StartCoroutine(WriteEffect(lineText, dialogLines[currentLine]));
            }
            // Si il n'y a pas d'autres lignes alors le dialogue se termine
            else
            {
                StopDialog();
            }
        }
    }

    // Affiche les lettres une par une
    private IEnumerator WriteEffect (Text ui, string line)
    {
        ui.text = "";
        int length = line.Length;
        canNextLine = false; // Empeche de skip la ligne en cours d'écriture

        for (int i = 0; i <= length; i++)
        {
            ui.text = line.Substring(0, i);

            // Sound
            source.PlayOneShot(letterClip);
            source.pitch = Random.Range(0.5f, 1.5f);

            // Delay between letter print
            yield return new WaitForSeconds(0.012f);
        }

        canNextLine = true;
    }

    // Arrete le dialogue quand il n'y a plus de phrases ou quand le joueur appuie sur Echap
    public void StopDialog ()
    {
        dialogLines = new List<string>();

        player.canMove = true;
        player.canAttack = true;

        inDialog = false;
        gameObject.SetActive(false);
    }

    // Si le joueur appuie sur Echap alors le dialogue se termine
    private void Update()
    {
        if (inDialog)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                NextLine();
            }else if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopDialog();
            }
        }
    }

}
