using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum MaterialClip
{
    BigPipes,
    Concrete,
    Course,
    DeepSnow,
    JibbingBox,
    Rails,
    Rock,
    SmallRail,
    SoftFence,
}

public enum AssetType
{
    Forward,
    Turn,
    LandingHard,
    LandingSoft,
}

public struct AudioProperties
{
    public AudioClip[] audioClips;
    public bool isReplaced;
    public float volumeMin;
    public float volumeMax;
    public float pitchMin;
    public float pitchMax;
}

public struct MaterialClipsInfo
{
    public AudioProperties forwardInfo;
    public AudioProperties turnInfo;
    public AudioProperties landingHardInfo;
    public AudioProperties landingSoftInfo;

    public Dictionary<AssetType, AudioProperties> assetPropertiesDict;
}

namespace AudioReplacerMod
{
    class AssetManager
    {
        private GameObject _audioReplacerDataObject;
        private GameObject _materialParent;
        public Dictionary<MaterialClip, MaterialClipsInfo> materialClipsDict = new Dictionary<MaterialClip, MaterialClipsInfo>();

        public void Init()
        {
            ModLogger.Log("Initializing AssetManager");
            ModLogger.Log("Finding data file");

            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Path.GetFullPath("."), "UserData/"));
            FileInfo[] files = dir.GetFiles("*.audio");

            if (files.Length > 0)
                ModLogger.Log($"Data file found with name: {files[0].Name}");
            else
                ModLogger.Log("No data file found");


