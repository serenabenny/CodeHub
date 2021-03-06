using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace CodeHub.iOS.DialogElements
{
    public class HtmlElement : Element, IElementSizing, IDisposable
    {
        private readonly ISubject<string> _urlSubject = new Subject<string>();
        protected UIWebView WebView;
        private nfloat _height;
        protected readonly NSString Key;

        public Action<nfloat> HeightChanged;

        public IObservable<string> UrlRequested
        {
            get { return _urlSubject.AsObservable(); }
        }

        public nfloat Height
        {
            get { return _height; }
        }

        public string ContentPath
        {
            set
            {
                if (value == null)
                {
                    WebView.LoadHtmlString(string.Empty, NSBundle.MainBundle.BundleUrl);
                }
                else
                {
                    WebView.LoadRequest(new NSUrlRequest(NSUrl.FromFilename(value)));
                }
            }
        }

        public string Value
        {
            set
            {
                if (value == null)
                {
                    WebView.LoadHtmlString(string.Empty, NSBundle.MainBundle.BundleUrl);
                }
                else
                {
                    WebView.LoadHtmlString(value, NSBundle.MainBundle.BundleUrl);
                }
            }
        }

        private bool ShouldStartLoad (NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (!request.Url.AbsoluteString.StartsWith("file://"))
            {
                if (UrlRequested != null)
                    _urlSubject.OnNext(request.Url.AbsoluteString);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            WebView.RemoveFromSuperview();
            WebView.Dispose();
            WebView = null;
        }

        public HtmlElement (string cellKey) 
        {
            Key = new NSString(cellKey);

            _height = 10f;

            WebView = new UIWebView();
            WebView.Frame = new CGRect(0, 0, 320f, 44f);
            WebView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            WebView.ScrollView.ScrollEnabled = false;
            WebView.ScalesPageToFit = true;
            WebView.ScrollView.Bounces = false;
            WebView.ShouldStartLoad = (w, r, n) => ShouldStartLoad(r, n);

            WebView.LoadFinished += (sender, e) => CheckHeight();

            HeightChanged = (x) => Reload();
        }

        public void CheckHeight()
        {
            var f = WebView.Frame;
            f.Height = 1;
            WebView.Frame = f;
            f.Height = _height = int.Parse(WebView.EvaluateJavascript("document.body.scrollHeight;"));
            WebView.Frame = f;

            if (HeightChanged != null)
                HeightChanged(_height);
        }

        private void Reload()
        {
            var root = this.GetRootElement();
            if (root != null)
                root.Reload(this);
        }

        public nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return _height;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell (Key);
            if (cell == null){
                cell = new UITableViewCell (UITableViewCellStyle.Default, Key);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }  

            WebView.Frame = new CGRect(0, 0, cell.ContentView.Frame.Width, cell.ContentView.Frame.Height);
            WebView.RemoveFromSuperview();
            cell.ContentView.AddSubview (WebView);
            cell.ContentView.Layer.MasksToBounds = true;
            cell.ContentView.AutosizesSubviews = true;
            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            return cell;
        }
    }
}

