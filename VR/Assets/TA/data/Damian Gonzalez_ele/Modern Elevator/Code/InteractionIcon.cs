using UnityEngine;
using UnityEngine.UI;

namespace ModernElevator {
    public class InteractionIcon : MonoBehaviour {
        public static InteractionIcon inst; //global access
        private Image thisIcon;
        private GameObject tip;
        [SerializeField] private Color noInteractableColor = new Color(1, 1, 1, .1f);
        [SerializeField] private Color interactableColor = Color.green;

        void Awake() {
            inst = this;
            thisIcon = GetComponent<Image>();
            tip = transform.GetChild(0)?.gameObject;
        }


        public void SetInteractable(bool value) {
            if (thisIcon != null) thisIcon.color = value ? interactableColor : noInteractableColor;
            if (tip != null) tip.SetActive(value);
        }

    }
}
