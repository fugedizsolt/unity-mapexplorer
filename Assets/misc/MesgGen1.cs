using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MesgGen1 : MonoBehaviour
{
    private Mesh mesh;
	private readonly int gridSize = 10;

	// Start is called before the first frame update
	void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;

        CreateShape();
    }

	private void CreateShape()
	{
        var texture = new Texture2D( gridSize+1,gridSize+1,TextureFormat.ARGB32,false );
 
        // connect texture to material of GameObject this script is attached to
        GetComponent<Renderer>().material.mainTexture = texture;

		Vector3[] vertices = new Vector3[(gridSize + 1)*(gridSize + 1)];
		for (int z = 0; z <= gridSize; z++)
        {
			for (int x = 0; x <= gridSize; x++)
            {
				float valh = getHeight(x, z);
                if ( valh<=0.1 ) texture.SetPixel( x, z, Color.blue );
                else if ( valh>1.9 ) texture.SetPixel( x, z, Color.red );
                else texture.SetPixel( x, z, Color.green );

                vertices[x+z*(gridSize+1)] = new Vector3( x,valh,z );
			}
		}
        texture.Apply();

		int[] triangles = generateIndexBuffer( gridSize+1 );

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x/gridSize, vertices[i].z/gridSize);
        }

        this.mesh.Clear();
        this.mesh.vertices = vertices;
        this.mesh.triangles = triangles;
        this.mesh.uv = uvs;
        this.mesh.RecalculateNormals();
	}

	// Update is called once per frame
	void Update()
    {
    }

	float getHeight( int xp,int zp )
	{
		//return ((float)xp/(float)terrainSize)*2f*AMP - AMP;
		if ( xp>2 && zp>2 && xp<5 && zp<5 )
			return 2;
		else if ( xp>1 && zp>1 && xp<6 && zp<6 )
			return 1;
		else
			return 0;
	}

	public int[] generateIndexBuffer( int vertexCount )
    {
		int indexCount = (vertexCount - 1) * (vertexCount - 1) * 6;
		int[] indices = new int[indexCount];
		int pointer = 0;
		for (int col = 0; col < vertexCount - 1; col++)
        {
			for (int row = 0; row < vertexCount - 1; row++)
            {
				int topLeft = (row * vertexCount) + col;
				int topRight = topLeft + 1;
				int bottomLeft = ((row + 1) * vertexCount) + col;
				int bottomRight = bottomLeft + 1;

                indices[pointer++] = topLeft;
                indices[pointer++] = bottomLeft;
                indices[pointer++] = bottomRight;
                indices[pointer++] = topLeft;
                indices[pointer++] = bottomRight;
                indices[pointer++] = topRight;
			}
		}
		return indices;
	}
}
