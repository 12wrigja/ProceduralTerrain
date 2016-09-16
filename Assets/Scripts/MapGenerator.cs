using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour {

	public enum DrawMode {
		NoiseMap,
		ColorMap,
		Mesh
	}

	public const int mapChunkSize = 241;

	public DrawMode drawMode;

	//LOD will be multiplied by 2 to get the actual scaling change. Unless it's 0, then it will be manually adjusted to 1.
	[Range(0,6)]
	public int LevelOfDetail;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public bool randomSeedOnGenerate = false;

	public Vector2 offset;

	public bool AutoUpdate = false;

	public TerrainType[] regions;

	public void GenerateMap(){

		if(randomSeedOnGenerate){
			seed = (new System.Random()).Next();
		}

		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize,mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colorMap = new Color[mapChunkSize*mapChunkSize];

		for(int y=0; y<mapChunkSize; y++){
			for(int x=0; x<mapChunkSize; x++){
				float currentHeight = noiseMap[x,y];
				for(int i=0; i<regions.Length; i++){
					if(currentHeight <= regions[i].height){
						colorMap[y*mapChunkSize + x] = regions[i].color;
						break;
					}
				}
			}
		}
		MapDisplay disp = GetComponent<MapDisplay>();
		if(drawMode == DrawMode.NoiseMap){
			disp.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		} else if (drawMode == DrawMode.ColorMap){
			disp.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh){
			disp.DrawMesh(MeshGenerator.GenerateTerrainMeshData(noiseMap, meshHeightMultiplier, meshHeightCurve, LevelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
		}
		
	}

	void OnValidate(){
		if(lacunarity < 1){
			lacunarity = 1;
		}
		if(octaves < 1){
			octaves = 1;
		}
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color color;
}
