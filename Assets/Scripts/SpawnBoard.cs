using Unity.Netcode;
using UnityEngine;

public class SpawnBoard : NetworkBehaviour
{
    [SerializeField]
    private GameObject board;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        if (!IsHost) return;

        SpawnServerRpc();
    }

    [ServerRpc]
    private void SpawnServerRpc()
    {
        GameObject spawnedBoard = Instantiate(board, transform.position, Quaternion.identity);
        spawnedBoard.transform.localPosition = Vector3.zero;

        spawnedBoard.GetComponent<NetworkObject>().Spawn(true);
        spawnedBoard.GetComponent<NetworkObject>().TrySetParent(transform, false);
    }
}
