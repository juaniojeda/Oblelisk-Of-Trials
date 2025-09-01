using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class PlayerNameTag : MonoBehaviourPun
{
    [Header("Refs")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform target; // dónde se posiciona el tag (ej: cabeza)
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    [Header("Comportamiento")]
    [SerializeField] private bool billboardToCamera = true;
    [SerializeField] private bool tintLocal = true;
    [SerializeField] private Color localColor = Color.cyan;
    [SerializeField] private Color remoteColor = Color.white;

    private void Reset()
    {
        nameText = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (nameText == null) nameText = GetComponent<TMP_Text>();
        if (target == null) target = transform.parent != null ? transform.parent : transform;
    }

    private void OnEnable()
    {
        // Nick de su dueño (se replica a todos automáticamente)
        string nick = photonView.Owner != null ? photonView.Owner.NickName : "Player";
        nameText.text = string.IsNullOrEmpty(nick) ? "Player" : nick;

        if (tintLocal)
            nameText.color = photonView.IsMine ? localColor : remoteColor;
    }

    private void LateUpdate()
    {
        // Seguir al target con offset
        if (target) transform.position = target.position + offset;

        // Mirar a la cámara local
        if (billboardToCamera && Camera.main != null)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
