using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

/*
Primeiro passo: Criar uma referencia ao CallbackResult
Segundo passo: Criar o Callback passando o método de callback
Terceiro passo: Implementar o método de callback
*/
public class LeaderboardManager : MonoBehaviour
{

    public static LeaderboardManager instance = null;
    private CallResult<LeaderboardFindResult_t> m_LeaderBoardCreated;
    public CallResult<LeaderboardScoreUploaded_t> m_LeaderBoardUpdated;
    public SteamLeaderboard_t leaderboardHandle;
    void Start()
    {

    }

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_LeaderBoardCreated = CallResult<LeaderboardFindResult_t>.Create(OnFindOrCreateLeaderBoard);
            m_LeaderBoardUpdated = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderBoardUpdated);
            SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard("roguelikersdevigo", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
            m_LeaderBoardCreated.Set(handle);
            Debug.Log("Called FindOrCreateLeaderboard()");
        }
    }

    private void OnLeaderBoardUpdated(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bSuccess == 0)
        {
            Debug.LogError("Não foi possível atualizar seu score nenhum LeaderBoard");
        }
        else
        {
            Debug.LogFormat("Seu score foi: {0}", pCallback.m_bScoreChanged);
            Debug.LogFormat("Seu novo ranking é {0}", pCallback.m_nGlobalRankNew);
        }
    }

    private void OnFindOrCreateLeaderBoard(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bLeaderboardFound == 0)
        {
            Debug.LogError("Não foi encontrado nenhum LeaderBoard");
        }
        else
        {
            leaderboardHandle = pCallback.m_hSteamLeaderboard;
        }
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);


    }

    void Update()
    {
    }
}