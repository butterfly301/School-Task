using System;
using UnityEngine;

namespace UI
{
    public class FightUIManager : MonoBehaviour
    {
        public static FightUIManager Instance;
        
        private bool isOpenPausePanel;
        
        private void Awake()
        {
            /*if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }*/
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public GameObject informationPanel;
        public GameObject pausePanel;
        
        public VisionPanel visionPanel;
        public CountdownTimer countdownTimer;
        public SignalController signalController;
        public ScrollingDialogueController scrollingDialogueController;

        public void TogglePausePanel()
        { 
            isOpenPausePanel = !isOpenPausePanel;
            if (isOpenPausePanel)
            {
                GameFlowCoordinator.Instance.EnterPause();
            }
            else
            {
                GameFlowCoordinator.Instance.ResumeInteractiveFlow();
            }
            pausePanel.SetActive(!pausePanel.activeSelf);
        }

        private void Update()
        {
            if(Time.timeScale != 0)
                SlowMotion();
            if(Input.GetKeyDown(KeyCode.Escape))
                TogglePausePanel();
        }

        public void ShowInformationPanel(ItemData itemData)
        {
            informationPanel.GetComponent<InformationPanel>()
                .SetItemDiscription(itemData.itemName, itemData.itemInformation, itemData.itemSprite, itemData.price);
            informationPanel.SetActive(true);
        }

        public void HideInformationPanel()
        {
            informationPanel.SetActive(false);
        }

        public void StartCountdownTimer()
        {
            countdownTimer.StartTimer();
            countdownTimer.ToggleTimer();
        }

        public void StopCountdownTimer()
        {
            countdownTimer.StopTimer();
            countdownTimer.ToggleTimer();
        }
        
        void SlowMotion()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                Time.timeScale = 0.5f;
            
            if(Input.GetKeyUp(KeyCode.LeftShift))
                Time.timeScale = 1f;
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null; // 防止销毁后残留无效引用
            }
        }
    }
}
