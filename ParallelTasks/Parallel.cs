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
    public class Info
    {
        public string id;
        public string time_end;

    }

    public class Parallel
    {
        private Form1 form;
        private int n; //размер массива
        private Queue<Info> q;
        public delegate void TaskHandler(Info s);
        private event TaskHandler TaskComplete;

        public Parallel(Form1 form, int n)
        {
            this.form = form;
            this.n = n;
            q = new Queue<Info>();
            TaskComplete += Log;
        }

        public void Execute()
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            // Главный поток 
            int i = 0;
            form.Log("[TIME START FOR A]: "+ DateTime.Now+":"+DateTime.Now.Millisecond);
			//Task который возвращает значение внутри себя
            Task<ArraysTurple> a = new Task<ArraysTurple>(() => A());
			// Запуск задачи и передача управления

            a.Start();
            // Ожидание, пока закончиться поток А. Все управление передаеться только потоку А,
            // код в процедуре "А" являеться критической секцией, главный поток блокируеться, пока не будет выполнен поток получивший управление 
            a.Wait();
 
            // В главном потоке устанавливаем progress bar
            form.Progress(1);
            // Вывод завершенного Task из очереди
            
            while (q.Count > i)
            {
                form.Log(q.Peek().time_end);
                form.Log("[COMPLETE]: " + q.Peek().id);
                q.Dequeue();
                form.Log("//---------------------------------------------------------------//");
            }
                
            // ContinueWith - cоздает задачу продолжения, которая выполняется после завершения другой задачи (в данном случаи А)
            // Задачи исполняються параллельно
            // x => B(x.Result) - это лямбда выражение для получения результата из предыдущего Task
            // В данной реализации каждый Task<> будет иметь поле .Result 

            var b = a.ContinueWith(x => B(x.Result));
            var c = a.ContinueWith(x => C(x.Result));
            form.Log("[TIME START FOR B, C]: " + DateTime.Now + ":" + DateTime.Now.Millisecond);
            // Ожидание пока будут завершены все Tasks 
            form.Log("[TIME START FOR D, E]: " + DateTime.Now + ":" + DateTime.Now.Millisecond);
            form.Progress(3);
            var d = b.ContinueWith(x => D(x.Result));
            var e = c.ContinueWith(x => E(x.Result));
            Task.WaitAll(d, e);
            while (q.Count > i)
            {
                form.Log(q.Peek().time_end);
                form.Log("[COMPLETE]: " + q.Peek().id);
                q.Dequeue();
                form.Log("//---------------------------------------------------------------//");
            }
            form.Progress(5);

            form.Log("[TIME START FOR F, G, H]: " + DateTime.Now + ":" + DateTime.Now.Millisecond);
            // Task могут быть так же описаны сразу массивом задач
            var fgh = new Task<int>[3] {
                new Task<int>(() => F(e.Result,d.Result)),
                new Task<int>(() => G(e.Result,d.Result)),
                new Task<int>(() => H(e.Result,d.Result))
            };
			// Запуск параллельно сразу всех задач из массива
            foreach (var t in fgh)
                t.Start();
            Task.WaitAll(fgh);
            while (q.Count > i)
            {
                form.Log(q.Peek().time_end);
                form.Log("[COMPLETE]: " + q.Peek().id);
                q.Dequeue();
                form.Log("//---------------------------------------------------------------//");
            }
            form.Progress(8);
            // Данный Task ничего не возвратит 
            form.Log("[TIME START FOR K]: " + DateTime.Now + ":" + DateTime.Now.Millisecond);
            var k = new Task(() => K(fgh[0].Result, fgh[1].Result, fgh[2].Result));
            k.Start();
            k.Wait();
            while (q.Count > i)
            {
                form.Log(q.Peek().time_end);
                form.Log("[COMPLETE]: " + q.Peek().id);
                q.Dequeue();
                form.Log("//---------------------------------------------------------------//");
            }
            form.Progress(9);
            form.Log("DONE");
            q.Clear();
        }

        public void Log(Info invoker)
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

            var infoTask = new Info() { id = "A", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
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

            var infoTask = new Info() { id = "B", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
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

            var infoTask = new Info() { id = "C", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public int D(int result)
        {
            var res = result + 100;
            var infoTask = new Info() { id = "D", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public int E(int result)
        {
            var res = result / 50;
            var infoTask = new Info() { id = "E", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public int F(int resE,int resD)
        {
            int res = resD * resE;
            var infoTask = new Info() { id = "F", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public int G(int resE, int resD)
        {
            int res = (resD * resE)-45;
            var infoTask = new Info() { id = "G", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public int H(int resE, int resD)
        {
            int res = resD * resE*3;
            var infoTask = new Info() { id = "H", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
            return res;
        }

        public void K(int resF, int resG,int resH)
        {
            //some func
            int res = resF + resG + resH;
            var infoTask = new Info() { id = "K", time_end = "[TIME STOP]: " + DateTime.Now + ":" + DateTime.Now.Millisecond };
            TaskComplete?.Invoke(infoTask);
        }
    }
}