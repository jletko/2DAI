using System.Linq;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.Volleyball
{
    public class PlayerAgent : Agent
    {
        [FormerlySerializedAs("_leftHandRigidbody")] [SerializeField] private Rigidbody2D leftHandRigidbody;
        [FormerlySerializedAs("_rightHandRigidbody")] [SerializeField] private Rigidbody2D rightHandRigidbody;
        [FormerlySerializedAs("_otherPlayer")] [SerializeField] private Transform otherPlayer;
        [FormerlySerializedAs("_gym")] [SerializeField] private Transform gym;
        [FormerlySerializedAs("_moveSpeed")] [SerializeField] private float moveSpeed;
        [FormerlySerializedAs("_handTorque")] [SerializeField] private float handTorque;
        [FormerlySerializedAs("_ballRigidBody")] [SerializeField] private Rigidbody2D ballRigidBody;

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
            _leftHingeJoint2D = GetComponents<HingeJoint2D>().Single(o => o.connectedBody.Equals(leftHandRigidbody));
            _rightHingeJoint2D = GetComponents<HingeJoint2D>().Single(o => o.connectedBody.Equals(rightHandRigidbody));
            leftHandRigidbody.centerOfMass = _leftHingeJoint2D.connectedAnchor;
            rightHandRigidbody.centerOfMass = _rightHingeJoint2D.connectedAnchor;

            base.Initialize();
        }

        public void Restart()
        {
            LeftHand.Restart();
            RightHand.Restart();
            CollisionTag = string.Empty;

            Vector2 gymQuarterScale = gym.localScale / 4;
            float randomXOffset = gymQuarterScale.x * Random.Range(-0.8f, 0.8f);
            float xPosition = gym.position.x + _playerSign * gymQuarterScale.x + randomXOffset;
            float yPosition = gym.position.y - 2 * gymQuarterScale.y + 2.6f;

            transform.position = new Vector2(xPosition, yPosition);
            _rigidbody.velocity = Vector2.zero;
        }

        public void RestartWithServing()
        {
            Restart();
            Ball ball = ballRigidBody.GetComponent<Ball>();
            ball.Restart((Vector2)transform.position + new Vector2(2f * _playerSign, Random.Range(5f, gym.localScale.y / 2)));
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation((transform.position.x - gym.position.x) * _playerSign);
            sensor.AddObservation((ballRigidBody.transform.position.x - transform.position.x) * _playerSign);
            sensor.AddObservation(ballRigidBody.transform.position.y - transform.position.y);
            sensor.AddObservation(ballRigidBody.velocity.x * _playerSign);
            sensor.AddObservation(ballRigidBody.velocity.y);
            sensor.AddObservation(leftHandRigidbody.transform.position.y - transform.position.y > 0 ? 1 : 0);
            sensor.AddObservation(rightHandRigidbody.transform.position.y - transform.position.y > 0 ? 1 : 0);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Power = 0;
            Move(actions.DiscreteActions.Array);
            MoveHands(actions.DiscreteActions.Array);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            int[] actions = new int[3];

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

            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                discreteActions[2] = 1;
            }

            for (int i = 0; i < discreteActions.Length; i++)
            {
                discreteActions[i] = actions[i];
            }
        }

        private void Move(int[] actions)
        {
            switch (actions[0])
            {
                case 1:
                    _rigidbody.MovePosition(_rigidbody.position + moveSpeed * Vector2.left * _playerSign);
                    Power += 1;
                    break;
                case 2:
                    _rigidbody.MovePosition(_rigidbody.position - moveSpeed * Vector2.left * _playerSign);
                    Power += 1;
                    break;
            }
        }

        private void MoveHands(int[] actions)
        {
            int leftHand = Mathf.FloorToInt(actions[1]);
            int rightHand = Mathf.FloorToInt(actions[2]);
            if (_isLeftPlayer)
            {
                if (leftHand == 1)
                {
                    leftHandRigidbody.AddTorque(-handTorque);
                    Power += 1;
                }

                if (rightHand == 1)
                {
                    rightHandRigidbody.AddTorque(handTorque);
                    Power += 1;
                }
            }
            else
            {
                if (leftHand == 1)
                {
                    rightHandRigidbody.AddTorque(handTorque);
                    Power += 1;
                }

                if (rightHand == 1)
                {
                    leftHandRigidbody.AddTorque(-handTorque);
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
