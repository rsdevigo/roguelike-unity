using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public abstract class MonoBehauviorLeaderboardCallBacks : MonoBehaviour
{
    private CallResult<LeaderboardFindResult_t> m_LeaderBoardFound;
    public CallResult<LeaderboardScoreUploaded_t> m_LeaderBoardUpdated;
    public CallResult<LeaderboardScoresDownloaded_t> m_LeaderBoardScoreDownload;
    public SteamLeaderboard_t handle;
    public string LeaderboardName;
    public bool loadingLeaderBoardMeta = true;
    public

    void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_LeaderBoardFound = CallResult<LeaderboardFindResult_t>.Create(OnFindLeaderBoard);
            m_LeaderBoardUpdated = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderBoardUpdated);
            m_LeaderBoardScoreDownload = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderBoardScoreDownload);
            SteamAPICall_t handle = SteamUserStats.FindLeaderboard(this.LeaderboardName);
            m_LeaderBoardFound.Set(handle);
        }
    }

    public abstract void OnLeaderBoardUpdated(LeaderboardScoreUploaded_t pCallback, bool bIOFailure);
    public void OnFindLeaderBoard(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bLeaderboardFound == 0 || bIOFailure)
        {
            Debug.LogError("Leaderboard não foi encontrado.");
        }
        else
        {
            this.handle = pCallback.m_hSteamLeaderboard;
        }
    }
    public abstract void OnLeaderBoardScoreDownload(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure);
}
