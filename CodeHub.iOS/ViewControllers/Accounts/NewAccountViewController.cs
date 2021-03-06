using CoreGraphics;
using CodeHub.Core.ViewModels.Accounts;
using UIKit;
using ReactiveUI;

namespace CodeHub.iOS.ViewControllers.Accounts
{
    public class NewAccountViewController : BaseViewController<NewAccountViewModel>
    {
        private readonly UIColor DotComBackgroundColor = UIColor.FromRGB(239, 239, 244);
        private readonly UIColor EnterpriseBackgroundColor = UIColor.FromRGB(50, 50, 50);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var dotComButton = new AccountButton("GitHub.com", Images.Logos.GitHub);
            var enterpriseButton = new AccountButton("Enterprise", Images.Logos.Enterprise);

            dotComButton.BackgroundColor = DotComBackgroundColor;
            dotComButton.Label.TextColor = EnterpriseBackgroundColor;
            dotComButton.ImageView.TintColor = EnterpriseBackgroundColor;

            enterpriseButton.BackgroundColor = EnterpriseBackgroundColor;
            enterpriseButton.Label.TextColor = dotComButton.BackgroundColor;
            enterpriseButton.ImageView.TintColor = dotComButton.BackgroundColor;

            Add(dotComButton);
            Add(enterpriseButton);

            View.ConstrainLayout(() => 
                dotComButton.Frame.Top == View.Frame.Top &&
                dotComButton.Frame.Left == View.Frame.Left &&
                dotComButton.Frame.Right == View.Frame.Right &&
                dotComButton.Frame.Bottom == View.Frame.GetMidY() &&

                enterpriseButton.Frame.Top == dotComButton.Frame.Bottom &&
                enterpriseButton.Frame.Left == View.Frame.Left &&
                enterpriseButton.Frame.Right == View.Frame.Right &&
                enterpriseButton.Frame.Bottom == View.Frame.Bottom);

            this.WhenActivated(d => {
                d(enterpriseButton.GetClickedObservable().InvokeCommand(ViewModel.GoToEnterpriseLoginCommand));
                d(dotComButton.GetClickedObservable().InvokeCommand(ViewModel.GoToDotComLoginCommand));
            });
        }

        private class AccountButton : UIButton
        {
            public UILabel Label { get; private set; }

            public new UIImageView ImageView { get; private set; }

            public AccountButton(string text, UIImage image)
                : base(new CGRect(0, 0, 100, 100))
            {
                Label = new UILabel(new CGRect(0, 0, 100, 100));
                Label.Text = text;
                Label.TextAlignment = UITextAlignment.Center;
                Add(Label);

                ImageView = new UIImageView(new CGRect(0, 0, 100, 100));
                ImageView.Image = image;
                ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                Add(ImageView);

                this.ConstrainLayout(() => 
                    ImageView.Frame.Top == this.Frame.Top + 30f &&
                    ImageView.Frame.Left == this.Frame.Left &&
                    ImageView.Frame.Right == this.Frame.Right &&
                    ImageView.Frame.Bottom == this.Frame.Bottom - 60f &&

                    Label.Frame.Top == ImageView.Frame.Bottom + 10f &&
                    Label.Frame.Left == this.Frame.Left &&
                    Label.Frame.Right == this.Frame.Right &&
                    Label.Frame.Height == 20);
            }
        }
    }
}

