using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class Board : NetworkBehaviour
{
    private Button[,] buttons = new Button[3,3];
    private TMP_Text[,] buttonText = new TMP_Text[3,3];
    private char[,] boardValues = new char[3, 3]
    {
        {'-', '-', '-'},
        {'-', '-', '-'},
        {'-', '-', '-'},
    };
    private char playerTurn = 'X';
    private TMP_Text winLoseText;

    private void Start()
    {
        winLoseText = FindObjectOfType<WinLoseCanvas>().transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public override void OnNetworkSpawn()
    {
        int buttonCount = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int row = i;
                int col = j;

                buttons[i, j] = transform.GetChild(buttonCount).GetComponent<Button>();
                buttonText[i, j] = buttons[i, j].GetComponentInChildren<TMP_Text>();
                buttonText[i, j].text = boardValues[i, j].ToString();
                buttons[i, j].onClick.AddListener(() =>
                {
                    if(boardValues[row, col] == 'X' || boardValues[row, col] == 'O') return;
                    ChangePlayerTurnServerRpc(row, col, new ServerRpcParams());
                });
                ChangeTextServerRpc(i, j);
                buttonCount++;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        winLoseText.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerTurnServerRpc(int row, int col, ServerRpcParams serverRpcParams)
    {
        if (boardValues[row, col] == 'X' || boardValues[row, col] == 'O') return;

        if (playerTurn == 'X' && serverRpcParams.Receive.SenderClientId == 0)
        {
            boardValues[row, col] = playerTurn;
            ChangeTextClientRpc(row, col, playerTurn);

            bool won = ChekWin();
            if (won)
            {
                WonClientRpc(serverRpcParams.Receive.SenderClientId);
            }

            playerTurn = 'O';
        }
        else if(playerTurn == 'O' && serverRpcParams.Receive.SenderClientId > 0)
        {
            boardValues[row, col] = playerTurn;
            ChangeTextClientRpc(row, col, playerTurn);

            bool won = ChekWin();
            if (won)
            {
                WonClientRpc(serverRpcParams.Receive.SenderClientId);
            }

            playerTurn = 'X';
        }
    }

    [ClientRpc]
    private void ChangeTextClientRpc(int i, int j, char value)
    {
        buttonText[i, j].text = value.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeTextServerRpc(int i, int j)
    {
        ChangeTextClientRpc(i, j, boardValues[i, j]);
    }

    [ClientRpc]
    private void WonClientRpc(ulong clientId)
    {
        if(NetworkManager.LocalClient.ClientId == clientId)
        {
            winLoseText.text = "You Win...";
        }
        else
        {
            winLoseText.text = "You Lose...";
        }
        winLoseText.gameObject.SetActive(true);
    }

    private bool ChekWin()
    {
        if ((boardValues[0, 0] == boardValues[1, 1]) && (boardValues[0, 0] == boardValues[2, 2])
            && (boardValues[0, 0] == playerTurn))
        {
            return true;
        }
        else if ((boardValues[2, 0] == boardValues[1, 1]) && (boardValues[2, 0] == boardValues[0, 2])
             && (boardValues[2, 0] == playerTurn))
        {
            return true;
        }

        for (int i = 0; i < boardValues.GetLength(0); i++)
        {
            if ((boardValues[i, 0] == boardValues[i, 1]) && (boardValues[i, 0] == boardValues[i, 2])
                 && (boardValues[i, 0] == playerTurn))
            {
                return true;
            }
        }

        for (int i = 0; i < boardValues.GetLength(0); i++)
        {
            if ((boardValues[0, i] == boardValues[1, i]) && (boardValues[0, i] == boardValues[2, i])
                 && (boardValues[0, i] == playerTurn))
            {
                return true;
            }
        }

        return false;
    }
} 