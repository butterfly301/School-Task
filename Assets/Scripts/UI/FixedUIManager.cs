using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class FixedUIManager : MonoBehaviour
    {
        public static FixedUIManager Instance;
        private bool isOpenBagPanel = false;
        private bool isOpenShopPanel = false;
        
        public GameObject bagPanel;
        public GameObject shopPanel;
        public GameObject summaryPanel;

        
        private Inventory UIinventory;
        private UI_Inventory _uiBag;
        private UI_Inventory _uiShop;
        private UI_Inventory _uiData;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            var playerStateController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateController>();
            if (playerStateController != null)
            {
                UIinventory = playerStateController.inventory;
            }
            _uiBag= transform.Find("Canvas/BagPanel").GetComponent<UI_Inventory>();
            _uiShop = transform.Find("Canvas/ShopPanel/BagPanelForShopPanel").GetComponent<UI_Inventory>();
            _uiData = transform.Find("Canvas/SummaryPanel/DataPanel/Collect/BagPanelForDataPanel").GetComponent<UI_Inventory>();
            _uiBag.SetInventory(UIinventory);
            _uiShop.SetInventory(UIinventory);
            _uiData.SetInventory(UIinventory);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleBagPanel();
            if (Input.GetKeyDown(KeyCode.Escape) && !isOpenBagPanel)
            {
                if (isOpenBagPanel)
                {
                    ToggleBagPanel();
                }
            }
        }

        public void ToggleBagPanel()
        {
            if(summaryPanel.activeSelf)
                return;
            isOpenBagPanel = !isOpenBagPanel;
            if (isOpenBagPanel)
            {
                GameFlowCoordinator.Instance.EnterOverlay();
            }
            else
            {
                GameFlowCoordinator.Instance.ResumeInteractiveFlow();
            }
            bagPanel.SetActive(!bagPanel.activeSelf);
        }

        public void ToggleShopPanel()
        {
            isOpenShopPanel = !isOpenShopPanel;
            if (isOpenShopPanel)
            {
                GameFlowCoordinator.Instance.EnterOverlay();
                shopPanel.SetActive(!shopPanel.activeSelf);
            }
            else
            {
                shopPanel.SetActive(!shopPanel.activeSelf);
                GameFlowCoordinator.Instance.ResumeInteractiveFlow();
            }
        }

        public void SetItemInventory(Item item)
        {
            item.SetInventory(UIinventory);
        }
        public void SetPurchaseInventory(Purchase purchase)
        {
            purchase.SetInventory(UIinventory);
        }

        public void ShowSummaryPanel(string result)
        {
            GameFlowCoordinator.Instance.EnterSummary();
            summaryPanel.SetActive(true);
            summaryPanel.GetComponent<SummaryPanel>().WhichAnimationShouldBeShown(result);
        }

        public void ClosePanel()
        {
            bagPanel.SetActive(false);
            shopPanel.SetActive(false);
            summaryPanel.SetActive(false);
        }
    }
}
