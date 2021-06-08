using System;
using Common.Helpers.UI.BaseUiElements;
using UnityEngine;

namespace SceneScripts.MainMenu
{
    public class MainMenuController : BaseWindowController<MainMenuController>
    {
        [SerializeField] private Canvas optionsCanvas;
        [SerializeField] private Camera optionsCamera;

        protected override void InheritAwake()
        {
            base.InheritAwake();
        }


        public override Coroutine Open(Action onOpened = null)
        {
            if (onOpened == null)
            {
                onOpened = () => Init();
            }
            else
            {
                onOpened += () => Init();
            }

            optionsCanvas.gameObject.SetActive(true);
            optionsCamera.gameObject.SetActive(true);

            return base.Open(onOpened);
        }

        public override Coroutine Close(Action onClosed = null)
        {
            MainMenuController.Instance?.Open();

            return base.Close(() =>
            {
                optionsCanvas.gameObject.SetActive(false);
                optionsCamera.gameObject.SetActive(false);

                onClosed?.Invoke();
            });
        }

        void Init()
        {

        }
    }
}
