using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WeiMsgLib.Message;
using WeiMsgLib.Exceptions;

namespace WeiMsgLib.Utility
{
    public static class EntityHelper
    {

        //new()表示T必须是一个具有无参构造器的类型，即可以用new关键字实例化（var t = new T()）,加了this编译器可以根据entity的类型推断T的具体类型,若在static class中，则表示为扩展方法
        public static void FillEntityWithXml<T>(this T entity, XDocument xml) where T : class, new()
        {
            entity = entity ?? new T();
            var root = xml.Root;
            //反射：取得entity的所有属性
            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                var element = root.Element(propName);
                if (element != null)
                {
                    switch (prop.PropertyType.Name)
                    {
                        //基本类型
                        case "DateTime":
                            //为entity的prop复制
                            prop.SetValue(entity, DateTimeHelper.GetDateTimeFromXml(element.Value), null);
                            break;
                        case "Int32":
                            prop.SetValue(entity, int.Parse(element.Value), null);
                            break;
                        case "Int64":
                            prop.SetValue(entity, long.Parse(element.Value), null);
                            break;
                        case "Double":
                            prop.SetValue(entity, double.Parse(element.Value), null);
                            break;
                        
                        //枚举类型
                        case "RequestMsgType":
                        case "ResponseMsgType":
                        case "Event":
                            break;

                        //自定义类型,List<Article>中泛型参数Article是自定义类型
                        case "List`1"://List<T>类型,与ResponseNewsMsg中的Ariticles匹配
                            var genericArguments = prop.PropertyType.GetGenericArguments();
                            if (genericArguments[0].Name == "Article")
                            {
                                var articles = new List<Article>();
                                foreach (var item in element.Elements("item"))
                                {
                                    var article = new Article();
                                    //递归，用XML中的子节点映射子Entity
                                    FillEntityWithXml(article, new XDocument(item));
                                }
                                prop.SetValue(entity, articles, null);
                            }
                            break;
                        case "Music": //ResponseMusicMsg中
                            var music = new Music();
                            FillEntityWithXml(music, new XDocument(element));
                            prop.SetValue(entity, music, null);
                            break;
                        case "Image": //ResponseImageMsg
                            var image = new Image();
                            FillEntityWithXml(image, new XDocument(element));
                            prop.SetValue(entity, image, null);
                            break;
                        case "Voice": //ResponseVoiceMsg
                            var voice = new Voice();
                            FillEntityWithXml(voice, new XDocument(element));
                            break;
                        case "Video": //ResponseVideoMsg
                            var video = new Video();
                            FillEntityWithXml(video, new XDocument(element));
                            prop.SetValue(entity, video, null);
                            break;
                        default://string类型,不需要转换
                            prop.SetValue(entity, element.Value, null);
                            break;
                    }
                }
            }
        }

