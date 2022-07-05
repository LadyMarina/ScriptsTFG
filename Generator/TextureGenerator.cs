using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator 
{
    public static Texture2D TextureFromColorMap(Color[] map, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(map);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, map[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TexturesFromHeightsMap(float[][,] map, BiomeType[] biomes, int mapWidth, int mapHeight, MapGenerator mapGen)
    {
        int width = mapWidth;
        int height = mapHeight;

        float[,] biomeMap = Noise.GenerateVoronoiNoise(mapWidth, mapHeight, mapGen.seed, mapGen.biomeGrid, biomes.Length, mapGen.noiseMultiplayer, mapGen.noiseDistance);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                Biome currentBiome = biomes[(int)biomeMap[x, y]].biome;
                int biome = (int)biomeMap[x, y];
                for (int i = 0; i < currentBiome.regions.Length; i++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, map[i][x, y]);
                }
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
