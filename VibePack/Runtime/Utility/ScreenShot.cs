using System.Collections;
using UnityEngine;
using System.IO;
using System;
/*
namespace VibePack.Utility
{
    public class ScreenShot : MonoBehaviour
    {
        public static ScreenShot instance;

        //for android
        private bool isProcessing = false;
        public string message;

        private void Awake() => Initialize();

        private void Start()
        {
            ButtonShare();
            print(Application.persistentDataPath);
        }

        private void Initialize()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void ButtonShare()
        {
            if (isProcessing)
                return;

            StartCoroutine(ShareScreenshot());
        }

        private IEnumerator ShareScreenshot()
        {
            isProcessing = true;
            yield return new WaitForEndOfFrame();

            Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            byte[] dataToSave = screenTexture.EncodeToPNG();
            string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
            File.WriteAllBytes(destination, dataToSave);

            if (!Application.isEditor)
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

                intentObject.Call<AndroidJavaObject>("setType", "text/plain");
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "" + message);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");

                intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

                currentActivity.Call("startActivity", intentObject);
            }

            isProcessing = false;
        }

        //for iOS
        public static event Action ScreenshotFinishedSaving;
        public static event Action ImageFinishedSaving;

        public static string savedImagePath = string.Empty;

    #if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern bool saveToGallery (string path);
    #endif

        public static IEnumerator Save(string fileName, string albumName = "MyScreenshots", bool callback = false)
        {
    #if UNITY_IPHONE

            bool photoSaved = false;
            string date = System.DateTime.Now.ToString ("dd-MM-yy");

            ScreenshotHandler.ScreenShotNumber++;

            string screenshotFilename = fileName + "_" + ScreenshotHandler.ScreenShotNumber + "_" + date + ".png";

            Debug.Log ("Save screenshot " + screenshotFilename);


            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                Debug.Log ("iOS platform detected");

                string iosPath = Application.persistentDataPath + "/" + fileName;
                savedImagePath = iosPath;
                Application.CaptureScreenshot (screenshotFilename);

                while (!photoSaved) {
                    photoSaved = saveToGallery (iosPath);
                    yield return new WaitForSeconds (.5f);
                }				

                iPhone.SetNoBackupFlag (iosPath);

            }
            else
                Application.CaptureScreenshot (screenshotFilename);


        #endif
            yield return 0;
            if (callback)
                ScreenshotFinishedSaving();
            }


        public static IEnumerator SaveExisting(string filePath, bool callback = false)
        {
            yield return 0;
    #if UNITY_IPHONE
            bool photoSaved = false;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                while (!photoSaved)
                {
                    photoSaved = saveToGallery(filePath);
                    yield return new WaitForSeconds(0.5f);
                }

                iPhone.SetNoBackupFlag(filePath);
            }
    #endif

            if (callback)
                ImageFinishedSaving();
            }

            public static int ScreenShotNumber
            {
                set => PlayerPrefs.SetInt("screenShotNumber", value);
                get => PlayerPrefs.GetInt("screenShotNumber");
            }
    }
}*/