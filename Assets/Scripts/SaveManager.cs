using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ce composant charge et sauvegarde les données de l'utilisateur

public class SaveManager : MonoBehaviour
{
    // Singleton pour acceder a l'instance de la classe via une variable statique (SaveManager.instance)
    #region Singleton
    public static SaveManager instance;

    void Awake()
    {
        // Si la variable instance est deja assignée il faut detruire cette instance de SaveManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            playerHealth = 100;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // List des boomerang disponibles dans le jeu
    public List<BommerangProfile> boomerangsProfiles = new List<BommerangProfile>();

    [Header("Loaded variables")]
    // Variables chargées et sauvegardées
    public int playerCurrentBoomerang;
    public int sceneId;
    public float playerHealth;
    public int playerMoney;

    // Defini si le jeu doit sauvegarder ou non a la fermeture de l'application
    private bool saveAtQuit = true;

    public void Save ()
    {
        // Si il y a un instance du joueur dans la scene alors recupere ces stats (boomerang, vie et argent)
        // pour les sauvegarder
        if (PlayerBehavior.instance)
        {
            // Boomerang
            PlayerBehavior player = PlayerBehavior.instance;

            playerCurrentBoomerang = boomerangsProfiles.IndexOf(player.currentBoomerang);
            PlayerPrefs.SetInt("playerCurrentBoomerang", playerCurrentBoomerang);

            // Vie
            if (player.health > 10)
            {
                playerHealth = player.health;
            }
            else
            {
                playerHealth = 25;
            }

            // Argent
            playerMoney = player.money;
            PlayerPrefs.SetInt("playerMoney", playerMoney);
        }

        // Sauvegarde de la scene actuelle (sauf si le joueur se trouve dans le menu principale (index 0) ou dans la scene de mort (index 1))
        int _sceneId = SceneManager.GetActiveScene().buildIndex;
        if (_sceneId > 1)
        {
            sceneId = _sceneId;
            PlayerPrefs.SetInt("sceneId", _sceneId);
        }
    }

    public void Load ()
    {
        // Recupere les 3 variables stockées dans PlayerPrefs (boomerang actuel, id de la derniere scene et argent du joueur)
        playerCurrentBoomerang = PlayerPrefs.GetInt("playerCurrentBoomerang", 0);
        sceneId = PlayerPrefs.GetInt("sceneId", 2);
        playerMoney = PlayerPrefs.GetInt("playerMoney", 0);
        
        // Une fois recuperées met a jour les variables de l'instance de PlayerBehaviour (si elle existe)
        ApplyToScene();
    }

    public void ApplyToScene ()
    {
        // Si elle existe, met a jour les variables (argent et boommerang) de l'instance de PlayerBehaviour fraichement chargées
        if(PlayerBehavior.instance)
        {
            // Boomerang
            if(playerCurrentBoomerang < boomerangsProfiles.Count)
            {
                PlayerBehavior.instance.ChangeCurrentBoomerang(boomerangsProfiles[playerCurrentBoomerang]);
            }else
            {
                PlayerBehavior.instance.ChangeCurrentBoomerang(boomerangsProfiles[boomerangsProfiles.Count - 1]);
            }

            // Vie
            PlayerBehavior.instance.health = playerHealth;
            // Argent
            PlayerBehavior.instance.money = playerMoney;
        }
    }

    // A la permiere image (dans la scene MainMenu), charge les donées sauvegardées
    private void Start()
    {
        Load();
    }

    // Quand un niveau est chargé, si ce n'est ni le niveau du menu principal ni celui de la mort
    // alors met a jour l'instance de PlayerBehaviour et sauvegarde (on sauvegarde ici pour mettre a jour la scene puisque on vient de chagner de scene)
    private void OnLevelWasLoaded(int level)
    {
        if(level > 1)
        {
            ApplyToScene();
            Save();
        }
    }

    // Quand l'application est sur le point de se fermer, si j'ai le droit, sauvegarde
    private void OnApplicationQuit()
    {
        Debug.Log("Quitting application (non-editor only)");
        if (saveAtQuit)
        {
            Save();
        }
    }

    void Update()
    {
        // Si les touches Control et Delete sont préssées en meme temps alors detruit la sauvegarde actuelle
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSave();
        }    
    }

    // Supprime la sauvegarde atcuelle
    public void DeleteSave ()
    {
        // Remet les stats a 0
        playerHealth = 100;
        playerCurrentBoomerang = 0;
        playerMoney = 0;
        sceneId = 2;

        // Re-sauvegarde les données fraichements remises a zero
        PlayerPrefs.SetInt("playerCurrentBoomerang", 0);
        PlayerPrefs.SetInt("playerMoney", 0);
        PlayerPrefs.SetInt("sceneId", 2);

        // Empeche le jeu de sauvegarder lorsque l'application se ferme
        // car si le joueur supprime la sauvegarde dans un niveau la sauvegarde automatique a la fermeture re-sauvegarderait
        Debug.Log("Save deleted");
        saveAtQuit = false;
    }
}
