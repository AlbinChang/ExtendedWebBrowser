using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MyWebBrowser
{

    public class ExtendedWebBrowser : System.Windows.Forms.WebBrowser
    {
        //将一个 ActiveX 控件连接到处理该控件的事件的客户端
        System.Windows.Forms.AxHost.ConnectionPointCookie cookie;
        //This class will capture events from the WebBrowser
        WebBrowserExtendedEvents events;

        //This method will be called to give you a chance to create your own event sink
        protected override void CreateSink()
        {
            //MAKE SURE TO CALL THE BASE or the normal events won't fire
            //使基础 ActiveX 控件与可以处理控件事件的客户端相关联
            base.CreateSink();
            events = new WebBrowserExtendedEvents(this);
            //将ActiveX控件与事件相关联
            cookie = new System.Windows.Forms.AxHost.ConnectionPointCookie(this.ActiveXInstance, events, typeof(DWebBrowserEvents2));
        }

        protected override void DetachSink()
        {
            if (null != cookie)
            {
                cookie.Disconnect();
                cookie = null;
            }
            //从基础 ActiveX 控件中释放附加在 System.Windows.Forms.WebBrowser.CreateSink() 方法中的事件处理客户端。
            base.DetachSink();
        }

        //This new event will fire when the page is navigating
        public event EventHandler<WebBrowserExtendedNavigatingEventArgs> BeforeNavigate;
        public event EventHandler<WebBrowserExtendedNavigatingEventArgs> BeforeNewWindow;

        protected void OnBeforeNewWindow(string url, out bool cancel)
        {
            EventHandler<WebBrowserExtendedNavigatingEventArgs> h = BeforeNewWindow;
            WebBrowserExtendedNavigatingEventArgs args = new WebBrowserExtendedNavigatingEventArgs(url, null);
            if (null != h)
            {
                //处罚事件
                h(this, args);
            }
            cancel = args.Cancel;
        }

        protected void OnBeforeNavigate(string url, string frame, out bool cancel)
        {
            EventHandler<WebBrowserExtendedNavigatingEventArgs> h = BeforeNavigate;
            WebBrowserExtendedNavigatingEventArgs args = new WebBrowserExtendedNavigatingEventArgs(url, frame);
            if (null != h)
            {
                //触发事件
                h(this, args);
            }
            //Pass the cancellation chosen back out to the events

            cancel = args.Cancel;
        }

        //This class will capture events from the WebBrowser
        //用标准 OLE STA 封送拆收器替换标准公共语言运行时 (CLR) 自由线程封送拆收器。
        class WebBrowserExtendedEvents : System.Runtime.InteropServices.StandardOleMarshalObject, DWebBrowserEvents2
        {
            //ActiveX控件实例
            ExtendedWebBrowser _Browser;
            //用一个ActiveX控件实例来初始化
            public WebBrowserExtendedEvents(ExtendedWebBrowser browser) { _Browser = browser; }

            //Implement whichever events you wish
            public void BeforeNavigate2(object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
            {
                //传递事件参数给ActiveX控件实例，让其触发事件，调用事件处理函数
                _Browser.OnBeforeNavigate((string)URL, (string)targetFrameName, out cancel);
            }

            public void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref object URLContext, ref object URL)
            {
                //传递事件参数给ActiveX控件实例，让其触发事件，调用事件处理函数
                _Browser.OnBeforeNewWindow((string)URL, out cancel);
            }

        }
        [System.Runtime.InteropServices.ComImport(), System.Runtime.InteropServices.Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
        System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIDispatch),
        System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FHidden)]
        public interface DWebBrowserEvents2
        {

            [System.Runtime.InteropServices.DispId(250)]
            void BeforeNavigate2(
                [System.Runtime.InteropServices.In,
                System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.IDispatch)] object pDisp,
                [System.Runtime.InteropServices.In] ref object URL,
                [System.Runtime.InteropServices.In] ref object flags,
                [System.Runtime.InteropServices.In] ref object targetFrameName, [System.Runtime.InteropServices.In] ref object postData,
                [System.Runtime.InteropServices.In] ref object headers,
                [System.Runtime.InteropServices.In,
                System.Runtime.InteropServices.Out] ref bool cancel);
            [System.Runtime.InteropServices.DispId(273)]
            void NewWindow3(
                [System.Runtime.InteropServices.In,
                System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.IDispatch)] object pDisp,
                [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] ref bool cancel,
                [System.Runtime.InteropServices.In] ref object flags,
                [System.Runtime.InteropServices.In] ref object URLContext,
                [System.Runtime.InteropServices.In] ref object URL);

        }
    }

    public class WebBrowserExtendedNavigatingEventArgs : System.ComponentModel.CancelEventArgs
    {
        private string _Url;
        public string Url
        {
            get { return _Url; }
        }

        private string _Frame;
        public string Frame
        {
            get { return _Frame; }
        }

        public WebBrowserExtendedNavigatingEventArgs(string url, string frame)
            : base()
        {
            _Url = url;
            _Frame = frame;
        }
    }
}
