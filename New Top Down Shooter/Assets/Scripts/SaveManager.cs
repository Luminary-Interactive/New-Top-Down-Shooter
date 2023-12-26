using System;
using System.IO;
using TimeStrike.SaveManagement.Serializers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TimeStrike.SaveManagement
{
    public static class SaveManager
    {
        private const string ENCRYPTIONKEY = "rPMAfMYaBXuCcittn1tui5e2eTrna42S";

        private static ISerializer s_serializer = new JSONSerializer(); // Switch this field out to switch serialization system.
        private static string s_pathPrefix;
        /// <summary>
        /// Saves an object to the provided path, with optional encryption.
        /// </summary>
        /// <param name="toSave">The object to save. NOTE: Must be serializable and have the System.Serializable attribute.</param>
        /// <param name="path">A file path to save the object to, this path is appended to Application.persistentDataPath and a Saved Data folder.</param>
        /// <param name="encrypt">Whether the data should be encrypted or not.</param>
        public static void Save(object toSave, string path, bool encrypt)
        {
            s_pathPrefix ??= Application.persistentDataPath + "/Saved Data/";
            string toWrite;

            if (encrypt)
            {
                toWrite = s_serializer.SerializeAndEncrypt(toSave, ENCRYPTIONKEY);
            } else 
            {
                toWrite = s_serializer.SerializeObject(toSave);
            }

            try 
            {
                string totalPath = Path.Combine(s_pathPrefix, path);
                string directoryPath = Path.GetDirectoryName(totalPath);
                if (!Directory.Exists(directoryPath)) 
                { 
                    Directory.CreateDirectory(directoryPath); 
                }
                using (FileStream stream = new FileStream(totalPath, FileMode.Create)) 
                {
                    using (StreamWriter writer = new StreamWriter(stream)) {

                        writer.Write(toWrite);
                    }
                }
            } catch (Exception err) 
            {
                Debug.LogException(err);
                return;
            }
        }
        /// <summary>
        /// Loads a saved object from the players files.
        /// </summary>
        /// <typeparam name="T">The type the loaded data should be casted to.</typeparam>
        /// <param name="path">The path the object is saved at; this path is appended to Application.persistentDataPath and a Saved Data folder.</param>
        /// <param name="encrypted">Whether or not the data is encrypted and should be decrypted before being returned</param>
        /// <returns>Returns a tuple: the first field containing either the loaded object or the error message, the second field a boolean which is true if the loading was successful.</returns>
        public static (object data, bool success) Load<T>(string path, bool encrypted)
        {
            s_pathPrefix ??= Application.persistentDataPath + "/Saved Data/";
            try {
                string totalPath = Path.Combine(s_pathPrefix, path);
                string directoryPath = Path.GetDirectoryName(totalPath);
                string loaded = "";
                if (!File.Exists(totalPath)) return ("No save file found at path.", false);
                if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
                using (FileStream stream = new FileStream(totalPath, FileMode.Open)) 
                {
                    using (StreamReader reader = new StreamReader(stream)) 
                    {
                        loaded = reader.ReadToEnd();
                    }
                }
                if (encrypted)
                {
                    return (s_serializer.DecryptAndDeserialize<T>(loaded, ENCRYPTIONKEY), true);
                } else
                {
                    return (s_serializer.DeserializeObject<T>(loaded), true);
                }

            } catch (Exception err) {
                Debug.LogException(err);
                return (err.Message, false);
            }
        }

        #if UNITY_EDITOR

        [Serializable]
        private class TestSaveData
        {
            public string a;
            public int b;
            public bool c;
            public float d;

            public TestSaveData() 
            {
                a = "Now is the time for all good men to come to the aid of the party.";
                b = 42;
                c = true;
                d = 3.14f;
            }
        }
        [MenuItem("Debug/SaveTestDataNoEncryption")]
        public static void DebugSaveTestDataNoEncrypt()
        {
            TestSaveData tosave = new();
            Save(tosave, "Debug/TestSaveManagerDataNoEncrypt.data", false);
        }
        [MenuItem("Debug/SaveTestDataWithEncryption")]
        public static void DebugSaveTestDataEncrypt()
        {
            TestSaveData tosave = new();
            Save(tosave, "Debug/TestSaveManagerDataEncrypt.data", true);
        }
        [MenuItem("Debug/LoadTestDataNoEncryption")]
        public static void DebugLoadTestDataNoEncrypt()
        {
            var saveData = Load<TestSaveData>("Debug/TestSaveManagerDataNoEncrypt.data", false);
            if (saveData.success)
            {
                TestSaveData loaded = (TestSaveData)saveData.data;
                Debug.Log($"Loaded non encrypted data: A: {loaded.a}, B: {loaded.b}, C: {loaded.c}, D: {loaded.d}");
            }
            else
            {
                Debug.Log(saveData.data);
            }
        }
        [MenuItem("Debug/LoadTestDataWithEncryption")]
        public static void DebugLoadTestDataEncrypted()
        {
            var saveData = Load<TestSaveData>("Debug/TestSaveManagerDataEncrypt.data", true);
            if (saveData.success)
            {
                TestSaveData loaded = (TestSaveData)saveData.data;
                Debug.Log($"Loaded encrypted data: A: {loaded.a}, B: {loaded.b}, C: {loaded.c}, D: {loaded.d}");
            }
            else
            {
                Debug.Log(saveData.data);
            }
        }
        
        #endif
    }
}