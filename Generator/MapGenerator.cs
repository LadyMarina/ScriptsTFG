using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { OneNoise, BiomesPatch, BiomesNoise,  BiomesColors, BiomesMesh };
    public int biomeNoseShow;
    public DrawMode drawMode;

  
    int mapChunkSize = 241;

    [Range(0, 6)]
    int levelOfDetail;

    [Header("OVER ALL BIOMES PARAM")]
    [Range(140, 400)]
    public int biomeGrid; 
    public float noiseMultiplayer; 
    public float noiseDistance; 
    public float meshHeightMultiplayer;
    public AnimationCurve meshHeightCurve;
    public int lerpBiomeDistance = 4;

    [Header("SEED SETTINGS")]
    public bool autoUpdate;
    public bool randomSeed;
    public int seed;

    public BiomeType[] biomes;
    int[] regions;

    [Header("SEED SETTINGS")]
    bool oneTime = false;

    private void Start()
    {
        GenerateMap();
    }
   
    public void GenerateMap()
    {
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();


        if (drawMode == DrawMode.BiomesColors)
        {
            Color[] biomesColorMap = GenerateColor();
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(biomesColorMap, mapChunkSize, mapChunkSize));
            mapDisplay.meshObject.SetActive(false);
            mapDisplay.displayObject.SetActive(true);
        }
        else if (drawMode == DrawMode.BiomesNoise)
        {
            Color[] biomesColorMap = GenerateNoiseTexture();
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(biomesColorMap, mapChunkSize, mapChunkSize));
            mapDisplay.meshObject.SetActive(false);
            mapDisplay.displayObject.SetActive(true);
        }
        else if (drawMode == DrawMode.OneNoise)
        {
            float[,] noiseMap = GenerateOneNoise(biomeNoseShow);
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            mapDisplay.meshObject.SetActive(false);
            mapDisplay.displayObject.SetActive(true);
        }
        else if (drawMode == DrawMode.BiomesPatch)
        {
            Color[] biomesColorMap = GeneratePath(); 
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(biomesColorMap, mapChunkSize, mapChunkSize));
            mapDisplay.meshObject.SetActive(false);
            mapDisplay.displayObject.SetActive(true);

        }
        else if (drawMode == DrawMode.BiomesMesh)
        {
            mapChunkSize = 241;
            Color[] biomesColorMap = GenerateColor();
            float[,] noiseMap = GenerateNoise();
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplayer, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(biomesColorMap, mapChunkSize, mapChunkSize));

            mapDisplay.meshObject.SetActive(true);
            mapDisplay.displayObject.SetActive(false);
        }
    }
    public Color[] GenerateNoiseTexture()
    {
        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapChunkSize, mapChunkSize, seed, biomeGrid, biomes.Length, noiseMultiplayer, noiseDistance);

        Color[] biomesColorMap = new Color[mapChunkSize * mapChunkSize];

        float[][,] noiseXBiome = new float[biomes.Length][,];


        for (int i = 0; i < biomes.Length; i++)
        {
            noiseXBiome[i] = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, biomes[i].biome.seed, biomes[i].biome.noiseScale, biomes[i].biome.octaves, biomes[i].biome.persistance, biomes[i].biome.lacunarity, biomes[i].biome.offset);
        }

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                int biome = (int)biomeMap[x, y];
                biomesColorMap[y * mapChunkSize + x] = Color.Lerp(Color.black, Color.white, noiseXBiome[biome][x, y]);
            }
        }

        return biomesColorMap;
    }

    public float[,] GenerateOneNoise(int i)
    {
        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapChunkSize, mapChunkSize, seed, biomeGrid, biomes.Length, noiseMultiplayer, noiseDistance);

        biomeMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, biomes[i].biome.seed, biomes[i].biome.noiseScale, biomes[i].biome.octaves, biomes[i].biome.persistance, biomes[i].biome.lacunarity, biomes[i].biome.offset);

        return biomeMap;
    }
    public float[,] GenerateNoise()
    {
        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapChunkSize, mapChunkSize, seed, biomeGrid, biomes.Length, noiseMultiplayer, noiseDistance);

        float[][,] noiseXBiome = new float[biomes.Length][,];

        bool newObjective = false;
        float objectiveHight = 0;
        float mult = 0;
        int toObjective = 0;

        for (int i = 0; i < biomes.Length; i++)
        {
            noiseXBiome[i] = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, biomes[i].biome.seed, biomes[i].biome.noiseScale, biomes[i].biome.octaves, biomes[i].biome.persistance, biomes[i].biome.lacunarity, biomes[i].biome.offset);
        }


        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {


                int biome = (int)biomeMap[x, y];
                int nextBiome = biome;


                if (x < mapChunkSize - lerpBiomeDistance)
                {
                    nextBiome = (int)biomeMap[x + lerpBiomeDistance, y];

                    if (nextBiome != biome)
                    {
                      
                        if (newObjective == false)
                        {
                            newObjective = true;
                            objectiveHight = noiseXBiome[nextBiome][x + lerpBiomeDistance, y];
                            mult = objectiveHight / lerpBiomeDistance;
                            toObjective = 0;
                        }
                        //HACIA ARRIBA
                        if (objectiveHight > noiseXBiome[biome][x, y])
                        {
                            if (toObjective == lerpBiomeDistance - 1)
                            {
                                biomeMap[x, y] = objectiveHight;
                            }
                            else
                            {
                                toObjective++;
                                biomeMap[x, y] = Mathf.Min(noiseXBiome[biome][x, y] + mult, objectiveHight);
                                mult += mult;

                            }
                        }
                        else //abajo
                        {
                            if (toObjective == lerpBiomeDistance - 1)
                            {
                                biomeMap[x, y] = objectiveHight;
                            }
                            else
                            {
                                toObjective++;
                                biomeMap[x, y] = Mathf.Max(noiseXBiome[biome][x, y] - mult, objectiveHight);
                                mult += mult;

                            }
                        }
                    }
                    else
                    {
                        biomeMap[x, y] = noiseXBiome[biome][x, y];
                        newObjective = false;
                    }
                } 
                else
                {
                        biomeMap[x, y] = noiseXBiome[biome][x, y];
                        biomeMap[x, y] = noiseXBiome[biome][x, y];
                }
            }
        }

        return biomeMap;
    }

    public Color[] GeneratePath()
    {
        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapChunkSize, mapChunkSize, seed, biomeGrid, biomes.Length, noiseMultiplayer, noiseDistance);

        Color[] biomesColorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                biomesColorMap[y * mapChunkSize + x] = biomes[(int)biomeMap[x, y]].referenceColor;

            }
        }
        return biomesColorMap;
    }
    public Color[] GenerateColor()
    {

        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapChunkSize, mapChunkSize, seed, biomeGrid, biomes.Length, noiseMultiplayer, noiseDistance);

        Color[] biomesColorMap = new Color[mapChunkSize * mapChunkSize];

        float[][,] noiseXBiome = new float[biomes.Length][,];


        for (int i = 0; i < biomes.Length; i++)
        {
            noiseXBiome[i] = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, biomes[i].biome.seed, biomes[i].biome.noiseScale, biomes[i].biome.octaves, biomes[i].biome.persistance, biomes[i].biome.lacunarity, biomes[i].biome.offset);

        }

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {

                Biome currentBiome = biomes[(int)biomeMap[x, y]].biome;
                int biome = (int)biomeMap[x, y];
                for (int i = 0; i < currentBiome.regions.Length; i++)
                {

                    float currentHeight = noiseXBiome[biome][x, y];

                    if (currentHeight <= biomes[(int)biomeMap[x, y]].biome.regions[i].height)
                    {
                        biomesColorMap[y * mapChunkSize + x] = biomes[(int)biomeMap[x, y]].biome.regions[i].color;
                    }


                }
            }
        }

        return biomesColorMap;
    }

   
   
}



[System.Serializable]
public struct BiomeType
{
    public string name;
    public Color referenceColor;
    public Biome biome;
}


[System.Serializable]
public struct Biome 
{
    public bool randomSeed;
    public int seed;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public Regions[] regions;

}



[System.Serializable]
public struct Regions
{
    public float height;
    public Color color;
}
