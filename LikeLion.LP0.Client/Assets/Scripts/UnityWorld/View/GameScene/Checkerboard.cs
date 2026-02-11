using LikeLion.LP0.Client.Core;
using LikeLion.LP0.Client.Core.GameScene;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LikeLion.LP0.Client.UnityWorld.View.GameScene
{
    public class Checkerboard : MonoBehaviour, Core.View.GameScene.ICheckerboard
    {
        //[SerializeField]
        //private GameObject _blockPrefab;
        [SerializeField]
        private GameObject _stonePointPrefab;
        [SerializeField]
        private float _gapX;
        [SerializeField]
        private float _gapY;
        [SerializeField]
        private GameObject _root;
        [SerializeField]
        private GameObject _originPoint;
        [SerializeField]
        private float _blockSize;
        [SerializeField]
        private Vector2 _offset;
        [SerializeField]
        private Vector3 _blockPivot;
        [SerializeField]
        private Vector2 _checkerboardSize;
        [SerializeField]
        private ObjectPool _blackStonePool;
        [SerializeField]
        private ObjectPool _whiteStonePool;

        private List<Tuple<int, GameObject>> _activeStones;

        private System.Collections.Generic.List<System.Collections.Generic.List<StonePoint>> _stonePoints;
        private bool _isDestroyed;

        public event EventHandler<Core.View.GameScene.StonePointClickedEventArgs> StonePointClickedEvent;
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        void Start()
        {
            int rows = (int)_checkerboardSize.x;
            int cols = (int)_checkerboardSize.y;

            _activeStones = new List<Tuple<int, GameObject>>();
            _stonePoints = new System.Collections.Generic.List<System.Collections.Generic.List<StonePoint>>();

            //BuildCheckerboard(rows, cols);
            BuildStonePoints(rows, cols);
        }

        //private void BuildCheckerboard(int rows, int cols)
        //{
        //    float boardWidth = cols * _blockSize + (cols - 1) * _gap;
        //    float boardHeight = rows * _blockSize + (rows - 1) * _gap;

        //    void SpawnElement(Vector3 localPos, Vector3 localScale)
        //    {
        //        GameObject obj = Instantiate(_blockPrefab, _root.transform);
        //        obj.transform.localPosition = localPos;
        //        obj.transform.localScale = localScale;
        //    }

        //    for (int y = 0; y < rows; y++)
        //    {
        //        for (int x = 0; x < cols; x++)
        //        {
        //            float localX = _offset.x + x * (_blockSize + _gap);
        //            float localZ = _offset.y + y * (_blockSize + _gap);

        //            Vector3 localPos = new Vector3(localX, 0, localZ);
        //            SpawnElement(localPos, new Vector3(_blockSize, _blockSize, _blockSize));
        //        }
        //    }

        //    float centerX = _offset.x + boardWidth / 2f - _blockPivot.x;
        //    float centerZ = _offset.y + boardHeight / 2f - _blockPivot.z;
        //    float edgeOffset = _gap + _blockSize / 2f;

        //    Vector3 horizontalScale = new Vector3(boardWidth + _gap * 2 + _blockSize * 2, _blockSize, _blockSize);
        //    Vector3 verticalScale = new Vector3(_blockSize, _blockSize, boardHeight + _gap * 2 + _blockSize * 2);

        //    // Top
        //    SpawnElement(new Vector3(centerX, 0, _offset.y - edgeOffset - _blockPivot.z), horizontalScale);
        //    // Bottom
        //    SpawnElement(new Vector3(centerX, 0, _offset.y + boardHeight + edgeOffset - _blockPivot.z), horizontalScale);
        //    // Left
        //    SpawnElement(new Vector3(_offset.x - edgeOffset - _blockPivot.x, 0, centerZ), verticalScale);
        //    // Right
        //    SpawnElement(new Vector3(_offset.x + boardWidth + edgeOffset - _blockPivot.x, 0, centerZ), verticalScale);
        //}

        private void BuildStonePoints(int rows, int cols)
        {
            int pointRows = rows + 1;
            int pointCols = cols + 1;

            float originPtX = _originPoint.transform.localPosition.x;
            float originPtY = _originPoint.transform.localPosition.y;
            float startPointX = originPtX;// + _offset.x - (_blockSize / 2f + _gap / 2f);
            float startPointY = originPtY;// + _offset.y - (_blockSize / 2f + _gap / 2f);

            for (int y = 0; y < pointRows; y++)
            {
                _stonePoints.Add(new System.Collections.Generic.List<StonePoint>());
                for (int x = 0; x < pointCols; x++)
                {
                    float pX = startPointX + x * (_blockSize + _gapX);
                    float pY = startPointY + y * (_blockSize + _gapY);

                    Vector3 pointPos = new Vector3(pX, pY, 0.0f);

                    GameObject pointObj = Instantiate(_stonePointPrefab, _root.transform);
                    pointObj.transform.localPosition = pointPos;

                    var stonePt = pointObj.GetComponent<StonePoint>();

                    _stonePoints[y].Add(stonePt);

                    int row = y;
                    int col = x;
                    stonePt.ClickedEvent += (sender, args) =>
                    {
                        StonePointClickedEvent?.Invoke(this, new Core.View.GameScene.StonePointClickedEventArgs()
                        {
                            Row = row,
                            Column = col,
                        });
                    };
                }
            }
        }

        public void PutStone(int column, int row, int stoneType)
        {
            GameObject stone = null;
            if (stoneType == StoneType.White)
                stone = _whiteStonePool.Get();
            else if (stoneType == StoneType.Black)
                stone = _blackStonePool.Get();

            if (stone == null)
                return;

            _activeStones.Add(new Tuple<int, GameObject>(stoneType, stone));

            var pos = _stonePoints[row][column].transform.localPosition;
            stone.transform.localPosition = new Vector3(pos.x, pos.y, 0.0f);
        }

        public void Clear()
        {
            foreach (var entry in _activeStones)
            {
                if (entry.Item1 == StoneType.White)
                    _whiteStonePool.Return(entry.Item2);
                else if (entry.Item1 == StoneType.Black)
                    _blackStonePool.Return(entry.Item2);
            }

            _activeStones.Clear();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            StonePointClickedEvent = null;
            _activeStones.Clear();
            _stonePoints.Clear();
        }
    }
}