            AssetBundle modDataBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetFullPath("."), $"UserData/{files[0].Name}"));
           
            if (modDataBundle)
            {
                ModLogger.Log($"{files[0].Name} data file loaded");
                var audioReplacerDataObject = modDataBundle.LoadAsset("AudioReplacerData");
                _audioReplacerDataObject = audioReplacerDataObject.Cast<GameObject>();
                ModLogger.Log($"AudioReplacerDataObject loaded. Object has name: {_audioReplacerDataObject.name}");
                ModLogger.Log("Populating material clips dictionnary.");
                PopulateMaterialClipsDictionnary();
                ModLogger.Log($"Material clips dictionnary populated with {materialClipsDict.Keys.Count} elements.");
            }
            else
                ModLogger.Log("Can't load data file");
        }

        public void PopulateMaterialClipsDictionnary()
        {
            for (int i = 0; i < Enum.GetNames(typeof(MaterialClip)).Length; i++) //Checking assets for each material
            {
                MaterialClip _currentMaterial = (MaterialClip)i;

                string _currentMaterialName = _currentMaterial.ToString();

                ModLogger.Log("Finding object for material " + _currentMaterialName);

                Transform _transform = _audioReplacerDataObject.transform.FindChild(_currentMaterialName + "_Parent");

                if (_transform)
                {
                    GameObject _materialParent = _transform.gameObject;

                    ModLogger.Log(_materialParent.name + " parent object found.");

                    MaterialClipsInfo _materialClipsInfo = new MaterialClipsInfo();
                    _materialClipsInfo.assetPropertiesDict = new Dictionary<AssetType, AudioProperties>();

                    for (int j = 0; j < Enum.GetNames(typeof(AssetType)).Length; j++) //Checking properties for each asset type
                    {
                        ModLogger.Log($"Checking properties for asset type: {(AssetType)j}.");

                        _transform = _materialParent.transform.Find((AssetType)j + "_Properties");
                        
                        if (_transform)
                        {
                            GameObject _assetObject = _transform.gameObject;
                            ModLogger.Log($"Properties found.");
                            Text _textAsset = _assetObject.GetComponent<Text>();
                            if (_textAsset)
                            {
                                ModLogger.Log($"TextAsset found.");
                                AudioProperties _audioProperties = ParseTextAsset(_textAsset);
                                _materialClipsInfo.assetPropertiesDict.Add((AssetType)j, _audioProperties); //Adding assets properties to the material information dictionnary
                                ModLogger.Log($"Properties found for asset type {(AssetType)j} with properties:\n" +
                                    $"audioClips = {_audioProperties.audioClips}\n" +
                                    $"isReplaced = {_audioProperties.isReplaced}\n" +
                                    $"volumeMin = {_audioProperties.volumeMin}\n" +
                                    $"volumeMax = {_audioProperties.volumeMax}\n" +
                                    $"pitchMin = {_audioProperties.pitchMin}\n" +
                                    $"pitchMax = {_audioProperties.pitchMax}\n");
                            }
                            else ModLogger.Log($"TextAsset not found.");
                        }
                        else ModLogger.Log($"Properties for {(AssetType)j} not found.");
                    }

                    materialClipsDict.Add(_currentMaterial, _materialClipsInfo); //Adding the material audio information to the material dictionnary

                    /*
                    GameObject _forwardObject = _materialParent.transform.FindChild("Forward_Properties").gameObject;
                    if (_forwardObject)
                    {
                        Text _textAsset = _forwardObject.GetComponent<Text>();
                        if (_textAsset)
                        {
                            AudioProperties _audioProperties = ParseTextAsset(_textAsset);
                            _materialClipsInfo.forwardInfo.isReplaced = _audioProperties.isReplaced;
                            _materialClipsInfo.forwardInfo.volumeMin = _audioProperties.volumeMin;
                        }
                    }
                    GameObject _turnObject = _materialParent.transform.FindChild("Turn_Properties").gameObject;
                    GameObject _landingHardObject = _materialParent.transform.FindChild("LandingHard_Properties").gameObject;
                    GameObject _landingSoftObject = _materialParent.transform.FindChild("LandingSoft_Properties").gameObject;
                    */
                }
                else
                    ModLogger.Log($"Parent object for {(MaterialClip)i} not found.");
            }
        }

        public AudioProperties ParseTextAsset(Text in_textAsset)
        {
            AudioProperties _audioInfo = new AudioProperties();

            string textStr = in_textAsset.text;

            _audioInfo.isReplaced = bool.Parse(Regex.Split(Regex.Split(textStr, "<isReplaced>")[1], ";")[0]);
            _audioInfo.volumeMin = ParseFloatInText(textStr, "<VolumeMin>");
            _audioInfo.volumeMax = ParseFloatInText(textStr, "<VolumeMax>");
            _audioInfo.pitchMin = ParseFloatInText(textStr, "<PitchMin>");
            _audioInfo.pitchMax = ParseFloatInText(textStr, "<PitchMax>");

            ModLogger.Log($"isReplaced = {_audioInfo.isReplaced}");
            ModLogger.Log($"volumeMin = {_audioInfo.volumeMin}");
            ModLogger.Log($"isReplaced = {_audioInfo.volumeMax}");
            ModLogger.Log($"volumeMin = {_audioInfo.pitchMin}");
            ModLogger.Log($"volumeMin = {_audioInfo.pitchMax}");

            return _audioInfo;
        }

        public float ParseFloatInText(string in_text, string in_pattern)
        {
            if (Regex.Split(in_text, in_pattern).Length > 1)
                if (Regex.Split(Regex.Split(in_text, in_pattern)[1], ";").Length > 0)
                    return float.Parse(Regex.Split(Regex.Split(in_text, in_pattern)[1], ";")[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            return -1;
        }

        public string GetAudioPropertiesAsString(AudioProperties audioProperties)
        {

            return $"Properties:\n" +
                   $"audioClips = {audioProperties.audioClips}\n" +
                   $"isReplaced = {audioProperties.isReplaced}\n" +
                   $"volumeMin = {audioProperties.volumeMin}\n" +
                   $"volumeMax = {audioProperties.volumeMax}\n" +
                   $"pitchMin = {audioProperties.pitchMin}\n" +
                   $"pitchMax = {audioProperties.pitchMax}\n";
        }

        public AudioProperties GetAudioProperties(MaterialClip in_materialClip, AssetType in_assetType)
        {
            MaterialClipsInfo _materialClipInfo;
            materialClipsDict.TryGetValue(in_materialClip, out _materialClipInfo);

            AudioProperties _audioProperties;
            _materialClipInfo.assetPropertiesDict.TryGetValue(in_assetType, out _audioProperties);

            return _audioProperties;    
        }
    }
}
