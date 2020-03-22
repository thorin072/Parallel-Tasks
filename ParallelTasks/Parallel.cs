using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTasks
{
    public class ArraysTurple
    {
        public int[,] M1 { get; set; }
        public int[] M2 { get; set; }
    }

    public class Parallel
    {
        private Form1 form;
        private int n; //размер массива

        private Queue<string> q;
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
            int i = 0;
            Task<ArraysTurple> a = new Task<ArraysTurple>(() => A());
            a.Start();
            a.Wait();
            form.Progress(1);
            while (q.Count > i)
                form.Log("Task complete: " + q.Dequeue());
            var b = a.ContinueWith(x => B(x.Result));
            var c = a.ContinueWith(x => C(x.Result));
            Task.WaitAll(b, c);
            while (q.Count > i)
                form.Log("Task complete: " + q.Dequeue());
            form.Progress(3);
            var d = b.ContinueWith(x => D(x.Result));
            var e = c.ContinueWith(x => E(x.Result));
            Task.WaitAll(d, e);
            while (q.Count > i)
                form.Log("Task complete: " + q.Dequeue());
            form.Progress(5);
            // form.Progress();
            var fgh = new Task<int>[3] {
                new Task<int>(() => F(e.Result,d.Result)),
                new Task<int>(() => G(e.Result,d.Result)),
                new Task<int>(() => H(e.Result,d.Result))
            };

            foreach (var t in fgh)
                t.Start();
            Task.WaitAll(fgh);
            while (q.Count > i)
                form.Log("Task complete: " + q.Dequeue());
            form.Progress(8);
            var k = new Task(() => K(fgh[0].Result, fgh[1].Result, fgh[2].Result));
            k.Start();
            k.Wait();
            while (q.Count > i)
                form.Log("Task complete: " + q.Dequeue());
            form.Progress(9);
            form.Log("DONE");
            q.Clear();

        }

        public void Log(string invoker)
        {
            q.Enqueue(invoker);
        }

        public ArraysTurple A()
        {
            int[,] M1 = new int[n, n];
            int[] M2 = new int[n];

            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M1[i, j] = rand.Next(n);
                }
            }

            for (int i = 0; i < M2.Length; i++)
            {
                M2[i] = rand.Next(0, 100);
            }

            TaskComplete?.Invoke("A");
            return new ArraysTurple { M1 = M1, M2 = M2 };
        }

        public int B(ArraysTurple result)
        {
            var M1 = result.M1;
            var M2 = result.M2;
            int res = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    res = M1[i, j] + res;
                }
            }

            TaskComplete?.Invoke("B");
            return res;
        }

        public int C(ArraysTurple result)
        {
            var M1 = result.M1;
            var M2 = result.M2;
            int res = 0;
            for (int i = 0; i < M2.Length; i++)
            {
                res = M2[i] - 100;
            }
            TaskComplete?.Invoke("C");
            return res;
        }

        public int D(int result)
        {
            var res = result + 100;
            TaskComplete?.Invoke("D");
            return res;
        }

        public int E(int result)
        {
            var res = result / 50;
            TaskComplete?.Invoke("E");
            return res;
        }

        public int F(int resE,int resD)
        {
            int res = resD * resE;
            TaskComplete?.Invoke("F");
            return res;
        }

        public int G(int resE, int resD)
        {
            int res = (resD * resE)-45;
            TaskComplete?.Invoke("G");
            return res;
        }

        public int H(int resE, int resD)
        {
            int res = resD * resE*3;
            TaskComplete?.Invoke("H");
            return res;
        }

        public void K(int resF, int resG,int resH)
        {
            //some func
            int res = resF + resG + resH;
            TaskComplete?.Invoke("K");
        }
    }
}