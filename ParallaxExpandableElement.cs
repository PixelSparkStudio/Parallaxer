﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukaiFox.Math;

namespace YoukaiFox.Parallax
{
    public class ParallaxExpandableElement : ParallaxLayeredElement
    {
        #region Serialized fields

        [SerializeField] [BeginGroup("Values")] [EndGroup]
        private float _parallaxExitDistance = 1.5f;

        #endregion

        #region Non-serialized fields

        private SpriteRenderer[] _copies;
        private float _boundsSizeX;


        #endregion

        #region Constant fields

        private const int CopiesQuantity = 3;

        #endregion

        #region Custom structures

        private enum Position
        {
            Left = 0, Central = 1, Right = 2
        }
        #endregion

        #region Unity events
        #endregion

        #region Public methods
        #endregion

        #region Protected methods

        #region Overridden methods

        protected override void Initialize()
        {
            base.Initialize();
            CreateCopies();
            PlaceChildrenCopies();
        }

        protected override void OnLateUpdateEnter()
        {
            CheckDisplacement();
            base.OnLateUpdateEnter();
        }

        protected override Vector3 CalculateNextPosition()
        {
            if (!ParallaxManager.Instance)
                return Vector3.zero;

            return base.Transform.position - ParallaxManager.Instance.CurrentCameraDisplacement * ParallaxSpeed;
        }

        #endregion

        #endregion

        #region Private methods

        private void CheckDisplacement()
        {
            if (!ParallaxManager.Instance)
                return;

            Direction.Directions direction = ParallaxManager.Instance.MovementDirection;

            if (direction == Direction.Directions.None)
                return;

            Bounds cameraBounds = ParallaxManager.Instance.GetCameraBounds();
            float currentDistance = 0f;

            if (direction == Direction.Directions.Right)
            {
                currentDistance = _copies[(int) Position.Right].bounds.max.x - cameraBounds.max.x;
            }
            else if (direction == Direction.Directions.Left)
            {
                currentDistance = _copies[(int) Position.Left].bounds.min.x - cameraBounds.min.x;
            }

            if (YoukaiMath.Abs(currentDistance) <= _parallaxExitDistance)
            {
                RotateCopies(direction);
            }
        }

        private void CreateCopies()
        {
            _boundsSizeX = base.SpriteRenderer.bounds.size.x;
            base.SpriteRenderer.enabled = false;
            _copies = new SpriteRenderer[CopiesQuantity];
            GameObject dummyObject = new GameObject();
            SpriteRenderer spriteRenderer = dummyObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = base.SpriteRenderer.sortingOrder;
            spriteRenderer.sprite = base.SpriteRenderer.sprite;

            for (int i = 0; i < _copies.Length; i++)
            {
                _copies[i] = Instantiate(spriteRenderer, base.transform, false);
            }

            Destroy(dummyObject);
        }

        private void PlaceChildrenCopies()
        {
            if (_copies.Length <= 0)
                return;

            Bounds bounds = base.SpriteRenderer.bounds;

            _copies[(int) Position.Left].transform.localPosition = new Vector3(
                -bounds.size.x, 
                _copies[(int) Position.Left].transform.localPosition.y, 
                _copies[(int) Position.Left].transform.localPosition.z);

            _copies[(int) Position.Right].transform.localPosition = new Vector3(
                bounds.size.x, 
                _copies[(int) Position.Right].transform.localPosition.y, 
                _copies[(int) Position.Right].transform.localPosition.z);
        }

        private void RotateCopies(Direction.Directions direction)
        {
            SpriteRenderer extendedSprite = null;

            if (direction == Direction.Directions.Right)
            {
                extendedSprite = _copies[(int) Position.Left];
                extendedSprite.transform.localPosition = new Vector3(
                    _copies[(int) Position.Right].transform.localPosition.x + _boundsSizeX,
                    extendedSprite.transform.localPosition.y,
                    extendedSprite.transform.localPosition.z
                );

                _copies[(int) Position.Left] = _copies[(int) Position.Central];
                _copies[(int) Position.Central] = _copies[(int) Position.Right];
                _copies[(int) Position.Right] = extendedSprite;
            }
            else if (direction == Direction.Directions.Left)
            {
                extendedSprite = _copies[(int) Position.Right];
                extendedSprite.transform.localPosition = new Vector3(
                    _copies[(int) Position.Left].transform.localPosition.x - _boundsSizeX,
                    extendedSprite.transform.localPosition.y,
                    extendedSprite.transform.localPosition.z
                );

                _copies[(int) Position.Right] = _copies[(int) Position.Central];
                _copies[(int) Position.Central] = _copies[(int) Position.Left];
                _copies[(int) Position.Left] = extendedSprite;
            }
        }

        #endregion
    }
}