﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
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
    public List<AudioClip> audioClips;
    public bool isReplaced;
    public float volumeBase;
    public float volumeVar;
    public float pitchBase;
    public float pitchVar;
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
        public Dictionary<MaterialClip, MaterialClipsInfo> materialClipsDict = new Dictionary<MaterialClip, MaterialClipsInfo>();
        public GameObject instantiatedDataObject;

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
            }
            else
                ModLogger.Log("Can't load data file");
        }

        public void SetupData()
        {
            instantiatedDataObject = GameObject.Instantiate(_audioReplacerDataObject);
            UnityEngine.Object.DontDestroyOnLoad(instantiatedDataObject);

            PopulateMaterialClipsDictionnary();
        }

        public void PopulateMaterialClipsDictionnary()
        {
            ModLogger.Log("Populating material clips dictionnary.");
            for (int i = 0; i < Enum.GetNames(typeof(MaterialClip)).Length; i++) //Checking assets for each material
            {
                MaterialClip _currentMaterial = (MaterialClip)i;

                string _currentMaterialName = _currentMaterial.ToString();

                ModLogger.Log("Finding object for material " + _currentMaterialName);

                Transform _transform = instantiatedDataObject.transform.FindChild(_currentMaterialName + "_Parent");

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

                                AudioSource[] _audioSources = _assetObject.GetComponents<AudioSource>();
                                ModLogger.Log($"{_audioSources.Length} AudioSources found on gameObject {_assetObject.name}");
                                _audioProperties.audioClips = new List<AudioClip>();
                                foreach (AudioSource _audioSource in _audioSources)
                                    _audioProperties.audioClips.Add(_audioSource.clip);

                                _materialClipsInfo.assetPropertiesDict.Add((AssetType)j, _audioProperties); //Adding assets properties to the material information dictionnary
                                ModLogger.Log($"Properties found for asset type {(AssetType)j} with properties:\n" +
                                    $"audioClips = {_audioProperties.audioClips.Count}\n" +
                                    $"isReplaced = {_audioProperties.isReplaced}\n" +
                                    $"volumeBase = {_audioProperties.volumeBase}\n" +
                                    $"volumeVar = {_audioProperties.volumeVar}\n" +
                                    $"pitchBase = {_audioProperties.pitchBase}\n" +
                                    $"pitchVar = {_audioProperties.pitchVar}\n");
                            }
                            else ModLogger.Log($"TextAsset not found.");
                        }
                        else ModLogger.Log($"Properties for {(AssetType)j} not found.");
                    }

                    materialClipsDict.Add(_currentMaterial, _materialClipsInfo); //Adding the material audio information to the material dictionnary
                }
                else
                    ModLogger.Log($"Parent object for {(MaterialClip)i} not found.");
            }
            ModLogger.Log($"Material clips dictionnary populated with {materialClipsDict.Keys.Count} elements.");
        }

        public AudioProperties ParseTextAsset(Text in_textAsset)
        {
            AudioProperties _audioInfo = new AudioProperties();

            string textStr = in_textAsset.text;

            _audioInfo.isReplaced = bool.Parse(Regex.Split(Regex.Split(textStr, "<isReplaced>")[1], ";")[0]);
            _audioInfo.volumeBase = ParseFloatInText(textStr, "<VolumeBase>");
            _audioInfo.volumeVar = ParseFloatInText(textStr, "<VolumeVar>");
            _audioInfo.pitchBase = ParseFloatInText(textStr, "<PitchBase>");
            _audioInfo.pitchVar = ParseFloatInText(textStr, "<PitchVar>");

            ModLogger.Log($"isReplaced = {_audioInfo.isReplaced}");
            ModLogger.Log($"volumeBase = {_audioInfo.volumeBase}");
            ModLogger.Log($"volumeVar = {_audioInfo.volumeVar}");
            ModLogger.Log($"pitchBase = {_audioInfo.pitchBase}");
            ModLogger.Log($"pitchVar = {_audioInfo.pitchVar}");

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
                   $"audioClips = {audioProperties.audioClips[0].name}\n" +
                   $"isReplaced = {audioProperties.isReplaced}\n" +
                   $"volumeBase = {audioProperties.volumeBase}\n" +
                   $"volumeVar = {audioProperties.volumeVar}\n" +
                   $"pitchBase = {audioProperties.pitchBase}\n" +
                   $"pitchVar = {audioProperties.pitchVar}\n";
        }

        public AudioProperties GetAudioProperties(MaterialClip in_materialClip, AssetType in_assetType)
        {
            MaterialClipsInfo _materialClipInfo;
            AudioProperties _audioProperties;

            ModLogger.Log($"Trying to get properties for material {in_materialClip} and asset type {in_assetType}");

            if (materialClipsDict.TryGetValue(in_materialClip, out _materialClipInfo))
            {
                ModLogger.Log($"MaterialClipInfo for material {in_materialClip} found");
                if (_materialClipInfo.assetPropertiesDict.TryGetValue(in_assetType, out _audioProperties))
                {
                    ModLogger.Log($"Properties for asset {in_assetType} found");
                    return _audioProperties;
                }
                else ModLogger.Log($"Properties for asset {in_assetType} not found");
            }
            else ModLogger.Log($"MaterialClipInfo for material {in_materialClip} not found");

            _audioProperties = new AudioProperties();
            _audioProperties.isReplaced = false;

            return _audioProperties;    
        }
    }
}
