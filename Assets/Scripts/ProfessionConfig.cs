using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class ProfessionConfig : ScriptableObject
{
    public class Entry
    {
        public ProfessionType type;
        public Sprite sprite;
    }
    [SerializeField] List<Entry> entries = new List<Entry>();

    public bool GetEntry(ProfessionType type, out Entry entry)
    {
        entry = entries.FirstOrDefault(e => e.type == type);
        return entry != null;
    }
}
