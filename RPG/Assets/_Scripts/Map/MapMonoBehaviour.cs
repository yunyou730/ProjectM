using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;


namespace ayy
{
    [System.Serializable]
    public class GridPrefab
    {
        public GridType gridType;
        public GameObject prefab;
    }

    public class MapMonoBehaviour : MonoBehaviour
    {
        MapRecord _mapRecord = new MapRecord();

        public GridPrefab[] _gridPrefabRelation = null;

        public Dictionary<GridType, GameObject> _gridTypePrefabMap = null;

        public float _gridShortRadius = 0.5f;
        private float _gridRadius = 0.0f;

        private void Awake()
        {

        }


        void Start()
        {
            CalcGridRadius();
            InitGridPrefabMap();
            LoadMapData();
            CreateMap();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void InitGridPrefabMap()
        {
            _gridTypePrefabMap = new Dictionary<GridType, GameObject>();
            foreach (GridPrefab relation in _gridPrefabRelation)
            {
                _gridTypePrefabMap.Add(relation.gridType,relation.prefab);
            }
        }

        private void LoadMapData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("MapConfig/1-1");
            string strJson = textAsset.text;
            _mapRecord.Load(strJson);
        }


        private void CreateMap()
        {
            for (int row = 0;row < _mapRecord.GetRows();row++)
            {
                for (int col = 0;col < _mapRecord.GetCols();col++)
                {
                    Vector3 pos = GetGridPosAt(row,col);
                    GridRecord gridRecord = _mapRecord.GetGridRecordAt(row,col);

                    if (gridRecord != null)
                    {
                        GridType gridType = gridRecord.gridType;
                        Debug.Log("[" + row + "," + col + "]" + pos);

                        GameObject prefab = _gridTypePrefabMap[gridType];
                        GameObject gridObject = GameObject.Instantiate(prefab, pos, Quaternion.identity);
                        gridObject.transform.parent = transform;
                    }
                }
            }
        }

        private void CalcGridRadius()
        {
            _gridRadius = Mathf.Sqrt((4.0f/3.0f) * _gridShortRadius * _gridShortRadius);
        }

        /*
            (2,0) (2,1) (2,2) (2,3)
               (1,1) (1,2) (1,3)
            (0,0),(0,1),(0,2) (0,3)
        */
        Vector3 GetGridPosAt(int row,int col)
        {
            float z = row * 1.5f * _gridRadius;
            float startXOfRow = row % 2 == 0 ? 0 : -_gridShortRadius;
            float x = startXOfRow + col * 2 * _gridShortRadius;
            return new Vector3(x,0,z);
        }
    }

}
