using MLAgents;
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
        [SerializeField] private float _handHitForce;
        [SerializeField] private Rigidbody2D _ballRigidBody;

        private Rigidbody2D _rigidbody;
        private float _playerSign;
        private bool _isLeftPlayer;

        public string CollisionTag { get; private set; }
        public Hand LeftHand { get; private set; }
        public Hand RightHand { get; private set; }

        public void Restart()
        {
            LeftHand.Restart();
            RightHand.Restart();
            CollisionTag = string.Empty;

            Vector2 gymQuarterScale = _gym.localScale / 4;
            float randomXOffset = gymQuarterScale.x * Random.Range(-0.8f, 0.8f);
            float xPosition = _gym.position.x + _playerSign * gymQuarterScale.x + randomXOffset;
            float yPosition = _gym.position.y - 2 * gymQuarterScale.y + 1.1f;

            transform.position = new Vector2(xPosition, yPosition);
            _rigidbody.velocity = Vector2.zero;
        }

        public void RestartWithServing()
        {
            Restart();
            Ball ball = _ballRigidBody.GetComponent<Ball>();
            ball.Restart((Vector2)transform.position + new Vector2(2f * _playerSign, Random.Range(5f, _gym.localScale.y / 2)));
        }

        public override void InitializeAgent()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerSign = Tags.GetPlayerSign(tag);
            _isLeftPlayer = Tags.IsLeftPlayer(tag);
            LeftHand = transform.Find("LeftHand").GetComponent<Hand>();
            RightHand = transform.Find("RightHand").GetComponent<Hand>();

            base.InitializeAgent();
        }

        public override void CollectObservations()
        {
            AddVectorObs((transform.position.x - _gym.position.x) * _playerSign);
            AddVectorObs((_ballRigidBody.transform.position.x - transform.position.x) * _playerSign);
            AddVectorObs(_ballRigidBody.transform.position.y - transform.position.y);
            AddVectorObs(_leftHandRigidbody.transform.position.y - transform.position.y);
            AddVectorObs(_rightHandRigidbody.transform.position.y - transform.position.y);
        }

        public override void AgentAction(float[] vectorAction)
        {
            Move(vectorAction);
            MoveHands(vectorAction);
        }

        public override float[] Heuristic()
        {
            return new[] { Input.GetAxis("Horizontal"), Input.GetAxis("Fire2"), Input.GetAxis("Fire1") };
        }

        private void Move(float[] vectorAction)
        {
            float movement = Mathf.Clamp(vectorAction[0], -1f, 1f);
            _rigidbody.MovePosition(_rigidbody.position + _moveSpeed * movement * Vector2.left * _playerSign);
        }

        private void MoveHands(float[] vectorAction)
        {
            float leftHand = Mathf.Clamp01(vectorAction[1]);
            if (_isLeftPlayer)
            {
                _rightHandRigidbody.AddRelativeForce(_handHitForce * leftHand * Vector2.up, ForceMode2D.Impulse);
            }
            else
            {
                _leftHandRigidbody.AddRelativeForce(_handHitForce * leftHand * Vector2.up, ForceMode2D.Impulse);
            }

            float rightHand = Mathf.Clamp01(vectorAction[2]);
            if (_isLeftPlayer)
            {
                _leftHandRigidbody.AddRelativeForce(_handHitForce * rightHand * Vector2.up, ForceMode2D.Impulse);
            }
            else
            {
                _rightHandRigidbody.AddRelativeForce(_handHitForce * rightHand * Vector2.up, ForceMode2D.Impulse);
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
