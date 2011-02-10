using System;
using WindowlessControls;
using System.Drawing;
using System.Windows.Forms;

namespace TrainStationAdvisor.ClassLibrary
{
    public class StationPresenter : StackPanel, IInteractiveContentPresenter
    {
        // selection state rectangle
        WindowlessRectangle m_Rectangle = new WindowlessRectangle();
        // contact picture
        WindowlessImage m_Image = new WindowlessImage();

        // obvious properties
        WindowlessLabel m_StationName = new WindowlessLabel(string.Empty, WindowlessLabel.DefaultBoldFont);
        WindowlessLabel m_StationNumber = new WindowlessLabel();
        WindowlessLabel m_StationAddress = new WindowlessLabel();
        WindowlessLabel m_Distance = new WindowlessLabel(string.Empty, WindowlessLabel.DefaultBoldFont);
        // this panel will house the email and address, and only be visible when focused
        StackPanel m_ExtendedInfo = new StackPanel();

        public StationPresenter()
        {
            OverlayPanel overlay = new OverlayPanel();

            // stretch to fit
            overlay.HorizontalAlignment = WindowlessControls.HorizontalAlignment.Stretch;
            overlay.VerticalAlignment = VerticalAlignment.Stretch;

            // dock the picture to the right and the info to the left
            DockPanel dock = new DockPanel();
            StackPanel left = new StackPanel();
            left.Layout = new DockLayout(new LayoutMeasurement(0, LayoutUnit.Star), DockStyle.Left);
            //m_Image.Layout = new DockLayout(new LayoutMeasurement(0, LayoutUnit.Star), DockStyle.Right);
            //m_Image.MaxHeight = 100;
            //m_Image.MaxWidth = 100;

            //dock.Controls.Add(m_Image);

            m_Distance.Layout = new DockLayout(new LayoutMeasurement(0, LayoutUnit.Star), DockStyle.Right);
            m_Distance.MaxHeight = 100;
            m_Distance.MaxWidth = 100;

            dock.Controls.Add(m_Distance);

            dock.Controls.Add(left);
            // 5 pixel border around the item contents
            dock.Margin = new Thickness(5, 5, 5, 5);

            // make the overlay fit the dock, so as to limit the size of the selection rectangle
            overlay.FitWidthControl = overlay.FitHeightControl = dock;

            // set up the rectangle color and make it fill the region
            m_Rectangle.Color = SystemColors.Highlight;
            m_Rectangle.HorizontalAlignment = WindowlessControls.HorizontalAlignment.Stretch;
            m_Rectangle.VerticalAlignment = VerticalAlignment.Stretch;
            // the rectangle does not paint by default
            m_Rectangle.PaintSelf = false;

            // add the extended info that is only visible when focused
            StackPanel nameAndNumber = new StackPanel();
            nameAndNumber.Controls.Add(m_StationName);
            nameAndNumber.Controls.Add(m_StationNumber);
            m_ExtendedInfo.Visible = false;
            m_ExtendedInfo.Controls.Add(m_StationAddress);

            // set up the left side
            left.Controls.Add(nameAndNumber);
            left.Controls.Add(m_ExtendedInfo);

            // add the foreground and the background selection
            overlay.Controls.Add(m_Rectangle);
            overlay.Controls.Add(dock);

            // add the item
            Controls.Add(overlay);

            // this is the bottom border
            WindowlessRectangle bottomBorder = new WindowlessRectangle(Int32.MaxValue, 1, Color.Gray);
            Controls.Add(bottomBorder);
        }

        #region IInteractiveStyleControl Members

        public void ApplyFocusedStyle()
        {
            // make the rectangle paint to denote that it is focused
            m_Rectangle.PaintSelf = true;
            m_ExtendedInfo.Visible = true;
        }

        public void ApplyClickedStyle()
        {
        }

        #endregion

        #region IContentPresenter Members
        Station m_Station;
        public object Content
        {
            get
            {
                return m_Station;
            }
            set
            {
                // populate the various UI elements with the contact
                m_Station = value as Station;
                RefreshContent();
            }
        }

        public void RefreshContent()
        {
            //m_Station.Bitmap = new StandardBitmap(new Bitmap(myContact.Picture));

            m_StationName.Text = m_Station.Name;

            DistanceTypes _DistanceType = m_Station.DistanceType;
            string _DistanceFormat;

            if (_DistanceType == DistanceTypes.Meters)
                _DistanceFormat = "{0} mtrs";
            else
                _DistanceFormat = "{0:0.00} km";

            m_Distance.Text = string.Format(_DistanceFormat,
                (_DistanceType == DistanceTypes.Meters ? Convert.ToInt32(m_Station.DistanceToDestination) : m_Station.DistanceToDestination));

            Color _Color;
            if (m_Station.AlertDistance > 0 && m_Station.DistanceToDestination < m_Station.AlertDistance)
            {
                _Color = (m_Station.PerformAlert ? Color.Red : Color.Blue);

                if (null != ParentControl)
                {
                    (ParentControl as ItemsControl).CurrentSelection = m_Station;
                }
            }
            else
                _Color = Color.Green;

            m_Distance.ForeColor = _Color;

            m_StationNumber.Text = m_Station.StationIndex.ToString();

            m_StationAddress.Text = string.Format("Lat:{0}-Long:{1}", m_Station.Latitude, m_Station.Longitude);
        }

        public WindowlessControlHost ParentControl { get; set; }

        #endregion
    }
}
