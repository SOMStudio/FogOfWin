using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class StoreItemManager : MonoBehaviour
    {
        [Header("Main")]
        public TypeBuster typeBuster;
        public int costBuster;
        public int countBuster;
        
        [Header("Content")]
        public Text amountText;
        public Text countText;
        public Button buyButton;

        public void UpdateAmount(int value)
        {
            amountText.text = costBuster + "/" + value;
        }

        public void UpdateCount(int value)
        {
            countText.text = countBuster + "/" + value;
        }
    }
}
