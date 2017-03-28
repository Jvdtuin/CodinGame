using CodersStrikeBack.AI;
using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CodersStrikeBack
{
    public partial class Display : Form
    {
        private Color[] colors = new Color[4]
        {
            Color.Blue,
            Color.Yellow,
            Color.Green,
            Color.Purple,
        };


        public Display()
        {
            InitializeComponent();
            _raceInfo = new RaceInfo();

        }

        private double _scaleFactor;

        private RaceInfo _raceInfo;

        private List<Pod> _pods = new List<Pod>();

        public void DrawMap()
        {

            Graphics g = CreateGraphics();
            g.Clear(Color.Black);
            double fx = this.Width / 16000.0;
            double fy = (this.Height - 50) / 9000.0;
            _scaleFactor = (fx < fy) ? fx : fy;
            Pen pen = new Pen(Color.Red, 1);
            // Draw checkpoints
            bool finish = true;
            foreach (Checkpoint cp in _raceInfo.Checkpoints)
            {
                int x = (int)(cp.Position.X * _scaleFactor);
                int y = (int)(cp.Position.Y * _scaleFactor);
                int r = (int)(cp.Radius * _scaleFactor);

                g.DrawEllipse(pen, x - r, y - r, 2 * r, 2 * r);
                if (finish)
                {
                    g.DrawEllipse(pen, x - 2*r, y -2* r, 4 * r, 4 * r);
                    finish = false;
                }                                
            }
        }

        private void DrawPod(Pod pod, Color color)
        {
            Graphics g = CreateGraphics();
            Pen pen = new Pen(color, 1);

            int x = (int)(pod.Position.X * _scaleFactor);
            int y = (int)(pod.Position.Y * _scaleFactor);
            int vx = (int)(pod.Velocity.X * _scaleFactor);
            int vy = (int)(pod.Velocity.Y * _scaleFactor);

            int r = (int)(pod.Radius * _scaleFactor);
            g.DrawEllipse(pen, x - r, y - r, 2 * r, 2 * r);
            g.DrawLine(pen, x, y, x + vx, y + vy);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _raceInfo = new RaceInfo();
            race = new Race(_raceInfo);
            DrawMap();
            for (int j = 0; j < race.Pods.Length; j++)
            {
                DrawPod(race.Pods[j], colors[j]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (running)
            {
                timer1.Stop();
                running = false;
            }
            else
            {
                timer1.Start();
                running = true;
            }

        }

        bool running = false;
        private Race race;

        private void RaceMove(Race race)
        {
            if (race.Move().HasValue)
            {
                timer1.Stop();

            }
            else

            {
                for (int j = 0; j < race.Pods.Length; j++)
                {
                    DrawPod(race.Pods[j], colors[j]);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RaceMove(race);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DrawMap();
            for (int j = 0; j < race.Pods.Length; j++)
            {
                DrawPod(race.Pods[j], colors[j]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Optimizer.Optimizer<TransformedPodBrain>
                optimizer = new Optimizer.Optimizer<TransformedPodBrain>();
            optimizer.CalcultatePopulationScores();
            
                
         }



        private int[] scores = new int[4] { 0, 0, 0, 0 };
    }
}
