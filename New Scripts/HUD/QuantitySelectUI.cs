using System;
using UnityEngine;
using UnityEngine.UI;

namespace TNT
{
    public class QuantitySelectUI : MonoBehaviour
    {
        public Slider quantitySlider; public InputField quantityInput;
        public Button quantityBoxConfirm; public Button quantityBoxCancel;
        internal Inventory parentInventory; internal int selectedItemIndex;

        public void OpenQuantitySelect(Inventory inventory, int itemIndex, Action<int> onConfirm)
        {
            parentInventory = inventory; selectedItemIndex = itemIndex;

            quantitySlider.onValueChanged.RemoveAllListeners(); 
            quantitySlider.onValueChanged.AddListener(OnQuantitySliderChanged);
            quantitySlider.wholeNumbers = true; 
            quantitySlider.minValue = 1; 
            quantitySlider.maxValue = parentInventory.storedItems[selectedItemIndex].quantity;

            quantityInput.onEndEdit.RemoveAllListeners(); 
            quantityInput.onEndEdit.AddListener(OnConfirmInput);
            quantitySlider.value = quantitySlider.minValue; 
            quantityInput.text = quantitySlider.value.ToString();

            quantityBoxCancel.onClick.RemoveAllListeners(); 
            quantityBoxCancel.onClick.AddListener(CloseBox);
            quantityBoxConfirm.onClick.RemoveAllListeners();
            quantityBoxConfirm.onClick.AddListener(() => { onConfirm?.Invoke((int)quantitySlider.value); CloseBox(); });
        }

        public void CloseBox() => gameObject.SetActive(false);
        public void OnQuantitySliderChanged(float quantity) => quantityInput.text = quantity.ToString();

        public void OnConfirmInput(string result)
        {
            if (int.TryParse(result, out int num)) quantitySlider.value = Mathf.Clamp(num, (int)quantitySlider.minValue, (int)quantitySlider.maxValue);
            quantityInput.text = quantitySlider.value.ToString();
        }
    }
}