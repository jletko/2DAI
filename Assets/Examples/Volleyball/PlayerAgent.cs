﻿using MLAgents;
using MLAgents.Sensors;
using System.Linq;
using UnityEngine;

namespace Examples.Volleyball
{
    public class PlayerAgent : Agent
    {
        [SerializeField] private Rigidbody2D _leftHandRigidbody;
        [SerializeField] private Rigidbody2D _rightHandRigidbody;
        [SerializeField] private Transform _otherPlayer;
        [SerializeField] private Transform _gym;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _handTorque;
        [SerializeField] private Rigidbody2D _ballRigidBody;

        private Rigidbody2D _rigidbody;
        private HingeJoint2D _leftHingeJoint2D;
        private HingeJoint2D _rightHingeJoint2D;
        private float _playerSign;
        private bool _isLeftPlayer;

        public string CollisionTag { get; private set; }
        public Hand LeftHand { get; private set; }
        public Hand RightHand { get; private set; }
        public float Power { get; private set; }

        public override void Initialize()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerSign = Tags.GetPlayerSign(tag);
            _isLeftPlayer = Tags.IsLeftPlayer(tag);
            LeftHand = transform.Find("LeftHand").GetComponent<Hand>();
            RightHand = transform.Find("RightHand").GetComponent<Hand>();
            _leftHingeJoint2D = GetComponents<HingeJoint2D>().Single(o => o.connectedBody.Equals(_leftHandRigidbody));
            _rightHingeJoint2D = GetComponents<HingeJoint2D>().Single(o => o.connectedBody.Equals(_rightHandRigidbody));
            _leftHandRigidbody.centerOfMass = _leftHingeJoint2D.connectedAnchor;
            _rightHandRigidbody.centerOfMass = _rightHingeJoint2D.connectedAnchor;

            base.Initialize();
        }

        public void Restart()
        {
            LeftHand.Restart();
            RightHand.Restart();
            CollisionTag = string.Empty;

            Vector2 gymQuarterScale = _gym.localScale / 4;
            float randomXOffset = gymQuarterScale.x * Random.Range(-0.8f, 0.8f);
            float xPosition = _gym.position.x + _playerSign * gymQuarterScale.x + randomXOffset;
            float yPosition = _gym.position.y - 2 * gymQuarterScale.y + 2.6f;

            transform.position = new Vector2(xPosition, yPosition);
            _rigidbody.velocity = Vector2.zero;
        }

        public void RestartWithServing()
        {
            Restart();
            Ball ball = _ballRigidBody.GetComponent<Ball>();
            ball.Restart((Vector2)transform.position + new Vector2(2f * _playerSign, Random.Range(5f, _gym.localScale.y / 2)));
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation((transform.position.x - _gym.position.x) * _playerSign);
            sensor.AddObservation((_ballRigidBody.transform.position.x - transform.position.x) * _playerSign);
            sensor.AddObservation(_ballRigidBody.transform.position.y - transform.position.y);
            sensor.AddObservation(_ballRigidBody.velocity.x * _playerSign);
            sensor.AddObservation(_ballRigidBody.velocity.y);
            sensor.AddObservation(_leftHandRigidbody.transform.position.y - transform.position.y > 0 ? 1 : 0);
            sensor.AddObservation(_rightHandRigidbody.transform.position.y - transform.position.y > 0 ? 1 : 0);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            Power = 0;
            Move(vectorAction);
            MoveHands(vectorAction);
        }

        public override float[] Heuristic()
        {
            float[] actions = new float[3];

            //TODO: use axis but with "infinite" sensitivity so the action is instant 
            // but consider other example scenes/agents are using the same axis and are analog based
            if (Input.GetKey(KeyCode.RightArrow))
            {
                actions[0] = 1;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                actions[0] = 2;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                actions[1] = 1;
            }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                actions[2] = 1;
            }

            return actions;
        }

        private void Move(float[] vectorAction)
        {
            int movement = Mathf.FloorToInt(vectorAction[0]);
            switch (movement)
            {
                case 1:
                    _rigidbody.MovePosition(_rigidbody.position + _moveSpeed * Vector2.left * _playerSign);
                    Power += 1;
                    break;
                case 2:
                    _rigidbody.MovePosition(_rigidbody.position - _moveSpeed * Vector2.left * _playerSign);
                    Power += 1;
                    break;
            }
        }

        private void MoveHands(float[] vectorAction)
        {
            int leftHand = Mathf.FloorToInt(vectorAction[1]);
            int rightHand = Mathf.FloorToInt(vectorAction[2]);
            if (_isLeftPlayer)
            {
                if (leftHand == 1)
                {
                    _leftHandRigidbody.AddTorque(-_handTorque);
                    Power += 1;
                }

                if (rightHand == 1)
                {
                    _rightHandRigidbody.AddTorque(_handTorque);
                    Power += 1;
                }
            }
            else
            {
                if (leftHand == 1)
                {
                    _rightHandRigidbody.AddTorque(_handTorque);
                    Power += 1;
                }

                if (rightHand == 1)
                {
                    _leftHandRigidbody.AddTorque(-_handTorque);
                    Power += 1;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionTag = collision.collider.tag;
        }

        private void OnCollisionExit2D()
        {
            CollisionTag = string.Empty;
        }
    }
}
