using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveData
{
    public Vector3 PlayerPosition;
    public float health;

    public int equippedAmmo;
}

public class SaveLoad : MonoBehaviour
{
    string savepath;

    [SerializeField] GameObject noLoadWarning;

    private void Start()
    {
        savepath = Application.dataPath + "/Saves/Save.json";
    }
    public void SaveGame()
    {
        SaveData _saveData = new SaveData();
        FPSController player = FindObjectOfType<FPSController>();

        _saveData.PlayerPosition = player.transform.position;
        _saveData.health = player.Health;

        if (player.GetComponentInChildren<Gun>() != null)
            _saveData.equippedAmmo = player.GetComponentInChildren<Gun>().Ammo;

        string jsonSave = JsonUtility.ToJson(_saveData);
        File.WriteAllText(savepath, jsonSave);
    }

    public void LoadGame()
    {
        try
        {
            string jsonSave = File.ReadAllText(savepath);
            var player = FindObjectOfType<FPSController>();

            SaveData _loadData = JsonUtility.FromJson<SaveData>(jsonSave);

            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = _loadData.PlayerPosition;
            player.GetComponent<CharacterController>().enabled = true;

            if (player.GetComponentInChildren<Gun>() != null)
                player.GetComponentInChildren<Gun>().Ammo = _loadData.equippedAmmo;

            player.Health = _loadData.health;

        }
        catch
        {
            StartCoroutine("NoLoadFound");
        }
    }

    IEnumerator NoLoadFound() //has to be a coroutine because TimeScale is 0
    {
        var nl = Instantiate(noLoadWarning, this.transform);
        yield return new WaitForSecondsRealtime(1);
        Destroy(nl);
    }
}
