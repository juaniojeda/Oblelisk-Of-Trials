using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private string gameSceneName = "GameScene";

    private const string PLAYER_ID_KEY = "PLAYER_ID";
    private bool _loading;

    private void Start()
    {
        // Autocompletar último ID si existe
        if (PlayerPrefs.HasKey(PLAYER_ID_KEY))
            idInput.text = PlayerPrefs.GetString(PLAYER_ID_KEY);

        // Enter mientras el campo tiene foco (según versión de TMP)
        idInput.onSubmit.AddListener(_ => OnPlayClicked());
        idInput.onEndEdit.AddListener(OnEndEdit);
    }

    private void Update()
    {
        // Enter en cualquier momento (si querés solo con foco, agrega: && idInput.isFocused)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            OnPlayClicked();
    }

    private void OnEndEdit(string _)
    {
        // En algunas plataformas Enter dispara EndEdit
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            OnPlayClicked();
    }

    public void OnPlayClicked()
    {
        if (_loading) return;

        string userId = idInput ? idInput.text.Trim() : "";
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("Ingresa un ID válido.");
            return;
        }

        // Guardar y usar como Nick de Photon
        PlayerPrefs.SetString(PLAYER_ID_KEY, userId);
        PlayerPrefs.Save();
        PhotonNetwork.NickName = userId;

        _loading = true;
        SceneManager.LoadScene(gameSceneName);
    }
}