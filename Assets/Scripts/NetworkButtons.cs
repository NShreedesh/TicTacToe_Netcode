using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButtons : NetworkBehaviour
{
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button clientButton;
    [SerializeField]
    private Button serverButton;
    [SerializeField]
    private Button cancelButton;

    private void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.StartHost();
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.StartClient();
        });
        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.StartServer();
        });
        cancelButton.onClick.AddListener(() =>
        {
            NetworkManager.Shutdown();
        });
    }
}
