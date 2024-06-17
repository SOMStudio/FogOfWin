using Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui.Menu
{
    public class StoreItemManager : MonoBehaviour
    {
        [FormerlySerializedAs("typeBuster")] [Header("Main")]
        public BusterType busterType;
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
