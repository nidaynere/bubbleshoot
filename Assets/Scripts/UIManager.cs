using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GamePlayEvents gamePlayEvents;

        private void Start()
        {
            gamePlayEvents.OnGameStarted += GameStarted;
            gamePlayEvents.OnGameOver += GameOver;
            gamePlayEvents.OnScoreUpdate += ScoreUpdate;
        }

        public void ScoreUpdate(int score)
        {
            
        }

        public void GameOver(int score)
        {
            
        }

        public void GameStarted()
        {
            
        }
    }
}