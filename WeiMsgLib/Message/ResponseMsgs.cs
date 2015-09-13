using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiMsgLib.Message
{
    public class ResponseTextMsg : ResponseMsgBase, IResponseMsgBase
    {
        //new 关键字声明一个同名属性并隐藏基类中的属性（与override不同，override是改写或扩展基类属性）
        new public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Text;
            }
        }

        public string Content { get; set; }
    }

    public class ResponseImageMsg : ResponseMsgBase, IResponseMsgBase
    {
        public ResponseImageMsg()
        {
            Image = new Image();
        }

        new public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Image;
            }
        }

        public Image Image { get; set; }
    }
    public class Image
    {
        public string MediaId { get; set; }
    }

    public class ResponseVoiceMsg : ResponseMsgBase, IResponseMsgBase
    {
        public ResponseVoiceMsg()
        {
            Voice = new Voice();
        }

        new public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Voice;
            }
        }

        public Voice Voice { get; set; }
    }
    public class Voice
    {
        public string MediaId { get; set; }
    }

    public class ResponseVideoMsg : ResponseMsgBase, IResponseMsgBase
    {
        public ResponseVideoMsg()
        {
            Video = new Video();
        }

        new public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Video;
            }
        }

        public Video Video { get; set; }
    }
    public class Video
    {
        public string MediaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ResponseMusicMsg : ResponseMsgBase, IResponseMsgBase
    {
        public ResponseMusicMsg()
        {
            Music = new Music();
        }

        public override ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.Music;
            }
        }

        public Music Music { get; set; }
    }
    public class Music
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string MusicUrl { get; set; }
        public string HQMusicUrl { get; set; }
        //缩略图的媒体Id
        public string ThumbMediaId { get; set; }
    }

    public class ResponseNewsMsg : ResponseMsgBase, IResponseMsgBase
    {
        public ResponseNewsMsg()
        {
            Articles = new List<Article>();
        }

        new public virtual ResponseMsgType MsgType
        {
            get
            {
                return ResponseMsgType.News;
            }
        }

        public int ArticleCount
        {
            get
            {
                return (Articles ?? new List<Article>()).Count;
            }
            set
            {
                //此处开放set是为了逆向从Response的XML转成实体的操作一致性，并没有实际意义
            }
        }

        //文章列表，微信客户端只能输出前10条（未来可能随接口变化）
        public List<Article> Articles { get; set; }
    }
    public class Article
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Url { get; set; }
    }
}