using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEditor;
using System.Reflection;
using System;

namespace VibePack.Utility
{
    /// <summary>
    /// Custom class for logging.
    /// </summary>
    public class Oracle : MonoBehaviour
    {
        /// <summary>
        /// Determines wether or not messages get logged.
        /// </summary>
        [SerializeField] bool printActive;
        /// <summary>
        /// Determines wether or not messages are displayed.
        /// </summary>
        [SerializeField] bool displayActive;
        /// <summary>
        /// Text mesh to display messaes in.
        /// </summary>
        [SerializeField] TextMeshProUGUI displayText;

        /// <summary>
        /// Instance for singleton.
        /// </summary>
        static Oracle instance;

        /// <summary>
        /// Stores all the active display messages.
        /// </summary>
        readonly Dictionary<string, string> displayMessages = new Dictionary<string, string>();

        /// <summary>
        /// Logger used to print messages.
        /// </summary>
        ILogger logger;

        /// <summary>
        /// Initializes the class singleton and logger. Don't call this method manually unless you know what you're doing.
        /// </summary>
        public void Initialize()
        {
            instance = this;
            logger = Debug.unityLogger;
        }

        /// <summary>
        /// Recieves an array to format and log.
        /// </summary>
        /// <param name="key">Identifier Key.</param>
        /// <param name="parameters">Objects to be logged.</param>
        private void PrintMessages(string key, object[] parameters)
        {
            if (!printActive)
                return;
            
            logger.Log(LogType.Log, parameters is { Length: 0 } ? $"==> {key}" : $"==> {key}, {Format(parameters)}");
        }

        /// <summary>
        /// Displays a message.
        /// </summary>
        /// <param name="key">Key to modify</param>
        /// <param name="parameters">Objects to display.</param>
        private void UpdateMessage(string key, object[] parameters)
        {
            if (!displayActive || !displayMessages.ContainsKey(key))
                return;

            displayMessages[key] = Format(parameters);
            Display();
        }

        /// <summary>
        /// Updates the on-screen display.
        /// </summary>
        private void Display() => displayText.text = Enumerable.Aggregate(displayMessages.ToArray(), "", (x, y) => $"{x}==> {y.Key}, {y.Value}\n");

        /// <summary>
        /// Formats an array of objects into a single string.
        /// </summary>
        /// <param name="parameters">Parameters to format.</param>
        /// <returns>Formatted string.</returns>
        private string Format(object[] parameters) => Enumerable.Aggregate(parameters, "", (x, y) => $"{x}{y}, ")[..^2];

        /// <summary>
        /// Adds a key to the displayMessages dictionary.
        /// </summary>
        /// <param name="key">Key to add.</param>
        private void AddKeyToMessages(string key)
        {
            if (!displayActive)
                return;

            displayMessages.TryAdd(key, "");
            Display();
        }

        /// <summary>
        /// Removes a key from the displayMessages dictionary.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        private void RemoveKeyFromMessages(string key)
        {
            if (!displayActive)
                return;

            displayMessages.Remove(key);
            Display();
        }

        /// <summary>
        /// Recieves a set of strings and prints them out in format.
        /// </summary>
        /// <param name="key">Identifier Key.</param>
        /// <param name="parameters">Objects to be logged.</param>
        public static void Log(string key, params object[] parameters) => instance.PrintMessages(key, parameters);

        public static void EditorLog(string parameter) => instance.PrintMessages("EDITOR", new object[] { parameter });

        /// <summary>
        /// Adds a key to the displayMessages dictionary.
        /// </summary>
        /// <param name="key">Key to add.</param>
        public static void AddKey(string key) => instance.AddKeyToMessages(key);

        /// <summary>
        /// Updates a keys value.
        /// </summary>
        /// <param name="key">Key to update.</param>
        /// <param name="parameters">Text to display.</param>
        public static void UpdateKey(string key, params object[] parameters) => instance.UpdateMessage(key, parameters);

        /// <summary>
        /// Removes a key from the displayMessages dictionary.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        public static void RemoveKey(string key) => instance.RemoveKeyFromMessages(key);

        /// <summary>
        /// Clears Unity's Console
        /// </summary>
        public static void ClearLogs()
        {
# if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }
    }
}
