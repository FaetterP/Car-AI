using UnityEngine;

namespace Assets
{
    class Car : MonoBehaviour
    {
        [SerializeField] float speed = 0.1f;
        [SerializeField] float rot_speed = 10;
        public MLP[,] perceptron;
        RaycastHit2D[] hits = new RaycastHit2D[5];

        private bool isCrash = false, isFinish = false;

        int score = 0;

        public void CreateMLP()
        {
            perceptron = new MLP[Settings.ScreenSizeX / Settings.CellSizeX, Settings.ScreenSizeY / Settings.CellSizeY];
            for (int i = 0; i < perceptron.GetLength(0); i++)
            {
                for (int j = 0; j < perceptron.GetLength(1); j++)
                {
                    perceptron[i, j] = new MLP(new int[3] { 5, 5, 1 });
                }
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Contains("wall"))
                isCrash = true;

            if (collision.tag.Contains("finish"))
                isFinish = true;
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag.Contains("path") && isNeedRestart() == false)
                score++;
        }

        void Update()
        {
            if (isNeedRestart())
                return;

            float z = transform.rotation.eulerAngles.z;
            z *= Mathf.PI / 180;
            transform.position += new Vector3(Mathf.Cos(z) * speed, Mathf.Sin(z) * speed, 0);

            for (int i = 0; i < 5; i++)
            {
                hits[i] = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(3.14f / 5 * (i - 2) + z), Mathf.Sin(3.14f / 5 * (i - 2) + z)), Mathf.Infinity, LayerMask.GetMask("wall"));
            }

            double[] inp = new double[5]
            {
                hits[0].distance,
                hits[1].distance,
                hits[2].distance,
                hits[3].distance,
                hits[4].distance
            };
            MLP p = perceptron[(int)(transform.localPosition.x + Settings.ScreenSizeX / 2) / Settings.CellSizeX, (int)(transform.localPosition.y + Settings.ScreenSizeY / 2) / Settings.CellSizeY];
            double[] r = p.Predict(inp);
            transform.Rotate(new Vector3(0, 0, ((float)r[0] - 0.5f) * rot_speed));
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var t in hits)
            {
                Gizmos.color = isNeedRestart() ? Color.gray : Color.red;
                Gizmos.DrawSphere(t.point, 0.1f);
            }
        }

        public bool isNeedRestart()
        {
            return isCrash || isFinish;
        }

        public double getScore()
        {
            if (isCrash)
                return score / 10.0;

            if (isFinish)
                return score * 10;
            return score;
        }

        public void RandomW(float p)
        {
            for (int i = 0; i < perceptron.GetLength(0); i++)
            {
                for (int j = 0; j < perceptron.GetLength(1); j++)
                {
                    perceptron[i, j].RandomW(p);
                }
            }
        }

        public void SetW(MLP[,] w)
        {
            perceptron = new MLP[w.GetLength(0), w.GetLength(1)];

            for (int i = 0; i < perceptron.GetLength(0); i++)
            {
                for (int j = 0; j < perceptron.GetLength(1); j++)
                {
                    perceptron[i, j] = new MLP(new int[3] { 5, 5, 1 });
                    perceptron[i, j].SetW(w[i, j].GetW());
                }
            }
        }
    }
}
