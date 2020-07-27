
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using System.IO;

namespace Blog.Controllers
{
    public class BlogbackController : Controller
    {
        // GET: Blogback
        //首页
        public ActionResult Index()
        {
            return View();
        }
        //相册
        public ActionResult photo(string str)
        {
            using (BlognEntities blog = new BlognEntities())
            {

                var photo = blog.Phabum.ToList();
                if (str != "" && str != null)
                {
                    photo = photo.Where(a => a.phtitle.Contains(str)).ToList();
                    if (photo.Count() == 0)
                    {
                        photo = blog.Phabum.ToList();
                    }
                }

                ViewData["Phabum"] = photo;
            }
            return View();
        }
        //相册布局页
        public ActionResult photos(string str,string fen = "1")
        {
            using (BlognEntities blog = new BlognEntities())
            {

                var photo = blog.Phabum.ToList();
                if (str != "" && str != null)
                {
                    photo = photo.Where(a => a.phtitle.Contains(str)).ToList();
                    if (photo.Count() == 0)
                    {
                        photo = blog.Phabum.ToList();
                    }
                }

                int t = photo.Count();
                int p = int.Parse(fen);
                if (t > 0)
                {
                    if (p <= 1)
                    {
                        photo = photo.Take(8).ToList();
                    }
                    else
                    {
                        photo = photo.Skip((p - 1) * 8).Take(8).ToList();
                    }

                }
                ViewData["Phabum"] = photo;
            }
            return View();
        }
        //添加相册
        public ActionResult photoadd(string title, string back)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (title == "" || back == "")
                    {
                        return Json(new { date = "请填写完整", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    int count = blog.Phabum.Where(a => a.phtitle == title).ToList().Count();
                    if (count>0)
                    {
                        return Json(new { date = "相册已经存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //创建文件夹
                    string filePhysicalPath = Server.MapPath("~/images/"+title+"/");
                    if (!Directory.Exists(filePhysicalPath))//判断上传文件夹是否存在，若不存在，则创建
                    {
                        Directory.CreateDirectory(filePhysicalPath);//创建文件夹
                    }
                    Phabum plu = new Phabum();
                    plu.phtitle = title;
                    plu.phback = back;
                    plu.phdate = DateTime.Now;
                    blog.Phabum.Add(plu);
                    blog.SaveChanges();
                    return Json(new { date = "请填写完整", type = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                return Json(new { date = "保存出错，原因是：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //删除相册
        public ActionResult phremoe(string id)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    int idd = int.Parse(id);
                    var photo = blog.Phabum.Where(a => a.phid == idd).FirstOrDefault();
                    //删除图片
                    var photoimg = blog.Photograph.Where(a => a.phid == idd).ToList();
                    if (photoimg.Count() > 0)
                    {
                        foreach (var item in photoimg)
                        {//删除图片
                            blog.Photograph.Remove(item);
                            string path = "~/images/"+photo.phtitle+"/"+item.photos;
                            string paths = Server.MapPath(path);
                            bool sta = System.IO.File.Exists(paths);
                            if (sta)
                            {
                                System.IO.File.Delete(paths);

                            }
                           
                        }
                    }
                    //删除文件夹                    
                    string pathn = Server.MapPath("~/images/" + photo.phtitle);
                    if (Directory.Exists(pathn))//判断上传文件夹是否存在，若不存在，则创建
                    {
                        Directory.Delete(pathn);//创建文件夹
                    }                   
                    //删除相册点赞
                    var phlike = blog.Phlike.Where(a => a.phid == idd).ToList();
                    if (phlike.Count() > 0)
                    {
                        foreach (var item in phlike)
                        {
                            blog.Phlike.Remove(item);
                        }
                    }
                    //删除评论点赞
                    var phc = blog.Photocuss.Where(a => a.phid == idd).ToList();
                    if (phc.Count() > 0)
                    {
                        foreach (var item in phc)
                        {
                            int i = item.pcid;
                            var phcs = blog.Pclike.Where(a => a.plcid == i).ToList();
                            if (phcs.Count() > 0)
                            {
                                foreach (var items in phcs)
                                {
                                    blog.Pclike.Remove(items);
                                }

                            }

                            blog.Photocuss.Remove(item);
                        }
                    }                    
                    blog.Phabum.Remove(photo);
                    blog.SaveChanges();
                    return Json(new { data = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {

                return Json(new { data = "删除失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //相册详情
        public ActionResult photoimg(string id)
        {

            if (id == "" || id == null)
            {
                return Redirect("/Blogback/photo");
            }
            int idd = int.Parse(id);
            using (BlognEntities blog = new BlognEntities())
            {
                var ji = blog.Phabum.Where(a => a.phid == idd).FirstOrDefault();
                if (ji == null)
                {
                    return Redirect("/Blogback/photo");
                }
                else
                {
                    var img = blog.Photograph.Where(a => a.phid == idd).ToList();
                    ViewData["ph"] = ji;
                    ViewData["img"] = img;
                    //评论
                    var ping = blog.Photocuss.Where(a => a.phid == idd).ToList();
                    var imgsc = new List<imgsc>();
                    foreach (var item in ping)
                    {
                        imgsc imgg = new Models.imgsc();
                        int usid = item.usid;
                        imgg.ph = item;
                        var j = blog.Usersd.Where(a => a.usid == usid).FirstOrDefault();
                        imgg.us = j;
                        imgsc.Add(imgg);

                    }
                    ViewData["imgsc"] = imgsc;


                }
            }
            return View();
        }
        //保存图片
        public ActionResult imgadd(HttpPostedFileBase file, string name, string idd)
        {
            try
            {
                if (file == null || name == null || name == "")
                {
                    return Json(new { data = "请填写完整", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (BlognEntities blog = new BlognEntities())
                    {
                        int id = int.Parse(idd);
                        var i = blog.Photograph.Where(a => a.otoname == name&&a.phid==id).Count();
                        if (i > 0)
                        {
                            return Json(new { data = "这个名称已经有了", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var hu = name + file.FileName.Substring(file.FileName.LastIndexOf("."));
                        var kl = blog.Phabum.Where(a => a.phid == id).FirstOrDefault();
                        var j = Server.MapPath("~/images/"+kl.phtitle+"/") + hu;
                        file.SaveAs(j);

                        Photograph pimg = new Photograph();
                        pimg.otoname = name;
                        pimg.phid = id;
                        pimg.otodate = DateTime.Now;
                        pimg.photos =hu;
                        blog.Photograph.Add(pimg);
                        blog.SaveChanges();
                        return  Redirect("/Blogback/photoimg?id="+idd); ;
                    }
                }
            }
            catch (Exception e)
            {

                return Json(new { data = "添加失败:"+e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //删除图片
        public ActionResult imgeremove(string idd)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (idd == null || idd == "")
                    {
                        return Json(new { data = "没有相应的编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int id = int.Parse(idd);
                        var ph = blog.Photograph.Where(a => a.otoid == id).FirstOrDefault();
                        int idf = ph.phid;
                        var phm = blog.Phabum.Where(a => a.phid == idf).FirstOrDefault();
                        string path = "~/images/"+phm.phtitle+"/"+ ph.photos;
                        string paths = Server.MapPath(path);
                        bool sta = System.IO.File.Exists(paths);
                        if (sta)
                        {
                            System.IO.File.Delete(paths);

                        }
                        else
                        {
                            return Json(new { data = "删除失败", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        blog.Photograph.Remove(ph);
                        blog.SaveChanges();
                        return Json(new { data = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {

                return Json(new { data = "删除失败:"+e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
           
        }
        //删除评论
        public ActionResult pcdelete(string idd)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (idd == null || idd == "")
                    {
                        return Json(new { data = "没有相应的编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int id = int.Parse(idd);
                        var ph = blog.Photocuss.Where(a => a.pcid == id).FirstOrDefault();
                       
                        blog.Photocuss.Remove(ph);
                        blog.SaveChanges();
                        return Json(new { data = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {

                return Json(new { data = "删除失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //文章      
        public ActionResult Essay(string title)
        {
            using (BlognEntities blog=new BlognEntities())
            {
               
                var essay = blog.Essay.ToList();
                if (title!=""|| title!=null)
                {
                    essay= blog.Essay.Where(a=>a.estitle.Contains(title)).ToList();
                    if (essay.Count()==0)
                    {
                        essay = blog.Essay.ToList();
                    }
                }
                ViewData["ess"] = essay;
            }
            return View();
        }
        //文章布局页     
        public ActionResult Essaying(string title, string fen = "1")
        {
            using (BlognEntities blog = new BlognEntities())
            {

                var essay = blog.Essay.ToList();
                if (title != "" || title != null)
                {
                    essay = blog.Essay.Where(a => a.estitle.Contains(title)).ToList();
                    if (essay.Count() == 0)
                    {
                        essay = blog.Essay.ToList();
                    }
                }
                int t = essay.Count();
                int p = int.Parse(fen);
                if (t > 0)
                {
                    if (p <= 1)
                    {
                        essay = essay.Take(8).ToList();
                    }
                    else
                    {
                        essay = essay.Skip((p - 1) * 8).Take(8).ToList();
                    }

                }
                ViewData["ess"] = essay;
            }
            return View();
        }
        //添加文章
        [HttpPost]
        public ActionResult eayadd(string title, string titcount, bool ko)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (title == "" || titcount == "")
                    {
                        return Json(new { date = "请填写完整", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    int count = blog.Essay.Where(a => a.estitle == title).ToList().Count();
                    if (count > 0)
                    {
                        return Json(new { date = "文章名已经存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    Essay es = new Essay();
                    es.estitle = title;
                    es.esconter = titcount;
                    es.esdate = DateTime.Now;
                    es.eisno = ko;
                    blog.Essay.Add(es);
                    blog.SaveChanges();
                    return Json(new { date = "已添加", type = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                return Json(new { date = "保存出错，原因是：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //删除
        [HttpPost]
        public ActionResult eaydelete(string id)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (id == "" || id ==null)
                    {
                        return Json(new { date = "请输入编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    int idd = int.Parse(id);

                    int count = blog.Essay.Where(a => a.esid == idd).ToList().Count();
                    if (count == 0)
                    {
                        return Json(new { date = "没有此编号的文章哦", type = 0 }, JsonRequestBehavior.AllowGet);
                    }


                    Essay es = blog.Essay.Where(a => a.esid == idd).FirstOrDefault();
                   
                    blog.Essay.Remove(es);
                    blog.SaveChanges();
                    return Json(new { date = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                return Json(new { date = "保存出错，原因是：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //修改是否推荐
        [HttpPost]
        public ActionResult eayup(bool ko,string id)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (id == "" || id == null)
                    {
                        return Json(new { date = "请输入编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    int idd = int.Parse(id);

                    int count = blog.Essay.Where(a => a.esid == idd).ToList().Count();
                    if (count == 0)
                    {
                        return Json(new { date = "没有此编号的文章哦", type = 0 }, JsonRequestBehavior.AllowGet);
                    }


                    Essay es = blog.Essay.Where(a => a.esid == idd).FirstOrDefault();
                    es.eisno = ko;
                    blog.SaveChanges();
                    return Json(new { date = "已修改", type = 1 ,isno= es.eisno }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                return Json(new { date = "保存出错，原因是：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //文章详情
        public ActionResult Essays(string id)
        {
            using (BlognEntities blog = new BlognEntities())
            {

                if (id == "" || id == null)
                {
                    return Redirect("/Blogback/Essay");
                }
                else
                {
                    int idd = int.Parse(id);
                    var essay = blog.Essay.Where(a => a.esid==idd).FirstOrDefault();
                    if (essay == null)
                    {
                        return Redirect("/Blogback/Essay");
                        
                    }
                    //文章
                    ViewData["ess"] = essay;
                    //评论
                    var dis = blog.Discuss.Where(a => a.esid == essay.esid).ToList();
                    List<essayus> esus = new List<essayus>();
                    foreach (var item in dis)
                    {
                      int  uid = item.usid;
                        var usd = blog.Usersd.Where(a => a.usid == uid).FirstOrDefault();
                        essayus esu = new essayus();
                        esu.dis = item;
                        esu.us = usd;
                        esus.Add(esu);
                    }
                    ViewData["essya"] = esus;
                }


            }
            return View();
        }
        //修改文章
        [HttpPost]
        public ActionResult Essupdata(string title, string titcount, string id)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (id == "" || id == null)
                    {
                        return Json(new { date = "请输入编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    if (title==""|| title==null)
                    {
                        return Json(new { date = "标题不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    if (titcount == "" || titcount == null)
                    {
                        return Json(new { date = "文章内容不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    int idd = int.Parse(id);

                    int count = blog.Essay.Where(a => a.esid == idd).ToList().Count();
                    if (count == 0)
                    {
                        return Json(new { date = "没有此编号的文章哦", type = 0 }, JsonRequestBehavior.AllowGet);
                    }


                    Essay es = blog.Essay.Where(a => a.esid == idd).FirstOrDefault();
                    es.estitle = title;
                    es.esconter = titcount;
                    blog.SaveChanges();
                    return Json(new { date = "已修改", type = 1, isno = es.eisno }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {

                return Json(new { date = "保存出错，原因是：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //删除评论
        [HttpPost]
        public ActionResult esdisde(string idd)
        {
            try
            {
                using (BlognEntities blog = new BlognEntities())
                {
                    if (idd == null || idd == "")
                    {
                        return Json(new { data = "没有相应的编号", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int id = int.Parse(idd);
                        var ph = blog.Discuss.Where(a => a.diid == id).FirstOrDefault();

                        blog.Discuss.Remove(ph);
                        blog.SaveChanges();
                        return Json(new { data = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {

                return Json(new { data = "删除失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        //留言
        public ActionResult message()
        {
            using (BlognEntities blog = new BlognEntities())
            {
                var mess = blog.Message.ToList();
                List<messeus> mu = new List<messeus>();
                foreach (var item in mess)
                {
                    var m = new messeus();
                    m.messages = item;
                    int id = item.usid;
                    var us = blog.Usersd.Where(a => a.usid == id).FirstOrDefault();
                    m.users = us;
                    mu.Add(m);
                }
                ViewData["mess"] = mu;
            }
            return View();
        }
        //留言布局页
        public ActionResult messages(string fen = "1")
        {
            using (BlognEntities blog = new BlognEntities())
            {
                var mess = blog.Message.ToList();
                List<messeus> mu = new List<messeus>();
                foreach (var item in mess)
                {
                    var m = new messeus();
                    m.messages = item;
                    int id = item.usid;
                    var us = blog.Usersd.Where(a => a.usid == id).FirstOrDefault();
                    m.users = us;
                    mu.Add(m);
                }
                int t = mu.Count();
                int p = int.Parse(fen);
                if (t > 0)
                {
                    if (p <= 1)
                    {
                        mu = mu.Take(5).ToList();
                    }
                    else
                    {
                        mu = mu.Skip((p - 1) * 5).Take(5).ToList();
                    }

                }
                ViewData["mess"] = mu;
            }
            return View();
        }
        //用户管理      
        public ActionResult user()
        {
            using (BlognEntities blog = new BlognEntities())
            {

                var us = blog.Usersd.ToList();
                
                ViewData["user"] = us;
            }
            return View();
        }
        //用户管理布局页     
        public ActionResult users(string title, string fen = "1")
        {
            using (BlognEntities blog = new BlognEntities())
            {

                var us = blog.Usersd.ToList();
                if (title != "" || title != null)
                {
                    us = blog.Usersd.Where(a => a.usname.Contains(title)).ToList();
                    if (us.Count() == 0)
                    {
                        us = blog.Usersd.ToList();
                    }
                }
                int t = us.Count();
                int p = int.Parse(fen);
                if (t > 0)
                {
                    if (p <= 1)
                    {
                        us = us.Take(8).ToList();
                    }
                    else
                    {
                        us = us.Skip((p - 1) * 8).Take(8).ToList();
                    }

                }
                ViewData["user"] = us;
            }
            return View();
        }
        //留言删除
        public ActionResult mesdelete(string idd)
        {
            using (BlognEntities blog = new BlognEntities())
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "没有编号", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int id = int.Parse(idd);
                    var ph = blog.Message.Where(a => a.meid == id).FirstOrDefault();
                    blog.Message.Remove(ph);
                    blog.SaveChanges();
                    return Json(new { data = "已删除", type = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
          
        }
    }
}