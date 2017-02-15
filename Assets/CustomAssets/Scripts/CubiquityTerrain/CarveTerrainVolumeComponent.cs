using UnityEngine;

using Cubiquity;

public class CarveTerrainVolumeComponent : MonoBehaviour {
	public int RangeX = 2;
	public int RangeY = 4;
	public int RangeZ = 1;
	public int SmoothFactor = 0;
	public bool Euclidean = false;
	public bool Erase = true;
	private TerrainVolume _terrainVolume;

	public void Start() {
		_terrainVolume = gameObject.GetComponent<TerrainVolume>();
		if (_terrainVolume == null) {
			DebugMsg.ComponentNotFound(Debug.Log,typeof(CarveTerrainVolumeComponent));
		}
	}

	/// <summary>
	/// <para>Returns Vector3[] of Length 2.</para>
	/// elem 0 is upper left corner and elem 1 is lower right corner
	/// </summary>
	/// <param name="ray"></param>
	/// <returns></returns>
	public Vector3[] DoCarveAction(Ray ray) {
		if (_terrainVolume == null) {
			return new[] { Vector3.zero, Vector3.zero };
		}

		PickSurfaceResult pickResult;
		bool hit = Picking.PickSurface(_terrainVolume, ray, 1000.0f, out pickResult);

		if (hit) {
			int x = (int) pickResult.volumeSpacePos.x;
			int y = (int) pickResult.volumeSpacePos.y;
			int z = (int) pickResult.volumeSpacePos.z;
			DestroyVoxels(x, y, z);
			return new[] {new Vector3(x - RangeX, pickResult.worldSpacePos.y, z + RangeZ), new Vector3(x + RangeX, pickResult.worldSpacePos.y, z - RangeZ)};
		}
		return new[] { Vector3.zero, Vector3.zero };
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