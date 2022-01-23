using UnityEngine;
using System.Collections;

public class TileAnimation : MonoBehaviour
{
	[Range(1, 32)]
	public int tileX = 1;
	[Range(1, 32)]
	public int tileY = 1;
	public int maxTileCount = 10;
	[Range(1, 30)]
	public int frame = 10;
	[Range(0,5)]
	public float speed = 1;
	public bool loop = false;

	[SerializeField]
	int AniID = 0;
	[SerializeField]
	private Material[] materials;
	bool isCheck = false;


	private float tileSizeX;
	private float tileSizeY;

	private Mesh mesh;
	private Vector3[] vertices;
	private Vector2[] defUV;
	private Vector2[] newUV;

	private float changeTime;
	private int tileIndex;

	void Start ()
	{
		InitVariable();
	}

    private void Update()
    {
		if (GameManager.instance.checkReadyTimer > 0 && isCheck == false && AniID == 1)
		{
			StartCoroutine("CheckAni");
			isCheck = true;
        }
		else if(GameManager.instance.checkReadyTimer <= 0 && isCheck == true && AniID == 1)
        {
			StartCoroutine("TeachAni");
			isCheck = false;
		}
	}

    private void InitVariable ()
	{
		tileSizeX = 1.0f / tileX;
		tileSizeY = 1.0f / tileY;
		
		mesh = GetComponent<MeshFilter>().mesh;

		vertices = mesh.vertices;
		defUV = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; ++i)
		{
			defUV[i] = new Vector2(mesh.uv[i].x*tileSizeX, mesh.uv[i].y*tileSizeY + tileSizeY*(tileY-1));
		}
		newUV = new Vector2[vertices.Length];
	}

	void OnEnable ()
	{
		tileIndex = 0;
		changeTime = 1.0f/(float)frame / speed;
		Invoke("UpdateRepeat", 0);
	}

	void OnDisable()
	{
		CancelInvoke();
	}

	void UpdateRepeat ()
	{
		if(GameManager.instance.state != GameState.Stop)
        {
			int uIndex = tileIndex % tileX;
			int vIndex = (int)(tileIndex * tileSizeX);
			float offset_u = uIndex * tileSizeX;
			float offset_v = vIndex * tileSizeY;

			for (int i = 0; i < vertices.Length; ++i)
			{
				newUV[i].Set(defUV[i].x + offset_u, defUV[i].y - offset_v);
			}
			mesh.uv = newUV;

			tileIndex++;
			if (tileIndex == maxTileCount)
			{
				if (loop)
					tileIndex = 0;
                else
					return;
			}
			Invoke("UpdateRepeat", changeTime);
		}	
		else
			Invoke("UpdateRepeat", 0.1f);
	}

	/*
	public void ChangeAni(int _type, int _tilex = 2, int _tiley = 2, int _max = 10, int _frame = 10, float _speed = 1, bool _loop = false)
    {
		tileIndex = 0;
		tileX = _tilex;
		tileY = _tiley;
		maxTileCount = _max;
		frame = _frame;
		speed = _speed;
		loop = _loop;
		changeTime = 1.0f / (float)frame / speed;

		gameObject.GetComponent<MeshRenderer>().material = materials[_type];

		InitVariable();
	}
	*/

	public void ChangeFrame(int _type, int _max = 10, int _frame = 10, float _speed = 1, bool _loop = false)
	{
		tileIndex = 0;
		maxTileCount = _max;
		frame = _frame;
		speed = _speed;
		loop = _loop;
		changeTime = 1.0f / (float)frame / speed;
		gameObject.GetComponent<MeshRenderer>().material = materials[_type];


		Invoke("UpdateRepeat", 0);
	}

	IEnumerator CheckAni()
    {
		gameObject.GetComponent<MeshRenderer>().material = materials[1];
		yield return new WaitForSeconds(0.33f);
		gameObject.GetComponent<MeshRenderer>().material = materials[2];
    }
	IEnumerator TeachAni()
	{
		yield return new WaitForSeconds(0.5f);
		gameObject.GetComponent<MeshRenderer>().material = materials[0];
	}

}