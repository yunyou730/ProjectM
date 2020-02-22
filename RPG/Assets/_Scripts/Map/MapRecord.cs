using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace ayy
{
    public class MapRecord
    {
        int _rows = 0;
        int _cols = 0;

        //Dictionary<>
        GridRecord[,] _gridRecords = null;

        public void Load(string strJson)
        {
            JsonData jd = JsonMapper.ToObject(strJson);
            _rows = (int)jd["rows"];
            _cols = (int)jd["cols"];
            _gridRecords = new GridRecord[_rows,_cols];

            for (int row = 0;row < _rows;row++)
            {
                for (int col = 0;col < _cols;col++)
                {
                    GridRecord gridRecord = new GridRecord();
                    gridRecord.gridType = GridType.Green;
                    _gridRecords[row,col] = gridRecord;
                }
            }
        }


        public GridType GetGridTypeAt(int row,int col)
        {
            if (row >= 0 && row < _rows && col >= 0 && col < _cols)
            {
                return _gridRecords[row,col].gridType;
            }
            return GridType.Max;
        }

        public GridRecord GetGridRecordAt(int row,int col)
        {
            if (row >= 0 && row < _rows && col >= 0 && col < _cols)
            {
                return _gridRecords[row, col];
            }
            return null;
        }


        public int GetRows() { return _rows; }
        public int GetCols() { return _cols; }
    }
}
