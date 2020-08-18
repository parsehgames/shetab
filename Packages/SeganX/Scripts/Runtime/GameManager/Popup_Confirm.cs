using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    public class Popup_Confirm : GameState
    {
        [SerializeField] private float minHeight = 200;
        [SerializeField] private float maxHeight = 450;
        [SerializeField] private float buttonSize = 55;
        [SerializeField] private RectTransform panel = null;
        [SerializeField] private Text descLabel = null;
        [SerializeField] private Button okButton = null;
        [SerializeField] private Button cancelButton = null;
        [SerializeField] private Button backgroundCancelButton = null;

        private System.Action<bool> callbackFunc = null;

        public Popup_Confirm Setup(bool displayOkButton, bool displayCancelButton, System.Action<bool> callback)
        {
            callbackFunc = callback;
            okButton.gameObject.SetActive(displayOkButton);
            cancelButton.gameObject.SetActive(displayCancelButton);
            okButton.onClick.AddListener(() => Close(true));
            cancelButton.onClick.AddListener(() => Close(false));
            if (displayCancelButton && backgroundCancelButton) backgroundCancelButton.onClick.AddListener(() => Close(false));

            if (displayOkButton)
            {
                var botEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Bottom);
                descLabel.rectTransform.SetEdge(RectTransform.Edge.Bottom, botEdge + buttonSize);
            }

            return this;
        }

#if SX_PARSI
        public Popup_Confirm SetText(int stringId, params object[] args)
        {
            return SetText(LocalizationService.Get(stringId), args);
        }
#endif

        public Popup_Confirm SetText(string desc, params object[] args)
        {
            var str = args == null ? desc : string.Format(desc, args);
#if SX_PARSI
            if (LocalizationService.IsPersian)
                descLabel.SetTextAndWrap(str, false, LocalizationService.IsPersian);
            else
#endif
                descLabel.text = str;
            return this;
        }

#if SX_PARSI
        public Popup_Confirm Setup(int stringId, bool displayOkButton, bool displayCancelButton, System.Action<bool> callback)
        {
            return Setup(LocalizationService.Get(stringId), displayOkButton, displayCancelButton, callback);
        }
#endif

        public Popup_Confirm Setup(string desc, bool displayOkButton, bool displayCancelButton, System.Action<bool> callback)
        {
            return Setup(displayOkButton, displayCancelButton, callback).SetText(desc);
        }

        private void Start()
        {
            var topEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Top);
            var botEdge = descLabel.rectTransform.GetEdge(RectTransform.Edge.Bottom);
            var panelheight = Mathf.Clamp(topEdge + descLabel.preferredHeight + botEdge, minHeight, maxHeight);
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelheight);
            UiShowHide.ShowAll(transform);
        }

        public override void Back()
        {
            Close(false);
        }

        private void Close(bool isok)
        {
            base.Back();
            callbackFunc?.Invoke(isok);
        }
    }
}