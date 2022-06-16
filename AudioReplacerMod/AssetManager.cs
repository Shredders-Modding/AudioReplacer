using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

public struct AudioInfo
{
    public AudioClip[] audioClips;
    public float volumeMin;
    public float volumeMax;
    public float pitchMin;
    public float pitchMax;
}

public struct MaterialClipsInfo
{
    public AudioInfo forwardInfo;
    public AudioInfo turnInfo;
    public AudioInfo landingHardInfo;
    public AudioInfo landingSoftInfo;
}

namespace AudioReplacerMod
{
    class AssetManager
    {
        private GameObject _audioReplacerDataObject;
        public GameObject instantiatedMenu;
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
                ModLogger.Log("AudioReplacerDataObject loaded.");
            }
            else
                ModLogger.Log("Can't load data file");
        }

        public void PopulateMaterialClipsDictionnary()
        {
            for (int i = 0; i < Enum.GetNames(typeof(MaterialClip)).Length; i++)
            {
                MaterialClip _currentMaterial = (MaterialClip)i;
                string _currentMaterialName = _currentMaterial.ToString();
                ModLogger.Log("Finding object for material " + _currentMaterialName);
                GameObject _materialParent = _audioReplacerDataObject.transform.FindChild(_currentMaterialName + "_Parent").gameObject;
                if (_materialParent)
                {
                    ModLogger.Log(_materialParent.name + " object found");

                    MaterialClipsInfo _materialClipsInfo = new MaterialClipsInfo();

                    GameObject _forwardObject = _materialParent.transform.FindChild("Forward_Properties").gameObject;
                    GameObject _turnObject = _materialParent.transform.FindChild("Turn_Properties").gameObject;
                    GameObject _landingHardObject = _materialParent.transform.FindChild("LandingHard_Properties").gameObject;
                    GameObject _landingSoftObject = _materialParent.transform.FindChild("LandingSoft_Properties").gameObject;


                    //_materialClipsInfo.forwardInfo.audioClips;
                }
            }
        }
    }
}
