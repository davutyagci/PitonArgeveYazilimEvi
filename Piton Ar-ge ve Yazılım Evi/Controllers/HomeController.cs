using Piton_Ar_ge_ve_Yazılım_Evi.Models;
using Piton_Ar_ge_ve_Yazılım_Evi.ModelsInfo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Piton_Ar_ge_ve_Yazılım_Evi.Controllers
{
    public class HomeController : Controller
    {
        Entities db = new Entities();

        // GET: Home
        public ActionResult Index()
        {
            Session.RemoveAll();
            return View();
        }

        public ActionResult YapilacakIsKaydet(string baslangictarihi, string bitistarihi, string yapilacakis)
        {
            bool result = false;

            char[] ayrac = { '-' };

            string[] parcalar = baslangictarihi.Split(ayrac);
            string gun = parcalar[2].Substring(0, 2);
            string saat = parcalar[2].Substring(2, 5);

            baslangictarihi = parcalar[0] + "-" + parcalar[1] + "-" + gun;

            string[] parcalar2 = bitistarihi.Split(ayrac);
            string gun2 = parcalar2[2].Substring(0, 2);
            string saat2 = parcalar2[2].Substring(2, 5);

            bitistarihi = parcalar2[0] + "-" + parcalar2[1] + "-" + gun2;

            YapilacakIsler yi = new YapilacakIsler();
            yi.Id = Guid.NewGuid();
            yi.BaslangicTarih = Convert.ToDateTime(baslangictarihi + " " + saat);
            yi.BitisTarihi = Convert.ToDateTime(bitistarihi + " " + saat2);

            yi.YapilacakIs = yapilacakis;
            yi.Deleted = 0;

            try
            {
                var a = db.YapilacakIsler.Add(yi);
                db.SaveChanges();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return Json(result);
        }

        public ActionResult YapilacakIslerGunluk()
        {
            var list = db.YapilacakIsler.ToList();

            List<YapilacakIslerInfo> listInfo = new List<YapilacakIslerInfo>();

            foreach (var item in list)
            {
                char[] ayrac = { ' ' };
                string[] parcalar = item.BaslangicTarih.ToString().Split(ayrac);

                if (Convert.ToDateTime(parcalar[0]).ToString("dd/MM/yyyy") == DateTime.Today.ToString("dd/MM/yyyy"))
                {
                    YapilacakIslerInfo yii = new YapilacakIslerInfo();
                    yii.Id = item.Id;
                    yii.BaslangicTarih = Convert.ToDateTime(item.BaslangicTarih).ToString("dd/MM/yyyy HH:mm");
                    yii.BitisTarihi = Convert.ToDateTime(item.BitisTarihi).ToString("dd/MM/yyyy HH:mm");
                    yii.YapilacakIs = item.YapilacakIs;

                    listInfo.Add(yii);
                }
            }

            ViewBag.YapilacakIslerGunlukList = listInfo.OrderBy(x => x.BaslangicTarih);

            return PartialView("YapilacakIslerGunluk");
        }

        public ActionResult YapilacakIslerHaftalik(string hafta, string btn)
        {
            var list = db.YapilacakIsler.ToList();

            List<YapilacakIslerInfo> listInfo = new List<YapilacakIslerInfo>();

            CultureInfo ciCurr = CultureInfo.CurrentCulture;

            DateTime a = new DateTime();
            DateTime b = new DateTime();
            string d = "";

            if (hafta == null && btn == null)
            {
                a = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek);
                b = DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek);

                d = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();
            }
            else
            {
                if (btn == "1")
                {
                    d = (Convert.ToInt32(hafta) - 1).ToString();

                    if (d == "0")
                    {
                        d = "53";
                    }
                }
                else if (btn == "2")
                {
                    d = (Convert.ToInt32(hafta) + 1).ToString();

                    if (d == "54")
                    {
                        d = "1";
                    }
                }

                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime YilinIlkGunu = new DateTime(DateTime.Now.Year, 1, 1);
                int GunDengesi = DayOfWeek.Thursday - YilinIlkGunu.DayOfWeek;
                DateTime IlkPersembe = YilinIlkGunu.AddDays(GunDengesi);
                Calendar Takvim = dfi.Calendar;
                int IlkHafta = Takvim.GetWeekOfYear(IlkPersembe, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                int HaftaNumarasi = Convert.ToInt32(d);

                if (IlkHafta <= 1)
                {
                    HaftaNumarasi -= 1;
                }

                a = IlkPersembe.AddDays(HaftaNumarasi * 7).AddDays(-3);
                b = a.AddDays(7 - (int)a.DayOfWeek);
            }

            foreach (var item in list)
            {
                char[] ayrac = { ' ' };
                string[] parcalar = item.BaslangicTarih.ToString
                    ().Split(ayrac);

                DateTime c = Convert.ToDateTime(Convert.ToDateTime(parcalar[0]).ToString("dd/MM/yyyy"));

                if (c >= a && c <= b)
                {
                    YapilacakIslerInfo yii = new YapilacakIslerInfo();
                    yii.Id = item.Id;
                    yii.BaslangicTarih = Convert.ToDateTime(item.BaslangicTarih).ToString("dd/MM/yyyy HH:mm");
                    yii.BitisTarihi = Convert.ToDateTime(item.BitisTarihi).ToString("dd/MM/yyyy HH:mm");
                    yii.YapilacakIs = item.YapilacakIs;

                    listInfo.Add(yii);
                }
            }

            Session["hafta"] = d;
            ViewBag.YapilacakIslerHaftalikList = listInfo.OrderBy(x => x.BaslangicTarih);

            return PartialView("YapilacakIslerHaftalik");
        }

        public ActionResult YapilacakIslerAylik(string ay, string btn)
        {
            var list = db.YapilacakIsler.ToList();

            List<YapilacakIslerInfo> listInfo = new List<YapilacakIslerInfo>();

            DateTime a = new DateTime();
            DateTime b = new DateTime();
            string t = "";
            string d = "";

            if (ay == null && btn == null)
            {
                a = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                b = a.AddMonths(1).AddDays(-1);

                t = DateTime.Now.ToString("MMMM");
                d = DateTime.Now.ToString("MM");
            }
            else
            {
                int r = Convert.ToInt32(DateTime.Now.ToString("MM"));

                if (btn == "1")
                {
                    a = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(Convert.ToInt32(ay) - r - 1).Month, 1);
                    b = a.AddMonths(1).AddDays(-1);

                    t = DateTime.Now.AddMonths(Convert.ToInt32(ay) - r - 1).ToString("MMMM");
                    d = DateTime.Now.AddMonths(Convert.ToInt32(ay) - r - 1).ToString("MM");
                }
                else if (btn == "2")
                {
                    a = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(Convert.ToInt32(ay) - r + 1).Month, 1);
                    b = a.AddMonths(1).AddDays(-1);

                    t = DateTime.Now.AddMonths(Convert.ToInt32(ay) - r + 1).ToString("MMMM");
                    d = DateTime.Now.AddMonths(Convert.ToInt32(ay) - r + 1).ToString("MM");
                }
            }

            foreach (var item in list)
            {
                char[] ayrac = { ' ' };
                string[] parcalar = item.BaslangicTarih.ToString().Split(ayrac);

                DateTime c = Convert.ToDateTime(Convert.ToDateTime(parcalar[0]).ToString("dd/MM/yyyy"));

                if (c >= a && c <= b)
                {
                    YapilacakIslerInfo yii = new YapilacakIslerInfo();
                    yii.Id = item.Id;
                    yii.BaslangicTarih = Convert.ToDateTime(item.BaslangicTarih).ToString("dd/MM/yyyy HH:mm");
                    yii.BitisTarihi = Convert.ToDateTime(item.BitisTarihi).ToString("dd/MM/yyyy HH:mm");
                    yii.YapilacakIs = item.YapilacakIs;

                    listInfo.Add(yii);
                }
            }

            Session["ayAdi"] = t;
            Session["ay"] = d;
            ViewBag.YapilacakIslerAylikList = listInfo.OrderBy(x => x.BaslangicTarih);

            return PartialView("YapilacakIslerAylik");
        }
    }
}