        public static XDocument ConvertEntityToXml<T>(this T entity) where T : class, new()
        {
            entity = entity ?? new T();
            var doc = new XDocument();
            doc.Add(new XElement("xml"));
            var root = doc.Root;
            /*
             * 微信对字段排序有严格要求
            */
            var propNameOrder = new List<string>() { "ToUserName", "FromUserName", "CreateTime", "MsgType" };
            //不同的返回类型需要对应不同的特殊格式进行排序
            if (entity is ResponseNewsMsg)
            {
                propNameOrder.AddRange(new[] { "ArticleCount", "Articles",/*以下是Atricle属性*/ "Title ", "Description ", "PicUrl", "Url" });
            }
            else if (entity is ResponseMusicMsg)
            {
                propNameOrder.AddRange(new[] { "Music",/*以下是Music属性*/ "Title ", "Description ", "MusicUrl", "HQMusicUrl" });
            }
            else if (entity is ResponseImageMsg)
            {
                propNameOrder.AddRange(new[] { "Image",/*以下是Image属性*/ "MediaId " });
            }
            else if (entity is ResponseVoiceMsg)
            {
                propNameOrder.AddRange(new[] { "Voice",/*以下是Voice属性*/ "MediaId " });
            }
            else if (entity is ResponseVideoMsg)
            {
                propNameOrder.AddRange(new[] { "Video",/*以下是Video属性*/ "MediaId ", "Title", "Description" });
            }
            else
            {
                //如Text类型
                propNameOrder.AddRange(new[] { "Content" });
            }
            //委托类型（变相实现函数是一等公民的效果），获取索引值
            Func<string, int> orderByPropName = propNameOrder.IndexOf;
            //根据索引值排序（保证XML中的顺序跟proNameOrder中的顺序一致）
            var props = entity.GetType().GetProperties().OrderBy(p => orderByPropName(p.Name)).ToList();
            foreach (var prop in props)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(entity, null);
                switch (propName)
                {
                    case "Articles":
                        var articlesElement = new XElement("Articles");
                        var articles = propValue as List<Article>;
                        //递归，将每个Article实例转化为XML节点并添加到articlesElement（取Root.Elements是因为要去掉“xml”标签）
                        articles.ForEach(a => articlesElement.Add(new XElement("item", ConvertEntityToXml(a).Root.Elements())));
                        root.Add(articlesElement);
                        break;
                    case "Music":
                    case "Image":
                    case "Video":
                    case "Voice":
                        var mediaElement = new XElement(propName);
                        var media = propValue;
                        var subNodes = ConvertEntityToXml(media).Root.Elements();
                        mediaElement.Add(subNodes);
                        root.Add(mediaElement);
                        break;
                    default:
                        switch (prop.PropertyType.Name)
                        {
                            case "String":
                                root.Add(new XElement(propName, new XCData(propValue as string ?? "")));
                                break;
                            case "DateTime":
                                root.Add(new XElement(propName, DateTimeHelper.GetWeixinDateTime((DateTime)propValue)));
                                break;
                            case "ResponseMsgType":
                                root.Add(new XElement(propName, new XCData(propValue.ToString().ToLower())));
                                break;
                            case "Article":
                                root.Add(new XElement(propName, propValue.ToString().ToLower()));
                                break;
                            default:
                                root.Add(new XElement(propName, propValue));
                                break;
                        }
                        break;
                }
            }

            return doc;
        }
    
        //RequestMsgBase扩展方法
        public static T CreateResponseMsg<T>(this IRequestMsgBase requestMsg) where T : ResponseMsgBase
        {
            try
            {
                //根据调用时填入的泛型参数确定T的类型
                var tType = typeof(T);
                //把ResponseMsg类型名转化为Enum中的ResponseMsgType:去掉“Response”和“Msg”（如:ResponseImageMsg -> Image）
                var responseName = tType.Name.Replace("Response", "").Replace("Msg", "");
                var msgType = (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), responseName);
                return CreateFromRequestMsg(requestMsg, msgType) as T;
            }
            catch (Exception e)
            {
                throw new WeixinException("Fail: RequestMsgBase.CreateResponseMsg<T>过程发生异常", e);
            } 
        }

        private static IResponseMsgBase CreateFromRequestMsg(IRequestMsgBase requestMsg, ResponseMsgType msgType)
        {
            ResponseMsgBase responseMsg = null;
            try
            {
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMsg = new ResponseTextMsg();
                        break;
                    case ResponseMsgType.News:
                        responseMsg = new ResponseNewsMsg();
                        break;
                    case ResponseMsgType.Music:
                        responseMsg = new ResponseMusicMsg();
                        break;
                    case ResponseMsgType.Image:
                        responseMsg = new ResponseImageMsg();
                        break;
                    case ResponseMsgType.Voice:
                        responseMsg = new ResponseVoiceMsg();
                        break;
                    case ResponseMsgType.Video:
                        responseMsg = new ResponseVideoMsg();
                        break;
                    default:
                        throw new UnknowRequestMsgTypeException(string.Format("缺少{0}类型的处理程序.", msgType), new ArgumentOutOfRangeException());
                }
                responseMsg.ToUserName = requestMsg.FromUserName;
                responseMsg.FromUserName = requestMsg.ToUserName;
                responseMsg.CreateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                throw new WeixinException("CreateFromRequestMsg过程发生异常", e);
            }
            return responseMsg;
        }

    }
}