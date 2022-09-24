using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Services;
using System.Diagnostics;

namespace Solver_Sample1
{
    class Urun
    {
        public string Ad;
        public int Maliyet;
        public int Satis;
        public int MaksimumStok;

        public int Kar
        {
            get { return Satis - Maliyet; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //En fazla 5000 TL üretim yapabiliriz.  
            int UretimButce = 5000;


            //Örnek basit ve anlaşılır olsun diye collection kullanılmadı. 
            //İstenilirse collection türünden nesne kabul eden versiyonlarıda var  
            //Urunlerimiz : Decision
            Urun A_Urun = new Urun { Ad = "A_Ürünü", Maliyet = 5, Satis = 8, MaksimumStok = 1350 };
            Urun B_Urun = new Urun { Ad = "B_Ürünü", Maliyet = 8, Satis = 13, MaksimumStok = 875 };
            Urun C_Urun = new Urun { Ad = "C_Ürünü", Maliyet = 3, Satis = 5, MaksimumStok = 1100 };
            

            Decision DecisionA = new Decision(Domain.IntegerNonnegative, A_Urun.Ad);
            Decision DecisionB = new Decision(Domain.IntegerNonnegative, B_Urun.Ad);
            Decision DecisionC = new Decision(Domain.IntegerNonnegative, C_Urun.Ad);

           
            // Çözücü
            var solver = SolverContext.GetContext();

            //Model Tanımı Decision + Constraint
            var model = solver.CreateModel();

            model.AddDecision(DecisionA);
            model.AddDecision(DecisionB);
            model.AddDecision(DecisionC);

            //Kısıtlamalar, Şartlar
            model.AddConstraint("Butce_Kisitlamasi",
                           A_Urun.Maliyet * DecisionA +
                           B_Urun.Maliyet * DecisionB +
                           C_Urun.Maliyet * DecisionC <= UretimButce);
            //Ürettiğimiz ürünlerin toplam maliyeti üretim bütcemizden kücük veya eşit olmalı 

            model.AddConstraint("A_Urunu_Stoklama_Kapasitemiz", DecisionA < A_Urun.MaksimumStok);
            model.AddConstraint("B_Urunu_Stoklama_Kapasitemiz", DecisionB < B_Urun.MaksimumStok);
            model.AddConstraint("C_Urunu_Stoklama_Kapasitemiz", DecisionC < C_Urun.MaksimumStok);
            //Üreteceğimiz ürünler depomuza sığmalı

            //Girdiler tanımlandı.
            //Şimdi sonuç olarak ne istediğimizi tanımlayalım
            //Üreteceğimiz ürünlerin karlığının en fazla olduğu optimizasyonu istiyoruz
            model.AddGoal("En_iyi_uretim_stok", GoalKind.Maximize,
            (A_Urun.Kar * DecisionA) +
            (B_Urun.Kar * DecisionB) +
            (C_Urun.Kar * DecisionC));


            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Lütfen Bekleyiniz"); 


            // Çöz
            Solution solution = solver.Solve();

            // Get our decisions
            Console.WriteLine("Çözüm Kalitesi : " + solution.Quality.ToString());
            Console.WriteLine("A Ürününden {0} adet ", DecisionA);
            Console.WriteLine("B Ürününden {0} adet ", DecisionB);
            Console.WriteLine("C Ürününden {0} adet ", DecisionC);

            double gerceklesen_toplam_maliyet =
            DecisionA.ToDouble() * A_Urun.Maliyet +
            DecisionB.ToDouble() * B_Urun.Maliyet +
            DecisionC.ToDouble() * C_Urun.Maliyet;
            
            double toplam_satis_fiyat =
            DecisionA.ToDouble() * A_Urun.Satis +
            DecisionB.ToDouble() * B_Urun.Satis +
            DecisionC.ToDouble() * C_Urun.Satis;


            Console.WriteLine("Üretim: Harcanan : {0} Planlanan: {1}", gerceklesen_toplam_maliyet, UretimButce);
            Console.WriteLine("Satış Değeri : {0} Kar: {1}", toplam_satis_fiyat, toplam_satis_fiyat - gerceklesen_toplam_maliyet);
            Console.WriteLine("Optimizasyon {0} milisaniyede tamamlandı" , sw.ElapsedMilliseconds);
            Console.ReadLine();



        }

    }
}
