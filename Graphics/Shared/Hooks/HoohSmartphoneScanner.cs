using System.Collections;
using UnityEngine;

namespace Graphics
{
    public class HoohSmartphoneScanner : MonoBehaviour
    {
        private bool running;
        private Coroutine scanCoroutine;

        private void OnEnable()
        {
            running = true;
            scanCoroutine = StartCoroutine(ScanForSmartPhones());
        }

        private IEnumerator ScanForSmartPhones()
        {
            while (running)
            {
                GameObject commonSpace = GameObject.Find("CommonSpace");
                if (commonSpace != null)
                {
                    foreach (Transform child in commonSpace.transform)
                    {
                        if (child.gameObject.name == "camera_colorable")
                        {
                            Camera cam = child.gameObject.GetComponentInChildren<Camera>();
                            if (cam != null && cam.GetComponent<SSS>() == null)
                            {
                                SSS camSSS = cam.gameObject.AddComponent<SSS>();
                                camSSS.enabled = true;
                                camSSS.Enabled = true;
                                SSSManager.RegisterAdditionalInstance(camSSS);
                                Graphics.Instance.Log.LogInfo($"Adding SSS Component to Camera: {cam.name} GO: {cam.gameObject.name}");
                            }
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(5);
            }
        }

        private void OnDisable()
        {
            running = false;
            StopCoroutine(scanCoroutine);
        }

    }
}
