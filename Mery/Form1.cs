using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Mery
{
    public partial class Form1 : Form
    {
        string Filename1;
        List<well> wells;
        List<int> strnamewells;
        List<OilField> Oilfields;
        List<string> spOilFields;

        public Form1()
        {
            this.wells = new List<well>();
            this.strnamewells = new List<int>();
            this.Oilfields = new List<OilField>();
            this.spOilFields = new List<string>();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string s;
                List<string> h = new List<string>();
                int i = 0;
                StreamReader sr = new StreamReader(openFileDialog1.FileName, Encoding.Default);
                while ((s = sr.ReadLine()) != null)
                {
                    h.Add(this.Tochki(s.ToString()));
                    i = i + 1;
                }

                Filename1 = openFileDialog1.FileName;
                for (int j = 0; j < h.Count; j++)
                {
                    if (h[j].Contains("ЭКСПЛУАТАЦИОННАЯ  КАРТОЧКА  НЕФТЯНОЙ  СКВАЖИНЫ  № ") || (j == h.Count - 1))
                    {
                        this.strnamewells.Add(j);
                    }
                }
                this.strnamewells[this.strnamewells.Count - 1]++;
                this.Zapolnenie(h);
            }
        }

        //заполнение скважин
        private void Zapolnenie(List<string> h)
        {
            for (int i = 0; i < this.strnamewells.Count - 1; i++)
            {
                well w1 = new well();
                string[] f = this.DellTab(h[strnamewells[i]]);
                w1.wellname = f[5];
                for (int j = strnamewells[i]; j < strnamewells[i + 1]; j++)
                {
                    w1.wellstrings.Add(h[j]);
                }
                this.wells.Add(this.WellPreobr(w1));
            }
            this.NaFields();
        }

        private well WellPreobr(well ww)
        {
            List<int> Nostr = new List<int>();
            for (int i = 0; i < ww.wellstrings.Count; i++)
            {
                if (ww.wellstrings[i].Contains("МЕСТОРОЖДЕНИЕ : "))
                {
                    string[] t = this.DellTab(ww.wellstrings[i]);
                    ww.wellfield = t[2];
                }
                if (ww.wellstrings[i].Contains("ДАТА НАЧАЛА ДОБЫЧИ"))
                {
                    string[] t = this.DellTab(ww.wellstrings[i]);
                    ww.wellstart = t[4];
                }
                if (ww.wellstrings[i].Contains("итого по скважине"))
                {
                    Nostr.Add(i);
                }
            }
            for (int k = Nostr[0] + 1; k < Nostr[1]; k++)
            {
                ww.wellstroki.Add(this.DellTabonly(ww.wellstrings[k]));
            }
            if (Nostr.Count > 2)
            {
                for (int u = 1; u < Nostr.Count - 1; u++)
                {
                    for (int r = Nostr[u] + 2; r < Nostr[u + 1]; r++)
                    {
                        ww.wellstroki.Add(this.DellTabonly(ww.wellstrings[r]));
                    }
                }
            }
            ww = this.Naplasty(ww);
            return ww;
        }

        private well Naplasty(well w1)
        {
            List<int> NoPl = new List<int>();
            List<string> naPl = new List<string>();
            string pl = w1.wellstroki[0][1];
            naPl.Add(pl);
            NoPl.Add(0);
            //w1.plasty.Add(new Plast(pl));

            for (int i = 1; i < w1.wellstroki.Count; i++)
            {
                if (naPl.Contains(w1.wellstroki[i][1]) != true)
                {
                    naPl.Add(w1.wellstroki[i][1]);
                    //Plast pl1 = new Plast(w1.wellstroki[i][1]);
                    //pl1.wellstrokipl.Add(w1.wellstroki[i]);
                    //w1.plasty.Add(pl1);
                }
                //else
                //{
                //    int hj = naPl.IndexOf(w1.wellstroki[i][1]);
                //    w1.plasty[hj].wellstrokipl.Add(w1.wellstroki[i]);
                //}
            }
            //NoPl[NoPl.Count - 1]++;

            for (int i = 0; i < naPl.Count; i++)
            {
                Plast pl1 = new Plast();
                pl1.name = naPl[i];
            //    for (int j = NoPl[i]; j < NoPl[i + 1]; j++)
            //    {
            //        pl1.wellstrokipl.Add(w1.wellstroki[j]);
            //    }
                w1.plasty.Add(pl1);
            }

            for (int i = 0; i < w1.wellstroki.Count; i++)
            {
                int hj = naPl.IndexOf(w1.wellstroki[i][1]);
                w1.plasty[hj].wellstrokipl.Add(w1.wellstroki[i]);
            }

            for (int i = 0; i < w1.plasty.Count; i++)
            {
                List<int> Nogods = new List<int>();
                string[] t = w1.plasty[i].wellstrokipl[0][0].Split(',');
                string gd = t[1];
                Nogods.Add(0);
                for (int j = 1; j < w1.plasty[i].wellstrokipl.Count; j++)
                {
                    string[] tt = w1.plasty[i].wellstrokipl[j][0].Split(',');
                    if ((tt[1] != gd) || (j == w1.plasty[i].wellstrokipl.Count - 1))
                    {
                        Nogods.Add(j);
                        gd = tt[1];
                    }
                }
                Nogods[Nogods.Count - 1]++;
                if (w1.plasty[i].wellstrokipl.Count == 1)
                {
                    God g1 = new God();
                    string[] tg = w1.plasty[i].wellstrokipl[0][0].Split(',');
                    g1.year = Int32.Parse(tg[1]);
                    string[] tg2 = w1.plasty[i].wellstrokipl[0][0].Split(',');
                    g1.month.Add(tg2[0]);
                    g1.sposobexpl.Add(w1.plasty[i].wellstrokipl[0][2]);
                    g1.days.Add(w1.plasty[i].wellstrokipl[0][4]);
                    g1.hours.Add(w1.plasty[i].wellstrokipl[0][6]);
                    g1.monoilproduction.Add(w1.plasty[i].wellstrokipl[0][10]);
                    g1.oilproduction.Add(w1.plasty[i].wellstrokipl[0][11]);
                    g1.monwaterproduction.Add(w1.plasty[i].wellstrokipl[0][13]);
                    g1.waterproduction.Add(w1.plasty[i].wellstrokipl[0][14]);
                    g1.percent.Add(w1.plasty[i].wellstrokipl[0][19]);
                    g1.oilrate.Add(w1.plasty[i].wellstrokipl[0][21]);
                    g1.liquidrate.Add(w1.plasty[i].wellstrokipl[0][23]);
                    double sumh = 0;
                    for (int l = 0; l < g1.hours.Count; l++)
                    {
                        sumh = sumh + double.Parse(g1.hours[l]);
                        g1.sumhours.Add(sumh);
                    }
                    w1.plasty[i].gods.Add(g1);
                }
                else
                {
                    int nom = 0;
                    for (int j = 1; j < Nogods.Count; j++)
                    {
                        God g1 = new God();
                        string[] tg = w1.plasty[i].wellstrokipl[nom][0].Split(',');
                        g1.year = Int32.Parse(tg[1]);
                        for (int k = nom; k < Nogods[j]; k++)
                        {
                            string[] tg2 = w1.plasty[i].wellstrokipl[k][0].Split(',');
                            g1.month.Add(tg2[0]);
                            g1.sposobexpl.Add(w1.plasty[i].wellstrokipl[k][2]);
                            g1.days.Add(w1.plasty[i].wellstrokipl[k][4]);
                            g1.hours.Add(w1.plasty[i].wellstrokipl[k][6]);
                            g1.monoilproduction.Add(w1.plasty[i].wellstrokipl[k][10]);
                            g1.oilproduction.Add(w1.plasty[i].wellstrokipl[k][11]);
                            g1.monwaterproduction.Add(w1.plasty[i].wellstrokipl[k][13]);
                            g1.waterproduction.Add(w1.plasty[i].wellstrokipl[k][14]);
                            g1.percent.Add(w1.plasty[i].wellstrokipl[k][19]);
                            g1.oilrate.Add(w1.plasty[i].wellstrokipl[k][21]);
                            g1.liquidrate.Add(w1.plasty[i].wellstrokipl[k][23]);
                        }
                        double sumh = 0;
                        for (int l = 0; l < g1.hours.Count; l++)
                        {
                            sumh = sumh + double.Parse(g1.hours[l]);
                            g1.sumhours.Add(sumh);
                        }
                        w1.plasty[i].gods.Add(g1);
                        nom = Nogods[j];
                    }
                }

                w1.plasty[i].sposob = w1.plasty[i].wellstrokipl[w1.plasty[i].wellstrokipl.Count - 1][2];
                for (int k = 0; k < w1.plasty[i].gods.Count; k++)
                {
                    w1.plasty[i].sumdays = w1.plasty[i].sumdays + double.Parse(w1.plasty[i].gods[k].days[w1.plasty[i].gods[k].days.Count - 1]);
                    w1.plasty[i].sumhours = w1.plasty[i].sumhours + w1.plasty[i].gods[k].sumhours[w1.plasty[i].gods[k].sumhours.Count - 1];
                    w1.plasty[i].sumoilproduction = w1.plasty[i].sumoilproduction + double.Parse(w1.plasty[i].gods[k].oilproduction[w1.plasty[i].gods[k].oilproduction.Count - 1]);
                    w1.plasty[i].sumwaterproduction = w1.plasty[i].sumwaterproduction + double.Parse(w1.plasty[i].gods[k].waterproduction[w1.plasty[i].gods[k].waterproduction.Count - 1]);
                }
                w1.plasty[i].sumpercentstart = w1.plasty[i].gods[0].percent[0];
                w1.plasty[i].sumpercentend = w1.plasty[i].gods[w1.plasty[i].gods.Count - 1].percent[w1.plasty[i].gods[w1.plasty[i].gods.Count - 1].percent.Count - 1];
            }
            return w1;
        }

        private OilField CreatOil(string name1, List<Plast> Plasty1, List<string> nameplasty1)
        {
            OilField ofd = new OilField(name1, Plasty1, nameplasty1);
            return ofd;
        }

        private void NaFields()
        {
            string field = wells[0].wellfield;
            spOilFields.Add(field);
            Oilfields.Add(new OilField());
            int no = 0;
            string namepl = wells[0].plasty[0].name;
            int k = 0;
            for (int i = 0; i < wells.Count; i++)
            {
                if (wells[i].wellfield != field)
                {
                    Oilfields.Add(new OilField());
                    namepl = wells[i].plasty[0].name;
                    field = wells[i].wellfield;
                    spOilFields.Add(field);
                    k = 0;
                    no++;
                    //ofd.Plasty.Add(new Plast(wells[i].plasty[0].name));
                    //ofd.nameplasty.Add(wells[i].plasty[0].name);                           
                }

                Oilfields[no].name = field;


                for (int j = 0; j < wells[i].plasty.Count; j++)
                {
                    if (Oilfields[no].Plasty.Count != 0)
                    {
                        if (Oilfields[no].nameplasty.Contains(wells[i].plasty[j].name))
                        {
                            k = Oilfields[no].nameplasty.IndexOf(wells[i].plasty[j].name);
                        }
                        else
                        {
                            Oilfields[no].Plasty.Add(new Plast(wells[i].plasty[j].name));
                            Oilfields[no].nameplasty.Add(wells[i].plasty[j].name);
                            k++;
                        }
                    }
                    else
                    {
                        Oilfields[no].Plasty.Add(new Plast(wells[i].plasty[j].name));
                        Oilfields[no].nameplasty.Add(wells[i].plasty[j].name);
                        k = 0;

                    }
                    Oilfields[no].Plasty[k].sumoilproduction = Oilfields[no].Plasty[k].sumoilproduction + wells[i].plasty[j].sumoilproduction;
                    Oilfields[no].Plasty[k].sumwaterproduction = Oilfields[no].Plasty[k].sumwaterproduction + wells[i].plasty[j].sumwaterproduction;
                }
            }
        }

        private string Tochki(string s)
        {
            string[] t = s.Split('.');
            string r = t[0];
            for (int i = 1; i < t.Length; i++)
            {
                r = r + ',' + t[i];
            }
            return r;
        }

        private string Zapyatie(string s)
        {
            string[] t = s.Split(',');
            string r = t[0];
            for (int i = 1; i < t.Length; i++)
            {
                r = r + '.' + t[i];
            }
            return r;
        }


        private string[] DellProbel(string stroka)
        {
            string[] t = stroka.Split(' ');
            string l = null;
            for (int j = 0; j < t.Length; j++)
            {
                if (t[j] != "")
                {
                    l = l + t[j] + " ";
                }
            }
            string[] q = l.Split(' ');
            return q;
        }

        private string[] DellTabonly(string stroka)
        {
            string[] t = stroka.Split('\t');

            return t;
        }

        private string[] DellTab(string stroka)
        {
            char[] del = new char[] { ' ', '\t' };
            string[] t = stroka.Split(del);
            string l = null;
            for (int j = 0; j < t.Length; j++)
            {
                if (t[j] != "")
                {
                    l = l + t[j] + " ";
                }
            }
            string[] q = l.Split(' ');
            return q;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] t = Filename1.Split('.');
            string s = t[0] + "_izm.txt";
            StreamWriter file = new StreamWriter(s, true, Encoding.Default);
            file.WriteLine("Скважина" + "\t" + "Месторождение" + "\t" + "Дата ввода в эксплуатацию" + "\t" + "Пласт" + "\t" + "Период" + "\t" + "Текущий способ эксплуатации" + "\t" + "Суммарное количество дней работы" + "\t" + "Суммарное количество часов работы" + "\t" + "Накопленная добыча нефти, т" + "\t" + "Накопленная добыча воды, т" + "\t" + "Процент обводненности на начало периода" + "\t" + "Процент обводненности на конец периода" + "\t" + "Дебит нефти на начало периода, т\\сут" + "\t" + "Дебит нефти на конец периода, т\\сут" + "\t" + "Дебит жидкости на начало периода, т\\сут" + "\t" + "Дебит жидкости на конец периода, т\\сут");
            string field = wells[0].wellfield;
            for (int i = 0; i < wells.Count; i++)
            {
                if (wells[i].wellfield != field)
                {
                    for (int j = 0; j < Oilfields[this.spOilFields.IndexOf(field)].Plasty.Count; j++)
                    {
                        file.WriteLine("Итого по месторождению" + "\t" + Oilfields[this.spOilFields.IndexOf(field)].name + "\t" + "" + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].name + "\t" + "Весь период" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].sumoilproduction.ToString() + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].sumwaterproduction.ToString());
                    }
                    field = wells[i].wellfield;
                    file.WriteLine("");
                }

                for (int j = 0; j < wells[i].plasty.Count; j++)
                {
                    for (int m = 0; m < wells[i].plasty[j].gods.Count; m++)
                    {
                        string k1 = wells[i].plasty[j].gods[m].year.ToString();
                        string k2 = wells[i].plasty[j].gods[m].sposobexpl[wells[i].plasty[j].gods[m].sposobexpl.Count - 1];
                        string k3 = wells[i].plasty[j].gods[m].days[wells[i].plasty[j].gods[m].days.Count - 1];
                        string k4 = wells[i].plasty[j].gods[m].sumhours[wells[i].plasty[j].gods[m].sumhours.Count - 1].ToString();
                        string k5 = wells[i].plasty[j].gods[m].oilproduction[wells[i].plasty[j].gods[m].oilproduction.Count - 1];
                        string k6 = wells[i].plasty[j].gods[m].waterproduction[wells[i].plasty[j].gods[m].waterproduction.Count - 1];
                        string k7 = wells[i].plasty[j].gods[m].percent[0];
                        string k8 = wells[i].plasty[j].gods[m].percent[wells[i].plasty[j].gods[m].percent.Count - 1];
                        string k9 = wells[i].plasty[j].gods[m].oilrate[0];
                        string k10 = wells[i].plasty[j].gods[m].oilrate[wells[i].plasty[j].gods[m].oilrate.Count - 1];
                        string k11 = wells[i].plasty[j].gods[m].liquidrate[0];
                        string k12 = wells[i].plasty[j].gods[m].liquidrate[wells[i].plasty[j].gods[m].liquidrate.Count - 1];
                        file.WriteLine(wells[i].wellname + "\t" + wells[i].wellfield + "\t" + wells[i].wellstart + "\t" + wells[i].plasty[j].name + "\t" + k1 + "\t" + k2 + "\t" + k3 + "\t" + k4 + "\t" + k5 + "\t" + k6 + "\t" + k7 + "\t" + k8 + "\t" + k9 + "\t" + k10 + "\t" + k11 + "\t" + k12);
                    }

                    file.WriteLine(wells[i].wellname + "\t" + wells[i].wellfield + "\t" + wells[i].wellstart + "\t" + wells[i].plasty[j].name + "\t" + "Весь период" + "\t" + wells[i].plasty[j].sposob + "\t" + wells[i].plasty[j].sumdays.ToString() + "\t" + wells[i].plasty[j].sumhours.ToString() + "\t" + wells[i].plasty[j].sumoilproduction.ToString() + "\t" + wells[i].plasty[j].sumwaterproduction.ToString() + "\t" + wells[i].plasty[j].sumpercentstart + "\t" + wells[i].plasty[j].sumpercentend);
                    file.WriteLine("");
                }
                //if (i == wells.Count -1)
                //{
                //    field = wells[i].wellfield;
                //    file.WriteLine("");
                //    for (int j = 0; j < Oilfields[this.spOilFields.IndexOf(field)].Plasty.Count; j++)
                //    {
                //        file.WriteLine("Итого по месторождению" + "\t" + field + "\t" + "" + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].name + "\t" + "Весь период" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].sumoilproduction.ToString() + "\t" + Oilfields[this.spOilFields.IndexOf(field)].Plasty[j].sumwaterproduction.ToString());
                //    }                    
                //    file.WriteLine("");
                //}               
            }
            for (int j = 0; j < Oilfields[Oilfields.Count - 1].Plasty.Count; j++)
            {
                file.WriteLine("Итого по месторождению" + "\t" + Oilfields[Oilfields.Count - 1].name + "\t" + "" + "\t" + Oilfields[Oilfields.Count - 1].Plasty[j].name + "\t" + "Весь период" + "\t" + "" + "\t" + "" + "\t" + "" + "\t" + Oilfields[Oilfields.Count - 1].Plasty[j].sumoilproduction.ToString() + "\t" + Oilfields[Oilfields.Count - 1].Plasty[j].sumwaterproduction.ToString());
            }
            file.WriteLine("");
            file.Close();
            this.Oilfields.Clear();
            this.wells.Clear();
            this.strnamewells.Clear();
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int ww = 0; ww < wells.Count; ww++)
            {
                for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
                {
                    int kol = 0;
                    int uroven1 = 300;
                    int otstup = 40;
                    int shirina = 15;
                    float koordY = 240;
                    int prob_Oy = 40;
                    int k_Oy = 5;
                    float sumoi = 0;
                    float sumw = 0;
                    
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].monoilproduction.Count; j++)
                        {
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monoilproduction[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].monwaterproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monwaterproduction[j] = "0";
                            }
                            sumoi = sumoi + float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]);
                            sumw = sumw + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]);
                            wells[ww].plasty[pp].gods[i].sumoilproduction.Add(sumoi.ToString());
                            wells[ww].plasty[pp].gods[i].sumwaterproduction.Add(sumw.ToString());
                            kol++;                            
                        }
                    }

                    int c1 = wells[ww].plasty[pp].gods.Count - 1;
                    int c2 = wells[ww].plasty[pp].gods[c1].sumwaterproduction.Count - 1;
                    float max_sum_prod = float.Parse(wells[ww].plasty[pp].gods[c1].sumwaterproduction[c2]) + float.Parse(wells[ww].plasty[pp].gods[c1].sumoilproduction[c2]);


                    if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
                    {
                        this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 150;
                    }

                    this.pictureBox1.Width = otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 110;
                    this.pictureBox1.Height = uroven1 + 100;

                    Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
                    pictureBox1.Image = bmp;

                    //оси

                    Graphics gr = Graphics.FromImage(bmp);
                    // Graphics gr = this.pictureBox1.CreateGraphics();
                    Font font1 = new System.Drawing.Font("Times New Roman", 7);

                    SolidBrush s_osi = new SolidBrush(Color.Black);
                    SolidBrush s_oil = new SolidBrush(Color.Red);
                    SolidBrush s_w = new SolidBrush(Color.Blue);
                    Pen p = new Pen(s_osi, 1);// цвет линии и ширина
                    PointF p_Oy = new PointF(otstup, uroven1 - koordY);// первая точка
                    PointF p_Ox = new PointF(otstup * 2 + 10 + (kol + wells[ww].plasty[pp].gods.Count) * (shirina + 2), uroven1);// вторая точка
                    PointF p0 = new PointF(otstup, uroven1);// вторая точка
                    PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
                    PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
                    PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
                    PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
                    gr.DrawLine(p, p0, p_Oy);// рисуем линию
                    gr.DrawLine(p, p0, p_Ox);
                    gr.DrawLine(p, str_p2_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Ox, p_Ox);
                    gr.DrawLine(p, str_p2_Ox, p_Ox);

                    int delprob = 5;
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 3)
                    {
                        delprob = 50;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 4)
                    {
                        delprob = 500;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 5)
                    {
                        delprob = 5000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 6)
                    {
                        delprob = 50000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 7)
                    {
                        delprob = 500000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 8)
                    {
                        delprob = 5000000;
                    }

                    float maxznach = 0;
                    int intervOy = Int32.Parse((Math.Round(double.Parse(Math.Round(max_sum_prod / 5).ToString()) / delprob) * delprob).ToString());
                    for (int o = 0; o < k_Oy; o++)
                    {
                        PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF t_Oy = new PointF(0, uroven1 - (prob_Oy + 8 + o * prob_Oy));
                        gr.DrawLine(p, ch_Oy_1, ch_Oy_2);                        
                        gr.DrawString((intervOy + o * intervOy).ToString(), font1, s_osi, t_Oy);
                        maxznach = maxznach + intervOy;
                    }

                    //прямоугольники
                    int koll = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            float r_oil_p1X = otstup * 2 + koll * (shirina + 2);
                            float r_oil_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumoilproduction[j]) / maxznach;
                            float h = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumoilproduction[j]) / maxznach;
                            if (wells[ww].plasty[pp].gods[i].sumoilproduction[j] == "0")
                            {
                                h = 0;
                                r_oil_p1Y = uroven1;
                            }
                            gr.FillRectangle(s_oil, r_oil_p1X, r_oil_p1Y, shirina, h);
                            float r_w_p1Y = r_oil_p1Y - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumwaterproduction[j]) / maxznach;
                            float hw = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumwaterproduction[j]) / maxznach;
                            gr.FillRectangle(Brushes.Blue, r_oil_p1X, r_w_p1Y, shirina, hw);

                            //месяцы
                            PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
                            PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
                            PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
                            gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
                            gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
                            koll++;

                        }

                        koll++;
                    }


                    //года
                    int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
                    PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
                    gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
                    for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
                        PointF t_Ox_g2 = new PointF(otstup * 2 + shirina / 2 + (oh + i) * (shirina + 2), uroven1 + 15);
                        gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
                    }

                    //подписи
                    PointF t_g = new PointF(p_Ox.X, uroven1 + 15);
                    PointF t_m = new PointF(p_Ox.X, uroven1 + 3);
                    PointF t_d = new PointF(otstup - 20, uroven1 - koordY - 15);
                    PointF t_p1 = new PointF(otstup, uroven1 - koordY - 30);
                    PointF t_p2 = new PointF(otstup, uroven1 - koordY - 45);
                    PointF t_p3 = new PointF(otstup, uroven1 - koordY - 60);
                    gr.DrawString("Год", font1, s_osi, t_g);
                    gr.DrawString("Месяц", font1, s_osi, t_m);
                    gr.DrawString("Добыча с начала разработки, т", font1, s_osi, t_d);

                    Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
                    gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
                    gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
                    gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);


                    //условки
                    string w = " - Добыча воды";
                    string oi = " - Добыча нефти";
                    gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
                    PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
                    gr.DrawString(oi, font1, s_osi, t_o);
                    gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
                    PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
                    gr.DrawString(w, font1, s_osi, t_w);

                    //название файла
                    string[] t = Filename1.Split('\\');
                    string s = "";
                    for (int l = 0; l < t.Length - 1; l++)
                    {
                        s = s + t[l] + "\\";
                    }

                    s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_добыча_с_начала_разработки" + ".png";

                    //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
                    // this.pictureBox1.Image = bmp;
                    bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    gr.Dispose();
                    //bmp.Dispose();
                }
            }
        }

        //дебит
        private void button4_Click(object sender, EventArgs e)
        {
            for (int ww = 0; ww < wells.Count; ww++)
            {
                for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
                {
                    int kol = 0;                    
                    float max_sum_rate = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            if (wells[ww].plasty[pp].gods[i].liquidrate[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].liquidrate[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].oilrate[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].oilrate[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].oilproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].oilproduction[j] = "0";
                            }
                            kol++;
                            if (max_sum_rate < float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]))
                            {
                                max_sum_rate = float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]);
                            }
                        }
                    }

                    int uroven1 = 300;
                    int otstup = 20;
                    int shirina = 15;
                    float koordY = 240;

                    int prob_Oy = 40;
                    int k_Oy = 5;

                    
                    if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
                    {
                        this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 150;
                    }
                    this.pictureBox1.Width = otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 110;
                    this.pictureBox1.Height = uroven1+100;
                    Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
                    pictureBox1.Image = bmp;
                    Font font1 = new System.Drawing.Font("Times New Roman", 7);
                  
                    Graphics gr = Graphics.FromImage(bmp);
                    
                    //оси
                    SolidBrush s_osi = new SolidBrush(Color.Black);
                    SolidBrush s_oil = new SolidBrush(Color.Red);
                    SolidBrush s_w = new SolidBrush(Color.Blue);
                    Pen p = new Pen(s_osi, 1);// цвет линии и ширина
                    PointF p_Oy = new PointF(otstup, uroven1-koordY);// первая точка
                    PointF p_Ox = new PointF(otstup * 2 + 10 + (kol + wells[ww].plasty[pp].gods.Count) * (shirina + 2), uroven1);// вторая точка
                    PointF p0 = new PointF(otstup, uroven1);// вторая точка
                    PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
                    PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
                    PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
                    PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
                    gr.DrawLine(p, p0, p_Oy);// рисуем линию
                    gr.DrawLine(p, p0, p_Ox);
                    gr.DrawLine(p, str_p2_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Ox, p_Ox);
                    gr.DrawLine(p, str_p2_Ox, p_Ox);
                    float maxznach = 0;
                    int delprob = 5;
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 3)
                    {
                        delprob = 50;
                    }
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 4)
                    {
                        delprob = 500;
                    }
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 5)
                    {
                        delprob = 5000;
                    }
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 6)
                    {
                        delprob = 50000;
                    }
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 7)
                    {
                        delprob = 500000;
                    }
                    if (Math.Round(double.Parse(max_sum_rate.ToString())).ToString().Length == 8)
                    {
                        delprob = 5000000;
                    }

                    
                    int intervOy = Int32.Parse((Math.Round(double.Parse(Math.Round(max_sum_rate / 5).ToString()) / delprob) * delprob).ToString());
                    for (int o = 0; o < k_Oy; o++)
                    {
                        PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF t_Oy = new PointF(0, uroven1 - (prob_Oy + 8 + o * prob_Oy));
                        gr.DrawLine(p, ch_Oy_1, ch_Oy_2);                        
                        gr.DrawString((intervOy + o * intervOy).ToString(), font1, s_osi, t_Oy);
                        maxznach = maxznach + intervOy;
                    }

                    //прямоугольники
                    int koll = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            float r_oil_p1X = otstup * 2 + koll * (shirina + 2);
                            float r_oil_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j]) / maxznach;
                            float h = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j]) / maxznach;
                            if (wells[ww].plasty[pp].gods[i].oilrate[j] == "")
                            {
                                h = 0;
                                r_oil_p1Y = uroven1;
                            }
                            gr.FillRectangle(s_oil, r_oil_p1X, r_oil_p1Y, shirina, h);
                            float r_w_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]) / maxznach;
                            float hw = (koordY - prob_Oy) * (float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]) - float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j])) / maxznach;
                            gr.FillRectangle(Brushes.Blue, r_oil_p1X, r_w_p1Y, shirina, hw);
                            //подписи месяцев
                            PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
                            PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
                            PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
                            gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
                            gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
                            koll++;

                        }

                        koll++;
                    }

                    //подписи годов
                    int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
                    PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
                    gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
                    for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
                        PointF t_Ox_g2 = new PointF(otstup * 2 + shirina / 2 + (oh + i) * (shirina + 2), uroven1 + 15);
                        gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
                    }

                    
                    //подписи
                    PointF t_g = new PointF(p_Ox.X, uroven1 + 15);
                    PointF t_m = new PointF(p_Ox.X, uroven1 + 3);
                    PointF t_d = new PointF(otstup - 20, uroven1 - koordY - 15);
                    PointF t_p1 = new PointF(otstup, uroven1 - koordY - 30);
                    PointF t_p2 = new PointF(otstup, uroven1 - koordY - 45);
                    PointF t_p3 = new PointF(otstup, uroven1 - koordY - 60);
                    gr.DrawString("Год", font1, s_osi, t_g);
                    gr.DrawString("Месяц", font1, s_osi, t_m);
                    gr.DrawString("Дебит, т/сут", font1, s_osi, t_d);

                    Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
                    gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
                    gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
                    gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);

                    //условки
                    string w = " - Дебит воды";
                    string oi = " - Дебит нефти";
                    gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
                    PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
                    gr.DrawString(oi, font1, s_osi, t_o);
                    gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
                    PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
                    gr.DrawString(w, font1, s_osi, t_w);
                    //название файла и запись
                    string[] t = Filename1.Split('\\');
                    string s = "";
                    for (int l = 0; l < t.Length - 1; l++)
                    {
                        s = s + t[l] + "\\";
                    }

                    s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_дебит"+".png";

                    //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
                    // this.pictureBox1.Image = bmp;
                    bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    gr.Dispose();
                    //bmp.Dispose();
                }
            }          
        }

        //добыча за месяц
        private void button5_Click(object sender, EventArgs e)
        {
            for (int ww = 0; ww < wells.Count; ww++)
            {
                for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
                {
                    int kol = 0;
                    int uroven1 = 300;
                    int otstup = 30;
                    int shirina = 15;
                    float koordY = 240;
                    int prob_Oy = 40;
                    int k_Oy = 5;
                    float max_sum_prod = 0;                    
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].monoilproduction.Count; j++)
                        {
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monoilproduction[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].monwaterproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monwaterproduction[j] = "0";
                            }
                            kol++;
                            if (max_sum_prod < (float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j])))
                            {
                                max_sum_prod = float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]);
                            }
                        }
                    }


                                       
                    if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
                    {
                        this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 150;
                    }

                    this.pictureBox1.Width = otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 110;
                    this.pictureBox1.Height = uroven1 + 100;

                    Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
                    pictureBox1.Image = bmp;

                    //оси
                    
                    Graphics gr = Graphics.FromImage(bmp);
                    // Graphics gr = this.pictureBox1.CreateGraphics();
                    Font font1 = new System.Drawing.Font("Times New Roman", 7);
                    
                    SolidBrush s_osi = new SolidBrush(Color.Black);
                    SolidBrush s_oil = new SolidBrush(Color.Red);
                    SolidBrush s_w = new SolidBrush(Color.Blue);
                    Pen p = new Pen(s_osi, 1);// цвет линии и ширина
                    PointF p_Oy = new PointF(otstup, uroven1 - koordY);// первая точка
                    PointF p_Ox = new PointF(otstup * 2 + 10 + (kol+ wells[ww].plasty[pp].gods.Count) * (shirina + 2), uroven1);// вторая точка
                    PointF p0 = new PointF(otstup, uroven1);// вторая точка
                    PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
                    PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
                    PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
                    PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
                    gr.DrawLine(p, p0, p_Oy);// рисуем линию
                    gr.DrawLine(p, p0, p_Ox);
                    gr.DrawLine(p, str_p2_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Ox, p_Ox);
                    gr.DrawLine(p, str_p2_Ox, p_Ox);

                    int delprob = 5;
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 3)
                    {
                        delprob = 50;
                    }
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 4)
                    {
                        delprob = 500;
                    }
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 5)
                    {
                        delprob = 5000;
                    }
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 6)
                    {
                        delprob = 50000;
                    }
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 7)
                    {
                        delprob = 500000;
                    }
                    if (Math.Round(double.Parse(max_sum_prod.ToString())).ToString().Length == 8)
                    {
                        delprob = 5000000;
                    }

                    float maxznach = 0;
                    int intervOy = Int32.Parse((Math.Round(double.Parse(Math.Round(max_sum_prod / 5).ToString()) / delprob) * delprob).ToString());
                    for (int o = 0; o < k_Oy; o++)
                    {
                        PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF t_Oy = new PointF(0, uroven1 - (prob_Oy + 8 + o * prob_Oy));
                        gr.DrawLine(p, ch_Oy_1, ch_Oy_2);                        
                        gr.DrawString((intervOy + o * intervOy).ToString(), font1, s_osi, t_Oy);
                        maxznach = maxznach + intervOy;
                    }

                    //прямоугольники
                    int koll = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            float r_oil_p1X = otstup * 2 + koll * (shirina + 2);
                            float r_oil_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) / maxznach;
                            float h = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) / maxznach;
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "0")
                            {
                                h = 0;
                                r_oil_p1Y = uroven1;
                            }
                            gr.FillRectangle(s_oil, r_oil_p1X, r_oil_p1Y, shirina, h);
                            float r_w_p1Y = r_oil_p1Y - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]) / maxznach;
                            float hw = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]) / maxznach;
                            gr.FillRectangle(Brushes.Blue, r_oil_p1X, r_w_p1Y, shirina, hw);
                            
                            //месяцы
                            PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
                            PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
                            PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
                            gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
                            gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
                            koll++;

                        }

                        koll++;
                    }
                    
                   
                    //года
                    int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
                    PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
                    gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
                    for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
                        PointF t_Ox_g2 = new PointF(otstup * 2 + shirina / 2 + (oh + i) * (shirina + 2), uroven1 + 15);
                        gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
                    }

                    //подписи
                    PointF t_g = new PointF(p_Ox.X, uroven1 + 15);
                    PointF t_m = new PointF(p_Ox.X, uroven1 + 3);
                    PointF t_d = new PointF(otstup - 20, uroven1 - koordY - 15);
                    PointF t_p1 = new PointF(otstup, uroven1 - koordY - 30);
                    PointF t_p2 = new PointF(otstup, uroven1 - koordY - 45);
                    PointF t_p3 = new PointF(otstup, uroven1 - koordY - 60);
                    gr.DrawString("Год", font1, s_osi, t_g);
                    gr.DrawString("Месяц", font1, s_osi, t_m);
                    gr.DrawString("Добыча за месяц, т", font1, s_osi, t_d);

                    Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
                    gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
                    gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
                    gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);


                    //условки
                    string w = " - Добыча воды";
                    string oi = " - Добыча нефти";
                    gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
                    PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
                    gr.DrawString(oi, font1, s_osi, t_o);
                    gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
                    PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
                    gr.DrawString(w, font1, s_osi, t_w);

                    //название файла
                    string[] t = Filename1.Split('\\');
                    string s = "";
                    for (int l = 0; l < t.Length - 1; l++)
                    {
                        s = s + t[l] + "\\";
                    }

                    s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_добыча_за_месяц" + ".png";

                    //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
                    // this.pictureBox1.Image = bmp;
                    bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    gr.Dispose();
                    //bmp.Dispose();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int ww = 0; ww < wells.Count; ww++)
            {
                for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
                {
                    int kol = 0;
                    int uroven1 = 300;
                    int otstup = 40;
                    int shirina = 15;
                    float koordY = 240;
                    int prob_Oy = 40;
                    int k_Oy = 5;
                    float sumoi = 0;
                    float sumw = 0;
                    float max_sum_prod = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].monoilproduction.Count; j++)
                        {
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monoilproduction[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].monwaterproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monwaterproduction[j] = "0";
                            }
                            sumoi = sumoi + float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]);
                            sumw = sumw + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]);
                            wells[ww].plasty[pp].gods[i].sumoilproduction.Add(sumoi.ToString());
                            wells[ww].plasty[pp].gods[i].sumwaterproduction.Add(sumw.ToString());
                            kol++;
                        }
                    }

                    int c1 = wells[ww].plasty[pp].gods.Count - 1;
                    int c2 = wells[ww].plasty[pp].gods[c1].sumwaterproduction.Count - 1;
                    if (float.Parse(wells[ww].plasty[pp].gods[c1].sumwaterproduction[c2]) >= float.Parse(wells[ww].plasty[pp].gods[c1].sumoilproduction[c2]))
                    {
                        max_sum_prod = float.Parse(wells[ww].plasty[pp].gods[c1].sumwaterproduction[c2]);
                    }
                    else
                    {
                        max_sum_prod = float.Parse(wells[ww].plasty[pp].gods[c1].sumoilproduction[c2]);
                    }

                    if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
                    {
                        this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 150;
                    }

                    this.pictureBox1.Width = otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 110;
                    this.pictureBox1.Height = uroven1 + 100;

                    Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
                    pictureBox1.Image = bmp;

                    //оси

                    Graphics gr = Graphics.FromImage(bmp);
                    // Graphics gr = this.pictureBox1.CreateGraphics();
                    Font font1 = new System.Drawing.Font("Times New Roman", 7);

                    SolidBrush s_osi = new SolidBrush(Color.Black);
                    SolidBrush s_oil = new SolidBrush(Color.Red);
                    SolidBrush s_w = new SolidBrush(Color.Blue);
                    Pen p = new Pen(s_osi, 1);// цвет линии и ширина
                    Pen penoil = new Pen(s_oil, 3);
                    Pen penw = new Pen(s_w, 3);
                    PointF p_Oy = new PointF(otstup, uroven1 - koordY);// первая точка
                    PointF p_Ox = new PointF(otstup * 2 + 10 + (kol + wells[ww].plasty[pp].gods.Count) * (shirina + 2), uroven1);// вторая точка
                    PointF p0 = new PointF(otstup, uroven1);// вторая точка
                    PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
                    PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
                    PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
                    PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
                    gr.DrawLine(p, p0, p_Oy);// рисуем линию
                    gr.DrawLine(p, p0, p_Ox);
                    gr.DrawLine(p, str_p2_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Ox, p_Ox);
                    gr.DrawLine(p, str_p2_Ox, p_Ox);

                    int delprob = 5;
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 3)
                    {
                        delprob = 50;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 4)
                    {
                        delprob = 500;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 5)
                    {
                        delprob = 5000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 6)
                    {
                        delprob = 50000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 7)
                    {
                        delprob = 500000;
                    }
                    if (Math.Round(max_sum_prod / 5).ToString().Length == 8)
                    {
                        delprob = 5000000;
                    }

                    float maxznach = 0;
                    int intervOy = Int32.Parse((Math.Round(double.Parse(Math.Round(max_sum_prod / 5).ToString()) / delprob) * delprob).ToString());
                    for (int o = 0; o < k_Oy; o++)
                    {
                        PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF t_Oy = new PointF(0, uroven1 - (prob_Oy + 8 + o * prob_Oy));
                        gr.DrawLine(p, ch_Oy_1, ch_Oy_2);
                        gr.DrawString((intervOy + o * intervOy).ToString(), font1, s_osi, t_Oy);
                        maxznach = maxznach + intervOy;
                    }

                    //прямые
                    int koll = 0;
                    List<PointF> P_oils = new List<PointF>();
                    List<PointF> P_ws = new List<PointF>();
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            float r_oil_p1X = otstup * 2 + shirina / 2 + koll * (shirina + 2);
                            float r_oil_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumoilproduction[j]) / maxznach;
                            float r_w_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].sumwaterproduction[j]) / maxznach;
                            if (wells[ww].plasty[pp].gods[i].sumoilproduction[j] == "0")
                            {                               
                                r_oil_p1Y = uroven1;
                            }
                            if (wells[ww].plasty[pp].gods[i].sumwaterproduction[j] == "0")
                            {
                                r_w_p1Y = uroven1;
                            }
                            P_oils.Add(new PointF(r_oil_p1X, r_oil_p1Y));
                            P_ws.Add(new PointF(r_oil_p1X, r_w_p1Y));
                            //месяцы
                            PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
                            PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
                            PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
                            gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
                            gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
                            koll++;
                        }
                        koll++;
                    }

                    for (int i = 0; i < P_oils.Count - 1; i++)
                    {
                        gr.DrawLine(penoil, P_oils[i], P_oils[i + 1]);
                        gr.DrawLine(penw, P_ws[i], P_ws[i + 1]);
                    }


                    //года
                    int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
                    PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
                    gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
                    for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
                        PointF t_Ox_g2 = new PointF(otstup * 2 + shirina / 2 + (oh + i) * (shirina + 2), uroven1 + 15);
                        gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
                    }

                    //подписи
                    PointF t_g = new PointF(p_Ox.X, uroven1 + 15);
                    PointF t_m = new PointF(p_Ox.X, uroven1 + 3);
                    PointF t_d = new PointF(otstup - 20, uroven1 - koordY - 15);
                    PointF t_p1 = new PointF(otstup, uroven1 - koordY - 30);
                    PointF t_p2 = new PointF(otstup, uroven1 - koordY - 45);
                    PointF t_p3 = new PointF(otstup, uroven1 - koordY - 60);
                    gr.DrawString("Год", font1, s_osi, t_g);
                    gr.DrawString("Месяц", font1, s_osi, t_m);
                    gr.DrawString("Добыча с начала разработки, т", font1, s_osi, t_d);

                    Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
                    gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
                    gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
                    gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);


                    //условки
                    string w = " - Добыча воды";
                    string oi = " - Добыча нефти";
                    gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
                    PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
                    gr.DrawString(oi, font1, s_osi, t_o);
                    gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
                    PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
                    gr.DrawString(w, font1, s_osi, t_w);

                    //название файла
                    string[] t = Filename1.Split('\\');
                    string s = "";
                    for (int l = 0; l < t.Length - 1; l++)
                    {
                        s = s + t[l] + "\\";
                    }

                    s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_добыча_с_начала_разработки_нефть_и_вода" + ".png";

                    //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
                    // this.pictureBox1.Image = bmp;
                    bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    gr.Dispose();
                    //bmp.Dispose();
                }
            }
        }


        private void PaintGraf(string stroka)
        {            
            for (int ww = 0; ww < wells.Count; ww++)
            {
                for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
                {
                    int kol = 0;
                    int uroven1 = 300;
                    int otstup = 30;
                    int shirina = 15;
                    float koordY = 240;
                    int prob_Oy = 40;
                    int k_Oy = 5;
                    float max_sum_prod = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].monoilproduction.Count; j++)
                        {
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monoilproduction[j] = "0";
                            }
                            if (wells[ww].plasty[pp].gods[i].monwaterproduction[j] == "")
                            {
                                wells[ww].plasty[pp].gods[i].monwaterproduction[j] = "0";
                            }
                            kol++;
                            if (max_sum_prod < (float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j])))
                            {
                                max_sum_prod = float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) + float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]);
                            }
                        }
                    }



                    if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
                    {
                        this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 150;
                    }

                    this.pictureBox1.Width = otstup * 2 + (kol + wells[ww].plasty[pp].gods.Count - 1) * (shirina + 2) + 110;
                    this.pictureBox1.Height = uroven1 + 100;

                    Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
                    pictureBox1.Image = bmp;

                    //оси

                    Graphics gr = Graphics.FromImage(bmp);
                    // Graphics gr = this.pictureBox1.CreateGraphics();
                    Font font1 = new System.Drawing.Font("Times New Roman", 7);

                    SolidBrush s_osi = new SolidBrush(Color.Black);
                    SolidBrush s_oil = new SolidBrush(Color.Red);
                    SolidBrush s_w = new SolidBrush(Color.Blue);
                    Pen p = new Pen(s_osi, 1);// цвет линии и ширина
                    PointF p_Oy = new PointF(otstup, uroven1 - koordY);// первая точка
                    PointF p_Ox = new PointF(otstup * 2 + 10 + (kol + wells[ww].plasty[pp].gods.Count) * (shirina + 2), uroven1);// вторая точка
                    PointF p0 = new PointF(otstup, uroven1);// вторая точка
                    PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
                    PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
                    PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
                    PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
                    gr.DrawLine(p, p0, p_Oy);// рисуем линию
                    gr.DrawLine(p, p0, p_Ox);
                    gr.DrawLine(p, str_p2_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Oy, p_Oy);
                    gr.DrawLine(p, str_p1_Ox, p_Ox);
                    gr.DrawLine(p, str_p2_Ox, p_Ox);

                    float maxznach = 0;
                    for (int o = 0; o < k_Oy; o++)
                    {
                        PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
                        PointF t_Oy = new PointF(0, uroven1 - (prob_Oy + 8 + o * prob_Oy));
                        gr.DrawLine(p, ch_Oy_1, ch_Oy_2);
                        int intervOy = Int32.Parse((Math.Round(double.Parse(Math.Round(max_sum_prod / 5).ToString()) / 50) * 50).ToString());
                        gr.DrawString((intervOy + o * intervOy).ToString(), font1, s_osi, t_Oy);
                        maxznach = maxznach + intervOy;
                    }

                    //прямоугольники
                    int koll = 0;
                    for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
                        {
                            float r_oil_p1X = otstup * 2 + koll * (shirina + 2);
                            float r_oil_p1Y = uroven1 - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) / maxznach;
                            float h = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monoilproduction[j]) / maxznach;
                            if (wells[ww].plasty[pp].gods[i].monoilproduction[j] == "0")
                            {
                                h = 0;
                                r_oil_p1Y = uroven1;
                            }
                            gr.FillRectangle(s_oil, r_oil_p1X, r_oil_p1Y, shirina, h);
                            float r_w_p1Y = r_oil_p1Y - (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]) / maxznach;
                            float hw = (koordY - prob_Oy) * float.Parse(wells[ww].plasty[pp].gods[i].monwaterproduction[j]) / maxznach;
                            gr.FillRectangle(Brushes.Blue, r_oil_p1X, r_w_p1Y, shirina, hw);

                            //месяцы
                            PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
                            PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
                            PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
                            gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
                            gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
                            koll++;

                        }

                        koll++;
                    }


                    //года
                    int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
                    PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
                    gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
                    for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
                    {
                        oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
                        PointF t_Ox_g2 = new PointF(otstup * 2 + shirina / 2 + (oh + i) * (shirina + 2), uroven1 + 15);
                        gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
                    }

                    //подписи
                    PointF t_g = new PointF(p_Ox.X, uroven1 + 15);
                    PointF t_m = new PointF(p_Ox.X, uroven1 + 3);
                    PointF t_d = new PointF(otstup - 20, uroven1 - koordY - 15);
                    PointF t_p1 = new PointF(otstup, uroven1 - koordY - 30);
                    PointF t_p2 = new PointF(otstup, uroven1 - koordY - 45);
                    PointF t_p3 = new PointF(otstup, uroven1 - koordY - 60);
                    gr.DrawString("Год", font1, s_osi, t_g);
                    gr.DrawString("Месяц", font1, s_osi, t_m);
                    gr.DrawString("Добыча за месяц, т", font1, s_osi, t_d);

                    Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
                    gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
                    gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
                    gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);


                    //условки
                    string w = " - Добыча воды";
                    string oi = " - Добыча нефти";
                    gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
                    PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
                    gr.DrawString(oi, font1, s_osi, t_o);
                    gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
                    PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
                    gr.DrawString(w, font1, s_osi, t_w);

                    //название файла
                    string[] t = Filename1.Split('\\');
                    string s = "";
                    for (int l = 0; l < t.Length - 1; l++)
                    {
                        s = s + t[l] + "\\";
                    }

                    s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_добыча_за_месяц" + ".png";

                    //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
                    // this.pictureBox1.Image = bmp;
                    bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
                    gr.Dispose();
                    //bmp.Dispose();
                }
            }            
        }





        //for (int ww = 0; ww < wells.Count; ww++)
        //    {
        //        for (int pp = 0; pp < wells[ww].plasty.Count; pp++)
        //        {
        //            int kol = 0;                    
        //            float max_sum_rate = 0;
        //            for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
        //            {
        //                for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
        //                {
        //                    if (wells[ww].plasty[pp].gods[i].liquidrate[j] == "")
        //                    {
        //                        wells[ww].plasty[pp].gods[i].liquidrate[j] = "0";
        //                    }
        //                    if (wells[ww].plasty[pp].gods[i].oilrate[j] == "")
        //                    {
        //                        wells[ww].plasty[pp].gods[i].oilrate[j] = "0";
        //                    }
        //                    if (wells[ww].plasty[pp].gods[i].oilproduction[j] == "")
        //                    {
        //                        wells[ww].plasty[pp].gods[i].oilproduction[j] = "0";
        //                    }
        //                    kol++;
        //                    if (max_sum_rate < float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]))
        //                    {
        //                        max_sum_rate = float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]);
        //                    }
        //                }
        //            }

        //            int uroven1 = 300;
        //            int otstup = 20;
        //            int shirina = 15;
                    
        //            if (otstup * 2 + kol * (shirina + 2) + 50 > this.pictureBox1.Width)
        //            {
        //                this.Width = this.Width + this.pictureBox1.Width - otstup * 2 + kol * (shirina + 2) + 150;
        //            }
        //            this.pictureBox1.Width = otstup * 2 + kol * (shirina + 2) + 50;
        //            this.pictureBox1.Height = uroven1;
        //            Bitmap bmp = new Bitmap(Image.FromFile("C:\\Users\\Juju\\Downloads\\white.png"), this.pictureBox1.Width, this.pictureBox1.Height);
        //            pictureBox1.Image = bmp;

        //            //pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);   
        //            int prob_Oy = 20;
        //            int k_Oy = Int32.Parse(Math.Round(max_sum_rate / prob_Oy).ToString());
        //            Graphics gr = Graphics.FromImage(bmp);
        //            // Graphics gr = this.pictureBox1.CreateGraphics();
        //            Font font1 = new System.Drawing.Font("Times New Roman", 7);
        //            SolidBrush s_osi = new SolidBrush(Color.Black);
        //            SolidBrush s_oil = new SolidBrush(Color.Red);
        //            SolidBrush s_w = new SolidBrush(Color.Blue);
        //            Pen p = new Pen(s_osi, 1);// цвет линии и ширина
        //            PointF p_Oy = new PointF(otstup, uroven1 - otstup - max_sum_rate);// первая точка
        //            PointF p_Ox = new PointF(otstup * 4 + 10 + kol * (shirina + 2), uroven1);// вторая точка
        //            PointF p0 = new PointF(otstup, uroven1);// вторая точка
        //            PointF str_p1_Oy = new PointF(p_Oy.X - 5, p_Oy.Y + 5);
        //            PointF str_p2_Oy = new PointF(p_Oy.X + 5, p_Oy.Y + 5);
        //            PointF str_p1_Ox = new PointF(p_Ox.X - 5, p_Ox.Y - 5);
        //            PointF str_p2_Ox = new PointF(p_Ox.X - 5, p_Ox.Y + 5);
        //            gr.DrawLine(p, p0, p_Oy);// рисуем линию
        //            gr.DrawLine(p, p0, p_Ox);
        //            gr.DrawLine(p, str_p2_Oy, p_Oy);
        //            gr.DrawLine(p, str_p1_Oy, p_Oy);
        //            gr.DrawLine(p, str_p1_Ox, p_Ox);
        //            gr.DrawLine(p, str_p2_Ox, p_Ox);
        //            for (int o = 0; o < k_Oy; o++)
        //            {
        //                PointF ch_Oy_1 = new PointF(otstup - 3, uroven1 - (prob_Oy + o * prob_Oy));
        //                PointF ch_Oy_2 = new PointF(otstup, uroven1 - (prob_Oy + o * prob_Oy));
        //                PointF t_Oy = new PointF(otstup - 20, uroven1 - (prob_Oy + 8 + o * prob_Oy));
        //                gr.DrawLine(p, ch_Oy_1, ch_Oy_2);
        //                gr.DrawString((prob_Oy + o * prob_Oy).ToString(), font1, s_osi, t_Oy);
        //            }

        //            int koll = 0;
        //            for (int i = 0; i < wells[ww].plasty[pp].gods.Count; i++)
        //            {
        //                for (int j = 0; j < wells[ww].plasty[pp].gods[i].month.Count; j++)
        //                {
        //                    float r_oil_p1X = otstup * 2 + koll * (shirina + 2);
        //                    float r_oil_p1Y = uroven1 - float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j]);
        //                    float h = float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j]);
        //                    if (wells[ww].plasty[pp].gods[i].oilrate[j] == "")
        //                    {
        //                        h = 0;
        //                        r_oil_p1Y = uroven1;
        //                    }
        //                    gr.FillRectangle(s_oil, r_oil_p1X, r_oil_p1Y, shirina, h);
        //                    float r_w_p1Y = uroven1 - float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]);
        //                    float hw = float.Parse(wells[ww].plasty[pp].gods[i].liquidrate[j]) - float.Parse(wells[ww].plasty[pp].gods[i].oilrate[j]);
        //                    gr.FillRectangle(Brushes.Blue, r_oil_p1X, r_w_p1Y, shirina, hw);
        //                    PointF ch_Ox_1 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1);
        //                    PointF ch_Ox_2 = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2), uroven1 + 3);
        //                    PointF t_Ox = new PointF(otstup * 2 + shirina / 2 + koll * (shirina + 2) - 4, uroven1 + 3);
        //                    gr.DrawLine(p, ch_Ox_1, ch_Ox_2);
        //                    gr.DrawString(wells[ww].plasty[pp].gods[i].month[j], font1, s_osi, t_Ox);
        //                    koll++;

        //                }

        //                koll++;
        //            }
        //            int oh = wells[ww].plasty[pp].gods[0].month.Count / 2;
        //            PointF t_Ox_g = new PointF(otstup + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
        //            gr.DrawString(wells[ww].plasty[pp].gods[0].year.ToString(), font1, s_osi, t_Ox_g);
        //            for (int i = 1; i < wells[ww].plasty[pp].gods.Count; i++)
        //            {
        //                oh = oh + wells[ww].plasty[pp].gods[i - 1].month.Count / 2 + wells[ww].plasty[pp].gods[i].month.Count / 2;
        //                PointF t_Ox_g2 = new PointF(otstup * (i + 1) + shirina / 2 + oh * (shirina + 2), uroven1 + 15);
        //                gr.DrawString(wells[ww].plasty[pp].gods[i].year.ToString(), font1, s_osi, t_Ox_g2);
        //            }


        //            PointF t_g = new PointF(otstup * 2 + koll * (shirina + 2), uroven1 + 15);
        //            PointF t_m = new PointF(otstup * 2 + koll * (shirina + 2), uroven1 + 3);
        //            PointF t_d = new PointF(otstup - 20, uroven1 - otstup - max_sum_rate - 15);
        //            PointF t_p1 = new PointF(otstup, uroven1 - otstup - max_sum_rate - 30);
        //            PointF t_p2 = new PointF(otstup, uroven1 - otstup - max_sum_rate - 45);
        //            PointF t_p3 = new PointF(otstup, uroven1 - otstup - max_sum_rate - 60);
        //            gr.DrawString("Год", font1, s_osi, t_g);
        //            gr.DrawString("Месяц", font1, s_osi, t_m);
        //            gr.DrawString("Дебит, т/сут", font1, s_osi, t_d);

        //            Font font2 = new System.Drawing.Font("Times New Roman", 9, FontStyle.Bold);
        //            gr.DrawString("Месторождение " + wells[ww].wellfield, font2, s_osi, t_p2);
        //            gr.DrawString("Пласт " + wells[ww].plasty[pp].name, font2, s_osi, t_p3);
        //            gr.DrawString("Скважина " + wells[ww].wellname, font2, s_osi, t_p1);

        //            string w = " - Дебит воды";
        //            string oi = " - Дебит нефти";
        //            gr.FillRectangle(Brushes.Red, otstup * 2, uroven1 + 30, 15, 8);
        //            PointF t_o = new PointF(otstup * 2 + 15, uroven1 + 27);
        //            gr.DrawString(oi, font1, s_osi, t_o);
        //            gr.FillRectangle(Brushes.Blue, otstup * 2 + 15 + oi.Length * 4 + 10, uroven1 + 30, 15, 8);
        //            PointF t_w = new PointF(otstup * 2 + 15 + oi.Length * 4 + 25, uroven1 + 27);
        //            gr.DrawString(w, font1, s_osi, t_w);
        //            string[] t = Filename1.Split('\\');
        //            string s = "";
        //            for (int l = 0; l < t.Length - 1; l++)
        //            {
        //                s = s + t[l] + "\\";
        //            }

        //            s = s + wells[ww].plasty[pp].name + "_" + wells[ww].wellfield + "_" + wells[ww].wellname + "_дебит"+".png";

        //            //this.pictureBox1.DrawToBitmap(bmp, this.pictureBox1.Bounds);
        //            // this.pictureBox1.Image = bmp;
        //            bmp.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
        //            //pictureBox1.Image.Save(@s, System.Drawing.Imaging.ImageFormat.Png);
        //            gr.Dispose();
        //            //bmp.Dispose();
        //        }
        //    }         
    }
}

