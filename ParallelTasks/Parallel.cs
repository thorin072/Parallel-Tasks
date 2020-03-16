using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTasks
{
    public class Parallel
    {
        private Form1 form;
        private int n;

        private Queue<string> q;

        private int[,] M;
        private bool[] R;

        private int[,] resC;
        private int resD;
        private int resE;
        private int resF;
        private int resG;
        private int resH;
        private int resK;

        public delegate void TaskHandler(string s);
        private event TaskHandler TaskComplete;

        public Parallel(Form1 form, int n)
        {
            this.form = form;
            this.n = n;

            q = new Queue<string>();

            TaskComplete += Log;
        }

        public void Execute()
        {
            Task[] ab = new Task[2];
            ab[0] = Task.Run(A);
            ab[1] = Task.Run(B);

            Task.WaitAll(ab);
            form.Progress(0);
            while (q.Count > 0)
                form.Log("Task complete: " + q.Dequeue());
            Task c = Task.Run(C);

            Task.WaitAll(c);
            form.Progress(1);
            while (q.Count > 0)
                form.Log("Task complete: " + q.Dequeue());
            Task[] de = new Task[2];
            de[0] = Task.Run(D);
            de[1] = Task.Run(E);
            Task[] fgh = new Task[3];
            fgh[0] = Task.Run(F);

            Task.WaitAll(de);
            form.Progress(2);
            while (q.Count > 0)
                form.Log("Task complete: " + q.Dequeue());
            fgh[1] = Task.Run(G);
            fgh[2] = Task.Run(H);

            Task.WaitAll(fgh);
            form.Progress(3);
            while (q.Count > 0)
                form.Log("Task complete: " + q.Dequeue());
            Task k = Task.Run(K);

            Task.WaitAll(k);
            form.Progress(4);
            while (q.Count > 0)
                form.Log("Task complete: " + q.Dequeue());
        }

        public void Log(string invoker)
        {
            q.Enqueue(invoker);
        }

        public void A()
        {
            M = new int[n, n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i, j] = rand.Next(n);
                }
            }

            TaskComplete?.Invoke("A");
        }

        public void B()
        {
            R = new bool[n];
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                R[i] = rand.Next(2) == 1;
            }

            TaskComplete?.Invoke("B");
        }

        public void C()
        {
            resC = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //some func
                    if (R[i])
                    {
                        resC[i, j] = M[i, j];
                    }
                }
            }

            TaskComplete?.Invoke("C");
        }

        public void D()
        {
            resD = 0;
            foreach (var i in resC)
            {
                //some func
                resD += i;
            }

            TaskComplete?.Invoke("D");
        }

        public void E()
        {
            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
            }

            TaskComplete?.Invoke("E");
        }

        public void F()
        {
            resE = 1;
            foreach (var i in resC)
            {
                //some func
                resE *= i;
                resE -= 10 * i;
            }

            TaskComplete?.Invoke("F");
        }

        public void G()
        {
            //some func
            resG = resD * 2;

            TaskComplete?.Invoke("G");
        }

        public void H()
        {
            //some func
            resH = resE + 100;

            TaskComplete?.Invoke("H");
        }

        public void K()
        {
            //some func
            resK = resG + resH;

            TaskComplete?.Invoke("K");
        }
    }
}