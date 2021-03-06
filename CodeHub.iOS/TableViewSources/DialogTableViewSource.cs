using UIKit;
using Foundation;
using System;
using System.Reactive.Subjects;
using CoreGraphics;
using CodeHub.iOS.DialogElements;
using System.Reactive.Linq;

namespace CodeHub.iOS.TableViewSources
{
    public class DialogTableViewSource : UITableViewSource
    {
        private readonly RootElement _root;
        private readonly Subject<CGPoint> _scrolledSubject = new Subject<CGPoint>();
        private readonly Subject<Element> _selectedSubject = new Subject<Element>();

        public IObservable<CGPoint> ScrolledObservable { get { return _scrolledSubject.AsObservable(); } }

        public IObservable<Element> SelectedObservable { get { return _selectedSubject.AsObservable(); } }

        public RootElement Root
        {
            get { return _root; }
        }

        ~DialogTableViewSource()
        {
            Console.WriteLine("Goodbye DialogTableViewSource");
        }

        public DialogTableViewSource(UITableView container)
        {
            container.RowHeight = UITableView.AutomaticDimension;
            _root = new RootElement(container);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Root[(int)section].Count;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Root.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return Root[(int)section].Header;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            return Root[(int)section].Footer;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];
            var cell = element.GetCell(tableView);
            cell.Hidden = element.Hidden;
            return cell;
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];
            element.Deselected(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];
            element.Selected(tableView, indexPath);
            _selectedSubject.OnNext(element);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint sectionIdx)
        {
            var section = Root[(int)sectionIdx];
            return section.HeaderView;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint sectionIdx)
        {
            var section = Root[(int)sectionIdx];
            return section.HeaderView == null ? -1 : section.HeaderView.Frame.Height;
        }

        public override UIView GetViewForFooter(UITableView tableView, nint sectionIdx)
        {
            var section = Root[(int)sectionIdx];
            return section.FooterView;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint sectionIdx)
        {
            var section = Root[(int)sectionIdx];
            return section.FooterView == null ? -1 : section.FooterView.Frame.Height;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _scrolledSubject.OnNext(Root.TableView.ContentOffset);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];

            if (element.Hidden)
                return 0f;

            var sizable = element as IElementSizing;
            return sizable == null ? tableView.RowHeight : sizable.GetHeight(tableView, indexPath);
        }
    }
}
   