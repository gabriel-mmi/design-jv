using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// Ce composant gere le menu principal

public class MainMenu : MonoBehaviour
{
    // References aux cinematiques (videosa lire)
    public GameObject startVideoParent, uiParent, creditsVideoParent, tutoVideoParent, yakoVideoParent;
    public VideoPlayer startVideoPlayer, creditsVideoPlayer, tutoVideoPlayer, yakoVideoPlayer;

    private PlayerMenu player;
    private AudioReverbZone reverbZone;
    private bool inStartVideo = false, inCreditsVideo = false;
    private int startVideoStep = 0;
    private float skipFirstVideoTime;

    private void Start()
    {
        player = FindObjectOfType<PlayerMenu>();
        reverbZone = FindObjectOfType<AudioReverbZone>();
        Cursor.visible = false; // Affiche le curseur
    }

    #region Jouer et arreter les videos
    private IEnumerator PlayVideo(VideoPlayer player, GameObject rawImage, bool autoStopVideo = true)
    {
        reverbZone.gameObject.SetActive(false); // Desactive l'effet de reverberation
        rawImage.SetActive(true);
        VideoPlayer _player = player;
        _player.time = 0.0f;
        _player.Play();

        if (autoStopVideo)
        {
            yield return new WaitForSeconds((float)_player.clip.length);

            _player.Stop();
            _player.time = 0.0f;
            rawImage.SetActive(false);
            reverbZone.gameObject.SetActive(true); // Active l'effet de reverberation
        }
    }

    private void StopVideo (VideoPlayer player, GameObject rawImage)
    {
        player.Stop();
        player.time = 0.0f;
        rawImage.SetActive(false);
    }
    #endregion

    #region Bouton "Commencer"
    // Lorsque le joueur entre dans la porte "Commencer"
    public void OnStart ()
    {
        startVideoStep = 0;
        StartCoroutine(StartGame()); // Joue la video d'introduction et le tutoriel
    }

    private IEnumerator StartGame ()
    {
        if (startVideoStep == 0) // Joue la premiere video ici
        {
            // Affiche la video et desactive les autres objets
            inStartVideo = true;
            uiParent.SetActive(false); // Desactive le menu principal
            player.gameObject.SetActive(false); // Desactive le joueur
            FindObjectOfType<Crosshair>().gameObject.SetActive(false); // Desactive le reticule

            StartCoroutine(PlayVideo(startVideoPlayer, startVideoParent)); // Joue la premiere video (story speech)
            yield return new WaitForSeconds((float)startVideoPlayer.clip.length); // Attendre la fin de la video
            startVideoStep = 1; // Le joueur regarde la premiere video
        }

        if (startVideoStep == 1)
        {
            Cursor.visible = true;
            StartCoroutine(PlayVideo(tutoVideoPlayer, tutoVideoParent, false)); // Joue la deuxieme video (inputs tuto)

            yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Space) && (Time.time - skipFirstVideoTime) > 0.5f) );
            startVideoStep = 2; // Le joueur regarde la deuxieme video
            tutoVideoPlayer.Stop(); // Arrete la video
            tutoVideoPlayer.time = 0.0f;
            tutoVideoParent.SetActive(false);
            Cursor.visible = false;
        }

        if (startVideoStep == 2)
        {
            StartCoroutine(PlayVideo(yakoVideoPlayer, yakoVideoParent)); // Joue la troisieme video (yako message)
            yield return new WaitForSeconds((float)yakoVideoPlayer.clip.length); // Attendre la fin de la video
            startVideoStep = 3; // Le joueur regarde la troisieme video
        }

        // Lorsque la video est terminée alors je charge la scene suivant
        inStartVideo = false;
        SceneManager.LoadScene(SaveManager.instance.sceneId);
    }
    #endregion

    #region Bouton "Options"
    // Lorsque le joueur entre dans la porte "Options"
    public void OnSettings ()
    {
        //... show settings
    }
    #endregion

    #region Bouton "Crédits"
    // Lorsque le joueur entre dans la porte "Crédits"
    public void OnCredits ()
    {
        StartCoroutine(ShowCredits()); // Joue la video des crédits
    }

    private IEnumerator ShowCredits()
    {
        inCreditsVideo = true;
        player.transform.position = Vector3.zero; // Replace la joueur au centre
        player.gameObject.SetActive(false); // Désactive le joueur
        uiParent.SetActive(false); // Desactive le menu

        StartCoroutine(PlayVideo(creditsVideoPlayer, creditsVideoParent));
        yield return new WaitForSeconds((float)creditsVideoPlayer.clip.length);

        uiParent.SetActive(true); // Active le menu
        player.gameObject.SetActive(true); // Active le joueur
        player.canMove = true;
        inCreditsVideo = false;
    }
    #endregion

    #region Bouton "Quitter"
    // Lorsque le joueur entre dans la porte "Quitter"
    public void  OnQuit ()
    {
        Application.Quit();
    }
    #endregion

    // Skip videos
    private void Update()
    {
        // E et Espace permettent de passer les cinématiques
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            // Si je veux passer les crédits...
            if (inCreditsVideo)
            {
                // Arrete la lecture des credits
                StopAllCoroutines();

                // Reset la video des credits
                creditsVideoPlayer.Stop();
                creditsVideoPlayer.time = 0.0f;
                creditsVideoParent.SetActive(false);

                uiParent.SetActive(true); // Active le menu
                player.gameObject.SetActive(true); // Active le joueur
                inCreditsVideo = false;
                reverbZone.gameObject.SetActive(true); // Active l'effet de reverberation
            }
            // Ou si je veux passer la vidéo d'introduction
            else if (inStartVideo)
            {
                if(startVideoStep == 0)
                {
                    // Arreter la video actuelle
                    StopAllCoroutines();

                    StopVideo(startVideoPlayer, startVideoParent);
                    // Lire la video suivante
                    skipFirstVideoTime = Time.time;
                    startVideoStep = 1;
                    StartCoroutine(StartGame());
                    reverbZone.gameObject.SetActive(true); // Active l'effet de reverberation
                }
                else if (startVideoStep == 1)
                {
                    Cursor.visible = true;
                    // Arreter la video actuelle
                    StopAllCoroutines();

                    StopVideo(tutoVideoPlayer, tutoVideoParent);
                    // Lire la video suivante
                    startVideoStep = 2;
                    StartCoroutine(StartGame());
                    reverbZone.gameObject.SetActive(true); // Active l'effet de reverberation
                }
                else if (startVideoStep == 2)
                {
                    SceneManager.LoadScene(SaveManager.instance.sceneId);
                }
                else
                {
                    SceneManager.LoadScene(SaveManager.instance.sceneId);
                }
            }
        }
    }
}
