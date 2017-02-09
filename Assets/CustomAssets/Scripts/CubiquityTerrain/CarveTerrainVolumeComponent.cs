using UnityEngine;

using Cubiquity;

public class CarveTerrainVolumeComponent : MonoBehaviour {
	public int RangeX = 2;
	public int RangeY = 4;
	public int RangeZ = 2;
	public int SmoothFactor = 2;
	public bool Euclidean = false;
	public bool Erase = true;
	private TerrainVolume _terrainVolume;

	public void Start() {
		_terrainVolume = gameObject.GetComponent<TerrainVolume>();
		if (_terrainVolume == null) {
			Debug.LogError(
				"This 'CarveTerrainVolumeComponent' script should be attached to a game object with a TerrainVolume component");
		}
	}


	public void DoCarveAction(Ray ray) {
		if (_terrainVolume == null) {
			return;
		}

		PickSurfaceResult pickResult;
		bool hit = Picking.PickSurface(_terrainVolume, ray, 1000.0f, out pickResult);

		if (hit) {
			DestroyVoxels((int) pickResult.volumeSpacePos.x, (int) pickResult.volumeSpacePos.y, (int) pickResult.volumeSpacePos.z);
		}
	}


	private void DestroyVoxels(int xPos, int yPos, int zPos) {
		int aux = Mathf.Max(RangeX, RangeY, RangeZ);
		int rangeSquared = aux * aux;
		MaterialSet emptyMaterialSet = new MaterialSet();
		MaterialSet fillMaterialSet = new MaterialSet();
		fillMaterialSet.weights[0] = 255;
		fillMaterialSet.weights[1] = 255;
		fillMaterialSet.weights[2] = 255;

		for (int z = zPos - RangeZ; z < zPos + RangeZ; z++) {
			for (int y = yPos - RangeY; y < yPos + RangeY; y++) {
				for (int x = xPos - RangeX; x < xPos + RangeX; x++) {
					int xDistance = x - xPos;
					int yDistance = y - yPos;
					int zDistance = z - zPos;

					int distSquared = 0;
					if (Euclidean) {
						distSquared = SqrEuclideanDistance(xDistance, yDistance, zDistance);
					}
					else {
						distSquared = ManhattanDistance(xDistance, yDistance, zDistance);
					}

					if (distSquared < rangeSquared) {
						if (Erase) {
							_terrainVolume.data.SetVoxel(x, y, z, emptyMaterialSet);
						}
						else {
							_terrainVolume.data.SetVoxel(x, y, z, fillMaterialSet);
						}
					}
				}
			}
		}

		TerrainVolumeEditor.BlurTerrainVolume(_terrainVolume,
			new Region(
				xPos - RangeX + SmoothFactor,
				yPos - RangeY + SmoothFactor,
				zPos - RangeZ + SmoothFactor,
				xPos + RangeX + SmoothFactor,
				yPos + RangeY + SmoothFactor,
				zPos + RangeZ + SmoothFactor)
		);
	}

	private static int SqrEuclideanDistance(int x, int y, int z) {
		return x * x + y * y + z * z;
	}

	private static int ManhattanDistance(int x, int y, int z) {
		return x + y + z;
	}

}