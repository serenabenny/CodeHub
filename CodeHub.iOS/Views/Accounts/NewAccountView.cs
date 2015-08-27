using CoreGraphics;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using CodeHub.Core.ViewModels.Accounts;
using MonoTouch;
using UIKit;

namespace CodeHub.iOS.Views.Accounts
{
    public partial class NewAccountView : MvxViewController
    {
        public NewAccountView()
			: base ("NewAccountView", null)
        {
            Title = "Account";
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.BackButton, UIBarButtonItemStyle.Plain,(s, e) => NavigationController.PopViewController(true));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			View.BackgroundColor = UIColor.FromRGB(239, 239, 244);
			
            InternetButton.SetBackgroundImage(Images.Buttons.GreyButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);
            EnterpriseButton.SetBackgroundImage(Images.Buttons.BlackButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);

            InternetButton.Layer.ShadowColor = UIColor.Black.CGColor;
            InternetButton.Layer.ShadowOffset = new CGSize(0, 1);
            InternetButton.Layer.ShadowOpacity = 0.3f;

            EnterpriseButton.Layer.ShadowColor = UIColor.Black.CGColor;
            EnterpriseButton.Layer.ShadowOffset = new CGSize(0, 1);
            EnterpriseButton.Layer.ShadowOpacity = 0.3f;

            var set = this.CreateBindingSet<NewAccountView, NewAccountViewModel>();
            set.Bind(EnterpriseButton).To(x => x.GoToEnterpriseLoginCommand);
            set.Bind(InternetButton).To(x => x.GoToDotComLoginCommand);
            set.Apply();

            ScrollView.ContentSize = new CGSize(View.Bounds.Width, EnterpriseButton.Frame.Bottom + 10f);
        }
    }
}

