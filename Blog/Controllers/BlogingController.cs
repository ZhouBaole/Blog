using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using System.Net.Mail;
using System.Net;
using Flower.code;

namespace Blog.Controllers
{
    public class BlogingController : Controller
    {
        // GET: Bloging
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (BlognEntities blo = new BlognEntities())
            {
                List<Phabum> ji = blo.Phabum.ToList();
                ViewData["photo"] = ji;
                var text = blo.Essay.ToList();
                ViewData["text"] = text;
                //相册加图片
                List<photoimg> photoimgs = new List<photoimg>();
                foreach (var item in ji)
                {
                    photoimg p = new photoimg();
                    int id = item.phid;
                    List<Photograph> imgs = blo.Photograph.Where(a => a.phid == id).ToList();
                    p.phabums = item;
                    p.pimg = imgs;
                    photoimgs.Add(p);
                }
                ViewData["photoimgd"] = photoimgs;
            }
            return View();
        }
        /// <summary>
        /// 相册
        /// </summary>
        /// <returns></returns>
        public ActionResult share()
        {
            using (BlognEntities blog = new BlognEntities())
            {
                var pum = blog.Phabum.ToList();

                List<photoimg> photo = new List<photoimg>();
                foreach (var item in pum)
                {
                    photoimg pimg = new photoimg();
                    var id = item.phid;
                    var img = blog.Photograph.Where(a => a.phid == id).ToList();
                    pimg.phabums = item;
                    pimg.pimg = img;
                    photo.Add(pimg);
                }
                ViewData["pumimg"] = photo;
            }
            return View();
        }
        /// <summary>
        /// 相片
        /// </summary>
        /// <returns></returns>
        public ActionResult shares(string id)
        {
            if (id == "" || id == null)
            {
                return Redirect("/Bloging/share");
            }
            else
            {
                using (BlognEntities blo = new BlognEntities())
                {
                    int idd = int.Parse(id);
                    photoimg pimg = new photoimg();
                    var pum = blo.Phabum.Where(a => a.phid == idd).FirstOrDefault();
                    if (pum == null)
                    {
                        return Redirect("/Bloging/share");
                    }
                    var pimgs = blo.Photograph.Where(a => a.phid == idd).ToList();

                    pimg.phabums = pum;
                    pimg.pimg = pimgs;
                    ViewData["pimgd"] = pimg;
                    //相册评论
                    List<imgsc> imgus = new List<imgsc>();
                    var photocs = blo.Photocuss.Where(a => a.phid == idd).ToList();
                    foreach (var item in photocs)
                    {
                        imgsc imgs = new imgsc();
                        int pidc = item.usid;
                        var us = blo.Usersd.Where(a => a.usid == pidc).FirstOrDefault();
                        imgs.ph = item;
                        imgs.us = us;
                        imgus.Add(imgs);

                    }
                    ViewData["pucimg"] = imgus;

                }
            }
            return View();
        }
        /// <summary>
        /// 相册点赞
        /// </summary>
        /// <returns></returns>
        public ActionResult photoimgs(string id)
        {
            if (id == "" || id == null)
            {
                return Json(new { data = "没有相应的编号", type = 0 }, JsonRequestBehavior.AllowGet);
            }
            var name = Session["name"];
            if (name == null || name.ToString() == "")
            {
                return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
            }
            using (BlognEntities blo = new BlognEntities())
            {
                int idd = int.Parse(id);
                var pum = blo.Phabum.Where(a => a.phid == idd).FirstOrDefault();
                if (pum == null)
                {
                    return Json(new { data = "没有相应的编号", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                var user = blo.Usersd.Where(a => a.usname == name.ToString()).FirstOrDefault();
                int naid = user.usid;
                //判断用户是否点赞
                int uslike = blo.Phlike.Where(a => a.usid == naid && a.phid == idd).ToList().Count();
                if (uslike >= 1)
                {
                    return Json(new { data = "您已经点过赞喽", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                //添加相册点赞
                Phlike phk = new Phlike();
                phk.usid = naid;
                phk.phid = idd;
                phk.pldate = DateTime.Now;
                blo.Phlike.Add(phk);
                //修改相册点击量
                pum.phliang = pum.phliang + 1;
                blo.SaveChanges();
                return Json(new { data = "点赞成功", type = 1, liang = pum.phliang }, JsonRequestBehavior.AllowGet);
            }

        }
        //文章评论点赞
        public ActionResult plikead(string idd)
        {
            try
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                }

                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {
                    int id = int.Parse(idd);
                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;
                    //判断该用户是否点过赞
                    int i = blog.Pclike.Where(a => a.usid == uid && a.pcid == id).ToList().Count();
                    if (i > 0)
                    {
                        return Json(new { data = "您已经点过赞了", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //评论
                    Photocuss text = blog.Photocuss.Where(a => a.pcid == id).FirstOrDefault();
                    if (text == null)
                    {
                        return Json(new { data = "评论不存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //点赞
                    var eslike = new Pclike();
                    eslike.usid = user.usid;
                    eslike.pcid = text.pcid;
                    eslike.plcdate = DateTime.Now;
                    blog.Pclike.Add(eslike);

                    text.pcliang = text.pcliang + 1;
                    blog.SaveChanges();

                    return Json(new { data = "谢谢客官", type = 1, liang = text.pcliang }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                return Json(new { data = "点赞失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 关于我
        /// </summary>
        /// <returns></returns>
        public ActionResult about()
        {
            return View();
        }
        /// <summary>
        /// 文章
        /// </summary>
        /// <returns></returns>
        public ActionResult diary(string id)
        {
            using (BlognEntities blo = new BlognEntities())
            {
                List<Phabum> ji = blo.Phabum.ToList();
                ViewData["photo"] = ji;
                var text = blo.Essay.ToList();
                ViewData["text"] = text;

                var texts = new Essay();
                //文章评论
                List<essayus> esyus = new List<essayus>();
                if (id != "" && id != null)
                {
                    int idd = int.Parse(id);
                    texts = blo.Essay.Where(a => a.esid == idd).FirstOrDefault();



                }
                else
                {
                    texts = blo.Essay.Take(1).FirstOrDefault();

                }
                int eid = texts.esid;
                List<Discuss> dis = blo.Discuss.Where(a => a.esid == eid).ToList();
                foreach (var item in dis)
                {
                    essayus esuss = new essayus();
                    esuss.dis = item;
                    int usid = item.usid;
                    var us = blo.Usersd.Where(a => a.usid == usid).FirstOrDefault();
                    esuss.us = us;
                    esyus.Add(esuss);


                }
                ViewData["textx"] = texts;
                //评论
                ViewData["pinglun"] = esyus;


            }
            return View();

        }
        /// <summary>
        /// 留言
        /// </summary>
        /// <returns></returns>
        public ActionResult message()
        {
            using (BlognEntities blog = new BlognEntities())
            {
                List<messeus> mesus = new List<messeus>();
                var mes = blog.Message.ToList();
                foreach (var item in mes)
                {
                    messeus me = new messeus();
                    int uid = item.usid;
                    var us = blog.Usersd.Where(a => a.usid == uid).FirstOrDefault();

                    me.messages = item;
                    me.users = us;
                    mesus.Add(me);

                }
                ViewData["mes"] = mesus;


            }
            return View();
        }
        /// <summary>
        /// 发送邮件验证码
        /// </summary>
        public ActionResult SendEmail1(string mailTo, string mailSubject, string mailContent)
        {
            try
            {
                //验证码
                SecurityCode scode = new SecurityCode();
                string code = scode.CreateRandomCode(5);
                Session["code"] = code;
                SmtpClient mailClient = new SmtpClient("smtp.qq.com");
                mailClient.EnableSsl = true;
                mailClient.UseDefaultCredentials = false;
                //Credentials登陆SMTP服务器的身份验证.
                mailClient.Credentials = new NetworkCredential("zhoubaoles@qq.com", "aiookfzkwekjijif");//邮箱，
                MailMessage message = new MailMessage(new MailAddress("zhoubaoles@qq.com"), new MailAddress(mailTo));//发件人，收件人
                message.IsBodyHtml = true;               
                message.Body = mailContent + ":" + code;//邮件内容
                message.Subject = mailSubject;//邮件主题                                             
                mailClient.Send(message); // 发送邮件
                return Json(new { date = "已发送,请查收", type = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { date = "发送失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 注册
        /// </summary>

        public ActionResult signin()
        {
            try
            {

                //名字
                var name = Request.Form["usname"].ToString();
                //密码
                var pwd = Request.Form["pwd"].ToString();
                //邮箱验证码
                var emialyan = Request.Form["emialyan"].ToString();
                //邮箱
                var emial = Request.Form["emial"].ToString();
                //手机号
                var phone = Request.Form["usphone"].ToString();

                //图片
                HttpPostedFileBase post = Request.Files["file"];
                if (post.FileName == "")
                {
                    return Json(new { data = "请选择头像", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                var code = Session["code"].ToString();
                if (code.ToLower() != emialyan.ToLower())
                {
                    return Json(new { data = "验证码错误", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    using (BlognEntities blo = new BlognEntities())
                    {
                        var uname = blo.Usersd.Where(a => a.usname == name).FirstOrDefault();
                        if (uname != null)
                        {
                            return Json(new { data = "名字已存在请重新输入", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var emal = blo.Usersd.Where(a => a.usemal == emial).FirstOrDefault();
                        if (emal != null)
                        {
                            return Json(new { data = "邮箱已存在，建议进行邮箱登录", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var uphono = blo.Usersd.Where(a => a.usphono == phone).FirstOrDefault();
                        if (uphono != null)
                        {
                            return Json(new { data = "手机号已存在，建议进行手机号登录", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        var hu = name + post.FileName.Substring(post.FileName.LastIndexOf("."));
                        var j = Server.MapPath("~/images/users/") + hu;
                        post.SaveAs(j);
                        Usersd user = new Usersd();
                        user.usname = name;
                        user.usphono = phone;
                        user.uspic = hu;
                        user.usemal = emial;
                        user.uspwd = pwd;
                        blo.Usersd.Add(user);
                        blo.SaveChanges();
                        Session["name"] = name;
                        Session["img"] = hu;
                        Session["phone"] = phone;
                        return Json(new { data = "注册完成", type = 1 }, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            catch (Exception e)
            {

                return Json(new { data = "注册失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 密码登录
        /// </summary>
        public ActionResult Logen()
        {
            try
            {
                using (BlognEntities blo = new BlognEntities())
                {
                    //手机号
                    var phone = Request.Form["logphone"].ToString();
                    //密码
                    var pwd = Request.Form["logpwd"].ToString();

                    var us = blo.Usersd.Where(a => a.usphono == phone).FirstOrDefault();
                    if (us == null)
                    {
                        return Json(new { data = "请先注册", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    us = blo.Usersd.Where(a => a.usphono == phone && a.uspwd == pwd).FirstOrDefault();
                    if (us == null)
                    {
                        return Json(new { data = "密码错误", type = 0 }, JsonRequestBehavior.AllowGet);
                    }

                    Session["name"] = us.usname;
                    Session["img"] = us.uspic;
                    Session["phone"] = us.usphono;
                    return Json(new { data = "登陆成功", type = 1 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                return Json(new { data = "登录失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 邮箱登录
        /// </summary>
        public ActionResult Logeemal()
        {
            try
            {
                using (BlognEntities blo = new BlognEntities())
                {
                    //验证码
                    var codes = Request.Form["logcod"].ToString();
                    var code = Session["code"].ToString();
                    if (code.ToLower() != codes.ToLower())
                    {
                        return Json(new { data = "验证码错误", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //邮箱
                    var logemal = Request.Form["logemal"].ToString();

                    var us = blo.Usersd.Where(a => a.usemal == logemal).FirstOrDefault();
                    if (us == null)
                    {
                        return Json(new { data = "该邮箱还没有注册哦", type = 0 }, JsonRequestBehavior.AllowGet);
                    }


                    Session["name"] = us.usname;
                    Session["img"] = us.uspic;
                    Session["phone"] = us.usphono;
                    return Json(new { data = "登陆成功", type = 1 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                return Json(new { data = "登录失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        public ActionResult remove()
        {
            try
            {

                Session["name"] = null;
                Session["img"] = null;
                Session["phone"] = null;
                return Json(new { data = "已退出", type = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { data = "退出失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 文章点赞
        /// </summary>
        public ActionResult textzanadd(string idd)
        {
            try
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                }

                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {
                    int id = int.Parse(idd);
                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;
                    //判断该用户是否点过赞
                    int i = blog.Eslike.Where(a => a.usid == uid && a.esid == id).ToList().Count();
                    if (i > 0)
                    {
                        return Json(new { data = "您已经点过赞了", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //文章
                    Essay text = blog.Essay.Where(a => a.esid == id).FirstOrDefault();
                    if (text == null)
                    {
                        return Json(new { data = "文章不存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //点赞
                    var eslike = new Eslike();
                    eslike.usid = user.usid;
                    eslike.esid = text.esid;
                    eslike.eldate = DateTime.Now;
                    blog.Eslike.Add(eslike);
                    text.esliang = text.esliang + 1;
                    blog.SaveChanges();
                    return Json(new { data = "谢谢客官", type = 1, liang = text.esliang }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { data = "点赞失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 文章评论点赞
        /// </summary>
        public ActionResult tlikeadd(string idd)
        {
            try
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                }

                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {
                    int id = int.Parse(idd);
                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;
                    //判断该用户是否点过赞
                    int i = blog.Dislike.Where(a => a.usid == uid && a.diid == id).ToList().Count();
                    if (i > 0)
                    {
                        return Json(new { data = "您已经点过赞了", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //评论
                    Discuss text = blog.Discuss.Where(a => a.diid == id).FirstOrDefault();
                    if (text == null)
                    {
                        return Json(new { data = "评论不存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //点赞
                    var eslike = new Dislike();
                    eslike.usid = user.usid;
                    eslike.diid = text.diid;
                    eslike.dldate = DateTime.Now;
                    blog.Dislike.Add(eslike);
                    text.disliang = text.disliang + 1;
                    blog.SaveChanges();
                    return Json(new { data = "谢谢客官", type = 1, liang = text.disliang }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { data = "点赞失败:" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加文章评论
        /// </summary>
        /// <returns></returns>
        public ActionResult Disadd(string idd, string count)
        {
            try
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                }

                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {
                    int id = int.Parse(idd);
                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;
                    //文章
                    Essay text = blog.Essay.Where(a => a.esid == id).FirstOrDefault();
                    if (text == null)
                    {
                        return Json(new { data = "文章不存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //评论
                    Discuss dis = new Discuss();
                    dis.dicount = count;
                    dis.disdate = DateTime.Now;
                    dis.esid = id;
                    dis.usid = uid;
                    blog.Discuss.Add(dis);
                    blog.SaveChanges();
                    return Json(new { data = "已评论", type = 1 }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                return Json(new { data = "评论失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 添加相册评论
        /// </summary>
        /// <returns></returns>
        public ActionResult phcsadd(string idd, string count)
        {
            try
            {
                if (idd == null || idd == "")
                {
                    return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
                }

                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {
                    int id = int.Parse(idd);
                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;
                    //相册
                    Phabum text = blog.Phabum.Where(a => a.phid == id).FirstOrDefault();
                    if (text == null)
                    {
                        return Json(new { data = "相册不存在", type = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    //评论
                    Photocuss dis = new Photocuss();
                    dis.pccount = count;
                    dis.pcdate = DateTime.Now;
                    dis.phid = id;
                    dis.usid = uid;
                    blog.Photocuss.Add(dis);
                    blog.SaveChanges();
                    return Json(new { data = "已评论", type = 1 }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                return Json(new { data = "评论失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 添加留言
        /// </summary>
        /// <returns></returns>
        public ActionResult messageadd(string count)
        {
            try
            {


                if (Session["name"] == null || Session["name"].ToString() == "")
                {
                    return Json(new { data = "请先登录", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                using (BlognEntities blog = new BlognEntities())
                {

                    var name = Session["name"].ToString();
                    var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                    int uid = user.usid;

                    //评论
                    Message dis = new Message();
                    dis.meconter = count;
                    dis.mesdate = DateTime.Now;

                    dis.usid = uid;
                    blog.Message.Add(dis);
                    blog.SaveChanges();
                    return Json(new { data = "已评论", type = 1 }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception e)
            {
                return Json(new { data = "评论失败：" + e.Data, type = 0 }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 个人界面
        /// </summary>
        /// <returns></returns>
        public ActionResult users()
        {
            if (Session["name"] == null || Session["name"].ToString() == "")
            {
                return Redirect("/Bloging/Index");
            }
            using (BlognEntities blog = new BlognEntities())
            {
                var name = Session["name"].ToString();
                var user = blog.Usersd.Where(a => a.usname == name).FirstOrDefault();
                ViewData["userd"] = user;
                return View();
            }

        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        public ActionResult usersd()
        {
            var id = Request.Form["id"];

            if (id == null || id == "")
            {
                return Json(new { data = "编号不能为空", type = 0 }, JsonRequestBehavior.AllowGet);
            }
            using (BlognEntities blog = new BlognEntities())
            {
                //名字
                var name = Request.Form["usname"].ToString();
                //密码
                var pwd = Request.Form["pwd"].ToString();
                //邮箱验证码
                var emialyan = Request.Form["emialyan"].ToString();
                //邮箱
                var emial = Request.Form["emial"].ToString();
                //手机号
                var phone = Request.Form["usphone"].ToString();
                var code = Session["code"].ToString();
                if (code.ToLower() != emialyan.ToLower())
                {
                    return Json(new { data = "验证码错误", type = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var usesr = blog.Usersd.Where(a => a.usemal == emial).FirstOrDefault();
                    int idd = int.Parse(id);
                    var user = blog.Usersd.Where(a => a.usid == idd).FirstOrDefault();
                    if (emial != user.usemal)
                    {
                        if (usesr != null)
                        {
                            return Json(new { data = "此邮箱已经被使用", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    user.usname = name;
                    user.usphono = phone;
                    user.usemal = emial;
                    user.uspwd = pwd;
                    //图片
                    HttpPostedFileBase post = Request.Files["file"];
                    if (post.FileName != "")
                    {
                        //删除当前头像
                        string path = "~/images/users/" + user.uspic;
                        string paths = Server.MapPath(path);
                        bool sta = System.IO.File.Exists(paths);
                        if (sta)
                        {
                            System.IO.File.Delete(paths);

                        }
                        else
                        {
                            return Json(new { data = "头像删除失败", type = 0 }, JsonRequestBehavior.AllowGet);
                        }
                        //保存当前头像
                        var hu = name + post.FileName.Substring(post.FileName.LastIndexOf("."));
                        var j = Server.MapPath("~/images/users/") + hu;
                        post.SaveAs(j);
                        user.uspic = hu;
                    }
                    blog.SaveChanges();
                    Session["name"] = user.usname;
                    Session["img"] = user.uspic;
                    Session["phone"] = user.usphono;
                    return Json(new { data = "修改成功", type = 1 }, JsonRequestBehavior.AllowGet);
                }


            }

        }
    }
}