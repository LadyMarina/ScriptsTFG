using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditorBiome : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGenerator.autoUpdate)
            {

                if (mapGenerator.randomSeed)
                {
                    mapGenerator.seed = Random.Range(0, 500);

                }

                mapGenerator.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            if (mapGenerator.randomSeed)
            {
                mapGenerator.seed = Random.Range(0, 500);

            }

            mapGenerator.GenerateMap();
        }
    }
}

